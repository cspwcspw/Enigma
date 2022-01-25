// See https://aka.ms/new-console-template for more information
 
// Grab some pics from the musuem site. The pics were most helpful.

HttpClient client = new HttpClient();

for (int num = 0; num < 100; num++)
{
    string img = num.ToString("D3");
    string uri = $"https://www.cryptomuseum.com/crypto/enigma/i/img/300002/{img}/full.jpg";
    string filename = $"C:\\Users\\Peter\\Desktop\\Enigma\\{num.ToString("D3")}.jpg";

    if (!File.Exists(filename))
    {
        HttpResponseMessage response = client.GetAsync(uri).Result;  // Blocking call!
        if (response.IsSuccessStatusCode)
        {
            Stream strm = response.Content.ReadAsStream();  
            Stream dest = new FileStream(filename, FileMode.CreateNew);
            strm.CopyTo(dest);
            dest.Close();
        }
        else
        {
            Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
        }
    }
}

/*  Some helpful pics of things we found

https://www.cryptomuseum.com/crypto/enigma/i/img/300002/018/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/021/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/022/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/024/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/026/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/027/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/028/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/029/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/030/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/031/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/032/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/033/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/035/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/036/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/038/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/039/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/040/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/041/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/042/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/043/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/044/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/045/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/046/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/047/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/048/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/050/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/051/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/052/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/053/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/055/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/056/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/058/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/062/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/063/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/065/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/066/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/071/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/073/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/075/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/077/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/080/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/081/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/083/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/084/full.jpg
https://www.cryptomuseum.com/crypto/enigma/i/img/300002/085/full.jpg
 .
*/ 