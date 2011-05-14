using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using VGMToolbox.util;

namespace VGMToolbox.format.iso
{
    public class NintendoGameCube
    {
        public static readonly byte[] STANDARD_IDENTIFIER = new byte[] { 0xC2, 0x33, 0x9F, 0x3D };
        public const uint IDENTIFIER_OFFSET = 0x1C;
        public static string FORMAT_DESCRIPTION_STRING = "GameCube";
    }

    public class NintendoGameCubeVolume : IVolume
    {
        public string VolumeIdentifier { set; get; }
        public string FormatDescription { set; get; }
        public long VolumeBaseOffset { set; get; }
        public bool IsRawDump { set; get; }
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

        public long RootDirectoryOffset { set; get; }
        public DateTime ImageCreationTime { set; get; }

        public long NameTableOffset { set; get; }

        public void Initialize(FileStream isoStream, long offset, bool isRawDump)
        {
            byte[] volumeIdentifierBytes;
            byte[] imageDateBytes;
            string imageDateString;

            this.VolumeBaseOffset = offset;
            this.FormatDescription = NintendoGameCube.FORMAT_DESCRIPTION_STRING;
            this.IsRawDump = isRawDump;
            this.DirectoryStructureArray = new ArrayList();

            // get identifier
            volumeIdentifierBytes = ParseFile.ParseSimpleOffset(isoStream, this.VolumeBaseOffset + 0x20, 64);
            volumeIdentifierBytes = FileUtil.ReplaceNullByteWithSpace(volumeIdentifierBytes);
            this.VolumeIdentifier = ByteConversion.GetEncodedText(volumeIdentifierBytes, ByteConversion.GetPredictedCodePageForTags(volumeIdentifierBytes)).Trim(); ;

            // get date 
            imageDateBytes = ParseFile.ParseSimpleOffset(isoStream, this.VolumeBaseOffset + 0x2440, 0xA);
            imageDateString = ByteConversion.GetAsciiText(imageDateBytes);

            try
            {
                this.ImageCreationTime = new DateTime(int.Parse(imageDateString.Substring(0, 4)),
                                                      int.Parse(imageDateString.Substring(5, 2)), 
                                                      int.Parse(imageDateString.Substring(8, 2)));
            }
            catch(Exception)
            {
                this.ImageCreationTime = new DateTime();
            }

            // get offset of file table
            this.RootDirectoryOffset = (long)ByteConversion.GetUInt32BigEndian(ParseFile.ParseSimpleOffset(isoStream, this.VolumeBaseOffset + 0x424, 4));

            this.LoadDirectories(isoStream);
        }

        public void ExtractAll(FileStream isoStream, string destinationFolder, bool extractAsRaw)
        {
            foreach (NintendoGameCubeDirectoryStructure ds in this.DirectoryStructureArray)
            {
                ds.Extract(isoStream, destinationFolder, extractAsRaw);
            }
        }

        public void LoadDirectories(FileStream isoStream)
        {
            byte[] rootDirectoryBytes;
            NintendoGameCubeDirectoryRecord rootDirectoryRecord;
            NintendoGameCubeDirectoryStructure rootDirectory;

            // Get name table offset
            rootDirectoryBytes = ParseFile.ParseSimpleOffset(isoStream, this.RootDirectoryOffset, 0xC);
            rootDirectoryRecord = new NintendoGameCubeDirectoryRecord(rootDirectoryBytes);
            this.NameTableOffset = this.RootDirectoryOffset + ((long)rootDirectoryRecord.FileSize * 0xC);

            rootDirectory = new NintendoGameCubeDirectoryStructure(isoStream,
                isoStream.Name, rootDirectoryRecord, this.ImageCreationTime, this.VolumeBaseOffset,
                this.RootDirectoryOffset, this.RootDirectoryOffset, this.NameTableOffset, String.Empty, String.Empty);

            this.DirectoryStructureArray.Add(rootDirectory);
        }
    }

    public class NintendoGameCubeFileStructure : IFileStructure
    {
        public string ParentDirectoryName { set; get; }
        public string SourceFilePath { set; get; }
        public string FileName { set; get; }

        public long VolumeBaseOffset { set; get; }
        public long Lba { set; get; }
        public long Size { set; get; }
        public bool IsRaw { set; get; }
        public CdSectorType FileMode { set; get; }
        public int NonRawSectorSize { set; get; }


        public DateTime FileDateTime { set; get; }

        public int CompareTo(object obj)
        {
            if (obj is NintendoGameCubeFileStructure)
            {
                NintendoGameCubeFileStructure o = (NintendoGameCubeFileStructure)obj;

                return this.FileName.CompareTo(o.FileName);
            }

            throw new ArgumentException("object is not an NintendoGameCubeFileStructure");
        }

        public NintendoGameCubeFileStructure(string parentDirectoryName, string sourceFilePath, string fileName, long volumeBaseOffset, uint lba, long size, DateTime fileTime)
        {
            this.ParentDirectoryName = parentDirectoryName;
            this.SourceFilePath = sourceFilePath;
            this.FileName = fileName;
            this.VolumeBaseOffset = volumeBaseOffset;
            this.Lba = lba;
            this.IsRaw = false;
            this.NonRawSectorSize = -1;
            this.Size = size;
            this.FileMode = CdSectorType.Unknown;
            this.FileDateTime = fileTime;
        }

        public void Extract(FileStream isoStream, string destinationFolder, bool extractAsRaw)
        {
            string destinationFile = Path.Combine(Path.Combine(destinationFolder, this.ParentDirectoryName), this.FileName);
            ParseFile.ExtractChunkToFile(isoStream, this.Lba, this.Size, destinationFile);
        }
    }

    public class NintendoGameCubeDirectoryStructure : IDirectoryStructure
    {
        public long DirectoryRecordOffset { set; get; }
        public string ParentDirectoryName { set; get; }

        public ArrayList SubDirectoryArray { set; get; }
        public IDirectoryStructure[] SubDirectories
        {
            set { this.SubDirectories = value; }
            get
            {
                SubDirectoryArray.Sort();
                return (IDirectoryStructure[])SubDirectoryArray.ToArray(typeof(NintendoGameCubeDirectoryStructure));
            }
        }

        public ArrayList FileArray { set; get; }
        public IFileStructure[] Files
        {
            set { this.Files = value; }
            get
            {
                FileArray.Sort();
                return (IFileStructure[])FileArray.ToArray(typeof(NintendoGameCubeFileStructure));
            }
        }

        public string SourceFilePath { set; get; }
        public string DirectoryName { set; get; }

        public int CompareTo(object obj)
        {
            if (obj is NintendoGameCubeDirectoryStructure)
            {
                NintendoGameCubeDirectoryStructure o = (NintendoGameCubeDirectoryStructure)obj;

                return this.DirectoryName.CompareTo(o.DirectoryName);
            }

            throw new ArgumentException("object is not an NintendoGameCubeDirectoryStructure");
        }

        public void Extract(FileStream isoStream, string destinationFolder, bool extractAsRaw)
        {
            string fullDirectoryPath = Path.Combine(destinationFolder, Path.Combine(this.ParentDirectoryName, this.DirectoryName));

            // create directory
            if (!Directory.Exists(fullDirectoryPath))
            {
                Directory.CreateDirectory(fullDirectoryPath);
            }

            foreach (NintendoGameCubeFileStructure f in this.FileArray)
            {
                f.Extract(isoStream, destinationFolder, extractAsRaw);
            }

            foreach (NintendoGameCubeDirectoryStructure d in this.SubDirectoryArray)
            {
                d.Extract(isoStream, destinationFolder, extractAsRaw);
            }
        }

        public NintendoGameCubeDirectoryStructure(
            FileStream isoStream,
            string sourceFilePath, 
            NintendoGameCubeDirectoryRecord directoryRecord, 
            DateTime creationDateTime,
            long baseOffset,
            long rootDirectoryOffset,
            long directoryOffset, 
            long nameTableOffset,
            string directoryName, 
            string parentDirectory)
        {
            string nextDirectory;
            this.SourceFilePath = SourceFilePath;
            this.SubDirectoryArray = new ArrayList();
            this.FileArray = new ArrayList();
            this.DirectoryRecordOffset = directoryOffset;

            if (String.IsNullOrEmpty(parentDirectory))
            {
                this.ParentDirectoryName = String.Empty;
                this.DirectoryName = directoryName;
                nextDirectory = this.DirectoryName;
            }
            else
            {
                this.ParentDirectoryName = parentDirectory;
                this.DirectoryName = directoryName;
                nextDirectory = this.ParentDirectoryName + Path.DirectorySeparatorChar + this.DirectoryName;
            }

            this.parseDirectoryRecord(isoStream, directoryRecord, creationDateTime, baseOffset, rootDirectoryOffset, directoryOffset, nameTableOffset, nextDirectory);
        }


        private void parseDirectoryRecord(
            FileStream isoStream,
            NintendoGameCubeDirectoryRecord directoryRecord,
            DateTime creationDateTime,
            long baseOffset,
            long rootDirectoryOffset,
            long directoryOffset,
            long nameTableOffset,
            string parentDirectory)
        {
            long directoryRecordEndOffset;
            long newDirectoryEndOffset;
            long currentOffset = baseOffset + directoryOffset;

            int itemNameSize;
            byte[] itemNameBytes;
            string itemName;

            NintendoGameCubeDirectoryRecord newDirectoryRecord;
            NintendoGameCubeDirectoryStructure newDirectory;
            NintendoGameCubeFileStructure newFile;

            directoryRecordEndOffset = rootDirectoryOffset + (directoryRecord.FileSize * 0xC);
            currentOffset += 0xC;

            while (currentOffset < directoryRecordEndOffset)
            {
                newDirectoryRecord = new NintendoGameCubeDirectoryRecord(ParseFile.ParseSimpleOffset(isoStream, currentOffset, 0xC));

                itemNameSize = ParseFile.GetSegmentLength(isoStream, (int)(nameTableOffset + newDirectoryRecord.NameOffset), Constants.NullByteArray);
                itemNameBytes = ParseFile.ParseSimpleOffset(isoStream, nameTableOffset + newDirectoryRecord.NameOffset, itemNameSize);
                itemName = ByteConversion.GetEncodedText(itemNameBytes, ByteConversion.GetPredictedCodePageForTags(itemNameBytes));                
                
                if (!newDirectoryRecord.IsDirectory)
                {
                    newFile = new NintendoGameCubeFileStructure(parentDirectory,
                        this.SourceFilePath, itemName,
                        baseOffset, newDirectoryRecord.FileOffset,
                        newDirectoryRecord.FileSize, creationDateTime);

                    this.FileArray.Add(newFile);
                    currentOffset += 0xC;         
                }
                else
                {
                    newDirectory = 
                        new NintendoGameCubeDirectoryStructure(isoStream, 
                            isoStream.Name, newDirectoryRecord,
                            creationDateTime, baseOffset, rootDirectoryOffset,
                            currentOffset, nameTableOffset,
                            itemName, parentDirectory);
                    
                    this.SubDirectoryArray.Add(newDirectory);

                    newDirectoryEndOffset = rootDirectoryOffset + (newDirectoryRecord.FileSize * 0xC);
                    currentOffset = newDirectoryEndOffset;         
                }
            }
        }
    }

    public class NintendoGameCubeDirectoryRecord
    {
        public uint NameOffset { set; get; }
        public uint FileOffset { set; get; }
        public uint FileSize { set; get; }
        public bool IsDirectory { set; get; }
              
        public NintendoGameCubeDirectoryRecord(byte[] directoryBytes)
        {
            this.NameOffset = ByteConversion.GetUInt32BigEndian(ParseFile.ParseSimpleOffset(directoryBytes, 0, 4));            
            this.FileOffset = ByteConversion.GetUInt32BigEndian(ParseFile.ParseSimpleOffset(directoryBytes, 4, 4));
            this.FileSize = ByteConversion.GetUInt32BigEndian(ParseFile.ParseSimpleOffset(directoryBytes, 8, 4));
            this.IsDirectory = ((this.NameOffset & 0xFF000000) != 0);

            this.NameOffset &= 0xFFFFFF;
        }
    }
}
