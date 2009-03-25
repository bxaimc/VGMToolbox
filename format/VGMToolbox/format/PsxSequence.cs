using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using VGMToolbox.util;

namespace VGMToolbox.format
{
    public class PsxSequence
    {
        public static readonly byte[] ASCII_SIGNATURE = new byte[] {0x70, 0x51, 0x45, 0x53 }; //pQES
        public static readonly byte[] END_SEQUENCE = new byte[] { 0xFF, 0x2F, 0x00 };
        public const string FILE_EXTENSION = ".seq";

        public struct PsxSqTimingStruct
        {
            public double TimeInSeconds;
            public int FadeInSeconds;
            public string Warnings;
        }

        struct SqHeaderStruct
        {
            public byte[] MagicBytes;
            public UInt32 Version;
            public UInt16 Resolution;
            public UInt32 InitialTempo;
            public UInt16 unknown;
        }

        public struct PsxSqInitStruct
        {
            public bool force2Loops;
            public bool forceOppositeFormatType;
        }

        private SqHeaderStruct sqHeader;
        private PsxSqTimingStruct timingInfo;
        private bool force2Loops;
        private bool forceOppositeFormatType;
        private Dictionary<int, int> dataBytesPerCommand;

        public PsxSqTimingStruct TimingInfo { get { return timingInfo; } }

        public PsxSequence(Stream pStream, PsxSqInitStruct pPsxSqInitStruct)
        {
            this.sqHeader = parseSqHeader(pStream);

            this.force2Loops = pPsxSqInitStruct.force2Loops;
            this.forceOppositeFormatType = pPsxSqInitStruct.forceOppositeFormatType;

            this.dataBytesPerCommand = new Dictionary<int, int>();
            this.dataBytesPerCommand.Add(0x80, 2);
            this.dataBytesPerCommand.Add(0x90, 2);
            this.dataBytesPerCommand.Add(0xA0, 2);
            this.dataBytesPerCommand.Add(0xB0, 2);
            this.dataBytesPerCommand.Add(0xC0, 1);
            this.dataBytesPerCommand.Add(0xD0, 1);
            this.dataBytesPerCommand.Add(0xE0, 2);
            this.dataBytesPerCommand.Add(0xF0, 0);

            timingInfo = getTimingInfo(pStream);
        }

        private SqHeaderStruct parseSqHeader(Stream pStream)
        {
            SqHeaderStruct ret;
            
            byte[] temp; // used to reverse from big endian to little endian

            // magic bytes
            ret = new SqHeaderStruct();
            ret.MagicBytes = ParseFile.parseSimpleOffset(pStream, 0, 4); // pQES

            // version
            temp = ParseFile.parseSimpleOffset(pStream, 4, 4);
            Array.Reverse(temp);
            ret.Version = BitConverter.ToUInt32(temp, 0);

            // resolution
            temp = ParseFile.parseSimpleOffset(pStream, 8, 2);
            Array.Reverse(temp);
            ret.Resolution = BitConverter.ToUInt16(temp, 0);

            // tempo
            temp = new byte[4];
            Array.Copy(ParseFile.parseSimpleOffset(pStream, 10, 3), 0, temp, 1, 3);
            Array.Reverse(temp);
            ret.InitialTempo = BitConverter.ToUInt32(temp, 0);

            // unknown
            temp = ParseFile.parseSimpleOffset(pStream, 13, 2);
            Array.Reverse(temp);
            ret.unknown = BitConverter.ToUInt16(temp, 0);

            return ret;
        }

        private PsxSqTimingStruct getTimingInfo(Stream pStream)
        {
            PsxSqTimingStruct ret = new PsxSqTimingStruct();
            ret.Warnings = String.Empty;

            long incomingStreamPosition = pStream.Position;

            int currentByte;
            long currentOffset = 0;
            long iterationOffset = 0;
            long commandOffset = 0;
            int dataByte1;
            int dataByte2;

            int deltaTimeByteCount;

            int runningCommand = 0;
            int dataByteCount;
            uint tempo = this.sqHeader.InitialTempo;

            int metaCommandByte;
            int metaCommandLengthByte;

            bool running = false;

            bool loopFound = false;
            bool loopEndFound = false;
            double loopTime;

            int loopTimeMultiplier = 1;
            Stack<double> loopTimeStack = new Stack<double>();
            ulong loopTicks;
            Stack<ulong> loopTickStack = new Stack<ulong>();

            int loopsOpened = 0;
            int loopsClosed = 0;

            UInt64 currentTicks;
            UInt64 totalTicks = 0;

            double currentTime;
            double totalTime = 0;
            double timeSinceLastLoopEnd = 0; // used for extra Loop End tag hack

            byte[] tempoValBytes;

            switch (this.sqHeader.Version)
            {
                case 0:
                    if (!this.forceOppositeFormatType)
                    {
                        pStream.Position = 19;
                    }
                    else
                    {
                        pStream.Position = 15;
                    }
                    break;
                case 1:
                    if (!this.forceOppositeFormatType)
                    {
                        pStream.Position = 15;
                    }
                    else
                    {
                        pStream.Position = 19;
                    }
                    break;
            }


            if (this.forceOppositeFormatType)
            {
                ret.Warnings += "Failed as internal version type, trying to force opposite version.  Please verify after completion." + Environment.NewLine;
            }
            
            while (pStream.Position < pStream.Length)
            {
                iterationOffset = pStream.Position;

                currentByte = pStream.ReadByte();
                currentOffset = pStream.Position - 1;

                // build 7-bit num from bytes (variable length string)                
                if ((currentByte & 0x80) != 0)
                {
                    deltaTimeByteCount = 1;
                    currentTicks = (ulong)(currentByte & 0x7F);

                    do
                    {
                        currentByte = pStream.ReadByte();
                        currentOffset = pStream.Position - 1;
                        deltaTimeByteCount++;

                        if (deltaTimeByteCount > 4)
                        {
                            int x4 = 1;
                        }

                        currentTicks = (currentTicks << 7) + (ulong)(currentByte & 0x7F);
                    } while ((currentByte & 0x80) != 0);
                }
                else // only one byte, no need for conversion
                {
                    currentTicks = (ulong)currentByte;
                }

                if ((tempo == 0) && (currentTicks != 0))
                {
                    throw new Exception("Tempo not set and current ticks not equal zero");
                }

                currentTime = currentTicks != 0 ? (double)((currentTicks * (ulong)tempo) / (ulong)this.sqHeader.Resolution) : 0;

                if (currentTime > 10000000) // debug code used to find strange values
                {
                    int x2 = 1;
                }

                if (loopTimeStack.Count > 0)
                {
                    loopTime = loopTimeStack.Pop();
                    loopTime += currentTime;
                    loopTimeStack.Push(loopTime);

                    loopTicks = loopTickStack.Pop();
                    loopTicks += currentTicks;
                    loopTickStack.Push(loopTicks);
                }
                else
                {
                    if (pStream.Position != pStream.Length) // time at the end of the file is ignored
                    {
                        totalTime += currentTime;
                        totalTicks += (ulong)currentTicks;

                        timeSinceLastLoopEnd += currentTime;
                    }
                    else
                    {
                        goto DONE;
                    }

                }

                // get command
                currentByte = pStream.ReadByte();
                currentOffset = pStream.Position - 1;

                if (currentOffset > 0x1FCF) // code to quickly get to a position for debugging
                {
                    int x = 1;
                }

                //if ((currentTime != 0) && (currentTime != 38461) && (currentTime != 192307))
                //{
                //    int x3 = 1;
                //}


                if ((currentByte & 0x80) == 0) // data byte, we should be running
                {
                    if (runningCommand == 0)
                    {
                        throw new PsxSeqFormatException(String.Format("Empty running command at 0x{0}", currentOffset.ToString("X2")));
                    }
                    else
                    {
                        running = true;
                    }
                }
                else // new command
                {
                    runningCommand = currentByte;
                    running = false;

                    commandOffset = currentOffset;
                }

                dataByteCount = this.dataBytesPerCommand[runningCommand & 0xF0];

                if (dataByteCount == 0)
                {
                    // get meta command bytes
                    if (!running)
                    {
                        metaCommandByte = pStream.ReadByte();
                        currentOffset = pStream.Position - 1;
                    }
                    else
                    {
                        metaCommandByte = currentByte;
                    }
                    // check for tempo switch here
                    if (metaCommandByte == 0x51)
                    {
                        // tempo switch
                        tempoValBytes = new byte[4];
                        Array.Copy(ParseFile.parseSimpleOffset(pStream, pStream.Position, 3), 0, tempoValBytes, 1, 3);

                        Array.Reverse(tempoValBytes); // flip order to LE for use with BitConverter
                        tempo = BitConverter.ToUInt32(tempoValBytes, 0);

                        pStream.Position += 3;
                    }
                    else
                    {
                        // get length bytes                   
                        metaCommandLengthByte = pStream.ReadByte();

                        // skip data bytes, they have already been grabbed above if needed
                        pStream.Position += (long)metaCommandLengthByte;
                    }
                    currentOffset = pStream.Position;

                }
                else
                {
                    if (running)
                    {
                        dataByte1 = currentByte;
                    }
                    else
                    {
                        dataByte1 = pStream.ReadByte();
                        currentOffset = pStream.Position - 1;
                    }

                    if (dataByteCount == 2)
                    {
                        dataByte2 = pStream.ReadByte();
                        currentOffset = pStream.Position - 1;


                        // check for loop start
                        if ((((currentByte & 0xB0) == 0xB0) || ((runningCommand & 0xB0) == 0xB0)) &&
                             dataByte1 == 0x63 && dataByte2 == 0x14)
                        {
                            loopTimeStack.Push(0);
                            loopTickStack.Push(0);
                            loopsOpened++;
                        }

                        // check for loop end
                        if ((((currentByte & 0xB0) == 0xB0) || ((runningCommand & 0xB0) == 0xB0)) &&
                             dataByte1 == 0x63 && dataByte2 == 0x1E)
                        {
                            loopEndFound = true;
                            loopsClosed++;
                        }

                        if ((((currentByte & 0xB0) == 0xB0) || ((runningCommand & 0xB0) == 0xB0)) &&
                            dataByte1 == 0x06)
                        {
                            loopTimeMultiplier = dataByte2;
                        }

                        // check for loop count
                        if (loopEndFound)
                        {
                            if (this.force2Loops)
                            {
                                loopTimeMultiplier = 2;
                                loopFound = true;
                            }
                            else if (loopTimeMultiplier == 127)
                            {
                                loopTimeMultiplier = 2;
                                loopFound = true;
                            }

                            // add loop time
                            loopTime = loopTimeStack.Pop();
                            loopTime = (loopTime * loopTimeMultiplier);
                            totalTime += loopTime;

                            loopTicks = loopTickStack.Pop();
                            loopTicks = (loopTicks * (ulong)loopTimeMultiplier);
                            totalTicks += loopTicks;

                            timeSinceLastLoopEnd = 0;

                            loopEndFound = false;
                        }
                    }

                    currentOffset = pStream.Position - 1;
                }

            } // while (pStream.Position < pEndOffset)


DONE:       // Marker used for skipping delta ticks at the end of a file.

            // Not sure how to handle, but for now count each unclosed loop twice, 
            //   since it should be the outermost loop.            
            if (loopTimeStack.Count > 0)
            {
                ret.Warnings += "Unmatched Loop Start tag(s) found." + Environment.NewLine;

                while (loopTimeStack.Count > 0)
                {
                    totalTime += loopTimeStack.Pop() * 2d;
                    loopFound = true;
                }
            }

            if (loopsClosed > loopsOpened)
            {
                totalTime -= timeSinceLastLoopEnd;
            }

            ret.TimeInSeconds = ((totalTime) * Math.Pow(10, -6));

            if (loopFound)
            {
                ret.FadeInSeconds = 10; // looping
            }
            else
            {
                ret.FadeInSeconds = 1;  // non-looping
            }

            // return stream to incoming position
            pStream.Position = incomingStreamPosition;
            return ret;        
        }
    }

    public class PsxSeqFormatException : Exception
    {
        public PsxSeqFormatException(string pMessage)
            : base(pMessage) { }
    }
}
