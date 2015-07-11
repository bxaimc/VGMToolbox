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
        public struct ExtractCriCpkStruct : IVgmtWorkerStruct
        {
            public string[] SourcePaths { set; get; }
        }

        public ExtractCriCpkWorker() :
            base() { }

        protected override void DoTaskForFile(string pPath, IVgmtWorkerStruct pExtractCriCpkStruct, DoWorkEventArgs e)
        {
            ExtractCriCpkStruct extractCriCpkStruct = (ExtractCriCpkStruct)pExtractCriCpkStruct;

            using (FileStream fs = File.Open(pPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                CriUtfTable cpkUtf = new CriUtfTable();
                cpkUtf.Initialize(fs, 0x10);

                CriUtfTable tocUtf = new CriUtfTable();
                tocUtf.Initialize(fs, (long)((ulong)cpkUtf.Rows[0]["TocOffset"].Value) + 0x10);

                //CriUtfTable etocUtf = new CriUtfTable();
                //tocUtf.Initialize(fs, (long)((ulong)cpkUtf.Rows[0]["EtocOffset"].Value) + 0x10);

                //CriUtfTable itocUtf = new CriUtfTable();
                //tocUtf.Initialize(fs, (long)((ulong)cpkUtf.Rows[0]["ItocOffset"].Value) + 0x10);

                //CriUtfTable gtocUtf = new CriUtfTable();
                //tocUtf.Initialize(fs, (long)((ulong)cpkUtf.Rows[0]["GtocOffset"].Value) + 0x10);


            }
        }
    }
}
