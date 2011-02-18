using System;
using System.ComponentModel;

using VGMToolbox.format;
using VGMToolbox.plugin;

namespace VGMToolbox.tools.stream
{
    class MpegDemuxWorker : AVgmtDragAndDropWorker, IVgmtBackgroundWorker
    {
        
        
        public struct MpegDemuxStruct : IVgmtWorkerStruct
        {
            public MpegStream.MpegSupportedDataFormats SourceFormat { set; get; }
            public string[] SourcePaths { set; get; }
        }

        public MpegDemuxWorker() :
            base() { }

        protected override void DoTaskForFile(string path, IVgmtWorkerStruct pMpegDemuxStruct, DoWorkEventArgs e)
        {
            MpegDemuxStruct demuxStruct = (MpegDemuxStruct)pMpegDemuxStruct;

            switch (demuxStruct.SourceFormat)
            { 
                case MpegStream.MpegSupportedDataFormats.SofdecVideo:
                    SofdecStream ss = new SofdecStream(path);
                    ss.DemultiplexStreams();
                    break;
                default:
                    throw new FormatException("Source format not defined.");
            }            
        }               
    }
}
