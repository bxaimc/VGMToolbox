using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VGMToolbox.util;

namespace VGMToolbox.format
{
    public class BinkStream
    {
        const string DefaultFileExtensionAudio = ".bink.audio";
        const string DefaultFileExtensionVideo = ".bink.video";

        public uint FrameCount { set; get; }
        public uint AudioTrackCount { set; get; }
        public uint[] AudioTrackIds { set; get; }
        public uint[] FrameOffsetList { set; get; }
        public byte[] FullHeader { set; get; }

        public uint[][] NewFrameOffsetsAudio { set; get; }
        public uint[] NewFrameOffsetsVideo { set; get; }

        public string FilePath { get; set; }
        public string FileExtensionAudio { get; set; }
        public string FileExtensionVideo { get; set; }

        public BinkStream(string filePath)
        {
            this.FilePath = filePath;
            this.FileExtensionAudio = BinkStream.DefaultFileExtensionAudio;
            this.FileExtensionVideo = BinkStream.DefaultFileExtensionVideo;
        }

        private void writeChunkToStream(
            byte[] chunk,
            uint chunkId,
            Dictionary<uint, FileStream> streamWriters,
            string fileExtension)
        {
            string destinationFile;

            if (!streamWriters.ContainsKey(chunkId))
            {
                destinationFile = Path.Combine(Path.GetDirectoryName(this.FilePath),
                    String.Format("{0}_{1}{2}", Path.GetFileNameWithoutExtension(this.FilePath), chunkId.ToString("X4"), fileExtension));
                streamWriters[chunkId] = File.Open(destinationFile, FileMode.Create, FileAccess.ReadWrite);
            }

            streamWriters[chunkId].Write(chunk, 0, chunk.Length);
        }

        public void ParseHeader(FileStream inStream, long offsetToHeader)
        {
            this.FrameCount = BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(inStream, offsetToHeader + 8, 4), 0);
            this.AudioTrackCount = BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(inStream, offsetToHeader + 0x28, 4), 0);

            // get Audio Track Ids
            this.AudioTrackIds = new uint[this.AudioTrackCount];

            for (uint i = 0; i < this.AudioTrackCount; i++)
            {
                this.AudioTrackIds[i] = BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(inStream, offsetToHeader + 0x2C + (i * 0xC) + 8, 4), 0);
            }

            // get frame offsets
            this.FrameOffsetList = new uint[this.FrameCount];
            
            for (uint i = 0; i < this.FrameCount; i++)
            {
                this.FrameOffsetList[i] = BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(inStream, offsetToHeader + 0x2C + (this.AudioTrackCount * 0xC) + (i * 4), 4), 0);
            }

            // get full header
            this.FullHeader = ParseFile.ParseSimpleOffset(inStream, 0, (int)(offsetToHeader + 0x2C + (this.AudioTrackCount * 0xC) + (this.FrameCount * 4) + 4));

        }

        protected virtual void DoFinalTasks(Dictionary<uint, FileStream> streamWriters,
            MpegStream.DemuxOptionsStruct demuxOptions)
        {
            string sourceFile;
            string headeredFile;

            byte[] headerBytes;

            foreach (uint key in streamWriters.Keys)
            {
                if (demuxOptions.AddHeader)
                {
                    if (streamWriters[key].Name.EndsWith(this.FileExtensionAudio))
                    {
                        // write header
                        
                        // replace offsets

                        // append to file
                    }
                    if (streamWriters[key].Name.EndsWith(this.FileExtensionVideo))
                    {

                    }
                }

                // close streams if open
                if (streamWriters[key] != null &&
                    streamWriters[key].CanRead)
                {
                    streamWriters[key].Close();
                    streamWriters[key].Dispose();
                }
            }
        }

        public virtual void DemultiplexStreams(MpegStream.DemuxOptionsStruct demuxOptions)
        {
            long currentOffset = 0;
            long fileSize;
            long packetEndOffset;

            uint audioPacketSize;
            byte[] audioPacket;

            uint videoPacketSize;
            byte[] videoPacket;

            long currentPacketOffset;
            
            Dictionary<uint, FileStream> streamOutputWriters = new Dictionary<uint, FileStream>();

            try
            {
                using (FileStream fs = File.OpenRead(this.FilePath))
                {
                    fileSize = fs.Length;
                    currentOffset = 0;
                    
                    // parse the header
                    this.ParseHeader(fs, 0);
                    
                    // setup new offsets and first frame
                    this.NewFrameOffsetsVideo = new uint[this.FrameCount];
                    this.NewFrameOffsetsVideo[0] = this.FrameOffsetList[0] - (this.AudioTrackCount * 0xC); // subtract audio frame header info

                    for (uint i = 0; i < this.AudioTrackCount; i++)
                    {
                        this.NewFrameOffsetsAudio[i] = new uint[this.FrameCount];
                        this.NewFrameOffsetsAudio[i][0] = this.FrameOffsetList[0] - ((this.AudioTrackCount - 1) * 0xC); // only need one audio header area
                    }

                    //////////////////////
                    // process each frame
                    //////////////////////
                    for (uint frameId = 0; frameId < this.FrameCount; frameId++)
                    {
                        currentPacketOffset = 0;
                        
                        //////////////////
                        // extract audio  
                        //////////////////
                        for (uint audioTrackId = 0; audioTrackId < this.AudioTrackCount; audioTrackId++)
                        {
                            audioPacketSize = BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(fs, this.FrameOffsetList[frameId] + currentPacketOffset, 4) , 0);
                            audioPacketSize += 4;

                            if (demuxOptions.ExtractAudio)
                            {
                                audioPacket = ParseFile.ParseSimpleOffset(fs, this.FrameOffsetList[frameId] + currentPacketOffset, (int)audioPacketSize);
                                this.writeChunkToStream(audioPacket, this.AudioTrackIds[audioTrackId], streamOutputWriters, this.FileExtensionAudio);
                            }

                            currentPacketOffset += audioPacketSize; // goto next packet                        

                            // update audio frame id
                            this.NewFrameOffsetsAudio[audioTrackId][frameId + 1] = this.NewFrameOffsetsAudio[audioTrackId][frameId] + audioPacketSize;
                        }

                        /////////////////
                        // extract video
                        /////////////////
                        if (frameId == (this.FrameCount - 1)) // last frame
                        {
                            packetEndOffset = fileSize;
                        }
                        else
                        {
                            packetEndOffset = this.FrameOffsetList[frameId + 1];
                        }

                        videoPacketSize = (uint)(packetEndOffset - (this.FrameOffsetList[frameId] + currentPacketOffset));
                        
                        if (demuxOptions.ExtractVideo)
                        {
                            // parse video packet
                            videoPacket = ParseFile.ParseSimpleOffset(fs, this.FrameOffsetList[frameId] + currentPacketOffset, (int)videoPacketSize);
                            this.writeChunkToStream(videoPacket, 0, streamOutputWriters, this.FileExtensionVideo);
                        }                    
                    
                        // update video frame offset
                        this.NewFrameOffsetsVideo[frameId + 1] = this.NewFrameOffsetsVideo[frameId] + (uint)videoPacketSize;
                    
                    } // for (uint frameId = 0; frameId < this.FrameCount; frameId++)

                                        
 

                } // using (FileStream fs = File.OpenRead(this.FilePath))
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Exception processing block at offset 0x{0}: {1}{2}", currentOffset.ToString("X"), ex.Message, Environment.NewLine));
            }
            finally
            {
                // clean up
                this.DoFinalTasks(streamOutputWriters, demuxOptions);
            }
        }
    }
    
}

        