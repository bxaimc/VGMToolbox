using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

using CueSharp;

using VGMToolbox.format.iso;
using VGMToolbox.plugin;
using VGMToolbox.util;

namespace VGMToolbox.tools.extract
{
    class IsoExtractorWorker : AVgmtDragAndDropWorker, IVgmtBackgroundWorker
    {
        public static int MAX_ID_BYTES_LENGTH = 0x100;

        public enum IsoFormatType
        {
            Iso9660,
            XDvdFs,
            Panasonic3do
        };
        
        public struct IsoExtractorStruct : IVgmtWorkerStruct
        {
            public string[] SourcePaths { set; get; }
            public string DestinationFolder;

            public IsoFormatType IsoFormat { set; get; }
            public bool ExtractAsRaw { set; get; }

            public IFileStructure[] Files { set; get; }
            public IDirectoryStructure[] Directories { set; get; }
        }
        
        public IsoExtractorWorker() :
            base() 
        {
            this.progressCounterIncrementer = 10;
        }

        protected override void DoTaskForFile(string pPath, IVgmtWorkerStruct pTaskStruct, DoWorkEventArgs e)
        {
            IsoExtractorStruct taskStruct = (IsoExtractorStruct)pTaskStruct;

            using (FileStream fs = File.OpenRead(pPath))
            {
                foreach (IDirectoryStructure d in taskStruct.Directories)
                {
                    if (!CancellationPending)
                    {
                        d.Extract(fs, taskStruct.DestinationFolder, taskStruct.ExtractAsRaw);
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
                        f.Extract(fs, taskStruct.DestinationFolder, taskStruct.ExtractAsRaw);
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

            // check for .cue sheet style entries first
            if (pPath.ToUpper().EndsWith(".CUE"))
            {
                Track cueTrack;
                string basePath = String.Empty;
                string trackPath;

                CueSheet cueSheet = new CueSheet(pPath);

                for (int i = 0; i < cueSheet.Tracks.Length; i++)
                {
                    cueTrack = cueSheet.Tracks[i];
                    basePath = String.IsNullOrEmpty(cueTrack.DataFile.Filename) ? basePath : cueTrack.DataFile.Filename;
                    trackPath = Path.Combine(Path.GetDirectoryName(pPath), basePath);

                    // Add Data Type Entries
                    if (cueTrack.TrackDataType != DataType.AUDIO)
                    {
                        IVolume[] dataVolumes = GetDataVolumes(trackPath, 0);

                        for (int j = 0; j < dataVolumes.Length; j++)
                        {
                            volumeList.Add(dataVolumes[j]);
                        }
                    }
                    
                    // Add Audio Entries
                    else
                    {
                    
                    }
                }
            }
            else if (pPath.ToUpper().EndsWith(".GDI"))
            {
                //------------------
                // NULLDC GDI IMAGE
                //------------------
                NullDcGdi dcDisc = new NullDcGdi(pPath);

            }
            else
            {
                IVolume[] dataVolumes = GetDataVolumes(pPath, 0);

                for (int i = 0; i < dataVolumes.Length; i++)
                {
                    volumeList.Add(dataVolumes[i]);
                }
            }
            
            return (IVolume[])volumeList.ToArray(typeof(IVolume));
        }

        public static IVolume[] GetDataVolumes(string pPath, long startingOffset)
        {
            ArrayList volumeList = new ArrayList();

            // read the file, scanning for identifying bytes
            using (FileStream fs = File.OpenRead(pPath))
            {
                long currentOffset = startingOffset;
                long fileSize = fs.Length;

                byte[] sectorBytes;
                byte[] sectorDataBytes;
                byte[] volumeIdBytes;

                bool isRawFormat = false;
                int sectorSize = 0x800;
                CdSectorType mode;

                //----------------------
                // check for sync bytes
                //----------------------
                byte[] syncCheckBytes = ParseFile.ParseSimpleOffset(fs, 0, CdRom.SYNC_BYTES.Length);

                if (ParseFile.CompareSegment(syncCheckBytes, 0, CdRom.SYNC_BYTES))
                {
                    isRawFormat = true;
                    sectorSize = 0x930;
                }

                while (currentOffset < fileSize)
                {
                    // get sector
                    sectorBytes = ParseFile.ParseSimpleOffset(fs, currentOffset, sectorSize);

                    // get data bytes from sector
                    sectorDataBytes = CdRom.GetDataChunkFromSector(sectorBytes, isRawFormat);

                    // get header bytes
                    volumeIdBytes = ParseFile.ParseSimpleOffset(sectorDataBytes, 0, MAX_ID_BYTES_LENGTH);

                    //----------
                    // ISO 9660
                    //----------
                    if (ParseFile.CompareSegmentUsingSourceOffset(volumeIdBytes, 0, Iso9660.VOLUME_DESCRIPTOR_IDENTIFIER.Length, Iso9660.VOLUME_DESCRIPTOR_IDENTIFIER))
                    {
                        Iso9660Volume isoVolume;
                        isoVolume = new Iso9660Volume();
                        isoVolume.Initialize(fs, currentOffset, isRawFormat);
                        volumeList.Add((IVolume)isoVolume);

                        if ((isoVolume.Directories.Length == 1) &&
                            (isoVolume.Directories[0].SubDirectories.Length == 0) &&
                            (isoVolume.Directories[0].Files.Length == 0))
                        {
                            // possible empty/dummy volume (XBOX)
                            currentOffset += sectorSize;
                        }
                        else
                        {
                            currentOffset = isoVolume.VolumeBaseOffset + ((long)isoVolume.VolumeSpaceSize * (long)sectorSize);
                        }
                    }
                    //-----------------
                    // Green Book CD-I
                    //-----------------
                    else if (ParseFile.CompareSegmentUsingSourceOffset(volumeIdBytes, 0, GreenBookCdi.VOLUME_DESCRIPTOR_IDENTIFIER.Length, GreenBookCdi.VOLUME_DESCRIPTOR_IDENTIFIER))
                    {
                        GreenBookCdiVolume isoVolume;
                        isoVolume = new GreenBookCdiVolume();
                        isoVolume.Initialize(fs, currentOffset, isRawFormat);
                        volumeList.Add((IVolume)isoVolume);

                        break;
                    }

                    //-----------------------
                    // XDVDFS (XBOX/XBOX360)
                    //-----------------------
                    else if (ParseFile.CompareSegmentUsingSourceOffset(volumeIdBytes, 0, XDvdFs.STANDARD_IDENTIFIER.Length, XDvdFs.STANDARD_IDENTIFIER))
                    {
                        if (isRawFormat)
                        {
                            throw new FormatException("Raw dumps not supported for XDVDFS (XBOX/XBOX360) format.");
                        }
                        else
                        {
                            XDvdFsVolume isoVolume;
                            isoVolume = new XDvdFsVolume();
                            isoVolume.Initialize(fs, currentOffset, isRawFormat);
                            volumeList.Add((IVolume)isoVolume);

                            // XDVDFS should be the last volume
                            break;
                        }
                    }

                    //---------------
                    // PANASONIC 3DO
                    //---------------
                    else if (ParseFile.CompareSegmentUsingSourceOffset(volumeIdBytes, 0, Panasonic3do.STANDARD_IDENTIFIER.Length, Panasonic3do.STANDARD_IDENTIFIER))
                    {
                        Panasonic3doVolume isoVolume;
                        isoVolume = new Panasonic3doVolume();
                        isoVolume.Initialize(fs, currentOffset, isRawFormat);
                        volumeList.Add((IVolume)isoVolume);

                        // should be the last volume
                        break;
                    }

                    //-------------------
                    // NINTENDO GAMECUBE
                    //-------------------
                    else if (ParseFile.CompareSegmentUsingSourceOffset(volumeIdBytes, (int)NintendoGameCube.IDENTIFIER_OFFSET, NintendoGameCube.STANDARD_IDENTIFIER.Length, NintendoGameCube.STANDARD_IDENTIFIER))
                    {
                        NintendoGameCubeVolume isoVolume;
                        isoVolume = new NintendoGameCubeVolume();
                        isoVolume.Initialize(fs, currentOffset, isRawFormat);
                        volumeList.Add((IVolume)isoVolume);

                        // should be the last volume
                        break;
                    }

                    //--------------
                    // NINTENDO WII
                    //--------------
                    else if (ParseFile.CompareSegmentUsingSourceOffset(volumeIdBytes, (int)NintendoWiiOpticalDisc.IDENTIFIER_OFFSET, NintendoWiiOpticalDisc.STANDARD_IDENTIFIER.Length, NintendoWiiOpticalDisc.STANDARD_IDENTIFIER))
                    {
                        if (ParseFile.CompareSegmentUsingSourceOffset(volumeIdBytes, 0x60, 1, Constants.NullByteArray))
                        {
                            NintendoWiiOpticalDisc wiiDisc = new NintendoWiiOpticalDisc();
                            wiiDisc.Initialize(fs, currentOffset, isRawFormat);

                            foreach (NintendoWiiOpticalDiscVolume isoVolume in wiiDisc.Volumes)
                            {
                                volumeList.Add((IVolume)isoVolume);
                            }

                            break;
                        }
                        else // Decrypted WII ISO
                        {
                            NintendoGameCubeVolume isoVolume;
                            isoVolume = new NintendoGameCubeVolume();
                            isoVolume.Initialize(fs, currentOffset, isRawFormat);
                            volumeList.Add((IVolume)isoVolume);
                        }
                        // should be the last volume
                        break;
                    }

                    else
                    {
                        currentOffset += sectorSize;
                    }
                }
            }

            return (IVolume[])volumeList.ToArray(typeof(IVolume));
        }
    }
}
