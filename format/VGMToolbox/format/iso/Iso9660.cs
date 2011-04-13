using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Text;

using VGMToolbox.util;

namespace VGMToolbox.format.iso
{
    public class Iso9660
    {
        public const long EMPTY_HEADER_SIZE = 0x8000;        
        public static readonly byte[] STANDARD_IDENTIFIER = new byte[] { 0x43, 0x44, 0x30, 0x30, 0x31 };
        public static readonly byte[] EMPTY_DATETIME = new byte[] { 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 
                                                                    0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x00 };
        public static readonly byte[] VOLUME_DESCRIPTOR_IDENTIFIER = new byte[] { 0x01, 0x43, 0x44, 0x30, 0x30, 0x31 };

        public static DateTime GetIsoDateTime(byte[] isoDateArray)
        {
            DateTime dateValue = new DateTime();
            string dateString;

            if (ParseFile.CompareSegment(isoDateArray, 0, EMPTY_DATETIME))
            {
                dateValue = DateTime.MinValue;
            }
            else
            {
                dateString = ByteConversion.GetAsciiText(isoDateArray);
                dateValue = new DateTime(Int32.Parse(dateString.Substring(0, 4)),
                                         Int16.Parse(dateString.Substring(4, 2).ToString()),
                                         Int16.Parse(dateString.Substring(6, 2).ToString()),
                                         Int16.Parse(dateString.Substring(8, 2).ToString()),
                                         Int16.Parse(dateString.Substring(10, 2).ToString()),
                                         Int16.Parse(dateString.Substring(12, 2).ToString()),
                                         Int16.Parse(dateString.Substring(14, 2).ToString()));
            }

            return dateValue;
        }

        public static IVolume[] GetVolumes(string isoPath)
        {
            ArrayList volumeList = new ArrayList();
            Iso9660Volume volume;
            long volumeOffset;
            string outputPath;

            using (FileStream fs = File.OpenRead(isoPath))
            {
                volumeOffset = ParseFile.GetNextOffset(fs, 0, Iso9660.VOLUME_DESCRIPTOR_IDENTIFIER);

                while (volumeOffset > -1)
                {
                    volume = new Iso9660Volume();
                    volume.Initialize(fs, volumeOffset);
                    volumeList.Add(volume);

                    //outputPath = Path.Combine(Path.Combine(Path.GetDirectoryName(pPath),
                    //    String.Format("VOLUME_{0}", volume.VolumeSequenceNumber.ToString("X4"))), volume.VolumeIdentifier);

                    //volume.ExtractAll(fs, outputPath);

                    volumeOffset = ParseFile.GetNextOffset(fs, volume.VolumeBaseOffset + (volume.VolumeSpaceSize * volume.LogicalBlockSize), Iso9660.VOLUME_DESCRIPTOR_IDENTIFIER);
                }
            }

            return (IVolume[])volumeList.ToArray(typeof(Iso9660Volume));
        }
    }

    public class Iso9660Volume : IVolume
    {
        public long VolumeBaseOffset { set; get; }
        public ArrayList DirectoryStructureArray { set; get; }
        public IDirectoryStructure[] Directories 
        {
            set { Directories = value; }
            get 
            {
                DirectoryStructureArray.Sort();
                return (IDirectoryStructure[])DirectoryStructureArray.ToArray(typeof(Iso9660DirectoryStructure)); 
            }
        }

        #region Standard Attributes

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

        public byte[] DirectoryRecordForRootDirectoryBytes { set; get; }
        public Iso9660DirectoryRecord DirectoryRecordForRootDirectory { set; get; }

        public string VolumeSetIdentifier { set; get; }
        public string PublisherIdentifier { set; get; }
        public string DataPreparerIdentifier { set; get; }
        public string ApplicationIdentifier { set; get; }
        public string CopyrightFileIdentifier { set; get; }
        public string AbstractFileIdentifier { set; get; }
        public string BibliographicFileIdentifier { set; get; }

        public DateTime VolumeCreationDateAndTime { set; get; }
        public DateTime VolumeModificationDateAndTime { set; get; }
        public DateTime VolumeExpirationDateAndTime { set; get; }
        public DateTime VolumeEffectiveDateAndTime { set; get; }

        public byte FileStructureVersion { set; get; }

        public byte Reserved1 { set; get; }

        public byte[] ApplicationUse { set; get; }

        public byte[] Reserved2 { set; get; }

        #endregion

        public void Initialize(FileStream isoStream, long offset)
        {
            this.VolumeBaseOffset = offset - Iso9660.EMPTY_HEADER_SIZE;
            this.DirectoryStructureArray = new ArrayList();

            this.VolumeDescriptorType = ParseFile.ParseSimpleOffset(isoStream, offset + 0x00, 1)[0];
            this.StandardIdentifier = ParseFile.ParseSimpleOffset(isoStream, offset + 0x01, 5);
            this.VolumeDescriptorVersion = ParseFile.ParseSimpleOffset(isoStream, offset + 0x06, 1)[0];

            this.UnusedField1 = ParseFile.ParseSimpleOffset(isoStream, offset + 0x07, 1)[0];
            
            this.SystemIdentifier = ByteConversion.GetAsciiText(ParseFile.ParseSimpleOffset(isoStream, offset + 0x08, 0x20)).Trim();
            this.VolumeIdentifier = ByteConversion.GetAsciiText(ParseFile.ParseSimpleOffset(isoStream, offset + 0x28, 0x20)).Trim();

            this.UnusedField2 = ParseFile.ParseSimpleOffset(isoStream, offset + 0x48, 0x08);

            this.VolumeSpaceSize = BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(isoStream, offset + 0x50, 0x04), 0);

            this.UnusedField3 = ParseFile.ParseSimpleOffset(isoStream, offset + 0x58, 0x20);

            this.VolumeSetSize = BitConverter.ToUInt16(ParseFile.ParseSimpleOffset(isoStream, offset + 0x78, 0x02), 0);
            this.VolumeSequenceNumber = BitConverter.ToUInt16(ParseFile.ParseSimpleOffset(isoStream, offset + 0x7C, 0x02), 0);
            this.LogicalBlockSize = BitConverter.ToUInt16(ParseFile.ParseSimpleOffset(isoStream, offset + 0x80, 0x02), 0);

            this.PathTableSize = BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(isoStream, offset + 0x84, 0x04), 0);
            this.LocationOfOccurrenceOfTypeLPathTable = BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(isoStream, offset + 0x8C, 0x04), 0);
            this.LocationOfOptionalOccurrenceOfTypeLPathTable = BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(isoStream, offset + 0x90, 0x04), 0);
            this.LocationOfOccurrenceOfTypeMPathTable = ByteConversion.GetUInt32BigEndian(ParseFile.ParseSimpleOffset(isoStream, offset + 0x94, 0x04));
            this.LocationOfOptionalOccurrenceOfTypeMPathTable = ByteConversion.GetUInt32BigEndian(ParseFile.ParseSimpleOffset(isoStream, offset + 0x98, 0x04));

            this.DirectoryRecordForRootDirectoryBytes = ParseFile.ParseSimpleOffset(isoStream, offset + 0x9C, 0x22);
            this.DirectoryRecordForRootDirectory = new Iso9660DirectoryRecord(this.DirectoryRecordForRootDirectoryBytes);

            this.VolumeSetIdentifier = ByteConversion.GetAsciiText(ParseFile.ParseSimpleOffset(isoStream, offset + 0xBE, 0x80)).Trim();
            this.PublisherIdentifier = ByteConversion.GetAsciiText(ParseFile.ParseSimpleOffset(isoStream, offset + 0x13E, 0x80)).Trim();
            this.DataPreparerIdentifier = ByteConversion.GetAsciiText(ParseFile.ParseSimpleOffset(isoStream, offset + 0x1BE, 0x80)).Trim();
            this.ApplicationIdentifier = ByteConversion.GetAsciiText(ParseFile.ParseSimpleOffset(isoStream, offset + 0x23E, 0x80)).Trim();
            this.CopyrightFileIdentifier = ByteConversion.GetAsciiText(ParseFile.ParseSimpleOffset(isoStream, offset + 0x2BE, 0x25)).Trim();
            this.AbstractFileIdentifier = ByteConversion.GetAsciiText(ParseFile.ParseSimpleOffset(isoStream, offset + 0x2E3, 0x25)).Trim();
            this.BibliographicFileIdentifier = ByteConversion.GetAsciiText(ParseFile.ParseSimpleOffset(isoStream, offset + 0x308, 0x25)).Trim();

            this.VolumeCreationDateAndTime = Iso9660.GetIsoDateTime(ParseFile.ParseSimpleOffset(isoStream, offset + 0x32D, 0x11));
            this.VolumeModificationDateAndTime = Iso9660.GetIsoDateTime(ParseFile.ParseSimpleOffset(isoStream, offset + 0x33E, 0x11));
            this.VolumeExpirationDateAndTime = Iso9660.GetIsoDateTime(ParseFile.ParseSimpleOffset(isoStream, offset + 0x34F, 0x11));
            this.VolumeEffectiveDateAndTime = Iso9660.GetIsoDateTime(ParseFile.ParseSimpleOffset(isoStream, offset + 0x360, 0x11));           
            
            this.FileStructureVersion = ParseFile.ParseSimpleOffset(isoStream, offset + 0x371, 1)[0];

            this.Reserved1 = ParseFile.ParseSimpleOffset(isoStream, offset + 0x372, 1)[0];

            this.ApplicationUse = ParseFile.ParseSimpleOffset(isoStream, offset + 0x373, 0x200);

            this.Reserved2 = ParseFile.ParseSimpleOffset(isoStream, offset + 0x573, 0x28D);

            this.LoadDirectories(isoStream);
        }

        public void ExtractAll(FileStream isoStream, string destintionFolder)
        {
            foreach (Iso9660DirectoryStructure ds in this.DirectoryStructureArray)
            {
                ds.Extract(isoStream, destintionFolder);
            }
        }

        public void LoadDirectories(FileStream isoStream)
        {
            // change name of top level folder
            this.DirectoryRecordForRootDirectory.FileIdentifierString = String.Empty;

            // populate this volume's directory structure
            Iso9660DirectoryStructure rootDirectory = new Iso9660DirectoryStructure(isoStream, isoStream.Name, this.VolumeBaseOffset, this.DirectoryRecordForRootDirectory, this.LogicalBlockSize, null);
            this.DirectoryStructureArray.Add(rootDirectory);
        }
    }

    public class Iso9660DirectoryRecord
    {
        public byte LengthOfDirectoryRecord { set; get; }
        public byte ExtendedAttributeRecordLength { set; get; }
        public uint LocationOfExtent { set; get; }
        public uint DataLength { set; get; }
        public DateTime RecordingDateAndTime { set; get; }
        public byte FileFlags { set; get; }
        public byte FileUnitSize { set; get; }
        public byte InterleaveGapSize { set; get; }
        public ushort VolumeSequenceNumber { set; get; }
        public byte LengthOfFileIdentifier { set; get; }

        public byte[] FileIdentifier { set; get; }
        public string FileIdentifierString { set; get; }
        
        public byte[] PaddingField { set; get; }
        public byte[] SystemUse { set; get; }

        public bool FlagExistance { set; get; }
        public bool FlagDirectory { set; get; }
        public bool FlagAssociatedFile { set; get; }
        public bool FlagRecord { set; get; }
        public bool FlagProtection { set; get; }
        public bool FlagMultiExtent { set; get; }


        public Iso9660DirectoryRecord(byte[] directoryBytes)
        {
            this.LengthOfDirectoryRecord = directoryBytes[0];
            this.ExtendedAttributeRecordLength = directoryBytes[1];
            this.LocationOfExtent = BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(directoryBytes, 2, 4), 0);
            this.DataLength = BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(directoryBytes, 0x0A, 4), 0);

            this.RecordingDateAndTime = new DateTime(directoryBytes[0x12] + 1900,
                                                     directoryBytes[0x13],
                                                     directoryBytes[0x14],
                                                     directoryBytes[0x15],
                                                     directoryBytes[0x16],
                                                     directoryBytes[0x17]);

            this.FileFlags = directoryBytes[0x19];
            
            this.FlagExistance = (this.FileFlags & 0x1) == 0x1? true: false;
            this.FlagDirectory = (this.FileFlags & 0x2) == 0x2? true: false;
            this.FlagAssociatedFile = (this.FileFlags & 0x4) == 0x4? true: false;
            this.FlagRecord = (this.FileFlags & 0x08) == 0x08? true: false;
            this.FlagProtection = (this.FileFlags & 0x10) == 0x10? true: false;
            this.FlagMultiExtent = (this.FileFlags & 0x80) == 0x80? true : false;
            
            this.FileUnitSize = directoryBytes[0x1A];            
            this.InterleaveGapSize = directoryBytes[0x1B];
            this.VolumeSequenceNumber = BitConverter.ToUInt16(ParseFile.ParseSimpleOffset(directoryBytes, 0x1C, 2), 0);
            this.LengthOfFileIdentifier = directoryBytes[0x20];

            this.FileIdentifier = ParseFile.ParseSimpleOffset(directoryBytes, 0x21, this.LengthOfFileIdentifier);

            // parse identifier
            if (this.LengthOfFileIdentifier > 1)
            {
                this.FileIdentifierString = ByteConversion.GetAsciiText(this.FileIdentifier);
            }
            else if (this.FileIdentifier[0] == 0)
            {
                this.FileIdentifierString = ".";
            }
            else if (this.FileIdentifier[0] == 1)
            {
                this.FileIdentifierString = "..";
            }

            /*
            
            public byte[] PaddingField { set; get; }
            public byte[] SystemUse { set; get; }        
            */ 
        }
    }

    public class Iso9660FileStructure : IFileStructure
    {
        public string ParentDirectoryName { set; get; }
        public string SourceFilePath { set; get; }
        public string FileName { set; get; }
        public long Offset { set; get; }
        public long Size { set; get; }

        public int CompareTo(object obj)
        {
            if (obj is Iso9660FileStructure)
            {
                Iso9660FileStructure o = (Iso9660FileStructure)obj;

                return this.FileName.CompareTo(o.FileName);
            }

            throw new ArgumentException("object is not an Iso9660FileStructure");
        }    

        public Iso9660FileStructure(string parentDirectoryName, string sourceFilePath, string fileName, long offset, long size)
        { 
            this.ParentDirectoryName = parentDirectoryName;
            this.SourceFilePath = sourceFilePath;
            this.FileName = fileName;
            this.Offset = offset;
            this.Size = size;        
        }

        public void Extract(FileStream isoStream, string destinationFolder)
        {
            string destinationFile = Path.Combine(Path.Combine(destinationFolder, this.ParentDirectoryName), this.FileName);
            ParseFile.ExtractChunkToFile(isoStream, this.Offset, this.Size, destinationFile);
        }
    }

    public class Iso9660DirectoryStructure : IDirectoryStructure
    {
        public Iso9660DirectoryRecord ParentDirectoryRecord { set; get; }
        public string ParentDirectoryName { set; get; }
        
        public ArrayList SubDirectoryArray { set; get; }
        public IDirectoryStructure[] SubDirectories {
            set { this.SubDirectories = value; }
            get 
            {
                //SubDirectoryArray.Sort();
                return (IDirectoryStructure[])SubDirectoryArray.ToArray(typeof(Iso9660DirectoryStructure)); 
            }
        }
        
        public ArrayList FileArray { set; get; }
        public IFileStructure[] Files
        {
            set { this.Files = value; }
            get 
            {
                FileArray.Sort();
                return (IFileStructure[])FileArray.ToArray(typeof(Iso9660FileStructure)); 
            }
        }

        public string SourceFilePath { set; get; }
        public string DirectoryName { set; get; }

        public int CompareTo(object obj)
        {
            if (obj is Iso9660DirectoryStructure)
            {
                Iso9660DirectoryStructure o = (Iso9660DirectoryStructure)obj;

                return this.DirectoryName.CompareTo(o.DirectoryName);
            }

            throw new ArgumentException("object is not an Iso9660DirectoryStructure");
        }    

        public Iso9660DirectoryStructure(FileStream isoStream, string sourceFilePath, long baseOffset, Iso9660DirectoryRecord directoryRecord, uint logicalBlockSize, string parentDirectory)
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

            this.parseDirectoryRecord(isoStream, baseOffset, directoryRecord, logicalBlockSize, nextDirectory);            
        }

        public void Extract(FileStream isoStream, string destinationFolder)
        {
            foreach (Iso9660FileStructure f in this.FileArray)
            { 
                f.Extract(isoStream, destinationFolder);
            }

            foreach (Iso9660DirectoryStructure d in this.SubDirectoryArray)
            {
                d.Extract(isoStream, destinationFolder);
            }
        }

        private void parseDirectoryRecord(FileStream isoStream, long baseOffset, Iso9660DirectoryRecord directoryRecord, uint logicalBlockSize, string parentDirectory)
        {
            byte recordSize;
            long currentOffset;
            byte[] directoryRecordBytes;
            Iso9660DirectoryRecord tempDirectoryRecord;
            Iso9660DirectoryStructure tempDirectory;
            Iso9660FileStructure tempFile;
            long rootDirectoryOffset = baseOffset + directoryRecord.LocationOfExtent * logicalBlockSize;

            currentOffset = rootDirectoryOffset;

            while (currentOffset < (rootDirectoryOffset + directoryRecord.DataLength))
            {
                recordSize = ParseFile.ParseSimpleOffset(isoStream, currentOffset, 1)[0];
                directoryRecordBytes = ParseFile.ParseSimpleOffset(isoStream, currentOffset, recordSize);
                tempDirectoryRecord = new Iso9660DirectoryRecord(directoryRecordBytes);

                if (!tempDirectoryRecord.FileIdentifierString.Equals(".") &&
                    !tempDirectoryRecord.FileIdentifierString.Equals("..")) // skip "this" directory
                {
                    if (tempDirectoryRecord.FlagDirectory)
                    {
                        tempDirectory = new Iso9660DirectoryStructure(isoStream, isoStream.Name, baseOffset, tempDirectoryRecord, logicalBlockSize, parentDirectory);
                        this.SubDirectoryArray.Add(tempDirectory);
                    }
                    else
                    {
                        tempFile = new Iso9660FileStructure(parentDirectory,
                            this.SourceFilePath,
                            tempDirectoryRecord.FileIdentifierString.Replace(";1", String.Empty),
                            baseOffset + (tempDirectoryRecord.LocationOfExtent * logicalBlockSize), 
                            tempDirectoryRecord.DataLength);
                        this.FileArray.Add(tempFile);            
                    }
                }
                else if (tempDirectoryRecord.FileIdentifierString.Equals(".."))
                {
                    this.ParentDirectoryRecord = tempDirectoryRecord;
                }

                currentOffset += recordSize;
            }

        }        
    }
}
