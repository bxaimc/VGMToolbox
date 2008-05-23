﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using ICSharpCode.SharpZipLib.Checksums;

using VGMToolbox.util;
using VGMToolbox.util.ObjectPooling;

namespace VGMToolbox.format
{
    class Vgm : IFormat
    {
        public Vgm() { }
        
        // Constants
        private static readonly byte[] ASCII_SIGNATURE = new byte[] { 0x56, 0x67, 0x6d, 0x20 }; // "Vgm "

        private const string TYPE_VGM = "VGM";
        private const string TYPE_VGM7Z = "VGM7Z";
        private const string TYPE_VGZ = "VGZ";

        private static readonly byte[] VERSION_0100 = new byte[] { 0x00, 0x01, 0x00, 0x00 };
        private static readonly byte[] VERSION_0101 = new byte[] { 0x01, 0x01, 0x00, 0x00 };
        private static readonly byte[] VERSION_0110 = new byte[] { 0x10, 0x01, 0x00, 0x00 };
        private static readonly byte[] VERSION_0150 = new byte[] { 0x50, 0x01, 0x00, 0x00 };

        private static readonly int INT_VERSION_UNKNOWN = -1;
        private static readonly int INT_VERSION_0100 = 100;
        private static readonly int INT_VERSION_0101 = 101;
        private static readonly int INT_VERSION_0110 = 110;
        private static readonly int INT_VERSION_0150 = 150;

        // Version 1.0 or greater
        private const int SIG_OFFSET = 0x00;
        private const int SIG_LENGTH = 0x04;

        private const int EOF_OFFSET_OFFSET = 0x04;
        private const int EOF_OFFSET_LENGTH = 0x04;

        private const int VERSION_OFFSET = 0x08;
        private const int VERSION_LENGTH = 0x04;

        private const int SN76489_CLOCK_OFFSET = 0x0C;
        private const int SN76489_CLOCK_LENGTH = 0x04;

        private const int YM2413_CLOCK_OFFSET = 0x10;
        private const int YM2413_CLOCK_LENGTH = 0x04;

        private const int GD3_OFFSET_OFFSET = 0x14;
        private const int GD3_OFFSET_LENGTH = 0x04;

        private const int TOTAL_NUM_SAMPLES_OFFSET = 0x18;
        private const int TOTAL_NUM_SAMPLES_LENGTH = 0x04;

        private const int LOOP_OFFSET_OFFSET = 0x1C;
        private const int LOOP_OFFSET_LENGTH = 0x04;

        private const int LOOP_NUM_SAMPLES_OFFSET = 0x20;
        private const int LOOP_NUM_SAMPLES_LENGTH = 0x04;

        private const int VGM_DATA_OFFSET_PRE150 = 0x40;

        private byte[] signatureTag;
        private byte[] eofOffset;
        private byte[] version;
        private byte[] sn76489Clock;
        private byte[] ym2413Clock;
        private byte[] gd3Offset;
        private byte[] totalNumOfSamples;
        private byte[] loopOffset;
        private byte[] loopNumOfSamples;
        private byte[] vgmData;

        public byte[] SignatureTag { get { return this.signatureTag; } }
        public byte[] EofOffset { get { return this.eofOffset; } }
        public byte[] Version { get { return this.version; } }
        public byte[] Sn76489Clock { get { return this.sn76489Clock; } }
        public byte[] Ym2413Clock { get { return this.ym2413Clock; } }
        public byte[] Gd3Offset { get { return this.gd3Offset; } }
        public byte[] TotalNumOfSamples { get { return this.totalNumOfSamples; } }
        public byte[] LoopOffset { get { return this.loopOffset; } }
        public byte[] LoopNumOfSamples { get { return this.loopNumOfSamples; } }
        public byte[] VgmData { get { return this.vgmData; } }


        // Version 1.01 or greater
        private const int RATE_OFFSET = 0x24;
        private const int RATE_LENGTH = 0x04;

        private byte[] rate;

        public byte[] Rate { get { return this.rate; } }

        // Version 1.10 or greater
        private const int SN76489_FEEDBACK_OFFSET = 0x28;
        private const int SN76489_FEEDBACK_LENGTH = 0x02;

        private const int SN76489_SRW_OFFSET = 0x2A;
        private const int SN76489_SRW_LENGTH = 0x01;

        private const int RESERVED1_OFFSET = 0x2B;
        private const int RESERVED1_LENGTH = 0x01;

        private const int YM2612_CLOCK_OFFSET = 0x2C;
        private const int YM2612_CLOCK_LENGTH = 0x04;

        private const int YM2151_CLOCK_OFFSET = 0x30;
        private const int YM2151_CLOCK_LENGTH = 0x04;

        private byte[] sn76489Feedback;
        private byte[] sn76489Srw;
        private byte[] ym2612Clock;
        private byte[] ym2151Clock;

        public byte[] Sn76489Feedback { get { return this.sn76489Feedback; } }
        public byte[] Sn76489Srw { get { return this.sn76489Srw; } }
        public byte[] Ym2612Clock { get { return this.ym2612Clock; } }
        public byte[] Ym2151Clock { get { return this.ym2151Clock; } }

        // Version 1.50 or greater        
        private const int VGM_DATA_OFFSET_OFFSET = 0x34;
        private const int VGM_DATA_OFFSET_LENGTH = 0x04;

        private const int RESERVED2_OFFSET = 0x38;
        private const int RESERVED2_LENGTH = 0x08;

        private byte[] vgmDataOffset;

        public byte[] VgmDataOffset { get { return this.vgmDataOffset; } }

        // Others
        Dictionary<string, string> tagHash = new Dictionary<string, string>();
        private int vgmDataAbsoluteOffset;
        private int gd3AbsoluteOffset;
        private int eofAbsoluteOffset;

        // Methods        

        #region BYTE FUNCTIONS
        protected byte[] getEofOffset(byte[] pBytes)
        {
            return ParseFile.parseSimpleOffset(pBytes, EOF_OFFSET_OFFSET, EOF_OFFSET_LENGTH);
        }

        protected byte[] getGd3Offset(byte[] pBytes)
        {
            return ParseFile.parseSimpleOffset(pBytes, GD3_OFFSET_OFFSET, GD3_OFFSET_LENGTH);
        }

        protected byte[] getLoopNumOfSamples(byte[] pBytes)
        {
            return ParseFile.parseSimpleOffset(pBytes, LOOP_NUM_SAMPLES_OFFSET, LOOP_NUM_SAMPLES_LENGTH);
        }

        protected byte[] getLoopOffset(byte[] pBytes)
        {
            return ParseFile.parseSimpleOffset(pBytes, LOOP_OFFSET_OFFSET, LOOP_OFFSET_LENGTH);
        }

        /*
        private void getPackedType(FileStream pFileStream)
        {
            this.fileType = checkPackedType(pFileStream, FileUtil.GZIP_SIGNATURE, "VGZ");
            if (this.fileType == null)
            {
                this.fileType = checkPackedType(pFileStream, ASCII_SIGNATURE, "VGM");
            }
            if (this.fileType == null)
            {
                this.fileType = checkPackedType(pFileStream, FileUtil.SEVEN_ZIP_SIGNATURE, "VGM7Z");
            }
        }
        */

        protected byte[] getRate(byte[] pBytes)
        {
            return ParseFile.parseSimpleOffset(pBytes, RATE_OFFSET, RATE_LENGTH);
        }

        protected byte[] getSignatureTag(byte[] pBytes)
        {
            return ParseFile.parseSimpleOffset(pBytes, SIG_OFFSET, SIG_LENGTH);
        }

        protected byte[] getSn76489Clock(byte[] pBytes)
        {
            return ParseFile.parseSimpleOffset(pBytes, SN76489_CLOCK_OFFSET, SN76489_CLOCK_LENGTH);
        }

        protected byte[] getSn76489Feedback(byte[] pBytes)
        {
            return ParseFile.parseSimpleOffset(pBytes, SN76489_FEEDBACK_OFFSET, SN76489_FEEDBACK_LENGTH);
        }

        protected byte[] getSn76489Srw(byte[] pBytes)
        {
            return ParseFile.parseSimpleOffset(pBytes, SN76489_SRW_OFFSET, SN76489_SRW_LENGTH);
        }

        protected byte[] getTotalNumOfSamples(byte[] pBytes)
        {
            return ParseFile.parseSimpleOffset(pBytes, TOTAL_NUM_SAMPLES_OFFSET, TOTAL_NUM_SAMPLES_LENGTH);
        }

        protected byte[] getVersion(byte[] pBytes)
        {
            return ParseFile.parseSimpleOffset(pBytes, VERSION_OFFSET, VERSION_LENGTH);
        }

        protected byte[] getVgmData(byte[] pBytes)
        {
            // NEED TO FIX
            return ParseFile.parseSimpleOffset(pBytes, (VGM_DATA_OFFSET_OFFSET + BitConverter.ToInt32(this.getVgmDataOffset(pBytes), 0)), 4);
        }

        protected byte[] getVgmDataOffset(byte[] pBytes)
        {
            return ParseFile.parseSimpleOffset(pBytes, VGM_DATA_OFFSET_OFFSET, VGM_DATA_OFFSET_LENGTH);
        }

        protected byte[] getYm2151Clock(byte[] pBytes)
        {
            return ParseFile.parseSimpleOffset(pBytes, YM2151_CLOCK_OFFSET, YM2151_CLOCK_LENGTH);
        }

        protected byte[] getYm2413Clock(byte[] pBytes)
        {
            return ParseFile.parseSimpleOffset(pBytes, YM2413_CLOCK_OFFSET, YM2413_CLOCK_LENGTH);
        }

        protected byte[] getYm2612Clock(byte[] pBytes)
        {
            return ParseFile.parseSimpleOffset(pBytes, YM2612_CLOCK_OFFSET, YM2612_CLOCK_LENGTH);
        }

        /*
        public static bool isVgm(FileStream pFileStream)
        {
            string str = checkPackedType(pFileStream, FileUtil.GZIP_SIGNATURE, "VGZ");
            if (str == null)
            {
                str = checkPackedType(pFileStream, ASCII_SIGNATURE, "VGM");
            }
            if (str == null)
            {
                str = checkPackedType(pFileStream, FileUtil.SEVEN_ZIP_SIGNATURE, "VGM7Z");
            }
            if (str != null)
            {
                string str2 = str;
                if (str2 != null)
                {
                    byte[] buffer;
                    if (!(str2 == "VGM"))
                    {
                        if (str2 == "VGZ")
                        {
                            buffer = FileUtil.decompressGzip(FileUtil.getChunk(pFileStream, 0L, (int)pFileStream.Length));
                        }
                        else if (str2 == "VGM7Z")
                        {
                        }
                    }
                    else
                    {
                        buffer = FileUtil.getChunk(pFileStream, 0L, (int)pFileStream.Length);
                    }
                }
            }
            return (str != null);
        }
        */
#endregion

        #region STREAM FUNCTIONS
        protected byte[] getEofOffset(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, EOF_OFFSET_OFFSET, EOF_OFFSET_LENGTH);
        }

        protected byte[] getGd3Offset(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, GD3_OFFSET_OFFSET, GD3_OFFSET_LENGTH);
        }

        protected byte[] getLoopNumOfSamples(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, LOOP_NUM_SAMPLES_OFFSET, LOOP_NUM_SAMPLES_LENGTH);
        }

        protected byte[] getLoopOffset(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, LOOP_OFFSET_OFFSET, LOOP_OFFSET_LENGTH);
        }

        protected byte[] getRate(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, RATE_OFFSET, RATE_LENGTH);
        }

        protected byte[] getSignatureTag(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, SIG_OFFSET, SIG_LENGTH);
        }

        protected byte[] getSn76489Clock(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, SN76489_CLOCK_OFFSET, SN76489_CLOCK_LENGTH);
        }

        protected byte[] getSn76489Feedback(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, SN76489_FEEDBACK_OFFSET, SN76489_FEEDBACK_LENGTH);
        }

        protected byte[] getSn76489Srw(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, SN76489_SRW_OFFSET, SN76489_SRW_LENGTH);
        }

        protected byte[] getTotalNumOfSamples(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, TOTAL_NUM_SAMPLES_OFFSET, TOTAL_NUM_SAMPLES_LENGTH);
        }

        protected byte[] getVersion(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, VERSION_OFFSET, VERSION_LENGTH);
        }

        protected byte[] getVgmData(Stream pStream)
        {
            // NEED TO FIX
            return new byte[1];
        }

        protected byte[] getVgmDataOffset(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, VGM_DATA_OFFSET_OFFSET, VGM_DATA_OFFSET_LENGTH);
        }

        protected byte[] getYm2151Clock(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, YM2151_CLOCK_OFFSET, YM2151_CLOCK_LENGTH);
        }

        protected byte[] getYm2413Clock(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, YM2413_CLOCK_OFFSET, YM2413_CLOCK_LENGTH);
        }

        protected byte[] getYm2612Clock(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, YM2612_CLOCK_OFFSET, YM2612_CLOCK_LENGTH);
        }

        private int getIntVersion()
        {
            int ret = INT_VERSION_UNKNOWN;

            if (ParseFile.compareSegment(this.version, 0, VERSION_0100))
            {
                ret = INT_VERSION_0100;
            }
            else if (ParseFile.compareSegment(this.version, 0, VERSION_0101))
            {
                ret = INT_VERSION_0101;
            }
            else if (ParseFile.compareSegment(this.version, 0, VERSION_0110))
            {
                ret = INT_VERSION_0110;
            }
            else if (ParseFile.compareSegment(this.version, 0, VERSION_0150))
            {
                ret = INT_VERSION_0150;
            }

            return ret;
        }

        #endregion

        public byte[] getAsciiSignature()
        {
            return ASCII_SIGNATURE;
        }

        public string getFormatAbbreviation()
        {
            return "VGM";
        }

        public bool IsFileLibrary(string pPath)
        {
            return false;
        }

        public Dictionary<string, string> GetTagHash()
        {
            return this.tagHash;
        }
    
        public void initialize(Stream pStream)
        {
            int intVersion = INT_VERSION_UNKNOWN;
            
            // Version 1.0 or greater (all versions)
            this.signatureTag = this.getSignatureTag(pStream);
            this.eofOffset = this.getEofOffset(pStream);
            this.version = this.getVersion(pStream);
            this.sn76489Clock = this.getSn76489Clock(pStream);
            this.ym2413Clock = this.getYm2413Clock(pStream);
            this.gd3Offset = this.getGd3Offset(pStream);
            this.totalNumOfSamples = this.getTotalNumOfSamples(pStream);
            this.loopOffset = this.getLoopOffset(pStream);
            this.loopNumOfSamples = this.getLoopNumOfSamples(pStream);
            // vgmData; get only during checksum to save memory

            // Get version to determine if other tags are present
            intVersion = this.getIntVersion();

            
            if (intVersion >= INT_VERSION_0101)
            {
                // Version 1.01 or greater
                this.rate = this.getRate(pStream);

                if (intVersion >= INT_VERSION_0110)
                {
                    // Version 1.10 or greater
                    this.sn76489Feedback = this.getSn76489Feedback(pStream);
                    this.sn76489Srw = this.getSn76489Srw(pStream);
                    this.ym2612Clock = this.getYm2612Clock(pStream);
                    this.ym2151Clock = this.getYm2151Clock(pStream);

                    if (intVersion >= INT_VERSION_0150)
                    {
                        // Version 1.50 or greater
                        this.vgmDataOffset = this.getVgmDataOffset(pStream);
                    }
                }
            }

            if (intVersion < INT_VERSION_0150)
            {
                this.vgmDataOffset = BitConverter.GetBytes(VGM_DATA_OFFSET_PRE150);
            }

            this.getAbsoluteOffsets(intVersion);
        }

        private void getAbsoluteOffsets(int pIntVersion)
        {
            if (pIntVersion >= INT_VERSION_0150)
            {
                this.vgmDataAbsoluteOffset = BitConverter.ToInt32(this.vgmDataOffset, 0) + VGM_DATA_OFFSET_OFFSET;
            }
            else 
            {
                this.vgmDataAbsoluteOffset = VGM_DATA_OFFSET_PRE150;
            }

            this.gd3AbsoluteOffset = BitConverter.ToInt32(this.gd3Offset, 0) + GD3_OFFSET_OFFSET;
            this.eofAbsoluteOffset = BitConverter.ToInt32(this.eofOffset, 0) + EOF_OFFSET_OFFSET;
        }
        
        public void getDatFileCrc32(string pPath, ref Dictionary<string, ByteArray> pLibHash,
            ref Crc32 pChecksum, bool pUseLibHash)
        { 
        
        
        
        }
    }
}