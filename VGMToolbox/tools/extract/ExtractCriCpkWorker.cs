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
    class ExtractCriCpkWorker : AVgmtDragAndDropWorker, IVgmtBackgroundWorker
    {
        public const string CRI_CPK_EXTRACTION_FOLDER = "VGMT_CPK_EXTRACT";
        
        public struct ExtractCriCpkStruct : IVgmtWorkerStruct
        {
            public string[] SourcePaths { set; get; }
        }

        public ExtractCriCpkWorker() :
            base() { }

        protected override void DoTaskForFile(string pPath, IVgmtWorkerStruct pExtractCriCpkStruct, DoWorkEventArgs e)
        {
            ExtractCriCpkStruct extractCriCpkStruct = (ExtractCriCpkStruct)pExtractCriCpkStruct;
            string extractionDirectoryBase;
            string extractionDirectory;

            using (FileStream fs = File.Open(pPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                CriUtfTable cpkUtf = new CriUtfTable();
                cpkUtf.Initialize(fs, 0x10);

                CriUtfTable tocUtf = initializeToc(fs, cpkUtf, "TocOffset");
                CriUtfTable etocUtf = initializeToc(fs, cpkUtf, "EtocOffset");
                CriUtfTable itocUtf = initializeToc(fs, cpkUtf, "ItocOffset");
                CriUtfTable gtocUtf = initializeToc(fs, cpkUtf, "GtocOffset");

                extractionDirectoryBase = Path.Combine(Path.GetDirectoryName(fs.Name), CRI_CPK_EXTRACTION_FOLDER);
                
                // TOC
                extractionDirectory = Path.Combine(extractionDirectoryBase, "TOC");                
                this.ExtractFilesForToc(fs, cpkUtf, tocUtf, extractionDirectory);
            }
        }

        private CriUtfTable initializeToc(FileStream fs, CriUtfTable cpkUtf, string tocIdentifierString)
        {
            CriUtfTable toc = null;
            if (cpkUtf.Rows[0].ContainsKey(tocIdentifierString) &&
                cpkUtf.Rows[0][tocIdentifierString].Value != null)
            {
                toc = new CriUtfTable();
                toc.Initialize(fs, (long)((ulong)cpkUtf.Rows[0][tocIdentifierString].Value) + 0x10);
            }

            return toc;
        }

        private object getUtfFieldForRow(CriUtfTable utfTable, int rowIndex, string key)
        {
            object ret = null;

            if (utfTable.Rows.GetLength(0) > rowIndex)
            {
                if (utfTable.Rows[rowIndex].ContainsKey(key))
                {
                    ret = utfTable.Rows[rowIndex][key].Value;
                }
            }

            return ret;
        }

        private CriUtfTocFileInfo getUtfTocFileInfo(CriUtfTable tocUtf, int rowIndex)
        {
            CriUtfTocFileInfo fileInfo = new CriUtfTocFileInfo();
            object temp;

            temp = getUtfFieldForRow(tocUtf, rowIndex, "DirName");
            fileInfo.DirName = temp != null? (string)temp : null;

            temp = getUtfFieldForRow(tocUtf, rowIndex, "FileName");
            fileInfo.FileName = temp != null ? (string)temp : null;

            temp = getUtfFieldForRow(tocUtf, rowIndex, "FileOffset");
            fileInfo.FileOffset = temp != null ? (ulong)temp : ulong.MaxValue;

            temp = getUtfFieldForRow(tocUtf, rowIndex, "FileSize");
            fileInfo.FileSize = temp != null ? (uint)temp : uint.MaxValue;

            temp = getUtfFieldForRow(tocUtf, rowIndex, "ExtractSize");
            fileInfo.ExtractSize = temp != null ? (uint)temp : uint.MaxValue;

            return fileInfo;
        }

        private void ExtractFilesForToc(FileStream fs, CriUtfTable cpkUtf, CriUtfTable tocUtf, string extractionDestination)
        {
            CriUtfTocFileInfo fileInfo = new CriUtfTocFileInfo();

            long trueTocBaseOffset;
            string destinationFile;
            ulong contentOffset = (ulong)getUtfFieldForRow(cpkUtf, 0, "ContentOffset");

            // create destination if needed
            if (!Directory.Exists(extractionDestination))
            {
                Directory.CreateDirectory(extractionDestination);
            }

            using (BufferedStream bs = new BufferedStream(fs))
            {

                // loop over files
                for (int i = 0; i < tocUtf.Rows.GetLength(0); i++)
                {
                    fileInfo = this.getUtfTocFileInfo(tocUtf, i);

                    // set true base offset, since UTF header starts at 0x10 of a container type
                    trueTocBaseOffset = tocUtf.BaseOffset - 0x10;

                    // get absolute offset
                    if (contentOffset < (ulong)trueTocBaseOffset)
                    {
                        fileInfo.FileOffset += contentOffset;
                    }
                    else
                    {
                        fileInfo.FileOffset += (ulong)trueTocBaseOffset;
                    }

                    // build output file path
                    destinationFile = Path.Combine(extractionDestination, fileInfo.DirName);
                    destinationFile = Path.Combine(destinationFile, fileInfo.FileName);

                    if (fileInfo.ExtractSize > fileInfo.FileSize)
                    {
                        CriCpkArchive.Uncompress2(bs, (long)fileInfo.FileOffset, fileInfo.FileSize, destinationFile);
                    }
                    else
                    {
                        ParseFile.ExtractChunkToFile64(fs, fileInfo.FileOffset, fileInfo.FileSize, destinationFile, false, false);
                    }
                } // for (int i = 0; i < tocUtf.Rows.GetLength(0); i++)
            } // using (BufferedStream bs = new BufferedStream(fs))
        }
    }

}
