using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VGMToolbox.format.iso
{
    public interface IFileStructure : IComparable
    {
        string SourceFilePath { set; get; }
        string FileName { set; get; }

        long VolumeBaseOffset { set; get; }
        long Lba { set; get; }
        long Size { set; get; }
        bool IsRaw { set; get; }
        int NonRawSectorSize { set; get; }

        DateTime FileDateTime { set; get; }

        void Extract(FileStream isoStream, string destinationFolder, bool extractAsRaw);
    }
}
