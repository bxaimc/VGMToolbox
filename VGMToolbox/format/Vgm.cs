using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.GZip;

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

        // GD3 TAG
        private const int GD3_SIGNATURE_OFFSET = 0x00;
        private const int GD3_SIGNATURE_LENGTH = 0x04;
        
        private const int GD3_VERSION_OFFSET = 0x04;
        private const int GD3_VERSION_LENGTH = 0x04;
        
        private const int GD3_DATA_LENGTH_OFFSET = 0x08;
        private const int GD3_DATA_LENGTH_LENGTH = 0x04;

        private byte[] gd3Signature;
        private byte[] gd3Version;
        private byte[] gd3DataLength;

        private const int GD3_TRACK_NAME_E_IDX = 0;
        private const int GD3_TRACK_NAME_J_IDX = 1;
        private const int GD3_GAME_NAME_E_IDX = 2;
        private const int GD3_GAME_NAME_J_IDX = 3;
        private const int GD3_SYSTEM_NAME_E_IDX = 4;
        private const int GD3_SYSTEM_NAME_J_IDX = 5;
        private const int GD3_AUTHOR_NAME_E_IDX = 6;
        private const int GD3_AUTHOR_NAME_J_IDX = 7;
        private const int GD3_GAME_DATE_IDX = 8;
        private const int GD3_RIPPER_IDX = 9;
        private const int GD3_NOTES = 10;

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

            if (ParseFile.CompareSegment(this.version, 0, VERSION_0100))
            {
                ret = INT_VERSION_0100;
            }
            else if (ParseFile.CompareSegment(this.version, 0, VERSION_0101))
            {
                ret = INT_VERSION_0101;
            }
            else if (ParseFile.CompareSegment(this.version, 0, VERSION_0110))
            {
                ret = INT_VERSION_0110;
            }
            else if (ParseFile.CompareSegment(this.version, 0, VERSION_0150))
            {
                ret = INT_VERSION_0150;
            }

            return ret;
        }

        #endregion

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
            return "VGM";
        }

        public bool IsFileLibrary(string pPath)
        {
            return false;
        }

        public bool HasMultipleFileExtensions()
        {
            return true;
        }

        public Dictionary<string, string> GetTagHash()
        {
            return this.tagHash;
        }
    
        public void Initialize(Stream pStream)
        {
            if (FormatUtil.IsGzipFile(pStream))
            {
                GZipInputStream gZipInputStream = new GZipInputStream(pStream);
                string tempGzipFile = Path.GetTempFileName();
                FileStream gZipFileStream = new FileStream(tempGzipFile, FileMode.Open, FileAccess.ReadWrite);

                int size = 4096;
                byte[] writeData = new byte[size];
                while (true)
                {
                    size = gZipInputStream.Read(writeData, 0, size);
                    if (size > 0)
                    {
                        gZipFileStream.Write(writeData, 0, size);
                    }
                    else
                    {
                        break;
                    }
                }

                gZipFileStream.Seek(0, SeekOrigin.Begin);
                this.Initialize(gZipFileStream);

                gZipFileStream.Close();
                gZipFileStream.Dispose();
                File.Delete(tempGzipFile);
            }
            else
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
                this.initializeTagHash(pStream);

            } // if (FormatUtil.IsGzipFile(pFileStream))
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

            if (BitConverter.ToInt32(this.gd3Offset, 0) != 0)
            {
                this.gd3AbsoluteOffset = BitConverter.ToInt32(this.gd3Offset, 0) + GD3_OFFSET_OFFSET;
            }
            else
            {
                this.gd3AbsoluteOffset = 0;
            }
            
            this.eofAbsoluteOffset = BitConverter.ToInt32(this.eofOffset, 0) + EOF_OFFSET_OFFSET;
        }
        
        public void GetDatFileCrc32(string pPath, ref Dictionary<string, ByteArray> pLibHash,
            ref Crc32 pChecksum, bool pUseLibHash)
        {
            int intVersion = INT_VERSION_UNKNOWN;
            
            pChecksum.Reset();            

            // Version 1.0 or greater (all versions)
            pChecksum.Update(this.version);
            pChecksum.Update(this.sn76489Clock);
            pChecksum.Update(this.ym2413Clock);
            pChecksum.Update(this.totalNumOfSamples);
            pChecksum.Update(this.loopOffset);
            pChecksum.Update(this.loopNumOfSamples);            

            // Get version to determine if other tags are present
            intVersion = this.getIntVersion();

            if (intVersion >= INT_VERSION_0101)
            {
                // Version 1.01 or greater
                pChecksum.Update(this.rate);

                if (intVersion >= INT_VERSION_0110)
                {
                    // Version 1.10 or greater
                    pChecksum.Update(this.sn76489Feedback);
                    pChecksum.Update(this.sn76489Srw);
                    pChecksum.Update(this.ym2612Clock);
                    pChecksum.Update(this.ym2151Clock);
                }
            }

            // Add data
            FileStream fs = new FileStream(pPath, FileMode.Open, FileAccess.Read);
            this.addDataSection(fs, ref pChecksum);
            fs.Close();
            fs.Dispose();
        }

        private void addDataSection(Stream pFileStream, ref Crc32 pChecksum)
        {
            long currentPosition = pFileStream.Position;
            int bytesToRead = 0;

            // Determine count of bytes to read for data section
            if (this.gd3AbsoluteOffset > this.vgmDataAbsoluteOffset)
            {
                bytesToRead = this.gd3AbsoluteOffset - this.vgmDataAbsoluteOffset;
            }
            else
            {
                bytesToRead = this.eofAbsoluteOffset - this.vgmDataAbsoluteOffset;
            }

            if (FormatUtil.IsGzipFile(pFileStream))
            {
                GZipInputStream gZipInputStream = new GZipInputStream(pFileStream);
                string tempGzipFile = Path.GetTempFileName();
                FileStream gZipFileStream = new FileStream(tempGzipFile, FileMode.Open, FileAccess.ReadWrite);

                int size = 4096;
                byte[] writeData = new byte[size];
                while (true)
                {
                    size = gZipInputStream.Read(writeData, 0, size);
                    if (size > 0)
                    {
                        gZipFileStream.Write(writeData, 0, size);
                    }
                    else
                    {
                        break;
                    }
                }

                ParseFile.AddChunkToChecksum(gZipFileStream, this.vgmDataAbsoluteOffset, bytesToRead,
                    ref pChecksum);

                gZipFileStream.Close();
                gZipFileStream.Dispose();
                File.Delete(tempGzipFile);
            }
            else
            {
                ParseFile.AddChunkToChecksum(pFileStream, this.vgmDataAbsoluteOffset, bytesToRead,
                    ref pChecksum);
            }

            pFileStream.Position = currentPosition;
        }

        private void initializeTagHash(Stream pStream)
        {
            if (this.gd3AbsoluteOffset != 0)
            {
                this.gd3Signature = ParseFile.parseSimpleOffset(pStream,
                    this.gd3AbsoluteOffset + GD3_SIGNATURE_OFFSET, GD3_SIGNATURE_LENGTH);
                this.gd3Version = ParseFile.parseSimpleOffset(pStream,
                    this.gd3AbsoluteOffset + GD3_VERSION_OFFSET, GD3_VERSION_LENGTH);
                this.gd3DataLength = ParseFile.parseSimpleOffset(pStream,
                    this.gd3AbsoluteOffset + GD3_DATA_LENGTH_OFFSET, GD3_DATA_LENGTH_LENGTH);

                byte[] tagBytes = ParseFile.parseSimpleOffset(pStream,
                    this.gd3AbsoluteOffset + GD3_SIGNATURE_LENGTH + GD3_VERSION_LENGTH + GD3_DATA_LENGTH_LENGTH,
                    BitConverter.ToInt32(this.gd3DataLength, 0));

                System.Text.Encoding enc = System.Text.Encoding.Unicode;
                string tagsString = enc.GetString(tagBytes);

                char[] tagSeparator = new char[] { (char)(0x00), (char)(0x00) };
                string[] splitTags = tagsString.Trim().Split(tagSeparator);

                //try
                //{
                string versionTag = getIntVersion().ToString();
                this.tagHash.Add("VGM Format Version", versionTag);                
                
                this.tagHash.Add("Track Name (E)", splitTags[GD3_TRACK_NAME_E_IDX]);
                this.tagHash.Add("Track Name (J)", splitTags[GD3_TRACK_NAME_J_IDX]);
                this.tagHash.Add("Game Name (E)", splitTags[GD3_GAME_NAME_E_IDX]);
                this.tagHash.Add("Game Name (J)", splitTags[GD3_GAME_NAME_J_IDX]);
                this.tagHash.Add("System Name (E)", splitTags[GD3_SYSTEM_NAME_E_IDX]);
                this.tagHash.Add("System Name (J)", splitTags[GD3_SYSTEM_NAME_J_IDX]);
                this.tagHash.Add("Author Name (E)", splitTags[GD3_AUTHOR_NAME_E_IDX]);
                this.tagHash.Add("Author Name (J)", splitTags[GD3_AUTHOR_NAME_J_IDX]);
                this.tagHash.Add("Game Date", splitTags[GD3_GAME_DATE_IDX]);
                this.tagHash.Add("Set Ripper", splitTags[GD3_RIPPER_IDX]);
                this.tagHash.Add("Notes", splitTags[GD3_NOTES]);
                //}
                //catch (Exception) { }
            }
        }
    }
}