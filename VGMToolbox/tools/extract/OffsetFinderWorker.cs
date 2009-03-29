using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.util;

namespace VGMToolbox.tools.extract
{
    class OffsetFinderWorker : BackgroundWorker
    {
        public const string LITTLE_ENDIAN = "Little Endian";
        public const string BIG_ENDIAN = "Big Endian";
        
        private int fileCount = 0;
        private int maxFiles = 0;
        private Constants.ProgressStruct progressStruct;

        public struct OffsetFinderStruct
        {
            public string[] sourcePaths;
            public string searchString;
            public bool treatSearchStringAsHex;

            public bool cutFile;
            public string searchStringOffset;            
            public string cutSize;
            public string cutSizeOffsetSize;
            public bool isCutSizeAnOffset;
            public string outputFileExtension;
            public bool isLittleEndian;

            public bool useTerminatorForCutsize;
            public string terminatorString;
            public bool treatTerminatorStringAsHex;
            public bool includeTerminatorLength;

            public string extraCutSizeBytes;
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

            // Report Progress
            int progress = (++fileCount * 100) / maxFiles;
            this.progressStruct.Clear();
            this.progressStruct.filename = pPath;
            ReportProgress(progress, this.progressStruct);

            try
            {
                ParseFile.FindOffsetStruct findOffsetStruct;
                findOffsetStruct.searchString = pOffsetFinderStruct.searchString;
                findOffsetStruct.treatSearchStringAsHex = pOffsetFinderStruct.treatSearchStringAsHex;

                findOffsetStruct.cutFile = pOffsetFinderStruct.cutFile;
                findOffsetStruct.searchStringOffset = pOffsetFinderStruct.searchStringOffset;
                findOffsetStruct.cutSize = pOffsetFinderStruct.cutSize;
                findOffsetStruct.cutSizeOffsetSize = pOffsetFinderStruct.cutSizeOffsetSize;
                findOffsetStruct.isCutSizeAnOffset = pOffsetFinderStruct.isCutSizeAnOffset;
                findOffsetStruct.outputFileExtension = pOffsetFinderStruct.outputFileExtension;
                findOffsetStruct.isLittleEndian = pOffsetFinderStruct.isLittleEndian;
                findOffsetStruct.useTerminatorForCutsize = pOffsetFinderStruct.useTerminatorForCutsize;
                findOffsetStruct.terminatorString = pOffsetFinderStruct.terminatorString;
                findOffsetStruct.treatTerminatorStringAsHex = pOffsetFinderStruct.treatTerminatorStringAsHex;
                findOffsetStruct.includeTerminatorLength = pOffsetFinderStruct.includeTerminatorLength;
                findOffsetStruct.extraCutSizeBytes = pOffsetFinderStruct.extraCutSizeBytes;

                string output = String.Empty;
                ParseFile.FindOffsetAndCutFile(pPath, findOffsetStruct, ref output);

                this.progressStruct.Clear();
                this.progressStruct.genericMessage = output;
                ReportProgress(Constants.PROGRESS_MSG_ONLY, this.progressStruct);
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

            if (!String.IsNullOrEmpty(offsetFinderStruct.outputFileExtension) &&
                !offsetFinderStruct.outputFileExtension.StartsWith("."))
            {
                offsetFinderStruct.outputFileExtension = "." + offsetFinderStruct.outputFileExtension;
            }

            this.findOffsets(offsetFinderStruct, e);
        }    
    }
}
