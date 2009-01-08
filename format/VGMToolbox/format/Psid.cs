using System;
using System.Collections.Generic;
using System.Text;

namespace format.VGMToolbox.format.sdat
{
    class Psid
    {
        public static readonly byte[] ASCII_SIGNATURE = new byte[] { 0x50, 0x53, 0x49, 0x44 }; // PSID

        private const int SIG_OFFSET = 0x00;
        private const int SIG_LENGTH = 0x04;

        private const int VERSION_OFFSET = 0x04;
        private const int VERSION_LENGTH = 0x02;

        private const int DATA_OFFSET = 0x06;
        private const int DATA_LENGTH = 0x02;

        private const int LOAD_ADDRESS_OFFSET = 0x08;
        private const int LOAD_ADDRESS_LENGTH = 0x02;

        private const int INIT_ADDRESS_OFFSET = 0x0A;
        private const int INIT_ADDRESS_LENGTH = 0x02;

        private const int PLAY_ADDRESS_OFFSET = 0x0C;
        private const int PLAY_ADDRESS_LENGTH = 0x02;

        private const int TOTAL_SONGS_OFFSET = 0x0E;
        private const int TOTAL_SONGS_LENGTH = 0x02;

        private const int STARTING_SONG_OFFSET = 0x10;
        private const int STARTING_SONG_LENGTH = 0x02;

        private const int SPEED_OFFSET = 0x12;
        private const int SPEED_LENGTH = 0x04;

        private const int NAME_OFFSET = 0x16;
        private const int NAME_LENGTH = 0x20;

        private const int ARTIST_OFFSET = 0x36;
        private const int ARTIST_LENGTH = 0x20;

        private const int COPYRIGHT_OFFSET = 0x56;
        private const int COPYRIGHT_LENGTH = 0x20;

        private const int V2_FLAGS_OFFSET = 0x76;
        private const int V2_FLAGS_LENGTH = 0x02;

        private const int V2_START_PAGE_OFFSET = 0x78;
        private const int V2_START_PAGE_LENGTH = 0x01;

        private const int V2_PAGE_LENGTH_OFFSET = 0x79;
        private const int V2_PAGE_LENGTH_LENGTH = 0x01;

        private const int V2_RESERVED_OFFSET = 0x7A;
        private const int V2_RESERVED_LENGTH = 0x02;

        private byte[] asciiSignature;
        private byte[] versionNumber;
        private byte[] dataOffset;
        private byte[] loadAddress;
        private byte[] initAddress;
        private byte[] playAddress;
        private byte[] totalSongs;
        private byte[] startingSong;
        private byte[] speed;
        private byte[] songName;
        private byte[] songArtist;
        private byte[] songCopyright;
        private byte[] v2Flags;
        private byte[] v2StartPage;
        private byte[] v2PageLength;
        private byte[] v2Reserved;
        private byte[] data;

        Dictionary<string, string> tagHash = new Dictionary<string, string>();

        public byte[] AsciiSignature { get { return this.asciiSignature; } }
        public byte[] VersionNumber { get { return this.versionNumber; } }
        public byte[] DataOffset { get { return this.dataOffset; } }
        public byte[] LoadAddress { get { return this.loadAddress; } }
        public byte[] InitAddress { get { return this.initAddress; } }
        public byte[] PlayAddress { get { return this.playAddress; } }
        public byte[] TotalSongs { get { return this.totalSongs; } }
        public byte[] StartingSong { get { return this.startingSong; } }
        public byte[] Speed { get { return this.speed; } }
        public byte[] SongName { get { return this.songName; } }
        public byte[] SongArtist { get { return this.songArtist; } }
        public byte[] SongCopyright { get { return this.songCopyright; } }
        public byte[] V2Flags { get { return this.v2Flags; } }
        public byte[] V2StartPage { get { return this.v2StartPage; } }
        public byte[] V2PageLength { get { return this.v2PageLength; } }
        public byte[] V2Reserved { get { return this.v2Reserved; } }
        public byte[] Data { get { return this.data; } }



    }
}
