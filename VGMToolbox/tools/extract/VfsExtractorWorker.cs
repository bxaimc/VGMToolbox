using System.ComponentModel;

using VGMToolbox.plugin;

namespace VGMToolbox.tools.extract
{
    class VfsExtractorWorker : AVgmtDragAndDropWorker, IVgmtBackgroundWorker
    {
        public const string LITTLE_ENDIAN = "Little Endian";
        public const string BIG_ENDIAN = "Big Endian";
        
        public struct VfsExtractorStruct : IVgmtWorkerStruct
        {
            public string[] SourcePaths { set; get; }

            // total files
            public bool UseFileCountOffset { set; get; }     // user entered value
            public string FileCountValue { set; get; }
            public string FileCountValueOffset { set; get; } // file count is in VFS
            public string FileCountValueLength { set; get; }
            public bool FileCountValueIsLittleEndian { set; get; }
            public string FileCountEndOffset { set; get; }

            // file information
            public string FileRecordsStartOffset { set; get; }
            public string FileRecordSize { set; get; }

            public bool UsePreviousFilesSizeToDetermineOffset { set; get; }
            public string BeginCuttingFilesAtOffset { set; get; }

            public string FileRecordOffsetOffset { set; get; }
            public string FileRecordOffsetLength { set; get; }
            public bool FileRecordOffsetIsLittleEndian { set; get; }
            public bool UseFileRecordOffsetMultiplier { set; get; }
            public string FileRecordOffsetMultiplier { set; get; }

            public string FileRecordLengthOffset { set; get; }
            public string FileRecordLengthLength { set; get; }
            public bool FileRecordLengthIsLittleEndian { set; get; }
            public bool UseLocationOfNextFileToDetermineLength { set; get; }

            public string FileRecordNameOffset { set; get; }
            public string FileRecordNameLength { set; get; }
            public bool FileRecordNameIsPresent { set; get; }            

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



            // this.outputBuffer.Append(output);           
        }
    }
}
