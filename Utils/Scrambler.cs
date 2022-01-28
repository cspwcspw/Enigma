using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class Scrambler
    {
        const int mapSz = 8 * 8 * 8;
        public int Index { get; set; }  // an integer representing the scrambler's present position
        public int StepOffsetInMenu { get; private set; }

        public Scrambler(int stepOffset)
        {
            StepOffsetInMenu = stepOffset;
        }

        // The Core in-out wiring map is exposed via indexing
        public string this[int index]
        {
            get
            {
                return theMap[index % mapSz];
            }
        }

        public string EncryptText(string plainText)
        {
            StringBuilder sb = new StringBuilder();
            foreach(char c in plainText)
            {
                int k = c - 'A';
                sb.Append(this[Index][k]);
                Index = (Index+1) % 512;  // OK, so this machine steps after the encoding, not before.
            }


            return sb.ToString();
        }

        public static string ToWindowView(int indx)
        {
            char[] buf = new char[3];
            buf[2] = (char)('A' + indx % 8);
            indx = indx / 8;
            buf[1] = (char)('A' + indx % 8);
            indx = indx / 8;
            buf[0] = (char)('A' + indx % 8);
            return new string(buf);
        }

        public static int FromWindowView(string s)
        {
            return (s[0] - 'A') * 8 * 8 + (s[1] - 'A') * 8 + (s[2] - 'A');
        }

        //static Scrambler()   // Once-off code generated the all-time map
        //{
        //    theMap = new string[mapSz];
        //    // Ensure we always get the same shared map for all scramber instances, and all runs.
        //    // We'll have some test data, cribs, and menus that we'll want to use permanently.
        //    Random rnd = new Random(42); 
        //    for (int i = 0; i < mapSz; i++)
        //    {
        //        // A valid mapping at any position of the rotors cannot map any letter to itself,
        //        // and it must honour enigma symmetry, e.g. E->A implies A->E 

        //        // Start with all the pins and pads available for potential cross-wiring
        //        List<int> pinsAvailable = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 };
        //        List<int> padsAvailable = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 };
        //        // These spaces tell us this pin position is unsoldered right now.
        //        char[] rowWiring = new char[8] { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' };
        //        while (padsAvailable.Count > 0)
        //        {
        //            int pad = padsAvailable[0];  // always pick the next pad
        //            int pin = pad;
        //            do
        //            {
        //                pin = pinsAvailable[rnd.Next(pinsAvailable.Count)];  // Choose a random available pin
        //            }
        //            while (pin == pad); // but don'e allow self-mapping || rowWiring[pin] != ' ');
        //            pinsAvailable.Remove(pin); // so they cannot be used again
        //            pinsAvailable.Remove(pad);
        //            padsAvailable.Remove(pin);
        //            padsAvailable.Remove(pad);
        //            // Now map pad->pin and pin->pad
        //            rowWiring[pad] = (char)('A' + pin);
        //            rowWiring[pin] = (char)('A' + pad);
        //        }
        //        theMap[i] = new string(rowWiring);
        //    }
        //}

        static private string[] theMap = {
"FEDCBAHG", "ECBHAGFD", "CEAFBDHG", "GEDCBHAF", "GEFHBCAD", "FEDCBAHG", "GEHFBDAC", "DGFAHCBE",
"CFAGHBDE", "DGFAHCBE", "DGEACHBF", "DGFAHCBE", "HDEBCGFA", "CEAHBGFD", "DCBAHGFE", "FHGEDACB",
"BAEFCDHG", "BADCGHEF", "HFEGCBDA", "CFAHGBED", "CHAEDGFB", "DHEACGFB", "GHFEDCAB", "GCBFHDAE",
"FCBEDAHG", "HFGEDBCA", "EGFHACBD", "DEHABGFC", "HFGEDBCA", "DFEACBHG", "HGDCFEBA", "EHGFADCB",
"CEAGBHDF", "BAHFGDEC", "FDHBGAEC", "GHFEDCAB", "HEFGBCDA", "FGDCHABE", "FCBGHADE", "ECBHAGFD",
"DEFABCHG", "FHDCGAEB", "CDABHGFE", "EDHBAGFC", "CHAGFEDB", "CFAHGBED", "FHDCGAEB", "GDFBHCAE",
"EDFBACHG", "FEDCBAHG", "BAHEDGFC", "CFAEDBHG", "HCBFGDEA", "FHDCGAEB", "DEHABGFC", "EGDCAHBF",
"HFDCGBEA", "GFDCHBAE", "GDHBFEAC", "GDEBCHAF", "DHEACGFB", "DCBAHGFE", "HEGFBDCA", "FEHGBADC",
"CHAFGDEB", "EDFBACHG", "BAHGFEDC", "DFHAGBEC", "FEHGBADC", "FHEGCADB", "CEAHBGFD", "GHFEDCAB",
"GHFEDCAB", "HFGEDBCA", "BADCHGFE", "HGFEDCBA", "DHGAFECB", "HEFGBCDA", "BAEFCDHG", "HFDCGBEA",
"DFGAHBCE", "HGFEDCBA", "GFDCHBAE", "GHDCFEAB", "BAFGHCDE", "DGEACHBF", "CEAGBHDF", "GDHBFEAC",
"CDABFEHG", "BAHEDGFC", "EDHBAGFC", "DCBAGHEF", "CGAEDHBF", "DHFAGCEB", "CFAEDBHG", "CHAEDGFB",
"BAFGHCDE", "GEHFBDAC", "HCBEDGFA", "FDGBHACE", "HEDCBGFA", "HCBFGDEA", "GDHBFEAC", "ECBHAGFD",
"GHFEDCAB", "DHEACGFB", "FDGBHACE", "CEAFBDHG", "HCBFGDEA", "GCBEDHAF", "CGAHFEBD", "GEFHBCAD",
"ECBGAHDF", "BAEFCDHG", "FEHGBADC", "FCBGHADE", "FHEGCADB", "BAFHGCED", "HFDCGBEA", "DHGAFECB",
"HGEFCDBA", "BAEFCDHG", "GHEFCDAB", "GFHEDBAC", "DEGABHCF", "CGAFHDBE", "HEFGBCDA", "HCBEDGFA",
"DGFAHCBE", "CEAFBDHG", "EHGFADCB", "GFEHCBAD", "BAHGFEDC", "CHAEDGFB", "BADCGHEF", "FEGHBACD",
"CEAHBGFD", "FCBHGAED", "EFHGABDC", "EDGBAHCF", "CDABFEHG", "CEAFBDHG", "GEDCBHAF", "CDABFEHG",
"BAHGFEDC", "FCBEDAHG", "BAEHCGFD", "EFHGABDC", "FGEHCABD", "BAFEDCHG", "HEDCBGFA", "FHGEDACB",
"BAGHFECD", "DGFAHCBE", "BAHEDGFC", "DHGAFECB", "BAFGHCDE", "DFEACBHG", "GCBEDHAF", "HCBEDGFA",
"CFAHGBED", "FEHGBADC", "HCBFGDEA", "GDEBCHAF", "HDFBGCEA", "EHDCAGFB", "FDEBCAHG", "HDEBCGFA",
"BAHGFEDC", "CFAHGBED", "FGHEDABC", "GHEFCDAB", "GDEBCHAF", "HEFGBCDA", "GHFEDCAB", "FDHBGAEC",
"HCBEDGFA", "HEDCBGFA", "BAGHFECD", "BAGHFECD", "EDFBACHG", "ECBGAHDF", "GFDCHBAE", "FHGEDACB",
"GEFHBCAD", "HDFBGCEA", "BAGEDHCF", "BAEHCGFD", "FEHGBADC", "GEDCBHAF", "CFAGHBDE", "HFGEDBCA",
"EGDCAHBF", "DHGAFECB", "ECBFADHG", "HDGBFECA", "CHAGFEDB", "EFHGABDC", "EFHGABDC", "HCBEDGFA",
"BAHGFEDC", "DCBAGHEF", "BAHGFEDC", "GCBHFEAD", "CDABHGFE", "DEHABGFC", "CEAGBHDF", "CEAHBGFD",
"EDFBACHG", "GCBEDHAF", "BAEHCGFD", "FCBGHADE", "EDHBAGFC", "DHEACGFB", "CHAGFEDB", "HFEGCBDA",
"DCBAHGFE", "HCBEDGFA", "GFEHCBAD", "DCBAFEHG", "DGHAFEBC", "DHFAGCEB", "EGHFADBC", "CFAEDBHG",
"BAEHCGFD", "GEFHBCAD", "BAEHCGFD", "DFEACBHG", "FGHEDABC", "BAGFHDCE", "BAFHGCED", "HGEFCDBA",
"GEFHBCAD", "GFHEDBAC", "HEGFBDCA", "BAEFCDHG", "CGAEDHBF", "CFAEDBHG", "FGEHCABD", "HDGBFECA",
"BADCGHEF", "EDHBAGFC", "CGAFHDBE", "BAFHGCED", "BAEFCDHG", "CHAFGDEB", "EFGHABCD", "GCBHFEAD",
"CHAGFEDB", "CDABFEHG", "BADCGHEF", "GDFBHCAE", "CEAGBHDF", "CEAFBDHG", "BADCGHEF", "DHEACGFB",
"DGEACHBF", "BAHEDGFC", "FCBHGAED", "BAHFGDEC", "BAGEDHCF", "EHFGACDB", "BADCFEHG", "BAHFGDEC",
"HCBGFEDA", "HCBEDGFA", "FCBHGAED", "GCBFHDAE", "ECBFADHG", "FGDCHABE", "EGDCAHBF", "CDABFEHG",
"CDABFEHG", "DCBAHGFE", "GCBFHDAE", "BAGFHDCE", "GEFHBCAD", "EHDCAGFB", "GCBHFEAD", "GFHEDBAC",
"GEHFBDAC", "DFGAHBCE", "FGHEDABC", "BADCGHEF", "HGFEDCBA", "HCBGFEDA", "EGFHACBD", "FGHEDABC",
"ECBHAGFD", "EHFGACDB", "FGDCHABE", "BADCGHEF", "DHEACGFB", "HDFBGCEA", "EFGHABCD", "FEDCBAHG",
"GDFBHCAE", "CHAFGDEB", "EGFHACBD", "GFDCHBAE", "EFDCABHG", "GFHEDBAC", "BAGFHDCE", "GFDCHBAE",
"GFEHCBAD", "GDEBCHAF", "HDEBCGFA", "DFGAHBCE", "GDFBHCAE", "HFEGCBDA", "EDFBACHG", "CGAHFEBD",
"DCBAHGFE", "BAFEDCHG", "BAEGCHDF", "EFGHABCD", "CEAFBDHG", "GHEFCDAB", "DGFAHCBE", "DCBAGHEF",
"EFHGABDC", "DFHAGBEC", "BAGEDHCF", "DFHAGBEC", "FEHGBADC", "DHFAGCEB", "GFEHCBAD", "HDGBFECA",
"HCBGFEDA", "ECBGAHDF", "HGDCFEBA", "EHGFADCB", "ECBGAHDF", "ECBGAHDF", "FCBHGAED", "HGDCFEBA",
"HCBGFEDA", "FEDCBAHG", "FGHEDABC", "CFAEDBHG", "DGHAFEBC", "GEHFBDAC", "FDEBCAHG", "CFAHGBED",
"CGAFHDBE", "BADCHGFE", "HFGEDBCA", "GHEFCDAB", "EGDCAHBF", "EGHFADBC", "BAHEDGFC", "CHAEDGFB",
"ECBGAHDF", "DFHAGBEC", "EGHFADBC", "FGHEDABC", "GHFEDCAB", "GHDCFEAB", "BAEHCGFD", "HEDCBGFA",
"DHEACGFB", "GCBFHDAE", "FDHBGAEC", "BAHFGDEC", "GDFBHCAE", "HFGEDBCA", "BADCGHEF", "FDHBGAEC",
"FEGHBACD", "HFEGCBDA", "GDFBHCAE", "BAEFCDHG", "DCBAFEHG", "FEDCBAHG", "BAFHGCED", "HFEGCBDA",
"DEFABCHG", "EGDCAHBF", "GDHBFEAC", "GHEFCDAB", "BAFGHCDE", "FGHEDABC", "GHFEDCAB", "BADCGHEF",
"HEDCBGFA", "HFGEDBCA", "CEAGBHDF", "CEAHBGFD", "BAGEDHCF", "HCBGFEDA", "FDGBHACE", "FEHGBADC",
"EDGBAHCF", "EHFGACDB", "DFEACBHG", "FEDCBAHG", "HEDCBGFA", "FGEHCABD", "BAGHFECD", "BAGFHDCE",
"HDGBFECA", "FCBHGAED", "DHFAGCEB", "BAEHCGFD", "EFGHABCD", "HCBEDGFA", "HDEBCGFA", "FHDCGAEB",
"DEGABHCF", "EFDCABHG", "CGAFHDBE", "BAGEDHCF", "HCBGFEDA", "CDABHGFE", "BAGHFECD", "HCBFGDEA",
"DFHAGBEC", "HDFBGCEA", "GFHEDBAC", "BADCGHEF", "DHFAGCEB", "EGFHACBD", "CGAFHDBE", "HEGFBDCA",
"FEDCBAHG", "GHDCFEAB", "DFEACBHG", "CGAEDHBF", "CFAGHBDE", "HFDCGBEA", "GHDCFEAB", "HGFEDCBA",
"HDFBGCEA", "FDEBCAHG", "EFGHABCD", "BAGEDHCF", "CFAEDBHG", "DEFABCHG", "BADCGHEF", "EGFHACBD",
"GFDCHBAE", "BAFHGCED", "BAFHGCED", "CEAFBDHG", "FGDCHABE", "BADCFEHG", "BAHGFEDC", "EGHFADBC",
"BADCGHEF", "BADCGHEF", "GDFBHCAE", "HGDCFEBA", "GEDCBHAF", "ECBGAHDF", "FCBEDAHG", "HDFBGCEA",
"EFHGABDC", "BAFGHCDE", "DHGAFECB", "EDGBAHCF", "FDGBHACE", "BAEHCGFD", "BAHFGDEC", "FHGEDACB",
"GDHBFEAC", "GHDCFEAB", "GDEBCHAF", "GFHEDBAC", "FHGEDACB", "BADCFEHG", "FCBGHADE", "FEDCBAHG",
"EFHGABDC", "DEFABCHG", "HFEGCBDA", "EHGFADCB", "GDHBFEAC", "CGAEDHBF", "DGHAFEBC", "EDHBAGFC",
"BAFHGCED", "EDHBAGFC", "ECBGAHDF", "EFGHABCD", "BAFHGCED", "GCBEDHAF", "HFEGCBDA", "CEAFBDHG",
"HEFGBCDA", "CFAHGBED", "BAEHCGFD", "CEAHBGFD", "CFAEDBHG", "EFDCABHG", "FGEHCABD", "FEHGBADC",
"BAGFHDCE", "CFAEDBHG", "EFGHABCD", "DHGAFECB", "CGAFHDBE", "DFEACBHG", "EDGBAHCF", "BADCGHEF",
"GCBFHDAE", "FGDCHABE", "DFGAHBCE", "HFGEDBCA", "GDFBHCAE", "CGAHFEBD", "HGEFCDBA", "HEFGBCDA",
            };

    }

}
