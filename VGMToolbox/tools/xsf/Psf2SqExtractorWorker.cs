using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.format;
using VGMToolbox.format.util;
using VGMToolbox.util;

namespace VGMToolbox.tools.xsf
{
    class Psf2SqExtractorWorker : BackgroundWorker
    {
        private int fileCount = 0;
        private int maxFiles = 0;
        Dictionary<string, string> extractedLibHash;
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
            extractedLibHash = new Dictionary<string, string>();
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
            string[] libPaths;            
            string[] sqFiles;
            string[] iniFiles;
            Psf2.Psf2IniSqIrxStruct psf2IniStruct;
            
            string arguments;
            bool isSuccess;

            string filePath;
            string fileDir;
            string fileName;
            string outputDir;
            string libOutputDir;

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

                        if (psf2File.getFormat().Equals(Xsf.FORMAT_NAME_PSF2) && 
                            (!psf2File.IsFileLibrary()))
                        {
                            filePath = Path.GetFullPath(pPath);
                            fileDir = Path.GetDirectoryName(filePath);
                            fileName = Path.GetFileNameWithoutExtension(filePath);
                            outputDir = Path.Combine(fileDir, fileName);
                            
                            try
                            {                                                                
                                // call unpkpsf2.exe
                                arguments = String.Format(" \"{0}\" \"{1}\"", filePath, outputDir);
                                unpkPsf2Process = new Process();
                                unpkPsf2Process.StartInfo = new ProcessStartInfo(PROGRAM_PATH, arguments);
                                unpkPsf2Process.StartInfo.UseShellExecute = false;
                                unpkPsf2Process.StartInfo.CreateNoWindow = true;
                                isSuccess = unpkPsf2Process.Start();
                                unpkPsf2Process.WaitForExit();
                                unpkPsf2Process.Close();
                                unpkPsf2Process.Dispose();

                                // parse ini
                                iniFiles = Directory.GetFiles(outputDir, "PSF2.INI", SearchOption.AllDirectories);
                                using (FileStream iniFs = File.Open(iniFiles[0], FileMode.Open, FileAccess.Read))
                                {
                                    // parse ini file to get SQ info
                                    psf2IniStruct = Psf2.ParseClsIniFile(iniFs);
                                }

                                // check for libs
                                libPaths = psf2File.GetLibPathArray();

                                if ((libPaths == null) || (libPaths.Length == 0)) // PSF2
                                {
                                    // copy the SQ file out (should only be one)
                                    sqFiles = Directory.GetFiles(outputDir, psf2IniStruct.SqFileName, SearchOption.AllDirectories);

                                    if (sqFiles.Length > 0)
                                    {
                                        if (!String.IsNullOrEmpty(psf2IniStruct.SequenceNumber))
                                        {
                                            outputSqFileName = String.Format("{0}_n={1}.SQ", outputDir, psf2IniStruct.SequenceNumber);
                                        }
                                        else
                                        {
                                            outputSqFileName = String.Format("{0}.SQ", outputDir);                                        
                                        }
                                        File.Copy(sqFiles[0], outputSqFileName, true);
                                    }
                                }
                                else // miniPSF2
                                {                                    
                                    // unpack each lib, looking for the needed file
                                    foreach (string libPath in libPaths)
                                    {
                                        fileDir = Path.GetDirectoryName(libPath);
                                        fileName = Path.GetFileNameWithoutExtension(libPath);
                                        libOutputDir = Path.Combine(fileDir, fileName);

                                        if (!extractedLibHash.ContainsKey(libPath))
                                        {
                                            // call unpkpsf2.exe
                                            arguments = String.Format(" \"{0}\" \"{1}\"", libPath, libOutputDir);
                                            unpkPsf2Process = new Process();
                                            unpkPsf2Process.StartInfo = new ProcessStartInfo(PROGRAM_PATH, arguments);
                                            unpkPsf2Process.StartInfo.UseShellExecute = false;
                                            unpkPsf2Process.StartInfo.CreateNoWindow = true;
                                            isSuccess = unpkPsf2Process.Start();
                                            unpkPsf2Process.WaitForExit();
                                            unpkPsf2Process.Close();
                                            unpkPsf2Process.Dispose();

                                            extractedLibHash.Add(libPath, libOutputDir);
                                        }

                                        // look for the file in this lib
                                        sqFiles = Directory.GetFiles(libOutputDir, psf2IniStruct.SqFileName, SearchOption.AllDirectories);

                                        if (sqFiles.Length > 0)
                                        {
                                            if (!String.IsNullOrEmpty(psf2IniStruct.SequenceNumber))
                                            {
                                                outputSqFileName = String.Format("{0}_n={1}.SQ", outputDir, psf2IniStruct.SequenceNumber);
                                            }
                                            else
                                            {
                                                outputSqFileName = String.Format("{0}.SQ", outputDir);
                                            }
                                            
                                            File.Copy(sqFiles[0], outputSqFileName, true);                                                                                       
                                            break;
                                        }

                                        // delete the unpkpsf2 output folder
                                        Directory.Delete(libOutputDir, true);
                                    
                                    } // foreach (string libPath in libPaths)
                                }
                                // delete the unpkpsf2 output folder
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
                        } // if (psf2File.getFormat().Equals(Xsf.FORMAT_NAME_PSF2) && (!psf2File.IsFileLibrary()))
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

            // delete lib folders
            foreach (string k in extractedLibHash.Keys)
            {
                Directory.Delete(extractedLibHash[k], true);
            }
        }        
    }
}
