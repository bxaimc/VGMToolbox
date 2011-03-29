using System;
using System.Collections.Generic;
using System.IO;

using VGMToolbox.util;

namespace VGMToolbox.format
{
    public class SonyPmfStream : Mpeg2Stream
    {
        new public const string DefaultAudioExtension = ".at3";

        public const string Atrac3AudioExtension = ".at3";
        public const string LpcmAudioExtension = ".lpcm";

        public const byte LpcmStreamId = 0x40;

        public static readonly byte[] Atrac3Bytes = new byte[] { 0x1E, 0x60, 0x04 };
        public static readonly byte[] LpcmBytes = new byte[] { 0x1E, 0x61, 0x80, 0x40 };

        public const string AvcVideoExtension = ".264";
        public static readonly byte[] AvcBytes = new byte[] { 0x00, 0x00, 0x00, 0x01 };

        public SonyPmfStream(string path)
            : base(path)
        {
            this.UsesSameIdForMultipleAudioTracks = true;
            this.FileExtensionAudio = DefaultAudioExtension;
            this.FileExtensionVideo = AvcVideoExtension;

            base.BlockIdDictionary[BitConverter.ToUInt32(Mpeg2Stream.PacketStartBytes, 0)] = new BlockSizeStruct(PacketSizeType.Static, 0xE); // Pack Header
            base.BlockIdDictionary[BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xBD }, 0)] = new BlockSizeStruct(PacketSizeType.SizeBytes, 2); // Audio Stream, two bytes following equal length (Big Endian)
        }

        protected override long GetStartOffset(Stream readStream, long currentOffset)
        {
            long startOffset = 0;

            uint seekOffsets = ByteConversion.GetUInt32BigEndian(ParseFile.ParseSimpleOffset(readStream, 0x86, 4));
            uint seekCount = ByteConversion.GetUInt32BigEndian(ParseFile.ParseSimpleOffset(readStream, 0x8A, 4));

            if (seekOffsets > 0)
            {
                startOffset = seekOffsets + (seekCount * 0x0A);
            }

            return startOffset;
        }

        protected override int GetAudioPacketHeaderSize(Stream readStream, long currentOffset)
        {
            /*
            int headerSize;
            UInt16 checkBytes;
            OffsetDescription od = new OffsetDescription();

            od.OffsetByteOrder = Constants.BigEndianByteOrder;
            od.OffsetSize = "2";
            od.OffsetValue = "6";

            checkBytes = (UInt16)ParseFile.GetVaryingByteValueAtRelativeOffset(readStream, od, currentOffset);

            switch (checkBytes)
            {
                // Thanks to FastElbJa and creator of pmfdemuxer (used to compare output) for this information
                case 0x8100:
                    headerSize = 0x07;
                    break;
                case 0x8180:
                case 0x8101:
                    headerSize = 0x0C;
                    break;
                case 0x8181:
                    headerSize = 0x0F;
                    break;
                default:
                    throw new FormatException(String.Format("Unexpected secondary bytes found for block starting at 0x{0}: 0x{1}", currentOffset.ToString("X8"), checkBytes.ToString("X4")));
            }
            return headerSize;
            */
            
            byte checkBytes;
            OffsetDescription od = new OffsetDescription();

            od.OffsetByteOrder = Constants.BigEndianByteOrder;
            od.OffsetSize = "1";
            od.OffsetValue = "8";

            checkBytes = (byte)ParseFile.GetVaryingByteValueAtRelativeOffset(readStream, od, currentOffset);

            return checkBytes + 7;           
        }

        protected override bool IsThisAnAudioBlock(byte[] blockToCheck)
        {
            return (blockToCheck[3] == 0xBD);
        }
        protected override bool IsThisAVideoBlock(byte[] blockToCheck)
        {
            return ((blockToCheck[3] >= 0xE0) && (blockToCheck[3] <= 0xEF));
        }

        protected override string GetAudioFileExtension(Stream readStream, long currentOffset)
        {
            string fileExtension;
            byte streamId = this.GetStreamId(readStream, currentOffset);

            if (streamId < 0x20)
            {
                fileExtension = Atrac3AudioExtension;
            }
            else if ((streamId >= 0x40) && (streamId < 0x50))
            {
                fileExtension = LpcmAudioExtension;
            }
            else
            {
                fileExtension = ".bin";
            }

            return fileExtension;
        }
        protected override string GetVideoFileExtension(Stream readStream, long currentOffset)
        {
            string fileExtension;
            byte[] checkBytes;
            int videoHeaderSize = this.GetVideoPacketHeaderSize(readStream, currentOffset);

            checkBytes = ParseFile.ParseSimpleOffset(readStream, (currentOffset + videoHeaderSize + 6), 4);

            if (ParseFile.CompareSegment(checkBytes, 0, AvcBytes))
            {
                fileExtension = AvcVideoExtension;
            }
            else
            {
                fileExtension = ".bin";
            }

            return fileExtension;
        }

        protected override byte GetStreamId(Stream readStream, long currentOffset) 
        {
            byte streamId;
            byte sizeValue;
            int offsetToCheck;

            sizeValue = ParseFile.ParseSimpleOffset(readStream, currentOffset + 8, 1)[0];
            offsetToCheck = sizeValue + 6 + 7 - 4;
            streamId = ParseFile.ParseSimpleOffset(readStream, currentOffset + offsetToCheck, 1)[0];

            return streamId;
        }

        protected override void DoFinalTasks(FileStream sourceFileStream, Dictionary<uint, FileStream> outputFiles, bool addHeader)
        {
            byte[] headerBytes;
            byte[] aa3HeaderBytes;
            uint headerBlock;
            string sourceFile;

            foreach (uint streamId in outputFiles.Keys)
            {
                if (this.IsThisAnAudioBlock(BitConverter.GetBytes(streamId)) &&
                    outputFiles[streamId].Name.EndsWith(Atrac3AudioExtension))
                {
                    headerBytes = ParseFile.ParseSimpleOffset(outputFiles[streamId], 0, 0x8);

                    // remove all header chunks
                    string cleanedFile = FileUtil.RemoveAllChunksFromFile(outputFiles[streamId], headerBytes);

                    // close stream and rename file
                    sourceFile = outputFiles[streamId].Name;

                    outputFiles[streamId].Close();
                    outputFiles[streamId].Dispose();

                    File.Delete(sourceFile);
                    File.Move(cleanedFile, sourceFile);

                    // add header
                    if (addHeader)
                    {
                        Array.Reverse(headerBytes);
                        headerBlock = BitConverter.ToUInt32(headerBytes, 4);

                        string headeredFile = Path.ChangeExtension(sourceFile, Atrac3Plus.FileExtensionPsp);
                        aa3HeaderBytes = Atrac3Plus.GetAa3Header(headerBlock);
                        FileUtil.AddHeaderToFile(aa3HeaderBytes, sourceFile, headeredFile);

                        File.Delete(sourceFile);
                    }
                }
            }
        }
    }
}
