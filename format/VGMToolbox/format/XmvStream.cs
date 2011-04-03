﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using VGMToolbox.format.util;
using VGMToolbox.util;

namespace VGMToolbox.format
{
    public class XmvStream
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
            public string OutputPath { set; get; }
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
            XmvAudioStreamHeader[] AudioStreams; 
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
            long blockStart = 0;
            long packetSize;
            long nextPacketSize = -1;

            long videoPacketSize;
            long[] audioStreamPacketSizes;

            Dictionary<string, FileStream> streamOutputWriters = new Dictionary<string, FileStream>();
            long audioStreamOffset;
            // string outputPath;

            GenhCreationStruct gcStruct;
            string genhFile;

            using (FileStream fs = File.Open(this.FilePath, FileMode.Open, FileAccess.Read))
            {
                fileSize = fs.Length;

                while ((nextPacketSize != 0) && (currentOffset < fileSize))
                {
                    blockStart = currentOffset;
                    
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
                            this.FileHeader.AudioHeaders[i] = new XmvAudioDataHeader();
                            this.FileHeader.AudioHeaders[i].WaveFormat = ParseFile.ParseSimpleOffset(fs, 0x24 + (i * 0xC), 2);
                            this.FileHeader.AudioHeaders[i].ChannelCount = ParseFile.ParseSimpleOffset(fs, 0x24 + (i * 0xC) + 2, 2);
                            this.FileHeader.AudioHeaders[i].SamplesPerSecond = ParseFile.ParseSimpleOffset(fs, 0x24 + (i * 0xC) + 4, 4);
                            this.FileHeader.AudioHeaders[i].BitsPerSample = ParseFile.ParseSimpleOffset(fs, 0x24 + (i * 0xC) + 8, 4);
                        }

                        // set next packet size
                        nextPacketSize = (long)BitConverter.ToUInt32(this.FileHeader.InitialPacketSize, 0);
                        currentOffset = 0x24 + (this.FileHeader.AudioStreamCount * 0xC);
                    }

                    // set packet size of current packet
                    packetSize = nextPacketSize;
                    
                    // get size of next packet
                    nextPacketSize = (long)BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(fs, currentOffset, 4), 0);

                    // get size of video data
                    videoPacketSize = BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(fs, currentOffset + 4, 4), 0);
                    videoPacketSize &= 0x000FFFFF;
                    videoPacketSize -= (this.FileHeader.AudioStreamCount * 4);

                    // setup audio for writing
                    audioStreamPacketSizes = new long[this.FileHeader.AudioStreamCount];
                    audioStreamOffset = currentOffset + 0xC + (this.FileHeader.AudioStreamCount * 4) + videoPacketSize;

                    for (uint i = 0; i < this.FileHeader.AudioStreamCount; i++)
                    {
                        audioStreamPacketSizes[i] = BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(fs, currentOffset + 0xC + (i * 4), 4), 0);

                        // write audio streams
                        this.FileHeader.AudioHeaders[i].OutputPath = Path.Combine(Path.GetDirectoryName(this.FilePath), String.Format("{0}_{1}.wav", Path.GetFileNameWithoutExtension(this.FilePath), i.ToString("X2")));

                        // add output stream to Dictionary
                        if (!streamOutputWriters.ContainsKey(this.FileHeader.AudioHeaders[i].OutputPath))
                        {
                            streamOutputWriters.Add(this.FileHeader.AudioHeaders[i].OutputPath, new FileStream(this.FileHeader.AudioHeaders[i].OutputPath, FileMode.Create, FileAccess.ReadWrite));
                        }

                        // write this audio packet
                        streamOutputWriters[this.FileHeader.AudioHeaders[i].OutputPath].Write(ParseFile.ParseSimpleOffset(fs, audioStreamOffset, (int)audioStreamPacketSizes[i]), 0, (int)audioStreamPacketSizes[i]);

                        // increase source offset to next packet
                        audioStreamOffset += audioStreamPacketSizes[i];
                    }                    

                    currentOffset = blockStart + packetSize;
                } // (currentOffset < fileSize)

                // close all writers
                foreach (string k in streamOutputWriters.Keys)
                {
                    if (streamOutputWriters[k].CanRead)
                    {
                        streamOutputWriters[k].Close();
                        streamOutputWriters[k].Dispose();
                    }
                }

                // interleave audio
                // needed at all, ONLY FOR MULTI-STREAM, SINGLE CHANNEL PER STREAM?  

                // add audio header
                for (int i = 0; i < this.FileHeader.AudioStreamCount; i++)
                {
                    gcStruct = new GenhCreationStruct();
                    gcStruct.Format = "0x01";
                    gcStruct.HeaderSkip = "0";
                    gcStruct.Interleave = "0x1";
                    gcStruct.Channels = BitConverter.ToUInt16(this.FileHeader.AudioHeaders[i].ChannelCount, 0).ToString();
                    gcStruct.Frequency = BitConverter.ToUInt16(this.FileHeader.AudioHeaders[i].SamplesPerSecond, 0).ToString();
                    gcStruct.NoLoops = true;

                    genhFile = GenhUtil.CreateGenhFile(this.FileHeader.AudioHeaders[i].OutputPath, gcStruct);

                    // delete original file
                    if (!String.IsNullOrEmpty(genhFile))
                    {
                        File.Delete(this.FileHeader.AudioHeaders[i].OutputPath);
                    }                
                }
                // add video header

            }
        }

    }
}