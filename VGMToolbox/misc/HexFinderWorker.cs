using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.ComponentModel;

using VGMToolbox.auditing;
using VGMToolbox.util;

namespace VGMToolbox.misc
{
    class HexFinderWorker : BackgroundWorker
    {
        private int fileCount = 0;
        private int maxFiles = 0;
        private string[] searchArray;

        public struct HexFinderStruct
        {
            public string[] pPaths;
            public string hexString;
            public int totalFiles;
        }

        public HexFinderWorker()
        {
            fileCount = 0;
            maxFiles = 0;
            
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
        }

        private void findHexValues(HexFinderStruct pHexFinderStruct, DoWorkEventArgs e)
        {
            AuditingUtil.ProgressStruct vProgressStruct;

            foreach (string path in pHexFinderStruct.pPaths)
            { 
            
            }
        }

        private void findHexValueInFile(string pFileName, DoWorkEventArgs e)
        {
            int progress = (++fileCount * 100) / maxFiles;
            
            AuditingUtil.ProgressStruct vProgressStruct = new AuditingUtil.ProgressStruct();
            vProgressStruct.newNode = null;
            vProgressStruct.filename = pFileName;            

            foreach (string ss in searchArray)
            {
                string searchString = ss.Trim();

                if (searchString.Length % 2 != 0)
                {
                    continue;
                }
                byte[] searchBytes = new byte[searchString.Length / 2];

                int i;
                int j = 0;
                for (i = 0; i < searchString.Length; i += 2)
                {
                    searchBytes[j] = BitConverter.GetBytes(Int16.Parse(searchString.Substring(i, 2), System.Globalization.NumberStyles.AllowHexSpecifier))[0];
                    j++;
                }

                using (FileStream fs = File.Open(pFileName, FileMode.Open, FileAccess.Read))
                {
                    int offset = 0;

                    byte[] checkBytes = new byte[searchBytes.Length];

                    string outputString = "    Found Bytes At: ";

                    while (offset < fs.Length)
                    {
                        fs.Seek(offset, SeekOrigin.Begin);
                        fs.Read(checkBytes, 0, searchBytes.Length);

                        if (ParseFile.CompareSegment(checkBytes, 0, searchBytes))
                        {
                            outputString += "[" + offset.ToString("X2") + "]";
                        }

                        offset++;
                    }
                    
                    fs.Close();
                    
                    vProgressStruct.genericMessage = vProgressStruct.filename + outputString;                   
                    ReportProgress(progress, vProgressStruct);
                }
            }
            

        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            HexFinderStruct hexFinderStruct = (HexFinderStruct)e.Argument;
            maxFiles = hexFinderStruct.totalFiles;
            searchArray = hexFinderStruct.hexString.Split(',');
            
            this.findHexValues(hexFinderStruct, e);
        } 
    }
}
