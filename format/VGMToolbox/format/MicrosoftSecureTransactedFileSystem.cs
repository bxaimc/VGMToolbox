using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using VGMToolbox.format.iso;
using VGMToolbox.util;

namespace VGMToolbox.format
{
    public class MicrosoftSecureTransactedFileSystem
    {
        public static readonly byte[] STANDARD_IDENTIFIER = new byte[] { 0x4C, 0x49, 0x56, 0x45 };
        public const uint IDENTIFIER_OFFSET = 0x00;
        public static string FORMAT_DESCRIPTION_STRING = "Microsoft STFS";    
    }

    public class MicrosoftSecureTransactedFileSystemVolume : IVolume
    {
        public long VolumeBaseOffset { set; get; }
        public string FormatDescription { set; get; }
        public VolumeDataType VolumeType { set; get; }
        public string VolumeIdentifier { set; get; }
        public bool IsRawDump { set; get; }

        public short FileTableBlockCount { set; get; }
        public int FileTableBlockNumber { set; get; }

        public ArrayList DirectoryStructureArray { set; get; }
        public IDirectoryStructure[] Directories
        {
            set { Directories = value; }
            get
            {
                DirectoryStructureArray.Sort();
                return (IDirectoryStructure[])DirectoryStructureArray.ToArray(typeof(NintendoGameCubeDirectoryStructure));
            }        
        }

        public void Initialize(FileStream isoStream, long offset, bool isRawDump)
        {
            byte[] volumeIdentifierBytes;

            this.VolumeBaseOffset = offset;
            this.IsRawDump = isRawDump;
            this.VolumeType = VolumeDataType.Data;
            this.DirectoryStructureArray = new ArrayList();
           
            // get identifier
            volumeIdentifierBytes = ParseFile.ParseSimpleOffset(isoStream, this.VolumeBaseOffset + 0x1691, 0x80);
            this.VolumeIdentifier = Encoding.BigEndianUnicode.GetString(volumeIdentifierBytes);

            // get file table info
            this.FileTableBlockCount = ParseFile.ReadInt16BE(isoStream, this.VolumeBaseOffset + 0x37C);
            this.FileTableBlockNumber = ParseFile.ReadInt32BE(isoStream, this.VolumeBaseOffset + 0x37E);
            this.FileTableBlockNumber >>= 8;
        }

        public void ExtractAll(ref Dictionary<string, FileStream> streamCache, string destintionFolder, bool extractAsRaw)
        { 
        
        }
    }
}
