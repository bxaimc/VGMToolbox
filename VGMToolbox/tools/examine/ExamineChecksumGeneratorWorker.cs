using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using System.Text;

using ICSharpCode.SharpZipLib.Checksums;

using VGMToolbox.format;
using VGMToolbox.format.util;
using VGMToolbox.plugin;
using VGMToolbox.util;

namespace VGMToolbox.tools.examine
{
    class ExamineChecksumGeneratorWorker : AVgmtDragAndDropWorker, IVgmtBackgroundWorker
    {
        private Dictionary<string, string[]> duplicateCheckHashStandard;
        private Dictionary<string, string[]> duplicateCheckHashVgmt;
        
        public struct ExamineChecksumGeneratorStruct : IVgmtWorkerStruct
        {
            public bool DoVgmtChecksums;
            public bool CheckForDuplicates;

            private string[] sourcePaths;
            public string[] SourcePaths
            {
                get { return sourcePaths; }
                set { sourcePaths = value; }
            }
        }

        public ExamineChecksumGeneratorWorker() : 
            base() 
        { 
            this.duplicateCheckHashStandard = new Dictionary<string,string[]>(StringComparer.InvariantCultureIgnoreCase);
            this.duplicateCheckHashVgmt = new Dictionary<string, string[]>(StringComparer.InvariantCultureIgnoreCase);

            this.progressCounterIncrementer = 10;
        }

        protected override void DoTaskForFile(string pPath,
            IVgmtWorkerStruct pExamineChecksumGeneratorStruct, DoWorkEventArgs e)
        {
            ExamineChecksumGeneratorStruct examineChecksumGeneratorStruct =
                (ExamineChecksumGeneratorStruct)pExamineChecksumGeneratorStruct;

            string crc32;
            string md5;
            string sha1;

            string vgmtCrc32 = "Not implemented for this format.";
            string vgmtMd5 = "Not implemented for this format.";
            string vgmtSha1 = "Not implemented for this format.";

            string checksumKey;

            Type formatType = null;
            IFormat vgmData = null;

            using (FileStream fs = File.OpenRead(pPath))
            {
                crc32 = ChecksumUtil.GetCrc32OfFullFile(fs);
                md5 = ChecksumUtil.GetMd5OfFullFile(fs);
                sha1 = ChecksumUtil.GetSha1OfFullFile(fs);

                if (examineChecksumGeneratorStruct.CheckForDuplicates)
                {
                    checksumKey = String.Format("{0}/{1}/{2}", crc32, md5, sha1);
                    this.addChecksumToHash(checksumKey, pPath, true);
                }

                formatType = FormatUtil.getObjectType(fs);
                if (formatType != null)
                {
                    vgmData = (IFormat)Activator.CreateInstance(formatType);
                    vgmData.Initialize(fs, pPath);
                }
            }

            if ((vgmData != null) && (examineChecksumGeneratorStruct.DoVgmtChecksums))
            {
                Crc32 crc32Generator = new Crc32();

                MD5CryptoServiceProvider md5Hash = new MD5CryptoServiceProvider();
                FileStream md5FileStream = new FileStream(Path.GetTempFileName(), FileMode.Create, FileAccess.Write);
                CryptoStream md5CryptoStream = new CryptoStream(md5FileStream, md5Hash, CryptoStreamMode.Write);

                SHA1CryptoServiceProvider sha1Hash = new SHA1CryptoServiceProvider();
                FileStream sha1FileStream = new FileStream(Path.GetTempFileName(), FileMode.Create, FileAccess.Write);
                CryptoStream sha1CryptoStream = new CryptoStream(sha1FileStream, sha1Hash, CryptoStreamMode.Write);

                vgmData.GetDatFileChecksums(ref crc32Generator, ref md5CryptoStream, 
                    ref sha1CryptoStream);
                md5CryptoStream.FlushFinalBlock();
                sha1CryptoStream.FlushFinalBlock();
                                                                
                vgmtCrc32 = crc32Generator.Value.ToString("X8");
                vgmtMd5 = ParseFile.ByteArrayToString(md5Hash.Hash);
                vgmtSha1 = ParseFile.ByteArrayToString(sha1Hash.Hash);

                if (examineChecksumGeneratorStruct.CheckForDuplicates)
                {
                    checksumKey = String.Format("{0}/{1}/{2}", vgmtCrc32, vgmtMd5, vgmtSha1);
                    this.addChecksumToHash(checksumKey, pPath, false);
                }

                md5FileStream.Close();
                md5FileStream.Dispose();
                sha1FileStream.Close();
                sha1FileStream.Dispose();

                md5CryptoStream.Close();
                md5CryptoStream.Dispose();
                sha1CryptoStream.Close();
                sha1CryptoStream.Dispose();
            }

            this.outputBuffer.AppendFormat("<{0}>{1}", pPath, Environment.NewLine);
            this.outputBuffer.AppendFormat("CRC32: {0}{1}", crc32, Environment.NewLine);
            this.outputBuffer.AppendFormat("MD5: {0}{1}", md5, Environment.NewLine);
            this.outputBuffer.AppendFormat("SHA1: {0}{1}", sha1, Environment.NewLine);

            if (examineChecksumGeneratorStruct.DoVgmtChecksums)
            {
                this.outputBuffer.AppendFormat("CRC32 (VGMT): {0}{1}", vgmtCrc32, Environment.NewLine);
                this.outputBuffer.AppendFormat("MD5 (VGMT): {0}{1}", vgmtMd5, Environment.NewLine);
                this.outputBuffer.AppendFormat("SHA1 (VGMT): {0}{1}", vgmtSha1, Environment.NewLine);
            }
            this.outputBuffer.AppendLine();
        }

        protected override void DoFinalTask(IVgmtWorkerStruct pExamineChecksumGeneratorStruct) 
        {
            ExamineChecksumGeneratorStruct examineChecksumGeneratorStruct =
                (ExamineChecksumGeneratorStruct)pExamineChecksumGeneratorStruct;

            string headerFormat = "The following duplicates were found using {0} checksums." + Environment.NewLine +
                "(CRC32/MD5/SHA1)";

            if (examineChecksumGeneratorStruct.CheckForDuplicates)
            {
                this.outputDuplicatesForDictionary(
                    this.duplicateCheckHashStandard, String.Format(headerFormat, "standard"));

                if (examineChecksumGeneratorStruct.DoVgmtChecksums)
                {
                    this.outputDuplicatesForDictionary(
                        this.duplicateCheckHashVgmt, String.Format(headerFormat, "VGMToolbox method"));                
                }
            
            }
        }

        private void outputDuplicatesForDictionary(Dictionary<string,string[]> hashList, string hashLabel)
        { 
            StringBuilder duplicateList = new StringBuilder();
            string[] paths;

            duplicateList.AppendFormat("{0}:{1}", hashLabel, Environment.NewLine);
            
            foreach (string key in hashList.Keys)
            {
                paths = hashList[key];

                if (paths.Length > 1)
                {
                    duplicateList.AppendFormat("{0}:{1}", key, Environment.NewLine);
                    
                    foreach (string s in paths)
                    {
                        duplicateList.AppendFormat("  {0}{1}", s, Environment.NewLine);
                    }
                }
            }

            this.progressStruct.Clear();
            progressStruct.GenericMessage = duplicateList.ToString();
            ReportProgress(Constants.ProgressMessageOnly, progressStruct);
        }


        private void addChecksumToHash(string checksum, string path, bool isStandardChecksum)
        {
            ArrayList paths = new ArrayList();
            
            if (isStandardChecksum)
            {
                if (this.duplicateCheckHashStandard.ContainsKey(checksum))
                {
                    paths = new ArrayList(this.duplicateCheckHashStandard[checksum]);
                    
                }

                paths.Add(path);
                this.duplicateCheckHashStandard[checksum] = (string[])paths.ToArray(typeof(string));
            }
            else
            {
                // do VGMT checksums
                if (this.duplicateCheckHashVgmt.ContainsKey(checksum))
                {
                    paths = new ArrayList(this.duplicateCheckHashVgmt[checksum]);

                }

                paths.Add(path);
                this.duplicateCheckHashVgmt[checksum] = (string[])paths.ToArray(typeof(string));            
            }
        }
    }
}
