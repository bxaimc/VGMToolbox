using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using VGMToolbox.format.util;
using VGMToolbox.util;

namespace VGMToolbox.format
{
    public class MobiclipWiiStream : MobiclipStream
    {
        public static readonly byte[] StreamTypeBytes = new byte[] { 0x43, 0x35 }; //C5
        public const ushort AudioChunkSignature = 0x414D;

        public MobiclipWiiStream(string path)
        {
            this.FilePath = path;
            this.FileExtensionAudio = ".raw";
            this.FileExtensionVideo = ".bin";
        }

        protected override long GetBlockSize(FileStream inStream, long currentOffset)
        {
            long blockSize = (long)BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(inStream, currentOffset, 4), 0);

            if (blockSize > 0)
            {
                blockSize += 4;

                if ((blockSize % 4) != 0)
                {
                    blockSize -= (blockSize % 4);
                }
            }
            return blockSize;
        }

        protected override void ReadHeader(FileStream inStream, long offset) 
        { 
            long currentOffset = offset;
            
            ushort chunkTypeId;
            ushort subChunkCount;

            // read header size
            this.HeaderSize = ParseFile.ReadUintLE(inStream, currentOffset + 4) + 8;
            currentOffset += 8;

            while (currentOffset < this.HeaderSize)
            { 
                // read chunk type id
                chunkTypeId = ParseFile.ReadUshortBE(inStream, currentOffset);

                // read subchunk count
                subChunkCount = ParseFile.ReadUshortLE(inStream, currentOffset + 2);

                switch (chunkTypeId)
                { 
                    // get audio details
                    case MobiclipWiiStream.AudioChunkSignature:
                        this.AudioStreamCount = ParseFile.ReadUintLE(inStream, currentOffset + 4);
                        this.AudioStreamFeatures = new MobiclipAudioStreamFeatures[this.AudioStreamCount];

                        for (uint i = 0; i < this.AudioStreamCount; i++)
                        {
                            this.AudioStreamFeatures[i] = new MobiclipAudioStreamFeatures();
                            this.AudioStreamFeatures[i].StreamType = ParseFile.ReadUshortBE(inStream, currentOffset + 8 + (i * 0xA));
                            this.AudioStreamFeatures[i].Frequency = ParseFile.ReadUintLE(inStream, currentOffset + 8 + (i * 0xA) + 2);
                            this.AudioStreamFeatures[i].Channels = ParseFile.ReadUintLE(inStream, currentOffset + 8 + (i * 0xA) + 6);
                        }

                        break;                
                    
                    
                    
                    default:                        
                        break;
                }

                currentOffset += (subChunkCount * 4) + 4;            
            }
        }

        protected override ChunkStruct GetVideoChunk(FileStream inStream, long currentOffset)
        {
            ChunkStruct videoChunkStruct = new ChunkStruct();

            long chunkSize = (long)ParseFile.ReadUintLE(inStream, currentOffset + 4);
            byte[] videoChunk = ParseFile.ParseSimpleOffset(inStream, currentOffset + 8, (int)chunkSize);

            videoChunkStruct.Chunk = videoChunk;
            videoChunkStruct.ChunkId = 0xFFFF;
            
            return videoChunkStruct;        
        }

        protected override ChunkStruct GetAudioChunk(FileStream inStream, long currentOffset, long blockSize, long videoChunkSize)
        {
            ChunkStruct audioChunkStruct = new ChunkStruct();
            byte[] audioChunk;

            long absoluteOffset = currentOffset + 8 + videoChunkSize + 4;
            int chunkSize = (int)(blockSize - videoChunkSize - 8 - 4);

            if (chunkSize > 0)
            {                
                audioChunk = ParseFile.ParseSimpleOffset(inStream, absoluteOffset, chunkSize);

                audioChunkStruct.Chunk = audioChunk;
                audioChunkStruct.ChunkId = BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(inStream, absoluteOffset - 4, 4), 0);
                audioChunkStruct.ChunkId = 0;
            }

            return audioChunkStruct;
        }

        protected override void DoFinalTasks(Dictionary<uint, FileStream> streamWriters,
            MpegStream.DemuxOptionsStruct demuxOptions)
        {
            GenhCreationStruct gcStruct;
            ushort frequency;
            string sourceFile;
            string genhFile;

            foreach (uint key in streamWriters.Keys)
            {
                if (demuxOptions.AddHeader)
                {
                    if (streamWriters[key].Name.EndsWith(this.FileExtensionAudio))
                    {
                        // get frequency
                        //using (FileStream fs = File.OpenRead(this.FilePath))
                        //{
                        //    frequency = BitConverter.ToUInt16(ParseFile.ParseSimpleOffset(fs, 0xDE, 2), 0);
                        //}

                        sourceFile = streamWriters[key].Name;

                        streamWriters[key].Close();
                        streamWriters[key].Dispose();

                        gcStruct = new GenhCreationStruct();

                        //@TODO: Update to use this.AudioStreamFeatures[key].StreamType
                        gcStruct.Format = "0x04";
                                                
                        gcStruct.HeaderSkip = "0";
                        gcStruct.Interleave = "0x2";
                        gcStruct.Channels = this.AudioStreamFeatures[key].Channels.ToString();
                        gcStruct.Frequency = this.AudioStreamFeatures[key].Frequency.ToString();
                        gcStruct.NoLoops = true;

                        genhFile = GenhUtil.CreateGenhFile(sourceFile, gcStruct);

                        // delete original file
                        if (!String.IsNullOrEmpty(genhFile))
                        {
                            File.Delete(sourceFile);
                        }
                    }
                }
            }

            base.DoFinalTasks(streamWriters, demuxOptions);
        }
    }
}
