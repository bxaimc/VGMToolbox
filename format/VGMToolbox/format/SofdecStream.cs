using System;
using System.IO;

namespace VGMToolbox.format
{
    public class SofdecStream : MpegStream
    {
        new public const string DefaultAudioExtension = ".adx";
        new public const string DefaultVideoExtension = ".m2v";

        public SofdecStream(string path): base(path) 
        {
            this.FileExtensionAudio = DefaultAudioExtension;
            this.FileExtensionVideo = DefaultVideoExtension;
            
            base.BlockIdDictionary[BitConverter.ToUInt32(MpegStream.PacketStartByes, 0)] = new BlockSizeStruct(PacketSizeType.Static, 0xC); // Pack Header
        }

        protected override int GetAudioPacketHeaderSize(Stream readStream, long currentOffset)
        {
            return 7;
        }
    }
}
