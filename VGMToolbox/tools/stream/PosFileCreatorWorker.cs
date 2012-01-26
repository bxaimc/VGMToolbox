using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

using VGMToolbox.plugin;
using VGMToolbox.util;

namespace VGMToolbox.tools.stream
{
    public class PosFileCreatorWorker : AVgmtDragAndDropWorker, IVgmtBackgroundWorker
    {
        private const long DEFAULT_LOOP_VALUE = -99887766;

        public const string FILEMASK_BASE_NAME = "$B";
        public const string FILEMASK_EXTENSION = "$E";
        
        public struct PosFileCreatorStruct : IVgmtWorkerStruct
        {
            public string[] SourcePaths { set; get; }
            public string OutputFolder { set; get; }
            public string OutputFileMask { set; get; }

            // Loop Start
            public bool DoLoopStartStatic { set; get; }
            public bool DoLoopStartOffset { set; get; }
            public bool DoLoopStartRiffOffset { set; get; }

            public string LoopStartStaticValue { set; get; }
            public CalculatingOffsetDescription LoopStartCalculatingOffset { set; get; }
            public RiffCalculatingOffsetDescription LoopStartRiffCalculatingOffset { set; get; }

            // Loop End
            public bool LoopEndIsLoopEnd { set; get; }
            public bool LoopEndIsLoopLength { set; get; }
            
            public bool DoLoopEndStatic { set; get; }
            public bool DoLoopEndOffset { set; get; }
            public bool DoLoopEndRiffOffset { set; get; }

            public string LoopEndStaticValue { set; get; }
            public CalculatingOffsetDescription LoopEndCalculatingOffset { set; get; }
            public RiffCalculatingOffsetDescription LoopEndRiffCalculatingOffset { set; get; }

            // Loop Shift
            public bool DoStaticLoopShift { set; get; }
            public bool DoLoopShiftWavCompare { set; get; }
            public bool PredictLoopShiftForBatch { set; get; }
            public string StaticLoopShiftValue { set; get; }

        }

        public struct WorkingItemStruct
        {
            public long LoopStart { set; get; }
            public long LoopEnd { set; get; }

            public long LoopShift { set; get; }
            public long WavSamples { set; get; }
            public long SampleDifference { set; get; }            
        }



        public PosFileCreatorWorker() : base() { }

        private Dictionary<string, WorkingItemStruct> WorkingList = new Dictionary<string, WorkingItemStruct>();

        // DoTaskForFile
        protected override void DoTaskForFile(string pPath, IVgmtWorkerStruct pPosFileCreatorStruct,
            DoWorkEventArgs e)
        { 
            PosFileCreatorStruct posStruct = (PosFileCreatorStruct) pPosFileCreatorStruct;

            WorkingItemStruct currentItem = new WorkingItemStruct();
            currentItem.LoopShift = DEFAULT_LOOP_VALUE;
            currentItem.WavSamples = DEFAULT_LOOP_VALUE;
            currentItem.SampleDifference = DEFAULT_LOOP_VALUE;

            long loopStartValue = DEFAULT_LOOP_VALUE;
            long loopEndValue = DEFAULT_LOOP_VALUE;

            string outputFileMask;
            string[] outputFileList;

            outputFileMask = posStruct.OutputFileMask.Replace(FILEMASK_BASE_NAME, Path.GetFileNameWithoutExtension(pPath));
            outputFileMask = outputFileMask.Replace(FILEMASK_EXTENSION, Path.GetExtension(pPath).Remove(0, 1));

            #region LOOP POINTS
            using (FileStream fs = File.OpenRead(pPath))
            {
                //----------------
                // Get Loop Start
                //----------------
                if (posStruct.DoLoopStartStatic)
                {
                    loopStartValue = ByteConversion.GetLongValueFromString(posStruct.LoopStartStaticValue);
                }
                else if (posStruct.DoLoopStartOffset)
                {
                    loopStartValue = ParseFile.GetVaryingByteValueAtAbsoluteOffset(fs, posStruct.LoopStartCalculatingOffset, true);
                }
                else if (posStruct.DoLoopStartRiffOffset)
                {
                    loopStartValue = ParseFile.GetRiffCalculatedVaryingByteValueAtAbsoluteOffset(fs, posStruct.LoopStartRiffCalculatingOffset, true);
                }

                if (loopStartValue != DEFAULT_LOOP_VALUE)
                {
                    //----------------
                    // Get Loop End
                    //----------------
                    if (posStruct.DoLoopEndStatic)
                    {
                        loopEndValue = ByteConversion.GetLongValueFromString(posStruct.LoopEndStaticValue);
                    }
                    else if (posStruct.DoLoopEndOffset)
                    {
                        loopEndValue = ParseFile.GetVaryingByteValueAtAbsoluteOffset(fs, posStruct.LoopEndCalculatingOffset, true);
                    }
                    else if (posStruct.DoLoopEndRiffOffset)
                    {
                        loopEndValue = ParseFile.GetRiffCalculatedVaryingByteValueAtAbsoluteOffset(fs, posStruct.LoopEndRiffCalculatingOffset, true);
                    }

                    if (loopEndValue != DEFAULT_LOOP_VALUE)
                    {
                        // Calculate Loop End if Needed
                        if (posStruct.LoopEndIsLoopLength)
                        {
                            loopEndValue += loopStartValue;
                        }

                        // update working item
                        currentItem.LoopStart = loopStartValue;
                        currentItem.LoopEnd = loopEndValue;
                    }
                    else
                    {
                        throw new IndexOutOfRangeException(String.Format("Loop End Value not Found: {0}", pPath));
                    }
                
                }
                else
                {
                    throw new IndexOutOfRangeException(String.Format("Loop Start Value not Found: {0}", pPath));
                }
            }
            #endregion

            #region LOOP SHIFT
            if ((loopStartValue != DEFAULT_LOOP_VALUE) && (loopEndValue != DEFAULT_LOOP_VALUE))
            {
                // get list of matching WAV files
                outputFileList = Directory.GetFiles(Path.GetDirectoryName(pPath), outputFileMask, SearchOption.TopDirectoryOnly);
                
                // loop over list and add each item to the dictionary
                foreach (string outputFile in outputFileList)
                {
                    // Static Loop Shift
                    if (posStruct.DoStaticLoopShift)
                    {
                        // update shift
                        currentItem.LoopShift = ByteConversion.GetLongValueFromString(posStruct.StaticLoopShiftValue);
                    }
                    else if (posStruct.DoLoopShiftWavCompare) // Wav Compare
                    {
                        // get samplecount for this file
                        currentItem.WavSamples = GetSampleCountForRiffHeaderedFile(outputFile);
                        currentItem.SampleDifference = currentItem.LoopEnd - currentItem.WavSamples;

                        // update shift
                        currentItem.LoopShift = currentItem.SampleDifference;
                    }

                    this.WorkingList.Add(outputFile, currentItem);
                }
                
                
                
                
                               


            }
            
            #endregion

        }

        public static long GetSampleCountForRiffHeaderedFile(string path)
        {
            long sampleCount = -1;

            ushort channelCount;
            uint dataSize;

            RiffCalculatingOffsetDescription riffValue = new RiffCalculatingOffsetDescription();

            // get values from file
            using (FileStream fs = File.OpenRead(path))
            {
                // over kill, but i don't feel like recoding it
                riffValue.CalculationString = String.Empty;
                riffValue.OffsetByteOrder = Constants.LittleEndianByteOrder;
                riffValue.OffsetSize = "2";
                riffValue.OffsetValue = "10";
                riffValue.RelativeLocationToRiffChunkString = RiffCalculatingOffsetDescription.START_OF_STRING;
                riffValue.RiffChunkString = "fmt ";

                channelCount = (ushort)ParseFile.GetRiffCalculatedVaryingByteValueAtAbsoluteOffset(fs, riffValue, true);

                riffValue.CalculationString = String.Empty;
                riffValue.OffsetByteOrder = Constants.LittleEndianByteOrder;
                riffValue.OffsetSize = "4";
                riffValue.OffsetValue = "4";
                riffValue.RelativeLocationToRiffChunkString = RiffCalculatingOffsetDescription.START_OF_STRING;
                riffValue.RiffChunkString = "data";

                dataSize = (uint)ParseFile.GetRiffCalculatedVaryingByteValueAtAbsoluteOffset(fs, riffValue, true);

                sampleCount = (long)dataSize / 2 / (long)channelCount;

            } // using (FileStream fs = File.OpenRead(path))
                        
            return sampleCount;
        }
    }
}
