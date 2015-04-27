using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.format.iso;
using VGMToolbox.util;

namespace VGMToolbox.format
{
    public class SimpleOffset
    {
        public uint Offset { set; get; }
        public uint Size { set; get; }

        public SimpleOffset(uint offset, uint size)
        {
            this.Offset = offset;
            this.Size = size;
        }
    }
    
    public class Nintendo3dsCtr
    {
        public static readonly byte[] STANDARD_IDENTIFIER = new byte[] { 0x4E, 0x43, 0x53, 0x44 }; // "NCSD"
        public const uint IDENTIFIER_OFFSET = 0x100;
        public static string FORMAT_DESCRIPTION_STRING = "3DS CTR Encrypted"; // currently unsupported
        public static string FORMAT_DESCRIPTION_STRING_DECRYPTED = "3DS CTR Decrypted";
        public const long MEDIA_UNIT_SIZE = 0x200;

        public const string CTR_SYSTEM_PARTITION = "CTR-P-CTAP";

        public long DiscBaseOffset { set; get; }
        public string SourceFileName { set; get; }
        public bool IsRawDump { set; get; }
        
        public ArrayList VolumeArrayList { set; get; }
        public Nintendo3dsNcchContainer[] Volumes
        {
            get
            {
                return (Nintendo3dsNcchContainer[])this.VolumeArrayList.ToArray(typeof(Nintendo3dsNcchContainer));
            }

        }

        // NCSD Header

        public byte[] NcsdHash { set; get; }
        public uint MagicBytes { set; get; }
        public uint ImageSize { set; get; }
        public ulong MediaId { set; get; }
        public ulong PartitionsFsType { set; get; }
        public ulong PartitionsEncryptionType { set; get; }
        public SimpleOffset[] NcchOffsetInfo { set; get; }

        public byte[] ExHeaderHash { set; get; }
        public uint AdditionalHeaderSize { set; get; }
        public uint SectorZeroOffset { set; get; }
        public ulong PartitionFlags { set; get; }
        public ulong[] PartitionIds { set; get; }
        public byte[] Reserved01 { set; get; }
        public byte[] Reserved02 { set; get; }
        public byte Unknown01 { set; get; }
        public byte Unknown02 { set; get; }

        // Card Info Header

        public int CardInfoWritableAddress { set; get; }
        public uint CardInfoBitmask { set; get; }
        public byte[] CardInfoReserved01 { set; get; }
        public ulong CardInfoMediaId { set; get; }
        public ulong CardInfoReserved02 { set; get; }
        public byte[] CardInfoInitialData { set; get; }
        public byte[] CardInfoReserved03 { set; get; }
        public byte[] CardInfoNcchHeaderCopy { set; get; }

        public Nintendo3dsCtr(string sourceFile)
        { 
            // check magic bytes
            if (Nintendo3dsCtr.Is3dsCtrFile(sourceFile))
            {
                // read header
                using (FileStream fs = File.OpenRead(sourceFile))
                {
                    // set source file
                    this.SourceFileName = sourceFile; 

                    // initialize
                    this.Initialize(fs, 0, false);

                } // using (FileStream fs = File.OpenRead(sourceFile))
            }
            else
            {
                throw new FormatException("'NCSD' magic bytes not found at offset 0x100.");
            }
        }

        public void Initialize(FileStream fs, long offset, bool isRawDump)
        {
            // @TODO: Is there a way to determine if this file is exncrypted or not?
            //this.FormatDescription = Nintendo3dsCtr.FORMAT_DESCRIPTION_STRING_DECRYPTED;        
            this.DiscBaseOffset = offset;
            this.IsRawDump = isRawDump;

            // parse NCSD header
            this.ParseNcsdHeader(fs, offset);

            // initialize volumes
            this.VolumeArrayList = new ArrayList();
            this.LoadVolumes(fs);
        }


        public void LoadVolumes(FileStream isoStream)
        {
            Nintendo3dsNcchContainer newVolume;
            long streamLength = isoStream.Length;
            long ncchAbsoluteOffset;

            for (int i = 0; i < NcchOffsetInfo.Length; i++)
            {
                if (this.NcchOffsetInfo[i].Offset > 0)  
                {
                    ncchAbsoluteOffset = this.DiscBaseOffset + (long)((long)this.NcchOffsetInfo[i].Offset * Nintendo3dsCtr.MEDIA_UNIT_SIZE);

                    // skip trimmed update partition
                    if (ncchAbsoluteOffset + 1 < streamLength)
                    {
                        newVolume = new Nintendo3dsNcchContainer();
                        newVolume.Initialize(isoStream, ncchAbsoluteOffset, this.IsRawDump);
                        this.VolumeArrayList.Add(newVolume);
                    }
                }
            }
        }

        private void ParseNcsdHeader(FileStream fs, long offset)
        {
            // parse NCSD header
            this.NcsdHash = ParseFile.ParseSimpleOffset(fs, offset, 0x100);
            this.MagicBytes = ParseFile.ReadUintBE(fs, offset + 0x100);
            this.ImageSize = ParseFile.ReadUintLE(fs, offset + 0x104);
            this.MediaId = ParseFile.ReadUlongLE(fs, offset + 0x108);
            this.PartitionsFsType = ParseFile.ReadUlongLE(fs, offset + 0x110);
            this.PartitionsEncryptionType = ParseFile.ReadUlongLE(fs, offset + 0x118);

            this.NcchOffsetInfo = new SimpleOffset[8];
            for (int i = 0; i < 8; i++)
            {
                this.NcchOffsetInfo[i] = new SimpleOffset(
                    ParseFile.ReadUintLE(fs, offset + 0x120 + (8 * i)),
                    ParseFile.ReadUintLE(fs, offset + 0x120 + ((8 * i) + 4)));
            }

            // parse Exheader
            this.ExHeaderHash = ParseFile.ParseSimpleOffset(fs, offset + 0x160, 0x20);
            this.AdditionalHeaderSize = ParseFile.ReadUintLE(fs, offset + 0x180);
            this.SectorZeroOffset = ParseFile.ReadUintLE(fs, offset + 0x184);
            this.PartitionFlags = ParseFile.ReadUlongLE(fs, offset + 0x188);

            this.PartitionIds = new ulong[8];
            for (int i = 0; i < 8; i++)
            {
                this.PartitionIds[i] = ParseFile.ReadUlongLE(fs, offset + 0x190 + (8 * i));
            }


            this.Reserved01 = ParseFile.ParseSimpleOffset(fs, offset + 0x1D0, 0x20);
            this.Reserved02 = ParseFile.ParseSimpleOffset(fs, offset + 0x1F0, 0xE);
            this.Unknown01 = ParseFile.ReadByte(fs, 0x1FE);
            this.Unknown02 = ParseFile.ReadByte(fs, 0x1FF);            
          
            // parse Card Info
            this.CardInfoWritableAddress = ParseFile.ReadInt32LE(fs, offset + 0x200);
            this.CardInfoBitmask = ParseFile.ReadUintLE(fs, offset + 0x204);
            this.CardInfoReserved01 = ParseFile.ParseSimpleOffset(fs, offset + 0x208, 0xDF8);
            this.CardInfoMediaId = ParseFile.ReadUlongLE(fs, offset + 0x1000);
            this.CardInfoReserved02 = ParseFile.ReadUlongLE(fs, offset + 0x1008);
            this.CardInfoInitialData = ParseFile.ParseSimpleOffset(fs, offset + 0x1010, 0x30);
            this.CardInfoReserved03 = ParseFile.ParseSimpleOffset(fs, offset + 0x1040, 0xC0);
            this.CardInfoNcchHeaderCopy = ParseFile.ParseSimpleOffset(fs, offset + 0x1011, 0x100);


            //// parse nodes
            //this.NodeArray = new ArrayList();
            //this.ParseNodes(fs);
            //this.NameTableOffset = this.RootNodeOffset + ((uint)this.NodeList.Length * 0xC);

            //// build directory structure
            //this.BuildDirectoryTree(fs);
        }

        /// <summary>
        /// Checks for 3DS CTR file Magic Bytes.
        /// </summary>
        /// <param name="sourceFile">Full path to file to check.</param>
        /// <returns>Boolean value indicating if input file has 3DS CTR magic bytes.</returns>
        public static bool Is3dsCtrFile(string sourceFile)
        {
            bool isCtr = false;
            byte[] magicBytes = new byte[Nintendo3dsCtr.STANDARD_IDENTIFIER.Length];

            using (FileStream fs = File.OpenRead(sourceFile))
            {
                magicBytes = ParseFile.ParseSimpleOffset(fs, Nintendo3dsCtr.IDENTIFIER_OFFSET, Nintendo3dsCtr.STANDARD_IDENTIFIER.Length);

                if (ParseFile.CompareSegment(magicBytes, 0, Nintendo3dsCtr.STANDARD_IDENTIFIER))
                {
                    isCtr = true;
                }
            }

            return isCtr;
        }
    }

    //--------------------------------------------
    // NCCH
    //--------------------------------------------
    public class Nintendo3dsDummyFolder : IDirectoryStructure
    {
        public string SourceFilePath { set; get; }
        public string DirectoryName { set; get; }
        public string ParentDirectoryName { set; get; }

        public ArrayList SubDirectoryArray { set; get; }
        public IDirectoryStructure[] SubDirectories
        {
            set { this.SubDirectories = value; }
            get
            {
                this.SubDirectories = new IDirectoryStructure[this.SubDirectoryArray.Count];

                for (int i = 0; i < this.SubDirectoryArray.Count; i++)
                {
                    SubDirectories[i] = (IDirectoryStructure)this.SubDirectoryArray[i];
                }

                return SubDirectories;          
            }
        }

        public ArrayList FileArray { set; get; }
        public IFileStructure[] Files
        {
            set { this.Files = value; }
            get
            {
                return null;
                //FileArray.Sort();
                //return (IFileStructure[])FileArray.ToArray(typeof(Nintendo3dsCtrExeFileSystemFile));
            }
        }

        public Nintendo3dsDummyFolder(string sourceFilePath, string parentDirectoryName)
        {
            this.SourceFilePath = sourceFilePath;
            this.DirectoryName = null;
            this.ParentDirectoryName = parentDirectoryName;

            this.SubDirectoryArray = new ArrayList();
            this.SubDirectories = null;

            this.FileArray = new ArrayList();
            this.Files = null;
        }

        public void Extract(ref Dictionary<string, FileStream> streamCache, string destinationFolder, bool extractAsRaw)
        {
            IDirectoryStructure dir;
            string fullDirectoryPath = Path.Combine(destinationFolder, Path.Combine(this.ParentDirectoryName, this.DirectoryName));
            
            // create directory
            if (!Directory.Exists(fullDirectoryPath))
            {
                Directory.CreateDirectory(fullDirectoryPath);
            }

            for (int i = 0; i < this.SubDirectories.Length; i++)
            {
                dir = (IDirectoryStructure)this.SubDirectories[i];
                dir.Extract(ref streamCache, destinationFolder, extractAsRaw);
            }
        }

        public int CompareTo(object obj)
        {
            if ((obj is Nintendo3dsDummyFolder))
            {
                Nintendo3dsDummyFolder o = (Nintendo3dsDummyFolder)obj;

                return this.DirectoryName.CompareTo(o.DirectoryName);
            }

            throw new ArgumentException("object is not an Nintendo3dsDummyFoldesr");
        }    
    
    
    
    }
    
    public class Nintendo3dsNcchContainer : IVolume 
    {
        public byte[] NcsdHash { set; get; }
        public uint MagicBytes { set; get; }
        public uint ContentSize { set; get; }
        public ulong PartitionId { set; get; }

        public ushort MakerCode { set; get; }
        public ushort Version { set; get; }
        public uint Reserved01 { set; get; }
        public ulong ProgramId { set; get; }
        public byte[] Reserved02 { set; get; }
        public byte[] LogoRegionHash { set; get; }
        public string ProductCode { set; get; }
        
        public byte[] ExtendedHeaderHash { set; get; }
        public uint ExtendedHeaderSize { set; get; }

        public uint Reserved03 { set; get; }
        public ulong Flags { set; get; }

        public uint PlainRegionOffset { set; get; }
        public uint PlainRegionSize { set; get; }
        
        public uint LogoRegionOffset { set; get; }
        public uint LogoRegionSize { set; get; }
        
        public uint ExeFsOffset { set; get; }
        public uint ExeFsSize { set; get; }
        public uint ExeFsHashSize { set; get; }
        public uint Reserved04 { set; get; }

        public uint RomFsOffset { set; get; }
        public uint RomFsSize { set; get; }
        public uint RomFsHashSize { set; get; }
        public uint Reserved05 { set; get; }

        public byte[] ExeSuperblockHash { set; get; }
        public byte[] RomFsSuperblockHash { set; get; }

        public Nintendo3dsCtrExeFileSystem ExeFs { set; get; }
        public Nintendo3dsCtrRomFileSystem RomFs { set; get; }

        #region IVolume
        public ArrayList DirectoryStructureArray { set; get; }
        public IDirectoryStructure[] Directories
        {
            set { Directories = value; }
            get
            {
                DirectoryStructureArray.Sort();
                return (IDirectoryStructure[])DirectoryStructureArray.ToArray(typeof(Nintendo3dsCtrRomFileSystemDirectory)); // @TODO May need to customize for each directory type or use separate array list for each type (ExeFs, RomFs, etc...) and combine on output
            }
        }

        public long VolumeBaseOffset { set; get; }
        public string FormatDescription { set; get; }
        public VolumeDataType VolumeType { set; get; }
        public string VolumeIdentifier { set; get; }
        public bool IsRawDump { set; get; }

        #endregion

        private void ParseNcchHeader(FileStream fs, long offset)
        {
            this.NcsdHash = ParseFile.ParseSimpleOffset(fs, offset, 0x100);
            this.MagicBytes = ParseFile.ReadUintBE(fs, offset + 0x100);
            this.ContentSize = ParseFile.ReadUintLE(fs, offset + 0x104);
            this.PartitionId = ParseFile.ReadUlongLE(fs, offset + 0x108);

            this.MakerCode = ParseFile.ReadUshortLE(fs, offset + 0x110);
            this.Version = ParseFile.ReadUshortLE(fs, offset + 0x112);
            this.Reserved01 = ParseFile.ReadUintLE(fs, offset + 0x114);
            this.ProgramId = ParseFile.ReadUlongLE(fs, offset + 0x118);
            this.Reserved02 = ParseFile.ParseSimpleOffset(fs, offset + 0x120, 0x10);
            this.LogoRegionHash = ParseFile.ParseSimpleOffset(fs, offset + 0x130, 0x20);
            this.ProductCode = ParseFile.ReadAsciiString(fs, offset + 0x150);

            this.ExtendedHeaderHash = ParseFile.ParseSimpleOffset(fs, offset + 0x160, 0x10);
            this.ExtendedHeaderSize = ParseFile.ReadUintLE(fs, offset + 0x180);

            this.Reserved03 = ParseFile.ReadUintLE(fs, offset + 0x184);
            this.Flags = ParseFile.ReadUlongLE(fs, offset + 0x188);

            this.PlainRegionOffset = ParseFile.ReadUintLE(fs, offset + 0x190);
            this.PlainRegionSize = ParseFile.ReadUintLE(fs, offset + 0x194);

            this.LogoRegionOffset = ParseFile.ReadUintLE(fs, offset + 0x198);
            this.LogoRegionSize = ParseFile.ReadUintLE(fs, offset + 0x19C);

            this.ExeFsOffset = ParseFile.ReadUintLE(fs, offset + 0x1A0);
            this.ExeFsSize = ParseFile.ReadUintLE(fs, offset + 0x1A4);
            this.ExeFsHashSize = ParseFile.ReadUintLE(fs, offset + 0x1A8);
            this.Reserved04 = ParseFile.ReadUintLE(fs, offset + 0x1AC);

            this.RomFsOffset = ParseFile.ReadUintLE(fs, offset + 0x1B0);
            this.RomFsSize = ParseFile.ReadUintLE(fs, offset + 0x1B4);
            this.RomFsHashSize = ParseFile.ReadUintLE(fs, offset + 0x1B8);
            this.Reserved05 = ParseFile.ReadUintLE(fs, offset + 0x1BC);

            this.ExeSuperblockHash = ParseFile.ParseSimpleOffset(fs, offset + 0x1C0, 0x20);
            this.RomFsSuperblockHash = ParseFile.ParseSimpleOffset(fs, offset + 0x1E0, 0x20);        
        }

        public void Initialize(FileStream fs, long offset, bool isRawDump)
        {
            long absoluteOffset;

            this.FormatDescription = Nintendo3dsCtr.FORMAT_DESCRIPTION_STRING_DECRYPTED;

            this.VolumeBaseOffset = offset;
            this.IsRawDump = isRawDump;
            this.VolumeType = VolumeDataType.Data;
            this.DirectoryStructureArray = new ArrayList();

            // parse NCCH header
            this.ParseNcchHeader(fs, offset);

            // set volume name
            this.VolumeIdentifier = this.ProductCode;

            // parse File Systems
            //Nintendo3dsDummyFolder dummyRoot = new Nintendo3dsDummyFolder(fs.Name, null);
            Nintendo3dsCtrRomFileSystemDirectory dummyRoot = new Nintendo3dsCtrRomFileSystemDirectory();
            dummyRoot.SubDirectoryArray = new ArrayList();
            dummyRoot.FileArray = new ArrayList();

            //if (this.ExeFsOffset > 0)
            //{                
            //    absoluteOffset = offset + ((long)this.ExeFsOffset * Nintendo3dsCtr.MEDIA_UNIT_SIZE);
            //    this.ExeFs = new Nintendo3dsCtrExeFileSystem(fs, fs.Name, this.VolumeIdentifier, absoluteOffset);
            //    dummyRoot.SubDirectoryArray.Add((IDirectoryStructure)this.ExeFs);
            //}

            if (this.RomFsOffset > 0)
            {
                absoluteOffset = offset + ((long)this.RomFsOffset * Nintendo3dsCtr.MEDIA_UNIT_SIZE);

                try
                {
                    this.RomFs = new Nintendo3dsCtrRomFileSystem(fs, fs.Name, this.VolumeIdentifier, absoluteOffset);
                    dummyRoot.SubDirectoryArray.Add((IDirectoryStructure)this.RomFs.RootDirectory);
                }
                catch (Exception ex)
                {
                    if ((ex is FormatException) &&
                        (this.VolumeIdentifier.Equals(Nintendo3dsCtr.CTR_SYSTEM_PARTITION)))
                    {
                        MessageBox.Show(String.Format("IVFC magic bytes not found at expected RomFS offset for Volume '{0}, are you sure it is decrypted?'  Note: System Volume, {1}, has not been tested due to unavailability of decrypted samples.",
                                                      this.VolumeIdentifier, Nintendo3dsCtr.CTR_SYSTEM_PARTITION), "Error Processing 3DS CTR");
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }

            if (dummyRoot.SubDirectoryArray.Count > 0)
            {
                this.DirectoryStructureArray.Add((IDirectoryStructure)dummyRoot);
            }
        }

        /// <summary>
        /// Extract all files in archive.
        /// </summary>
        /// <param name="streamCache"></param>
        /// <param name="destintionFolder"></param>
        /// <param name="extractAsRaw"></param>
        public void ExtractAll(ref Dictionary<string, FileStream> streamCache, string destintionFolder, bool extractAsRaw)
        {
            foreach (NintendoU8Directory ds in this.DirectoryStructureArray)
            {
                ds.Extract(ref streamCache, destintionFolder, extractAsRaw);
            }
        }
    
    
    }

    //--------------------------------------------
    // ExeFS
    //--------------------------------------------
    #region ExeFS
    public class Nintendo3dsCtrExeFileSystemFile : IFileStructure
    {
        public string ParentDirectoryName { set; get; }
        public string SourceFilePath { set; get; }
        public string FileName { set; get; }
        public long Offset { set; get; }

        public long VolumeBaseOffset { set; get; }
        public long Lba { set; get; }
        public long Size { set; get; }
        public bool IsRaw { set; get; }
        public CdSectorType FileMode { set; get; }
        public int NonRawSectorSize { set; get; }

        public DateTime FileDateTime { set; get; }
        public byte[] Sha256Hash { set; get; }

        public int CompareTo(object obj)
        {
            if (obj is Nintendo3dsCtrExeFileSystemFile)
            {
                Nintendo3dsCtrExeFileSystemFile o = (Nintendo3dsCtrExeFileSystemFile)obj;

                return this.FileName.CompareTo(o.FileName);
            }

            throw new ArgumentException("object is not a Nintendo3dsCtrExeFileSystemFile");
        }

        public Nintendo3dsCtrExeFileSystemFile(string parentDirectoryName, string sourceFilePath, 
            string fileName, long offset, long volumeBaseOffset, long lba, long size)
        {
            this.ParentDirectoryName = parentDirectoryName;
            this.SourceFilePath = sourceFilePath;
            this.FileName = fileName;
            this.Offset = offset;
            this.Size = size;
            this.FileDateTime = new DateTime();

            this.VolumeBaseOffset = volumeBaseOffset;
            this.Lba =lba;
            this.IsRaw = false;
            this.NonRawSectorSize = 0;
            this.FileMode = CdSectorType.Unknown;
        }

        public void Extract(ref Dictionary<string, FileStream> streamCache, string destinationFolder, bool extractAsRaw)
        {
            string destinationFile = Path.Combine(Path.Combine(destinationFolder, this.ParentDirectoryName), this.FileName);
            HashAlgorithm cryptoHash = SHA256.Create();

            if (!streamCache.ContainsKey(this.SourceFilePath))
            {
                streamCache[this.SourceFilePath] = File.OpenRead(this.SourceFilePath);
            }

            // write file while calculating hash
            byte[] outputHash = ParseFile.ExtractChunkToFile64ReturningHash(streamCache[this.SourceFilePath], (ulong)this.Offset, (ulong)this.Size, 
                destinationFile, cryptoHash, false,false);
            
            if (!ParseFile.CompareSegment(outputHash, 0, this.Sha256Hash))
            {
                // @TODO: only show error once per file
                //if (!sha1ErrorDisplayed)
                {
                    MessageBox.Show(String.Format("Warning: '{0},' failed SHA256 verification in ExeFS at offset 0x{1} during extraction.{2}",
                        Path.GetFileName(destinationFile), this.Offset, Environment.NewLine), "Warning - SHA256 Failure");
                }
            }            
        }        
    }
    
    public class Nintendo3dsCtrExeFileSystem : IDirectoryStructure
    {
        public string SourceFilePath { set; get; }
        public string DirectoryName { set; get; }
        public string ParentDirectoryName { set; get; }

        public ArrayList SubDirectoryArray { set; get; }
        public IDirectoryStructure[] SubDirectories
        {
            set { this.SubDirectories = value; }
            get
            {
                SubDirectoryArray.Sort();
                return (IDirectoryStructure[])SubDirectoryArray.ToArray(typeof(Nintendo3dsCtrExeFileSystem));
            }
        }

        public ArrayList FileArray { set; get; }
        public IFileStructure[] Files
        {
            set { this.Files = value; }
            get
            {
                FileArray.Sort();
                return (IFileStructure[])FileArray.ToArray(typeof(Nintendo3dsCtrExeFileSystemFile));
            }
        }

        public Nintendo3dsCtrExeFileSystem() { }

        public Nintendo3dsCtrExeFileSystem(FileStream isoStream, string sourceFilePath, string parentDirectoryName, long offset)
        {
            string fileName;
            long fileOffset;
            uint fileLength;
            byte[] hash;
            ArrayList tempFileList;
            long hashOffset;

            Nintendo3dsCtrExeFileSystemFile file;
            string nextDirectoryName;

            this.SourceFilePath = sourceFilePath;
            this.SubDirectoryArray = new ArrayList();            
            tempFileList = new ArrayList();
            this.FileArray = new ArrayList();
            this.ParentDirectoryName = parentDirectoryName;
            this.DirectoryName = "ExeFS"; // @TODO: Make a constant
            nextDirectoryName = this.ParentDirectoryName + Path.DirectorySeparatorChar + this.DirectoryName;

            // parse file entries

            for (int i = 0; i < 10; i++)
            {
                // check if row has data
                if (ParseFile.ReadByte(isoStream, offset + (0x10 * i)) != 0)
                {
                    // read VFS items
                    fileName = ParseFile.ReadAsciiString(isoStream, offset + (0x10 * i));

                    fileOffset = ParseFile.ReadUintLE(isoStream, offset + (0x10 * i) + 8);
                    fileOffset += offset + 0x200;

                    fileLength = ParseFile.ReadUintLE(isoStream, offset + (0x10 * i) + 0xC);

                    // read SHA256 hash
                    hashOffset = offset + 0x200 - (0x20 * (i + 1));
                    hash = ParseFile.ParseSimpleOffset(isoStream, hashOffset, 0x20);

                    // build file object
                    file = new Nintendo3dsCtrExeFileSystemFile(nextDirectoryName, this.SourceFilePath,
                        fileName, fileOffset, -1, fileOffset, fileLength);
                    file.Sha256Hash = hash;
                    this.FileArray.Add(file);                    
                }
            }
        }

        public int CompareTo(object obj)
        {
            if ((obj is Nintendo3dsCtrExeFileSystem))
            {
                Nintendo3dsCtrExeFileSystem o = (Nintendo3dsCtrExeFileSystem)obj;

                return this.DirectoryName.CompareTo(o.DirectoryName);
            }            

            throw new ArgumentException("object is not an Nintendo3dsCtrExeFileSystem");
        }

        public void Extract(ref Dictionary<string, FileStream> streamCache, string destinationFolder, bool extractAsRaw)
        {
            string fullDirectoryPath = Path.Combine(destinationFolder, Path.Combine(this.ParentDirectoryName, this.DirectoryName));

            // create directory
            if (!Directory.Exists(fullDirectoryPath))
            {
                Directory.CreateDirectory(fullDirectoryPath);
            }

            foreach (Nintendo3dsCtrExeFileSystemFile f in this.FileArray)
            {
                f.Extract(ref streamCache, destinationFolder, extractAsRaw);
            }        
        }    
    }
    #endregion

    //--------------------------------------------
    // RomFS
    //--------------------------------------------
    #region RomFS
    public class IvfcLevelInfo
    {
        public ulong HashOffset { set; get; }
        public ulong HashBlockSize { set; get; }
        public ulong DataOffset { set; get; }
        public ulong DataSize { set; get; }    
    }

    public class RomFsDirEntry 
    {
        public uint ParentOffset { set; get; }
        public int SiblingOffset { set; get; }
        public int ChildOffset { set; get; }
        public int FileOffset { set; get; }
        public int WeirdOffset { set; get; }
        public uint NameSize { set; get; }
        public string Name { set; get; }

        public RomFsDirEntry() { }
    }

    public class RomFsFileEntry
    {
        public uint ParentDirOffset { set; get; }
        public uint SiblingOffset { set; get; }
        public ulong DataOffset { set; get; }
        public ulong DataSize { set; get; }
        public uint WeirdOffset { set; get; }
        public uint NameSize { set; get; }
        public string Name { set; get; }

        public RomFsFileEntry() { }
    }
    
    public class Nintendo3dsCtrRomFileSystemFile : IFileStructure
    {
        public string ParentDirectoryName { set; get; }
        public string SourceFilePath { set; get; }
        public string FileName { set; get; }
        public long Offset { set; get; }

        public long VolumeBaseOffset { set; get; }
        public long Lba { set; get; }
        public long Size { set; get; }
        public bool IsRaw { set; get; }
        public CdSectorType FileMode { set; get; }
        public int NonRawSectorSize { set; get; }

        public DateTime FileDateTime { set; get; }
        public byte[] Sha256Hash { set; get; }

        public int CompareTo(object obj)
        {
            if (obj is Nintendo3dsCtrRomFileSystemFile)
            {
                Nintendo3dsCtrRomFileSystemFile o = (Nintendo3dsCtrRomFileSystemFile)obj;

                return this.FileName.CompareTo(o.FileName);
            }

            throw new ArgumentException("object is not a Nintendo3dsCtrRomFileSystemFile");
        }

        public Nintendo3dsCtrRomFileSystemFile(string parentDirectoryName, string sourceFilePath,
            string fileName, long offset, long volumeBaseOffset, long lba, long size)
        {
            this.ParentDirectoryName = parentDirectoryName;
            this.SourceFilePath = sourceFilePath;
            this.FileName = fileName;
            this.Offset = offset;
            this.Size = size;
            this.FileDateTime = new DateTime();

            this.VolumeBaseOffset = volumeBaseOffset;
            this.Lba = lba;
            this.IsRaw = false;
            this.NonRawSectorSize = 0;
            this.FileMode = CdSectorType.Unknown;
        }

        public void Extract(ref Dictionary<string, FileStream> streamCache, string destinationFolder, bool extractAsRaw)
        {
            string destinationFile = Path.Combine(Path.Combine(destinationFolder, this.ParentDirectoryName), this.FileName);
            HashAlgorithm cryptoHash = SHA256.Create();

            if (!streamCache.ContainsKey(this.SourceFilePath))
            {
                streamCache[this.SourceFilePath] = File.OpenRead(this.SourceFilePath);
            }

            // write file while calculating hash
            byte[] outputHash = ParseFile.ExtractChunkToFile64ReturningHash(streamCache[this.SourceFilePath], (ulong)this.Offset, (ulong)this.Size,
                destinationFile, cryptoHash, false, false);

            if (!ParseFile.CompareSegment(outputHash, 0, this.Sha256Hash))
            {
                // @TODO: only show error once per file
                //if (!sha1ErrorDisplayed)
                {
                    MessageBox.Show(String.Format("Warning: '{0},' failed SHA256 verification in ExeFS at offset 0x{1} during extraction.{2}",
                        Path.GetFileName(destinationFile), this.Offset, Environment.NewLine), "Warning - SHA256 Failure");
                }
            }
        }
    }

    public class Nintendo3dsCtrRomFileSystemDirectory : IDirectoryStructure
    {
        public string SourceFilePath { set; get; }
        public string DirectoryName { set; get; }
        public string ParentDirectoryName { set; get; }

        public ArrayList SubDirectoryArray { set; get; }
        public IDirectoryStructure[] SubDirectories
        {
            set { this.SubDirectories = value; }
            get
            {
                SubDirectoryArray.Sort();
                return (IDirectoryStructure[])SubDirectoryArray.ToArray(typeof(Nintendo3dsCtrRomFileSystemDirectory));
            }
        }

        public ArrayList FileArray { set; get; }
        public IFileStructure[] Files
        {
            set { this.Files = value; }
            get
            {
                FileArray.Sort();
                return (IFileStructure[])FileArray.ToArray(typeof(Nintendo3dsCtrRomFileSystemFile)); // @TODO:  Fix this
            }
        }

        public Nintendo3dsCtrRomFileSystemDirectory() { }

        public Nintendo3dsCtrRomFileSystemDirectory(FileStream isoStream, string sourceFilePath, string parentDirectoryName, 
            long ivfcOffset, long directoryEntryOffset, long directoryBlockOffset, long fileBlockOffset)
        {
            RomFsDirEntry dir = new RomFsDirEntry();
            RomFsFileEntry file = new RomFsFileEntry();
            Nintendo3dsCtrRomFileSystemDirectory subDir;
            
            byte[] nameBytes;
            string nextDirectory;

            this.SourceFilePath = sourceFilePath;
            this.ParentDirectoryName = parentDirectoryName;

            // initialize arrays
            this.SubDirectoryArray = new ArrayList();
            this.FileArray = new ArrayList();           

            // load dir
            dir.ParentOffset = ParseFile.ReadUintLE(isoStream, ivfcOffset + directoryBlockOffset + directoryEntryOffset);
            dir.SiblingOffset = ParseFile.ReadInt32LE(isoStream, ivfcOffset + directoryBlockOffset + directoryEntryOffset + 4);
            dir.ChildOffset = ParseFile.ReadInt32LE(isoStream, ivfcOffset + directoryBlockOffset + directoryEntryOffset + 8);
            dir.FileOffset = ParseFile.ReadInt32LE(isoStream, ivfcOffset + directoryBlockOffset + directoryEntryOffset + 0xC);
            dir.WeirdOffset = ParseFile.ReadInt32LE(isoStream, ivfcOffset + directoryBlockOffset + directoryEntryOffset + 0x10);
            dir.NameSize = ParseFile.ReadUintLE(isoStream, ivfcOffset + directoryBlockOffset + directoryEntryOffset + 0x14);

            // build directory name
            if (dir.NameSize > 0)
            {
                nameBytes = ParseFile.ParseSimpleOffset(isoStream, ivfcOffset + directoryBlockOffset + directoryEntryOffset + 0x18, (int)dir.NameSize);
                dir.Name = ByteConversion.GetUtf16LeText(nameBytes);
            }
            else // this is root
            {
                dir.Name = "RomFS";
            }

            this.DirectoryName = dir.Name;
            nextDirectory = this.ParentDirectoryName + Path.DirectorySeparatorChar + this.DirectoryName;

            // add files




            // add subdirs
            if (dir.ChildOffset != -1)
            {
                subDir = new Nintendo3dsCtrRomFileSystemDirectory(isoStream, sourceFilePath, nextDirectory,
                    ivfcOffset, dir.ChildOffset, directoryBlockOffset, fileBlockOffset);
                this.SubDirectoryArray.Add(subDir);
            }

        }

        public int CompareTo(object obj)
        {
            if (obj is Nintendo3dsCtrRomFileSystemDirectory)
            {
                Nintendo3dsCtrRomFileSystemDirectory o = (Nintendo3dsCtrRomFileSystemDirectory)obj;

                return this.DirectoryName.CompareTo(o.DirectoryName);
            }

            throw new ArgumentException("object is not an Nintendo3dsCtrRomFileSystemDirectory");
        }

        public void Extract(ref Dictionary<string, FileStream> streamCache, string destinationFolder, bool extractAsRaw)
        {
            string fullDirectoryPath = Path.Combine(destinationFolder, Path.Combine(this.ParentDirectoryName, this.DirectoryName));

            // create directory
            if (!Directory.Exists(fullDirectoryPath))
            {
                Directory.CreateDirectory(fullDirectoryPath);
            }

            foreach (Nintendo3dsCtrRomFileSystemFile f in this.FileArray)
            {
                f.Extract(ref streamCache, destinationFolder, extractAsRaw);
            }
        }
    }

    public class Nintendo3dsCtrRomFileSystem //: IDirectoryStructure
    {
        public string SourceFilePath { set; get; }
        public string DirectoryName { set; get; }
        public string ParentDirectoryName { set; get; }
        public Nintendo3dsCtrRomFileSystemDirectory RootDirectory { set; get; }

        // RomFS Header

        public uint MagicBytes { set; get; }
        public uint VersionNumber { set; get; }
        public uint MasterHashSize { set; get; }

        public ulong Level1Offset { set; get; }
        public ulong Level1HashDataSize { set; get; }
        public uint Level1BlockSize { set; get; }
        public uint Reserved01 { set; get; }

        public ulong Level2Offset { set; get; }
        public ulong Level2HashDataSize { set; get; }
        public uint Level2BlockSize { set; get; }
        public uint Reserved02 { set; get; }

        public ulong Level3Offset { set; get; }
        public ulong Level3HashDataSize { set; get; }
        public uint Level3BlockSize { set; get; }
        public uint Reserved03 { set; get; }

        public uint Reserved04 { set; get; }
        public uint OptionalInfoSize { set; get; }

        // IVFC Header
        public IvfcLevelInfo[] IvfcLevels { set; get; }
        
        public ulong BodyOffset { set; get; }
        public ulong BodySize { set; get; }

        // RomFS Section Header
        public uint RomFsHeaderSize { set; get; }
        public SimpleOffset[] RomFsSections { set; get; }
        public uint RomFsDataOffset { set; get; }



        public Nintendo3dsCtrRomFileSystem() { }

        public Nintendo3dsCtrRomFileSystem(FileStream isoStream, string sourceFilePath, string parentDirectoryName, long offset)
        {
            string nextDirectoryName;

            this.SourceFilePath = sourceFilePath;
            this.ParentDirectoryName = parentDirectoryName;
            this.DirectoryName = "RomFS"; // @TODO: Make a constant
            nextDirectoryName = this.ParentDirectoryName + Path.DirectorySeparatorChar + this.DirectoryName;

            // parse IVFC header
            this.ParseIvfcHeader(isoStream, offset);

            // Build Validation Info
            this.BuildIvfcLevels();
            
            // Validate IVFC Levels

            // Parse RomFS Header
            this.ParseRomFsHeader(isoStream, offset);

            // build directory structure
            this.RootDirectory = new Nintendo3dsCtrRomFileSystemDirectory(isoStream, sourceFilePath, parentDirectoryName, 
                offset, 0, (long)(this.RomFsSections[1].Offset + 0x1000), 
                (long)(this.RomFsSections[3].Offset + 0x1000));
        }

        public void ParseIvfcHeader(FileStream fs, long offset)
        {
            this.MagicBytes = ParseFile.ReadUintBE(fs, offset);
            
            // verify magic bytes
            if (this.MagicBytes != 0x49564643)
            {
                throw new FormatException(String.Format("IVFC bytes not found."));
            }
                        
            this.VersionNumber = ParseFile.ReadUintLE(fs, offset + 0x04);
            this.MasterHashSize = ParseFile.ReadUintLE(fs, offset + 0x08);

            this.Level1Offset = ParseFile.ReadUlongLE(fs, offset + 0x0C);
            this.Level1HashDataSize = ParseFile.ReadUlongLE(fs, offset + 0x14);
            this.Level1BlockSize = ParseFile.ReadUintLE(fs, offset + 0x1C);
            this.Reserved01 = ParseFile.ReadUintLE(fs, offset + 0x20);

            this.Level2Offset = ParseFile.ReadUlongLE(fs, offset + 0x24);
            this.Level2HashDataSize = ParseFile.ReadUlongLE(fs, offset + 0x2C);
            this.Level2BlockSize = ParseFile.ReadUintLE(fs, offset + 0x34);
            this.Reserved02 = ParseFile.ReadUintLE(fs, offset + 0x38);

            this.Level3Offset = ParseFile.ReadUlongLE(fs, offset + 0x3C);
            this.Level3HashDataSize = ParseFile.ReadUlongLE(fs, offset + 0x44);
            this.Level3BlockSize = ParseFile.ReadUintLE(fs, offset + 0x4C);
            this.Reserved03 = ParseFile.ReadUintLE(fs, offset + 0x50);

            this.Reserved04 = ParseFile.ReadUintLE(fs, offset + 0x54);
            this.OptionalInfoSize = ParseFile.ReadUintLE(fs, offset + 0x58);               
        }

        public void BuildIvfcLevels() // thanks to neimod's ctrtool
        {
            this.IvfcLevels = new IvfcLevelInfo[3];
            for (int i = 0; i < 3; i++) { this.IvfcLevels[i] = new IvfcLevelInfo(); }

            this.IvfcLevels[0].HashOffset = 0x60;
            this.IvfcLevels[0].HashBlockSize = (ulong)1 << (int)this.Level1BlockSize;
            this.IvfcLevels[1].HashBlockSize = (ulong)1 << (int)this.Level2BlockSize;
            this.IvfcLevels[2].HashBlockSize = (ulong)1 << (int)this.Level3BlockSize;

            this.BodyOffset = MathUtil.RoundUpToByteAlignment((this.IvfcLevels[0].HashOffset + (ulong)this.MasterHashSize), 
                                                               this.IvfcLevels[2].HashBlockSize);
            this.BodySize = this.Level3HashDataSize;

            this.IvfcLevels[2].DataOffset = this.BodyOffset;
            this.IvfcLevels[2].DataSize = MathUtil.RoundUpToByteAlignment(this.BodySize, this.IvfcLevels[2].HashBlockSize);

            this.IvfcLevels[1].HashOffset = MathUtil.RoundUpToByteAlignment((this.BodyOffset + this.BodySize),
                                                                             this.IvfcLevels[2].HashBlockSize);
            this.IvfcLevels[2].HashOffset = this.IvfcLevels[1].HashOffset + this.Level2Offset - this.Level1Offset;

            this.IvfcLevels[1].DataOffset = this.IvfcLevels[2].HashOffset;
            this.IvfcLevels[1].DataSize = MathUtil.RoundUpToByteAlignment(this.Level2HashDataSize, this.IvfcLevels[1].HashBlockSize);

            this.IvfcLevels[0].DataOffset = this.IvfcLevels[1].HashOffset;
            this.IvfcLevels[0].DataSize = MathUtil.RoundUpToByteAlignment(this.Level1HashDataSize, this.IvfcLevels[0].HashBlockSize);
        }

        public void ParseRomFsHeader(FileStream fs, long offset)
        {
            this.RomFsHeaderSize = ParseFile.ReadUintLE(fs, offset + 0x1000);
            
            this.RomFsSections = new SimpleOffset[4];
            for (int i = 0; i < 4; i++)
            {
                this.RomFsSections[i] = new SimpleOffset((ParseFile.ReadUintLE(fs, offset + 0x1004 + (8 * i))), 
                                                          ParseFile.ReadUintLE(fs, offset + 0x1004 + ((8 * i) + 4)));
            }

            this.RomFsDataOffset = ParseFile.ReadUintLE(fs, offset + 0x1024);
        }        
    }



    #endregion

}
