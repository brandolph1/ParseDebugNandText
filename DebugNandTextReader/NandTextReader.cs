using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WelchAllyn.DebugNandTextLib
{
    public class CNandTextReader
    {
        private const int SIZEOF_SPARE_AREA = 16;
        private const int SIZEOF_MAIN_AREA = 512;
        private bool Debug = false;

        public CNandTextReader(bool debug)
        {
            Debug = debug;
        }

        public CNandTextReader() { }

        public List<String> GetPageText(FileStream fs, int block, int page)
        {
            // Example search string:: === page:0 (block:0/page:0) ===
            String strSearch = String.Format("(block:{0}/page:{1})", block, page);
            String strLine;
            List<String> vstrPageOut = new List<string>();

            fs.Seek(0L, SeekOrigin.Begin);

            StreamReader sr = new StreamReader(fs, Encoding.ASCII);
            
            if (Debug) Console.Error.WriteLine("Searching for: '{0}'", strSearch);

            while (!sr.EndOfStream)
            {
                if (sr.ReadLine().Contains(strSearch))
                {
                    if (Debug) Console.Error.WriteLine("Found it!");
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
                    vstrPageOut.Add(strLine);
                }
            }

            return vstrPageOut;
        }

        public Byte[] ParsePage(List<String> vstrIn)
        {
            Byte[] abyOut = null;
            // Example List<string> data:
            // 0: <<<<<<<<< spare area >>>>>>>>>
            // 1: ffff ffff ffff 3f30 ffff ff03 ffff ffff
            // 2: <<<<<<<<< main area >>>>>>>>>
            // 3: ea000007 0001fd3c 65210a1b a7f007f8
            // 4: a7f0081c a7f00844 a7f00868 00000018
            // X: ....

            if (vstrIn.Count > 0)
            {
                int nn = 0;
                String astr = vstrIn[nn];

                while (!astr.Contains(" main area "))
                {
                    ++nn;
                    astr = vstrIn[nn];
                }

                astr = vstrIn[nn + 1];
                abyOut = new Byte[SIZEOF_MAIN_AREA];
                int ii = 0;

                while ( astr.Length > 1 )
                {
                    String[] astrMainLongs = astr.Split(' ');

                    if (astrMainLongs.Length >= 4)
                    {
                        foreach (String ss in astrMainLongs)
                        {
                            if (ss.Length > 0)
                            {
                                abyOut[ii + 0] = Convert.ToByte(ss.Substring(6, 2), 16);
                                abyOut[ii + 1] = Convert.ToByte(ss.Substring(4, 2), 16);
                                abyOut[ii + 2] = Convert.ToByte(ss.Substring(2, 2), 16);
                                abyOut[ii + 3] = Convert.ToByte(ss.Substring(0, 2), 16);
                                ii += 4;
                            }
                        }
                    }

                    astr = "";
                    ++nn;

                    if (nn + 1 < vstrIn.Count)
                    {
                        astr = vstrIn[nn + 1];
                    }
                }
            }

            return abyOut;
        }

        public Byte[] ParseSpare(List<String> vstrIn)
        {
            Byte[] abyOut = null;
            // Example List<string> data:
            // 0: <<<<<<<<< spare area >>>>>>>>>
            // 1: ffff ffff ffff 3f30 ffff ff03 ffff ffff
            // 2: <<<<<<<<< main area >>>>>>>>>
            // 3: ea000007 0001fd3c 65210a1b a7f007f8
            // 4: a7f0081c a7f00844 a7f00868 00000018
            // X: ....

            if (vstrIn.Count > 0)
            {
                int nn = 0;
                String astr = vstrIn[nn];

                while (!astr.Contains(" spare area "))
                {
                    ++nn;
                    astr = vstrIn[nn];
                }

                astr = vstrIn[nn + 1];
                String[] astrSpareWords = astr.Split(' ');

                if (astrSpareWords.Length >= 8)
                {
                    abyOut = new Byte[SIZEOF_SPARE_AREA];
                    int ii = 0;

                    foreach (String ss in astrSpareWords)
                    {
                        if (ss.Length > 0)
                        {
                            abyOut[ii + 0] = Convert.ToByte(ss.Substring(2, 2), 16);
                            abyOut[ii + 1] = Convert.ToByte(ss.Substring(0, 2), 16);
                            ii += 2;
                        }
                    }
                }
            }

            return abyOut;
        }
    }
}
