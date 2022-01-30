
// There are some real tests here, but this is a more a "playground" than a test suite.
// A lot of the code here is throw-away, test the idea, straighten my head, etc. 
// 
// http://users.telenet.be/d.rijmenants/en/enigmasim.htm
// http://www.ellsbury.com/

using System.Diagnostics;
using System.Text;
using Utils;

// testEllsbury();

findInterestingCribsForMiniBombe();

void findInterestingCribsForMiniBombe()
{
    Scrambler s = new Scrambler(0, 3, 4);
    string fullPlainText = "BEACHHEAD";




    //  for (int leng = fullPlainText.Length; leng > 0; leng--)
    int leng = 5;
        {
        Dictionary<string, List<int>> hits = new Dictionary<string, List<int>>();
        string plainText = fullPlainText.Substring(0, leng);

        for (int i = 0; i < 512; i++)
        {
            s.Index = i;
            string cipher = s.EncryptText(plainText);
            if (hits.Keys.Contains(cipher))
            {
                hits[cipher].Add(i);
            }
            else
            {
                hits[cipher] = new List<int>() { i };
            }
        }
        Console.WriteLine($"trying {plainText} got {hits.Count} entries in the dict.");

        // at indexes 110 and 128 (close enough together) both BEACH -> GBEEC, so that is a nice test case for single stepping, etc.
        //        BEACHHEAD->GBEECECBC at BFG
        //        BEACHHEAD->GBEECBGFH at CAA  --- If we use this we'll get one false stop at BFG before success at CAA
        s.Index = 110;
        string result1 = s.EncryptText("BEACHHEAD");
        s.Index = 128;
        string result2 = s.EncryptText("BEACHHEAD");

        Console.WriteLine($"BEACHHEAD -> {result1} at {Scrambler.ToWindowView(110)}");
        Console.WriteLine($"BEACHHEAD -> {result2} at {Scrambler.ToWindowView(128)}");

        List<int> interesting = hits["AHEEB"];  // Has a cycle of length 4, no false stops
        foreach(var entry in hits)
        {
            if (entry.Value.Count > 1)
            { Console.Write($"{entry.Key} is found at offsets ");
                foreach (var val in entry.Value)
                {
                    Console.Write($"{val} ");
                }
                Console.WriteLine();
            }
        }
    }

    // Top 3 finds are at indexes 49(15 loops) 91 (15 loops and 101 (7 loops)
    // 049 BEACHHEAD->EDCBBCAHC  loops found = 15
    // 091 BEACHHEAD->CDDABEBHB  loops found = 15
    //  101 BEACHHEAD->CFEFBEBHE loops found = 7

    string crib = "BEACH";
    for (int i = 0; i < 512; i++)
    {
        s.Index = i;
        string cipher = s.EncryptText(crib);
        Graph g = new Graph(cipher, crib);
        LoopGatherer gt = g.findLoops();
        int n = gt.TheLoops.Count;
        //if (i == 49 || i == 91 || i == 101)
        //{
        if (n > 0)
        {
            Console.WriteLine($"{i.ToString("D3")} {crib} -> {cipher}  loops = {n}");
            foreach (var x in gt.TheLoops)
            {
                Console.WriteLine(x);
            }
        }
    }
}


    bool someTestsFailed = false;
 //TestletterPairAttack();

// BombeData();
//TuringOriginalData();
// testChoices();
//testBruteForcer();

//testLyndatKnownPlugs();

//testLyndatMarkworth();

void testEllsbury()
{
    // http://www.ellsbury.com/enigma3.htm
    Enigma mc = new Enigma(Reflector.UKW_B, Rotor.IV, Rotor.I, Rotor.V, new Plugboard());
    mc.Rotors[0].SetRing0(22); // mine are 0-based
    mc.Rotors[1].SetRing0(01); // mine are 0-based
    mc.Rotors[2].SetRing0(16); // mine are 0-based
    mc.PlugB.SetWirings("AR KT MW LC XD EJ ZB UY PS HN");
    mc.ChangeRotorsToShow("RNF");
    string ciphered = mc.EncodeText("JRM");

    Console.WriteLine($"JRM -> {ciphered}, was expencting BKT, but Enigma simulator also says BGO");

}

void BombeData()
{
    //https://www.101computing.net/turing-welchman-bombe/

    string crib = "WETTERVORHERSAGE";   
    string ciph = "SNMKGGSTZZUGARLV";

    List<string> clicks = LetterPairAttacker.FindAllClickIndexes(ciph, crib);

    Enigma theMachine = new Enigma(Reflector.UKW_B, Rotor.I, Rotor.II, Rotor.III, new Plugboard());

    LetterPairAttacker lpa = new LetterPairAttacker(theMachine);
    Console.WriteLine(crib);
    Console.WriteLine(ciph);

    List<ClickSetsHit> hits = lpa.HuntForWheelsAndBindings(clicks);
    showCandidates(hits);
}

void TuringOriginalData()
{
    //  https://www.codesandciphers.org.uk/virtualbp/tbombe/tbombe.htm
    Console.WriteLine("ABCDEFGHIJKLMNOPQRSTUVWXYZ");

    string crib = "SPRUCHNUMMERXEINS";  // Track or Color Number one
    string ciph = "JYCQRPWYDEMCJMRSR";

    List<string> clicks = LetterPairAttacker.FindAllClickIndexes(ciph, crib);

    Enigma theMachine = new Enigma(Reflector.UKW_B, Rotor.I, Rotor.II, Rotor.III, new Plugboard());

    LetterPairAttacker lpa = new LetterPairAttacker(theMachine);
    Console.WriteLine(crib);
    Console.WriteLine(ciph);

 //   List<string> clicks = new List<string>() { "CEL", "JKN" };  // With just "JN" I get 153 candidate hits, but at "JKN" narrows down to 9 hits.
    List<ClickSetsHit> hits = lpa.HuntForWheelsAndBindings(clicks);
    showCandidates(hits);

}

void TestletterPairAttack()
{  // https://www.codesandciphers.org.uk/virtualbp/tbombe/tbombe.htm
   // Initial attacks used letter pairs only.
   // Let us generate a very contrived crib for which we know the answers as test for an attack.
    string crib = "ATSSSAASSSSATTTTTT";
    Enigma theMachine = new Enigma(Reflector.UKW_B, Rotor.I, Rotor.II, Rotor.III, new Plugboard());
    theMachine.ChangeRotorsToShow("SUE");
    string crypt = theMachine.EncodeText(crib);
    Console.WriteLine("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
    Console.WriteLine(crib);
    Console.WriteLine(crypt);

    //ABCDEFGHIJKLMNOPQRSTUV.... positions
    //ATSSSAASSSSATTTTTT
    //YSPFEYYIHLMQSRSSWS
    // Initial letter pairing duplicates for this rich crib
    // AY at indexes AFG and TS at (1,12,14,15,17)  

    List<string> clicks = new List<string>();  // The paper above says Bletchley called these clicks.
    LetterPairAttacker lpa = new LetterPairAttacker(theMachine);
    //clicks.Add("AF");
    //List<ClickSetsHit> hits2 = lpa.HuntForWheelsAndBindings(clicks);
    //showCandidates(hits2);
    //clicks.Clear();
    //clicks.Add("AFG");
    //List<ClickSetsHit> hits3 = lpa.HuntForWheelsAndBindings(clicks);
    //showCandidates(hits3);

    clicks.Clear();

    clicks.Add("BMOP");   // BMOP works. "BMOPR" does not work on the 'R', becaue of rollover ov wheel at V, or E+17
    List<ClickSetsHit> hits4 = lpa.HuntForWheelsAndBindings(clicks);
    showCandidates(hits4);



    //clicks.Clear();
    //clicks.Add("BMOPR");   // BMOP works. "BMOPR" does not work on the 'R', becaue of rollover ov wheel at V, or E+17
    //hits4 = lpa.HuntForWheelsAndBindings(clicks);
    //showCandidates(hits4);


    //clicks.Clear();
    //string cribPositionsWithLastPosnRollovever = "MBOP" + ((char)(26 + 'R'));
    //clicks.Add(cribPositionsWithLastPosnRollovever);   
    //hits4 = lpa.HuntForWheelsAndBindings(clicks);
    //showCandidates(hits4);

    clicks.Insert(0, "AFG");
    List<ClickSetsHit> hits3_5 = lpa.HuntForWheelsAndBindings(clicks);
    showCandidates(hits3_5);
}

void showCandidates(List<ClickSetsHit> hits)
{
    foreach (var hit in hits)
    {
        Console.WriteLine(hit);
    }
    Console.WriteLine($"----------- hits={hits.Count}");
}


void testLyndatKnownPlugs()
{
    https://courses.csail.mit.edu/6.857/2018/project/lyndat-nayoung-ssrusso-Enigma.pdf
    string ciph = "ERWEEMGZDQJSTPCPYUAUEFDZOANLN";  // Try find settings from lyndat
    string crib = "SASUNARUTOANIMEDATTEBAYOANIME";
    // OK, Lyndat works, but his rotor order and initial settings must be read in reverse.  This finds a match at YJF.
    Enigma theMachine = new Enigma(Reflector.UKW_B, Rotor.I, Rotor.II, Rotor.III, new Plugboard());
    BruteForcer bf = new BruteForcer(theMachine);
    bf.theMachine.PlugB.SetWirings("AC DE GS IM NO PV QY TZ BH KW");
    bf.TryAllInitialSettings(ciph, crib);
}

void testLyndatMarkworth()
{
https://courses.csail.mit.edu/6.857/2018/project/lyndat-nayoung-ssrusso-Enigma.pdf
    string ciph = "IUGHLUVFAOBNEWNAGZW";  // Try find settings from lyndat p.9
    string crib = "MARKWORTHXATTACKEDX";
    Enigma theMachine = new Enigma(Reflector.UKW_B, Rotor.I, Rotor.II, Rotor.III, new Plugboard());
    BruteForcer bf = new BruteForcer(theMachine);
    bf.theMachine.PlugB.SetWirings("AC DE GS IM NO PV QY TZ BH KW");
 
   List<Rotor> rs = new List<Rotor>() { Rotor.I, Rotor.II, Rotor.III };
   // bf.TryAllRotorSelections(rs, ciph, crib);
    bf.TryAllRotorsAndRings(rs, ciph, crib);
}

void TestPerms()
{  // Eyeball test
    Helpers.TestPerms(new List<string>() { "one", "two", "three", "four" });
    Helpers.TestPerms(new List<int>() { 1, 2, 3 });
    Helpers.TestPerms(new List<Rotor>() { Rotor.I, Rotor.II, Rotor.III });
}

void testChoices()
{  // Eyeball test
    Helpers.TestChooseFrom(2, new List<string>() { "one", "two", "three", "four" });
    Helpers.TestChooseFrom(3, new List<int>() { 1, 2, 3, 4, 5 });
    Helpers.TestChooseFrom(1, new List<Rotor>() { Rotor.I, Rotor.II, Rotor.III });
}


void testBruteForcer()
{
    Enigma theMachine = new Enigma(Reflector.UKW_B, Rotor.III, Rotor.II, Rotor.I, new Plugboard());
    BruteForcer bf = new BruteForcer(theMachine);
    string ciph = "ERWEEMGZDQJSTPCPYUAUEFDZOANLN";
    string crib = "SASUNARUTOANIMEDATTEBAYOANIME";

    string ok1a = "THISISATESTTHATWORKS";
    string ok1b = "DAGZDAEOIYVZRBRRKPNY"; // werified on Enigma simulator  wheels iii/a ii/a i/a
    bf.TryAllInitialSettings(ok1b, ok1a);

    bf.theMachine.PlugB.SetWirings("AC DE GS IM NO PV QY TZ BH KW");
    string ok1c = "HETPHEKEGMYZTRUOXVAV"; // Verified with plugboard settings on Enigma simulator
    bf.TryAllInitialSettings(ok1a, ok1c);
}

void findInvariants(string pbSettings)
{
    string plain = "MARKWORTHXATTACKNOWATDAWNTUES";
    Plugboard pb = new Plugboard();
    pb.SetWirings(pbSettings);
    Enigma m1 = new Enigma(Reflector.UKW_B, Rotor.I, Rotor.II, Rotor.IV, pb);
    m1.ChangeRotorsToShow("AAA");
    string cipText = m1.EncodeText(plain);
    Console.WriteLine($"------------------ Plugboard swaps = {pbSettings}");
    Console.WriteLine(plain);
    Console.WriteLine(cipText);

    Graph g = new Graph(plain, cipText);
    LoopGatherer result = g.findLoops();
    Console.WriteLine($"\nTotal number of loops found = {result.TheLoops.Count}");
    foreach (GPath gp in result.TheLoops)
    {
        Console.WriteLine(gp);
    }
}


//findInvariants("WC EF DZ QM XY");

//lyndat();

//TestRollover("AAA", "AAF", "ILBDA");
//TestRollover("AAU", "ABZ", "WPSBR");
//TestRollover("AEA", "BFF", "ILFDE");
//TestRollover("QEU", "RGZ", "ANSCC");
//TestRollover("ADU", "BFZ", "IBXXX");
//TestRollover("QEV", "RFA", "ZJQYJ");

//void TestRollover(string start, string final, string ciphertext)
//{
//    Enigma m1 = new Enigma(Reflector.UKW_B, Rotor.I, Rotor.II, Rotor.III, new Plugboard());
//    m1.ChangeRotorsToShow(start);
//    string cip = m1.EncodeText("HELLO");
//   if (final != m1.VisibleInWindows || cip != ciphertext)
//    {
//        someTestsFailed = true;
//        string problem = $"On initial setting {start}: Window expected {final} but got {m1.VisibleInWindows}; Cipher expected {ciphertext} but got {cip}";
//        Console.WriteLine(problem);
//    }

//}
//if (!someTestsFailed)
//{
//    Console.WriteLine("Tricky roll-over tests all passed.");
//}


//Test01();
//Test02();
//Test03();


//Rotor RIII = Rotor.III;

//RIII.SetRing0(1);
//RIII.SetRing0(2);
//RIII.SetRing0(25);


//rotorTests();

//someTestsFailed = false;
//enigmaTests();
//encodingTests();

void lyndat()
{  // https://courses.csail.mit.edu/6.857/2018/project/lyndat-nayoung-ssrusso-Enigma.pdf
    //string ciph = "ERWEEMGZDQJSTPCPYUAUEFDZOANLN";
    //string crib = "SASUNARUTOANIMEDATTEBAYOANIME";
    //Graph g = new Graph(ciph, crib);

    string cp2 = "IUGHLUVFAOBNEWNAGZW";
    string pt2 = "MARKWORTHXATTACKEDX";
    Graph g = new Graph(cp2, pt2);
    LoopGatherer result = g.findLoops();
    Console.WriteLine($"\nTotal number of loops found = {result.TheLoops.Count}");
    foreach (GPath gp in result.TheLoops)
    {
        Console.WriteLine(gp);
    }
    //Plugboard pb = new Plugboard();
    //pb.SetWirings("AC DE GS IM NO PV QY TZ BH KW");
    //Console.WriteLine($"{crib} expected.");
    //foreach (Reflector rf in new Reflector[] { Reflector.UKW_A, Reflector.UKW_B, Reflector.UKW_C })
    //{
    //    Enigma m1 = new Enigma(rf, Rotor.III, Rotor.II, Rotor.I, pb);
    //    m1.ChangeRotorsToShow("FJY");
    //    string actual = m1.EncodeText(ciph);
    //    Console.WriteLine(actual);
    //}
}



void Test01()
{
    Enigma m1 = new Enigma(Reflector.UKW_B, Rotor.I, Rotor.II, Rotor.IV, new Plugboard());

    m1.SetRotorRing(2, 'Z'); 

    m1.ChangeRotorsToShow("AAA");

    string inp = "ABCDE FGHIJ KLMNO PQRST UVWXY ZABCD EFGHI JKLMN OPQRS TUVWX YZ ".Replace(" ", "");
    string result = m1.EncodeText(inp);

    m1.ChangeRotorsToShow("AAA");
    string result2 = m1.EncodeText(result);

    Console.WriteLine(inp);
    Console.WriteLine(result);
    Console.WriteLine(result2);
    if (result == "SMKPXEQGGYBMSVNKDWQQJMAKSAFWRUYRTDYXFJZHFKYWPBENKWOQ")
    {
        Console.WriteLine("Pass");
    }
    else
    {
        Console.WriteLine("Fail");
    }
}

void Test02()
{
    Enigma m1 = new Enigma(Reflector.UKW_B, Rotor.I, Rotor.II, Rotor.IV, new Plugboard());

    m1.PlugB.SetWirings("QW ER TZ UI YP CX BV MN");
 
    m1.SetRotorRing(2, 'Z');
    m1.ChangeRotorsToShow("AAA");

    string inp = "ABCDE FGHIJ KLMNO PQRST UVWXY ZABCD EFGHI JKLMN OPQRS TUVWX YZ ".Replace(" ", "");
    string result = m1.EncodeText(inp);

    m1.ChangeRotorsToShow("AAA");
    string result2 = m1.EncodeText(result);

    Console.WriteLine(inp);
    Console.WriteLine(result);
    Console.WriteLine(result2);
 
    if (result == "SUOYZRWGLPVNKRMEILWQFTCRZDFXNIGEZDQCFJPDFHFMYFVCEFUU")
    {
        Console.WriteLine("Pass");
    }
    else
    {
        Console.WriteLine("Fail");
    }
}

void Test03()
{
    Enigma m1 = new Enigma(Reflector.UKW_C, Rotor.III, Rotor.II, Rotor.V, new Plugboard());

    m1.PlugB.SetWirings("QU ER FK ST BY");
    m1.SetRotorRing(1, 'Q');
    m1.ChangeRotorsToShow("EPW");

    string inp = "ABCDE FGHIJ KLMNO PQRST UVWXY ZABCD EFGHI JKLMN OPQRS TUVWX YZ ".Replace(" ", "");
    string result = m1.EncodeText(inp);

    m1.ChangeRotorsToShow("EPW");
    string result2 = m1.EncodeText(result);

    Console.WriteLine(inp);
    Console.WriteLine(result);
    Console.WriteLine(result2);
    if (result == "LCHXGZWDHSSIOJLMMLFXQFGRKSWYQUVCJATBLJAVYWUELMVQRMTK")
    {
        Console.WriteLine("Pass");
    }
    else
    {
        Console.WriteLine("Fail");
    }

}



void encodingTests()
{
    string s0 = "Oh,my love, my darling, I've hungered for your touch";
    //   Enigma m = new Enigma(Reflector.BACDF, Rotor.II, Rotor.IV, Rotor.I, new Plugboard());

    string s1 = Enigma.CleanText(s0);
    Console.WriteLine($"\"{s0}\" cleans to \n     {s1}");

    Rotor r1 = Rotor.III;
    Rotor r2 = Rotor.IV;
    Rotor r3 = Rotor.I;

    List<string> cipherStrings = new List<string>();
    Enigma m = new Enigma(Reflector.UKW_B, r1, r2, r3, new Plugboard());
    m.ChangeRotorsToShow("ABC");
    for (int rs = 1; rs <= 26; rs++)
    {

        r1.SetRing0(rs-1);
        m.ChangeRotorsToShow("ABC");
        string s2 = m.EncodeText(s1);
        cipherStrings.Add(s2);
      //  Console.WriteLine($"When R1 rs={rs}, {s1} ==>\n{s2}");

        Console.WriteLine($"When R1.Setring({rs.ToString("D2")}), ==> {s2}");

    }

    bool ok = true;
    for (int rs = 1; rs <= 26; rs++)
    {
        r1.SetRing0(rs-1);
        m.ChangeRotorsToShow("ABC");
        string s2 = m.EncodeText(cipherStrings[rs-1]);

        //  Console.WriteLine($"When rs={rs}, {s1} ==>\n{s2}");
        if (s2 != s1)
        {
            ok = false;

            Console.WriteLine($"OOPS: When rs={rs.ToString("D2")}, we decrypt to ==> {s2}");
        }

    }
    if (ok)
    {
        Console.WriteLine("All decrypts passed");
    }

    // https://en.wikipedia.org/wiki/Enigma_rotor_details
    // With the rotors I, II, III (from left to right), wide B - reflector,
    // all ring settings in B position, and start position AAA,
    // typing AAAAA will produce the encoded sequence EWTYX.
    Enigma m2 = new Enigma(Reflector.UKW_B, Rotor.I, Rotor.II, Rotor.III, new Plugboard());
    string s3 = "AAAAA";
    m2.SetRotorRing(0, 'B');
    m2.SetRotorRing(1, 'B');
    m2.SetRotorRing(2, 'B');
    m2.ChangeRotorsToShow("AAA");
    string s4 = m2.EncodeText(s3);
    Console.WriteLine($"{s3} ==> {s4}, expect \"EWTYX\" [wiki's Enigma Wiring Details]");

}


void enigmaTests()
{
    Enigma m = new Enigma(Reflector.BACDF, Rotor.Identity, Rotor.Identity, Rotor.Identity, new Plugboard());
    m.ChangeRotorsToShow("ZZZ");
    string win = m.VisibleInWindows;
    if (win != "ZZZ")
    {
        Console.WriteLine($"1. Expected  windows to show 'ZZZ' but got '{win}'.");
        someTestsFailed = true;
    }
    m.AdvanceRotors();
    win = m.VisibleInWindows;
    if (win != "AAA")
    {
        Console.WriteLine($"2. Expected windows to show 'AAA' but got '{win}'.");
        someTestsFailed = true;
    }

    for (int i=0; i < 26; i++)
    {
        m.AdvanceRotors();
    }
    win = m.VisibleInWindows;
    if (win != "ABA")
    {
        Console.WriteLine($"3. Expected window to show 'ABA' but got '{win}'.");
        someTestsFailed = true;
    }

    // Try some encoding but never advance the wheels.  They are all identity rotors anyway, and the plugboard has no
    // cross-connects;
    StringBuilder sb = new StringBuilder();
    for (int i=0; i < 26; i++)
    {
        char c = m.EncodeNoStep((char)('A' + i));
        sb.Append(c);
    }
    string expected = m.Reflect.Substitutions;
    string result = sb.ToString();
    if (sb.ToString() != expected)
    {
        Console.WriteLine($"Expected to see {expected} but got out {result}.");
        someTestsFailed = true;
    }

    m.ChangeRotorsToShow("ATZ");
    if (someTestsFailed)
    {
        Console.WriteLine("Other Enigma tests passed.");
    }
    else
    {
        Console.WriteLine("Enigma tests passed.");
    }

}


void rotorTests()
{

    // ---------------------------  Now some more real stuff

    Rotor RIII = Rotor.III;

    RIII.PosAtWin0 = 0;
    testRotorMapping(RIII, 0, 1);
    testRotorMapping(RIII, 24, 16);

    RIII.PosAtWin0 = 2;
    testRotorMapping(RIII, 0, 3);
    testRotorMapping(RIII, 1, 5);

    RIII.PosAtWin0 = 24;
    testRotorMapping(RIII, 2, 3);
    testRotorMapping(RIII, 4, 7);
    testRotorMapping(RIII, 3, 5);
    testRotorMapping(RIII, 15, 15);


    RIII.PosAtWin0 = 25;
    testRotorMapping(RIII, 2, 4);
    testRotorMapping(RIII, 3, 6);
    testRotorMapping(RIII, 25, 17);
    testRotorMapping(RIII, 1, 2);
    testRotorMapping(RIII, 14, 14);


    //testReverseMapping(RIII, 1, 'A');
    //testReverseMapping(RIII, 0, 'Z');
    //testReverseMapping(RIII, 25, 'Y'); 
    //// https://en.wikipedia.org/wiki/Enigma_rotor_details  section on ring settings, using a different rotor
    Rotor RI = Rotor.I;

    RI.PosAtWin0 = 0;

    testRotorMapping(RI, 0, 4);
    RI.SetRing0(01);
    testRotorMapping(RI, 0, 10);   // Wiki says A -> K



    RI.SetRing0(25);


    if (someTestsFailed)
    {
        Console.WriteLine("Other Rotor tests passed.");
    }
    else
    {
        Console.WriteLine("Rotor tests passed.");
    }
}

void testRotorMapping(Rotor r, int inPin, int expected)
{
   
    int pad = r.PinToPad(inPin);
    if (pad != expected)
    {
        someTestsFailed = true;
        string problem = $"Forward RotorMapping:{ r.RotorName} ring0={r.RingSetting0} Win={r.PosAtWin0} expected  (pad {expected} <- pin {inPin}), but got ({pad} <- {inPin})";
        Console.WriteLine(problem);
    }
    else  // Also test the reverse mapping, pad to pin.
    {
        int outPin = r.PadToPin(pad);
        if (outPin != inPin)
        {
            someTestsFailed = true;
            string problem = $"Reverse RotorMapping: {r.RotorName} ring0={r.RingSetting0} Win={r.PosAtWin0} return path expected (pad {pad}-> pin {inPin}), but got ({pad} -> {outPin})";
            Console.WriteLine(problem);
        }
    }
}

