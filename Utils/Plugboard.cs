using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class Plugboard
    {
        public string Map;
        public Plugboard()
        {
            Map = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";  // Effect when all wires are unplugged
        }

        public void SetWirings(string exchangePairs)  // example usage: SetWirings("AB CD PQ")
        {
            char[] table = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

            string[] pairs = exchangePairs.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);

            foreach (string pair in pairs)
            {
                string t = pair.Trim();
                char X = t[0];
                char Y = t[1];
                table[X - 'A'] = Y;
                table[Y - 'A'] = X;
            }
            Map = new string(table);
        }
    
        public char MapChar(char letterIn)
        {
            return Map[letterIn - 'A'];
        }
    }
}
