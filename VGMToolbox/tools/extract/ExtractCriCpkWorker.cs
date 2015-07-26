using System;
using System.ComponentModel;
using System.IO;

using VGMToolbox.format;
using VGMToolbox.plugin;

namespace VGMToolbox.tools.extract
{
    class ExtractCriCpkWorker : AVgmtDragAndDropWorker, IVgmtBackgroundWorker
    {
        public const string CRI_CPK_EXTRACTION_FOLDER = "VGMT_CPK_EXTRACT_{0}";
        
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

                CriUtfTable tocUtf = CriCpkArchive.InitializeToc(fs, cpkUtf, "TocOffset");
                CriUtfTable etocUtf = CriCpkArchive.InitializeToc(fs, cpkUtf, "EtocOffset");
                CriUtfTable itocUtf = CriCpkArchive.InitializeToc(fs, cpkUtf, "ItocOffset");
                CriUtfTable gtocUtf = CriCpkArchive.InitializeToc(fs, cpkUtf, "GtocOffset");

                extractionDirectoryBase = Path.Combine(Path.GetDirectoryName(fs.Name), 
                    String.Format(CRI_CPK_EXTRACTION_FOLDER, Path.GetFileName(fs.Name)));

                if (tocUtf != null)
                {
                    extractionDirectory = Path.Combine(extractionDirectoryBase, "TOC");
                    CriCpkArchive.ExtractFilesForToc(fs, cpkUtf, tocUtf, extractionDirectory);
                }

                if (etocUtf != null)
                {
                    extractionDirectory = Path.Combine(extractionDirectoryBase, "ETOC");
                    CriCpkArchive.ExtractFilesForToc(fs, cpkUtf, etocUtf, extractionDirectory);
                }

                if (itocUtf != null)
                {
                    extractionDirectory = Path.Combine(extractionDirectoryBase, "ITOC");
                    CriCpkArchive.ExtractFilesForToc(fs, cpkUtf, itocUtf, extractionDirectory);
                }

                if (gtocUtf != null)
                {
                    extractionDirectory = Path.Combine(extractionDirectoryBase, "GTOC");
                    CriCpkArchive.ExtractFilesForToc(fs, cpkUtf, gtocUtf, extractionDirectory);
                }
            }
        }                              
    }
}
