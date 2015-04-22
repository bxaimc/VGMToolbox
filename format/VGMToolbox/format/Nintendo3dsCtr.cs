using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using VGMToolbox.format.iso;
using VGMToolbox.util;

namespace VGMToolbox.format
{
    public class SimpleNcchOffset
    {
        public uint Offset { set; get; }
        public uint Size { set; get; }

        public SimpleNcchOffset(uint offset, uint size)
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

        public long DiscBaseOffset { set; get; }
        public string SourceFileName { set; get; }
        public bool IsRawDump { set; get; }
        public ArrayList VolumeArrayList { set; get; }
        

        // NCSD Header

        public byte[] NcsdHash { set; get; }
        public uint MagicBytes { set; get; }
        public uint ImageSize { set; get; }
        public ulong MediaId { set; get; }
        public ulong PartitionsFsType { set; get; }
        public ulong PartitionsEncryptionType { set; get; }
        public SimpleNcchOffset[] NcchOffsetInfo { set; get; }

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

            for (int i = 0; i < NcchOffsetInfo.Length; i++)
            {
                if (this.NcchOffsetInfo[i].Offset > 0)
                {
                    newVolume = new Nintendo3dsNcchContainer();
                    newVolume.Initialize(isoStream,
                        this.DiscBaseOffset + (long)((long)this.NcchOffsetInfo[i].Offset * Nintendo3dsCtr.MEDIA_UNIT_SIZE),
                        this.IsRawDump);
                    this.VolumeArrayList.Add(newVolume);
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

            this.NcchOffsetInfo = new SimpleNcchOffset[8];
            for (int i = 0; i < 8; i++)
            {
                this.NcchOffsetInfo[i] = new SimpleNcchOffset(
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
        public byte[] ProductCode { set; get; }
        
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

        #region IVolume
        public ArrayList DirectoryStructureArray { set; get; }
        public IDirectoryStructure[] Directories
        {
            set { Directories = value; }
            get
            {
                DirectoryStructureArray.Sort();
                return (IDirectoryStructure[])DirectoryStructureArray.ToArray(typeof(NintendoU8Directory)); // @TODO Fix this
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
            this.ProductCode = ParseFile.ParseSimpleOffset(fs, offset + 0x150, 0x10);

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
            this.FormatDescription = Nintendo3dsCtr.FORMAT_DESCRIPTION_STRING_DECRYPTED;

            this.VolumeBaseOffset = offset;
            this.IsRawDump = isRawDump;
            this.VolumeType = VolumeDataType.Data;
            this.DirectoryStructureArray = new ArrayList();

            this.ParseNcchHeader(fs, offset);   
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
}
