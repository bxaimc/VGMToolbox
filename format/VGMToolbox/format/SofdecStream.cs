using System;
using System.IO;

namespace VGMToolbox.format
{
    public class SofdecStream : Mpeg1Stream
    {
        new public const string DefaultAudioExtension = ".adx";
        new public const string DefaultVideoExtension = ".m2v";

        public SofdecStream(string path): base(path) 
        {
            this.FileExtensionAudio = DefaultAudioExtension;
            this.FileExtensionVideo = DefaultVideoExtension;          
        }
    }
}
