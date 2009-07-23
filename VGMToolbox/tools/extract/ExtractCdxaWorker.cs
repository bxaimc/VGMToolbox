using System.ComponentModel;

using VGMToolbox.format.util;
using VGMToolbox.plugin;

namespace VGMToolbox.tools.extract
{
    class ExtractCdxaWorker : AVgmtDragAndDropWorker, IVgmtBackgroundWorker
    {
        public struct ExtractCdxaStruct : IVgmtWorkerStruct
        {
            public bool AddRiffHeader;
            public bool PatchByte0x11;
            public uint SilentFramesCount;

            private string[] sourcePaths;
            public string[] SourcePaths
            {
                get { return sourcePaths; }
                set { sourcePaths = value; }
            }
        }

        public ExtractCdxaWorker():
            base() { }

        protected override void DoTaskForFile(string pPath, IVgmtWorkerStruct pExtractCdxaStruct, DoWorkEventArgs e)
        {
            ExtractCdxaStruct extractCdxaStruct = (ExtractCdxaStruct) pExtractCdxaStruct;
            
            ExtractXaStruct extStruct = new ExtractXaStruct();
            extStruct.Path = pPath;
            extStruct.AddRiffHeader = extractCdxaStruct.AddRiffHeader;
            extStruct.PatchByte0x11 = extractCdxaStruct.PatchByte0x11;
            extStruct.SilentFramesCount = extractCdxaStruct.SilentFramesCount;

            CdxaUtil.ExtractXaFiles(extStruct);            
        }       
    }
}
