using System;
using System.IO;

namespace VGMToolbox.format
{
    public class Mpeg1Stream : Mpeg2Stream
    {
        new public const string DefaultAudioExtension = ".mp2";
        new public const string DefaultVideoExtension = ".m1v";

        public Mpeg1Stream(string path)
            : base(path)
        {
            this.FileExtensionAudio = DefaultAudioExtension;
            this.FileExtensionVideo = DefaultVideoExtension;

            base.BlockIdDictionary[BitConverter.ToUInt32(Mpeg2Stream.PacketStartByes, 0)] = new BlockSizeStruct(PacketSizeType.Static, 0xC); // Pack Header
        }

        protected override int GetAudioPacketHeaderSize(Stream readStream, long currentOffset)
        {
            return 7;
        }
    }
}
