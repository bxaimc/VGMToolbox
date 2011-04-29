using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VGMToolbox.util;

namespace VGMToolbox.format.iso
{
    public class GreenBookCdi : Iso9660
    {
        new public static readonly byte[] VOLUME_DESCRIPTOR_IDENTIFIER = new byte[] { 0x01, 0x43, 0x44, 0x2D, 0x49, 0x20 };
        new public static string FORMAT_DESCRIPTION_STRING = "CDI";
    }

    public class GreenBookCdiVolume : Iso9660Volume
    {
        public virtual void Initialize(FileStream isoStream, long offset, bool isRawDump)
        {
            byte[] sectorBytes;
            byte[] sectorDataBytes;

            this.FormatDescription = Iso9660.FORMAT_DESCRIPTION_STRING;
            this.IsRawDump = isRawDump;
            this.DirectoryStructureArray = new ArrayList();

            this.VolumeBaseOffset =
                this.IsRawDump ? (offset - Iso9660.EMPTY_HEADER_SIZE_RAW) : (offset - Iso9660.EMPTY_HEADER_SIZE);
            this.SectorSize =
                this.IsRawDump ? (int)CdRom.RAW_SECTOR_SIZE : (int)CdRom.NON_RAW_SECTOR_SIZE;

            // parse inital level sector
            sectorBytes = ParseFile.ParseSimpleOffset(isoStream, offset, this.SectorSize);
            sectorDataBytes = CdRom.GetDataChunkFromSector(sectorBytes, this.IsRawDump);


            this.VolumeDescriptorType = ParseFile.ParseSimpleOffset(sectorDataBytes, 0x00, 1)[0];
            this.StandardIdentifier = ParseFile.ParseSimpleOffset(sectorDataBytes, 0x01, 5);
            this.VolumeDescriptorVersion = ParseFile.ParseSimpleOffset(sectorDataBytes, 0x06, 1)[0];

            this.UnusedField1 = ParseFile.ParseSimpleOffset(sectorDataBytes, 0x07, 1)[0];

            this.SystemIdentifier = ByteConversion.GetAsciiText(ParseFile.ParseSimpleOffset(sectorDataBytes, 0x08, 0x20)).Trim();
            this.VolumeIdentifier = ByteConversion.GetAsciiText(FileUtil.ReplaceNullByteWithSpace(ParseFile.ParseSimpleOffset(sectorDataBytes, 0x28, 0x20))).Trim();

            this.UnusedField2 = ParseFile.ParseSimpleOffset(sectorDataBytes, 0x48, 0x08);

            this.VolumeSpaceSize = ByteConversion.GetUInt32BigEndian(ParseFile.ParseSimpleOffset(sectorDataBytes, 0x54, 0x04));

            this.UnusedField3 = ParseFile.ParseSimpleOffset(sectorDataBytes, 0x58, 0x20);

            this.VolumeSetSize = ByteConversion.GetUInt16BigEndian(ParseFile.ParseSimpleOffset(sectorDataBytes, 0x7A, 0x02));
            this.VolumeSequenceNumber = ByteConversion.GetUInt16BigEndian(ParseFile.ParseSimpleOffset(sectorDataBytes, 0x7E, 0x02));
            this.LogicalBlockSize = ByteConversion.GetUInt16BigEndian(ParseFile.ParseSimpleOffset(sectorDataBytes, 0x82, 0x02));

            this.PathTableSize = ByteConversion.GetUInt32BigEndian(ParseFile.ParseSimpleOffset(sectorDataBytes, 0x88, 0x04));
            this.LocationOfOccurrenceOfTypeLPathTable = BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(sectorDataBytes, 0x8C, 0x04), 0);
            this.LocationOfOptionalOccurrenceOfTypeLPathTable = BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(sectorDataBytes, 0x90, 0x04), 0);
            this.LocationOfOccurrenceOfTypeMPathTable = ByteConversion.GetUInt32BigEndian(ParseFile.ParseSimpleOffset(sectorDataBytes, 0x94, 0x04));
            this.LocationOfOptionalOccurrenceOfTypeMPathTable = ByteConversion.GetUInt32BigEndian(ParseFile.ParseSimpleOffset(sectorDataBytes, 0x98, 0x04));

            this.DirectoryRecordForRootDirectoryBytes = ParseFile.ParseSimpleOffset(sectorDataBytes, 0x9C, 0x22);
            this.DirectoryRecordForRootDirectory = new Iso9660DirectoryRecord(this.DirectoryRecordForRootDirectoryBytes);

            this.VolumeSetIdentifier = ByteConversion.GetAsciiText(ParseFile.ParseSimpleOffset(sectorDataBytes, 0xBE, 0x80)).Trim();
            this.PublisherIdentifier = ByteConversion.GetAsciiText(ParseFile.ParseSimpleOffset(sectorDataBytes, 0x13E, 0x80)).Trim();
            this.DataPreparerIdentifier = ByteConversion.GetAsciiText(ParseFile.ParseSimpleOffset(sectorDataBytes, 0x1BE, 0x80)).Trim();
            this.ApplicationIdentifier = ByteConversion.GetAsciiText(ParseFile.ParseSimpleOffset(sectorDataBytes, 0x23E, 0x80)).Trim();
            this.CopyrightFileIdentifier = ByteConversion.GetAsciiText(ParseFile.ParseSimpleOffset(sectorDataBytes, 0x2BE, 0x25)).Trim();
            this.AbstractFileIdentifier = ByteConversion.GetAsciiText(ParseFile.ParseSimpleOffset(sectorDataBytes, 0x2E3, 0x25)).Trim();
            this.BibliographicFileIdentifier = ByteConversion.GetAsciiText(ParseFile.ParseSimpleOffset(sectorDataBytes, 0x308, 0x25)).Trim();

            this.VolumeCreationDateAndTime = Iso9660.GetIsoDateTime(ParseFile.ParseSimpleOffset(sectorDataBytes, 0x32D, 0x11));
            this.VolumeModificationDateAndTime = Iso9660.GetIsoDateTime(ParseFile.ParseSimpleOffset(sectorDataBytes, 0x33E, 0x11));
            this.VolumeExpirationDateAndTime = Iso9660.GetIsoDateTime(ParseFile.ParseSimpleOffset(sectorDataBytes, 0x34F, 0x11));
            this.VolumeEffectiveDateAndTime = Iso9660.GetIsoDateTime(ParseFile.ParseSimpleOffset(sectorDataBytes, 0x360, 0x11));

            this.FileStructureVersion = ParseFile.ParseSimpleOffset(sectorDataBytes, 0x371, 1)[0];

            this.Reserved1 = ParseFile.ParseSimpleOffset(sectorDataBytes, 0x372, 1)[0];

            this.ApplicationUse = ParseFile.ParseSimpleOffset(sectorDataBytes, 0x373, 0x200);

            this.Reserved2 = ParseFile.ParseSimpleOffset(sectorDataBytes, 0x573, 0x28D);

            this.LoadDirectories(isoStream);
        }

        public virtual void LoadDirectories(FileStream isoStream)
        {
            // change name of top level folder
            this.DirectoryRecordForRootDirectory.FileIdentifierString = String.Empty;

            // get first path record
            byte[] rootDirPathBytes = CdRom.GetSectorByLba(isoStream, this.VolumeBaseOffset, this.LocationOfOccurrenceOfTypeMPathTable, this.IsRawDump, this.LogicalBlockSize);
            rootDirPathBytes = CdRom.GetDataChunkFromSector(rootDirPathBytes, this.IsRawDump);

            // manually build directory records
            this.DirectoryRecordForRootDirectory.FlagDirectory = true;
            this.DirectoryRecordForRootDirectory.DataLength = this.LogicalBlockSize;
            this.DirectoryRecordForRootDirectory.LocationOfExtent = 
                ByteConversion.GetUInt32BigEndian(ParseFile.ParseSimpleOffset(rootDirPathBytes, 2, 4));


            // populate this volume's directory structure
            Iso9660DirectoryStructure rootDirectory =
                new Iso9660DirectoryStructure(isoStream, isoStream.Name,
                    this.VolumeBaseOffset, this.DirectoryRecordForRootDirectory,
                    this.LogicalBlockSize, this.IsRawDump, this.SectorSize, null);
            this.DirectoryStructureArray.Add(rootDirectory);
        }
    }
}
