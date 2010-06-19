using System.ComponentModel;

using VGMToolbox.plugin;
using VGMToolbox.util;

namespace VGMToolbox.tools.extract
{
    class VfsExtractorWorker : AVgmtDragAndDropWorker, IVgmtBackgroundWorker
    {
        public const string LITTLE_ENDIAN = "Little Endian";
        public const string BIG_ENDIAN = "Big Endian";

        public const string RELATIVE_TO_START_OF_FILE_RECORD = "Start of File Record";
        public const string RELATIVE_TO_END_OF_FILE_RECORD = "End of File Record";

        public struct VfsExtractorStruct : IVgmtWorkerStruct
        {            
            public string[] SourcePaths { set; get; }
            public string HeaderSourcePath { set; get; }
            public string OutputFolderPath { set; get; }

            public VfsExtractionStruct VfsExtractionInformation { set; get; }
        }
        
        public VfsExtractorWorker() : 
            base() 
        {
            this.progressCounterIncrementer = 10;
        }

        protected override void DoTaskForFile(string pPath, IVgmtWorkerStruct pVfsExtractorStruct,
            DoWorkEventArgs e)
        {
            VfsExtractorStruct taskStruct = (VfsExtractorStruct)pVfsExtractorStruct;
            VfsExtractionStruct extractionStruct = taskStruct.VfsExtractionInformation;
            string output;

            ParseFile.ParseVirtualFileSystem(pPath, taskStruct.HeaderSourcePath, taskStruct.OutputFolderPath, extractionStruct, out output, true, true);

            this.outputBuffer.Append(output);           
        }
    }
}
