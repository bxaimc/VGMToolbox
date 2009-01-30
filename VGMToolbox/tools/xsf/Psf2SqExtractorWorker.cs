using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.format;
using VGMToolbox.util;

namespace VGMToolbox.tools.xsf
{
    class Psf2SqExtractorWorker : BackgroundWorker
    {
        private int fileCount = 0;
        private int maxFiles = 0;
        Constants.ProgressStruct progressStruct;

        private readonly string PROGRAM_PATH =
            Path.Combine(Path.Combine(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "external"), "psf2"), "unpkpsf2.exe");

        public struct Psf2SqExtractorStruct
        {
            public string[] sourcePaths;
        }

        public Psf2SqExtractorWorker()
        {
            fileCount = 0;
            maxFiles = 0;
            progressStruct = new Constants.ProgressStruct();

            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
        }

        private void extractSqs(Psf2SqExtractorStruct pPsf2SqExtractorStruct, DoWorkEventArgs e)
        {
            maxFiles = FileUtil.GetFileCount(pPsf2SqExtractorStruct.sourcePaths);

            foreach (string path in pPsf2SqExtractorStruct.sourcePaths)
            {
                if (File.Exists(path))
                {
                    if (!CancellationPending)
                    {
                        this.extractSqFromFile(path, e);
                    }
                    else
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                else if (Directory.Exists(path))
                {
                    this.extractSqsFromDirectory(path, e);

                    if (CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }            
            return;
        }

        private void extractSqFromFile(string pPath, DoWorkEventArgs e)
        {
            Process unpkPsf2Process = null;
            string outputSqFileName;

            // Report Progress
            int progress = (++fileCount * 100) / maxFiles;
            progressStruct.Clear();
            progressStruct.filename = pPath;
            ReportProgress(progress, progressStruct);

            try
            {
                using (FileStream fs = File.OpenRead(pPath))
                {
                    Type dataType = FormatUtil.getObjectType(fs);

                    if (dataType != null && dataType.Name.Equals("Xsf"))
                    {
                        Xsf psf2File = new Xsf();
                        psf2File.Initialize(fs, pPath);

                        if (psf2File.getFormat().Equals(Xsf.FORMAT_NAME_PSF2))
                        {
                            string filePath = Path.GetFullPath(pPath);
                            string fileDir = Path.GetDirectoryName(filePath);
                            string fileName = Path.GetFileNameWithoutExtension(filePath);
                            string outputDir = Path.Combine(Path.GetDirectoryName(filePath),
                                fileName);
                            
                            try
                            {                                                                
                                // call unpkpsf2.exe
                                string arguments = String.Format(" \"{0}\" \"{1}\"", filePath, outputDir);
                                unpkPsf2Process = new Process();
                                unpkPsf2Process.StartInfo = new ProcessStartInfo(PROGRAM_PATH, arguments);
                                unpkPsf2Process.StartInfo.UseShellExecute = false;
                                unpkPsf2Process.StartInfo.CreateNoWindow = true;
                                bool isSuccess = unpkPsf2Process.Start();
                                unpkPsf2Process.WaitForExit();
                                unpkPsf2Process.Close();
                                unpkPsf2Process.Dispose();

                                // copy the SQ file out (should only be one, but let's make sure)
                                int i = 0;
                                string[] sqFiles = Directory.GetFiles(outputDir, "*.SQ", SearchOption.AllDirectories);

                                foreach (string s in sqFiles)
                                {
                                    if (sqFiles.Length > 1)
                                    {
                                        outputSqFileName = outputDir + "_" + i.ToString("X4") + ".SQ";
                                    }
                                    else
                                    {
                                        outputSqFileName = outputDir + ".SQ";
                                    }

                                    File.Copy(s, outputSqFileName);
                                    
                                    i++;
                                }

                                    
                                // delete the folder
                                Directory.Delete(outputDir, true);
                            }
                            catch (Exception ex)
                            {
                                if ((unpkPsf2Process != null) && (!unpkPsf2Process.HasExited))
                                {
                                    unpkPsf2Process.Close();
                                    unpkPsf2Process.Dispose();
                                }

                                this.progressStruct.Clear();
                                progressStruct.errorMessage = String.Format("Error processing <{0}>.  Error received: ", pPath) + ex.Message;
                                ReportProgress(progress, progressStruct);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.progressStruct.Clear();
                progressStruct.errorMessage = String.Format("Error processing <{0}>.  Error received: ", pPath) + ex.Message;
                ReportProgress(progress, progressStruct);
            }
        }    

        private void extractSqsFromDirectory(string pPath, DoWorkEventArgs e)
        {
            foreach (string d in Directory.GetDirectories(pPath))
            {
                if (!CancellationPending)
                {
                    this.extractSqsFromDirectory(d, e);
                }
                else
                {
                    e.Cancel = true;
                    break;
                }
            }
            foreach (string f in Directory.GetFiles(pPath))
            {
                if (!CancellationPending)
                {
                    this.extractSqFromFile(f, e);
                }
                else
                {
                    e.Cancel = true;
                    break;
                }
            }
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            Psf2SqExtractorStruct psf2SqExtractorStruct = (Psf2SqExtractorStruct)e.Argument;

            this.extractSqs(psf2SqExtractorStruct, e);
        }        
    }
}
