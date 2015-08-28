using System;
using System.Windows.Forms;

namespace WelchAllyn.FileDialog
{
    public class CFileDialog
    {
        public String GetFile()
        {
            String strInFile = null;
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.DefaultExt = "";
            ofd.Filter = "Text files (*.txt)|*.txt|Log files (*.log)|*.log|Bin files (*.bin)|*.bin|All files (*.*)|*.*";
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            ofd.Multiselect = false;
            ofd.ShowHelp = false;
            ofd.Title = "Open a NAND Flash Data File";

            if (DialogResult.OK == ofd.ShowDialog())
            {
                strInFile = ofd.FileName;
            }

            ofd.Dispose();
            return strInFile;
        }
    }
}
