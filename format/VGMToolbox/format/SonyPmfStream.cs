using System;

namespace VGMToolbox.format
{
    public class SonyPmfStream : MpegStream
    {
        new public const string DefaultAudioExtension = ".at3";
        new public const string DefaultVideoExtension = ".avi";

        public SonyPmfStream(string path)
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
            return SonyPmfStream.DefaultAudioExtension;
        }
        protected override string GetVideoFileExtension()
        {
            return SonyPmfStream.DefaultVideoExtension;
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
