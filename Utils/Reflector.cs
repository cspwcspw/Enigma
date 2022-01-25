using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class Reflector
    {
        public string Substitutions { get; private set; }

        private int[] subs = new int[26];  // parallel array of offsets rather than letters
        public string Name { get; set; }

        public Reflector(string name, string mapping)
        {
            Name = name;
            Substitutions = mapping.ToUpper();
            for (int i=0; i < 26; i++)
            {
                subs[i] = Substitutions[i] - 'A';
            }
        }

        public int Reflect(int pinIn)
        {
            int pinOut = subs[pinIn];
            return pinOut;
        }

        static public Reflector UKW_A = new Reflector("UKW-A", "EJMZALYXVBWFCRQUONTSPIKHGD");
        static public Reflector UKW_B = new Reflector("UKW-B", "YRUHQSLDPXNGOKMIEBFZCWVJAT");
        static public Reflector UKW_C = new Reflector("UKW-C", "FVPJIAOYEDRZXWGCTKUQSBNMHL");

        // A simple reflector that exchanges pairs of adjacent letters
        static public Reflector BACDF = new Reflector("BACDF", "BADCFEHGJILKNMPORQTSVUXWZY");
    }
}