using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class CycleFinder
    {
        const int MaxSpan = 30;
        Enigma theMachine;
        List<string> encodingMaps = new List<string>();
        int encodingFirstStepCount = 0;

        public CycleFinder(Enigma mc)
        {
            theMachine = mc;
            for (int i = 0; i < MaxSpan; i++)
            {
                theMachine.AdvanceRotors();
                string s = theMachine.EncodingMap();
                encodingMaps.Add(s);
                Console.WriteLine($"{i.ToString("d2")}: {s}");

            }

        }

        //public void FindCycles
    }
}
