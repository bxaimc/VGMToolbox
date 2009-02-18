using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.util;

namespace VGMToolbox.tools.xsf
{
    class SsfSeqTonExtractorWorker : BackgroundWorker
    {
        private static readonly string SSFTOOL_FOLDER_PATH =
            Path.GetFullPath(Path.Combine(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "external"), "ssf"));
        private static readonly string SEQEXT_SOURCE_PATH = Path.Combine(SSFTOOL_FOLDER_PATH, "seqext.py");
        private static readonly string TONEXT_SOURCE_PATH = Path.Combine(SSFTOOL_FOLDER_PATH, "tonext.py");
        private static readonly string WORKING_FOLDER_SEEK =
            Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "working_ssf_seqtonext"));

        private int fileCount = 0;
        private int maxFiles = 0;
        Constants.ProgressStruct progressStruct;


        private readonly string PROGRAM_PATH =
            Path.Combine(Path.Combine(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "external"), "psf2"), "unpkpsf2.exe");

        public struct SsfSeqTonExtractorStruct
        {
            public string[] sourcePaths;
            public bool extractToSubFolder;
        }

        public SsfSeqTonExtractorWorker()
        {
            fileCount = 0;
            maxFiles = 0;
            progressStruct = new Constants.ProgressStruct();

            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
        }

        private void extractSsfSeqTon(SsfSeqTonExtractorStruct pSsfSeqTonExtractorStruct, DoWorkEventArgs e)
        {
            maxFiles = FileUtil.GetFileCount(pSsfSeqTonExtractorStruct.sourcePaths);

            foreach (string path in pSsfSeqTonExtractorStruct.sourcePaths)
            {
                if (File.Exists(path))
                {
                    if (!CancellationPending)
                    {
                        this.extractSsfSeqTonFromFile(path, pSsfSeqTonExtractorStruct, e);
                    }
                    else
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                else if (Directory.Exists(path))
                {
                    this.extractSsfSeqTonFromDirectory(path, pSsfSeqTonExtractorStruct, e);

                    if (CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }            
            return;
        }

        private void extractSsfSeqTonFromFile(string pPath, SsfSeqTonExtractorStruct pSsfSeqTonExtractorStruct, DoWorkEventArgs e)
        {
            string destinationPath;
            string outputFolder;
            string[] extractedFiles;
            string arguments;
            bool isSuccess;
            string seekOutput;
            Process extractionProcess = null;

            // Report Progress
            int progress = (++fileCount * 100) / maxFiles;
            progressStruct.Clear();
            progressStruct.filename = pPath;
            ReportProgress(progress, progressStruct);

            try
            {
                // create temp folder
                if (!Directory.Exists(WORKING_FOLDER_SEEK))
                {
                    Directory.CreateDirectory(WORKING_FOLDER_SEEK);
                }

                // copy scripts
                string seqextScriptPath = Path.Combine(WORKING_FOLDER_SEEK, Path.GetFileName(SEQEXT_SOURCE_PATH));
                string tonextScriptPath = Path.Combine(WORKING_FOLDER_SEEK, Path.GetFileName(TONEXT_SOURCE_PATH));

                File.Copy(SEQEXT_SOURCE_PATH, seqextScriptPath, true);
                File.Copy(TONEXT_SOURCE_PATH, tonextScriptPath, true);

                // copy to working dir
                destinationPath = Path.Combine(WORKING_FOLDER_SEEK, Path.GetFileName(pPath));
                File.Copy(pPath, destinationPath, true);

                // extract SEQ
                arguments = String.Format(" {0} {1} .",
                    Path.GetFileName(seqextScriptPath), Path.GetFileName(pPath));
                extractionProcess = new Process();
                extractionProcess.StartInfo = new ProcessStartInfo("python.exe", arguments);
                extractionProcess.StartInfo.WorkingDirectory = WORKING_FOLDER_SEEK;
                extractionProcess.StartInfo.UseShellExecute = false;
                extractionProcess.StartInfo.CreateNoWindow = true;
                extractionProcess.StartInfo.RedirectStandardOutput = true;
                isSuccess = extractionProcess.Start();
                seekOutput = extractionProcess.StandardOutput.ReadToEnd();
                extractionProcess.WaitForExit();
                extractionProcess.Close();

                this.progressStruct.Clear();
                this.progressStruct.genericMessage = String.Format("[SEQEXT - {0}]", Path.GetFileName(pPath)) +
                    Environment.NewLine + seekOutput;
                this.ReportProgress(Constants.PROGRESS_MSG_ONLY, this.progressStruct);


                // extract TONE
                arguments = String.Format(" {0} {1} .",
                    Path.GetFileName(tonextScriptPath), Path.GetFileName(pPath));
                extractionProcess = new Process();
                extractionProcess.StartInfo = new ProcessStartInfo("python.exe", arguments);
                extractionProcess.StartInfo.WorkingDirectory = WORKING_FOLDER_SEEK;
                extractionProcess.StartInfo.UseShellExecute = false;
                extractionProcess.StartInfo.CreateNoWindow = true;
                extractionProcess.StartInfo.RedirectStandardOutput = true;
                isSuccess = extractionProcess.Start();
                seekOutput = extractionProcess.StandardOutput.ReadToEnd();
                extractionProcess.WaitForExit();

                extractionProcess.Close();
                extractionProcess.Dispose();

                this.progressStruct.Clear();
                this.progressStruct.genericMessage = String.Format("[TONEXT - {0}]", Path.GetFileName(pPath)) +
                    Environment.NewLine + seekOutput + Environment.NewLine;
                this.ReportProgress(Constants.PROGRESS_MSG_ONLY, this.progressStruct);

                // delete the originals
                File.Delete(destinationPath);
                File.Delete(seqextScriptPath);
                File.Delete(tonextScriptPath);

                // copy any output to output folder
                if (pSsfSeqTonExtractorStruct.extractToSubFolder)
                {
                    outputFolder = Path.Combine(Path.GetDirectoryName(pPath), Path.GetFileNameWithoutExtension(pPath));
                }
                else
                {
                    outputFolder = Path.GetDirectoryName(pPath);
                }
                
                extractedFiles = Directory.GetFiles(WORKING_FOLDER_SEEK);

                if (extractedFiles.Length > 0)
                {
                    if (!Directory.Exists(outputFolder))
                    {
                        Directory.CreateDirectory(outputFolder);
                    }

                    foreach (string file in extractedFiles)
                    {
                        File.Copy(file, Path.Combine(outputFolder, Path.GetFileName(file)), true);
                    }
                }
            }
            catch (Exception ex)
            {
                this.progressStruct.Clear();
                progressStruct.errorMessage = String.Format("Error processing <{0}>.  Error received: ", pPath) + ex.Message;
                ReportProgress(progress, progressStruct);
            }
            finally
            {
                if (extractionProcess != null)
                {
                    extractionProcess.Dispose();
                }

                if (Directory.Exists(WORKING_FOLDER_SEEK))
                {
                    Directory.Delete(WORKING_FOLDER_SEEK, true);
                }
            }
        }    

        private void extractSsfSeqTonFromDirectory(string pPath, SsfSeqTonExtractorStruct pSsfSeqTonExtractorStruct, DoWorkEventArgs e)
        {
            foreach (string d in Directory.GetDirectories(pPath))
            {
                if (!CancellationPending)
                {
                    this.extractSsfSeqTonFromDirectory(d, pSsfSeqTonExtractorStruct, e);
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
                    this.extractSsfSeqTonFromFile(f, pSsfSeqTonExtractorStruct, e);
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
            SsfSeqTonExtractorStruct ssfSeqTonExtractorStruct = (SsfSeqTonExtractorStruct)e.Argument;
            this.extractSsfSeqTon(ssfSeqTonExtractorStruct, e);
        }         
    
    }
}
