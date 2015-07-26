using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using VGMToolbox.util;

namespace VGMToolbox.format
{
    public class CriAfs2File
    {
        public ushort CueId { set; get; }
        public long FileOffsetRaw { set; get; }
        public long FileOffsetByteAligned { set; get; }
        public long FileLength { set; get; }

        // public string FileName { set; get; } // comes from ACB for now, maybe included in archive later?
    }
    
    public class CriAfs2Archive
    {
        public static readonly byte[] SIGNATURE = new byte[] { 0x41, 0x46, 0x53, 0x32 };

        public string SourceFile { set; get; }
        public byte[] MagicBytes { set; get; }
        public byte[] Version { set; get; }
        public uint FileCount { set; get; }
        public uint ByteAlignment { set; get; }
        public Dictionary <ushort, CriAfs2File> Files { set; get; }

        public CriAfs2Archive(FileStream fs, long offset)
        {
            if (IsCriAfs2Archive(fs, offset))
            {
                this.SourceFile = fs.Name;
                long afs2FileSize = fs.Length;
                
                this.MagicBytes = ParseFile.ParseSimpleOffset(fs, offset, SIGNATURE.Length);
                this.Version = ParseFile.ParseSimpleOffset(fs, offset + 4, 4);
                this.FileCount = ParseFile.ReadUintLE(fs, offset + 8);

                if (this.FileCount > ushort.MaxValue)
                {
                    throw new FormatException(String.Format("ERROR, file count exceeds max value for ushort.  Please report this at official feedback forums (see 'Other' menu item).", fs.Name));
                }
                
                this.ByteAlignment = ParseFile.ReadUintLE(fs, offset + 0xC);
                this.Files = new Dictionary<ushort, CriAfs2File>((int)this.FileCount);

                CriAfs2File dummy;

                for (ushort i = 0; i < this.FileCount; i++)
                {
                    dummy = new CriAfs2File();

                    dummy.CueId = ParseFile.ReadUshortLE(fs, (0x10 + (2 * i)));
                    dummy.FileOffsetRaw = ParseFile.ReadUintLE(fs, (0x10 + (this.FileCount * 2) + (4 * i)));

                    // set file offset to byte alignment
                    if ((dummy.FileOffsetRaw % this.ByteAlignment) != 0)
                    {
                        dummy.FileOffsetByteAligned = MathUtil.RoundUpToByteAlignment(dummy.FileOffsetRaw, this.ByteAlignment);
                    }
                    else
                    {
                        dummy.FileOffsetByteAligned = dummy.FileOffsetRaw;
                    }

                    // set file size
                    if (i > 0)
                    {
                        // last file will use EOF
                        if (i == this.FileCount - 1)
                        {
                            dummy.FileLength = afs2FileSize - dummy.FileOffsetByteAligned;
                        }
                        
                        // set length for previos entry
                        this.Files[(ushort)(i - 1)].FileLength = dummy.FileOffsetRaw - this.Files[(ushort)(i - 1)].FileOffsetByteAligned;                        

                    } // if (i > 0)

                    this.Files.Add(dummy.CueId, dummy);
                } // for (uint i = 0; i < this.FileCount; i++)

            }
            else
            {
                throw new FormatException(String.Format("AFS2 magic bytes not found at offset: 0x{0}.", offset.ToString("X8")));
            }        
        }

        public static bool IsCriAfs2Archive(FileStream fs, long offset)
        {
            bool ret = false;
            byte[] checkBytes = ParseFile.ParseSimpleOffset(fs, offset, SIGNATURE.Length);

            if (ParseFile.CompareSegment(checkBytes, 0, SIGNATURE))
            {
                ret = true;

            }

            return ret;        
        }

        // used when matching ACB to AWB
        public void SortFilesByCueNumber()
        { 
        
        }

    
    
    }
}
