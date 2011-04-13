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

        long Offset { set; get; }
        long Size { set; get; }

        void Extract(FileStream isoStream, string destinationFolder);
    }
}
