using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WelchAllyn.FileDialog;
using WelchAllyn.DebugNandTextLib;


namespace WelchAllyn.ParseDebugNandText
{
    class Program
    {
        private static String strInFile = null;
        private static bool Debug = false;

        private static void Check_Command_Args(string[] args)
        {
            foreach (string aa in args)
            {
                if (aa.ToLower() == "-debug")
                {
                    Debug = true;
                }
                else
                {
                    strInFile = aa;
                }
            }

            if (strInFile.Length == 0)
            {
                CFileDialog fd = new CFileDialog();

                strInFile = fd.GetFile();
            }
            else
            {
                if (!File.Exists(strInFile))
                {
                    Console.Error.WriteLine("ERROR: File not found or does not exist [{0}]", strInFile);
                    strInFile = "";
                }
            }
        }

        [STAThreadAttribute]        
        static void Main(string[] args)
        {
            FileStream fsInFile = null;

            Check_Command_Args(args);

            if (strInFile.Length == 0)
            {
                return;
            }

            Console.Error.WriteLine("Input file= {0}", strInFile);

            try
            {
                fsInFile = new FileStream(strInFile, FileMode.Open, FileAccess.Read);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return;
            }

            CNandTextReader ntr = new CNandTextReader(Debug);
            int nBlock = 0;
            int nPage = 0;
            List<String> vstrPageBlob = ntr.GetPageText(fsInFile, nBlock, nPage);

            if (vstrPageBlob.Count > 0)
            {
                FileStream fsOutFile = null;
                String strOutFile;

                try
                {
                    String strFsInName = fsInFile.Name;
                    String strDirName = Path.GetDirectoryName(strFsInName);

                    strOutFile = Path.Combine(strDirName, "out.bin");
                    fsOutFile = new FileStream(strOutFile, FileMode.Create, FileAccess.Write);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.ToString());
                    return;
                }

                Console.Error.WriteLine("Output file= {0}", strOutFile);
                bool fRun = true;

                do
                {
                    do
                    {
                        Byte[] main_page = ntr.ParsePage(vstrPageBlob);
                        Byte[] spare = ntr.ParseSpare(vstrPageBlob);

                        if ((main_page.Length == 512) &&
                            (spare.Length == 16))
                        {
                            fsOutFile.Write(main_page, 0, main_page.Length);
                            fsOutFile.Write(spare, 0, spare.Length);
                        }
                        else
                        {
                            Console.Error.Write("ERROR: Failed to parse ");

                            if (main_page.Length == 0)
                            {
                                Console.Error.WriteLine("main ");
                             }
                            else if (spare.Length == 0)
                            {
                                Console.Error.WriteLine("spare ");
                            }

                            Console.Error.WriteLine("area of page #{0} in block #{1}", nPage, nBlock);
                        }

                        ++nPage;

                        if (nPage < 32)
                        {
                            vstrPageBlob = ntr.GetPageText(fsInFile, nBlock, nPage);

                            if (!(vstrPageBlob.Count > 0))
                            {
                                Console.Error.WriteLine("Failed to read the requested page (b{0},p{1})", nBlock, nPage);
                                fRun = false;
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    while (true);

                    ++nBlock;
                    nPage = 0;
                    vstrPageBlob = ntr.GetPageText(fsInFile, nBlock, nPage);

                    if (!(vstrPageBlob.Count > 0))
                    {
                        Console.Error.WriteLine("Failed to read the requested page (b{0},p{1}), may have reached end of input file", nBlock, nPage);
                        fRun = false;
                    }
                }
                while (fRun);

                fsOutFile.Flush();
                fsOutFile.Close();
            }
            else
            {
                Console.Error.WriteLine("Failed to read the first page!");
            }

            fsInFile.Close();
            
            if (Debug)
            {
                Console.Error.Write("Press <Enter> to continue...");
                Console.ReadLine();
            }
        }
    }
}
