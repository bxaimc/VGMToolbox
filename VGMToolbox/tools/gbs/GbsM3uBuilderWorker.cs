using System.ComponentModel;

using VGMToolbox.format.util;
using VGMToolbox.plugin;

namespace VGMToolbox.tools.gbs
{
    class GbsM3uBuilderWorker : AVgmtDragAndDropWorker
    {
        public struct GbsM3uWorkerStruct : IVgmtWorkerStruct
        {
            public bool onePlaylistPerFile;

            private string[] sourcePaths;
            public string[] SourcePaths
            {
                get { return sourcePaths; }
                set { sourcePaths = value; }
            }
        }

        public GbsM3uBuilderWorker() :
            base() { }

        protected override void DoTaskForFile(string pPath, IVgmtWorkerStruct pGbsM3uWorkerStruct, DoWorkEventArgs e)
        {
            GbsM3uWorkerStruct gbsM3uWorkerStruct = (GbsM3uWorkerStruct)pGbsM3uWorkerStruct;
                      
            GbsUtil.GbsM3uBuilderStruct gbsM3uBuilderStruct = new GbsUtil.GbsM3uBuilderStruct();
            gbsM3uBuilderStruct.OnePlaylistPerFile = gbsM3uWorkerStruct.onePlaylistPerFile;
            gbsM3uBuilderStruct.Path = pPath;
            GbsUtil.BuildM3uForFile(gbsM3uBuilderStruct);           
        }                       
    }
}
