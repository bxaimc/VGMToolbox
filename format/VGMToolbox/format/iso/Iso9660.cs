using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using VGMToolbox.util;

namespace VGMToolbox.format.iso
{
    class Iso9660
    {
        public const long EMPTY_HEADER_SIZE = 0x8000;
        public static readonly byte[] STANDARD_IDENTIFIER = new byte[] { 0x43, 0x44, 0x30, 0x30, 0x31 };

        public string FileName { set; get; }

        //-------------
        // Constructor
        //-------------
        public Iso9660(string path)
        {
            this.FileName = path;
        }

        public virtual long GetStartingOffset()
        {
            return EMPTY_HEADER_SIZE;
        }

        public virtual void Initialize()
        {
            long currentOffset;
            long fileLength;

            byte descriptorType;
            byte[] standardIdentifier;
            byte volumeDescriptorVersion;

            using (FileStream fs = File.OpenRead(this.FileName))
            {
                fileLength = fs.Length;
                currentOffset = this.GetStartingOffset();

                while (currentOffset < fileLength)
                {
                    descriptorType = ParseFile.ParseSimpleOffset(fs, currentOffset, 1)[0];
                    standardIdentifier = ParseFile.ParseSimpleOffset(fs, currentOffset + 1, 5);
                    volumeDescriptorVersion = ParseFile.ParseSimpleOffset(fs, currentOffset + 6, 1)[0];


                
                
                } // while (currentOffset < fileLength)


            } // using (FileStream fs = File.OpenRead(this.FileName))
        }
    }

    public class Iso9660Volume
    {
        public byte VolumeDescriptorType { set; get; }
        public byte[] StandardIdentifier { set; get; }
        public byte VolumeDescriptorVersion { set; get; }

        public byte UnusedField1 { set; get; }

        public string SystemIdentifier { set; get; }
        public string VolumeIdentifier { set; get; }

        public byte[] UnusedField2 { set; get; }

        public uint VolumeSpaceSize { set; get; }

        public byte[] UnusedField3 { set; get; }

        public ushort VolumeSetSize { set; get; }
        public ushort VolumeSequenceNumber { set; get; }
        public ushort LogicalBlockSize { set; get; }

        public uint PathTableSize { set; get; }
        public uint LocationOfOccurrenceOfTypeLPathTable { set; get; }
        public uint LocationOfOptionalOccurrenceOfTypeLPathTable { set; get; }
        public uint LocationOfOccurrenceOfTypeMPathTable { set; get; }
        public uint LocationOfOptionalOccurrenceOfTypeMPathTable { set; get; }

        public byte[] DirectoryRecordForRootDirectory { set; get; }

        public string VolumeSetIdentifier { set; get; }
        public string PublisherIdentifier { set; get; }
        public string DataPreparerIdentifier { set; get; }
        public string ApplicationIdentifier { set; get; }
        public string CopyrightFileIdentifier { set; get; }
        public string AbstractFileIdentifier { set; get; }
        public string BibliographicFileIdentifier { set; get; }


    }
}
