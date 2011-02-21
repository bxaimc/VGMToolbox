using System;
using System.ComponentModel;
using System.IO;

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
                    int mpegType = MpegStream.GetMpegStreamType(path);
                    
                    switch (mpegType)
                    {
                        case 1:
                            Mpeg1Stream mpeg1Stream = new Mpeg1Stream(path);
                            mpeg1Stream.DemultiplexStreams();
                            break;
                        case 2:
                            Mpeg2Stream mpeg2Stream = new Mpeg2Stream(path);
                            mpeg2Stream.DemultiplexStreams();
                            break;
                        default:
                            throw new FormatException(String.Format("Unsupported MPEG type, for file: {0}", Path.GetFileName(path)));
                    }
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
