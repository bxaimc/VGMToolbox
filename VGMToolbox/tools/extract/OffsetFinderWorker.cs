using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

using VGMToolbox.util;

namespace VGMToolbox.tools.extract
{
    class OffsetFinderWorker : BackgroundWorker
    {        
        private int fileCount = 0;
        private int maxFiles = 0;
        private Constants.ProgressStruct progressStruct;

        public struct OffsetFinderStruct
        {
            public string[] sourcePaths;
            public string searchString;
            public bool treatSearchStringAsHex;
        }

        public OffsetFinderWorker()
        {
            fileCount = 0;
            maxFiles = 0;
            progressStruct = new Constants.ProgressStruct();
            
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
        }

        private void findOffsets(OffsetFinderStruct pOffsetFinderStruct, 
            DoWorkEventArgs e)
        {
            this.maxFiles = FileUtil.GetFileCount(pOffsetFinderStruct.sourcePaths);

            foreach (string path in pOffsetFinderStruct.sourcePaths)
            {                
                if (File.Exists(path))
                {
                    if (!CancellationPending)
                    {
                        this.findOffsetsInFile(path, pOffsetFinderStruct, e);
                    }
                    else 
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                else if (Directory.Exists(path))
                {
                    this.findOffsetsInDirectory(path, pOffsetFinderStruct, e);

                    if (CancellationPending)
                    {
                        e.Cancel = true;
                        return;                        
                    }
                }                               
            }

            return;
        }

        private void findOffsetsInDirectory(string pPath, OffsetFinderStruct pOffsetFinderStruct, 
            DoWorkEventArgs e)
        {
            foreach (string d in Directory.GetDirectories(pPath))
            {
                if (!CancellationPending)
                {
                    this.findOffsetsInDirectory(d, pOffsetFinderStruct, e);
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
                    this.findOffsetsInFile(f, pOffsetFinderStruct, e);
                }
                else
                {
                    e.Cancel = true;
                    break;
                }
            }                
        }

        private void findOffsetsInFile(string pPath, OffsetFinderStruct pOffsetFinderStruct,
            DoWorkEventArgs e)
        {
            int i;
            int j = 0;
            byte[] searchBytes;
            System.Text.Encoding enc = System.Text.Encoding.ASCII;

            long offset;
            long previousOffset;
            
            // Report Progress
            int progress = (++fileCount * 100) / maxFiles;
            this.progressStruct.Clear();
            this.progressStruct.filename = pPath;
            ReportProgress(progress, this.progressStruct);

            try
            {
                if (pOffsetFinderStruct.treatSearchStringAsHex)
                {
                    searchBytes = new byte[pOffsetFinderStruct.searchString.Length / 2];

                    // convert the search string to bytes
                    for (i = 0; i < pOffsetFinderStruct.searchString.Length; i += 2)
                    {
                        searchBytes[j] = BitConverter.GetBytes(Int16.Parse(pOffsetFinderStruct.searchString.Substring(i, 2), System.Globalization.NumberStyles.AllowHexSpecifier))[0];
                        j++;
                    }
                }
                else
                {
                    searchBytes = enc.GetBytes(pOffsetFinderStruct.searchString);
                }
                
                using (FileStream fs = File.Open(Path.GetFullPath(pPath), FileMode.Open, FileAccess.Read))
                {
                    this.progressStruct.Clear();
                    this.progressStruct.genericMessage = String.Format("[{0}]", pPath) + Environment.NewLine;
                    ReportProgress(Constants.PROGRESS_MSG_ONLY, this.progressStruct);
                    
                    previousOffset = 0;

                    while ((offset = ParseFile.GetNextOffset(fs, previousOffset, searchBytes)) != -1)
                    {
                        this.progressStruct.Clear();
                        this.progressStruct.genericMessage = String.Format("  String found at: 0x{0}", offset.ToString("X8")) + Environment.NewLine;
                        ReportProgress(Constants.PROGRESS_MSG_ONLY, this.progressStruct);

                        previousOffset = offset + searchBytes.Length;
                    }
                }
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
            OffsetFinderStruct offsetFinderStruct = (OffsetFinderStruct)e.Argument;
            this.findOffsets(offsetFinderStruct, e);
        }    
    }
}
