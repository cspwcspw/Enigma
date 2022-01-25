using System.Diagnostics;

namespace Utils
{

    public class Rotor
    {
        public string RotorName { get; set; }

        // https://www.cryptomuseum.com/crypto/enigma/i/img/300002/056/full.jpg

        // The enigma was 1-based counting on the rings.  But my rotors are zero-based.
        // The Enigma machine that holds the rotors is responsible for mapping the 
        // 0-based internals to 01 or 'A'. 

        // The pins, pads, and cross-connect wiring are all part of the inner
        // hub of the rotor, and never change relative to each other.  
        // The wiring cross-connect can be rotated inside the alphabet ring (which also
        // contains the turnover notch) by the ringsetting.

        private string WiringKeyAtA;  
        // Wiring from web sites is specified on assumption RingSetting == 'A' or 0 here. 
        // This is only used when we build the jump tables for pinToPad and PadToPin, and we 
        // rebuild those tables whenever the ringsetting changes. 

        public int RingSetting0 { get; private set; }


        // The wiringKeyAtA is used to pre-compute the two delta jump tables. 
        // Here we look at the wheel from the Right (Pin) side, and always
        // count and encode jumps as a positive number of steps in the 
        // clockwise (forward in the alphabet) direction.
        // For example standard rotor III has substitions
        // "BDFHJC ...." which will result in a jump-forward-in-the-alphabet 
        // pin-to-pad table of (1(e.g. A->B), 2, 3, 4, 5(E->J), 22(F->L), ...}

        public int[] pinToPadDeltas { get; private set; }

        // Mappings on the return electrical path (Pad to Pin) are also encoded
        // as jumps forward in the alpabet (counter-clockwise jump distances) when
        // we are looking from the left side of the machine at the pads. 
        public int[] padToPinDeltas { get; private set; }


        /// <summary>
        ///  The position of a rotor on the axle at any given point in time is not really an attribute of the rotor: it
        ///  is something that the machine determines.  But it more convenient to store and use it here and manipulate it
        ///  from the machine when it needs changing.  Machine positions, like pads are numbered from 0.  In this simulator
        ///  we don't care about physical constraints, so the rotor position at the window is the reference point.
        /// </summary>
        public int PosAtWin0 { get; set; } = 0;

        // From https://www.cryptomuseum.com/crypto/enigma/working.htm
        // Each wheel has a ring that can be used to rotate the wiring independantly of the index.
        // This can be regarded as creating an offset in the opposite direction.The wheel-turnover
        // notches are fixed to the index/alphabet ring.Therefore the turnover of the next wheel, will always
        // happen at the same letter in the window, but the wiring might be rotated.


        /// <summary>
        /// The notch and alphabet or numbers are fixed / engraved on the left plate of the rotor.  
        /// https://www.cryptomuseum.com/crypto/enigma/wiring.htm 
        /// </summary>
        public int TurnoverPosition0 { get; set; }


        public bool IsAtNotch
        {
            get { return PosAtWin0 == TurnoverPosition0; }
        }

        public Rotor(string name, string wiringKey, char turnover)
        {
            Debug.Assert(wiringKey.Length == 26);
            RotorName = name;
            WiringKeyAtA = wiringKey;
            TurnoverPosition0 = Enigma.letterToNum0(turnover);

            pinToPadDeltas = new int[26];
            padToPinDeltas = new int[26];
            SetRing0(0);  // to build the jump tables
        }


        // Force small negative or positive numbers back into range 0..25
        static int mod26(int n)
        {
            return (n + 260) % 26;
        }


        void showDeltaTables() // debugging only
        {
            Console.Write($"{RotorName} PinToPad ring={RingSetting0}, deltas= ");
            for (int i = 0; i < 26; i++)
            {
                Console.Write($"{pinToPadDeltas[i].ToString("D2")} ");
            }
            Console.WriteLine();

            Console.Write($"{RotorName} PadToPin ring={RingSetting0}, deltas= ");
            for (int i = 0; i < 26; i++)
            {
                Console.Write($"{padToPinDeltas[i].ToString("D2")} ");
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        /// <summary>
        ///  zero-based setting, 0==A
        /// </summary>
        /// <param name="ringSetting"></param>
        public void SetRing0(int ringSetting)
        {
            Debug.Assert(ringSetting >= 0 && ringSetting < 26);
            RingSetting0 = ringSetting;

            // Build and-populate delta tables, taking into account
            // the ring setting. Position 0 in the jump tables represents the 
            // wiring jump-forward offsets when letter 'A' is shown in the window.

            for (int i = 0; i < 26; i++)
            {
                int pin = mod26(i + ringSetting);
                int padOffset = mod26(Enigma.letterToNum0(WiringKeyAtA[i]) - i);
                pinToPadDeltas[pin] = padOffset;

                int pad = mod26(pin + padOffset);
                int pinOffset = mod26(-padOffset);
                padToPinDeltas[pad] = pinOffset;
            }

          //   showDeltaTables();
        }

        public int PinToPad(int pin)
        {
            int pad = mod26(pin + pinToPadDeltas[mod26(pin + PosAtWin0)]);
            return pad;
        }

        public int PadToPin(int pad)
        {
            int pin = mod26(pad + padToPinDeltas[mod26(pad + PosAtWin0)]);
            return pin;
        }


        public override string ToString()
        {
            return $"{RotorName}/{(char)('A'+RingSetting0)}";
        }

        // https://www.cryptomuseum.com/crypto/enigma/wiring.htm
        // Main ones in use by german army and air force, plus a few of my own for testing, etc.

        static public Rotor I { get { return new Rotor("I", "EKMFLGDQVZNTOWYHXUSPAIBRCJ", 'Q'); } }
        static public Rotor II { get { return new Rotor("II", "AJDKSIRUXBLHWTMCQGZNPYFVOE", 'E'); } }
        static public Rotor III { get { return new Rotor("III", "BDFHJLCPRTXVZNYEIWGAKMUSQO", 'V'); } }
        static public Rotor IV { get { return new Rotor("IV", "ESOVPZJAYQUIRHXLNFTGKDCMWB", 'J'); } }
        static public Rotor V { get { return new Rotor("V", "VZBRGITYUPSDNHLXAWMJQOFECK", 'Z'); } }

        // For understanding things, it is nice to have some hemebuild rotors and reflectors handy to 
        // simplify rather than complicate the mappings. 
        static public Rotor Identity { get { return new Rotor("Id", "ABCDEFGHIJKLMNOPQRSTUVWXYZ", 'Z'); } }
        static public Rotor RotorBADCF { get { return new Rotor("BADCF", "BADCFEHGJILKNMPORQTSVUXWZY", 'Z'); } }

        static public string[] KnownRotors = { "I", "II", "III", "IV", "V", "Id", "BADCF" };

        static public Rotor ByName(string name)
        {
            switch (name)
            {
                case "I": return I;
                case "II": return II;
                case "III": return III;
                case "IV": return IV;
                case "V": return V;
                case "Id": return Identity;
                case "BADCF": return RotorBADCF;
                default: return null;
            }
        } 
    }
}
