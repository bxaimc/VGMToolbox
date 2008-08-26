using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

using VGMToolbox.util;

namespace VGMToolbox.extractors
{
    class Flx
    {
        struct SubFileInfo
        {
            public int subFileOffset;
            public int subFileLength;

            public SubFileInfo(byte[] pBytes)
            {
                subFileOffset = BitConverter.ToInt32(ParseFile.parseSimpleOffset(pBytes, 0, 4), 0);
                subFileLength = BitConverter.ToInt32(ParseFile.parseSimpleOffset(pBytes, 4, 4), 0);
            }
        }

        static readonly int commentOffset = 0x00;
        static readonly int numberOfFilesOffset = 0x50;
        static readonly int fileSizeOffset = 0x58;
        static readonly int subFileOffset = 0x80;

        static readonly int commentLength = 0x50;
        static readonly int numberOfFilesLength = 0x04;
        static readonly int fileSizeLength = 0x04;
        static readonly int subFileLength = 0x08;

        byte[] comment = new byte[commentLength];
        int numberOfFiles;
        int fileSize;

        SubFileInfo[] subFiles;

        public Flx(string pFileName)
        {
            int subFileIndex = 0;
            
            FileStream fs = new FileStream(pFileName, FileMode.Open, FileAccess.Read);

            comment = ParseFile.parseSimpleOffset(fs, commentOffset, commentLength);
            numberOfFiles = BitConverter.ToInt32(ParseFile.parseSimpleOffset(fs, numberOfFilesOffset, numberOfFilesLength), 0);
            fileSize = BitConverter.ToInt32(ParseFile.parseSimpleOffset(fs, fileSizeOffset, fileSizeLength), 0);

            
            subFiles = new SubFileInfo[numberOfFiles];

            subFileIndex = subFileOffset;
            for (int i = 0; i < numberOfFiles; i++)
            {
                subFiles[i] = new SubFileInfo(ParseFile.parseSimpleOffset(fs, subFileIndex, subFileLength));                
                subFileIndex += subFileLength;
            }


            fs.Close();
            fs.Dispose();
        }

        public string GetInfo()
        {
            System.Text.Encoding enc = System.Text.Encoding.ASCII;
            string ret = String.Empty;

            ret += "Comment: " + enc.GetString(FileUtil.ReplaceNullByteWithSpace(comment)).Trim() + Environment.NewLine;
            ret += "Number of Files: " + numberOfFiles + Environment.NewLine;
            ret += "File Size: " + fileSize + Environment.NewLine;

            ret += Environment.NewLine;

            for (int i = 0; i < numberOfFiles; i++)
            {
                ret += "SubFile " + i + ": " + Environment.NewLine;
                ret += "  offset: " + subFiles[i].subFileOffset + Environment.NewLine;
                ret += "  length: " + subFiles[i].subFileLength + Environment.NewLine;
                ret += Environment.NewLine;
            }

            return ret;
        }

        public void ExtractFiles(string pSourceFile, string pOutputDir)
        {
            BinaryWriter bw;
            FileStream fs = new FileStream(pSourceFile, FileMode.Open, FileAccess.Read);

            for (int i = 0; i < numberOfFiles; i++)
            {
                if (subFiles[i].subFileLength > 0)
                {
                    // build file name
                    string filename = pOutputDir + Path.DirectorySeparatorChar + "File_" + i.ToString().PadLeft(5, '0') + ".u9";

                    // delete existing file
                    if (File.Exists(filename))
                    {
                        File.Delete(filename);
                    }

                    bw = new BinaryWriter(File.Create(filename));
                    bw.Write(ParseFile.parseSimpleOffset(fs, subFiles[i].subFileOffset, subFiles[i].subFileLength));
                    bw.Close();
                }
            }

            fs.Close();
            fs.Dispose();
        }

    }
}
