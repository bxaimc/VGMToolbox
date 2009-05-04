using System.Collections.Generic;
using System.IO;

using ICSharpCode.SharpZipLib.Checksums;

using VGMToolbox.format.util;
using VGMToolbox.util;

namespace VGMToolbox.format
{
    public class Kss : IFormat, IHootFormat
    {
        private static readonly byte[] ASCII_SIGNATURE = new byte[] { 0x4B, 0x53, 0x43, 0x43 }; // KSCC
        private const string FORMAT_ABBREVIATION = "KSS";

        private const string HOOT_DRIVER_ALIAS = "MSX";
        private const string HOOT_DRIVER_TYPE = "kss";
        private const string HOOT_DRIVER = "msx";

        private const int SIG_OFFSET = 0x00;
        private const int SIG_LENGTH = 0x04;

        private const int Z80_LOAD_OFFSET = 0x04;
        private const int Z80_LOAD_LENGTH = 0x02;

        private const int INIT_DATA_LENGTH_OFFSET = 0x06;
        private const int INIT_DATA_LENGTH_LENGTH = 0x02;

        private const int Z80_INIT_OFFSET = 0x08;
        private const int Z80_INIT_LENGTH = 0x02;

        private const int Z80_PLAY_OFFSET = 0x0A;
        private const int Z80_PLAY_LENGTH = 0x02;

        private const int BANK_START_NO_OFFSET = 0x0C;
        private const int BANK_START_NO_LENGTH = 0x01;

        private const int BANKED_EXTRA_DATA_OFFSET = 0x0D;
        private const int BANKED_EXTRA_DATA_LENGTH = 0x01;

        private const int RESERVED_OFFSET = 0x0E;
        private const int RESERVED_LENGTH = 0x01;

        private const int EXTRA_CHIP_OFFSET = 0x0F;
        private const int EXTRA_CHIP_LENGTH = 0x01;

        private const int DATA_OFFSET = 0x10;

        private string filePath;
        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }

        private byte[] asciiSignature;
        private byte[] z80Load;
        private byte[] initDataLength;
        private byte[] z80Init;
        private byte[] z80Play;
        private byte[] bankStartNo;
        private byte[] bankedExtraData;
        private byte[] reserved;
        private byte[] extraChips;
        private byte[] data;

        Dictionary<string, string> tagHash = new Dictionary<string, string>();

        public byte[] AsciiSignature { get { return this.asciiSignature; } }
        public byte[] Z80Load { get { return this.z80Load; } }
        public byte[] InitDataLength { get { return this.initDataLength; } }
        public byte[] Z80Init { get { return this.z80Init; } }
        public byte[] Z80Play { get { return this.z80Play; } }
        public byte[] BankStartNo { get { return this.bankStartNo; } }
        public byte[] BankedExtraData { get { return this.bankedExtraData; } }
        public byte[] Reserved { get { return this.reserved; } }
        public byte[] ExtraChips { get { return this.extraChips; } }
        public byte[] Data { get { return this.data; } }

        #region METHODS

        public byte[] getAsciiSignature(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, SIG_OFFSET, SIG_LENGTH);
        }

        public byte[] getZ80Load(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, Z80_LOAD_OFFSET, Z80_LOAD_LENGTH);
        }

        public byte[] getInitDataLength(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, INIT_DATA_LENGTH_OFFSET, INIT_DATA_LENGTH_LENGTH);
        }

        public byte[] getZ80Init(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, Z80_INIT_OFFSET, Z80_INIT_LENGTH);
        }

        public byte[] getZ80Play(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, Z80_PLAY_OFFSET, Z80_PLAY_LENGTH);
        }

        public byte[] getBankStartNo(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, BANK_START_NO_OFFSET, BANK_START_NO_LENGTH);
        }

        public byte[] getBankedExtraData(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, BANKED_EXTRA_DATA_OFFSET, BANKED_EXTRA_DATA_LENGTH);
        }

        public byte[] getReserved(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, RESERVED_OFFSET, RESERVED_LENGTH);
        }

        public byte[] getExtraChips(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, EXTRA_CHIP_OFFSET, EXTRA_CHIP_LENGTH);
        }

        public byte[] getData(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, DATA_OFFSET, (int)(pStream.Length - DATA_OFFSET));
        }

        public void Initialize(Stream pStream, string pFilePath)
        {
            this.filePath = pFilePath;
            this.asciiSignature = this.getAsciiSignature(pStream);
            this.z80Load = this.getZ80Load(pStream);
            this.initDataLength = this.getInitDataLength(pStream);
            this.z80Init = this.getZ80Init(pStream);
            this.z80Play = this.getZ80Play(pStream);
            this.bankStartNo = this.getBankStartNo(pStream);
            this.bankedExtraData = this.getBankedExtraData(pStream);
            this.reserved = this.getReserved(pStream);
            this.extraChips = this.getExtraChips(pStream);
            this.data = this.getData(pStream);

            this.initializeTagHash();
        }

        private void initializeTagHash()
        {
            System.Text.Encoding enc = System.Text.Encoding.ASCII;
            tagHash.Add("Extra Chips Bytes", this.extraChips[0].ToString());
        }

        public void GetDatFileCrc32(ref Crc32 pChecksum)
        {
            pChecksum.Reset();

            pChecksum.Update(z80Load);
            pChecksum.Update(initDataLength);
            pChecksum.Update(z80Init);
            pChecksum.Update(z80Play);
            pChecksum.Update(bankStartNo);
            pChecksum.Update(bankedExtraData);
            pChecksum.Update(extraChips);
            pChecksum.Update(data);
        }

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

        public bool IsFileLibrary() { return false;}

        public bool HasMultipleFileExtensions()
        {
            return false;
        }

        public Dictionary<string, string> GetTagHash()
        {
            return this.tagHash;
        }

        public bool UsesLibraries() { return false; }
        public bool IsLibraryPresent() { return true; }

        public int GetStartingSong() { return 0; }
        public int GetTotalSongs() { return 0; }
        public string GetSongName() { return null; }
        
        #endregion

        #region HOOT

        public string GetHootDriverAlias() { return HOOT_DRIVER_ALIAS; }
        public string GetHootDriverType() { return HOOT_DRIVER_TYPE; }
        public string GetHootDriver() { return HOOT_DRIVER; }

        public bool usesPlaylist()
        {
            return true;
        }
        public NezPlugM3uEntry[] GetPlaylistEntries()
        {
            NezPlugM3uEntry[] entries = null;

            string m3uFileName = Path.ChangeExtension(this.filePath, NezPlugUtil.M3U_FILE_EXTENSION);
            entries = NezPlugUtil.GetNezPlugM3uEntriesFromFile(m3uFileName);

            return entries;
        }

        #endregion
    }
}
