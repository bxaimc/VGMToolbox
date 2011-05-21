using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VGMToolbox.format.iso
{
    public class NullDcGdi
    {
        public enum TrackType
        {
            Audio,
            Data
        };
        
        public struct TrackEntry
        {
            public uint TrackNumber { set; get; }
            public uint StartSector { set; get; }
            public TrackType EntryType { set; get; }
            public uint SectorSize { set; get; }
            public string FilePath { set; get; }
            public long Offset { set; get; }
        }
        
        public static string FORMAT_DESCRIPTION_STRING = "Dreamcast GDI";
        public const string GDI_CTRL_AUDIO = "0";
        public const string GDI_CTRL_DATA = "4";

        public int TrackCount { set; get; }
        public TrackEntry[] TrackEntries { set; get; }

        public NullDcGdi(string gdiPath)
        {
            try
            {
                this.ParseGdiFile(gdiPath);
            }
            catch (Exception ex)
            {
                throw new FileLoadException(String.Format("Error loading GDI: {0}.", ex.Message));
            }
        }

        public string GetFilePathForLba(uint sectorLba)
        {
            string filePath = String.Empty;

            for (int i = (this.TrackCount - 1); i > 0; i--)
            {
                if (sectorLba >= this.TrackEntries[i].StartSector)
                {
                    filePath = this.TrackEntries[i].FilePath;
                    break;
                }
            }
            
            return filePath;
        }
        
        private void ParseGdiFile(string gdiPath)
        {
            string trackCount;
            string trackEntryLine;
            string[] trackEntryLineArray;
            string[] splitDelimeters = new string[] { " " };

            using (FileStream gdiStream = File.OpenRead(gdiPath))
            {
                using (StreamReader gdiReader = new StreamReader(gdiStream))
                { 
                    // get track count
                    trackCount = gdiReader.ReadLine().Trim();
                    this.TrackCount = int.Parse(trackCount);

                    // populate track entries
                    this.TrackEntries = new TrackEntry[this.TrackCount];

                    for (int i = 0; i < this.TrackCount; i++)
                    {
                        trackEntryLine = gdiReader.ReadLine().Trim();
                        trackEntryLineArray = trackEntryLine.Split(splitDelimeters, StringSplitOptions.RemoveEmptyEntries);

                        this.TrackEntries[i] = new TrackEntry();
                        this.TrackEntries[i].TrackNumber = uint.Parse(trackEntryLineArray[0].Trim());
                        this.TrackEntries[i].StartSector = uint.Parse(trackEntryLineArray[1].Trim());
                        
                        switch (trackEntryLineArray[2].Trim())
                        {
                            case NullDcGdi.GDI_CTRL_AUDIO:
                                this.TrackEntries[i].EntryType = TrackType.Audio;
                                break;
                            case NullDcGdi.GDI_CTRL_DATA:
                                this.TrackEntries[i].EntryType = TrackType.Data;
                                break;
                        }

                        this.TrackEntries[i].SectorSize = uint.Parse(trackEntryLineArray[3].Trim());
                        this.TrackEntries[i].FilePath = Path.Combine(Path.GetDirectoryName(gdiPath), trackEntryLineArray[4].Trim());
                        this.TrackEntries[i].Offset = long.Parse(trackEntryLineArray[5].Trim());

                        // verify file exists
                        if (!File.Exists(this.TrackEntries[i].FilePath))
                        {
                            throw new FileNotFoundException(String.Format("Cannot find file referenced by GDI: {0}", this.TrackEntries[i].FilePath));
                        }
                    }
                }
            }
        }
    }
}
