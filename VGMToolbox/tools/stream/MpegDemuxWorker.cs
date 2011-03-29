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
            public bool AddHeader { set; get; }
            public bool ExtractAudio { set; get; }
            public bool ExtractVideo { set; get; }
            
            public string[] SourcePaths { set; get; }
        }

        public MpegDemuxWorker() :
            base() { }

        protected override void DoTaskForFile(string path, IVgmtWorkerStruct pMpegDemuxStruct, DoWorkEventArgs e)
        {
            MpegDemuxStruct demuxStruct = (MpegDemuxStruct)pMpegDemuxStruct;
            MpegStream.DemuxOptionsStruct demuxOptions = new MpegStream.DemuxOptionsStruct();

            demuxOptions.AddHeader = demuxStruct.AddHeader;
            demuxOptions.ExtractAudio = demuxStruct.ExtractAudio;
            demuxOptions.ExtractVideo = demuxStruct.ExtractVideo;

            switch (demuxStruct.SourceFormat)
            {
                case "DVD Video":
                    DvdVideoStream dvdStream = new DvdVideoStream(path);
                    dvdStream.DemultiplexStreams(demuxOptions);
                    break;
                case "MPEG":
                    int mpegType = MpegStream.GetMpegStreamType(path);
                    
                    switch (mpegType)
                    {
                        case 1:
                            Mpeg1Stream mpeg1Stream = new Mpeg1Stream(path);
                            mpeg1Stream.DemultiplexStreams(demuxOptions);
                            break;
                        case 2:
                            Mpeg2Stream mpeg2Stream = new Mpeg2Stream(path);
                            mpeg2Stream.DemultiplexStreams(demuxOptions);
                            break;
                        default:
                            throw new FormatException(String.Format("Unsupported MPEG type, for file: {0}", Path.GetFileName(path)));
                    }
                    break;
                case "PAM":
                    SonyPamStream pamStream = new SonyPamStream(path);
                    pamStream.DemultiplexStreams(demuxOptions);
                    break;
                
                case "PMF":
                    SonyPmfStream pmfStream = new SonyPmfStream(path);
                    pmfStream.DemultiplexStreams(demuxOptions);
                    break;
                
                case "PSS":
                    SonyPssStream sps = new SonyPssStream(path);
                    sps.DemultiplexStreams(demuxOptions);
                    break;

                case "SFD":
                    SofdecStream ss = new SofdecStream(path);
                    ss.DemultiplexStreams(demuxOptions);
                    break;

                case "USM":
                    CriUsmStream cus = new CriUsmStream(path);
                    cus.DemultiplexStreams(demuxOptions);
                    break;

                default:
                    throw new FormatException("Source format not defined.");
            }

            this.outputBuffer.Append(Path.GetFileName(path) + Environment.NewLine);
        }               
    }
}
