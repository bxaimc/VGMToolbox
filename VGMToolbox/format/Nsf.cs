using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ICSharpCode.SharpZipLib.Checksums;

using VGMToolbox.util;
using VGMToolbox.util.ObjectPooling;

namespace VGMToolbox.format
{
    class Nsf : IFormat
    {
        private static readonly byte[] ASCII_SIGNATURE = new byte[] { 0x4E, 0x45, 0x53, 0x4D, 0x1A };
        private const string FORMAT_ABBREVIATION = "NSF";

        private static readonly byte MASK_VRC6  = 1;
        private static readonly byte MASK_VRC7  = 2;
        private static readonly byte MASK_FDS   = 4;
        private static readonly byte MASK_MMC5  = 8;
        private static readonly byte MASK_N106  = 16;
        private static readonly byte MASK_FME07 = 32;

        private const string CHIP_VRC6  = "[VRC6]";
        private const string CHIP_VRC7  = "[VRC7]";
        private const string CHIP_FDS   = "[FDS Sound]";
        private const string CHIP_MMC5  = "[MMC5]";
        private const string CHIP_N106  = "[NAMCO106]";
        private const string CHIP_FME07 = "[Sunsoft FME-07]";

        private const int SIG_OFFSET = 0x00;
        private const int SIG_LENGTH = 0x05;

        private const int VERSION_OFFSET = 0x05;
        private const int VERSION_LENGTH = 0x01;

        private const int TOTAL_SONGS_OFFSET = 0x06;
        private const int TOTAL_SONGS_LENGTH = 0x01;

        private const int STARTING_SONG_OFFSET = 0x07;
        private const int STARTING_SONG_LENGTH = 0x01;

        private const int LOAD_ADDRESS_OFFSET = 0x08;
        private const int LOAD_ADDRESS_LENGTH = 0x02;

        private const int INIT_ADDRESS_OFFSET = 0x0A;
        private const int INIT_ADDRESS_LENGTH = 0x02;

        private const int PLAY_ADDRESS_OFFSET = 0x0C;
        private const int PLAY_ADDRESS_LENGTH = 0x02;

        private const int NAME_OFFSET = 0x0E;
        private const int NAME_LENGTH = 0x20;

        private const int ARTIST_OFFSET = 0x2E;
        private const int ARTIST_LENGTH = 0x20;

        private const int COPYRIGHT_OFFSET = 0x4E;
        private const int COPYRIGHT_LENGTH = 0x20;

        private const int SPEED_NTSC_OFFSET = 0x6E;
        private const int SPEED_NTSC_LENGTH = 0x02;

        private const int BANKSWITCH_INIT_OFFSET = 0x70;
        private const int BANKSWITCH_INIT_LENGTH = 0x08;

        private const int SPEED_PAL_OFFSET = 0x78;
        private const int SPEED_PAL_LENGTH = 0x02;

        private const int PAL_NTSC_BITS_OFFSET = 0x7A;
        private const int PAL_NTSC_BITS_LENGTH = 0x01;

        private const int EXTRA_SOUND_BITS_OFFSET = 0x7B;
        private const int EXTRA_SOUND_BITS_LENGTH = 0x01;

        private const int FUTURE_USE_OFFSET = 0x7C;
        private const int FUTURE_USE_LENGTH = 0x04;

        private const int DATA_OFFSET = 0x80;

        private byte[] asciiSignature;
        private byte[] versionNumber;
        private byte[] totalSongs;
        private byte[] startingSong;
        private byte[] loadAddress;
        private byte[] initAddress;
        private byte[] playAddress;
        private byte[] songName;
        private byte[] songArtist;
        private byte[] songCopyright;
        private byte[] ntscSpeed;
        private byte[] bankSwitchInit;
        private byte[] palSpeed;
        private byte[] palNtscBits;
        private byte[] extraChipsBits;
        private byte[] futureExpansion;
        private byte[] data;

        Dictionary<string, string> tagHash = new Dictionary<string, string>();

        public byte[] AsciiSignature { get { return this.asciiSignature; } }
        public byte[] VersionNumber { get { return this.versionNumber; } }
        public byte[] TotalSongs { get { return this.totalSongs; } }
        public byte[] StartingSong { get { return this.startingSong; } }
        public byte[] LoadAddress { get { return this.loadAddress; } }
        public byte[] InitAddress { get { return this.initAddress; } }
        public byte[] PlayAddress { get { return this.playAddress; } }
        public byte[] SongName { get { return this.songName; } }
        public byte[] SongArtist { get { return this.songArtist; } }
        public byte[] SongCopyright { get { return this.songCopyright; } }
        public byte[] NtscSpeed { get { return this.ntscSpeed; } }
        public byte[] BankSwitchInit { get { return this.bankSwitchInit; } }
        public byte[] PalSpeed { get { return this.palSpeed; } }
        public byte[] PalNtscBits { get { return this.palNtscBits; } }
        public byte[] ExtraChipsBits { get { return this.extraChipsBits; } }
        public byte[] FutureExpansion { get { return this.futureExpansion; } }
        public byte[] Data { get { return this.data; } }


        #region STREAM BASED METHODS
        public byte[] getAsciiSignature(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, SIG_OFFSET, SIG_LENGTH);
        }

        public byte[] getVersionNumber(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, VERSION_OFFSET, VERSION_LENGTH);
        }

        public byte[] getTotalSongs(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, TOTAL_SONGS_OFFSET, TOTAL_SONGS_LENGTH);
        }

        public byte[] getStartingSong(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, STARTING_SONG_OFFSET, STARTING_SONG_LENGTH);
        }

        public byte[] getLoadAddress(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, LOAD_ADDRESS_OFFSET, LOAD_ADDRESS_LENGTH);
        }

        public byte[] getInitAddress(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, INIT_ADDRESS_OFFSET, INIT_ADDRESS_LENGTH);
        }

        public byte[] getPlayAddress(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, PLAY_ADDRESS_OFFSET, PLAY_ADDRESS_LENGTH);
        }

        public byte[] getSongName(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, NAME_OFFSET, NAME_LENGTH);
        }

        public byte[] getSongArtist(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, ARTIST_OFFSET, ARTIST_LENGTH);
        }

        public byte[] getSongCopyright(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, COPYRIGHT_OFFSET, COPYRIGHT_LENGTH);
        }

        public byte[] getNtscSpeed(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, SPEED_NTSC_OFFSET, SPEED_NTSC_LENGTH);
        }

        public byte[] getBankSwitchInit(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, BANKSWITCH_INIT_OFFSET, BANKSWITCH_INIT_LENGTH);
        }

        public byte[] getPalSpeed(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, SPEED_PAL_OFFSET, SPEED_PAL_LENGTH);
        }

        public byte[] getPalNtscBits(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, PAL_NTSC_BITS_OFFSET, PAL_NTSC_BITS_LENGTH);
        }

        public byte[] getExtraChipsBits(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, EXTRA_SOUND_BITS_OFFSET, EXTRA_SOUND_BITS_LENGTH);
        }

        public byte[] getFutureExpansion(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, FUTURE_USE_OFFSET, FUTURE_USE_LENGTH);
        }

        public byte[] getData(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, DATA_OFFSET, (int) (pStream.Length - DATA_OFFSET));
        }

        public void Initialize(Stream pStream)
        {
            this.asciiSignature = this.getAsciiSignature(pStream);
            this.versionNumber = this.getVersionNumber(pStream);
            this.totalSongs = this.getTotalSongs(pStream);
            this.startingSong = this.getStartingSong(pStream);
            this.loadAddress = this.getLoadAddress(pStream);
            this.initAddress = this.getInitAddress(pStream);
            this.playAddress = this.getPlayAddress(pStream);
            this.songName = this.getSongName(pStream);
            this.songArtist = this.getSongArtist(pStream);
            this.songCopyright = this.getSongCopyright(pStream);
            this.ntscSpeed = this.getNtscSpeed(pStream);
            this.bankSwitchInit = this.getBankSwitchInit(pStream);
            this.palSpeed = this.getPalSpeed(pStream);
            this.palNtscBits = this.getPalNtscBits(pStream);
            this.extraChipsBits = this.getExtraChipsBits(pStream);
            this.futureExpansion = this.getFutureExpansion(pStream);
            this.data = this.getData(pStream);

            this.initializeTagHash();
        }
        #endregion

        #region BYTE[] BASED METHODS
        public byte[] getAsciiSignature(byte[] pBytes)
        {
            return ParseFile.parseSimpleOffset(pBytes, SIG_OFFSET, SIG_LENGTH);
        }

        public byte[] getVersionNumber(byte[] pBytes)
        {
            return ParseFile.parseSimpleOffset(pBytes, VERSION_OFFSET, VERSION_LENGTH);
        }

        public byte[] getTotalSongs(byte[] pBytes)
        {
            return ParseFile.parseSimpleOffset(pBytes, TOTAL_SONGS_OFFSET, TOTAL_SONGS_LENGTH);
        }

        public byte[] getStartingSong(byte[] pBytes)
        {
            return ParseFile.parseSimpleOffset(pBytes, STARTING_SONG_OFFSET, STARTING_SONG_LENGTH);
        }

        public byte[] getLoadAddress(byte[] pBytes)
        {
            return ParseFile.parseSimpleOffset(pBytes, LOAD_ADDRESS_OFFSET, LOAD_ADDRESS_LENGTH);
        }

        public byte[] getInitAddress(byte[] pBytes)
        {
            return ParseFile.parseSimpleOffset(pBytes, INIT_ADDRESS_OFFSET, INIT_ADDRESS_LENGTH);
        }

        public byte[] getPlayAddress(byte[] pBytes)
        {
            return ParseFile.parseSimpleOffset(pBytes, PLAY_ADDRESS_OFFSET, PLAY_ADDRESS_LENGTH);
        }

        public byte[] getSongName(byte[] pBytes)
        {
            return ParseFile.parseSimpleOffset(pBytes, NAME_OFFSET, NAME_LENGTH);
        }

        public byte[] getSongArtist(byte[] pBytes)
        {
            return ParseFile.parseSimpleOffset(pBytes, ARTIST_OFFSET, ARTIST_LENGTH);
        }

        public byte[] getSongCopyright(byte[] pBytes)
        {
            return ParseFile.parseSimpleOffset(pBytes, COPYRIGHT_OFFSET, COPYRIGHT_LENGTH);
        }

        public byte[] getNtscSpeed(byte[] pBytes)
        {
            return ParseFile.parseSimpleOffset(pBytes, SPEED_NTSC_OFFSET, SPEED_NTSC_LENGTH);
        }

        public byte[] getBankSwitchInit(byte[] pBytes)
        {
            return ParseFile.parseSimpleOffset(pBytes, BANKSWITCH_INIT_OFFSET, BANKSWITCH_INIT_LENGTH);
        }

        public byte[] getPalSpeed(byte[] pBytes)
        {
            return ParseFile.parseSimpleOffset(pBytes, SPEED_PAL_OFFSET, SPEED_PAL_LENGTH);
        }

        public byte[] getPalNtscBits(byte[] pBytes)
        {
            return ParseFile.parseSimpleOffset(pBytes, PAL_NTSC_BITS_OFFSET, PAL_NTSC_BITS_LENGTH);
        }

        public byte[] getExtraChipsBits(byte[] pBytes)
        {
            return ParseFile.parseSimpleOffset(pBytes, EXTRA_SOUND_BITS_OFFSET, EXTRA_SOUND_BITS_LENGTH);
        }

        public byte[] getFutureExpansion(byte[] pBytes)
        {
            return ParseFile.parseSimpleOffset(pBytes, FUTURE_USE_OFFSET, FUTURE_USE_LENGTH);
        }

        public byte[] getData(byte[] pBytes)
        {
            return ParseFile.parseSimpleOffset(pBytes, DATA_OFFSET, (int)(pBytes.Length - DATA_OFFSET));
        }

        public void initialize(byte[] pBytes)
        {
            this.asciiSignature = this.getAsciiSignature(pBytes);
            this.versionNumber = this.getVersionNumber(pBytes);
            this.totalSongs = this.getTotalSongs(pBytes);
            this.startingSong = this.getStartingSong(pBytes);
            this.loadAddress = this.getLoadAddress(pBytes);
            this.initAddress = this.getInitAddress(pBytes);
            this.playAddress = this.getPlayAddress(pBytes);
            this.songName = this.getSongName(pBytes);
            this.songArtist = this.getSongArtist(pBytes);
            this.songCopyright = this.getSongCopyright(pBytes);
            this.ntscSpeed = this.getNtscSpeed(pBytes);
            this.bankSwitchInit = this.getBankSwitchInit(pBytes);
            this.palSpeed = this.getPalSpeed(pBytes);
            this.palNtscBits = this.getPalNtscBits(pBytes);
            this.extraChipsBits = this.getExtraChipsBits(pBytes);
            this.futureExpansion = this.getFutureExpansion(pBytes);
            this.data = this.getData(pBytes);

            this.initializeTagHash();
        }                
        #endregion

        private void initializeTagHash()
        {
            System.Text.Encoding enc = System.Text.Encoding.ASCII;
            
            tagHash.Add("Name", enc.GetString(this.songName));
            tagHash.Add("Artist", enc.GetString(this.songArtist));
            tagHash.Add("Copyright", enc.GetString(this.songCopyright));
            tagHash.Add("Total Songs", this.totalSongs[0].ToString());
            tagHash.Add("Starting Song", this.startingSong[0].ToString());
            tagHash.Add("Extra Chips", getExtraChipsTag());            
        }
        
        public void GetDatFileCrc32(string pPath, ref Dictionary<string, ByteArray> pLibHash,
            ref Crc32 pChecksum, bool pUseLibHash)
        {
            pChecksum.Reset();    
        
            pChecksum.Update(versionNumber);
            pChecksum.Update(totalSongs);
            pChecksum.Update(startingSong);
            pChecksum.Update(loadAddress);
            pChecksum.Update(initAddress);
            pChecksum.Update(playAddress);
            pChecksum.Update(ntscSpeed);
            pChecksum.Update(bankSwitchInit);
            pChecksum.Update(palSpeed);
            pChecksum.Update(palNtscBits);
            pChecksum.Update(extraChipsBits);
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
        
        public bool IsFileLibrary(string pPath)
        {
            return false;
        }

        public Dictionary<string, string> GetTagHash()
        {
            return this.tagHash;
        }

        private string getExtraChipsTag()
        { 
            string _ret = String.Empty;
            
            if ((this.extraChipsBits[0] & MASK_VRC6) == MASK_VRC6) 
            {
                _ret += CHIP_VRC6;
            }
            if ((this.extraChipsBits[0] & MASK_VRC7) == MASK_VRC7)
            {
                _ret += CHIP_VRC7;
            }
            if ((this.extraChipsBits[0] & MASK_FDS) == MASK_FDS)
            {
                _ret += CHIP_FDS;
            }
            if ((this.extraChipsBits[0] & MASK_MMC5) == MASK_MMC5)
            {
                _ret += CHIP_MMC5;
            }
            if ((this.extraChipsBits[0] & MASK_N106) == MASK_N106)
            {
                _ret += CHIP_N106;
            }
            if ((this.extraChipsBits[0] & MASK_FME07) == MASK_FME07)
            {
                _ret += CHIP_FME07;
            }
            
            return _ret;
        }
    }
}
