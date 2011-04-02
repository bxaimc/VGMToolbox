using System;
using System.IO;
using System.Text;

using VGMToolbox.util;

namespace VGMToolbox.format
{
    class XmvStream
    {
        public static readonly byte[] XmvMagicBytes = new byte[] { 0x78, 0x6F, 0x62, 0x58 }; // xobX

        public static readonly byte[] WavFormatPcm = new byte[] { 0x01, 0x00 };
        public static readonly byte[] WavFormatXboxAdpcm = new byte[] { 0x69, 0x00 };

        public struct XmvAudioDataHeader
        {
            public byte[] WaveFormat { set; get; }
            public byte[] ChannelCount { set; get; }
            public byte[] SamplesPerSecond { set; get; }
            public byte[] BitsPerSample { set; get; }
        }

        public struct XmvVideoDataHeader
        {
            public byte[] InitialPacketSize { set; get; }
            public byte[] MagicBytes { set; get; }
            public byte[] Slices { set; get; }

            public byte[] VideoWidth { set; get; }
            public byte[] VideoHeight { set; get; }

            public UInt32 AudioStreamCount { set; get; }

            public XmvAudioDataHeader[] AudioHeaders { set; get; }
        }

        public struct XmvPacketHeader
        {
            byte[] NextPacketSize { set; get; }
        }

        public struct XmvAudioStreamHeader
        {
            byte[] AudioStreamSize { set; get; }
        }

        public struct XmvVideoPacketHeader 
        {
            byte[] VideoStreamLength { set; get; }
            byte[] FrameCount { set; get; }
            XmvAudioStreamHeader[] AudioSteams; 
        }

        public struct XmvVideoFrame
        {
            byte[] FrameSize { set; get; }
        }

        public struct XmvAdpcmChunk
        {
            byte[] AudioDataSize { set; get; }
        }


        
        public string FilePath { get; set; }
        public XmvVideoDataHeader FileHeader;


        public XmvStream(string path)
        {
            this.FilePath = path;
        }

        public void DemultiplexStreams()
        {
            long fileSize;
            long currentOffset = 0;
            long packetSize;

            long videoPacketSize;
            long[] audioStreamPacketSizes;

            using (FileStream fs = File.Open(this.FilePath, FileMode.Open, FileAccess.Read))
            {
                fileSize = fs.Length;
                
                while (currentOffset < fileSize)
                {
                    if (currentOffset == 0)
                    {
                        // get main header
                        this.FileHeader = new XmvVideoDataHeader();
                        this.FileHeader.MagicBytes = ParseFile.ParseSimpleOffset(fs, 0xC, 4);

                        if (!ParseFile.CompareSegment(this.FileHeader.MagicBytes, 0, XmvMagicBytes))
                        {
                            throw new FormatException(String.Format("XMV Magic Bytes: 'xobX' not found at offset 0xC{0}", Environment.NewLine));
                        }

                        this.FileHeader.InitialPacketSize = ParseFile.ParseSimpleOffset(fs, 4, 4);
                        this.FileHeader.VideoWidth = ParseFile.ParseSimpleOffset(fs, 0x14, 4);
                        this.FileHeader.VideoHeight = ParseFile.ParseSimpleOffset(fs, 0x18, 4);

                        // get audio subheaders
                        this.FileHeader.AudioStreamCount = BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(fs, 0x20, 4), 0);
                        this.FileHeader.AudioHeaders = new XmvAudioDataHeader[this.FileHeader.AudioStreamCount];

                        for (uint i = 0; i < this.FileHeader.AudioStreamCount; i++)
                        {
                            this.FileHeader.AudioHeaders[i].WaveFormat = ParseFile.ParseSimpleOffset(fs, 0x24 + (i * 0xC), 2);
                            this.FileHeader.AudioHeaders[i].ChannelCount = ParseFile.ParseSimpleOffset(fs, 0x24 + (i * 0xC) + 2, 2);
                            this.FileHeader.AudioHeaders[i].SamplesPerSecond = ParseFile.ParseSimpleOffset(fs, 0x24 + (i * 0xC) + 4, 4);
                            this.FileHeader.AudioHeaders[i].BitsPerSample = ParseFile.ParseSimpleOffset(fs, 0x24 + (i * 0xC) + 8, 4);
                        }

                        packetSize = (long)BitConverter.ToUInt32(this.FileHeader.InitialPacketSize, 0);
                        currentOffset = 0x24 + (this.FileHeader.AudioStreamCount * 0xC) + 0xC;
                    }
                    else
                    {
                        packetSize = (long)BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(fs, currentOffset, 4), 0);
                    }

                    videoPacketSize = BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(fs, currentOffset + 4, 4), 0);
                    videoPacketSize &= 0x00FFFFFF;

                    audioStreamPacketSizes = new long[this.FileHeader.AudioStreamCount];
                    
                    for (uint i = 0; i < this.FileHeader.AudioStreamCount; i++)
                    {
                        audioStreamPacketSizes[i] = BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(fs, currentOffset + 8 + (i * 4), 4), 0);
                    }
                    
                    // write streams



                    currentOffset += packetSize;
                } // (currentOffset < fileSize)

                // interleave audio

                // add audio header

                // add video header
            }
        }

    }
}
