using System;
using System.ComponentModel;
using System.IO;

using VGMToolbox.format;
using VGMToolbox.format.util;
using VGMToolbox.util;

namespace VGMToolbox.tools.nsf
{
    class NsfeM3uBuilderWorker :BackgroundWorker
    {
        private int fileCount = 0;
        private int maxFiles = 0;
        private Constants.ProgressStruct progressStruct;

        public struct NsfeM3uBuilderStruct
        {
            public string[] pPaths;
            public int totalFiles;
            public bool onePlaylistPerFile;
        }

        public NsfeM3uBuilderWorker()
        {
            fileCount = 0;
            maxFiles = 0;
            progressStruct = new Constants.ProgressStruct();
            
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
        }

        private void buildM3uFiles(NsfeM3uBuilderStruct pNsfeM3uBuilderStruct, DoWorkEventArgs e)
        {
            foreach (string path in pNsfeM3uBuilderStruct.pPaths)
            {                
                if (File.Exists(path))
                {
                    if (!CancellationPending)
                    {
                        this.buildM3uForFile(path, pNsfeM3uBuilderStruct.onePlaylistPerFile, e);
                    }
                    else 
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                else if (Directory.Exists(path))
                {
                    this.buildM3usForDirectory(path, pNsfeM3uBuilderStruct.onePlaylistPerFile, e);

                    if (CancellationPending)
                    {
                        e.Cancel = true;
                        return;                        
                    }
                }                               
            }

            return;
        }

        private void buildM3usForDirectory(string pPath, bool pOnePlaylistPerFile, DoWorkEventArgs e)
        {
            foreach (string d in Directory.GetDirectories(pPath))
            {
                if (!CancellationPending)
                {
                    this.buildM3usForDirectory(d, pOnePlaylistPerFile, e);
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
                    this.buildM3uForFile(f, pOnePlaylistPerFile, e);
                }
                else
                {
                    e.Cancel = true;
                    break;
                }
            }                
        }

        private void buildM3uForFile(string pPath, bool pOnePlaylistPerFile, DoWorkEventArgs e)
        {
            // Report Progress
            int progress = (++fileCount * 100) / maxFiles;
            this.progressStruct.Clear();
            this.progressStruct.filename = pPath;
            ReportProgress(progress, this.progressStruct);
          
            try
            {
                NsfUtil.NsfM3uBuilderStruct nsfM3uBuilderStruct = new NsfUtil.NsfM3uBuilderStruct();
                nsfM3uBuilderStruct.OnePlaylistPerFile = pOnePlaylistPerFile;
                nsfM3uBuilderStruct.Path = pPath;
                NsfUtil.BuildM3uForFile(nsfM3uBuilderStruct);                
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
            NsfeM3uBuilderStruct nsfeM3uBuilderStruct = (NsfeM3uBuilderStruct)e.Argument;
            maxFiles = nsfeM3uBuilderStruct.totalFiles;

            this.buildM3uFiles(nsfeM3uBuilderStruct, e);
        }            
    }
}
