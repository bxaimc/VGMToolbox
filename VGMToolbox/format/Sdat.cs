using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using VGMToolbox.util;

namespace VGMToolbox.format
{
    class Sdat
    {
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


        private void initialize(Stream pStream)
        { 
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
        }


    }

    class SdatSymbSection 
    {
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
        // conflict between specs here!
        public const int SYMB_RECORD_STRM_OFFSET_OFFSET = 0x24;
        public const int SYMB_RECORD_STRM_OFFSET_LENGTH = 4;

        public const int SYMB_RECORD_UNUSED_OFFSET = 0x28;
        public const int SYMB_RECORD_UNUSED_LENGTH = 24;

        // THESE SHOULD EXIST FOR EACH SECTION
        public const int SYMB_ENTRY_NUM_FILES_OFFSET = 0x00;        // RELATIVE TO SECTION OFFSET
        public const int SYMB_ENTRY_NUM_FILES_LENGTH = 4;

        public const int SYMB_ENTRY_FILE_NAMES_BEGIN_OFFSET = 0x04; // RELATIVE TO SECTION OFFSET

        // Add stuff here for subrecords for SEQARC
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
    }
}
