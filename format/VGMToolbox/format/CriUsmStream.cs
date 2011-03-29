using System;
using System.Collections.Generic;
using System.IO;

using VGMToolbox.util;

namespace VGMToolbox.format
{
    public class CriUsmStream : MpegStream
    {
        public const string DefaultAudioExtension = ".adx";
        public const string DefaultVideoExtension = ".m2v";

        protected static readonly byte[] CRID_BYTES = new byte[] { 0x43, 0x52, 0x49, 0x44 };
        protected static readonly byte[] SFV_BYTES = new byte[] { 0x40, 0x53, 0x46, 0x56 };
        protected static readonly byte[] SFA_BYTES = new byte[] { 0x40, 0x53, 0x46, 0x41 };

        protected static readonly byte[] HEADER_END_BYTES = 
            new byte[] { 0x23, 0x48, 0x45, 0x41, 0x44, 0x45, 0x52, 0x20, 
                         0x45, 0x4E, 0x44, 0x20, 0x20, 0x20, 0x20, 0x20, 
                         0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 
                         0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x00 };

        protected static readonly byte[] METADATA_END_BYTES =
            new byte[] { 0x23, 0x4D, 0x45, 0x54, 0x41, 0x44, 0x41, 0x54, 
                         0x41, 0x20, 0x45, 0x4E, 0x44, 0x20, 0x20, 0x20, 
                         0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 
                         0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x00 };

        protected static readonly byte[] CONTENTS_END_BYTES = 
            new byte[] { 0x23, 0x43, 0x4F, 0x4E, 0x54, 0x45, 0x4E, 0x54, 
                         0x53, 0x20, 0x45, 0x4E, 0x44, 0x20, 0x20, 0x20, 
                         0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 
                         0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x3D, 0x00 };

        public CriUsmStream(string path)
            : base(path)
        {
            this.UsesSameIdForMultipleAudioTracks = true;
            this.FileExtensionAudio = DefaultAudioExtension;
            this.FileExtensionVideo = DefaultVideoExtension;

            base.BlockIdDictionary.Clear();
            base.BlockIdDictionary[BitConverter.ToUInt32(CRID_BYTES, 0)] = new BlockSizeStruct(PacketSizeType.SizeBytes, 4); // CRID
            base.BlockIdDictionary[BitConverter.ToUInt32(SFV_BYTES, 0)] = new BlockSizeStruct(PacketSizeType.SizeBytes, 4); // @SFV
            base.BlockIdDictionary[BitConverter.ToUInt32(SFA_BYTES, 0)] = new BlockSizeStruct(PacketSizeType.SizeBytes, 4); // @SFA
        }

        protected override byte[] GetPacketStartBytes() { return CRID_BYTES; }

        protected override int GetAudioPacketHeaderSize(Stream readStream, long currentOffset)
        {
            UInt16 checkBytes;
            OffsetDescription od = new OffsetDescription();

            od.OffsetByteOrder = Constants.BigEndianByteOrder;
            od.OffsetSize = "2";
            od.OffsetValue = "8";

            checkBytes = (UInt16)ParseFile.GetVaryingByteValueAtRelativeOffset(readStream, od, currentOffset);

            return checkBytes;
        }
        protected override int GetVideoPacketHeaderSize(Stream readStream, long currentOffset)
        {
            UInt16 checkBytes;
            OffsetDescription od = new OffsetDescription();

            od.OffsetByteOrder = Constants.BigEndianByteOrder;
            od.OffsetSize = "2";
            od.OffsetValue = "8";

            checkBytes = (UInt16)ParseFile.GetVaryingByteValueAtRelativeOffset(readStream, od, currentOffset);

            return checkBytes;
        }

        protected override bool IsThisAnAudioBlock(byte[] blockToCheck)
        {
            return ParseFile.CompareSegment(blockToCheck, 0, SFA_BYTES);
        }
        protected override bool IsThisAVideoBlock(byte[] blockToCheck)
        {
            return ParseFile.CompareSegment(blockToCheck, 0, SFV_BYTES);
        }

        protected override byte GetStreamId(Stream readStream, long currentOffset)
        {
            byte streamId;
            
            streamId = ParseFile.ParseSimpleOffset(readStream, currentOffset + 0xC, 1)[0];

            return streamId;
        }

        protected override int GetAudioPacketFooterSize(Stream readStream, long currentOffset) 
        {
            UInt16 checkBytes;
            OffsetDescription od = new OffsetDescription();

            od.OffsetByteOrder = Constants.BigEndianByteOrder;
            od.OffsetSize = "2";
            od.OffsetValue = "0xA";

            checkBytes = (UInt16)ParseFile.GetVaryingByteValueAtRelativeOffset(readStream, od, currentOffset);

            return checkBytes;
        }

        protected override int GetVideoPacketFooterSize(Stream readStream, long currentOffset) 
        {
            UInt16 checkBytes;
            OffsetDescription od = new OffsetDescription();

            od.OffsetByteOrder = Constants.BigEndianByteOrder;
            od.OffsetSize = "2";
            od.OffsetValue = "0xA";

            checkBytes = (UInt16)ParseFile.GetVaryingByteValueAtRelativeOffset(readStream, od, currentOffset);

            return checkBytes;
        }
    }
}
