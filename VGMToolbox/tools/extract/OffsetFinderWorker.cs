using System;
using System.ComponentModel;
using System.IO;

using VGMToolbox.plugin;
using VGMToolbox.util;

namespace VGMToolbox.tools.extract
{
    class OffsetFinderWorker : AVgmtDragAndDropWorker, IVgmtBackgroundWorker
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
        
        protected override void DoTaskForFile(string pPath, IVgmtWorkerStruct pOffsetFinderStruct,
            DoWorkEventArgs e)
        {
            OffsetFinderStruct offsetFinderStruct = (OffsetFinderStruct) pOffsetFinderStruct;

            long lastLocation;
            using (FileStream fs = File.OpenRead(pPath))
            {
                lastLocation = ParseFile.GetPreviousOffset(fs, fs.Length, new byte[] { 0x49, 0x45, 0x43, 0x53, 0x73, 0x72, 0x65, 0x56 });
                lastLocation = ParseFile.GetPreviousOffset(fs, lastLocation - 1, new byte[] { 0x49, 0x45, 0x43, 0x53, 0x73, 0x72, 0x65, 0x56 });
                lastLocation = ParseFile.GetPreviousOffset(fs, fs.Length, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
                lastLocation = ParseFile.GetPreviousOffset(fs, lastLocation + 7, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
                lastLocation = ParseFile.GetPreviousOffset(fs, lastLocation - 1, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });

            }
            
            VGMToolbox.util.FindOffsetStruct findOffsetStruct = new VGMToolbox.util.FindOffsetStruct();
            findOffsetStruct.SearchString = offsetFinderStruct.searchString;
            findOffsetStruct.TreatSearchStringAsHex = offsetFinderStruct.treatSearchStringAsHex;

            findOffsetStruct.CutFile = offsetFinderStruct.cutFile;
            findOffsetStruct.SearchStringOffset = offsetFinderStruct.searchStringOffset;
            findOffsetStruct.CutSize = offsetFinderStruct.cutSize;
            findOffsetStruct.CutSizeOffsetSize = offsetFinderStruct.cutSizeOffsetSize;
            findOffsetStruct.IsCutSizeAnOffset = offsetFinderStruct.isCutSizeAnOffset;
            findOffsetStruct.OutputFileExtension = offsetFinderStruct.outputFileExtension;
            findOffsetStruct.IsLittleEndian = offsetFinderStruct.isLittleEndian;
            findOffsetStruct.UseTerminatorForCutSize = offsetFinderStruct.useTerminatorForCutsize;
            findOffsetStruct.TerminatorString = offsetFinderStruct.terminatorString;
            findOffsetStruct.TreatTerminatorStringAsHex = offsetFinderStruct.treatTerminatorStringAsHex;
            findOffsetStruct.IncludeTerminatorLength = offsetFinderStruct.includeTerminatorLength;
            findOffsetStruct.ExtraCutSizeBytes = offsetFinderStruct.extraCutSizeBytes;

            string output;
            ParseFile.FindOffsetAndCutFile(pPath, findOffsetStruct, out output);

            this.progressStruct.Clear();
            this.progressStruct.GenericMessage = output;
            ReportProgress(Constants.ProgressMessageOnly, this.progressStruct);            
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
