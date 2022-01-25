using System.Diagnostics;
using System.Text;

namespace Utils
{

    public class LetterPairAttacker
    {
        const int Ncubed = 26 * 26 * 26;

        Enigma theMachine;

        public LetterPairAttacker(Enigma theMachine)
        {
            this.theMachine = theMachine;
        }

        public static List<string> FindAllClickIndexes(string cipher, string plain)
        {
            int n = plain.Length;
            Debug.Assert(cipher.Length == n);
            Dictionary<string, List<int>> collector = new Dictionary<string, List<int>>();
            char[] buf = new char[2];
            for (int i=0; i < n; i++)
            {
                if (cipher[i] < plain[i])
                {
                    buf[0] = cipher[i];
                    buf[1] = plain[i];
                }
                else
                {
                    buf[0] = plain[i];
                    buf[1] = cipher[i];
                }
                string key = new string(buf);
                if (collector.Keys.Contains(key))
                {
                    collector[key].Add(i);
                }
                else
                {
                    collector[key] = new List<int>() { i };
                }
            }
            // The clicks are any dictionary entries with more than one index
            List<string> result = new List<string>();
            foreach (string key in collector.Keys)
            {
                if (collector[key].Count > 1)
                {
                    string theIndexSet = "";
                    foreach (int pos in collector[key])
                    {
                        theIndexSet += (char)(pos + 'A');
                    }
                    result.Add(theIndexSet);
                }

            }
            return result;
        }

   
        BoundClickSet clickSetValid(int posBeingTested, int[] indexes, byte srcPad)
        {
            // Put the voltage on srcPad, in Bombe-speak. Determine in this position of
            // the cores is valid for the clickset.  Allow the destPad to be a wildcard.
     
            int actualIndex = (posBeingTested + indexes[0]) % Ncubed;
            char expected = theMachine[actualIndex][srcPad];

            for (int i = 1; i < indexes.Length; i++)
            {
                actualIndex = (posBeingTested + indexes[i]) % Ncubed;
                if (expected != theMachine[actualIndex][srcPad])
                {
                    return null;
                }
            }
            BoundClickSet result = new BoundClickSet(srcPad, expected, indexes);
            return result;
        }

        List<BoundClickSet> clickSetValidForAnySrc(int pos, int[] indexes)
        {
            List<BoundClickSet> hits = new List<BoundClickSet>();
            for (byte srcPad = 0; srcPad < 26; srcPad++)
            {
                BoundClickSet boundResult = clickSetValid(pos, indexes, srcPad);
                if (boundResult != null)
                {
                    hits.Add(boundResult);
                }
            }
            return hits;
        }

        public List<ClickSetsHit> HuntForWheelsAndBindings(List<string> clickSets)
        {
            // Comvert each clickset into an array of indexes
            List<int[]> theIndexes = new List<int[]>();
            foreach (string s in clickSets)
            {
                theIndexes.Add(Enigma.GetIndexesFrom(s));
            }

            string mcDescription = theMachine.FullDescription;
            //$"{eCore.mc.Reflect.Name} {eCore.mc.Rotors[0].RotorName} {eCore.mc.Rotors[1].RotorName} {eCore.mc.Rotors[2].RotorName}";

            List<ClickSetsHit> hits = new List<ClickSetsHit>();

            for (int pos = 0; pos < Ncubed; pos++)
            {
                List<BoundClickSet> boundClickSets = new List<BoundClickSet>();
                bool b = true;
                foreach (int[] indexes in theIndexes)
                {
                   
                    List<BoundClickSet> boundResults = clickSetValidForAnySrc(pos, indexes);
                    b = b && (boundResults.Count > 0);
                    if (!b) break;
                    boundClickSets.AddRange(boundResults);
                }
                if (b)
                {
                    hits.Add(new ClickSetsHit(mcDescription, boundClickSets, pos));
                }

            }
            return hits;
        }


    }

    public class BoundClickSet
    {
        public byte srcPad;
        public char dstPad;
        public int[] indexes;

        public BoundClickSet(char src, char expect, string indxs)
        {
            srcPad = (byte)(src - 'A');
            dstPad = expect;
            indexes = new int[indxs.Length];
            for (int i = 0; i < indxs.Length; i++)
            {
                indexes[i] = indxs[i] - 'A';
            }
        }

        public BoundClickSet(byte src, char expect, int[] indxs)
        {
            srcPad = src;
            dstPad = expect;
            indexes = indxs;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (int n in indexes)
            {
                sb.Append((char)('A' + n));
            }

            return $"({(char)('A' + srcPad)}->{dstPad}:{sb.ToString()})";
        }
    }

    public class ClickSetsHit
    {
        public string Rotors { get; set; }
        public List<BoundClickSet> Probes { get; set; }
        public int hitIndex;

        public ClickSetsHit(string rots, List<BoundClickSet> ps, int hit)
        {
            Rotors = rots;
            Probes = ps;
            hitIndex = hit;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("ClickSets ");
            foreach (BoundClickSet click in Probes)
            {
                sb.Append(click);
                sb.Append(' ');
            }
            sb.Append($" {Rotors} at ");
            sb.Append(Enigma.IndexToView(hitIndex));
            sb.Append(' ');
            return sb.ToString();
        }
    }}