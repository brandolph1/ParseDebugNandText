using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WelchAllyn.DebugNandTextLib
{
    public class CNandTextReader
    {
        public String GetPageText(FileStream fs, int block, int page)
        {
            String strOut = null;
            // Example search string:: === page:0 (block:0/page:0) ===
            String strSearch = String.Format("(block:{0}/page:{1})", block, page);
            String strLine;

            fs.Seek(0L, SeekOrigin.Begin);

            StreamReader sr = new StreamReader(fs, Encoding.ASCII);
#if DEBUG
            Console.WriteLine("Searching for: '{0}'", strSearch);
#endif
            while (!sr.EndOfStream)
            {
                if (sr.ReadLine().Contains(strSearch))
                {
#if DEBUG
                    Console.WriteLine("Found it!");
#endif
                    break;
                }
            }

            while (!sr.EndOfStream)
            {
                strLine = sr.ReadLine();

                if (strLine.Length < 1)
                {
                    break;
                }
                else
                {
                    strOut += strLine;
                }
            }

            return strOut;
        }

        public Byte[] GetPage(String strIn)
        {
            Byte[] abyOut = null;
            // Example string data:
            // <<<<<<<<< spare area >>>>>>>>>
            // ffff ffff ffff 3f30 ffff ff03 ffff ffff
            // <<<<<<<<< main area >>>>>>>>>
            // ea000007 0001fd3c 65210a1b a7f007f8
            // a7f0081c a7f00844 a7f00868 00000018
            //

            return abyOut;
        }
    }
}
