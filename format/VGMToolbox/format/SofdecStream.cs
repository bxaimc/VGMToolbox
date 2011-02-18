using System;

namespace VGMToolbox.format
{
    public class SofdecStream : MpegStream
    {
        new public const string DefaultAudioExtension = ".adx";
        new public const string DefaultVideoExtension = ".m2v";

        public SofdecStream(string path): base(path) 
        {
            base.BlockIdDictionary[BitConverter.ToUInt32(MpegStream.PacketStartByes, 0)] = new BlockSizeStruct(PacketSizeType.Static, 0xC); // Pack Header
        }

        protected override int GetAudioPacketHeaderSize()
        {
            return 7;
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
            return SofdecStream.DefaultAudioExtension;
        }
        protected override string GetVideoFileExtension()
        {
            return SofdecStream.DefaultVideoExtension;
        }
    }
}
