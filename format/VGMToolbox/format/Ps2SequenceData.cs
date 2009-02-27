﻿using System;
using System.Collections;
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
            // useful references
            //http://www.dogsbodynet.com/fileformats/midi.html
            //http://faydoc.tripod.com/formats/mid.htm
            //http://cnx.org/content/m15052/latest/
            //http://jedi.ks.uiuc.edu/~johns/links/music/midifile.html
            //http://www.sonicspot.com/guide/midifiles.html
            //http://www.ccarh.org/courses/253/assignment/midifile/
            // http://www.skytopia.com/project/articles/midi.html
            // http://opensource.jdkoftinoff.com/jdks/svn/trunk/libjdkmidi/trunk/src/

            long bytesToRead;
            long ret = 0;

            int[] chantype =
            {
              0, 0, 0, 0, 0, 0, 0, 0,         // 0x00 through 0x70
              2, 2, 2, 2, 1, 1, 2, 0          // 0x80 through 0xf0
            };

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
                    //long currentOffset;
                    //int previousByte = -1;
                    UInt64 totalTicks = 0;


                    // int previousCommand = -1;
                    //long previousCommandOffset = -1;
                    //int previousCommandDataBlocksCount;
                    //int currentCommandDataBlocksCount;

                    while (pStream.Position < eofOffset)                    
                    {

                        /* 
                        currentByte = pStream.ReadByte();
                          currentOffset = pStream.Position - 1;

                          if (currentByte >= 128) // command
                          {
                              currentCommandDataBlocksCount =
                                  getByteCountForCommand(pStream, currentByte, pStream.Position - 1);

                              if ((previousCommand > 0))
                              { 
                                  previousCommandDataBlocksCount =
                                      getByteCountForCommand(pStream, previousCommand, previousCommandOffset);

                                  // check for delta time blocks
                                  if ((previousCommandOffset + previousCommandDataBlocksCount) < currentOffset)
                                  {
                                      // get delta blocks
                                      totalTicks += getDeltaTime(pStream, (previousCommandOffset + previousCommandDataBlocksCount), 
                                          (currentOffset));
                                  }
                              }
                            
                              previousCommand = currentByte;
                              previousCommandOffset = pStream.Position - 1;

                              // goto the next command
                              pStream.Position += (currentCommandDataBlocksCount - 1);
                          }
                          else                   // data
                          {
                              previousByte = currentByte;
                          }
                       */
                    }
                }                
            }

            return ret;
        }

        private int getByteCountForCommand(Stream pStream, int pCommand, long pCommandOffset)
        {
            int ret = 0;
            int noteOnCheck;
            int commandId = pCommand & 0xF0;

            switch (commandId)
            { 
                case 0x80: // note off
                case 0xC0: // program change
                case 0xD0: // channel aftertouch
                    ret = 2;
                    break;
                case 0xA0: // key aftertouch
                case 0xB0: // control change
                case 0xE0: // pitch wheel change
                    ret = 3;
                    break;
                case 0xF0:
                    ret = ParseFile.parseSimpleOffset(pStream, pCommandOffset + 2, 1)[0] + 3;
                    break;
                case 0x90: // note on (needs work)
                    ret = 3;

                    noteOnCheck = ParseFile.parseSimpleOffset(pStream, pCommandOffset + 2, 1)[0];
                    if ((noteOnCheck & 0xF0) == 0x80)
                    {
                        ret = 2;
                    }

                    break;
            }

            return ret;
        }

        private UInt32 getDeltaTime(Stream pStream, long pStartOffset, long pEndOffset)
        {
            UInt32 ret = 0;
            int deltaLength = (int)(pEndOffset - pStartOffset);
            byte[] deltaBytes = ParseFile.parseSimpleOffset(pStream, pStartOffset, deltaLength);
            byte[] timeBytes = new byte[4];

            Array.Copy(deltaBytes, 0, timeBytes, (timeBytes.Length - deltaLength), deltaLength);
            Array.Reverse(timeBytes); // convert to little endian
            ret = BitConverter.ToUInt32(timeBytes, 0);

            return ret;
        }
    }
}
