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
            public string SourceFormat { set; get; }
            public string[] SourcePaths { set; get; }
        }

        public MpegDemuxWorker() :
            base() { }

        protected override void DoTaskForFile(string path, IVgmtWorkerStruct pMpegDemuxStruct, DoWorkEventArgs e)
        {
            MpegDemuxStruct demuxStruct = (MpegDemuxStruct)pMpegDemuxStruct;

            switch (demuxStruct.SourceFormat)
            {
                case "MPEG":
                    MpegStream mpegStream = new MpegStream(path);
                    mpegStream.DemultiplexStreams();
                    break;

                case "PMF":
                    SonyPmfStream pmfStream = new SonyPmfStream(path);
                    pmfStream.DemultiplexStreams();
                    break;
                
                case "PSS":
                    SonyPssStream sps = new SonyPssStream(path);
                    sps.DemultiplexStreams();
                    break;

                case "SFD":
                    SofdecStream ss = new SofdecStream(path);
                    ss.DemultiplexStreams();
                    break;

                default:
                    throw new FormatException("Source format not defined.");
            }            
        }               
    }
}
