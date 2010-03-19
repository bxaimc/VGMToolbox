using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

using VGMToolbox.format;
using VGMToolbox.plugin;
using VGMToolbox.util;

namespace VGMToolbox.tools.extract
{
    class ExtractSonyAdpcmWorker : AVgmtDragAndDropWorker, IVgmtBackgroundWorker
    {
        public struct ExtractSonyAdpcmStruct : IVgmtWorkerStruct
        {
            public string[] SourcePaths { set; get; }
        }

        public ExtractSonyAdpcmWorker() :
            base() { }

        protected override void DoTaskForFile(string pPath, IVgmtWorkerStruct pExtractSonyAdpcmStruct, DoWorkEventArgs e)
        {
            ExtractSonyAdpcmStruct extractSonyAdpcmStruct = (ExtractSonyAdpcmStruct)pExtractSonyAdpcmStruct;

            long offset = 0;
            ArrayList adpcmList = new ArrayList();
            Psf.ProbableItemStruct adpcmDataItem = new Psf.ProbableItemStruct();

            int fileCount = 0;
            string outputPath = Path.Combine(Path.GetDirectoryName(pPath), "_sony_adpcm_ext");
            string outputFileName;
            string outputFilePath;
            
            FileInfo fi = new FileInfo(pPath);            

            using (FileStream fs = File.Open(pPath, FileMode.Open, FileAccess.Read))
            {
                    while (offset < fi.Length)
                    { 
                        if (!CancellationPending)
                        {
                            if (Psf.IsPotentialAdpcm(fs, offset, Psf.MIN_ADPCM_ROW_SIZE))
                            {
                                // create probable adpcm item
                                adpcmDataItem.Init();
                                
                                // set starting offset
                                adpcmDataItem.offset = offset;
                            
                                // move to next row
                                offset += Psf.SONY_ADPCM_ROW_SIZE;

                                // loop until end
                                while (Psf.IsSonyAdpcmRow(fs, offset))
                                {
                                    offset += Psf.SONY_ADPCM_ROW_SIZE;
                                }

                                adpcmDataItem.length = (uint)(offset - adpcmDataItem.offset);
                                adpcmList.Add(adpcmDataItem);
                            }

                            offset += 1;
                        }
                        else
                        {
                            e.Cancel = true;
                            return;
                        }
                    }


                    // extract files
                    foreach (Psf.ProbableItemStruct p in adpcmList)
                    {
                        outputFileName = String.Format("{0}_{1}.bin", Path.GetFileNameWithoutExtension(pPath), fileCount.ToString("X8"));
                        outputFilePath = Path.Combine(outputPath, outputFileName);

                        ParseFile.ExtractChunkToFile(fs, p.offset, (int)p.length, outputFilePath, true, true);

                        fileCount++;
                    }
            }
        }

    }
}
