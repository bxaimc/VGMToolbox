using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using VGMToolbox.util;

namespace VGMToolbox.format
{
    public class MpegStream
    {
        public const string DefaultAudioExtension = ".m2a";
        public const string DefaultVideoExtension = ".m2v";

        public static readonly byte[] PacketStartByes = new byte[] { 0x00, 0x00, 0x01, 0xBA };
        public static readonly byte[] PacketEndByes = new byte[] { 0x00, 0x00, 0x01, 0xB9 };

        public MpegStream(string path)
        {
            this.FilePath = path;
            this.FileExtensionAudio = DefaultAudioExtension;
            this.FileExtensionVideo = DefaultVideoExtension;
        }

        public enum PacketSizeType
        { 
            Static,
            SizeBytes,
            Eof
        }

        public enum MpegSupportedDataFormats
        { 
            SofdecVideo,
            SonyPssVideo,
        }

        public struct BlockSizeStruct
        {
            public PacketSizeType SizeType;
            public int Size;

            public BlockSizeStruct(PacketSizeType sizeTypeValue, int sizeValue)
            {
                this.SizeType = sizeTypeValue;
                this.Size = sizeValue;
            }
        }

        #region Dictionary Initialization

        protected Dictionary<uint, BlockSizeStruct> BlockIdDictionary =
            new Dictionary<uint, BlockSizeStruct>
            {                                
                //********************
                // System Packets
                //********************
                {BitConverter.ToUInt32(MpegStream.PacketEndByes, 0), new BlockSizeStruct(PacketSizeType.Eof, -1)},   // Program End
                {BitConverter.ToUInt32(MpegStream.PacketStartByes, 0), new BlockSizeStruct(PacketSizeType.Static, 0xE)}, // Pack Header
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xBB }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // System Header, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xBE }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Padding Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xBF }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Private Stream, two bytes following equal length (Big Endian)

                //****************************
                // Audio Streams
                //****************************
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xC0 }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Audio Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xC1 }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Audio Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xC2 }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Audio Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xC3 }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Audio Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xC4 }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Audio Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xC5 }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Audio Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xC6 }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Audio Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xC7 }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Audio Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xC8 }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Audio Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xC9 }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Audio Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xCA }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Audio Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xCB }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Audio Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xCC }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Audio Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xCD }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Audio Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xCE }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Audio Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xCF }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Audio Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xD0 }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Audio Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xD1 }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Audio Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xD2 }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Audio Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xD3 }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Audio Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xD4 }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Audio Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xD5 }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Audio Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xD6 }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Audio Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xD7 }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Audio Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xD8 }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Audio Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xD9 }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Audio Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xDA }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Audio Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xDB }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Audio Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xDC }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Audio Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xDD }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Audio Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xDE }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Audio Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xDF }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Audio Stream, two bytes following equal length (Big Endian)

                //****************************
                // Video Streams
                //****************************
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xE0 }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Video Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xE1 }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Video Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xE2 }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Video Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xE3 }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Video Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xE4 }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Video Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xE5 }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Video Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xE6 }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Video Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xE7 }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Video Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xE8 }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Video Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xE9 }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Video Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xEA }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Video Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xEB }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Video Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xEC }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Video Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xED }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Video Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xEE }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Video Stream, two bytes following equal length (Big Endian)
                {BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xEF }, 0), new BlockSizeStruct(PacketSizeType.SizeBytes, 2)}, // Video Stream, two bytes following equal length (Big Endian)
            };
        #endregion

        public string FilePath { get; set; }
        public string FileExtensionAudio { get; set; }
        public string FileExtensionVideo { get; set; }

        protected virtual int GetAudioPacketHeaderSize(Stream readStream, long currentOffset)
        {
            return 0;
        }

        protected virtual bool IsThisAnAudioBlock(byte[] blockToCheck)
        {
            return ((blockToCheck[3] >= 0xC0) && (blockToCheck[3] <= 0xDF));
        }
        protected virtual bool IsThisAVideoBlock(byte[] blockToCheck)
        {
            return ((blockToCheck[3] >= 0xE0) && (blockToCheck[3] <= 0xEF));
        }

        public void DemultiplexStreams()
        {
            using (FileStream fs = File.OpenRead(this.FilePath))
            {
                long fileSize = fs.Length;
                long currentOffset;
                
                byte[] currentBlockId;
                uint currentBlockIdVal;
                byte[] currentBlockIdNaming;
                
                BlockSizeStruct blockStruct = new BlockSizeStruct();
                byte[] blockSizeArray;
                uint blockSize;
                int audioBlockSkipSize;
                byte[] secondaryBlocks;

                bool eofFlagFound = false;

                Dictionary<uint, FileStream> streamOutputWriters = new Dictionary<uint,FileStream>();
                string outputFileName;

                // look for first packet
                currentOffset = ParseFile.GetNextOffset(fs, 0, MpegStream.PacketStartByes);

                while (currentOffset < fileSize)
                {
                    currentBlockId = ParseFile.ParseSimpleOffset(fs, currentOffset, 4);
                    currentBlockIdVal = BitConverter.ToUInt32(currentBlockId, 0);

                    if (BlockIdDictionary.ContainsKey(currentBlockIdVal))
                    {
                        blockStruct = BlockIdDictionary[currentBlockIdVal];

                        switch (blockStruct.SizeType)
                        { 
                            /////////////////////
                            // Static Block Size
                            /////////////////////
                            case PacketSizeType.Static:
                                currentOffset += blockStruct.Size; // skip this block
                                break;
                            
                            //////////////////
                            // End of Stream
                            //////////////////
                            case PacketSizeType.Eof:
                                eofFlagFound = true; // set EOF block found so we can exit the loop
                                break;
                            
                            //////////////////////
                            // Varying Block Size
                            //////////////////////
                            case PacketSizeType.SizeBytes:
                                
                                // Get the block size
                                blockSizeArray = ParseFile.ParseSimpleOffset(fs, currentOffset + currentBlockId.Length, 2);
                                Array.Reverse(blockSizeArray);
                                blockSize = (uint)BitConverter.ToUInt16(blockSizeArray, 0);
                                
                                // if block type is audio or video, extract it
                                // if (this.IsThisAnAudioBlock(currentBlockId) || this.IsThisAVideoBlock(currentBlockId))
                                if (this.IsThisAnAudioBlock(currentBlockId))
                                {
                                    if (!streamOutputWriters.ContainsKey(currentBlockIdVal))
                                    {
                                        // convert block id to little endian for naming
                                        currentBlockIdNaming = new byte[currentBlockId.Length];
                                        Array.Copy(currentBlockId, currentBlockIdNaming, currentBlockId.Length);
                                        Array.Reverse(currentBlockIdNaming);

                                        // build output file name
                                        outputFileName = Path.GetFileNameWithoutExtension(this.FilePath);
                                        outputFileName = outputFileName + "_" + BitConverter.ToUInt32(currentBlockIdNaming, 0).ToString("X8");

                                        // add proper extension
                                        if (this.IsThisAnAudioBlock(currentBlockId))
                                        {
                                            outputFileName += this.FileExtensionAudio;
                                        }
                                        // else
                                        // {
                                        //     outputFileName += this.FileExtensionVideo;
                                        // }

                                        // add output directory
                                        outputFileName = Path.Combine(Path.GetDirectoryName(this.FilePath), outputFileName);

                                        // add an output stream for writing
                                        streamOutputWriters[currentBlockIdVal] = new FileStream(outputFileName, FileMode.Create, FileAccess.Write);
                                    }

                                    // write the block
                                    if (this.IsThisAnAudioBlock(currentBlockId))
                                    {
                                        // write audio
                                        audioBlockSkipSize = this.GetAudioPacketHeaderSize(fs, currentOffset);
                                        streamOutputWriters[currentBlockIdVal].Write(ParseFile.ParseSimpleOffset(fs, currentOffset + currentBlockId.Length + blockSizeArray.Length + audioBlockSkipSize, (int)(blockSize - audioBlockSkipSize)), 0, (int)(blockSize - audioBlockSkipSize));
                                    }
                                    //else
                                    //{
                                    //    // write video
                                    //    if (this.SkipVideoPacketHeaderOnExtraction())
                                    //    {
                                    //        streamOutputWriters[currentBlockIdVal].Write(ParseFile.ParseSimpleOffset(fs, currentOffset + currentBlockId.Length + blockSizeArray.Length + this.GetVideoPacketHeaderSize(), (int)(blockSize - this.GetVideoPacketHeaderSize())), 0, (int)(blockSize - this.GetVideoPacketHeaderSize()));
                                    //    }
                                    //    else
                                    //    {
                                    //        streamOutputWriters[currentBlockIdVal].Write(ParseFile.ParseSimpleOffset(fs, currentOffset + currentBlockId.Length + blockSizeArray.Length, (int)blockSize), 0, (int)blockSize);
                                    //    }
                                    //}                                    
                                }

                                // move to next block
                                currentOffset += currentBlockId.Length + blockSizeArray.Length + blockSize;
                                blockSizeArray = new byte[] {};
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    { 
                        Array.Reverse(currentBlockId);
                        throw new FormatException(String.Format("Block ID at 0x{0} not found in table: 0x{1}", currentOffset.ToString("X8"), BitConverter.ToUInt32(currentBlockId, 0).ToString("X8")));
                    }

                    // exit loop if EOF block found
                    if (eofFlagFound)
                    {
                        break;
                    }
                } // while (currentOffset < fileSize)

                //////////////////////////
                // close all open writers
                //////////////////////////
                foreach (uint b in streamOutputWriters.Keys)
                {
                    streamOutputWriters[b].Close();
                    streamOutputWriters[b].Dispose();
                }

            } // using (FileStream fs = File.OpenRead(path))
        }
    }
}
