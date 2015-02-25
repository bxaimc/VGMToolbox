using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.util;

namespace VGMToolbox.format.iso
{
    // Thanks to CarlKenner, soneek, crediar's DiscU.exe output, and various forums
    public class NintendoWiiUOpticalDisc
    {
        public struct Partition
        {
            public ulong PartitionOffset { set; get; }
            public byte[] PartitionIdentifier { set; get; }
            public string PartitionName { set; get; }

            public byte[] PartitionKey { set; get; }

            public PartitionEntry[] PartitionEntries { set; get; }

            /*
            public byte[] TitleId { set; get; }
            public byte[] EncryptedTitleKey { set; get; }
            public byte[] DecryptedTitleKey { set; get; }
            public byte CommonKeyIndex { set; get; }

            public long RelativeDataOffset { set; get; }
            public long DataSize { set; get; }
            */ 
        }

        public struct PartitionEntry
        {
            // 0x00
            public bool IsDirectory { set; get; }  // 0x00
            public uint NameOffset { set; get; }   // 0x01 - 0x03
            public string EntryName { set; get; }  

            // 0x04
            public uint OffsetWithinCluster { set; get; }
            
            // 0x08
            public uint LastRowInDirectory { set; get; } // Only if IsDirectory is True
            public uint Size { set; get; }               // Only if IsDirectory is False

            // 0x0C
            public ushort Unknown { set; get; }          // 0x0C
            public ushort StartingCluster { set; get; }  // 0x0E

            public PartitionEntry(byte[] RawPartitionEntry): this()
            {                                
                // check if this is a directory entry
                if (RawPartitionEntry[0] == 1)
                {
                    this.IsDirectory = true;
                    this.LastRowInDirectory = ParseFile.ReadUintBE(RawPartitionEntry, 8);
                }
                else
                {
                    this.IsDirectory = false;
                    this.Size = ParseFile.ReadUintBE(RawPartitionEntry, 8);
                }

                this.NameOffset = ParseFile.ReadUintBE(RawPartitionEntry, 0) & 0x00FFFFFF;
                this.OffsetWithinCluster = ParseFile.ReadUintBE(RawPartitionEntry, 4);

                this.Unknown = ParseFile.ReadUshortBE(RawPartitionEntry, 0x0C);
                this.StartingCluster = ParseFile.ReadUshortBE(RawPartitionEntry, 0x0E);
            }
        }
        
        public static readonly byte[] STANDARD_IDENTIFIER = new byte[] { 0x57, 0x55, 0x50, 0x2D }; // "WUP-"
        public const uint IDENTIFIER_OFFSET = 0;
        public static string FORMAT_DESCRIPTION_STRING = "WiiU Encrypted";
        public static string FORMAT_DESCRIPTION_STRING_DECRYPTED = "WiiU Decrypted";
        
        private static readonly string PROGRAMS_FOLDER =
            Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "external"));
        public static readonly string WIIU_EXTERNAL_FOLDER = Path.Combine(PROGRAMS_FOLDER, "wiiu");
        public static readonly string COMMON_KEY_PATH = Path.Combine(WIIU_EXTERNAL_FOLDER, "ckey.bin");
        public static readonly string DISC_KEY_FILE_NAME = "disckey.bin";

        public const ulong WIIU_DECRYPTED_AREA_OFFSET = 0x18000;
        public const uint PARTITION_TOC_OFFSET = 0x800;
        public const uint PARTITION_TOC_ENTRY_SIZE = 0x80;

        public NintendoWiiUEncryptedDiscReader DiscReader { set; get; }

        private byte[] CommonKey;
        private byte[] DiscKey;

        public string GameSerial { set; get; }
        public string GameRevision { set; get; }
        public string GameRegion { set; get; }
        public string SystemMenuVersion { set; get; }

        public long DiscBaseOffset { set; get; }
        public ulong DecryptedAreaOffset { set; get; }
        public bool IsRawDump { set; get; }

        public uint PartitionCount { set; get; }
        public Partition[] Partitions { set; get; }

        public ArrayList VolumeArrayList { set; get; }
        public NintendoWiiUOpticalDiscVolume[] Volumes
        {
            get
            {
                return (NintendoWiiUOpticalDiscVolume[])this.VolumeArrayList.ToArray(typeof(NintendoWiiUOpticalDiscVolume));
            }

        }

        public static byte[] GetKeyFromFile(string path)
        {
            byte[] keyArray = null;

            if (!File.Exists(path))
            {
                throw new FileNotFoundException(String.Format("Key not found: {0}.{1}", path, Environment.NewLine));
            }
            else
            {
                keyArray = new byte[0x10];

                using (FileStream fs = File.OpenRead(path))
                {
                    fs.Read(keyArray, 0, 0x10);
                }
            }

            return keyArray;
        }

        public void Initialize(FileStream isoStream, long offset, bool isRawDump)
        {
            this.CommonKey = GetKeyFromFile(COMMON_KEY_PATH);
            this.DiscKey = GetKeyFromFile(Path.Combine(Path.GetDirectoryName(isoStream.Name), DISC_KEY_FILE_NAME));
            
            this.GameSerial = ByteConversion.GetAsciiText(ParseFile.ParseSimpleOffset(isoStream, this.DiscBaseOffset, 9));
            this.GameRevision = ByteConversion.GetAsciiText(ParseFile.ParseSimpleOffset(isoStream, this.DiscBaseOffset + 0xB, 2));
            this.SystemMenuVersion = ByteConversion.GetAsciiText(ParseFile.ParseSimpleOffset(isoStream, this.DiscBaseOffset + 0xE, 3));
            this.GameRegion = ByteConversion.GetAsciiText(ParseFile.ParseSimpleOffset(isoStream, this.DiscBaseOffset + 0x11, 3));

            this.DiscBaseOffset = offset;
            this.DecryptedAreaOffset = (ulong)this.DiscBaseOffset + WIIU_DECRYPTED_AREA_OFFSET;
            this.DiscReader = new NintendoWiiUEncryptedDiscReader();
            
            this.IsRawDump = isRawDump;
                        
            // initialize partition info
            this.InitializePartions(isoStream);
                        
            // initialize volumes
            this.VolumeArrayList = new ArrayList();
            this.LoadVolumes(isoStream);

        }

        private void InitializePartions(FileStream isoStream)
        {            
            // read and decrypt partition toc
            byte[] PartitionTocBlock = this.DiscReader.GetBytes(isoStream, this.DecryptedAreaOffset, 0x8000, this.DiscKey);

            // @TODO: Verify DiscKey before proceeding

            // initialize partitions TOC
            this.InitializePartitionToc(isoStream, PartitionTocBlock);
           
            // parse file table entries (FST block)
            this.ParsePartitionFileTables(isoStream);
        }

        private void InitializePartitionToc(FileStream isoStream, byte[] DecryptedPartitionToc)
        {
            byte[] temp = new byte[4];
    
            // get partition count from decrypted TOC
            Array.Copy(DecryptedPartitionToc, 0x1C, temp, 0, 4);
            this.PartitionCount = ByteConversion.GetUInt32BigEndian(temp);
        
            // initialize partition array
            this.Partitions = new Partition[this.PartitionCount];
        
            // populate partition information from decrypted TOC
            for (uint i = 0; i < this.PartitionCount; i++)
            {
                this.Partitions[i] = new Partition();

                // copy partition identifier bytes (SI, UP, GM, ??) from decrypted TOC
                this.Partitions[i].PartitionIdentifier = new byte[0x19];
                Array.Copy(DecryptedPartitionToc, (PARTITION_TOC_OFFSET + (i * PARTITION_TOC_ENTRY_SIZE)), this.Partitions[i].PartitionIdentifier, 0, 2);

                // convert partition identifier to string for display
                this.Partitions[i].PartitionName = ByteConversion.GetAsciiText(this.Partitions[i].PartitionIdentifier);

                // calculate partition offset (relative from WIIU_DECRYPTED_AREA_OFFSET) from decrypted TOC
                Array.Copy(DecryptedPartitionToc, (PARTITION_TOC_OFFSET + (i * PARTITION_TOC_ENTRY_SIZE) + 0x20), temp, 0, 4);
                this.Partitions[i].PartitionOffset = (ulong)((ByteConversion.GetUInt32BigEndian(temp) * 0x8000) - 0x10000);
            }
        }

        private void ParsePartitionFileTables(FileStream isoStream)
        {
            byte[] rawPartitionEntry;
            byte[] fileTableBlock;

            uint entriesOffset;
            uint nameTableOffset;
            uint totalEntries;

            ArrayList partitionEntries;
            PartitionEntry pe;

            // loop through each partition
            for (uint i = 0; i < this.Partitions.GetLength(0); i++)
            {
                partitionEntries = new ArrayList();

                // decrypt file table block 
                // @TODO: determine correct key to use per partition
                //      remove or alter IF statement below
                if (this.Partitions[i].PartitionName.StartsWith("SI") ||
                    this.Partitions[i].PartitionName.StartsWith("UP"))
                {
                    fileTableBlock = this.DiscReader.GetBytes(isoStream,
                        this.DecryptedAreaOffset + this.Partitions[i].PartitionOffset, 0x8000, this.DiscKey);

                    // @TODO: Verify FST magic bytes

                    // skip header to goto directory/file records
                    entriesOffset = (ParseFile.ReadUintBE(fileTableBlock, 4) * ParseFile.ReadUintBE(fileTableBlock, 8)) + 0x20;

                    // read first row for root directory and to get record count
                    rawPartitionEntry = ParseFile.SimpleArrayCopy(fileTableBlock, entriesOffset, 0x10);
                    pe = new PartitionEntry(rawPartitionEntry);

                    // save total number of entries
                    totalEntries = pe.LastRowInDirectory;
                    nameTableOffset = entriesOffset + (totalEntries * 0x10);

                    // get root directory Name (typically null for root dir)
                    pe.EntryName = ByteConversion.GetAsciiText(fileTableBlock, nameTableOffset + pe.NameOffset);

                    // add root directory to array list
                    partitionEntries.Add(pe);

                    // loop through remaining records                
                    for (uint j = 1; j < totalEntries; j++)
                    {
                        rawPartitionEntry = ParseFile.SimpleArrayCopy(fileTableBlock, entriesOffset + (j * 0x10), 0x10);
                        pe = new PartitionEntry(rawPartitionEntry);
                        pe.EntryName = ByteConversion.GetAsciiText(fileTableBlock, nameTableOffset + pe.NameOffset);
                        partitionEntries.Add(pe);

                        // @TODO: Get title key
                    } // for (uint j = 1; j < totalEntries; j++)

                    this.Partitions[i].PartitionEntries = (PartitionEntry[])partitionEntries.ToArray(typeof(PartitionEntry));
                } // if (this.Partitions[i].PartitionName.StartsWith("SI")
            } // for (uint i = 0; i < this.Partitions.GetLength(0); i++)
        }


        public void LoadVolumes(FileStream isoStream)
        {
            NintendoWiiUOpticalDiscVolume newVolume;

            for (uint i = 0; i < this.PartitionCount; i++)
            {
/*                
                    newVolume = new NintendoWiiOpticalDiscVolume(
                        this.Partitions[i].PartitionEntries[j].PartitionOffset,
                        this.Partitions[i].PartitionEntries[j].RelativeDataOffset,
                        this.Partitions[i].PartitionEntries[j].DecryptedTitleKey,
                        this.DiscReader);

                    newVolume.Initialize(isoStream, this.Partitions[i].PartitionEntries[j].PartitionOffset, this.IsRawDump);
                    this.VolumeArrayList.Add(newVolume);
 */ 
            }
        }
    }



    public class NintendoWiiUOpticalDiscVolume : IVolume
    {
        public NintendoWiiUOpticalDisc.Partition SourcePartition { set; get; }

        public string VolumeIdentifier { set; get; }
        public string FormatDescription { set; get; }
        public VolumeDataType VolumeType { set; get; }

        public long VolumeBaseOffset { set; get; }
        public long DataOffset { set; get; }        
        public byte[] PartitionKey { set; get; }

        public bool IsRawDump { set; get; }
        public NintendoWiiUEncryptedDiscReader DiscReader { set; get; }

        public ArrayList DirectoryStructureArray { set; get; }
        public IDirectoryStructure[] Directories
        {
            set { Directories = value; }
            get
            {
                DirectoryStructureArray.Sort();
                return (IDirectoryStructure[])DirectoryStructureArray.ToArray(typeof(NintendoWiiUOpticalDiscDirectoryStructure));
            }
        }

        public ulong RootDirectoryOffset { set; get; }
        public DateTime ImageCreationTime { set; get; }

        public ulong NameTableOffset { set; get; }

        public NintendoWiiUOpticalDiscVolume(NintendoWiiUOpticalDisc.Partition partition, 
            byte[] partitionKey, NintendoWiiUEncryptedDiscReader discReader)
        {
            this.SourcePartition = partition;
            
            this.PartitionKey = partitionKey;
            this.DiscReader = discReader;
                        
            this.DiscReader.CurrentDecryptedBlockNumber = -1;
            this.DiscReader.CurrentDecryptedBlock = null;
        }

        public int CompareTo(object obj)
        {
            if (obj is NintendoWiiUOpticalDiscVolume)
            {
                NintendoWiiUOpticalDiscVolume o = (NintendoWiiUOpticalDiscVolume)obj;

                return this.VolumeIdentifier.CompareTo(o.VolumeIdentifier);
            }

            throw new ArgumentException("object is not an NintendoWiiUOpticalDiscVolume");
        }

        public void Initialize(FileStream isoStream, long offset, bool isRawDump)
        {
            this.VolumeBaseOffset = (long)this.SourcePartition.PartitionOffset;
            this.VolumeIdentifier = this.SourcePartition.PartitionName;
            
            this.IsRawDump = isRawDump;
            this.DirectoryStructureArray = new ArrayList();

            this.FormatDescription = NintendoWiiUOpticalDisc.FORMAT_DESCRIPTION_STRING;
            this.VolumeType = VolumeDataType.Data;
                   
            this.LoadDirectories(isoStream);
        }

        public void ExtractAll(ref Dictionary<string, FileStream> streamCache, string destinationFolder, bool extractAsRaw)
        {
            foreach (NintendoWiiUOpticalDiscDirectoryStructure ds in this.DirectoryStructureArray)
            {
                ds.Extract(ref streamCache, destinationFolder, extractAsRaw);
            }
        }

        public void LoadDirectories(FileStream isoStream)
        {
            byte[] rootDirectoryBytes;
            NintendoWiiOpticalDiscDirectoryRecord rootDirectoryRecord;
            NintendoWiiOpticalDiscDirectoryStructure rootDirectory;

            //rootDirectoryBytes = this.Dos

            // Get name table offset
            //rootDirectoryBytes = DiscReader.GetBytes(isoStream, this.VolumeBaseOffset,
            //    this.DataOffset, this.RootDirectoryOffset, 0xC, this.PartitionKey);
            //rootDirectoryRecord = new NintendoWiiOpticalDiscDirectoryRecord(rootDirectoryBytes);
            //this.NameTableOffset = this.RootDirectoryOffset + ((long)rootDirectoryRecord.FileSize * 0xC);

            //rootDirectory = new NintendoWiiOpticalDiscDirectoryStructure(isoStream,
            //    isoStream.Name, rootDirectoryRecord, this.ImageCreationTime,
            //    this.VolumeBaseOffset, this.DataOffset, this.RootDirectoryOffset,
            //    this.RootDirectoryOffset, this.NameTableOffset,
            //    String.Empty, String.Empty, this.DiscReader, this.PartitionKey);

            //this.DirectoryStructureArray.Add(rootDirectory);
        }
    }

    public class NintendoWiiUOpticalDiscFileStructure : IFileStructure
    {
        public string ParentDirectoryName { set; get; }
        public string SourceFilePath { set; get; }
        public string FileName { set; get; }

        public long VolumeBaseOffset { set; get; }
        public long DataSectionOffset { set; get; }
        public long Lba { set; get; }
        public long Size { set; get; }
        public bool IsRaw { set; get; }
        public CdSectorType FileMode { set; get; }
        public int NonRawSectorSize { set; get; }

        public NintendoWiiUEncryptedDiscReader DiscReader { set; get; }
        public byte[] PartitionKey { set; get; }

        public DateTime FileDateTime { set; get; }

        public int CompareTo(object obj)
        {
            if (obj is NintendoWiiUOpticalDiscFileStructure)
            {
                NintendoWiiUOpticalDiscFileStructure o = (NintendoWiiUOpticalDiscFileStructure)obj;

                return this.FileName.CompareTo(o.FileName);
            }

            throw new ArgumentException("object is not an NintendoWiiUOpticalDiscFileStructure");
        }

        public NintendoWiiUOpticalDiscFileStructure(string parentDirectoryName,
            string sourceFilePath, string fileName, long volumeBaseOffset,
            long dataSectionOffset, long lba, long size, DateTime fileTime,
            NintendoWiiUEncryptedDiscReader discReader, byte[] partitionKey)
        {
            this.ParentDirectoryName = parentDirectoryName;
            this.SourceFilePath = sourceFilePath;
            this.FileName = fileName;
            this.VolumeBaseOffset = volumeBaseOffset;
            this.DataSectionOffset = dataSectionOffset;
            this.Lba = lba;
            this.IsRaw = false;
            this.NonRawSectorSize = -1;
            this.Size = size;
            this.FileMode = CdSectorType.Unknown;
            this.FileDateTime = fileTime;
            this.DiscReader = discReader;
            this.PartitionKey = partitionKey;
        }

        public virtual void Extract(ref Dictionary<string, FileStream> streamCache, string destinationFolder, bool extractAsRaw)
        {
            string destinationFile = Path.Combine(Path.Combine(destinationFolder, this.ParentDirectoryName), this.FileName);

            if (!streamCache.ContainsKey(this.SourceFilePath))
            {
                streamCache[this.SourceFilePath] = File.OpenRead(this.SourceFilePath);
            }

            this.DiscReader.ExtractFile(streamCache[this.SourceFilePath], destinationFile, this.VolumeBaseOffset,
                this.DataSectionOffset, this.Lba, this.Size, this.PartitionKey);
        }
    }

    public class NintendoWiiUOpticalDiscDirectoryStructure : IDirectoryStructure
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
                return (IDirectoryStructure[])SubDirectoryArray.ToArray(typeof(NintendoWiiOpticalDiscDirectoryStructure));
            }
        }

        public ArrayList FileArray { set; get; }
        public IFileStructure[] Files
        {
            set { this.Files = value; }
            get
            {
                FileArray.Sort();
                return (IFileStructure[])FileArray.ToArray(typeof(NintendoWiiUOpticalDiscFileStructure));
            }
        }

        public string SourceFilePath { set; get; }
        public string DirectoryName { set; get; }

        public int CompareTo(object obj)
        {
            if (obj is NintendoWiiUOpticalDiscDirectoryStructure)
            {
                NintendoWiiUOpticalDiscDirectoryStructure o = (NintendoWiiUOpticalDiscDirectoryStructure)obj;

                return this.DirectoryName.CompareTo(o.DirectoryName);
            }

            throw new ArgumentException("object is not an NintendoWiiUOpticalDiscDirectoryStructure");
        }

        public void Extract(ref Dictionary<string, FileStream> streamCache, string destinationFolder, bool extractAsRaw)
        {
            string fullDirectoryPath = Path.Combine(destinationFolder, Path.Combine(this.ParentDirectoryName, this.DirectoryName));

            // create directory
            if (!Directory.Exists(fullDirectoryPath))
            {
                Directory.CreateDirectory(fullDirectoryPath);
            }

            foreach (NintendoWiiUOpticalDiscFileStructure f in this.FileArray)
            {
                f.Extract(ref streamCache, destinationFolder, extractAsRaw);
            }

            foreach (NintendoWiiUOpticalDiscDirectoryStructure d in this.SubDirectoryArray)
            {
                d.Extract(ref streamCache, destinationFolder, extractAsRaw);
            }
        }

        public NintendoWiiUOpticalDiscDirectoryStructure(
            FileStream isoStream,
            string sourceFilePath,
            NintendoWiiUOpticalDiscDirectoryRecord directoryRecord,
            DateTime creationDateTime,
            long baseOffset,
            long dataSectionOffset,
            long rootDirectoryOffset,
            long directoryOffset,
            long nameTableOffset,
            string directoryName,
            string parentDirectory,
            NintendoWiiUEncryptedDiscReader discReader,
            byte[] partitionKey)
        {
            string nextDirectory;
            this.SourceFilePath = sourceFilePath;
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

            this.parseDirectoryRecord(isoStream, directoryRecord, creationDateTime, baseOffset, dataSectionOffset, rootDirectoryOffset, directoryOffset, nameTableOffset, nextDirectory, discReader, partitionKey);
        }


        private void parseDirectoryRecord(
            FileStream isoStream,
            NintendoWiiUOpticalDiscDirectoryRecord directoryRecord,
            DateTime creationDateTime,
            long baseOffset,
            long dataSectionOffset,
            long rootDirectoryOffset,
            long directoryOffset,
            long nameTableOffset,
            string parentDirectory,
            NintendoWiiUEncryptedDiscReader discReader,
            byte[] partitionKey)
        {
            long directoryRecordEndOffset;
            long newDirectoryEndOffset;
            long currentOffset = directoryOffset;

            int itemNameSize;
            byte[] itemNameBytes;
            string itemName;

            byte[] newDirectoryRecordBytes;
            NintendoWiiUOpticalDiscDirectoryRecord newDirectoryRecord;
            NintendoWiiUOpticalDiscDirectoryStructure newDirectory;
            NintendoWiiUOpticalDiscFileStructure newFile;

            directoryRecordEndOffset = rootDirectoryOffset + (directoryRecord.FileSize * 0xC);
            currentOffset += 0xC;

            while (currentOffset < directoryRecordEndOffset)
            {
                newDirectoryRecordBytes = discReader.GetBytes(isoStream, baseOffset, dataSectionOffset,
                    currentOffset, 0xC, partitionKey);
                newDirectoryRecord = new NintendoWiiUOpticalDiscDirectoryRecord(newDirectoryRecordBytes);

                itemNameBytes = discReader.GetBytes(isoStream, baseOffset, dataSectionOffset,
                    nameTableOffset + newDirectoryRecord.NameOffset, 512, partitionKey);
                itemNameSize = ParseFile.GetSegmentLength(itemNameBytes, 0, Constants.NullByteArray);
                itemNameBytes = discReader.GetBytes(isoStream, baseOffset, dataSectionOffset,
                    nameTableOffset + newDirectoryRecord.NameOffset, itemNameSize, partitionKey);
                itemName = ByteConversion.GetEncodedText(itemNameBytes, ByteConversion.GetPredictedCodePageForTags(itemNameBytes));

                if (!newDirectoryRecord.IsDirectory)
                {
                    newFile = new NintendoWiiUOpticalDiscFileStructure(parentDirectory,
                        this.SourceFilePath, itemName,
                        baseOffset, dataSectionOffset, newDirectoryRecord.FileOffset,
                        newDirectoryRecord.FileSize, creationDateTime, discReader,
                        partitionKey);

                    this.FileArray.Add(newFile);
                    currentOffset += 0xC;
                }
                else
                {
                    newDirectory =
                        new NintendoWiiUOpticalDiscDirectoryStructure(isoStream,
                            isoStream.Name, newDirectoryRecord,
                            creationDateTime, baseOffset, dataSectionOffset, rootDirectoryOffset,
                            currentOffset, nameTableOffset,
                            itemName, parentDirectory, discReader, partitionKey);

                    this.SubDirectoryArray.Add(newDirectory);

                    newDirectoryEndOffset = rootDirectoryOffset + (newDirectoryRecord.FileSize * 0xC);
                    currentOffset = newDirectoryEndOffset;
                }
            }
        }
    }

    public class NintendoWiiUOpticalDiscDirectoryRecord
    {
        public uint NameOffset { set; get; }
        public long FileOffset { set; get; }
        public uint FileSize { set; get; }
        public bool IsDirectory { set; get; }

        public NintendoWiiUOpticalDiscDirectoryRecord(byte[] directoryBytes)
        {
            this.NameOffset = ByteConversion.GetUInt32BigEndian(ParseFile.ParseSimpleOffset(directoryBytes, 0, 4));

            this.FileOffset = (long)ByteConversion.GetUInt32BigEndian(ParseFile.ParseSimpleOffset(directoryBytes, 4, 4));
            this.FileOffset <<= 2;

            this.FileSize = ByteConversion.GetUInt32BigEndian(ParseFile.ParseSimpleOffset(directoryBytes, 8, 4));
            this.IsDirectory = ((this.NameOffset & 0xFF000000) != 0);

            this.NameOffset &= 0xFFFFFF;
        }
    }

    public class NintendoWiiUEncryptedDiscReader
    {
        public byte[] CurrentDecryptedBlock { set; get; }
        public long CurrentDecryptedBlockNumber { set; get; }
        public Rijndael Algorithm { set; get; }

        public NintendoWiiUEncryptedDiscReader()
        {
            this.CurrentDecryptedBlock = null;
            this.CurrentDecryptedBlockNumber = -1;
            this.Algorithm = Rijndael.Create();
        }

        public byte[] GetBytes(FileStream isoStream, ulong offset, ulong size, byte[] key)
        {
            byte[] encryptedChunk = new byte[size];
            byte[] decryptedChunk = new byte[size];
            byte[] IV = new byte[0x10];

            encryptedChunk = ParseFile.ParseSimpleOffset(isoStream, (long)offset, (int)size);
            decryptedChunk = AESEngine.Decrypt(this.Algorithm, encryptedChunk, key, IV, CipherMode.CBC, PaddingMode.Zeros);

            return decryptedChunk;
        }
        
        public byte[] GetBytes(FileStream isoStream, long volumeOffset,
                long dataOffset, long blockOffset, long size, byte[] partitionKey)
        {
            byte[] value = new byte[size];
            
            /*
            byte[] encryptedPartitionCluster;
            byte[] encryptedClusterDataSection;
            byte[] decryptedClusterDataSection;
            byte[] clusterIV;

            long bufferLocation = 0;
            long maxCopySize;
            long copySize;

            while (size > 0)
            {
                // get block offset info
                NintendoWiiUOpticalDisc.WiiBlockStructure blockStructure =
                    NintendoWiiUOpticalDisc.GetWiiBlockStructureForOffset(blockOffset);

                // read current block
                //encryptedPartitionCluster = ParseFile.ParseSimpleOffset(isoStream,
                //    volumeOffset + dataOffset + (blockStructure.BlockNumber * 0x8000),
                //    0x8000);

                if (this.CurrentDecryptedBlockNumber != blockStructure.BlockNumber)
                {
                    encryptedPartitionCluster = ParseFile.ParseSimpleOffset(isoStream,
                        volumeOffset + dataOffset + (blockStructure.BlockNumber * 0x8000),
                        0x8000);

                    clusterIV = ParseFile.ParseSimpleOffset(encryptedPartitionCluster, 0x03D0, 0x10);
                    encryptedClusterDataSection = ParseFile.ParseSimpleOffset(encryptedPartitionCluster, 0x400, 0x7C00);
                    decryptedClusterDataSection = AESEngine.Decrypt(this.Algorithm, encryptedClusterDataSection,
                                    partitionKey, clusterIV, CipherMode.CBC, PaddingMode.Zeros);

                    this.CurrentDecryptedBlock = decryptedClusterDataSection;
                    this.CurrentDecryptedBlockNumber = blockStructure.BlockNumber;
                }

                // copy the decrypted data
                maxCopySize = 0x7C00 - blockStructure.BlockOffset;
                copySize = (size > maxCopySize) ? maxCopySize : size;
                Array.Copy(this.CurrentDecryptedBlock, blockStructure.BlockOffset,
                    value, bufferLocation, copySize);

                // update counters
                size -= copySize;
                bufferLocation += copySize;
                blockOffset += copySize;
            }
            */
            return value;
        }

        public void ExtractFile(FileStream isoStream, string destinationPath, long volumeOffset,
            long dataOffset, long blockOffset, long size, byte[] partitionKey)
        {
            /*
            byte[] value = new byte[0x7C00];
            byte[] encryptedPartitionCluster;
            byte[] encryptedClusterDataSection;
            byte[] decryptedClusterDataSection;
            byte[] clusterIV;

            long maxCopySize;
            long copySize;

            // create destination
            string destintionDirectory = Path.GetDirectoryName(destinationPath);

            if (!Directory.Exists(destintionDirectory))
            {
                Directory.CreateDirectory(destintionDirectory);
            }

            using (FileStream outStream = File.Open(destinationPath, FileMode.Create, FileAccess.Write))
            {
                while (size > 0)
                {
                    // get block offset info
                    NintendoWiiUOpticalDisc.WiiBlockStructure blockStructure =
                        NintendoWiiUOpticalDisc.GetWiiBlockStructureForOffset(blockOffset);

                    // read current block
                    //encryptedPartitionCluster = ParseFile.ParseSimpleOffset(isoStream,
                    //    volumeOffset + dataOffset + (blockStructure.BlockNumber * 0x8000),
                    //    0x8000);

                    if (this.CurrentDecryptedBlockNumber != blockStructure.BlockNumber)
                    {
                        encryptedPartitionCluster = ParseFile.ParseSimpleOffset(isoStream,
                            volumeOffset + dataOffset + (blockStructure.BlockNumber * 0x8000),
                            0x8000);

                        clusterIV = ParseFile.ParseSimpleOffset(encryptedPartitionCluster, 0x03D0, 0x10);
                        encryptedClusterDataSection = ParseFile.ParseSimpleOffset(encryptedPartitionCluster, 0x400, 0x7C00);
                        decryptedClusterDataSection = AESEngine.Decrypt(this.Algorithm, encryptedClusterDataSection,
                                        partitionKey, clusterIV, CipherMode.CBC, PaddingMode.Zeros);

                        this.CurrentDecryptedBlock = decryptedClusterDataSection;
                        this.CurrentDecryptedBlockNumber = blockStructure.BlockNumber;
                    }

                    // copy the encrypted data
                    maxCopySize = 0x7C00 - blockStructure.BlockOffset;
                    copySize = (size > maxCopySize) ? maxCopySize : size;
                    outStream.Write(this.CurrentDecryptedBlock, (int)blockStructure.BlockOffset, (int)copySize);

                    // update counters
                    size -= copySize;
                    blockOffset += copySize;
                }
              
            }
             */ 
        }
    }

}
