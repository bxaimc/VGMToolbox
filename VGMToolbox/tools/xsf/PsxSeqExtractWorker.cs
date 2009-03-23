using System;
using System.ComponentModel;
using System.IO;

using VGMToolbox.format.util;
using VGMToolbox.util;

namespace VGMToolbox.tools.xsf
{
    public partial class PsxSeqExtractWorker : BackgroundWorker
    {
        private int fileCount = 0;
        private int maxFiles = 0;

        public struct PsxSeqExtractStruct
        {
            public string[] sourcePaths;
        }

        public PsxSeqExtractWorker()
        {
            fileCount = 0;
            maxFiles = 0;
            
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
        }

        private void extractSeqs(PsxSeqExtractStruct pPsxSeqExtractStruct, DoWorkEventArgs e)
        {
            maxFiles = FileUtil.GetFileCount(pPsxSeqExtractStruct.sourcePaths);

            foreach (string path in pPsxSeqExtractStruct.sourcePaths)
            {
                if (File.Exists(path))
                {
                    if (!CancellationPending)
                    {
                        this.extractSeqsFromFile(path, e);
                    }
                    else
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                else if (Directory.Exists(path))
                {
                    this.extractSeqsFromDirectory(path, e);

                    if (CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }            
            return;
        }

        private void extractSeqsFromFile(string pPath, DoWorkEventArgs e)
        {            
            // Report Progress
            int progress = (++fileCount * 100) / maxFiles;
            Constants.ProgressStruct vProgressStruct = new Constants.ProgressStruct();
            vProgressStruct.newNode = null;
            vProgressStruct.filename = pPath;
            ReportProgress(progress, vProgressStruct);

            try
            {
                XsfUtil.ExtractPsxSequences(pPath);
            }
            catch (Exception ex)
            {
                vProgressStruct = new Constants.ProgressStruct();
                vProgressStruct.newNode = null;
                vProgressStruct.errorMessage = String.Format("Error processing <{0}>.  Error received: ", pPath) + ex.Message;
                ReportProgress(progress, vProgressStruct);
            }
        }    

        private void extractSeqsFromDirectory(string pPath, DoWorkEventArgs e)
        {
            foreach (string d in Directory.GetDirectories(pPath))
            {
                if (!CancellationPending)
                {
                    this.extractSeqsFromDirectory(d, e);
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
                    this.extractSeqsFromFile(f, e);
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
            PsxSeqExtractStruct psxSeqExtractStruct = (PsxSeqExtractStruct)e.Argument;

            this.extractSeqs(psxSeqExtractStruct, e);
        }        
    }
}
