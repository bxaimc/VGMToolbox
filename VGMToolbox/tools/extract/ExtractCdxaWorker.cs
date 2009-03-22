using System;
using System.ComponentModel;
using System.IO;

using VGMToolbox.format.util;
using VGMToolbox.util;

namespace VGMToolbox.tools.extract
{
    class ExtractCdxaWorker : BackgroundWorker
    {
        private int fileCount = 0;
        private int maxFiles = 0;
        private Constants.ProgressStruct progressStruct;

        public struct ExtractCdxaStruct
        {
            public string[] sourcePaths;
            public bool AddRiffHeader;
            public bool PatchByte0x11;
        }

        public ExtractCdxaWorker()
        {
            fileCount = 0;
            maxFiles = 0;
            progressStruct = new Constants.ProgressStruct();
            
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
        }

        private void extractXaFiles(ExtractCdxaStruct pExtractCdxaStruct, 
            DoWorkEventArgs e)
        {
            this.maxFiles = FileUtil.GetFileCount(pExtractCdxaStruct.sourcePaths);

            foreach (string path in pExtractCdxaStruct.sourcePaths)
            {                
                if (File.Exists(path))
                {
                    if (!CancellationPending)
                    {
                        this.extractXaFromFile(path, pExtractCdxaStruct, e);
                    }
                    else 
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                else if (Directory.Exists(path))
                {
                    this.extractXaFromDirectory(path, pExtractCdxaStruct, e);

                    if (CancellationPending)
                    {
                        e.Cancel = true;
                        return;                        
                    }
                }                               
            }

            return;
        }

        private void extractXaFromDirectory(string pPath, ExtractCdxaStruct pExtractCdxaStruct, 
            DoWorkEventArgs e)
        {
            foreach (string d in Directory.GetDirectories(pPath))
            {
                if (!CancellationPending)
                {
                    this.extractXaFromDirectory(d, pExtractCdxaStruct, e);
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
                    this.extractXaFromFile(f, pExtractCdxaStruct, e);
                }
                else
                {
                    e.Cancel = true;
                    break;
                }
            }                
        }

        private void extractXaFromFile(string pPath, ExtractCdxaStruct pExtractCdxaStruct, DoWorkEventArgs e)
        {
            // Report Progress
            int progress = (++fileCount * 100) / maxFiles;
            this.progressStruct.Clear();
            this.progressStruct.filename = pPath;
            ReportProgress(progress, this.progressStruct);

            try
            {
                CdxaUtil.ExtractXaStruct extStruct = new CdxaUtil.ExtractXaStruct();
                extStruct.Path = pPath;
                extStruct.AddRiffHeader = pExtractCdxaStruct.AddRiffHeader;
                extStruct.PatchByte0x11 = pExtractCdxaStruct.PatchByte0x11;

                CdxaUtil.ExtractXaFiles(extStruct);
            }
            catch (Exception ex)
            {
                this.progressStruct.Clear();
                this.progressStruct.errorMessage = String.Format("Error processing <{0}>.  Error received: ", pPath) + ex.Message;
                ReportProgress(progress, this.progressStruct);
            }            
        }    

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            ExtractCdxaStruct extractCdxaStruct = (ExtractCdxaStruct)e.Argument;
            this.extractXaFiles(extractCdxaStruct, e);
        }    
    }
}
