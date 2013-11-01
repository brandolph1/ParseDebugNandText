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
        [STAThreadAttribute]        
        static void Main(string[] args)
        {
            String strInFile = null;
            FileStream fsInFile = null;

            if (0 < args.Length)
            {
                strInFile = args[0];
            }
            else
            {
                CFileDialog fd = new CFileDialog();

                strInFile = fd.GetFile();
            }

            Console.WriteLine("Filename={0}", strInFile);

            try
            {
                fsInFile = new FileStream(strInFile, FileMode.Open, FileAccess.Read);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return;
            }

            CNandTextReader ntr = new CNandTextReader();
            String strPageBlob = ntr.GetPageText(fsInFile, 0, 0);

            if (strPageBlob.Length > 0)
            {
                Console.Write(strPageBlob);
            }
            else
            {
                Console.WriteLine("Failed to read the requested page!");
            }
        }
    }
}
