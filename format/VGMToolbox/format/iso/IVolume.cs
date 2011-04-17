using System;
using System.IO;
using System.Text;

namespace VGMToolbox.format.iso
{
    public interface IVolume
    {
        long VolumeBaseOffset { set; get; }
        string VolumeIdentifier { set; get; }
        IDirectoryStructure[] Directories { set; get; }

        void Initialize(FileStream isoStream, long offset);
        void ExtractAll(FileStream isoStream, string destintionFolder);
    }
}
