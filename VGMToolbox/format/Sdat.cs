using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ICSharpCode.SharpZipLib.Checksums;

using VGMToolbox.util;
using VGMToolbox.util.ObjectPooling;

namespace VGMToolbox.format
{
    class Sdat : IFormat
    {
        private static readonly byte[] ASCII_SIGNATURE = new byte[] { 0x53, 0x44, 0x41, 0x54 }; // SDAT
        private const string FORMAT_ABBREVIATION = "SDAT";
        private const string HEX_PREFIX = "0x";
        
        ///////////////////////////////////
        // Standard NDS Header Information
        /// ///////////////////////////////
        public const int STD_HEADER_SIGNATURE_OFFSET = 0x00;
        public const int STD_HEADER_SIGNATURE_LENGTH = 4;

        public const int STD_HEADER_UNK_CONSTANT_OFFSET = 0x04;
        public const int STD_HEADER_UNK_CONSTANT_LENGTH = 4;

        public const int STD_HEADER_FILE_SIZE_OFFSET = 0x08;
        public const int STD_HEADER_FILE_SIZE_LENGTH = 4;

        public const int STD_HEADER_HEADER_SIZE_OFFSET = 0x0C;
        public const int STD_HEADER_HEADER_SIZE_LENGTH = 2;

        public const int STD_HEADER_NUMBER_OF_SECTIONS_OFFSET = 0x0E;
        public const int STD_HEADER_NUMBER_OF_SECTIONS_LENGTH = 2;

        ///////////////////////////////////
        // SDAT Specific Header Information
        ///////////////////////////////////
        // SYMB
        public const int SDAT_HEADER_SYMB_OFFSET_OFFSET = 0x10;
        public const int SDAT_HEADER_SYMB_OFFSET_LENGTH = 4;
        public const int SDAT_HEADER_SYMB_SIZE_OFFSET = 0x14;
        public const int SDAT_HEADER_SYMB_SIZE_LENGTH = 4;

        // INFO
        public const int SDAT_HEADER_INFO_OFFSET_OFFSET = 0x18;
        public const int SDAT_HEADER_INFO_OFFSET_LENGTH = 4;
        public const int SDAT_HEADER_INFO_SIZE_OFFSET = 0x1C;
        public const int SDAT_HEADER_INFO_SIZE_LENGTH = 4;

        // FAT
        public const int SDAT_HEADER_FAT_OFFSET_OFFSET = 0x20;
        public const int SDAT_HEADER_FAT_OFFSET_LENGTH = 4;
        public const int SDAT_HEADER_FAT_SIZE_OFFSET = 0x24;
        public const int SDAT_HEADER_FAT_SIZE_LENGTH = 4;

        // FILE
        public const int SDAT_HEADER_FILE_OFFSET_OFFSET = 0x28;
        public const int SDAT_HEADER_FILE_OFFSET_LENGTH = 4;
        public const int SDAT_HEADER_FILE_SIZE_OFFSET = 0x2C;
        public const int SDAT_HEADER_FILE_SIZE_LENGTH = 4;

        // UNKNOWN CONSTANT - PADDING?
        public const int SDAT_HEADER_UNK_PADDING_OFFSET = 0x30;
        public const int SDAT_HEADER_UNK_PADDING_LENGTH = 16;

        /////////////
        // Variables
        /////////////
        private byte[] stdHeaderSignature;
        private byte[] stdHeaderUnkConstant;
        private byte[] stdHeaderFileSize;
        private byte[] stdHeaderHeaderSize;
        private byte[] stdHeaderNumberOfSections;

        private byte[] sdatHeaderSymbOffset;
        private byte[] sdatHeaderSymbSize;

        private byte[] sdatHeaderInfoOffset;
        private byte[] sdatHeaderInfoSize;

        private byte[] sdatHeaderFatOffset;
        private byte[] sdatHeaderFatSize;

        private byte[] sdatHeaderFileOffset;
        private byte[] sdatHeaderFileSize;

        private byte[] sdatHeaderUnkPadding;

        // Tag Hash
        Dictionary<string, string> tagHash = new Dictionary<string, string>();

        // Sections
        SdatSymbSection symbSection = null;
        SdatInfoSection infoSection = null;        
        SdatFileSection fileSection = null;

        // METHODS        
        public byte[] getStdHeaderSignature(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, STD_HEADER_SIGNATURE_OFFSET, STD_HEADER_SIGNATURE_LENGTH);
        }
        public byte[] getStdHeaderUnkConstant(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, STD_HEADER_UNK_CONSTANT_OFFSET, STD_HEADER_UNK_CONSTANT_LENGTH);
        }
        public byte[] getStdHeaderFileSize(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, STD_HEADER_FILE_SIZE_OFFSET, STD_HEADER_FILE_SIZE_LENGTH);
        }
        public byte[] getStdHeaderHeaderSize(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, STD_HEADER_HEADER_SIZE_OFFSET, STD_HEADER_HEADER_SIZE_LENGTH);
        }
        public byte[] getStdHeaderNumberOfSections(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, STD_HEADER_NUMBER_OF_SECTIONS_OFFSET, STD_HEADER_NUMBER_OF_SECTIONS_LENGTH);
        }

        public byte[] getSdatHeaderSymbOffset(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, SDAT_HEADER_SYMB_OFFSET_OFFSET, SDAT_HEADER_SYMB_OFFSET_LENGTH);
        }
        public byte[] getSdatHeaderSymbSize(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, SDAT_HEADER_SYMB_SIZE_OFFSET, SDAT_HEADER_SYMB_SIZE_LENGTH);
        }

        public byte[] getSdatHeaderInfoOffset(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, SDAT_HEADER_INFO_OFFSET_OFFSET, SDAT_HEADER_INFO_OFFSET_LENGTH);
        }
        public byte[] getSdatHeaderInfoSize(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, SDAT_HEADER_INFO_SIZE_OFFSET, SDAT_HEADER_INFO_SIZE_LENGTH);
        }

        public byte[] getSdatHeaderFatOffset(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, SDAT_HEADER_FAT_OFFSET_OFFSET, SDAT_HEADER_FAT_OFFSET_LENGTH);
        }
        public byte[] getSdatHeaderFatSize(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, SDAT_HEADER_FAT_SIZE_OFFSET, SDAT_HEADER_FAT_SIZE_LENGTH);
        }

        public byte[] getSdatHeaderFileOffset(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, SDAT_HEADER_FILE_OFFSET_OFFSET, SDAT_HEADER_FILE_OFFSET_LENGTH);
        }
        public byte[] getSdatHeaderFileSize(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, SDAT_HEADER_FILE_SIZE_OFFSET, SDAT_HEADER_FILE_SIZE_LENGTH);
        }

        public byte[] getSdatHeaderUnkPadding(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, SDAT_HEADER_UNK_PADDING_OFFSET, SDAT_HEADER_UNK_PADDING_LENGTH);
        }


        public void Initialize(Stream pStream)
        { 
            // SDAT
            stdHeaderSignature = getStdHeaderSignature(pStream);
            stdHeaderUnkConstant = getStdHeaderUnkConstant(pStream);
            stdHeaderFileSize = getStdHeaderFileSize(pStream);
            stdHeaderHeaderSize = getStdHeaderHeaderSize(pStream);
            stdHeaderNumberOfSections = getStdHeaderNumberOfSections(pStream);

            sdatHeaderSymbOffset = getSdatHeaderSymbOffset(pStream);
            sdatHeaderSymbSize = getSdatHeaderSymbSize(pStream);

            sdatHeaderInfoOffset = getSdatHeaderInfoOffset(pStream);
            sdatHeaderInfoSize = getSdatHeaderInfoSize(pStream);

            sdatHeaderFatOffset = getSdatHeaderFatOffset(pStream);
            sdatHeaderFatSize = getSdatHeaderFatSize(pStream);

            sdatHeaderFileOffset = getSdatHeaderFileOffset(pStream);
            sdatHeaderFileSize = getSdatHeaderFileSize(pStream);

            sdatHeaderUnkPadding = getSdatHeaderUnkPadding(pStream);

            // SYMB Section
            if (BitConverter.ToUInt32(this.sdatHeaderSymbSize, 0) > 0)
            {
                symbSection = new SdatSymbSection();
                symbSection.Initialize(pStream, BitConverter.ToInt32(this.sdatHeaderSymbOffset, 0));
            }
            
            // INFO
            /*
            if (BitConverter.ToUInt32(this.sdatHeaderInfoSize, 0) > 0)
            {
                infoSection = new SdatInfoSection();
                infoSection.Initialize(pStream, BitConverter.ToInt32(this.sdatHeaderInfoOffset, 0));
            }
            */
             
            // FILE Section
            if (BitConverter.ToUInt32(this.sdatHeaderFileSize, 0) > 0)
            {
                fileSection = new SdatFileSection();
                fileSection.Initialize(pStream, BitConverter.ToInt32(this.sdatHeaderFileOffset, 0));
            }

            this.initializeTagHash();
        }

        private void addNumberedListToTagHash(string pLabel, string[] pList)
        {
            int fileCount = 0;
            foreach (string s in pList)
            {
                tagHash.Add(pLabel + " " + fileCount.ToString("X4"), s);
                fileCount++;
            }        
        }
        
        private void initializeTagHash()
        {
            #region initializeTagHash - SDAT
            
            tagHash.Add("SDAT - File Size", HEX_PREFIX + BitConverter.ToUInt32(this.stdHeaderFileSize, 0).ToString("X4"));
            tagHash.Add("SDAT - Header Size", HEX_PREFIX + BitConverter.ToUInt16(this.stdHeaderHeaderSize, 0).ToString("X2"));
            tagHash.Add("SDAT - Number of Sections", HEX_PREFIX + BitConverter.ToUInt16(this.stdHeaderNumberOfSections, 0).ToString("X2"));
            tagHash.Add("SDAT - SYMB Offset, Length",
                HEX_PREFIX + BitConverter.ToUInt32(this.sdatHeaderSymbOffset, 0).ToString("X4") + ", " +
                HEX_PREFIX + BitConverter.ToUInt32(this.sdatHeaderSymbSize, 0).ToString("X4"));
            tagHash.Add("SDAT - INFO Offset, Length",
                HEX_PREFIX + BitConverter.ToUInt32(this.sdatHeaderInfoOffset, 0).ToString("X4") + ", " +
                HEX_PREFIX + BitConverter.ToUInt32(this.sdatHeaderInfoSize, 0).ToString("X4"));
            tagHash.Add("SDAT - FAT Offset, Length",
                HEX_PREFIX + BitConverter.ToUInt32(this.sdatHeaderFatOffset, 0).ToString("X4") + ", " +
                HEX_PREFIX + BitConverter.ToUInt32(this.sdatHeaderFatSize, 0).ToString("X4"));
            tagHash.Add("SDAT - FILE Offset, Length",
                HEX_PREFIX + BitConverter.ToUInt32(this.sdatHeaderFileOffset, 0).ToString("X4") + ", " +
                HEX_PREFIX + BitConverter.ToUInt32(this.sdatHeaderFileSize, 0).ToString("X4"));
            
            #endregion

            #region initializeTagHash - SYMB (INCOMPLETE)
            
            if (symbSection != null)
            {                
                tagHash.Add("SYMB - Section Size", HEX_PREFIX + BitConverter.ToUInt32(symbSection.StdHeaderSectionSize, 0).ToString("X4"));
                tagHash.Add("SYMB - SEQ Offset", HEX_PREFIX + BitConverter.ToUInt32(symbSection.SymbRecordSeqOffset, 0).ToString("X4"));
                tagHash.Add("SYMB - SEQARC Offset", HEX_PREFIX + BitConverter.ToUInt32(symbSection.SymbRecordSeqArcOffset, 0).ToString("X4"));
                tagHash.Add("SYMB - BANK Offset", HEX_PREFIX + BitConverter.ToUInt32(symbSection.SymbRecordBankOffset, 0).ToString("X4"));
                tagHash.Add("SYMB - WAVEARC Offset", HEX_PREFIX + BitConverter.ToUInt32(symbSection.SymbRecordWaveArcOffset, 0).ToString("X4"));
                tagHash.Add("SYMB - PLAYER Offset", HEX_PREFIX + BitConverter.ToUInt32(symbSection.SymbRecordPlayerOffset, 0).ToString("X4"));
                tagHash.Add("SYMB - GROUP Offset", HEX_PREFIX + BitConverter.ToUInt32(symbSection.SymbRecordGroupOffset, 0).ToString("X4"));
                tagHash.Add("SYMB - PLAYER2 Offset", HEX_PREFIX + BitConverter.ToUInt32(symbSection.SymbRecordPlayer2Offset, 0).ToString("X4"));
                tagHash.Add("SYMB - STRM Offset", HEX_PREFIX + BitConverter.ToUInt32(symbSection.SymbRecordStrmOffset, 0).ToString("X4"));
                
                addNumberedListToTagHash("SYMB - SEQ", symbSection.SymbSeqFileNames);

                addNumberedListToTagHash("SYMB - BANK", symbSection.SymbBankFileNames);
                addNumberedListToTagHash("SYMB - WAVEARC", symbSection.SymbWaveArcFileNames);
                addNumberedListToTagHash("SYMB - PLAYER", symbSection.SymbPlayerFileNames);
                addNumberedListToTagHash("SYMB - GROUP", symbSection.SymbGroupFileNames);
                addNumberedListToTagHash("SYMB - PLAYER2", symbSection.SymbPlayer2FileNames);
                addNumberedListToTagHash("SYMB - STRM", symbSection.SymbStrmFileNames);                
            }
            
            #endregion

            #region initializeTagHash - FILE

            if (fileSection != null)
            {
                tagHash.Add("FILE - Total Files", HEX_PREFIX + BitConverter.ToUInt32(fileSection.FileHeaderNumberOfFiles, 0).ToString("X4"));
            }

            #endregion

        }

        #region IFomat Required Functions
        
        public byte[] GetAsciiSignature()
        {
            return ASCII_SIGNATURE;
        }

        public string GetFileExtensions()
        {
            return null;
        }

        public string GetFormatAbbreviation()
        { 
            return FORMAT_ABBREVIATION;
        }

        public bool IsFileLibrary(string pPath)
        {
            return false;
        }

        public bool HasMultipleFileExtensions()
        {
            return false;
        }

        public int GetStartingSong() { return 0; }
        public int GetTotalSongs() { return 0; }
        public string GetSongName() { return null; }

        public string GetHootDriverAlias() { return null; }
        public string GetHootDriverType() { return null; }
        public string GetHootDriver() { return null; }

        public Dictionary<string, string> GetTagHash()
        {
            return this.tagHash;
        }

        public void GetDatFileCrc32(string pPath, ref Dictionary<string, ByteArray> pLibHash,
            ref Crc32 pChecksum, bool pUseLibHash)
        {
            pChecksum.Reset();
        }
        
        #endregion

    }

    class SdatSymbSection 
    {
        public SdatSymbSection() { }

        private static readonly byte[] NULL_BYTE_ARRAY = new byte[] { 0x00 }; // NULL

        public const int STD_HEADER_SIGNATURE_OFFSET = 0x00;
        public const int STD_HEADER_SIGNATURE_LENGTH = 4;

        public const int STD_HEADER_SECTION_SIZE_OFFSET = 0x04;
        public const int STD_HEADER_SECTION_SIZE_LENGTH = 4;

        // offsets, releative to this section
        public const int SYMB_RECORD_SEQ_OFFSET_OFFSET = 0x08;
        public const int SYMB_RECORD_SEQ_OFFSET_LENGTH = 4;

        public const int SYMB_RECORD_SEQARC_OFFSET_OFFSET = 0x0C;
        public const int SYMB_RECORD_SEQARC_OFFSET_LENGTH = 4;

        public const int SYMB_RECORD_BANK_OFFSET_OFFSET = 0x10;
        public const int SYMB_RECORD_BANK_OFFSET_LENGTH = 4;

        public const int SYMB_RECORD_WAVEARC_OFFSET_OFFSET = 0x14;
        public const int SYMB_RECORD_WAVEARC_OFFSET_LENGTH = 4;

        public const int SYMB_RECORD_PLAYER_OFFSET_OFFSET = 0x18;
        public const int SYMB_RECORD_PLAYER_OFFSET_LENGTH = 4;

        public const int SYMB_RECORD_GROUP_OFFSET_OFFSET = 0x1C;
        public const int SYMB_RECORD_GROUP_OFFSET_LENGTH = 4;

        // conflict between specs here!
        // http://tahaxan.arcnor.com/index.php?option=com_content&task=view&id=38&Itemid=36
        // http://kiwi.ds.googlepages.com/sdat.html

        public const int SYMB_RECORD_PLAYER2_OFFSET_OFFSET = 0x20;
        public const int SYMB_RECORD_PLAYER2_OFFSET_LENGTH = 4;
        
        public const int SYMB_RECORD_STRM_OFFSET_OFFSET = 0x24;
        public const int SYMB_RECORD_STRM_OFFSET_LENGTH = 4;

        public const int SYMB_RECORD_UNUSED_OFFSET = 0x28;
        public const int SYMB_RECORD_UNUSED_LENGTH = 24;

        // THESE SHOULD EXIST FOR EACH SECTION
        public const int SYMB_ENTRY_NUM_FILES_OFFSET = 0x00;        // RELATIVE TO SECTION OFFSET
        public const int SYMB_ENTRY_NUM_FILES_LENGTH = 4;

        public const int SYMB_ENTRY_FILE_NAME_SIZE = 4;

        // !!!!! Add stuff here for subrecords for SEQARC

        /////////////
        // Variables
        /////////////        

        // private
        private byte[] stdHeaderSignature;        
        private byte[] stdHeaderSectionSize;
        
        private byte[] symbRecordSeqOffset;
        private byte[] symbRecordSeqArcOffset;
        private byte[] symbRecordBankOffset;
        private byte[] symbRecordWaveArcOffset;
        private byte[] symbRecordPlayerOffset;
        private byte[] symbRecordGroupOffset;
        private byte[] symbRecordPlayer2Offset;
        private byte[] symbRecordStrmOffset;

        private byte[][] symbSeqSubRecords;
        private byte[][] symbSeqFileNames;

        private byte[][] symbBankSubRecords;
        private byte[][] symbBankFileNames;
        private byte[][] symbWaveArcSubRecords;
        private byte[][] symbWaveArcFileNames;
        private byte[][] symbPlayerSubRecords;
        private byte[][] symbPlayerFileNames;
        private byte[][] symbGroupSubRecords;
        private byte[][] symbGroupFileNames;
        private byte[][] symbPlayer2SubRecords;
        private byte[][] symbPlayer2FileNames;
        private byte[][] symbStrmSubRecords;
        private byte[][] symbStrmFileNames;

        struct sdatSymbRec
        {
            public byte[] nEntryOffset;
            public byte[] nSubRecOffset;
        }
        
        // public
        public byte[] StdHeaderSignature { get { return stdHeaderSignature; } }
        public byte[] StdHeaderSectionSize { get { return stdHeaderSectionSize; } }

        public byte[] SymbRecordSeqOffset { get { return symbRecordSeqOffset; } }
        public byte[] SymbRecordSeqArcOffset { get { return symbRecordSeqArcOffset; } }
        public byte[] SymbRecordBankOffset { get { return symbRecordBankOffset; } }
        public byte[] SymbRecordWaveArcOffset { get { return symbRecordWaveArcOffset; } }
        public byte[] SymbRecordPlayerOffset { get { return symbRecordPlayerOffset; } }
        public byte[] SymbRecordGroupOffset { get { return symbRecordGroupOffset; } }
        public byte[] SymbRecordPlayer2Offset { get { return symbRecordPlayer2Offset; } }
        public byte[] SymbRecordStrmOffset { get { return symbRecordStrmOffset; } }

        public string[] SymbSeqFileNames { get { return getSymbFileNames(symbSeqFileNames); } }

        public string[] SymbBankFileNames { get { return getSymbFileNames(symbBankFileNames); } }
        public string[] SymbWaveArcFileNames { get { return getSymbFileNames(symbWaveArcFileNames); } }
        public string[] SymbPlayerFileNames { get { return getSymbFileNames(symbPlayerFileNames); } }
        public string[] SymbGroupFileNames { get { return getSymbFileNames(symbGroupFileNames); } }
        public string[] SymbPlayer2FileNames { get { return getSymbFileNames(symbPlayer2FileNames); } }
        public string[] SymbStrmFileNames { get { return getSymbFileNames(symbStrmFileNames); } }

        private string[] getSymbFileNames(byte[][] pFileNames)
        {
            System.Text.Encoding enc = System.Text.Encoding.ASCII;

            int titleCount = pFileNames.GetLength(0);
            string[] values = new string[pFileNames.GetLength(0)];

            for (int i = 0; i < titleCount; i++)
            {
                values[i] = enc.GetString(pFileNames[i]);
            }

            return values;
        }

        ////////////
        // METHODS
        ////////////
        public byte[] getStdHeaderSignature(Stream pStream, int pSectionOffset)
        {
            return ParseFile.parseSimpleOffset(pStream, pSectionOffset + STD_HEADER_SIGNATURE_OFFSET, 
                STD_HEADER_SIGNATURE_LENGTH);
        }
        public byte[] getStdHeaderSectionSize(Stream pStream, int pSectionOffset)
        {
            return ParseFile.parseSimpleOffset(pStream, pSectionOffset + STD_HEADER_SECTION_SIZE_OFFSET,
                STD_HEADER_SECTION_SIZE_LENGTH);
        }

        public byte[] getSymbRecordSeqOffset(Stream pStream, int pSectionOffset)
        {
            return ParseFile.parseSimpleOffset(pStream, pSectionOffset + SYMB_RECORD_SEQ_OFFSET_OFFSET,
                SYMB_RECORD_SEQ_OFFSET_LENGTH);
        }
        public byte[] getSymbRecordSeqArcOffset(Stream pStream, int pSectionOffset)
        {
            return ParseFile.parseSimpleOffset(pStream, pSectionOffset + SYMB_RECORD_SEQARC_OFFSET_OFFSET,
                SYMB_RECORD_SEQARC_OFFSET_LENGTH);
        }
        public byte[] getSymbRecordBankOffset(Stream pStream, int pSectionOffset)
        {
            return ParseFile.parseSimpleOffset(pStream, pSectionOffset + SYMB_RECORD_BANK_OFFSET_OFFSET,
                SYMB_RECORD_BANK_OFFSET_LENGTH);
        }
        public byte[] getSymbRecordWaveArcOffset(Stream pStream, int pSectionOffset)
        {
            return ParseFile.parseSimpleOffset(pStream, pSectionOffset + SYMB_RECORD_WAVEARC_OFFSET_OFFSET,
                SYMB_RECORD_WAVEARC_OFFSET_LENGTH);
        }
        public byte[] getSymbRecordPlayerOffset(Stream pStream, int pSectionOffset)
        {
            return ParseFile.parseSimpleOffset(pStream, pSectionOffset + SYMB_RECORD_PLAYER_OFFSET_OFFSET,
                SYMB_RECORD_PLAYER_OFFSET_LENGTH);
        }
        public byte[] getSymbRecordGroupOffset(Stream pStream, int pSectionOffset)
        {
            return ParseFile.parseSimpleOffset(pStream, pSectionOffset + SYMB_RECORD_GROUP_OFFSET_OFFSET,
                SYMB_RECORD_GROUP_OFFSET_LENGTH);
        }
        public byte[] getSymbRecordPlayer2Offset(Stream pStream, int pSectionOffset)
        {
            return ParseFile.parseSimpleOffset(pStream, pSectionOffset + SYMB_RECORD_PLAYER2_OFFSET_OFFSET,
                SYMB_RECORD_PLAYER2_OFFSET_LENGTH);
        }
        public byte[] getSymbRecordStrmOffset(Stream pStream, int pSectionOffset)
        {
            return ParseFile.parseSimpleOffset(pStream, pSectionOffset + SYMB_RECORD_STRM_OFFSET_OFFSET,
                SYMB_RECORD_STRM_OFFSET_LENGTH);
        }

        public void getSymbSubRecords(Stream pStream, int pSectionOffset, int pSubSectionOffset,
            ref byte[][] pSymbSubRecords, ref byte[][] pSymbFileNames)
        {
            byte[] subRecordCountBytes = ParseFile.parseSimpleOffset(pStream,
                pSectionOffset + pSubSectionOffset + SYMB_ENTRY_NUM_FILES_OFFSET,
                SYMB_ENTRY_NUM_FILES_LENGTH);
            int subRecordCount = BitConverter.ToInt32(subRecordCountBytes, 0);

            pSymbSubRecords = new byte[subRecordCount][];
            pSymbFileNames = new byte[subRecordCount][];

            for (int i = 1; i <= subRecordCount; i++)
            {
                pSymbSubRecords[i - 1] = ParseFile.parseSimpleOffset(pStream,
                    pSectionOffset + pSubSectionOffset + SYMB_ENTRY_NUM_FILES_OFFSET + (SYMB_ENTRY_FILE_NAME_SIZE * i),
                    SYMB_ENTRY_NUM_FILES_LENGTH);

                int fileOffset = BitConverter.ToInt32(pSymbSubRecords[i - 1], 0);
                int fileLength = ParseFile.getSegmentLength(pStream, pSectionOffset + fileOffset, NULL_BYTE_ARRAY);

                pSymbFileNames[i - 1] = ParseFile.parseSimpleOffset(pStream, pSectionOffset + fileOffset, fileLength);
            }
        }

        public void Initialize(Stream pStream, int pSectionOffset)
        {
            int intSymbRecordOffset;

            stdHeaderSignature = getStdHeaderSignature(pStream, pSectionOffset);
            stdHeaderSectionSize = getStdHeaderSectionSize(pStream, pSectionOffset);

            // SEQ
            symbRecordSeqOffset = getSymbRecordSeqOffset(pStream, pSectionOffset);
            intSymbRecordOffset = BitConverter.ToInt32(symbRecordSeqOffset, 0);
            getSymbSubRecords(pStream, pSectionOffset, intSymbRecordOffset,
                ref symbSeqSubRecords, ref symbSeqFileNames);

            // @TODO: Get subrecords for each section
            symbRecordSeqArcOffset = getSymbRecordSeqArcOffset(pStream, pSectionOffset);

            // BANK
            symbRecordBankOffset = getSymbRecordBankOffset(pStream, pSectionOffset);
            intSymbRecordOffset = BitConverter.ToInt32(symbRecordBankOffset, 0);
            getSymbSubRecords(pStream, pSectionOffset, intSymbRecordOffset,
                ref symbBankSubRecords, ref symbBankFileNames);
            

            // WAVEARC
            symbRecordWaveArcOffset = getSymbRecordWaveArcOffset(pStream, pSectionOffset);
            intSymbRecordOffset = BitConverter.ToInt32(symbRecordWaveArcOffset, 0);
            getSymbSubRecords(pStream, pSectionOffset, intSymbRecordOffset,
                ref symbWaveArcSubRecords, ref symbWaveArcFileNames);
            
            // PLAYER
            symbRecordPlayerOffset = getSymbRecordPlayerOffset(pStream, pSectionOffset);
            intSymbRecordOffset = BitConverter.ToInt32(symbRecordPlayerOffset, 0);
            getSymbSubRecords(pStream, pSectionOffset, intSymbRecordOffset,
                ref symbPlayerSubRecords, ref symbPlayerFileNames);
            
            // GROUP
            symbRecordGroupOffset = getSymbRecordGroupOffset(pStream, pSectionOffset);
            intSymbRecordOffset = BitConverter.ToInt32(symbRecordGroupOffset, 0);
            getSymbSubRecords(pStream, pSectionOffset, intSymbRecordOffset,
                ref symbGroupSubRecords, ref symbGroupFileNames);
            
            // PLAYER2
            symbRecordPlayer2Offset = getSymbRecordPlayer2Offset(pStream, pSectionOffset);
            intSymbRecordOffset = BitConverter.ToInt32(symbRecordPlayer2Offset, 0);
            getSymbSubRecords(pStream, pSectionOffset, intSymbRecordOffset,
                ref symbPlayer2SubRecords, ref symbPlayer2FileNames);
            
            // STRM
            symbRecordStrmOffset = getSymbRecordStrmOffset(pStream, pSectionOffset);
            intSymbRecordOffset = BitConverter.ToInt32(symbRecordStrmOffset, 0);
            getSymbSubRecords(pStream, pSectionOffset, intSymbRecordOffset,
                ref symbStrmSubRecords, ref symbStrmFileNames);
        }

    }
    
    class SdatInfoSection 
    {
        // section header
        public const int STD_HEADER_SIGNATURE_OFFSET = 0x00;
        public const int STD_HEADER_SIGNATURE_LENGTH = 4;

        public const int STD_HEADER_SECTION_SIZE_OFFSET = 0x04;
        public const int STD_HEADER_SECTION_SIZE_LENGTH = 4;
    
        // offsets, releative to this section
        public const int INFO_RECORD_SEQ_OFFSET_OFFSET = 0x08;
        public const int INFO_RECORD_SEQ_OFFSET_LENGTH = 4;

        public const int INFO_RECORD_SEQARC_OFFSET_OFFSET = 0x0C;
        public const int INFO_RECORD_SEQARC_OFFSET_LENGTH = 4;

        public const int INFO_RECORD_BANK_OFFSET_OFFSET = 0x10;
        public const int INFO_RECORD_BANK_OFFSET_LENGTH = 4;

        public const int INFO_RECORD_WAVEARC_OFFSET_OFFSET = 0x14;
        public const int INFO_RECORD_WAVEARC_OFFSET_LENGTH = 4;

        public const int INFO_RECORD_PLAYER_OFFSET_OFFSET = 0x18;
        public const int INFO_RECORD_PLAYER_OFFSET_LENGTH = 4;

        public const int INFO_RECORD_GROUP_OFFSET_OFFSET = 0x1C;
        public const int INFO_RECORD_GROUP_OFFSET_LENGTH = 4;

        // conflict between specs here!
        // http://tahaxan.arcnor.com/index.php?option=com_content&task=view&id=38&Itemid=36
        // http://kiwi.ds.googlepages.com/sdat.html
        public const int INFO_RECORD_PLAYER2_OFFSET_OFFSET = 0x20;
        public const int INFO_RECORD_PLAYER2_OFFSET_LENGTH = 4;
        // conflict between specs here!
        public const int INFO_RECORD_STRM_OFFSET_OFFSET = 0x24;
        public const int INFO_RECORD_STRM_OFFSET_LENGTH = 4;

        public const int INFO_RECORD_UNUSED_OFFSET = 0x28;
        public const int INFO_RECORD_UNUSED_LENGTH = 24;

        //////////////////////////////////////
        // INFO block entries, multiple types
        //////////////////////////////////////
        
        // SEQ
        public const int INFO_ENTRY_SEQ_FILEID_OFFSET = 0x00;
        public const int INFO_ENTRY_SEQ_FILEID_LENGTH = 2;

        public const int INFO_ENTRY_SEQ_UNKNOWN_OFFSET = 0x02;
        public const int INFO_ENTRY_SEQ_UNKNOWN_LENGTH = 2;

        public const int INFO_ENTRY_SEQ_BANKID_OFFSET = 0x04;
        public const int INFO_ENTRY_SEQ_BANKID_LENGTH = 2;

        public const int INFO_ENTRY_SEQ_VOL_OFFSET = 0x06;
        public const int INFO_ENTRY_SEQ_VOL_LENGTH = 1;

        public const int INFO_ENTRY_SEQ_CPR_OFFSET = 0x07;
        public const int INFO_ENTRY_SEQ_CPR_LENGTH = 1;

        public const int INFO_ENTRY_SEQ_PPR_OFFSET = 0x08;
        public const int INFO_ENTRY_SEQ_PPR_LENGTH = 1;

        public const int INFO_ENTRY_SEQ_PLY_OFFSET = 0x09;
        public const int INFO_ENTRY_SEQ_PLY_LENGTH = 1;

        public const int INFO_ENTRY_SEQ_UNKNOWN2_OFFSET = 0x0A;
        public const int INFO_ENTRY_SEQ_UNKNOWN2_LENGTH = 2;

        // SEQARC
        public const int INFO_ENTRY_SEQARC_FILEID_OFFSET = 0x00;
        public const int INFO_ENTRY_SEQARC_FILEID_LENGTH = 2;

        public const int INFO_ENTRY_SEQARC_UNKNOWN_OFFSET = 0x02;
        public const int INFO_ENTRY_SEQARC_UNKNOWN_LENGTH = 2;

        // BANK
        public const int INFO_ENTRY_BANK_FILEID_OFFSET = 0x00;
        public const int INFO_ENTRY_BANK_FILEID_LENGTH = 2;

        public const int INFO_ENTRY_BANK_UNKNOWN_OFFSET = 0x02;
        public const int INFO_ENTRY_BANK_UNKNOWN_LENGTH = 2;

        public const int INFO_ENTRY_BANK_WAVEARCID1_OFFSET = 0x04;
        public const int INFO_ENTRY_BANK_WAVEARCID1_LENGTH = 2;

        public const int INFO_ENTRY_BANK_WAVEARCID2_OFFSET = 0x06;
        public const int INFO_ENTRY_BANK_WAVEARCID2_LENGTH = 2;

        public const int INFO_ENTRY_BANK_WAVEARCID3_OFFSET = 0x08;
        public const int INFO_ENTRY_BANK_WAVEARCID3_LENGTH = 2;

        public const int INFO_ENTRY_BANK_WAVEARCID4_OFFSET = 0x0A;
        public const int INFO_ENTRY_BANK_WAVEARCID5_LENGTH = 2;

        // WAVARC
        public const int INFO_ENTRY_WAVARC_FILEID_OFFSET = 0x00;
        public const int INFO_ENTRY_WAVARC_FILEID_LENGTH = 2;

        public const int INFO_ENTRY_WAVARC_UNKNOWN_OFFSET = 0x02;
        public const int INFO_ENTRY_WAVARC_UNKNOWN_LENGTH = 2;

        // PLAYER
        public const int INFO_ENTRY_PLAYER_UNKNOWN_OFFSET = 0x00;
        public const int INFO_ENTRY_PLAYER_UNKNOWN_LENGTH = 1;

        public const int INFO_ENTRY_PLAYER_PADDING_OFFSET = 0x01;
        public const int INFO_ENTRY_PLAYER_PADDING_LENGTH = 3;

        public const int INFO_ENTRY_PLAYER_UNKNOWN2_OFFSET = 0x04;
        public const int INFO_ENTRY_PLAYER_UNKNOWN2_LENGTH = 4;

        // GROUP
        public const int INFO_ENTRY_GROUP_COUNT_OFFSET = 0x00;
        public const int INFO_ENTRY_GROUP_COUNT_LENGTH = 4;

        // GROUP - ENTRY
        public const int INFO_ENTRY_GROUP_ENTRY_TYPE_OFFSET = 0x00;
        public const int INFO_ENTRY_GROUP_ENTRY_TYPE_LENGTH = 4;

        public const int INFO_ENTRY_GROUP_ENTRY_ENTRYNUM_OFFSET = 0x04;
        public const int INFO_ENTRY_GROUP_ENTRY_ENTRYNUM_LENGTH = 4;

        // PLAYER2
        public const int INFO_ENTRY_PLAYER2_COUNT_OFFSET = 0x00;
        public const int INFO_ENTRY_PLAYER2_COUNT_LENGTH = 1;

        public const int INFO_ENTRY_PLAYER2_V_OFFSET = 0x01;
        public const int INFO_ENTRY_PLAYER2_V_LENGTH = 128;

        public const int INFO_ENTRY_PLAYER2_RESERVED_OFFSET = 0x81;
        public const int INFO_ENTRY_PLAYER2_RESERVED_LENGTH = 7;

        // STRM
        public const int INFO_ENTRY_STRM_FILEID_OFFSET = 0x00;
        public const int INFO_ENTRY_STRM_FILEID_LENGTH = 2;

        public const int INFO_ENTRY_STRM_UNKNOWN_OFFSET = 0x02;
        public const int INFO_ENTRY_STRM_UNKNOWN_LENGTH = 2;

        public const int INFO_ENTRY_STRM_VOL_OFFSET = 0x04;
        public const int INFO_ENTRY_STRM_VOL_LENGTH = 1;

        public const int INFO_ENTRY_STRM_PRI_OFFSET = 0x05;
        public const int INFO_ENTRY_STRM_PRI_LENGTH = 1;

        public const int INFO_ENTRY_STRM_PLY_OFFSET = 0x06;
        public const int INFO_ENTRY_STRM_PLY_LENGTH = 1;

        public const int INFO_ENTRY_STRM_RESERVED_OFFSET = 0x07;
        public const int INFO_ENTRY_STRM_RESERVED_LENGTH = 5;

        /////////////
        // Variables
        /////////////        

        // private
        private byte[] stdHeaderSignature;
        private byte[] stdHeaderSectionSize;

        private byte[] infoRecordSeqOffset;
        private byte[] infoRecordSeqArcOffset;
        private byte[] infoRecordBankOffset;
        private byte[] infoRecordWaveArcOffset;
        private byte[] infoRecordPlayerOffset;
        private byte[] infoRecordGroupOffset;
        private byte[] infoRecordPlayer2Offset;
        private byte[] infoRecordStrmOffset;

        // public
        public byte[] StdHeaderSignature { get { return stdHeaderSignature; } }
        public byte[] StdHeaderSectionSize { get { return stdHeaderSectionSize; } }

        ////////////
        // METHODS
        ////////////
        public byte[] getStdHeaderSignature(Stream pStream, int pSectionOffset)
        {
            return ParseFile.parseSimpleOffset(pStream, pSectionOffset + STD_HEADER_SIGNATURE_OFFSET,
                STD_HEADER_SIGNATURE_LENGTH);
        }
        public byte[] getStdHeaderSectionSize(Stream pStream, int pSectionOffset)
        {
            return ParseFile.parseSimpleOffset(pStream, pSectionOffset + STD_HEADER_SECTION_SIZE_OFFSET,
                STD_HEADER_SECTION_SIZE_LENGTH);
        }

        
        
        public void Initialize(Stream pStream, int pSectionOffset)
        {
            stdHeaderSignature = getStdHeaderSignature(pStream, pSectionOffset);
            stdHeaderSectionSize = getStdHeaderSectionSize(pStream, pSectionOffset);
        }
    }

    class SdatFatSection 
    {
        // Section Header
        public const int STD_HEADER_SIGNATURE_OFFSET = 0x00;
        public const int STD_HEADER_SIGNATURE_LENGTH = 4;

        public const int STD_HEADER_SECTION_SIZE_OFFSET = 0x04;
        public const int STD_HEADER_SECTION_SIZE_LENGTH = 4;

        public const int FAT_HEADER_NUMBER_OF_FILES_OFFSET = 0x08;
        public const int FAT_HEADER_NUMBER_OF_FILES_LENGTH = 4;

        // FAT Record - Offsets are relative
        public const int FAT_RECORD_FILE_OFFSET_OFFSET = 0x00;
        public const int FAT_RECORD_FILE_OFFSET_LENGTH = 4;

        public const int FAT_RECORD_FILE_SIZE_OFFSET = 0x04;
        public const int FAT_RECORD_FILE_SIZE_LENGTH = 4;

        public const int FAT_RECORD_RESERVED_OFFSET = 0x04;
        public const int FAT_RECORD_RESERVED_LENGTH = 4;

    }

    class SdatFileSection 
    {
        public const int FILE_HEADER_SIGNATURE_OFFSET = 0x00;
        public const int FILE_HEADER_SIGNATURE_LENGTH = 4;

        public const int FILE_HEADER_SECTION_SIZE_OFFSET = 0x04;
        public const int FILE_HEADER_SECTION_SIZE_LENGTH = 4;

        public const int FILE_HEADER_NUMBER_OF_FILES_OFFSET = 0x08;
        public const int FILE_HEADER_NUMBER_OF_FILES_LENGTH = 4;

        public const int FILE_HEADER_RESERVED_OFFSET = 0x0C;
        public const int FILE_HEADER_RESERVED_LENGTH = 4;

        #region VARIABLES

        private byte[] fileHeaderSignature;
        private byte[] fileHeaderSectionSize;
        private byte[] fileHeaderNumberOfFiles;
        private byte[] fileHeaderReservedSection;

        public byte[] FileHeaderSignature { get { return fileHeaderSignature; } }
        public byte[] FileHeaderSectionSize { get { return fileHeaderSectionSize; } }
        public byte[] FileHeaderNumberOfFiles { get { return fileHeaderNumberOfFiles; } }
        public byte[] FileHeaderReservedSection { get { return fileHeaderReservedSection; } }

        #endregion

        #region METHODS

        public void Initialize(Stream pStream, int pSectionOffset)
        {
            fileHeaderSignature = ParseFile.parseSimpleOffset(pStream, pSectionOffset + FILE_HEADER_SIGNATURE_OFFSET,
                FILE_HEADER_SIGNATURE_LENGTH);
            fileHeaderSectionSize = ParseFile.parseSimpleOffset(pStream, pSectionOffset + FILE_HEADER_SECTION_SIZE_OFFSET,
                FILE_HEADER_SECTION_SIZE_LENGTH);
            fileHeaderNumberOfFiles = ParseFile.parseSimpleOffset(pStream, pSectionOffset + FILE_HEADER_NUMBER_OF_FILES_OFFSET,
                FILE_HEADER_NUMBER_OF_FILES_LENGTH);
            fileHeaderReservedSection = ParseFile.parseSimpleOffset(pStream, pSectionOffset + FILE_HEADER_RESERVED_OFFSET,
                FILE_HEADER_RESERVED_LENGTH);
        }

        #endregion
    }
}
