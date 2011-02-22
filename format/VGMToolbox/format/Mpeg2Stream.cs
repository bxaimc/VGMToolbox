using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using VGMToolbox.util;

namespace VGMToolbox.format
{
    public class Mpeg2Stream : MpegStream
    {
        public const string DefaultAudioExtension = ".m2a";
        public const string DefaultVideoExtension = ".m2v";

        public Mpeg2Stream(string path):
            base(path)
        {
            this.FileExtensionAudio = DefaultAudioExtension;
            this.FileExtensionVideo = DefaultVideoExtension;

            base.BlockIdDictionary[BitConverter.ToUInt32(Mpeg2Stream.PacketStartByes, 0)] = new BlockSizeStruct(PacketSizeType.Static, 0xE); // Pack Header
        }

        protected int GetStandardPesHeaderSize(Stream readStream, long currentOffset)
        {
            byte checkBytes;
            OffsetDescription od = new OffsetDescription();

            od.OffsetByteOrder = Constants.BigEndianByteOrder;
            od.OffsetSize = "1";
            od.OffsetValue = "8";

            checkBytes = (byte)ParseFile.GetVaryingByteValueAtRelativeOffset(readStream, od, currentOffset);

            return checkBytes + 3;
        }

        protected override int GetAudioPacketHeaderSize(Stream readStream, long currentOffset)
        {            
            return GetStandardPesHeaderSize(readStream, currentOffset);
        }

        protected override int GetVideoPacketHeaderSize(Stream readStream, long currentOffset)
        {
            return GetStandardPesHeaderSize(readStream, currentOffset);
        }
    }
}
