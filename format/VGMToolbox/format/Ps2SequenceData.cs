using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using VGMToolbox.util;

namespace VGMToolbox.format
{
    public class Ps2SequenceData
    {
        private const int HEADER_CHUNK_OFFSET = 0x10;
        private const int MIDI_CHUNK_OFFSET = 0x30;

        private const int EMPTY_MIDI_OFFSET = -1;

        static readonly byte[] SET_TEMPO_BYTES = new byte[] { 0xFF, 0x51, 0x03 };
        static readonly byte[] END_OF_TRACK_BYTES = new byte[] { 0xFF, 0x2F, 0x00 };

        private VersionChunkStruct versionChunk;
        private SequChunkStruct headerChunk;
        private MidiChunkStruct midiChunk;

        public struct VersionChunkStruct
        {
            public UInt32 magicBytes;
            public UInt32 magicBytesSection;
            public UInt32 chunkSize;
            public UInt32 unknown;
        }

        public struct SequChunkStruct
        {
            public UInt32 magicBytes;
            public UInt32 magicBytesSection;
            public UInt32 chunkSize;
            public UInt32 fileSize;

            public UInt32 songChunkAddrAbsolute;
            public UInt32 midiChunkAddrAbsolute;
            public UInt32 seSequenceChunkAddrAbsolute;
            public UInt32 seSongChunkAddrAbsolute;
        }

        public struct MidiChunkStruct
        {
            public UInt32 magicBytes;
            public UInt32 magicBytesSection;
            public UInt32 chunkSize;
            public UInt32 maxSeqCount;

            public Int32[] subSeqOffsetAddr; // relative to chunk MIDI_CHUNK_OFFSET
        }

        public struct MidiDataChunkStruct
        {
            public UInt32 sequenceOffsetRelativeToChunk;
            public UInt16 resolution;
        }

        public Ps2SequenceData(Stream pStream)
        { 
            this.versionChunk = parseVersionChunk(pStream);
            this.headerChunk = parseHeaderChunk(pStream);
            this.midiChunk = parseMidiChunk(pStream);
        }
        
        public static VersionChunkStruct parseVersionChunk(Stream pStream)
        {
            VersionChunkStruct ret = new VersionChunkStruct();

            ret.magicBytes = BitConverter.ToUInt32(ParseFile.parseSimpleOffset(pStream, 0, 4), 0);
            ret.magicBytesSection = BitConverter.ToUInt32(ParseFile.parseSimpleOffset(pStream, 4, 4), 0);
            ret.chunkSize = BitConverter.ToUInt32(ParseFile.parseSimpleOffset(pStream, 8, 4), 0);
            ret.unknown = BitConverter.ToUInt16(ParseFile.parseSimpleOffset(pStream, 12, 4), 0);

            return ret;
        }

        public static SequChunkStruct parseHeaderChunk(Stream pStream)
        {
            SequChunkStruct ret = new SequChunkStruct();

            ret.magicBytes = BitConverter.ToUInt32(ParseFile.parseSimpleOffset(pStream, HEADER_CHUNK_OFFSET + 0, 4), 0);
            ret.magicBytesSection = BitConverter.ToUInt32(ParseFile.parseSimpleOffset(pStream, HEADER_CHUNK_OFFSET + 4, 4), 0);
            ret.chunkSize = BitConverter.ToUInt32(ParseFile.parseSimpleOffset(pStream, HEADER_CHUNK_OFFSET + 8, 4), 0);
            ret.fileSize = BitConverter.ToUInt32(ParseFile.parseSimpleOffset(pStream, HEADER_CHUNK_OFFSET + 12, 4), 0);

            ret.songChunkAddrAbsolute = BitConverter.ToUInt32(ParseFile.parseSimpleOffset(pStream, HEADER_CHUNK_OFFSET + 16, 4), 0);
            ret.midiChunkAddrAbsolute = BitConverter.ToUInt32(ParseFile.parseSimpleOffset(pStream, HEADER_CHUNK_OFFSET + 20, 4), 0);
            ret.seSequenceChunkAddrAbsolute = BitConverter.ToUInt32(ParseFile.parseSimpleOffset(pStream, HEADER_CHUNK_OFFSET + 24, 4), 0);
            ret.seSongChunkAddrAbsolute = BitConverter.ToUInt32(ParseFile.parseSimpleOffset(pStream, HEADER_CHUNK_OFFSET + 28, 4), 0);

            return ret;
        }

        public static MidiChunkStruct parseMidiChunk(Stream pStream)
        {
            MidiChunkStruct ret = new MidiChunkStruct();

            ret.magicBytes = BitConverter.ToUInt32(ParseFile.parseSimpleOffset(pStream, MIDI_CHUNK_OFFSET + 0, 4), 0);
            ret.magicBytesSection = BitConverter.ToUInt32(ParseFile.parseSimpleOffset(pStream, MIDI_CHUNK_OFFSET + 4, 4), 0);
            ret.chunkSize = BitConverter.ToUInt32(ParseFile.parseSimpleOffset(pStream, MIDI_CHUNK_OFFSET + 8, 4), 0);
            ret.maxSeqCount = BitConverter.ToUInt32(ParseFile.parseSimpleOffset(pStream, MIDI_CHUNK_OFFSET + 12, 4), 0);

            ret.subSeqOffsetAddr = new int[ret.maxSeqCount + 1];

            for (int i = 0; i < ret.subSeqOffsetAddr.Length; i++)
            {
                ret.subSeqOffsetAddr[i] = BitConverter.ToInt32(ParseFile.parseSimpleOffset(pStream, MIDI_CHUNK_OFFSET + 16 + (4 * i), 4), 0);
            }

            return ret;
        }



        public int GetMaxSequenceCount()
        {
            return (int)this.midiChunk.maxSeqCount;
        }
        
        public UInt32 getTempoForSequenceNumber(Stream pStream, int pSequenceNumber)
        {
            UInt32 ret = 0;

            if (pSequenceNumber <= this.midiChunk.maxSeqCount &&
                this.midiChunk.subSeqOffsetAddr[pSequenceNumber] != EMPTY_MIDI_OFFSET)
            {
                Int32 midiBlockOffset = MIDI_CHUNK_OFFSET + this.midiChunk.subSeqOffsetAddr[pSequenceNumber];

                long setTempoOffset = ParseFile.GetNextOffset(pStream, (long)midiBlockOffset, SET_TEMPO_BYTES);
                
                byte[] tempoValBytes = new byte[4];
                Array.Copy(ParseFile.parseSimpleOffset(pStream, setTempoOffset + 3, 3), 0, tempoValBytes, 1, 3);
                
                Array.Reverse(tempoValBytes); // flip order to LE for use with BitConverter
                ret = BitConverter.ToUInt32(tempoValBytes, 0);
            }

            return ret;
        }

        public UInt32 getEndOfTrackForSequenceNumber(Stream pStream, int pSequenceNumber)
        {
            UInt32 ret = 0;

            if (pSequenceNumber <= this.midiChunk.maxSeqCount &&
                this.midiChunk.subSeqOffsetAddr[pSequenceNumber] != EMPTY_MIDI_OFFSET)
            {
                Int32 midiBlockOffset = MIDI_CHUNK_OFFSET + this.midiChunk.subSeqOffsetAddr[pSequenceNumber];

                long endOfTrackOffset = ParseFile.GetNextOffset(pStream, (long)midiBlockOffset, END_OF_TRACK_BYTES);

                ret = (UInt32)endOfTrackOffset;
            }

            return ret;
        }

        public long getTimeForSequenceNumber(Stream pStream, int pSequenceNumber)
        {
            long ret = 0;
            
            if (pSequenceNumber <= this.midiChunk.maxSeqCount &&
                this.midiChunk.subSeqOffsetAddr[pSequenceNumber] != EMPTY_MIDI_OFFSET)
            {
                long tempo = this.getTempoForSequenceNumber(pStream, pSequenceNumber);
                long eofOffset = this.getEndOfTrackForSequenceNumber(pStream, pSequenceNumber);
                Int32 midiBlockOffset = MIDI_CHUNK_OFFSET + this.midiChunk.subSeqOffsetAddr[pSequenceNumber];

                MidiDataChunkStruct dataChunkHeader = new MidiDataChunkStruct();
                dataChunkHeader.sequenceOffsetRelativeToChunk = BitConverter.ToUInt32(ParseFile.parseSimpleOffset(pStream, (long)midiBlockOffset, 4), 0);
                dataChunkHeader.resolution = BitConverter.ToUInt16(ParseFile.parseSimpleOffset(pStream, (long)(midiBlockOffset + 4), 2), 0);

                if (dataChunkHeader.sequenceOffsetRelativeToChunk == 6) // uncompressed
                {
                    pStream.Position = (long)(midiBlockOffset + dataChunkHeader.sequenceOffsetRelativeToChunk);
                    
                    int currentByte;
                    int previousByte;

                    int previousCommand;
                    long previousCommandOffset;

                    while (pStream.Position < eofOffset)
                    {
                        currentByte = pStream.ReadByte();

                        if (currentByte >= 128) // command
                        {
                            
                            
                            previousCommand = currentByte;
                            previousCommandOffset = pStream.Position;
                        }
                        else                   // data
                        {
                            previousByte = currentByte;
                        }
                    }
                }                
            }

            return ret;
        }
    }
}
