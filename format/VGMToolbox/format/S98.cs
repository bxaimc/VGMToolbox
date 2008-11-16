using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using ICSharpCode.SharpZipLib.Checksums;

using VGMToolbox.util;
using VGMToolbox.util.ObjectPooling;

namespace VGMToolbox.format
{
    public class S98 : IFormat
    {
        private static readonly byte[] ASCII_SIGNATURE = new byte[] { 0x53, 0x39, 0x38 };
        private const string FORMAT_ABBREVIATION = "S98";

        private static readonly byte[] S98_VERSION_01 = new byte[] { 0x31 };
        private static readonly byte[] S98_VERSION_02 = new byte[] { 0x32 };
        private static readonly byte[] S98_VERSION_03 = new byte[] { 0x33 };

        private const string TAG_UTF8_INDICATOR = "utf8=";
        private const int TAG_IDENTIFIER_LENGTH = 5;

        private const string HOOT_DRIVER_ALIAS = null;
        private const string HOOT_DRIVER_TYPE = null;
        private const string HOOT_DRIVER = null;

        struct S98Device
        { 
            public byte[] DeviceType;
            public byte[] Clock;
            public byte[] Pan;
            public byte[] Reserve;
        }
        private const int S98DEVICE_TYPE_OFFSET = 0x00;
        private const int S98DEVICE_TYPE_LENGTH = 0x04;

        private const int S98DEVICE_CLOCK_OFFSET = 0x04;
        private const int S98DEVICE_CLOCK_LENGTH = 0x04;

        private const int S98DEVICE_PAN_OFFSET = 0x08;
        private const int S98DEVICE_PAN_LENGTH = 0x04;

        private const int S98DEVICE_RESERVED_OFFSET = 0x0C;
        private const int S98DEVICE_RESERVED_LENGTH = 0x04;

        // ALL VERSIONS

        private const int SIG_OFFSET = 0x00;
        private const int SIG_LENGTH = 0x03;

        private const int VERSION_OFFSET = 0x03;
        private const int VERSION_LENGTH = 0x01;

        private const int TIMER_INFO_OFFSET = 0x04;
        private const int TIMER_INFO_LENGTH = 0x04;

        private const int TIMER_INFO2_OFFSET = 0x08;
        private const int TIMER_INFO2_LENGTH = 0x04;

        private const int COMPRESSING_OFFSET = 0x0C;
        private const int COMPRESSING_LENGTH = 0x04;

        private const int SONG_NAME_OFFSET_OFFSET = 0x10;
        private const int SONG_NAME_OFFSET_LENGTH = 0x04;

        private const int DUMP_DATA_OFFSET_OFFSET = 0x14;
        private const int DUMP_DATA_OFFSET_LENGTH = 0x04;

        private const int LOOP_POINT_OFFSET_OFFSET = 0x18;
        private const int LOOP_POINT_OFFSET_LENGTH = 0x04;

        // V1 ONLY        
        private const int V1_RESERVED_OFFSET = 0x1C;
        private const int V1_RESERVED_LENGTH = 0x24;        

        // V2 ONLY
        private const int V2_COMPRESSED_DATA_OFFEST_OFFSET = 0x1C;
        private const int V2_COMPRESSED_DATA_OFFEST_LENGTH = 0x04;

        // V3 ONLY
        private const int V3_DEVICE_COUNT_OFFSET = 0x1C;
        private const int V3_DEVICE_COUNT_LENGTH = 0x04;

        private const int V3_DEVICE_INFO_OFFSET = 0x20;
        private const int V3_DEVICE_INFO_SIZE = 0x10;
        
        // ALL
        private byte[] asciiSignature;
        private byte[] version;
        private byte[] timerInfo;
        private byte[] timerInfo2;
        private byte[] compressing;
        private byte[] songNameOffset;
        private byte[] dumpDataOffset;
        private byte[] loopPointOffset;
        private byte[] data;

        public byte[] AsciiSignature { get { return this.asciiSignature; } }
        public byte[] Version { get { return this.version; } }
        public byte[] TimerInfo { get { return this.timerInfo; } }
        public byte[] TimerInfo2 { get { return this.timerInfo2; } }
        public byte[] Compressing { get { return this.compressing; } }
        public byte[] SongNameOffset { get { return this.songNameOffset; } }
        public byte[] DumpDataOffset { get { return this.dumpDataOffset; } }
        public byte[] LoopPointOffset { get { return this.loopPointOffset; } }
        public byte[] Data { get { return this.data; } }

        // V1
        private byte[] v1Reserved;
        
        public byte[] V1Reserved { get { return this.v1Reserved; } }        

        // V3
        private byte[] deviceCount;
        private byte[] v3Tags;
        private ArrayList s98Devices = new ArrayList();

        public byte[] DeviceCount { get { return this.deviceCount; } }
        public byte[] V3Tags { get { return this.v3Tags; } }

        Dictionary<string, string> tagHash = new Dictionary<string, string>();
        Dictionary<int, string> deviceHash = new Dictionary<int, string>();

        #region METHODS

        public byte[] getAsciiSignature(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, SIG_OFFSET, SIG_LENGTH);
        }
        public byte[] getVersion(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, VERSION_OFFSET, VERSION_LENGTH);
        }
        public byte[] getTimerInfo(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, TIMER_INFO_OFFSET, TIMER_INFO_LENGTH);
        }
        public byte[] getTimerInfo2(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, TIMER_INFO2_OFFSET, TIMER_INFO2_LENGTH);
        }
        public byte[] getCompressing(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, COMPRESSING_OFFSET, COMPRESSING_LENGTH);
        }
        public byte[] getSongNameOffset(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, SONG_NAME_OFFSET_OFFSET, SONG_NAME_OFFSET_LENGTH);
        }
        public byte[] getDumpDataOffset(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, DUMP_DATA_OFFSET_OFFSET, DUMP_DATA_OFFSET_LENGTH);
        }
        public byte[] getLoopPointOffset(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, LOOP_POINT_OFFSET_OFFSET, LOOP_POINT_OFFSET_LENGTH);
        }
        public byte[] getV1Reserved(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, V1_RESERVED_OFFSET, V1_RESERVED_LENGTH);
        }

        // Version 03
        public byte[] getDeviceCount(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, V3_DEVICE_COUNT_OFFSET, V3_DEVICE_COUNT_LENGTH);
        }
        public void getS98Devices(Stream pStream)
        {
            int offset = V3_DEVICE_INFO_OFFSET;
            int deviceCount = BitConverter.ToInt32(this.deviceCount, 0);

            for (int i = 0; i < deviceCount; i++)
            { 
                S98Device s98Device = new S98Device();
                s98Device.DeviceType = ParseFile.parseSimpleOffset(pStream, offset + S98DEVICE_TYPE_OFFSET, 
                                        S98DEVICE_TYPE_LENGTH);
                s98Device.Clock = ParseFile.parseSimpleOffset(pStream, offset + S98DEVICE_CLOCK_OFFSET, 
                                        S98DEVICE_CLOCK_LENGTH);
                s98Device.Pan = ParseFile.parseSimpleOffset(pStream, offset + S98DEVICE_PAN_OFFSET, 
                                        S98DEVICE_PAN_LENGTH);
                s98Device.Reserve = ParseFile.parseSimpleOffset(pStream, offset + S98DEVICE_RESERVED_OFFSET, 
                                        S98DEVICE_RESERVED_LENGTH);

                s98Devices.Add(s98Device);
                
                offset += V3_DEVICE_INFO_SIZE;
            }
        }

        public void Initialize(Stream pStream)
        {
            this.asciiSignature = this.getAsciiSignature(pStream);
            this.version = this.getVersion(pStream);
            this.timerInfo = this.getTimerInfo(pStream);
            this.timerInfo2 = this.getTimerInfo2(pStream);
            this.compressing = this.getCompressing(pStream);
            this.songNameOffset = this.getSongNameOffset(pStream);
            this.dumpDataOffset = this.getDumpDataOffset(pStream);
            this.loopPointOffset = this.getLoopPointOffset(pStream);
            
            // VERSION 01
            if (ParseFile.CompareSegment(this.version, 0, S98_VERSION_01))
            {
                int v1DataOffset = BitConverter.ToInt32(this.dumpDataOffset, 0);

                this.v1Reserved = this.getV1Reserved(pStream);
                this.data = ParseFile.parseSimpleOffset(pStream, v1DataOffset, (int) pStream.Length - v1DataOffset);
            }


            // VERSION 03
            if (ParseFile.CompareSegment(this.version, 0, S98_VERSION_03))
            {
                this.deviceCount = this.getDeviceCount(pStream);
                this.getS98Devices(pStream);

                Int32 tagOffset = BitConverter.ToInt32(this.songNameOffset, 0) + TAG_IDENTIFIER_LENGTH;
                this.v3Tags = ParseFile.parseSimpleOffset(pStream, tagOffset, (int)pStream.Length - tagOffset);

                this.data = ParseFile.parseSimpleOffset(pStream, V3_DEVICE_INFO_OFFSET, tagOffset - V3_DEVICE_INFO_OFFSET);
            }

            this.initDeviceHash();
            this.initializeTagHash(pStream);
            this.parseTagData();
            this.addDevicesToHash();
        }

        private void initializeTagHash(Stream pStream)
        {
            System.Text.Encoding enc = System.Text.Encoding.ASCII;

            tagHash.Add("S98 Version", enc.GetString(this.version));
            
            // Song Name
            if (ParseFile.CompareSegment(this.version, 0, S98_VERSION_01) ||
                ParseFile.CompareSegment(this.version, 0, S98_VERSION_02))
            {
                if (BitConverter.ToUInt32(this.songNameOffset, 0) != 0)
                {
                    Int32 songOffset = BitConverter.ToInt32(this.songNameOffset, 0);
                    Int32 songLength = ParseFile.getSegmentLength(pStream, songOffset, new byte[] { 0x00 });
                    byte[] songName = ParseFile.parseSimpleOffset(pStream, songOffset, songLength);
                    
                    tagHash.Add("Song Name", enc.GetString(songName));
                    tagHash.Add("Uncompressed Data Size", "0x" + BitConverter.ToInt32(this.compressing, 0).ToString("X2"));
                    tagHash.Add("Offset [Song Name]", "0x" + BitConverter.ToInt32(this.songNameOffset, 0).ToString("X2"));
                }
            }           
 
            // Offsets            
            tagHash.Add("Offset [Dump Data]", "0x" + BitConverter.ToInt32(this.dumpDataOffset, 0).ToString("X2"));
            tagHash.Add("Offset [Loop Point]", "0x" + BitConverter.ToInt32(this.loopPointOffset, 0).ToString("X2"));

        }

        private void parseTagData()
        {
            string tagsString;
            
            if (this.v3Tags != null)
            {
                this.v3Tags = FileUtil.ReplaceNullByteWithSpace(this.v3Tags);

                System.Text.Encoding enc = System.Text.Encoding.ASCII;
                tagsString = enc.GetString(this.v3Tags);

                // check for utf8 tag and reencode bytes if needed
                if (tagsString.IndexOf(TAG_UTF8_INDICATOR) > -1)
                {
                    enc = System.Text.Encoding.UTF8;
                    tagsString = enc.GetString(this.v3Tags);
                }

                string[] splitTags = tagsString.Trim().Split((char)0x0A);
                string[] tag;

                foreach (string s in splitTags)
                {
                    tag = s.Split((char)0x3D);

                    if (tag.Length >= 2)
                    {
                        if (!tagHash.ContainsKey(tag[0]))
                        {
                            tagHash.Add(tag[0].Trim(), tag[1].Trim());
                        }
                        else
                        {
                            string oldTag = tagHash[tag[0]] + Environment.NewLine;
                            tagHash.Remove(tag[0]);
                            tagHash.Add(tag[0], oldTag + tag[1]);
                        }
                    }
                }                        
            }
        }

        private void addDevicesToHash()
        {
            string deviceString = String.Empty;

            foreach (S98Device s in s98Devices)
            { 
                deviceString += "[" + deviceHash[BitConverter.ToInt32(s.DeviceType, 0)] + "]";                
            }

            if (s98Devices.Count > 0)
            {
                tagHash.Add("Devices Used", deviceString);
            }
        }

        private void initDeviceHash()
        {
            deviceHash.Add(0, "None");
            deviceHash.Add(1, "PSG (YM2149)");
            deviceHash.Add(2, "OPN (YM2203)");
            deviceHash.Add(3, "OPN2 (YM2612)");
            deviceHash.Add(4, "OPNA (YM2608)");
            deviceHash.Add(5, "OPM (YM2151)");
            deviceHash.Add(6, "OPLL (YM2413)");
            deviceHash.Add(7, "OPL (YM3526)");
            deviceHash.Add(8, "OPL2 (YM3812)");
            deviceHash.Add(9, "OPL3 (YMF262)");
            deviceHash.Add(15, "PSG (AY-3-8910)");
            deviceHash.Add(16, "DCSG (SN76489)");
        }

        public void GetDatFileCrc32(string pPath, ref Dictionary<string, ByteArray> pLibHash,
            ref Crc32 pChecksum, bool pUseLibHash)
        {
            pChecksum.Reset();

            pChecksum.Update(this.version);
            pChecksum.Update(this.timerInfo);
            pChecksum.Update(this.timerInfo2);                
            pChecksum.Update(this.compressing);

            // Version 03
            if (ParseFile.CompareSegment(this.version, 0, S98_VERSION_03))
            {
                pChecksum.Update(this.deviceCount);
            }

            pChecksum.Update(this.data);
            
            // ADD LOOP POINT AS LOOP OFFSET - DATA OFFSET?
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

        public bool HasMultipleFileExtensions()
        {
            return false;
        }

        public Dictionary<string, string> GetTagHash()
        {
            return this.tagHash;
        }

        public int GetStartingSong() { return 0; }
        public int GetTotalSongs() { return 0; }
        public string GetSongName() { return null; }

        #endregion

        #region HOOT

        public string GetHootDriverAlias() { return HOOT_DRIVER_ALIAS; }
        public string GetHootDriverType() { return HOOT_DRIVER_TYPE; }
        public string GetHootDriver() { return HOOT_DRIVER; }

        #endregion
    }
}
