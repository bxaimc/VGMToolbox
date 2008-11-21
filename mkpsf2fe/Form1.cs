using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace mkpsf2fe
{
    public partial class Form1 : Form
    {
        private readonly string WORKING_FOLDER = Path.Combine(".", "working");
        private readonly string MODULES_FOLDER = Path.Combine(".", "modules");
        private readonly string PROGRAMS_FOLDER = Path.Combine(".", "programs");
        private readonly string OUTPUT_FOLDER = Path.Combine(".", "psf2s");

        public Form1()
        {                        
            InitializeComponent();
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            tbOutput.Clear();

            string[] uniqueSqFiles = this.getUniqueFileNames();
            makePsf2s(uniqueSqFiles, tbModulesDirectory.Text,
                WORKING_FOLDER, MODULES_FOLDER, OUTPUT_FOLDER);
        }

        private string[] getUniqueFileNames()
        {
            int fileCount = 0;
            int i = 0;
            string[] ret = null;

            if (!Directory.Exists(tbSourceDirectory.Text))
            {
                tbOutput.Text += String.Format("ERROR: Directory {0} not found.", tbSourceDirectory.Text);
            }
            else
            {
                fileCount = Directory.GetFiles(tbSourceDirectory.Text, "*.SQ").Length;

                if (fileCount > 0)
                {
                    ret = new string[fileCount];
                }


                foreach (string f in Directory.GetFiles(tbSourceDirectory.Text, "*.SQ"))
                {
                    ret[i] = f;
                    i++;
                }
            }

            return ret;
        }

        private void makePsf2s(string[] pUniqueSqFiles, string pSourceModulesFolder, 
            string pWorkingFolder, string pGenericModulesFolder, string pOutputFolder)
        {
            Process makePsf2Process;
            
            Directory.CreateDirectory(pWorkingFolder);           
            
            // copy generic modules to working directory
            File.Copy(Path.Combine(pGenericModulesFolder, "psf2.irx"), Path.Combine(pWorkingFolder, "psf2.irx"), true);
            File.Copy(Path.Combine(pGenericModulesFolder, "sq.irx"), Path.Combine(pWorkingFolder, "sq.irx"), true);

            // copy source modules to working directory
            File.Copy(Path.Combine(pSourceModulesFolder, "LIBSD.IRX"), Path.Combine(pWorkingFolder, "LIBSD.IRX"), true);
            File.Copy(Path.Combine(pSourceModulesFolder, "MODHSYN.IRX"), Path.Combine(pWorkingFolder, "MODHSYN.IRX"), true);
            File.Copy(Path.Combine(pSourceModulesFolder, "MODMIDI.IRX"), Path.Combine(pWorkingFolder, "MODMIDI.IRX"), true);

            // copy program
            string makePsf2SourcePath = Path.Combine(PROGRAMS_FOLDER, "mkpsf2.exe");
            string makePsf2DestinationPath = Path.Combine(".", "mkpsf2.exe");
            File.Copy(makePsf2SourcePath, makePsf2DestinationPath, true);

            foreach (string f in pUniqueSqFiles)
            { 
                // copy data files to working directory
                string filePrefix = Path.GetFileNameWithoutExtension(f);
                string sourceDirectory = Path.GetDirectoryName(f);

                string bdFileName = filePrefix + ".bd";
                string hdFileName = filePrefix + ".hd";
                string sqFileName = filePrefix + ".sq";

                string sourceBdFile = Path.Combine(sourceDirectory, bdFileName);
                string sourceHdFile = Path.Combine(sourceDirectory, hdFileName);
                string sourceSqFile = Path.Combine(sourceDirectory, sqFileName);

                string destinationBdFile = Path.Combine(pWorkingFolder, bdFileName);
                string destinationHdFile = Path.Combine(pWorkingFolder, hdFileName);
                string destinationSqFile = Path.Combine(pWorkingFolder, sqFileName);

                File.Copy(sourceBdFile, destinationBdFile);
                File.Copy(sourceHdFile, destinationHdFile);
                File.Copy(sourceSqFile, destinationSqFile);

                // write ini file
                string iniPath = Path.Combine(pWorkingFolder, "psf2.ini");
                StreamWriter sw = File.CreateText(iniPath);
                sw.WriteLine("libsd.irx");
                sw.WriteLine("modhsyn.irx");
                sw.WriteLine("modmidi.irx");
                sw.WriteLine(String.Format("sq.irx -r=5 -d=16383 -s={0} -h={1} -b={2}",
                    sqFileName, hdFileName, bdFileName));
                sw.Close();
                sw.Dispose();

                // run makepsf2                
                string arguments = String.Format(" {0}.psf2 {1}", filePrefix, pWorkingFolder);
                makePsf2Process = new Process();
                makePsf2Process.StartInfo = new ProcessStartInfo(makePsf2DestinationPath, arguments);
                makePsf2Process.StartInfo.UseShellExecute = false;
                bool isSuccess = makePsf2Process.Start();
                makePsf2Process.WaitForExit();

                if (isSuccess)
                {
                    tbOutput.Text += String.Format("{0}.psf2 created.", filePrefix) + 
                        Environment.NewLine;
                    File.Move(filePrefix + ".psf2", Path.Combine(pOutputFolder, filePrefix + ".psf2"));
                }

                File.Delete(destinationBdFile);
                File.Delete(destinationHdFile);
                File.Delete(destinationSqFile);
                File.Delete(iniPath);

            }

            //Directory.Delete(pWorkingFolder, true);
            File.Delete(makePsf2DestinationPath);
        }

        private void btnSourceDirectoryBrowse_Click(object sender, EventArgs e)
        {
            folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                tbSourceDirectory.Text = folderBrowserDialog.SelectedPath;
            }
        }


    }
}
