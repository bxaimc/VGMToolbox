using System;
using System.Collections.Generic;
using System.Text;

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

        public const int STD_HEADER_SECTION_SIZE_OFFSET = 0x08;
        public const int STD_HEADER_SECTION_SIZE_LENGTH = 4;

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
    }

    class Symb 
    {
        public const int STD_HEADER_SIGNATURE_OFFSET = 0x00;
        public const int STD_HEADER_SIGNATURE_LENGTH = 4;

        public const int STD_HEADER_SECTION_SIZE_OFFSET = 0x04;
        public const int STD_HEADER_SECTION_SIZE_LENGTH = 4;

        public const int SYMB_HEADER_SEQ_OFFSET_OFFSET = 0x08;
        public const int SYMB_HEADER_SEQ_OFFSET_LENGTH = 4;

        public const int SYMB_HEADER_SEQARC_OFFSET_OFFSET = 0x0C;
        public const int SYMB_HEADER_SEQARC_OFFSET_LENGTH = 4;

        public const int SYMB_HEADER_BANK_OFFSET_OFFSET = 0x10;
        public const int SYMB_HEADER_BANK_OFFSET_LENGTH = 4;

        public const int SYMB_HEADER_WAVEARC_OFFSET_OFFSET = 0x14;
        public const int SYMB_HEADER_WAVEARC_OFFSET_LENGTH = 4;

        public const int SYMB_HEADER_PLAYER_OFFSET_OFFSET = 0x18;
        public const int SYMB_HEADER_PLAYER_OFFSET_LENGTH = 4;

        public const int SYMB_HEADER_GROUP_OFFSET_OFFSET = 0x1C;
        public const int SYMB_HEADER_GROUP_OFFSET_LENGTH = 4;

        // conflict between specs here!
        // http://tahaxan.arcnor.com/index.php?option=com_content&task=view&id=38&Itemid=36
        // http://kiwi.ds.googlepages.com/sdat.html
        public const int SYMB_HEADER_PLAYER2_OFFSET_OFFSET = 0x20;
        public const int SYMB_HEADER_PLAYER2_OFFSET_LENGTH = 4;
        // conflict between specs here!
        public const int SYMB_HEADER_STRM_OFFSET_OFFSET = 0x24;
        public const int SYMB_HEADER_STRM_OFFSET_LENGTH = 4;

        public const int SYMB_HEADER_UNUSED_OFFSET = 0x28;
        public const int SYMB_HEADER_UNUSED_LENGTH = 24;
    }
}
