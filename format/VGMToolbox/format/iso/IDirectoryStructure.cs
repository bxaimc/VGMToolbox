using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VGMToolbox.format.iso
{
    public interface IDirectoryStructure : IComparable
    {
        string SourceFilePath { set; get; }
        string DirectoryName { set; get; }

        IDirectoryStructure[] SubDirectories { set; get; }
        IFileStructure[] Files { set; get; }

        void Extract(FileStream isoStream, string destinationFolder);
    }
}
