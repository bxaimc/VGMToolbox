using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Text;

using VGMToolbox.format;
using VGMToolbox.plugin;
using VGMToolbox.util;

namespace VGMToolbox.tools.extract
{
    class ExtractOggWorker : AVgmtDragAndDropWorker, IVgmtBackgroundWorker
    {
        public struct ExtractOggStruct : IVgmtWorkerStruct
        {
            public string[] SourcePaths { set; get; }
            public bool DoStrictFormatValidation { set; get; }
        }

        public ExtractOggWorker() :
            base() { }

        protected override void DoTaskForFile(string pPath, IVgmtWorkerStruct pTaskStruct, DoWorkEventArgs e)
        {
            ExtractOggStruct extractOggStruct = (ExtractOggStruct)pTaskStruct;

            long offset = 0;

            byte pageType;
            long pageSize;
            uint bitstreamSerialNumber;
            
            byte segmentCount;
            uint sizeOfAllSegments;
            byte i;

            string outputPath;
            string outputFileName;
            bool pageWrittenToFile = false;

            Dictionary<uint, FileStream> outputStreams = new Dictionary<uint, FileStream>();

            using (FileStream fs = File.Open(pPath, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    outputPath = Path.Combine(Path.GetDirectoryName(pPath), String.Format("VGMT_EXTRACTED_OGGs", Path.GetFileNameWithoutExtension(pPath)));

                    while ((offset = ParseFile.GetNextOffset(fs, offset, XiphOrgOggContainer.MAGIC_BYTES)) > -1)
                    {
                        if (!this.CancellationPending)
                        {
                            pageWrittenToFile = false;
                            
                            //------------------
                            // get page details
                            //------------------
                            pageType = ParseFile.ParseSimpleOffset(fs, offset + 5, 1)[0];
                            bitstreamSerialNumber = BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(fs, offset + 0xE, 4), 0);
                            segmentCount = ParseFile.ParseSimpleOffset(fs, offset + 0x1A, 1)[0];

                            sizeOfAllSegments = 0;

                            for (i = 0; i < segmentCount; i++)
                            {
                                sizeOfAllSegments += ParseFile.ParseSimpleOffset(fs, offset + 0x1B + i, 1)[0];
                            }

                            pageSize = 0x1B + segmentCount + sizeOfAllSegments;

                            //-----------------------------
                            // write page
                            //-----------------------------
                            //if (pageType > 0)
                            //{
                                if ((pageType & XiphOrgOggContainer.PAGE_TYPE_BEGIN_STREAM) ==
                                    XiphOrgOggContainer.PAGE_TYPE_BEGIN_STREAM)
                                {
                                    // create the output stream
                                    outputFileName = Path.Combine(outputPath, String.Format("{0}_{1}.ogg", Path.GetFileNameWithoutExtension(pPath), bitstreamSerialNumber.ToString("X8")));

                                    if (!Directory.Exists(outputPath))
                                    {
                                        Directory.CreateDirectory(outputPath);
                                    }

                                    outputStreams[bitstreamSerialNumber] = File.Open(outputFileName, FileMode.CreateNew, FileAccess.Write);
                                    outputStreams[bitstreamSerialNumber].Write(ParseFile.ParseSimpleOffset(fs, offset, (int)pageSize), 0, (int)pageSize);
                                    pageWrittenToFile = true;
                                }
                                
                                //if ((pageType & XiphOrgOggContainer.PAGE_TYPE_CONTINUATION) ==
                                //    XiphOrgOggContainer.PAGE_TYPE_CONTINUATION)
                                //{
                                    if (outputStreams.ContainsKey(bitstreamSerialNumber) && !pageWrittenToFile)
                                    {
                                        outputStreams[bitstreamSerialNumber].Write(ParseFile.ParseSimpleOffset(fs, offset, (int)pageSize), 0, (int)pageSize);
                                        pageWrittenToFile = true;
                                    }
                                //}
                                

                                // close stream if needed
                                if ((pageType & XiphOrgOggContainer.PAGE_TYPE_END_STREAM) ==
                                        XiphOrgOggContainer.PAGE_TYPE_END_STREAM)
                                {
                                    if (outputStreams.ContainsKey(bitstreamSerialNumber))
                                    {
                                        if (!pageWrittenToFile)
                                        {
                                            outputStreams[bitstreamSerialNumber].Write(ParseFile.ParseSimpleOffset(fs, offset, (int)pageSize), 0, (int)pageSize);
                                            pageWrittenToFile = true;
                                        }

                                        outputStreams[bitstreamSerialNumber].Close();
                                        outputStreams[bitstreamSerialNumber].Dispose();

                                        // remove stream from dictionary
                                        outputStreams.Remove(bitstreamSerialNumber);
                                    }
                                }
                                //} if (pageType > 0)
                            
                            offset += pageSize;
                        }
                        else
                        {
                            e.Cancel = true;
                            return;
                        }
                    } // while
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    // close any unfinished streams
                    foreach (uint k in outputStreams.Keys)
                    {
                        outputStreams[k].Close();
                        outputStreams[k].Dispose();
                    }
                }
            }
        }
    }
}
