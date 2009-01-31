using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

using VGMToolbox.format;
using VGMToolbox.util;

namespace VGMToolbox.tools.nds
{
    class SdatFinderWorker : BackgroundWorker
    {
        private int fileCount = 0;
        private int maxFiles = 0;
        private Constants.ProgressStruct progressStruct;

        public struct SdatFinderStruct
        {
            public string[] sourcePaths;
        }

        public SdatFinderWorker()
        {
            fileCount = 0;
            maxFiles = 0;
            progressStruct = new Constants.ProgressStruct();
            
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
        }

        private void findSdats(SdatFinderStruct pSdatFinderStruct, 
            DoWorkEventArgs e)
        {
            this.maxFiles = FileUtil.GetFileCount(pSdatFinderStruct.sourcePaths);

            foreach (string path in pSdatFinderStruct.sourcePaths)
            {                
                if (File.Exists(path))
                {
                    if (!CancellationPending)
                    {
                        this.findSdatsInFile(path, e);
                    }
                    else 
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                else if (Directory.Exists(path))
                {
                    this.findSdatsInDirectory(path, e);

                    if (CancellationPending)
                    {
                        e.Cancel = true;
                        return;                        
                    }
                }                               
            }

            return;
        }

        private void findSdatsInDirectory(string pPath, DoWorkEventArgs e)
        {
            foreach (string d in Directory.GetDirectories(pPath))
            {
                if (!CancellationPending)
                {
                    this.findSdatsInDirectory(d, e);
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
                    this.findSdatsInFile(f, e);
                }
                else
                {
                    e.Cancel = true;
                    break;
                }
            }                
        }

        private void findSdatsInFile(string pPath, DoWorkEventArgs e)
        {
            string filePrefix = Path.GetFileNameWithoutExtension(pPath);
            string outputPath;

            int sdatIndex = 0;
            long sdatOffset;
            long previousOffset;

            byte[] sdatSizeBytes;
            int sdatSize;
            
            // Report Progress
            int progress = (++fileCount * 100) / maxFiles;
            this.progressStruct.Clear();
            this.progressStruct.filename = pPath;
            ReportProgress(progress, this.progressStruct);
         
            /*
            try
            {
                FileStream fs = File.OpenRead(pPath);
                Type dataType = FormatUtil.getObjectType(fs);

                if (dataType != null && dataType.Name.Equals("Sdat"))
                {
                    string fileName = Path.GetFileNameWithoutExtension(pPath);
                    string destinationPath =
                        Path.Combine(Path.GetDirectoryName(pPath), fileName);

                    Sdat sdat = new Sdat();

                    try
                    {
                        sdat.Initialize(fs, pPath);
                        sdat.ExtractSseqs(fs, destinationPath);
                        sdat.ExtractStrms(fs, destinationPath);
                        sdat.BuildSmap(destinationPath, fileName);
                    }
                    catch (Exception ex)
                    {
                        this.progressStruct.Clear();
                        this.progressStruct.errorMessage = String.Format("Error processing <{0}>.  Error received: ", pPath) + ex.Message;
                        ReportProgress(progress, this.progressStruct);                        
                    }
                }
                fs.Close();
                fs.Dispose();        
            }
            catch (Exception ex)
            {
                this.progressStruct.Clear();
                this.progressStruct.errorMessage = String.Format("Error processing <{0}>.  Error received: ", pPath) + ex.Message;
                ReportProgress(progress, this.progressStruct);
            }            
             * 
             */ 
        }    

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            SdatFinderStruct sdatFinderStruct = (SdatFinderStruct)e.Argument;
            this.findSdats(sdatFinderStruct, e);
        }    
    }
}
