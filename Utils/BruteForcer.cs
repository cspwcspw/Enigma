namespace Utils
{
    public class BruteForcer
    {

        public Enigma theMachine { get; set; }
        public BruteForcer(Enigma mc)
        {
            theMachine = mc;
        }

        public void TryAllInitialSettings(string cipher, string expected)
        {
            for (char c1 = 'A'; c1 <= 'Z'; c1++)
            {
             //   Console.Write($"{c1}");
                for (char c2 = 'A'; c2 <= 'Z';c2++)
                {
                    for (char c3 = 'A';c3 <= 'Z';c3++)
                    {
                        theMachine.ChangeRotorsToShow(c1, c2, c3);
                        string s = theMachine.EncodeText(cipher);
                        if (s == expected)
                        {
                            Console.Write($" (Matched {theMachine.Rotors[0]} {theMachine.Rotors[2]} {theMachine.Rotors[2]} {c1}{c2}{c3}) ");
                        }
                    }
                }
            }
           // Console.WriteLine();
        }

        public void TryAllRotorSelections(List<Rotor> rs, string cipher, string expected)
        {
            var perms = Helpers.Permutations(rs);
            foreach (var p in perms)
            {
            //    Console.Write($"{p[0]};{p[1]};{p[2]} ");
                theMachine.ReplaceRotor(0, p[0]);
                theMachine.ReplaceRotor(1, p[1]);
                theMachine.ReplaceRotor(2, p[2]);
                TryAllInitialSettings(cipher, expected);
            }
        //   Console.WriteLine();
        }

        public void TryAllRotorsAndRings(List<Rotor> rs, string cipher, string expected)
        {
            for (int r0 = 0; r0 < 26; r0++)
            {
                rs[0].SetRing0(r0);
                for (int r1 = 0; r1 < 26; r1++)
                {
                    rs[1].SetRing0(r1);
                    rs[2].SetRing0(0);
                    Console.Write($"{rs[0]};{rs[1]};{rs[2]} ");
                    for (int r2 = 0; r2 < 26; r2++)
                    {
                        rs[2].SetRing0(r2);
                        Console.Write((char)('A' + r2));
                        TryAllRotorSelections(rs, cipher, expected);              
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}
