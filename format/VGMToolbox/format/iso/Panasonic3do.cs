using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VGMToolbox.util;

namespace VGMToolbox.format.iso
{
    public class Panasonic3do
    {
        public static readonly byte[] STANDARD_IDENTIFIER =
            new byte[] { 0x01, 0x5A, 0x5A, 0x5A, 0x5A, 0x5A, 0x01, 0x00, 
                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                         0x43, 0x44, 0x2D, 0x52, 0x4F, 0x4D, 0x00, 0x00, 
                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        public static uint SECTOR_SIZE = 0x800;
        public static string FORMAT_DESCRIPTION_STRING = "3DO";        
    }

    public class Panasonic3doVolume : IVolume
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
                    return (IDirectoryStructure[])DirectoryStructureArray.ToArray(typeof(XDvdFsDirectoryStructure));
                }
            }

            public uint RootDirectoryCount { set; get; }
            public uint[] RootDirectoryLbas { set; get; }
            
            public DateTime ImageCreationTime { set; get; }

            public void Initialize(FileStream isoStream, long offset, bool isRawDump)
            {                
                byte[] rootSector;

                this.VolumeBaseOffset = offset;
                this.FormatDescription = Panasonic3do.FORMAT_DESCRIPTION_STRING;
                this.IsRawDump = isRawDump;
                this.DirectoryStructureArray = new ArrayList();

                rootSector = CdRom.GetSectorByLba(isoStream, this.VolumeBaseOffset, 0, this.IsRawDump, (int)Panasonic3do.SECTOR_SIZE);

                this.RootDirectoryCount = ByteConversion.GetUInt32BigEndian(ParseFile.ParseSimpleOffset(rootSector, 0x60, 4)) + 1;
                this.RootDirectoryLbas = new uint[this.RootDirectoryCount];

                for (uint i = 0; i < this.RootDirectoryCount; i++)
                { 
                    this.RootDirectoryLbas[i] = 
                        ByteConversion.GetUInt32BigEndian(ParseFile.ParseSimpleOffset(rootSector, 0x64 +  (i * 4), 4));
                }

                
                // Build Tree from Root Directory
                rootDirectoryOffset = this.VolumeBaseOffset + (this.RootDirectorySector * XDvdFs.SECTOR_SIZE);
                rootDir = new XDvdFsDirectoryStructure(isoStream, isoStream.Name, this.ImageCreationTime, this.VolumeBaseOffset, rootDirectoryOffset, XDvdFs.SECTOR_SIZE, String.Empty, String.Empty);
                this.DirectoryStructureArray.Add(rootDir);
            }

            public void ExtractAll(FileStream isoStream, string destintionFolder)
            {
                foreach (XDvdFsDirectoryStructure ds in this.DirectoryStructureArray)
                {
                    ds.Extract(isoStream, destintionFolder);
                }
            }

            public void LoadDirectories(FileStream isoStream)
            {
                // change name of top level folder
                this.DirectoryRecordForRootDirectory.FileIdentifierString = String.Empty;

                // populate this volume's directory structure
                Iso9660DirectoryStructure rootDirectory =
                    new Iso9660DirectoryStructure(isoStream, isoStream.Name,
                        this.VolumeBaseOffset, this.DirectoryRecordForRootDirectory,
                        this.LogicalBlockSize, this.IsRawDump, this.SectorSize, null);
                this.DirectoryStructureArray.Add(rootDirectory);
            }
        }

    public class Panasonic3doFileStructure : IFileStructure
    {
        public string ParentDirectoryName { set; get; }
        public string SourceFilePath { set; get; }
        public string FileName { set; get; }
        
        public long VolumeBaseOffset { set; get; }
        public long Lba { set; get; }
        public long Size { set; get; }
        public bool IsRaw { set; get; }
        public int NonRawSectorSize { set; get; }


        public DateTime FileDateTime { set; get; }

        public int CompareTo(object obj)
        {
            if (obj is Panasonic3doFileStructure)
            {
                Panasonic3doFileStructure o = (Panasonic3doFileStructure)obj;

                return this.FileName.CompareTo(o.FileName);
            }

            throw new ArgumentException("object is not an Panasonic3doFileStructure");
        }

        public Panasonic3doFileStructure(string parentDirectoryName, string sourceFilePath, string fileName, long lba, long size, DateTime fileTime)
        {
            this.ParentDirectoryName = parentDirectoryName;
            this.SourceFilePath = sourceFilePath;
            this.FileName = fileName;
            this.Lba = lba;
            this.Size = size;
            this.FileDateTime = fileTime;
            this.NonRawSectorSize = (int)Panasonic3do.SECTOR_SIZE;
        }

        public void Extract(FileStream isoStream, string destinationFolder)
        {
            string destinationFile = Path.Combine(Path.Combine(destinationFolder, this.ParentDirectoryName), this.FileName);
            CdRom.ExtractCdData(isoStream, destinationFile, this.VolumeBaseOffset, this.Lba, this.Size, this.IsRaw, this.NonRawSectorSize);
        }
    }

    public class Panasonic3doDirectoryStructure : IDirectoryStructure
    {
        // public Panasonic3doDirectoryRecord ParentDirectoryRecord { set; get; }
        public long DirectoryRecordOffset { set; get; }
        public string ParentDirectoryName { set; get; }

        public ArrayList SubDirectoryArray { set; get; }
        public IDirectoryStructure[] SubDirectories
        {
            set { this.SubDirectories = value; }
            get
            {
                SubDirectoryArray.Sort();
                return (IDirectoryStructure[])SubDirectoryArray.ToArray(typeof(Panasonic3doDirectoryStructure));
            }
        }

        public ArrayList FileArray { set; get; }
        public IFileStructure[] Files
        {
            set { this.Files = value; }
            get
            {
                FileArray.Sort();
                return (IFileStructure[])FileArray.ToArray(typeof(Panasonic3doFileStructure));
            }
        }

        public string SourceFilePath { set; get; }
        public string DirectoryName { set; get; }

        public int CompareTo(object obj)
        {
            if (obj is Panasonic3doDirectoryStructure)
            {
                Panasonic3doDirectoryStructure o = (Panasonic3doDirectoryStructure)obj;

                return this.DirectoryName.CompareTo(o.DirectoryName);
            }

            throw new ArgumentException("object is not an Panasonic3doDirectoryStructure");
        }

        public Panasonic3doDirectoryStructure(FileStream isoStream, string sourceFilePath, 
            long baseOffset, Iso9660DirectoryRecord directoryRecord, 
            uint logicalBlockSize, bool isRaw, int nonRawSectorSize, 
            string parentDirectory)
        {
            string nextDirectory;
            this.SourceFilePath = SourceFilePath;
            this.SubDirectoryArray = new ArrayList();
            this.FileArray = new ArrayList();            

            if (String.IsNullOrEmpty(parentDirectory))
            {
                this.ParentDirectoryName = String.Empty;
                this.DirectoryName = directoryRecord.FileIdentifierString;
                nextDirectory = this.DirectoryName;
            }
            else
            {
                this.ParentDirectoryName = parentDirectory;
                this.DirectoryName = directoryRecord.FileIdentifierString;
                nextDirectory = this.ParentDirectoryName + Path.DirectorySeparatorChar + this.DirectoryName;
            }

            this.parseDirectoryRecord(isoStream, baseOffset, directoryRecord, logicalBlockSize, isRaw, nonRawSectorSize, nextDirectory);            
        }

        public void Extract(FileStream isoStream, string destinationFolder)
        {
            string fullDirectoryPath = Path.Combine(destinationFolder, Path.Combine(this.ParentDirectoryName, this.DirectoryName));
            
            // create directory
            if (!Directory.Exists(fullDirectoryPath))
            {
                Directory.CreateDirectory(fullDirectoryPath);
            }

            foreach (Panasonic3doFileStructure f in this.FileArray)
            { 
                f.Extract(isoStream, destinationFolder);
            }

            foreach (Panasonic3doDirectoryStructure d in this.SubDirectoryArray)
            {
                d.Extract(isoStream, destinationFolder);
            }
        }

        private void parseDirectoryRecord(FileStream isoStream, long baseOffset, 
            long directoryLba, uint logicalBlockSize, 
            bool isRaw, int nonRawSectorSize, string parentDirectory)
        {
            byte recordSize;
            int currentOffset;
            uint bytesRead = 0;
            uint currentLba = directoryRecord.LocationOfExtent;
            byte[] directoryRecordBytes;
            Iso9660DirectoryRecord tempDirectoryRecord;
            Iso9660DirectoryStructure tempDirectory;
            Iso9660FileStructure tempFile;

            byte[] directorySector = CdRom.GetSectorByLba(isoStream, baseOffset, currentLba, isRaw, nonRawSectorSize);
            directorySector = CdRom.GetDataChunkFromSector(directorySector, isRaw);
            
            currentOffset = 0;

            while (bytesRead < directoryRecord.DataLength)
            {
                recordSize = ParseFile.ParseSimpleOffset(directorySector, currentOffset, 1)[0];

                if (recordSize > 0)
                {
                    directoryRecordBytes = ParseFile.ParseSimpleOffset(directorySector, currentOffset, recordSize);
                    tempDirectoryRecord = new Iso9660DirectoryRecord(directoryRecordBytes);

                    if (!tempDirectoryRecord.FileIdentifierString.Equals(".") &&
                        !tempDirectoryRecord.FileIdentifierString.Equals("..")) // skip "this" directory
                    {
                        if (tempDirectoryRecord.FlagDirectory)
                        {
                            tempDirectory = new Iso9660DirectoryStructure(isoStream, isoStream.Name, baseOffset, tempDirectoryRecord, logicalBlockSize, isRaw, nonRawSectorSize, parentDirectory);
                            this.SubDirectoryArray.Add(tempDirectory);
                        }
                        else
                        {
                            tempFile = new Iso9660FileStructure(parentDirectory,
                                this.SourceFilePath,
                                tempDirectoryRecord.FileIdentifierString.Replace(";1", String.Empty),
                                baseOffset,
                                tempDirectoryRecord.LocationOfExtent,
                                tempDirectoryRecord.DataLength,
                                isRaw,
                                nonRawSectorSize,
                                tempDirectoryRecord.RecordingDateAndTime);
                            this.FileArray.Add(tempFile);
                        }
                    }
                    else if (tempDirectoryRecord.FileIdentifierString.Equals(".."))
                    {
                        this.ParentDirectoryRecord = tempDirectoryRecord;
                    }

                    bytesRead += recordSize;
                    currentOffset += recordSize;
                }
                else if ((directoryRecord.DataLength - bytesRead) > (directorySector.Length - currentOffset))
                {
                    // move to start of next sector
                    directorySector = CdRom.GetSectorByLba(isoStream, baseOffset, ++currentLba, isRaw, nonRawSectorSize);
                    directorySector = CdRom.GetDataChunkFromSector(directorySector, isRaw);
                    bytesRead += (uint)(logicalBlockSize - currentOffset);
                    currentOffset = 0;                
                }
                else
                {
                    break;
                }
            }
        }        
    }

    public class Panasonic3doDirectoryRecord
    {
        // each dir folder (not item) starts with FFFFFFFF, but if it is 00000001 for example, then it will have n + 1 sectors (2 in the example)
        
        public byte[] DirectoryItemTypeBytes { set; get; } // 02 is file, 07 is dir, byte 0 == C0 means last in dir, byte 0 == 40 means dir continues in next sector        
        public byte[] Unknown1 { set; get; }
        public byte[] DirectoryItemTypeTextFlagBytes { set; get; } // 0x20202020 file, *dir directory
        public string DirectoryItemTypeTextFlag { set; get; } // 0x20202020 file, *dir directory
        public uint DirectoryItemLogicalBlockSize { set; get; }
        public uint DirectoryItemSize { set; get; } // filesize or 0x800 * sector count for dir
        public uint DirectoryItemSizeInSectors { set; get; }
        public byte[] Unknown2 { set; get; } // always 0x00000001?
        public byte[] Unknown3 { set; get; } // always 0x00000000?
        public byte[] DirectoryItemNameBytes { set; get; } // constant length of 0x20, null terminated
        public string DirectoryItemName { set; get; }
        public uint SubDirectoryCount { set; get; } // count - 1 on disc, always 0x00000000 for files?
        public uint[] SubDirectoryLbas { set; get; } // 4 bytes each, for total of SubDirectoryCount LBAs
        
        public Panasonic3doDirectoryRecord(byte[] directoryBytes)
        {
            this.DirectoryItemTypeBytes = ParseFile.ParseSimpleOffset(directoryBytes, 0, 4);
            this.Unknown1  = ParseFile.ParseSimpleOffset(directoryBytes, 4, 4);
            this.DirectoryItemTypeTextFlagBytes = ParseFile.ParseSimpleOffset(directoryBytes, 8, 4);
            this.DirectoryItemTypeTextFlag = 
                ByteConversion.GetEncodedText(this.DirectoryItemTypeTextFlagBytes, ByteConversion.GetPredictedCodePageForTags(this.DirectoryItemTypeTextFlagBytes));
            this.DirectoryItemLogicalBlockSize = ByteConversion.GetUInt32BigEndian(ParseFile.ParseSimpleOffset(directoryBytes, 0xC, 4));
            this.DirectoryItemSize = ByteConversion.GetUInt32BigEndian(ParseFile.ParseSimpleOffset(directoryBytes, 0x10, 4));
            this.DirectoryItemSizeInSectors = ByteConversion.GetUInt32BigEndian(ParseFile.ParseSimpleOffset(directoryBytes, 0x14, 4));
            this.Unknown2 = ParseFile.ParseSimpleOffset(directoryBytes, 0x18, 4);
            this.Unknown3 = ParseFile.ParseSimpleOffset(directoryBytes, 0x1C, 4);
            this.DirectoryItemNameBytes = ParseFile.ParseSimpleOffset(directoryBytes, 0x20, 0x20);
            this.DirectoryItemName =
                ByteConversion.GetEncodedText(this.DirectoryItemNameBytes, ByteConversion.GetPredictedCodePageForTags(this.DirectoryItemNameBytes));
            this.SubDirectoryCount = ByteConversion.GetUInt32BigEndian(ParseFile.ParseSimpleOffset(directoryBytes, 0x40, 4)) + 1;
            this.SubDirectoryLbas = new uint[this.SubDirectoryCount];

            for (uint i = 0; i < this.RootDirectoryCount; i++)
            {
                this.SubDirectoryLbas[i] =
                    ByteConversion.GetUInt32BigEndian(ParseFile.ParseSimpleOffset(directoryBytes, 0x44 + (i * 4), 4));
            }            
        }
    }
}
