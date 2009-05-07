using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ICSharpCode.SharpZipLib.Checksums;

using VGMToolbox.format;
using VGMToolbox.format.util;
using VGMToolbox.util;

namespace format.VGMToolbox.format
{
    class Hes : IHootFormat
    {
        public static readonly byte[] ASCII_SIGNATURE = new byte[] { 0x48, 0x45, 0x53, 0x4D }; // HESM
        private const string FORMAT_ABBREVIATION = "HES";

        private const string HOOT_DRIVER_ALIAS = "TurboGrafx-16";
        private const string HOOT_DRIVER_TYPE = "hes";
        private const string HOOT_DRIVER = "pcengine";
        private const string HOOT_CHIP = "Hudson WSG";

        private const int SIG_OFFSET = 0x00;
        private const int SIG_LENGTH = 0x04;

        private const int VERSION_OFFSET = 0x04;
        private const int VERSION_LENGTH = 0x01;

        private const int STARTING_SONG_OFFSET = 0x05;
        private const int STARTING_SONG_LENGTH = 0x01;

        private const int REQUEST_ADDRESS_OFFSET = 0x06;
        private const int REQUEST_ADDRESS_LENGTH = 0x02;

        private const int INITIAL_MPR0_OFFSET = 0x08;
        private const int INITIAL_MPR0_LENGTH = 0x01;

        private const int INITIAL_MPR1_OFFSET = 0x09;
        private const int INITIAL_MPR1_LENGTH = 0x01;

        private const int INITIAL_MPR2_OFFSET = 0x0A;
        private const int INITIAL_MPR2_LENGTH = 0x01;

        private const int INITIAL_MPR3_OFFSET = 0x0B;
        private const int INITIAL_MPR3_LENGTH = 0x01;

        private const int INITIAL_MPR4_OFFSET = 0x0C;
        private const int INITIAL_MPR4_LENGTH = 0x01;

        private const int INITIAL_MPR5_OFFSET = 0x0D;
        private const int INITIAL_MPR5_LENGTH = 0x01;

        private const int INITIAL_MPR6_OFFSET = 0x0E;
        private const int INITIAL_MPR6_LENGTH = 0x01;

        private const int INITIAL_MPR7_OFFSET = 0x0F;
        private const int INITIAL_MPR7_LENGTH = 0x01;

        private const int DATA_SIG_OFFSET = 0x10;
        private const int DATA_SIG_LENGTH = 0x04;

        private const int DATA_SIZE_OFFSET = 0x14;
        private const int DATA_SIZE_LENGTH = 0x04;

        private const int LOAD_ADDRESS_OFFSET = 0x18;
        private const int LOAD_ADDRESS_LENGTH = 0x04;

        private const int RESERVE_OFFSET = 0x1C;
        private const int RESERVE_LENGTH = 0x04;

        private const int DATA_OFFSET = 0x20;

        private string filePath;
        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }

        private byte[] asciiSignature;
        private byte[] versionNumber;
        private byte[] startingSong;
        private byte[] requestAddress;
        private byte[] initialMpr0;
        private byte[] initialMpr1;
        private byte[] initialMpr2;
        private byte[] initialMpr3;
        private byte[] initialMpr4;
        private byte[] initialMpr5;
        private byte[] initialMpr6;
        private byte[] initialMpr7;

        private byte[] dataSig;
        private byte[] dataSize;
        private byte[] loadAddress;
        private byte[] reserve;
        private byte[] data;

        Dictionary<string, string> tagHash = new Dictionary<string, string>();

        public byte[] AsciiSignature { get { return this.asciiSignature; } }
        public byte[] VersionNumber { get { return this.versionNumber; } }
        public byte[] StartingSong { get { return this.startingSong; } }
        public byte[] RequestAddress { get { return this.requestAddress; } }
        public byte[] InitialMpr0 { get { return this.initialMpr0; } }
        public byte[] InitialMpr1 { get { return this.initialMpr1; } }
        public byte[] InitialMpr2 { get { return this.initialMpr2; } }
        public byte[] InitialMpr3 { get { return this.initialMpr3; } }
        public byte[] InitialMpr4 { get { return this.initialMpr4; } }
        public byte[] InitialMpr5 { get { return this.initialMpr5; } }
        public byte[] InitialMpr6 { get { return this.initialMpr6; } }
        public byte[] InitialMpr7 { get { return this.initialMpr7; } }

        public byte[] DataSig { get { return this.dataSig; } }
        public byte[] DataSize { get { return this.dataSize; } }
        public byte[] LoadAddress { get { return this.loadAddress; } }
        public byte[] Reserve { get { return this.reserve; } }
        public byte[] Data { get { return this.data; } }

        public void Initialize(Stream pStream, string pFilePath)
        {
            this.filePath = pFilePath;
            
            this.asciiSignature = ParseFile.parseSimpleOffset(pStream, SIG_OFFSET, SIG_LENGTH);
            this.versionNumber = ParseFile.parseSimpleOffset(pStream, VERSION_OFFSET, VERSION_LENGTH);
            this.startingSong = ParseFile.parseSimpleOffset(pStream, STARTING_SONG_OFFSET, STARTING_SONG_LENGTH);
            this.requestAddress = ParseFile.parseSimpleOffset(pStream, REQUEST_ADDRESS_OFFSET, REQUEST_ADDRESS_LENGTH);
            this.initialMpr0 = ParseFile.parseSimpleOffset(pStream, INITIAL_MPR0_OFFSET, INITIAL_MPR0_LENGTH);
            this.initialMpr1 = ParseFile.parseSimpleOffset(pStream, INITIAL_MPR1_OFFSET, INITIAL_MPR1_LENGTH);
            this.initialMpr2 = ParseFile.parseSimpleOffset(pStream, INITIAL_MPR2_OFFSET, INITIAL_MPR2_LENGTH);
            this.initialMpr3 = ParseFile.parseSimpleOffset(pStream, INITIAL_MPR3_OFFSET, INITIAL_MPR3_LENGTH);
            this.initialMpr4 = ParseFile.parseSimpleOffset(pStream, INITIAL_MPR4_OFFSET, INITIAL_MPR4_LENGTH);
            this.initialMpr5 = ParseFile.parseSimpleOffset(pStream, INITIAL_MPR5_OFFSET, INITIAL_MPR5_LENGTH);
            this.initialMpr6 = ParseFile.parseSimpleOffset(pStream, INITIAL_MPR6_OFFSET, INITIAL_MPR6_LENGTH);
            this.initialMpr7 = ParseFile.parseSimpleOffset(pStream, INITIAL_MPR7_OFFSET, INITIAL_MPR7_LENGTH);

            this.dataSig = ParseFile.parseSimpleOffset(pStream, DATA_SIG_OFFSET, DATA_SIG_LENGTH);
            this.dataSize = ParseFile.parseSimpleOffset(pStream, DATA_SIZE_OFFSET, DATA_SIZE_LENGTH);                                                           
            this.loadAddress = ParseFile.parseSimpleOffset(pStream, LOAD_ADDRESS_OFFSET, LOAD_ADDRESS_LENGTH);
            this.reserve = ParseFile.parseSimpleOffset(pStream, RESERVE_OFFSET, RESERVE_LENGTH);
            
            int dataLength = BitConverter.ToInt32(this.dataSize, 0);
            this.data = ParseFile.parseSimpleOffset(pStream, DATA_OFFSET, dataLength);            

            this.initializeTagHash();
        }

        public void GetDatFileCrc32(ref Crc32 pChecksum)
        {
            pChecksum.Reset();

            pChecksum.Update(versionNumber);
            pChecksum.Update(startingSong);
            pChecksum.Update(requestAddress);

            pChecksum.Update(initialMpr0);
            pChecksum.Update(initialMpr1);
            pChecksum.Update(initialMpr2);
            pChecksum.Update(initialMpr3);
            pChecksum.Update(initialMpr4);
            pChecksum.Update(initialMpr5);
            pChecksum.Update(initialMpr6);
            pChecksum.Update(initialMpr7);

            pChecksum.Update(loadAddress);
            pChecksum.Update(reserve);
            pChecksum.Update(data);
        }

        private void initializeTagHash()
        {
            System.Text.Encoding enc = System.Text.Encoding.ASCII;
            
            tagHash.Add("Starting Song", this.startingSong[0].ToString());
            tagHash.Add("Data Size", BitConverter.ToInt32(this.dataSize, 0).ToString());
            tagHash.Add("Load Address", BitConverter.ToInt32(this.loadAddress, 0).ToString());
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

        public bool IsFileLibrary() { return false; }

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

        #region HOOT

        public int GetStartingSong() 
        {
            return this.startingSong[0];
        }
        public int GetTotalSongs()
        {
            NezPlugM3uEntry[] entries = this.GetPlaylistEntries();
            return entries.Length;
        }
        public string GetSongName()
        {
            return null;
        }

        public string GetHootDriverAlias() { return HOOT_DRIVER_ALIAS; }
        public string GetHootDriverType() { return HOOT_DRIVER_TYPE; }
        public string GetHootDriver() { return HOOT_DRIVER; }
        public string GetHootChips() { return HOOT_CHIP; }

        public bool UsesPlaylist()
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
