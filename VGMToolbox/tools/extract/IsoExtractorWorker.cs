using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

using VGMToolbox.format.iso;
using VGMToolbox.plugin;
using VGMToolbox.util;

namespace VGMToolbox.tools.extract
{
    class IsoExtractorWorker : AVgmtDragAndDropWorker, IVgmtBackgroundWorker
    {
        public static int MAX_ID_BYTES_LENGTH = 0x14;
        public static readonly byte[] SYNC_BYTES =
            new byte[] { 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 
                         0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00 };

        public enum IsoFormatType
        {
            Iso9660,
            XDvdFs
        };
        
        public struct IsoExtractorStruct : IVgmtWorkerStruct
        {
            public string[] SourcePaths { set; get; }
            public string DestinationFolder;

            public IsoFormatType IsoFormat { set; get; }

            public IFileStructure[] Files { set; get; }
            public IDirectoryStructure[] Directories { set; get; }
        }
        
        public IsoExtractorWorker() :
            base() { }

        protected override void DoTaskForFile(string pPath, IVgmtWorkerStruct pTaskStruct, DoWorkEventArgs e)
        {
            IsoExtractorStruct taskStruct = (IsoExtractorStruct)pTaskStruct;

            using (FileStream fs = File.OpenRead(pPath))
            {
                foreach (IDirectoryStructure d in taskStruct.Directories)
                {
                    if (!CancellationPending)
                    {
                        d.Extract(fs, taskStruct.DestinationFolder);
                    }
                    else
                    {
                        e.Cancel = false;
                        break;
                    }
                }

                foreach (IFileStructure f in taskStruct.Files)
                {
                    if (!CancellationPending)
                    {
                        f.Extract(fs, taskStruct.DestinationFolder);
                    }
                    else
                    {
                        e.Cancel = false;
                        break;
                    }
                }

            }
        }

        public static IVolume[] GetVolumes(string pPath)
        {
            ArrayList volumeList = new ArrayList();

            using (FileStream fs = File.OpenRead(pPath))
            {
                long currentOffset = 0;
                long fileSize = fs.Length;
                byte[] volumeIdBytes;
                bool isRawFormat = false;
                
                //----------------------
                // check for sync bytes
                //----------------------
                byte[] syncCheckBytes = ParseFile.ParseSimpleOffset(fs, 0, SYNC_BYTES.Length);

                if (ParseFile.CompareSegment(syncCheckBytes, 0, SYNC_BYTES))
                {
                    isRawFormat = true;

                    throw new FormatException("RAW Dumps not currently supported.");
                }

                while (currentOffset < fileSize)
                {
                    volumeIdBytes = ParseFile.ParseSimpleOffset(fs, currentOffset, MAX_ID_BYTES_LENGTH);

                    //----------
                    // ISO 9660
                    //----------
                    if (ParseFile.CompareSegmentUsingSourceOffset(volumeIdBytes, 0, Iso9660.VOLUME_DESCRIPTOR_IDENTIFIER.Length, Iso9660.VOLUME_DESCRIPTOR_IDENTIFIER))
                    {
                        Iso9660Volume isoVolume;
                        isoVolume = new Iso9660Volume();
                        isoVolume.Initialize(fs, currentOffset);
                        volumeList.Add((IVolume)isoVolume);

                        if ((isoVolume.Directories.Length == 1) &&
                            (isoVolume.Directories[0].SubDirectories.Length == 0) &&
                            (isoVolume.Directories[0].Files.Length == 0))
                        {
                            // possible empty/dummy volume (XBOX)
                            currentOffset += 0x800;
                        }
                        else
                        {
                            currentOffset = isoVolume.VolumeBaseOffset + ((long)isoVolume.VolumeSpaceSize * (long)isoVolume.LogicalBlockSize);
                        }

                    }
                    
                    //-----------------------
                    // XDVDFS (XBOX/XBOX360)
                    //-----------------------
                    else if (ParseFile.CompareSegmentUsingSourceOffset(volumeIdBytes, 0, XDvdFs.STANDARD_IDENTIFIER.Length, XDvdFs.STANDARD_IDENTIFIER))
                    {
                        XDvdFsVolume isoVolume;
                        isoVolume = new XDvdFsVolume();
                        isoVolume.Initialize(fs, currentOffset);
                        volumeList.Add((IVolume)isoVolume);

                        // XDVDFS should be the last volume
                        break;
                    }
                    else
                    {
                        currentOffset += 0x800;
                    }
                }
            }

            return (IVolume[])volumeList.ToArray(typeof(IVolume));
        }
    }
}
