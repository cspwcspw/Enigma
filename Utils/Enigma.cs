using System.Diagnostics;

namespace Utils
{
    public class Enigma
    {

        public const int NCubed = 26 * 26 * 26;

        public Reflector Reflect { get; }  // TODO private?

        public Rotor[] Rotors { get; }
      
        public Plugboard PlugB { get; }   

        public string FullDescription
        {
            get
            {
                return $"{Reflect.Name} {Rotors[0].RotorName} {Rotors[1].RotorName} {Rotors[2].RotorName}";
            }
        }

        // When the machine is constructed or rotors are swapped we build a full cache of 
        // all input->output mappings at each core rotor position. (i.e. a Bombe Cache)
        // "Core position" means "how the wiring interconnects, without taking into account
        // any ring settings or plugBoard. This is what Bombe and letterPair attacks were based on.
        private string[] coreCacheForCurrentRotorSet = new string[NCubed];

        // The Core cache mapping is exposed via indexing
        public string this[int index]
        {
            get
            {
                return coreCacheForCurrentRotorSet[index % NCubed];
            }
        }


        public string VisibleInWindows
        {
            get
            {
                char[] cs = new char[3];
                cs[0] = (char)('A' + Rotors[0].PosAtWin0);
                cs[1] = (char)('A' + Rotors[1].PosAtWin0);
                cs[2] = (char)('A' + Rotors[2].PosAtWin0);
                return new string(cs);
            }
        }

        public int CoreIndex
        {
            get
            {
                return Rotors[0].PosAtWin0 * 26 * 26 + Rotors[1].PosAtWin0 * 26 + Rotors[2].PosAtWin0;
            }
        }

        // Rotor order slowest is first in the array
        public Enigma(Reflector r, Rotor r1, Rotor r2, Rotor r3, Plugboard pb)
        {
            Reflect = r;
            Rotors = new Rotor[3] { r1, r2, r3 };
            PlugB = pb;
            buildCoreCacheForCurrentRotorSet();
        }

        private void buildCoreCacheForCurrentRotorSet()
        {
            int pos = 0;
            char[] buf = new char[26];
            for (int s = 0; s < 26; s++)
            {
                Rotors[0].PosAtWin0 = s;
                for (int m = 0; m < 26; m++)
                {
                    Rotors[1].PosAtWin0 = m;
                    for (int f = 0; f < 26; f++)
                    {
                        Rotors[2].PosAtWin0 = f;

                        for (int pad = 0; pad < 26; pad++)
                        {
                            buf[pad] = (char)('A' + EncodeCore(pad));
                        }
                        coreCacheForCurrentRotorSet[pos] = new string(buf, 0, 26);
                        pos++;
                    }
                }
            }
        }

        public void ReplaceRotor(int posn, Rotor r)
        {
            switch (posn)
            {
                case 0: Rotors[0] = r; break;
                case 1: Rotors[1] = r; break;
                case 2: Rotors[2] = r; break;
            }
            Rotors[posn] = r;
            buildCoreCacheForCurrentRotorSet();
        }

        public static char AlphabetAdd(char letter, int delta)
        {
            // implement mod 26 circular ring for alphabet A-Z, upper-case letters only, abs(delta) <= 25
             char result = (char) (((letter - 'A') + delta + 26) % 26 + 'A');
            return result;
        }

        static public int letterToNum0(char letter)
        {
            return letter - 'A';
        }


        static public string ToUserView(int pos0)
        {
            return $"{(pos0 + 1).ToString("D2")}({(char)('A'+pos0)})";
        }

        public void SetRotorRing(int rotorNum, char letter)
        {
            Rotors[rotorNum].SetRing0(letterToNum0(letter));
        }
 
        public void ChangeRotorsToShow(string wanted)
        {
            for (int i = 0; i < 3; i++)
            {
                Rotors[i].PosAtWin0 = letterToNum0(wanted[i]);
            }
            string whatHappended = VisibleInWindows;       
            Debug.Assert(whatHappended == wanted);
        }

        public void ChangeRotorsToShow(int fingerPrint)
        {
            Rotors[2].PosAtWin0 = fingerPrint % 26 ;
            fingerPrint = fingerPrint / 26;
            Rotors[1].PosAtWin0 = fingerPrint % 26;
            fingerPrint = fingerPrint / 26;
            Rotors[0].PosAtWin0 = fingerPrint % 26; ;
        }

        public void ChangeRotorsToShow(char a, char b, char c)
        {
            Rotors[0].PosAtWin0 = letterToNum0(a);
            Rotors[1].PosAtWin0 = letterToNum0(b);
            Rotors[2].PosAtWin0 = letterToNum0(c);
        }

        public void AdvanceRotors()
        {
            int n = 0;
            if (Rotors[2].PosAtWin0 == Rotors[2].TurnoverPosition0)
            {
                n = 1;
            }
            if (Rotors[1].PosAtWin0 == Rotors[1].TurnoverPosition0)
            {
                n = n + 2;
            }
            Rotors[2].PosAtWin0 = (Rotors[2].PosAtWin0 + 1) % 26;
         //   Console.WriteLine($"Rotor advance n = {n}");
            switch (n)
            {
                case 0: break;
                case 1:
                    Rotors[1].PosAtWin0 = (Rotors[1].PosAtWin0 + 1) % 26;
                    break;

                case 2: 
                case 3:
                    Rotors[1].PosAtWin0 = (Rotors[1].PosAtWin0 + 1) % 26;
                    Rotors[0].PosAtWin0 = (Rotors[0].PosAtWin0 + 1) % 26;
                    break;
            }
        }


        public char EncodeNoStep(char inp)
        {
            char c1 = PlugB.MapChar(inp);
            // Assume the Entry Wheel had no crosswiring.   From here on we work with pad numbers 
            // in machine frame of reference, i.e. 0 is at the noon position. 
            int pad0 = c1 - 'A';

            int pad1 = Rotors[2].PinToPad(pad0);
            int pad2 = Rotors[1].PinToPad(pad1);
            int pad3 = Rotors[0].PinToPad(pad2);
            int pin4 = Reflect.Reflect(pad3);
            int pin5 = Rotors[0].PadToPin(pin4);
            int pin6 = Rotors[1].PadToPin(pin5);
            int pin7 = Rotors[2].PadToPin(pin6);
            char c2 = (char)('A' + pin7);
            char toLamp = PlugB.MapChar(c2);
            return toLamp;
        }

        public int EncodeCore(int pad0)
        {  
            int pad1 = Rotors[2].PinToPad(pad0);
            int pad2 = Rotors[1].PinToPad(pad1);
            int pad3 = Rotors[0].PinToPad(pad2);
            int pin4 = Reflect.Reflect(pad3);
            int pin5 = Rotors[0].PadToPin(pin4);
            int pin6 = Rotors[1].PadToPin(pin5);
            int pin7 = Rotors[2].PadToPin(pin6);
            return pin7;
        }

        public void ChangeRotorToLetter(int i, char v)
        {
            Rotors[i].PosAtWin0 = letterToNum0(v);         
        }

        public char Encode(char inp)
        {
            AdvanceRotors();
            return EncodeNoStep(inp);
        }

        public string EncodingMap()
        {
            char[] result = new char[26];
            for (int i = 0; i < 26; i++)
            {
                result[i] = EncodeNoStep((char)('A'+i));
            }
            string s = new string(result);        
            return new string(result);
        }

        static public string CleanText(string text1)
        {
            string s0 = text1.ToUpper();
            string s1 = s0.Replace(" ", "");
            char[] result = new char[s1.Length];
            for (int i = 0; i < s1.Length; i++)
            {
                char c = s1[i];
                if (!char.IsLetter(c))
                {
                    c = 'X';
                }
                result[i] = c;
            }
            return new string(result);
        }

        public string EncodeText(string text1)
        {
            string s0 = text1.ToUpper();
            string s1 = s0.Replace(" ", "");
            char[] result = new char[s1.Length];
            for (int i=0; i < s1.Length; i++)
            {  char c = s1[i];
               if (!char.IsLetter(c))
                {
                    c = 'X';
                }
                result[i] = Encode(c); 
            }
            return new string(result);
        }

        public static string[] PossibleRingSettings
        {
            get
            {
                string[] result = new string[26];
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = $"{(i + 1).ToString("D2")}({(char)('A' + i)})";
                }
                return result;
            }
        }

        public static int[] GetIndexesFrom(string s)
        {
            int[] result = new int[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                result[i] = s[i] - 'A';
            }
            return result;
        }

        public static int GetIndexFrom(string s)
        {
            int  result = 0;
            for (int i = 0; i < s.Length; i++)
            {
                result = result*26 + (s[i] - 'A');
            }
            return result;
        }

        public static string IndexToView(int v)
        {
            // Compensate for the off-by-one.  The enigma steps before it makes the circuit.
            // so my cache indexes are off by one.
            //  v = (v + Ncubed) % Ncubed;
            char[] result = new char[3];
            result[2] = (char)((v % 26) + 'A');
            v = v / 26;
            result[1] = (char)((v % 26) + 'A');
            v = v / 26;
            result[0] = (char)((v % 26) + 'A');
            return new string(result);
        }
    }
}
