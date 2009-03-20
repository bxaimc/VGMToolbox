using System;
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
    class Psf2TimerWorker : BackgroundWorker
    {
        private int fileCount = 0;
        private int maxFiles = 0;
        Dictionary<string, string> extractedLibHash;
        Constants.ProgressStruct progressStruct;

        private const string BATCH_FILE_NAME = "!timing_batch.bat";

        public struct Psf2TimerStruct
        {
            public string[] sourcePaths;
        }

        public Psf2TimerWorker()
        {
            fileCount = 0;
            maxFiles = 0;
            extractedLibHash = new Dictionary<string, string>();
            progressStruct = new Constants.ProgressStruct();

            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
        }

        private void timePsf2s(Psf2TimerStruct pPsf2TimerStruct, DoWorkEventArgs e)
        {
            maxFiles = FileUtil.GetFileCount(pPsf2TimerStruct.sourcePaths);

            foreach (string path in pPsf2TimerStruct.sourcePaths)
            {
                if (File.Exists(path))
                {
                    if (!CancellationPending)
                    {
                        this.timePsf2FromFile(path, e);
                    }
                    else
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                else if (Directory.Exists(path))
                {
                    this.timePsf2sFromDirectory(path, e);

                    if (CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }            
            return;
        }

        private void timePsf2FromFile(string pPath, DoWorkEventArgs e)
        {
            string outputSqFileName = null;
            string[] libPaths;            
            string[] sqFiles;
            string[] iniFiles;
            Psf2.Psf2IniSqIrxStruct psf2IniStruct;
            
            string filePath;
            string fileDir;
            string fileName;
            string outputDir;
            string libOutputDir;

            string unpkOutput = null;
            string unpkError = null;

            Ps2SequenceData.Ps2SqTimingStruct psf2Time;
            int minutes;
            double seconds;
            int sequenceNumber = 0;
            
            StringBuilder batchFile = new StringBuilder();
            string batchFilePath;

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
                                outputDir = XsfUtil.UnpackPsf2(filePath, ref unpkOutput, ref unpkError);

                                // parse ini
                                iniFiles = Directory.GetFiles(outputDir, "PSF2.INI", SearchOption.AllDirectories);
                                using (FileStream iniFs = File.Open(iniFiles[0], FileMode.Open, FileAccess.Read))
                                {
                                    // parse ini file to get SQ info
                                    psf2IniStruct = Psf2.ParseClsIniFile(iniFs);
                                }

                                // check for libs
                                libPaths = psf2File.GetLibPathArray();

                                // copy the SQ file out (should only be one)
                                sqFiles = Directory.GetFiles(outputDir, psf2IniStruct.SqFileName, SearchOption.AllDirectories);

                                if (sqFiles.Length > 0)
                                {
                                    if (!String.IsNullOrEmpty(psf2IniStruct.SequenceNumber))
                                    {
                                        sequenceNumber = int.Parse(psf2IniStruct.SequenceNumber);
                                        outputSqFileName = String.Format("{0}_n={1}.SQ", outputDir, psf2IniStruct.SequenceNumber);
                                    }
                                    else
                                    {
                                        outputSqFileName = String.Format("{0}.SQ", outputDir);                                        
                                    }
                                    File.Copy(sqFiles[0], outputSqFileName, true);
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
                                            libOutputDir = XsfUtil.UnpackPsf2(libPath, ref unpkOutput, ref unpkError);                                                                                        
                                            extractedLibHash.Add(libPath, libOutputDir);
                                        }

                                        // look for the file in this lib
                                        sqFiles = Directory.GetFiles(libOutputDir, psf2IniStruct.SqFileName, SearchOption.AllDirectories);

                                        if (sqFiles.Length > 0)
                                        {
                                            if (!String.IsNullOrEmpty(psf2IniStruct.SequenceNumber))
                                            {
                                                sequenceNumber = int.Parse(psf2IniStruct.SequenceNumber);
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

                                // get time and add to script
                                if (!String.IsNullOrEmpty(outputSqFileName))
                                {
                                    psf2Time = XsfUtil.GetTimeForPsf2File(outputSqFileName, sequenceNumber);
                                    
                                    File.Delete(outputSqFileName);  // delete SQ file
                                    
                                    minutes = (int)(psf2Time.TimeInSeconds / 60d);
                                    seconds = (psf2Time.TimeInSeconds - (minutes * 60));
                                    seconds = Math.Ceiling(seconds);

                                    if (seconds > 59)
                                    {
                                        minutes++;
                                        seconds -= 60;
                                    }

                                    batchFile.AppendFormat("psfpoint.exe -length=\"{0}:{1}\" -fade=\"{2}\" \"{3}\"",
                                        minutes.ToString(), seconds.ToString().PadLeft(2, '0'),
                                        psf2Time.FadeInSeconds.ToString(), Path.GetFileName(pPath));
                                    batchFile.Append(Environment.NewLine);

                                    batchFilePath = Path.Combine(Path.GetDirectoryName(pPath), BATCH_FILE_NAME);

                                    if (!File.Exists(batchFilePath))
                                    {
                                        using (FileStream cfs = File.Create(batchFilePath)) { };                                        
                                    }

                                    using (StreamWriter sw = new StreamWriter(File.Open(batchFilePath, FileMode.Append, FileAccess.Write)))
                                    {
                                        sw.Write(batchFile.ToString());
                                    }

                                    // report warnings
                                    if (!String.IsNullOrEmpty(psf2Time.Warnings))
                                    {
                                        this.progressStruct.Clear();
                                        progressStruct.genericMessage = String.Format("{0}{1}  WARNINGS{2}    {3}", pPath, Environment.NewLine, Environment.NewLine, psf2Time.Warnings);
                                        ReportProgress(progress, progressStruct);
                                    }

                                }
                            }
                            catch (Exception ex)
                            {
                                this.progressStruct.Clear();
                                progressStruct.errorMessage = String.Format("Error processing <{0}>.  Error received: ", pPath) + ex.Message + Environment.NewLine;
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

        private void timePsf2sFromDirectory(string pPath, DoWorkEventArgs e)
        {
            foreach (string d in Directory.GetDirectories(pPath))
            {
                if (!CancellationPending)
                {
                    this.timePsf2sFromDirectory(d, e);
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
                    this.timePsf2FromFile(f, e);
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
            Psf2TimerStruct psf2SqExtractorStruct = (Psf2TimerStruct)e.Argument;
            this.timePsf2s(psf2SqExtractorStruct, e);

            // delete lib folders
            foreach (string k in extractedLibHash.Keys)
            {
                if (Directory.Exists(extractedLibHash[k]))
                {
                    Directory.Delete(extractedLibHash[k], true);
                }
            }
        }        
    }
}
