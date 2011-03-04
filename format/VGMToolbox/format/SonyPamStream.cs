using System;
using System.Collections.Generic;
using System.IO;

using VGMToolbox.util;

namespace VGMToolbox.format
{
    public class SonyPamStream : SonyPmfStream
    {
        new public const string DefaultAudioExtension = ".at3";

        new public static readonly byte[] Atrac3Bytes = new byte[] { 0x1E, 0x60, 0x14 };

        public const string Ac3AudioExtension = ".ac3";
        
        public const string M2vVideoExtension = ".m2v";
        public static readonly byte[] M2vBytes = new byte[] { 0x00, 0x00, 0x01, 0xB3 };

        public SonyPamStream(string path)
            : base(path)
        {
            this.UsesSameIdForMultipleAudioTracks = true;
            this.FileExtensionAudio = Atrac3AudioExtension;
            this.FileExtensionVideo = AvcVideoExtension;

            base.BlockIdDictionary[BitConverter.ToUInt32(Mpeg2Stream.PacketStartByes, 0)] = new BlockSizeStruct(PacketSizeType.Static, 0xE); // Pack Header
            base.BlockIdDictionary[BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xBD }, 0)] = new BlockSizeStruct(PacketSizeType.SizeBytes, 2); // Audio Stream, two bytes following equal length (Big Endian)
        }

        protected override string GetAudioFileExtension(Stream readStream, long currentOffset)
        {
            string fileExtension;
            byte[] checkBytes;

            byte streamId = this.GetStreamId(readStream, currentOffset);

            switch (streamId)
            { 
                case 0x30:
                    fileExtension = Ac3AudioExtension;
                    break;
                case 0x40:
                    fileExtension = LpcmAudioExtension;
                    break;
                default:
                    if (streamId < 0x20)
                    {
                        fileExtension = Atrac3AudioExtension;
                    }
                    else
                    {
                        fileExtension = ".bin";
                    }
                    break;
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
            else if (ParseFile.CompareSegment(checkBytes, 0, M2vBytes))
            {
                fileExtension = M2vVideoExtension;
            }
            else
            {
                fileExtension = ".bin";
            }

            return fileExtension;
        }
    }
}
