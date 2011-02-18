using System;

namespace VGMToolbox.format
{
    public class SonyPssStream : MpegStream
    {
        new public const string DefaultAudioExtension = ".ss2";
        new public const string DefaultVideoExtension = ".m2v";

        public SonyPssStream(string path)
            : base(path)
        {
            base.BlockIdDictionary[BitConverter.ToUInt32(MpegStream.PacketStartByes, 0)] = new BlockSizeStruct(PacketSizeType.Static, 0xE); // Pack Header
            base.BlockIdDictionary[BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xBD }, 0)] = new BlockSizeStruct(PacketSizeType.SizeBytes, 2); // Audio Stream, two bytes following equal length (Big Endian)
            base.BlockIdDictionary[BitConverter.ToUInt32(new byte[] { 0x00, 0x00, 0x01, 0xBF }, 0)] = new BlockSizeStruct(PacketSizeType.SizeBytes, 2); // Audio Stream, two bytes following equal length (Big Endian)
        }

        protected override int GetAudioPacketHeaderSize()
        {
            return 0x11;
        }
        protected override bool SkipAudioPacketHeaderOnExtraction()
        {
            return true;
        }

        protected override int GetVideoPacketHeaderSize()
        {
            return 0;
        }
        protected override bool SkipVideoPacketHeaderOnExtraction()
        {
            return false;
        }

        protected override string GetAudioFileExtension()
        {
            return SonyPssStream.DefaultAudioExtension;
        }
        protected override string GetVideoFileExtension()
        {
            return SonyPssStream.DefaultVideoExtension;
        }

        protected override bool IsThisAnAudioBlock(byte[] blockToCheck)
        {
            return ((blockToCheck[3] >= 0xBD) && 
                    (blockToCheck[3] <= 0xDF) &&
                    (blockToCheck[3] != 0xBE));
        }
        protected override bool IsThisAVideoBlock(byte[] blockToCheck)
        {
            return ((blockToCheck[3] >= 0xE0) && (blockToCheck[3] <= 0xEF));
        }
    }
}
