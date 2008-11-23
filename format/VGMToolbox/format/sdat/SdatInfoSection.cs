using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using VGMToolbox.util;
using VGMToolbox.util.ObjectPooling;

namespace VGMToolbox.format.sdat
{
    public class SdatInfoSection
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

        // INFO Record Structure Offsets
        public const int INFO_RECORD_STRUCT_COUNT_OFFSET = 0x00;
        public const int INFO_RECORD_STRUCT_COUNT_LENGTH = 4;

        public const int INFO_RECORD_STRUCT_OFFSETS_OFFSET = 0x04;
        public const int INFO_RECORD_STRUCT_OFFSETS_LENGTH = 4;

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

        public struct SdatInfoRec
        {
            public byte[] nCount;
            public byte[][] nEntryOffsets;
        }

        public struct SdatInfoSseq
        {
            public byte[] fileId;
            public byte[] unknown;
            public byte[] bnk;
            public byte[] vol;
            public byte[] cpr;
            public byte[] ppr;
            public byte[] ply;
            public byte[] unknown2;
        }

        public struct SdatInfoStrm
        {
            public byte[] fileId;
            public byte[] unknown;
            public byte[] vol;
            public byte[] pri;
            public byte[] ply;
            public byte[] reserved;
        }

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

        SdatInfoRec seqInfoRec;
        SdatInfoRec seqArcInfoRec;
        SdatInfoRec bankInfoRec;
        SdatInfoRec waveArcInfoRec;
        SdatInfoRec playerInfoRec;
        SdatInfoRec groupInfoRec;
        SdatInfoRec player2InfoRec;
        SdatInfoRec strmInfoRec;

        SdatInfoSseq[] sdatInfoSseqs;
        SdatInfoStrm[] sdatInfoStrms;

        // public
        public byte[] StdHeaderSignature { get { return stdHeaderSignature; } }
        public byte[] StdHeaderSectionSize { get { return stdHeaderSectionSize; } }

        public SdatInfoSseq[] SdatInfoSseqs { get { return sdatInfoSseqs; } }
        public SdatInfoStrm[] SdatInfoStrms { get { return sdatInfoStrms; } }

        ////////////
        // METHODS
        ////////////

        private SdatInfoRec getInfoRec(Stream pStream, int pSectionOffset,
            int pInfoRecOffset)
        {
            int entryOffsetCount;
            SdatInfoRec sdatInfoRec = new SdatInfoRec();

            sdatInfoRec.nCount = ParseFile.parseSimpleOffset(pStream,
                pSectionOffset + pInfoRecOffset + INFO_RECORD_STRUCT_COUNT_OFFSET,
                INFO_RECORD_STRUCT_COUNT_LENGTH);

            entryOffsetCount = BitConverter.ToInt32(sdatInfoRec.nCount, 0);
            sdatInfoRec.nEntryOffsets = new byte[entryOffsetCount][];

            for (int i = 1; i <= entryOffsetCount; i++)
            {
                sdatInfoRec.nEntryOffsets[i - 1] = ParseFile.parseSimpleOffset(pStream,
                    pSectionOffset + pInfoRecOffset + (INFO_RECORD_STRUCT_OFFSETS_OFFSET * i),
                    INFO_RECORD_STRUCT_OFFSETS_LENGTH);
            }

            return sdatInfoRec;
        }

        private SdatInfoSseq[] getInfoSseqEntries(Stream pStream, int pSectionOffset,
            SdatInfoRec pSdatInfoRec)
        {
            int entryCount = BitConverter.ToInt32(pSdatInfoRec.nCount, 0);
            SdatInfoSseq[] ret = new SdatInfoSseq[entryCount];

            for (int i = 0; i < entryCount; i++)
            {
                ret[i] = new SdatInfoSseq();
                int infoOffset = BitConverter.ToInt32(pSdatInfoRec.nEntryOffsets[i], 0);

                if (infoOffset > 0)
                {                    

                    ret[i].fileId = ParseFile.parseSimpleOffset(pStream,
                        pSectionOffset + infoOffset +
                        INFO_ENTRY_SEQ_FILEID_OFFSET, INFO_ENTRY_SEQ_FILEID_LENGTH);
                    ret[i].unknown = ParseFile.parseSimpleOffset(pStream,
                        pSectionOffset + infoOffset +
                        INFO_ENTRY_SEQ_UNKNOWN_OFFSET, INFO_ENTRY_SEQ_UNKNOWN_LENGTH);
                    ret[i].bnk = ParseFile.parseSimpleOffset(pStream,
                        pSectionOffset + infoOffset +
                        INFO_ENTRY_SEQ_BANKID_OFFSET, INFO_ENTRY_SEQ_BANKID_LENGTH);
                    ret[i].vol = ParseFile.parseSimpleOffset(pStream,
                        pSectionOffset + infoOffset +
                        INFO_ENTRY_SEQ_VOL_OFFSET, INFO_ENTRY_SEQ_VOL_LENGTH);
                    ret[i].cpr = ParseFile.parseSimpleOffset(pStream,
                        pSectionOffset + infoOffset +
                        INFO_ENTRY_SEQ_CPR_OFFSET, INFO_ENTRY_SEQ_CPR_LENGTH);
                    ret[i].ppr = ParseFile.parseSimpleOffset(pStream,
                        pSectionOffset + infoOffset +
                        INFO_ENTRY_SEQ_PPR_OFFSET, INFO_ENTRY_SEQ_PPR_LENGTH);
                    ret[i].ply = ParseFile.parseSimpleOffset(pStream,
                        pSectionOffset + infoOffset +
                        INFO_ENTRY_SEQ_PLY_OFFSET, INFO_ENTRY_SEQ_PLY_LENGTH);
                    ret[i].unknown2 = ParseFile.parseSimpleOffset(pStream,
                        pSectionOffset + infoOffset +
                        INFO_ENTRY_SEQ_UNKNOWN2_OFFSET, INFO_ENTRY_SEQ_UNKNOWN2_LENGTH);
                }
            }

            return ret;
        }

        private SdatInfoStrm[] getInfoStrmEntries(Stream pStream, int pSectionOffset,
            SdatInfoRec pSdatInfoRec)
        {
            int entryCount = BitConverter.ToInt32(pSdatInfoRec.nCount, 0);
            SdatInfoStrm[] ret = new SdatInfoStrm[entryCount];

            for (int i = 0; i < entryCount; i++)
            {
                ret[i] = new SdatInfoStrm();

                int infoOffset = BitConverter.ToInt32(pSdatInfoRec.nEntryOffsets[i], 0);

                if (infoOffset > 0)
                {
                    ret[i].fileId = ParseFile.parseSimpleOffset(pStream,
                        pSectionOffset + infoOffset +
                        INFO_ENTRY_STRM_FILEID_OFFSET, INFO_ENTRY_STRM_FILEID_LENGTH);
                    ret[i].unknown = ParseFile.parseSimpleOffset(pStream,
                        pSectionOffset + infoOffset +
                        INFO_ENTRY_STRM_UNKNOWN_OFFSET, INFO_ENTRY_STRM_UNKNOWN_LENGTH);
                    ret[i].vol = ParseFile.parseSimpleOffset(pStream,
                        pSectionOffset + infoOffset +
                        INFO_ENTRY_STRM_VOL_OFFSET, INFO_ENTRY_STRM_VOL_LENGTH);
                    ret[i].pri = ParseFile.parseSimpleOffset(pStream,
                        pSectionOffset + infoOffset +
                        INFO_ENTRY_STRM_PRI_OFFSET, INFO_ENTRY_STRM_PRI_LENGTH);
                    ret[i].ply = ParseFile.parseSimpleOffset(pStream,
                        pSectionOffset + infoOffset +
                        INFO_ENTRY_STRM_PLY_OFFSET, INFO_ENTRY_STRM_PLY_LENGTH);
                    ret[i].reserved = ParseFile.parseSimpleOffset(pStream,
                        pSectionOffset + infoOffset +
                        INFO_ENTRY_STRM_RESERVED_OFFSET, INFO_ENTRY_STRM_RESERVED_LENGTH);
                }
            }

            return ret;
        }

        public void Initialize(Stream pStream, int pSectionOffset)
        {
            // Header Info
            stdHeaderSignature = ParseFile.parseSimpleOffset(pStream, pSectionOffset + STD_HEADER_SIGNATURE_OFFSET,
                STD_HEADER_SIGNATURE_LENGTH);
            stdHeaderSectionSize = ParseFile.parseSimpleOffset(pStream, pSectionOffset + STD_HEADER_SECTION_SIZE_OFFSET,
                STD_HEADER_SECTION_SIZE_LENGTH);

            // SEQ
            infoRecordSeqOffset = ParseFile.parseSimpleOffset(pStream, pSectionOffset + INFO_RECORD_SEQ_OFFSET_OFFSET,
                INFO_RECORD_SEQ_OFFSET_LENGTH);
            seqInfoRec = getInfoRec(pStream, pSectionOffset, BitConverter.ToInt32(infoRecordSeqOffset, 0));
            sdatInfoSseqs = getInfoSseqEntries(pStream, pSectionOffset, seqInfoRec);

            // SEQARC
            infoRecordSeqArcOffset = ParseFile.parseSimpleOffset(pStream, pSectionOffset + INFO_RECORD_SEQARC_OFFSET_OFFSET,
                INFO_RECORD_SEQARC_OFFSET_LENGTH);
            seqArcInfoRec = getInfoRec(pStream, pSectionOffset, BitConverter.ToInt32(infoRecordSeqArcOffset, 0));

            // BANK
            infoRecordBankOffset = ParseFile.parseSimpleOffset(pStream, pSectionOffset + INFO_RECORD_BANK_OFFSET_OFFSET,
                INFO_RECORD_BANK_OFFSET_LENGTH);
            bankInfoRec = getInfoRec(pStream, pSectionOffset, BitConverter.ToInt32(infoRecordBankOffset, 0));

            // WAVEARC
            infoRecordWaveArcOffset = ParseFile.parseSimpleOffset(pStream, pSectionOffset + INFO_RECORD_WAVEARC_OFFSET_OFFSET,
                INFO_RECORD_WAVEARC_OFFSET_LENGTH);
            waveArcInfoRec = getInfoRec(pStream, pSectionOffset, BitConverter.ToInt32(infoRecordWaveArcOffset, 0));

            // PLAYER
            infoRecordPlayerOffset = ParseFile.parseSimpleOffset(pStream, pSectionOffset + INFO_RECORD_PLAYER_OFFSET_OFFSET,
                INFO_RECORD_PLAYER_OFFSET_LENGTH);
            playerInfoRec = getInfoRec(pStream, pSectionOffset, BitConverter.ToInt32(infoRecordPlayerOffset, 0));

            // GROUP
            infoRecordGroupOffset = ParseFile.parseSimpleOffset(pStream, pSectionOffset + INFO_RECORD_GROUP_OFFSET_OFFSET,
                INFO_RECORD_GROUP_OFFSET_LENGTH);
            groupInfoRec = getInfoRec(pStream, pSectionOffset, BitConverter.ToInt32(infoRecordGroupOffset, 0));

            // PLAYER2
            infoRecordPlayer2Offset = ParseFile.parseSimpleOffset(pStream, pSectionOffset + INFO_RECORD_PLAYER2_OFFSET_OFFSET,
                INFO_RECORD_PLAYER2_OFFSET_LENGTH);
            player2InfoRec = getInfoRec(pStream, pSectionOffset, BitConverter.ToInt32(infoRecordPlayer2Offset, 0));

            // STRM
            infoRecordStrmOffset = ParseFile.parseSimpleOffset(pStream, pSectionOffset + INFO_RECORD_STRM_OFFSET_OFFSET,
                INFO_RECORD_STRM_OFFSET_LENGTH);
            strmInfoRec = getInfoRec(pStream, pSectionOffset, BitConverter.ToInt32(infoRecordStrmOffset, 0));
            sdatInfoStrms = getInfoStrmEntries(pStream, pSectionOffset, strmInfoRec);
        }
    }    
}
