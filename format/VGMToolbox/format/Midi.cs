using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

using ICSharpCode.SharpZipLib.Checksums;

using VGMToolbox.format.util;
using VGMToolbox.util;

namespace VGMToolbox.format
{
    public struct MidiTrackInfo
    {
        public long StartOffset;
        public byte[] TrackHeader;
        public byte[] TrackLength;
    }

    public class Midi : IFormat, IExtractableFormat
    {
        public static readonly byte[] ASCII_SIGNATURE_MTHD = new byte[] { 0x4D, 0x54, 0x68, 0x64 }; // MThd
        public static readonly byte[] ASCII_SIGNATURE_MTRK = new byte[] { 0x4D, 0x54, 0x72, 0x6B }; // MTrk
        private const string FORMAT_ABBREVIATION = "MIDI";
        private const string FILE_EXTENSION = ".mid";

        private const int HEADER_SIG_OFFSET = 0x00;
        private const int HEADER_SIG_LENGTH = 0x04;

        private const int HEADER_SIZE_OFFSET = 0x04;
        private const int HEADER_SIZE_LENGTH = 0x04;

        private const int FILE_FORMAT_OFFSET = 0x08;
        private const int FILE_FORMAT_LENGTH = 0x02;

        private const int NUMBER_OF_TRACKS_OFFSET = 0x0A;
        private const int NUMBER_OF_TRACKS_LENGTH = 0x02;

        private const int DELTA_TICKS_OFFSET = 0x0C;
        private const int DELTA_TICKS_LENGTH = 0x02;

        private const int FIRST_TRACK_HEADER_SIG_OFFSET = 0x0E;
        private const int TRACK_HEADER_SIG_LENGTH = 0x04;

        private const int RELATIVE_TRACK_LENGTH_OFFSET = 0x04;
        private const int TRACK_LENGTH_LENGTH = 0x04;

        private const int FIRST_TRACK_DATA_OFFSET = 0x16;

        Dictionary<string, string> tagHash = new Dictionary<string, string>();

        private byte[] asciiSignature;
        private byte[] headerSize;
        private byte[] fileFormat;
        private byte[] numberOfTracks;
        private byte[] deltaTicks;
        private MidiTrackInfo[] midiTracks;

        private string filePath;        
        private long fileStartOffset;
        private long totalFileLength;
        
        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }
        public long FileStartOffset
        {
            get { return fileStartOffset; }
            set { fileStartOffset = value; }
        }
        public long TotalFileLength
        {
            get { return totalFileLength; }
            set { totalFileLength = value; }
        }

        #region METHODS

        public void Initialize(Stream pStream, string pFilePath)
        {
            this.Initialize(pStream, pFilePath, 0);
        }
        public void Initialize(Stream pStream, string pFilePath, long pFileOffset)
        {
            UInt16 numberOfTracks;
            long totalTrackLength;
            long offset;
                        
            this.filePath = pFilePath;
            this.fileStartOffset = pFileOffset;
            this.asciiSignature = ParseFile.ParseSimpleOffset(pStream, pFileOffset + HEADER_SIG_OFFSET, HEADER_SIG_LENGTH);
            this.headerSize = ParseFile.ParseSimpleOffset(pStream, pFileOffset + HEADER_SIZE_OFFSET, HEADER_SIZE_LENGTH);
            this.fileFormat = ParseFile.ParseSimpleOffset(pStream, pFileOffset + FILE_FORMAT_OFFSET, FILE_FORMAT_LENGTH);
            this.numberOfTracks = ParseFile.ParseSimpleOffset(pStream, pFileOffset + NUMBER_OF_TRACKS_OFFSET, NUMBER_OF_TRACKS_LENGTH);
            this.deltaTicks = ParseFile.ParseSimpleOffset(pStream, pFileOffset + DELTA_TICKS_OFFSET, DELTA_TICKS_LENGTH);

            // get tracks
            this.totalFileLength = Midi.HEADER_SIG_LENGTH + Midi.HEADER_SIG_LENGTH + 
                VGMToolbox.util.Encoding.GetUint32BigEndian(this.headerSize);
            numberOfTracks = VGMToolbox.util.Encoding.GetUint16BigEndian(this.numberOfTracks);
            
            midiTracks = new MidiTrackInfo[numberOfTracks];            
            offset = (long)FIRST_TRACK_HEADER_SIG_OFFSET;
            
            for (int i = 0; i < numberOfTracks; i++)
            {
                midiTracks[i] = new MidiTrackInfo();
                midiTracks[i].StartOffset = offset;
                midiTracks[i].TrackHeader = ParseFile.ParseSimpleOffset(pStream, pFileOffset + offset, TRACK_HEADER_SIG_LENGTH);
                midiTracks[i].TrackLength = ParseFile.ParseSimpleOffset(pStream, pFileOffset + offset + RELATIVE_TRACK_LENGTH_OFFSET, TRACK_LENGTH_LENGTH);

                totalTrackLength = (long)(TRACK_HEADER_SIG_LENGTH + TRACK_LENGTH_LENGTH + 
                   VGMToolbox.util.Encoding.GetUint32BigEndian(midiTracks[i].TrackLength));
                offset += totalTrackLength;
                this.totalFileLength += totalTrackLength;
            }

            tagHash.Add("MIDI Type", 
                VGMToolbox.util.Encoding.GetUint16BigEndian(this.fileFormat).ToString());
            tagHash.Add("Total Tracks", numberOfTracks.ToString());
            tagHash.Add("Total File Size", this.totalFileLength.ToString());

            for (int i = 0; i < numberOfTracks; i++)
            {
                tagHash.Add(String.Format("Track {0} Offset [Length]", i.ToString()), 
                    String.Format("0x{0} [0x{1}]", midiTracks[i].StartOffset.ToString("X8"), VGMToolbox.util.Encoding.GetUint32BigEndian(midiTracks[i].TrackLength).ToString("X4")));
            }

        }

        public byte[] GetAsciiSignature()
        {
            return ASCII_SIGNATURE_MTHD;
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

        public void GetDatFileCrc32(ref Crc32 pChecksum)
        {
            throw new NotImplementedException();
        }
        public void GetDatFileChecksums(ref Crc32 pChecksum, ref CryptoStream pMd5CryptoStream,
            ref CryptoStream pSha1CryptoStream)
        {
            throw new NotImplementedException();
        }

        public void ExtractToFile(Stream pStream, string pOutputDirectory)
        { 
            string outputFileName = Path.GetFileName(this.filePath);
            int fileCount = 0;

            if (!Directory.Exists(pOutputDirectory))
            {
                Directory.CreateDirectory(pOutputDirectory);
            }
            else
            {
                fileCount = Directory.GetFiles(pOutputDirectory, String.Format("{0}*", Path.GetFileNameWithoutExtension(outputFileName)), SearchOption.TopDirectoryOnly).Length;
            }

            outputFileName = String.Format("{0}_{1}{2}", Path.GetFileNameWithoutExtension(outputFileName), fileCount.ToString("X4"), Midi.FILE_EXTENSION);
            outputFileName = Path.Combine(pOutputDirectory, outputFileName);

            ParseFile.ExtractChunkToFile(pStream, this.fileStartOffset, (int) this.totalFileLength, outputFileName);
        }

        #endregion

        # region STATIC METHODS

        public static bool IsMidi(string pFilePath)
        {
            bool ret = false;

            if (!String.IsNullOrEmpty(pFilePath))
            {
                string fullPath = Path.GetFullPath(pFilePath);

                if (File.Exists(fullPath))
                {
                    using (FileStream fs = File.OpenRead(fullPath))
                    {
                        Type dataType = FormatUtil.getObjectType(fs);

                        if ((dataType != null) && (dataType.Name.Equals("Midi")))
                        {
                            ret = true;
                        }
                    }
                }
            }
            return ret;
        }

        #endregion
    }
}
