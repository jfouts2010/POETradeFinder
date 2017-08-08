using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TestingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            int counter = 0;
            double zip = 0;
            double buffer = 0;
            double zipbuffer = 0;
            double regular = 0;
            while (counter++ < 100)
            {
                DateTime start = DateTime.Now;
                WebRequest req = HttpWebRequest.Create("http://www.pathofexile.com/api/public-stash-tabs?id=76572574-80473084-75408440-87518536-81408323");
                req.Headers.Add("Accept-Encoding", "gzip");
                using (HttpWebResponse response = req.GetResponse() as HttpWebResponse)
                {
                    // Get the response stream  
                    using (GZipStream stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
                    {


                        using (StreamReader reader = new StreamReader(stream))
                        {
                            /*
                            char[] buffer = new char[65];
                            reader.ReadBlock(buffer, 0, 64);
                            string newstring = new string(buffer);
                            */
                            string longgg = reader.ReadToEnd();
                            double seconds0 = (DateTime.Now - start).TotalSeconds;
                            zip += seconds0;
                            Console.Write("Zip: " + seconds0);
                        }

                    }
                }
                start = DateTime.Now;
                req = HttpWebRequest.Create("http://www.pathofexile.com/api/public-stash-tabs?id=76572574-80473084-75408440-87518536-81408323");
                req.Headers.Add("Accept-Encoding", "gzip");
                using (HttpWebResponse response = req.GetResponse() as HttpWebResponse)
                {
                    // Get the response stream  
                    using (GZipStream stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
                    {
                        using (BufferedStream bs = new BufferedStream(stream))
                        {
                            using (StreamReader reader = new StreamReader(bs))
                            {
                                /*
                                char[] buffer = new char[65];
                                reader.ReadBlock(buffer, 0, 64);
                                string newstring = new string(buffer);
                                */
                                string longgg = reader.ReadToEnd();
                                double seconds0 = (DateTime.Now - start).TotalSeconds;
                                zipbuffer += seconds0;
                                Console.Write("  Zip buff: " + seconds0);
                            }
                        }
                    }
                }
                start = DateTime.Now;
                req = HttpWebRequest.Create("http://www.pathofexile.com/api/public-stash-tabs?id=76572574-80473084-75408440-87518536-81408323");

                using (HttpWebResponse response = req.GetResponse() as HttpWebResponse)
                {
                    // Get the response stream  
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (BufferedStream bs = new BufferedStream(stream))
                        {
                            using (StreamReader reader = new StreamReader(bs))
                            {
                                /*
                                char[] buffer = new char[65];
                                reader.ReadBlock(buffer, 0, 64);
                                string newstring = new string(buffer);
                                */
                                string longgg = reader.ReadToEnd();
                                double seconds0 = (DateTime.Now - start).TotalSeconds;
                                buffer += seconds0;
                                Console.Write("    Buffered: " + seconds0);
                            }
                        }
                    }
                }
                start = DateTime.Now;
                req = HttpWebRequest.Create("http://www.pathofexile.com/api/public-stash-tabs?id=76572574-80473084-75408440-87518536-81408323");

                using (HttpWebResponse response = req.GetResponse() as HttpWebResponse)
                {
                    // Get the response stream  
                    using (Stream stream = response.GetResponseStream())
                    {

                        using (StreamReader reader = new StreamReader(stream))
                        {
                            /*
                            char[] buffer = new char[65];
                            reader.ReadBlock(buffer, 0, 64);
                            string newstring = new string(buffer);
                            */
                            string longgg = reader.ReadToEnd();
                            double seconds0 = (DateTime.Now - start).TotalSeconds;
                            regular += seconds0;
                            Console.WriteLine("    Regular: " + seconds0);
                        }
                    }
                }

            }
            Console.WriteLine("Regular Average " + (regular / 100));
            Console.WriteLine("Buffer Average " + (buffer / 100));
            Console.WriteLine("Zip Average " + (zip / 100));
            Console.WriteLine("Combined Average " + (zipbuffer / 100));
            int x = 5;


        }
    }
}
