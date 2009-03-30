using System;
using System.ComponentModel;

using VGMToolbox.plugin;
using VGMToolbox.util;

namespace VGMToolbox.tools.extract
{
    class OffsetFinderWorker : AVgmtWorker
    {
        public const string LITTLE_ENDIAN = "Little Endian";
        public const string BIG_ENDIAN = "Big Endian";
        
        public struct OffsetFinderStruct : IVgmtWorkerStruct
        {
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

            private string[] sourcePaths;
            public string[] SourcePaths
            {
                get { return sourcePaths; }
                set { sourcePaths = value; }
            }
        }

        public OffsetFinderWorker() : 
            base() {}
        
        protected override void doTaskForFile(string pPath, IVgmtWorkerStruct pOffsetFinderStruct,
            DoWorkEventArgs e)
        {
            OffsetFinderStruct offsetFinderStruct = (OffsetFinderStruct) pOffsetFinderStruct;
            
            ParseFile.FindOffsetStruct findOffsetStruct;
            findOffsetStruct.searchString = offsetFinderStruct.searchString;
            findOffsetStruct.treatSearchStringAsHex = offsetFinderStruct.treatSearchStringAsHex;

            findOffsetStruct.cutFile = offsetFinderStruct.cutFile;
            findOffsetStruct.searchStringOffset = offsetFinderStruct.searchStringOffset;
            findOffsetStruct.cutSize = offsetFinderStruct.cutSize;
            findOffsetStruct.cutSizeOffsetSize = offsetFinderStruct.cutSizeOffsetSize;
            findOffsetStruct.isCutSizeAnOffset = offsetFinderStruct.isCutSizeAnOffset;
            findOffsetStruct.outputFileExtension = offsetFinderStruct.outputFileExtension;
            findOffsetStruct.isLittleEndian = offsetFinderStruct.isLittleEndian;
            findOffsetStruct.useTerminatorForCutsize = offsetFinderStruct.useTerminatorForCutsize;
            findOffsetStruct.terminatorString = offsetFinderStruct.terminatorString;
            findOffsetStruct.treatTerminatorStringAsHex = offsetFinderStruct.treatTerminatorStringAsHex;
            findOffsetStruct.includeTerminatorLength = offsetFinderStruct.includeTerminatorLength;
            findOffsetStruct.extraCutSizeBytes = offsetFinderStruct.extraCutSizeBytes;

            string output = String.Empty;
            ParseFile.FindOffsetAndCutFile(pPath, findOffsetStruct, ref output);

            this.progressStruct.Clear();
            this.progressStruct.genericMessage = output;
            ReportProgress(Constants.PROGRESS_MSG_ONLY, this.progressStruct);            
        }    

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            OffsetFinderStruct offsetFinderStruct = (OffsetFinderStruct)e.Argument;

            if (!String.IsNullOrEmpty(offsetFinderStruct.outputFileExtension) &&
                !offsetFinderStruct.outputFileExtension.StartsWith("."))
            {
                offsetFinderStruct.outputFileExtension = "." + offsetFinderStruct.outputFileExtension;
            }

            base.OnDoWork(e);
        }    
    }
}
