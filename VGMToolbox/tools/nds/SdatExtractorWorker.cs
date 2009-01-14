using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

using VGMToolbox.auditing;
using VGMToolbox.format;
using VGMToolbox.format.sdat;
using VGMToolbox.util;

namespace VGMToolbox.tools.nds
{
    class SdatExtractorWorker : BackgroundWorker
    {
        private int fileCount = 0;
        private int maxFiles = 0;

        public struct SdatExtractorStruct
        {
            public string[] pPaths;
            public int totalFiles;
        }

        public SdatExtractorWorker()
        {
            fileCount = 0;
            maxFiles = 0;
            
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
        }

        private void extractSdats(SdatExtractorStruct pSdatExtractorStruct, 
            DoWorkEventArgs e)
        {
            foreach (string path in pSdatExtractorStruct.pPaths)
            {                
                if (File.Exists(path))
                {
                    if (!CancellationPending)
                    {
                        this.extractSdatFromFile(path, e);
                    }
                    else 
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                else if (Directory.Exists(path))
                {
                    this.extractSdatsFromDirectory(path, e);

                    if (CancellationPending)
                    {
                        e.Cancel = true;
                        return;                        
                    }
                }                               
            }

            return;
        }

        private void extractSdatsFromDirectory(string pPath, DoWorkEventArgs e)
        {
            foreach (string d in Directory.GetDirectories(pPath))
            {
                if (!CancellationPending)
                {
                    this.extractSdatsFromDirectory(d, e);
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
                    this.extractSdatFromFile(f, e);
                    // fileCount++;
                }
                else
                {
                    e.Cancel = true;
                    break;
                }
            }                
        }

        private void extractSdatFromFile(string pPath, DoWorkEventArgs e)
        {
            // Report Progress
            int progress = (++fileCount * 100) / maxFiles;
            Constants.ProgressStruct vProgressStruct = new Constants.ProgressStruct();
            vProgressStruct.newNode = null;
            vProgressStruct.filename = pPath;
            ReportProgress(progress, vProgressStruct);
         
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
                        vProgressStruct = new Constants.ProgressStruct();
                        vProgressStruct.newNode = null;
                        vProgressStruct.errorMessage = String.Format("Error processing <{0}>.  Error received: ", pPath) + ex.Message;
                        ReportProgress(progress, vProgressStruct);                        
                    }
                }
                fs.Close();
                fs.Dispose();        
            }
            catch (Exception ex)
            {
                vProgressStruct = new Constants.ProgressStruct();
                vProgressStruct.newNode = null;
                vProgressStruct.errorMessage = String.Format("Error processing <{0}>.  Error received: ", pPath) + ex.Message;
                ReportProgress(progress, vProgressStruct);
            }            
        }    

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            SdatExtractorStruct sdatExtractorStruct = (SdatExtractorStruct)e.Argument;
            maxFiles = sdatExtractorStruct.totalFiles;

            this.extractSdats(sdatExtractorStruct, e);
        }    
    }
}
