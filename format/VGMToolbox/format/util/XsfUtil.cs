using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using Ionic.Zlib;

using Un4seen.Bass;
using Un4seen.Bass.AddOn.Midi;

using VGMToolbox.format.sdat;
using VGMToolbox.util;

namespace VGMToolbox.format.util
{
    public class XsfUtil
    {
        private delegate void XsfTagSetter(string pValue);
        private delegate string XsfTagGetter();
        
        public const string RECOMPRESSED_SUBFOLDER_NAME = "recompressed";
        public const int INVALID_DATA = -1;
        
        static readonly string UNPKPSF2_SOURCE_PATH =
            Path.Combine(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "external"), "psf2"), "unpkpsf2.exe");
        static readonly string BIN2PSF_SOURCE_PATH =
            Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "external"), "bin2psf.exe");
        static readonly string PSFPOINT_SOURCE_PATH =
            Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "external"), "psfpoint.exe");
        public const string PSFPOINT_BATCH_TXT = "psfpoint_batch.bat";

        // 2SF CONSTANTS
        public const string SSEQ2MID_TXT = "sseq2mid_output.txt";        
        public const string SSEQ2MID_TXT_MARKER = ".sseq:";
        public const string SSEQ2MID_TXT_END_OF_TRACK = "End of Track";
        public const string EMPTY_FILE_DIRECTORY = "Empty_Files";

        static readonly string SSEQ2MID_SOURCE_PATH =
            Path.Combine(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "external"), "2sf"), "sseq2mid.exe");

        static readonly byte[] MINI2SF_DATA_START = new byte[] { 0xC0, 0x0F, 0x0D, 0x00, 0x02, 0x00, 0x00, 0x00 };
        static readonly byte[] MINI2SF_SAVESTATE_ID = new byte[] { 0x53, 0x41, 0x56, 0x45 };


        public struct Xsf2ExeStruct
        {
            public bool IncludeExtension;
            public bool StripGsfHeader;        
        }

        public struct XsfBasicTaggingStruct
        {
            public string TagArtist;
            public string TagCopyright;
            public string TagYear;
            public string TagGame;
            public string TagComment;
            public string TagXsfByTagName;
            public string TagXsfByTagValue;
        }

        public struct TimePsf2Struct
        {
            public bool IncludeExtension;
            public bool StripGsfHeader;
        }

        public struct Time2sfStruct
        {
            public string Mini2sfDirectory;
            public string SdatPath;
            public bool DoSingleLoop;
        }

        public struct XsfRecompressStruct
        {
            public int CompressionLevel;
        }

        public struct XsfTagCopyStruct
        {
            public bool CopyEmptyTags;

            public bool UpdateTitleTag;
            public bool UpdateArtistTag;
            public bool UpdateGameTag;
            public bool UpdateYearTag;
            public bool UpdateGenreTag;
            public bool UpdateCommentTag;
            public bool UpdateCopyrightTag;
            public bool UpdateXsfByTag;
            public bool UpdateVolumeTag;
            public bool UpdateLengthTag;
            public bool UpdateFadeTag;
            public bool UpdateSystemTag;
        }

        public static bool IsPythonPresentInPath()
        {
            bool ret = false;
            
            string pathVariable = Environment.GetEnvironmentVariable("PATH");
            string[] paths = pathVariable.Split(new char[] { ';' });

            foreach (string p in paths)
            {
                if (Directory.Exists(p))
                {
                    string[] files = Directory.GetFiles(p, "python.exe");

                    if (files.Length > 0)
                    {
                        ret = true;
                        break;
                    }
                }
            }

            return ret;
        }
        
        public static string ExtractCompressedDataSection(string pPath, Xsf2ExeStruct pXsf2ExeStruct)
        {
            string outputFile = null;
            string formatString = GetXsfFormatString(pPath);

            if (!String.IsNullOrEmpty(formatString))
            {
                using (FileStream fs = File.OpenRead(pPath))
                {
                    Xsf vgmData = new Xsf();
                    vgmData.Initialize(fs, pPath);

                    if (vgmData.CompressedProgramLength > 0)
                    {
                        outputFile = Path.GetDirectoryName(pPath) + Path.DirectorySeparatorChar;
                        outputFile += (pXsf2ExeStruct.IncludeExtension ? Path.GetFileName(pPath) : Path.GetFileNameWithoutExtension(pPath)) + ".data.bin";

                        using (BinaryWriter bw = new BinaryWriter(File.Create(outputFile)))
                        {
                            using (ZlibStream zs = new ZlibStream(fs, Ionic.Zlib.CompressionMode.Decompress, true))
                            {
                                fs.Seek((long)(Xsf.RESERVED_SECTION_OFFSET + vgmData.ReservedSectionLength), SeekOrigin.Begin);
                                int read;
                                byte[] data = new byte[4096];

                                while ((read = zs.Read(data, 0, data.Length)) > 0)
                                {
                                    bw.Write(data, 0, read);
                                }                                
                            }
                        }

                        // strip GSF header
                        if (pXsf2ExeStruct.StripGsfHeader)
                        {
                            string strippedOutputFileName = outputFile + ".strip";

                            using (FileStream gsfStream = File.OpenRead(outputFile))
                            {
                                long fileOffset = 0x0C;
                                int fileLength = (int)(gsfStream.Length - fileOffset) + 1;

                                ParseFile.ExtractChunkToFile(gsfStream, fileOffset, fileLength,
                                    strippedOutputFileName);
                            }

                            File.Copy(strippedOutputFileName, outputFile, true);
                            File.Delete(strippedOutputFileName);
                        }

                    } // if (vgmData.CompressedProgramLength > 0)
                } // using (FileStream fs = File.OpenRead(pPath))       
            }
            return outputFile;
        }

        public static string ExtractReservedSection(string pPath, Xsf2ExeStruct pXsf2ExeStruct)
        {
            string outputFile = null;
            string formatString = GetXsfFormatString(pPath);

            if (!String.IsNullOrEmpty(formatString))
            {
                using (FileStream fs = File.OpenRead(pPath))
                {
                    Xsf vgmData = new Xsf();
                    vgmData.Initialize(fs, pPath);

                    if (vgmData.ReservedSectionLength > 0)
                    {
                        outputFile = String.Format("{0}.reserved.bin",
                            (pXsf2ExeStruct.IncludeExtension ? Path.GetFileName(pPath) : Path.GetFileNameWithoutExtension(pPath)));
                        outputFile = Path.Combine(Path.GetDirectoryName(pPath), outputFile);

                        ParseFile.ExtractChunkToFile(fs, Xsf.RESERVED_SECTION_OFFSET,
                            (int)vgmData.ReservedSectionLength, outputFile);

                    } // if (vgmData.CompressedProgramLength > 0)
                } // using (FileStream fs = File.OpenRead(pPath))       
            }
            
            return outputFile;        
        }

        public static string ReCompressDataSection(string pPath, XsfRecompressStruct pXsfRecompressStruct)
        {
            string outputFolder = Path.Combine(Path.GetDirectoryName(pPath), RECOMPRESSED_SUBFOLDER_NAME);
            string outputPath = null;

            Xsf2ExeStruct xsf2ExeStruct = new Xsf2ExeStruct();
            xsf2ExeStruct.IncludeExtension = true;
            xsf2ExeStruct.StripGsfHeader = false;

            string extractedDataPath = ExtractCompressedDataSection(pPath, xsf2ExeStruct);
            string deflatedCrc32String;

            if (extractedDataPath != null)
            {
                string deflatedOutputPath = null;
                
                try
                {
                    deflatedOutputPath = Path.ChangeExtension(extractedDataPath, ".deflated");

                    int read;
                    byte[] data = new byte[4096];

                    // open decompressed data section
                    using (FileStream inFs = File.OpenRead(extractedDataPath))
                    {
                        // open file for outputting deflated section
                        using (FileStream outFs = File.Open(deflatedOutputPath, FileMode.Create, FileAccess.Write))
                        {
                            using (ZlibStream zs = new ZlibStream(outFs, Ionic.Zlib.CompressionMode.Compress, (CompressionLevel)pXsfRecompressStruct.CompressionLevel, true))
                            {
                                while ((read = inFs.Read(data, 0, data.Length)) > 0)
                                {
                                    zs.Write(data, 0, read);
                                }

                                zs.Flush();
                            }
                        }
                    }

                    // create output path
                    outputPath = Path.Combine(outputFolder, Path.GetFileName(pPath));

                    if (!Directory.Exists(Path.GetDirectoryName(outputPath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
                    }

                    using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(outputPath)))
                    {
                        using (FileStream vgmFs = File.OpenRead(pPath))
                        {
                            Xsf vgmData = new Xsf();
                            vgmData.Initialize(vgmFs, pPath);

                            bw.Write(vgmData.AsciiSignature);
                            bw.Write(vgmData.VersionByte);
                            bw.Write(vgmData.ReservedSectionLength);
                            bw.Write(BitConverter.GetBytes((uint)new FileInfo(deflatedOutputPath).Length));

                            using (FileStream outFs = File.Open(deflatedOutputPath, FileMode.Open, FileAccess.Read))
                            {
                                // crc32
                                deflatedCrc32String = "0x" + ChecksumUtil.GetCrc32OfFullFile(outFs);
                                bw.Write((uint)VGMToolbox.util.Encoding.GetLongFromString(deflatedCrc32String));
                                // reserved section
                                bw.Write(ParseFile.ParseSimpleOffset(vgmFs, Xsf.RESERVED_SECTION_OFFSET, (int)vgmData.ReservedSectionLength));

                                // data section
                                outFs.Position = 0;
                                while ((read = outFs.Read(data, 0, data.Length)) > 0)
                                {
                                    bw.Write(data, 0, read);
                                }
                            }

                            long dataSectionEnd = Xsf.RESERVED_SECTION_OFFSET + vgmData.ReservedSectionLength + vgmData.CompressedProgramLength;
                            bw.Write(ParseFile.ParseSimpleOffset(vgmFs, dataSectionEnd, (int)(vgmFs.Length - dataSectionEnd)));
                        }
                    }
                }
                finally
                {
                    if (File.Exists(extractedDataPath))
                    {
                        File.Delete(extractedDataPath);
                    }

                    if ((deflatedOutputPath != null) && 
                        (File.Exists(deflatedOutputPath)))
                    {
                        File.Delete(deflatedOutputPath);
                    }
                }
            }
            return outputPath;
        }

        public static void Bin2Psf(string pExtension, int pVersionByte, string pPath, 
            ref string pStandardOutput, ref string pStandardError)
        {
            Process bin2PsfProcess = null;
            string filePath = Path.GetFullPath(pPath);

            // call bin2psf.exe
            string arguments = String.Format(" \"{0}\" {1} \"{2}\"", pExtension, pVersionByte.ToString(), filePath);
            bin2PsfProcess = new Process();
            bin2PsfProcess.StartInfo = new ProcessStartInfo(BIN2PSF_SOURCE_PATH, arguments);
            bin2PsfProcess.StartInfo.UseShellExecute = false;
            bin2PsfProcess.StartInfo.CreateNoWindow = true;

            bin2PsfProcess.StartInfo.RedirectStandardError = true;
            bin2PsfProcess.StartInfo.RedirectStandardOutput = true;

            bool isSuccess = bin2PsfProcess.Start();
            pStandardOutput = bin2PsfProcess.StandardOutput.ReadToEnd();
            pStandardError = bin2PsfProcess.StandardError.ReadToEnd();

            bin2PsfProcess.WaitForExit();
            bin2PsfProcess.Close();
            bin2PsfProcess.Dispose();

            // return outputDir;        
        }

        public static string GetXsfFormatString(string pPath)
        {
            string ret = null;

            using (FileStream typeFs = File.Open(pPath, FileMode.Open, FileAccess.Read))
            {
                Type dataType = FormatUtil.getObjectType(typeFs);

                if (dataType != null && dataType.Name.Equals("Xsf"))
                {
                    Xsf xsfFile = new Xsf();
                    xsfFile.Initialize(typeFs, pPath);

                    ret = xsfFile.getFormat();
                }
            }
            
            return ret;
        }

        public static void ExecutePsfPointBatchScript(string pFullPathToScript, bool pDeleteScriptAfterExecution)
        {
            Process batProcess;
            string folderContainingScript = Path.GetDirectoryName(pFullPathToScript);
            string scriptName = Path.GetFileName(pFullPathToScript);
            bool cleanupPsfpoint = false;

            // copy psfpoint.exe
            string psfpointDestinationPath = Path.Combine(folderContainingScript, Path.GetFileName(PSFPOINT_SOURCE_PATH));

            if (!File.Exists(psfpointDestinationPath))
            {
                File.Copy(PSFPOINT_SOURCE_PATH, psfpointDestinationPath, false);
                cleanupPsfpoint = true;
            }

            // execute script
            batProcess = new Process();
            batProcess.StartInfo = new ProcessStartInfo(PSFPOINT_BATCH_TXT);
            batProcess.StartInfo.WorkingDirectory = folderContainingScript;
            batProcess.StartInfo.CreateNoWindow = true;
            batProcess.Start();
            batProcess.WaitForExit();

            batProcess.Close();
            batProcess.Dispose();

            if (cleanupPsfpoint)
            {
                File.Delete(psfpointDestinationPath);
            }

            if (pDeleteScriptAfterExecution)
            {
                File.Delete(pFullPathToScript);
            }
        }

        public static string BuildBasicTaggingBatch(string pFullPathToScriptDestinationFolder, 
            XsfBasicTaggingStruct pXsfBasicTaggingStruct, string pFileMask)
        {            
            string batchFilePath = Path.Combine(pFullPathToScriptDestinationFolder, PSFPOINT_BATCH_TXT);
            if (File.Exists(batchFilePath))
            {
                batchFilePath += "_" + new Random().Next().ToString();
            }

            using (StreamWriter sw = new StreamWriter(File.Open(batchFilePath, FileMode.CreateNew, FileAccess.Write)))
            {
                string tagFormat = "psfpoint.exe {0}=\"{1}\" {2}";
                
                if (!String.IsNullOrEmpty(pXsfBasicTaggingStruct.TagArtist))
                {
                    sw.WriteLine(String.Format(tagFormat, "-artist", pXsfBasicTaggingStruct.TagArtist, pFileMask));
                }

                if (!String.IsNullOrEmpty(pXsfBasicTaggingStruct.TagCopyright))
                {
                    sw.WriteLine(String.Format(tagFormat, "-copyright", pXsfBasicTaggingStruct.TagCopyright, pFileMask));
                }

                if (!String.IsNullOrEmpty(pXsfBasicTaggingStruct.TagYear))
                {
                    sw.WriteLine(String.Format(tagFormat, "-year", pXsfBasicTaggingStruct.TagYear, pFileMask));
                }

                if (!String.IsNullOrEmpty(pXsfBasicTaggingStruct.TagGame))
                {
                    sw.WriteLine(String.Format(tagFormat, "-game", pXsfBasicTaggingStruct.TagGame, pFileMask));
                }

                if (!String.IsNullOrEmpty(pXsfBasicTaggingStruct.TagComment))
                {
                    sw.WriteLine(String.Format(tagFormat, "-comment", pXsfBasicTaggingStruct.TagComment, pFileMask));
                }

                if (!String.IsNullOrEmpty(pXsfBasicTaggingStruct.TagXsfByTagValue))
                {
                    sw.WriteLine(String.Format(tagFormat, pXsfBasicTaggingStruct.TagXsfByTagName, 
                        pXsfBasicTaggingStruct.TagXsfByTagValue, pFileMask));
                }
            }

            return batchFilePath;
        }

        public static string GetTitleForFileName(string pFilePath)
        {
            string fileName = Path.GetFileNameWithoutExtension(pFilePath);
            string[] splitFileName = fileName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string ret = fileName.Trim();

            if (splitFileName.Length > 1)
            {
                string firstSegment = splitFileName[0].Trim();
                string firstSegmentNumOnly = Regex.Replace(firstSegment, "[^0-9]", String.Empty);

                float firstSegmentLength = (float) firstSegment.Length;
                float firstSegmentNumOnlyLength = (float) firstSegmentNumOnly.Length;

                if ((firstSegmentNumOnlyLength > 0) && 
                    ((firstSegmentNumOnlyLength / firstSegmentLength) > 0.5)) // more than half are numeric
                {
                    ret = fileName.Substring(firstSegment.Length).Trim();
                }
            }

            return ret;
        }
        
        public static void CopyTags(IXsfTagFormat pSource, IXsfTagFormat pDestination, 
            XsfTagCopyStruct pXsfTagCopyStruct)
        {
            XsfTagGetter getter;
            XsfTagSetter setter;

            getter = new XsfTagGetter(pSource.GetTitleTag);
            setter = new XsfTagSetter(pDestination.SetTitleTag);
            CopyTag(pXsfTagCopyStruct.UpdateTitleTag, getter, setter, pXsfTagCopyStruct.CopyEmptyTags);

            getter = new XsfTagGetter(pSource.GetArtistTag);
            setter = new XsfTagSetter(pDestination.SetArtistTag);
            CopyTag(pXsfTagCopyStruct.UpdateArtistTag, getter, setter, pXsfTagCopyStruct.CopyEmptyTags);

            getter = new XsfTagGetter(pSource.GetGameTag);
            setter = new XsfTagSetter(pDestination.SetGameTag);
            CopyTag(pXsfTagCopyStruct.UpdateGameTag, getter, setter, pXsfTagCopyStruct.CopyEmptyTags);
            
            getter = new XsfTagGetter(pSource.GetYearTag);
            setter = new XsfTagSetter(pDestination.SetYearTag);
            CopyTag(pXsfTagCopyStruct.UpdateYearTag, getter, setter, pXsfTagCopyStruct.CopyEmptyTags);

            getter = new XsfTagGetter(pSource.GetGenreTag);
            setter = new XsfTagSetter(pDestination.SetGenreTag);
            CopyTag(pXsfTagCopyStruct.UpdateGenreTag, getter, setter, pXsfTagCopyStruct.CopyEmptyTags);
            
            getter = new XsfTagGetter(pSource.GetCommentTag);
            setter = new XsfTagSetter(pDestination.SetCommentTag);
            CopyTag(pXsfTagCopyStruct.UpdateCommentTag, getter, setter, pXsfTagCopyStruct.CopyEmptyTags);


            getter = new XsfTagGetter(pSource.GetCopyrightTag);
            setter = new XsfTagSetter(pDestination.SetCopyrightTag);
            CopyTag(pXsfTagCopyStruct.UpdateCopyrightTag, getter, setter, pXsfTagCopyStruct.CopyEmptyTags);            

            getter = new XsfTagGetter(pSource.GetXsfByTag);
            setter = new XsfTagSetter(pDestination.SetXsfByTag);
            CopyTag(pXsfTagCopyStruct.UpdateXsfByTag, getter, setter, pXsfTagCopyStruct.CopyEmptyTags);

            getter = new XsfTagGetter(pSource.GetVolumeTag);
            setter = new XsfTagSetter(pDestination.SetVolumeTag);
            CopyTag(pXsfTagCopyStruct.UpdateVolumeTag, getter, setter, pXsfTagCopyStruct.CopyEmptyTags);
            
            getter = new XsfTagGetter(pSource.GetLengthTag);
            setter = new XsfTagSetter(pDestination.SetLengthTag);
            CopyTag(pXsfTagCopyStruct.UpdateLengthTag, getter, setter, pXsfTagCopyStruct.CopyEmptyTags);

            getter = new XsfTagGetter(pSource.GetFadeTag);
            setter = new XsfTagSetter(pDestination.SetFadeTag);
            CopyTag(pXsfTagCopyStruct.UpdateFadeTag, getter, setter, pXsfTagCopyStruct.CopyEmptyTags);            

            getter = new XsfTagGetter(pSource.GetSystemTag);
            setter = new XsfTagSetter(pDestination.SetSystemTag);
            CopyTag(pXsfTagCopyStruct.UpdateSystemTag, getter, setter, pXsfTagCopyStruct.CopyEmptyTags);

            pDestination.UpdateTags();
        }

        private static void CopyTag(bool pDoCopy, XsfTagGetter pXsfTagGetter,
            XsfTagSetter pXsfTagSetter, bool pCopyEmptyTags)
        {                        
            if (pDoCopy)
            {
                string tagData = pXsfTagGetter();

                if ((pCopyEmptyTags) || 
                    ((!pCopyEmptyTags) && (!String.IsNullOrEmpty(tagData))))
                {
                    pXsfTagSetter(tagData);
                }
            }
        }

        // PSF2
        public static Ps2SequenceData.Ps2SqTimingStruct GetTimeForPsf2File(string pSqPath, int pSequenceId)
        {
            Ps2SequenceData.Ps2SqTimingStruct time = new Ps2SequenceData.Ps2SqTimingStruct();

            try
            {
                using (FileStream fs = File.Open(pSqPath, FileMode.Open, FileAccess.Read))
                {                    
                    Ps2SequenceData ps2SequenceData = new Ps2SequenceData(fs);

                    time = ps2SequenceData.getTimeInSecondsForSequenceNumber(fs, pSequenceId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }

            return time;
        }

        public static string UnpackPsf2(string pPath)
        { 
            return UnpackPsf2(pPath, null);
        }

        public static string UnpackPsf2(string pPath, string pDestinationFolder)
        {
            string outputDir = null;
            string formatString = GetXsfFormatString(pPath);

            if (!String.IsNullOrEmpty(formatString) && formatString.Equals(Xsf.FORMAT_NAME_PSF2))
            {
                Psf2 vgmData = new Psf2();
                
                using (FileStream fs = File.OpenRead(pPath))
                {
                    vgmData.Initialize(fs, pPath);                    
                }

                // prepare output folder
                if (String.IsNullOrEmpty(pDestinationFolder))
                {
                    outputDir = Path.Combine(Path.GetDirectoryName(pPath),
                        Path.GetFileNameWithoutExtension(pPath));
                }
                else
                {
                    outputDir = pDestinationFolder;
                }

                vgmData.Unpack(outputDir);
            }
            return outputDir;
        }

        // PSF        
        public static string ExtractPsxSequences(string pPath)
        {
            string outputPath = null;
            string fileToExtractFrom = null;
            string decompressedDataSection = null;

            long previousOffset = 0;
            long offsetLocation = -1;
            long seqEndLocation = -1;
            long seqEndLocationType2 = -1;
            long seqEndLocationType3 = -1;

            int i = 0;

            using (FileStream typeFs = File.Open(pPath, FileMode.Open, FileAccess.Read))
            {
                Type dataType = FormatUtil.getObjectType(typeFs);

                if (dataType != null && dataType.Name.Equals("Xsf"))
                {
                    Xsf xsfFile = new Xsf();
                    xsfFile.Initialize(typeFs, pPath);

                    if (xsfFile.getFormat().Equals(Xsf.FORMAT_NAME_PSF))
                    {
                        typeFs.Close();
                        typeFs.Dispose();

                        Xsf2ExeStruct xsf2ExeStruct = new Xsf2ExeStruct();
                        xsf2ExeStruct.IncludeExtension = true;
                        xsf2ExeStruct.StripGsfHeader = false;
                        decompressedDataSection = ExtractCompressedDataSection(pPath, xsf2ExeStruct);
                        fileToExtractFrom = decompressedDataSection;
                    }
                }
                else
                {
                    fileToExtractFrom = pPath;
                }
            }

            using (FileStream fs = File.Open(fileToExtractFrom, FileMode.Open, FileAccess.Read))
            {
                outputPath = Path.Combine(Path.GetDirectoryName(pPath), Path.ChangeExtension(pPath, PsxSequence.FILE_EXTENSION));

                if (outputPath.Equals(fileToExtractFrom))
                {
                    outputPath = Path.Combine(Path.GetDirectoryName(pPath), 
                        (Path.GetFileNameWithoutExtension(pPath) + "extract" + PsxSequence.FILE_EXTENSION));
                }

                while ((offsetLocation = ParseFile.GetNextOffset(fs, previousOffset, PsxSequence.ASCII_SIGNATURE)) > -1)
                {
                    seqEndLocation = ParseFile.GetNextOffset(fs, offsetLocation, PsxSequence.END_SEQUENCE);
                    seqEndLocationType2 = ParseFile.GetNextOffset(fs, offsetLocation, PsxSequence.END_SEQUENCE_TYPE2);
                    seqEndLocationType3 = ParseFile.GetNextOffset(fs, offsetLocation, PsxSequence.END_SEQUENCE_TYPE2);

                    if  (((seqEndLocation == -1) && (seqEndLocationType2 > -1)) ||
                        ((seqEndLocationType2 != -1) && (seqEndLocationType2 < seqEndLocation))) // SEP Type
                    {
                        seqEndLocation = seqEndLocationType2 + PsxSequence.END_SEQUENCE_TYPE2.Length;
                    }
                    else if (((seqEndLocation == -1) && (seqEndLocationType3 > -1)) ||
                        ((seqEndLocationType3 != -1) && (seqEndLocationType3 < seqEndLocation))) // SEP Type
                    {
                        seqEndLocation = seqEndLocationType3 + PsxSequence.END_SEQUENCE_TYPE3.Length;
                    }


                    if (seqEndLocation > -1)
                    {
                        seqEndLocation += PsxSequence.END_SEQUENCE.Length;  // add length to include the end bytes

                        // extract SEQ                    
                        ParseFile.ExtractChunkToFile(fs, offsetLocation, (int)(seqEndLocation - offsetLocation),
                            outputPath);


                        previousOffset = seqEndLocation + PsxSequence.END_SEQUENCE.Length;
                        i++;
                    }
                    else
                    {
                        fs.Close();
                        fs.Dispose();
                        
                        if (!String.IsNullOrEmpty(decompressedDataSection))
                        {
                            File.Delete(decompressedDataSection);
                        }
                        
                        throw new PsxSeqFormatException("SEQ begin found, but terminator bytes were not found.");
                    }
                }
            }

            if (!String.IsNullOrEmpty(decompressedDataSection))
            {
                File.Delete(decompressedDataSection);
            }

            return outputPath;
        }

        // 2SF
        public static int GetSongNumberForMini2sf(string pPath)
        {
            int ret = INVALID_DATA;
            
            string formatString = GetXsfFormatString(pPath);
            Xsf2ExeStruct xsf2ExeStruct;
            string decompressedDataSectionPath;

            if (!String.IsNullOrEmpty(formatString) && formatString.Equals(Xsf.FORMAT_NAME_2SF))
            {
                // extract data section
                xsf2ExeStruct = new Xsf2ExeStruct();
                xsf2ExeStruct.IncludeExtension = true;
                xsf2ExeStruct.StripGsfHeader = false;
                decompressedDataSectionPath = ExtractCompressedDataSection(pPath, xsf2ExeStruct);

                if (!String.IsNullOrEmpty(decompressedDataSectionPath))
                {
                    using (FileStream fs = File.OpenRead(decompressedDataSectionPath))
                    {
                        if (fs.Length == 0x0A) // make sure it is a mini2sf
                        {
                            ret = (int)BitConverter.ToUInt16(ParseFile.ParseSimpleOffset(fs, 8, 2), 0);
                        }
                    }

                    File.Delete(decompressedDataSectionPath);                
                }                
            }

            return ret;
        }

        public static int GetSongNumberForYoshiIslandMini2sf(string pPath)
        {
            int ret = INVALID_DATA;

            string formatString = GetXsfFormatString(pPath);
            Xsf2ExeStruct xsf2ExeStruct;
            string decompressedReservedSectionPath;
            string decompressedSaveStatePath;

            if (!String.IsNullOrEmpty(formatString) && formatString.Equals(Xsf.FORMAT_NAME_2SF))
            {
                // extract reserved section
                xsf2ExeStruct = new Xsf2ExeStruct();
                xsf2ExeStruct.IncludeExtension = true;
                xsf2ExeStruct.StripGsfHeader = false;
                decompressedReservedSectionPath = ExtractReservedSection(pPath, xsf2ExeStruct);

                if (!String.IsNullOrEmpty(decompressedReservedSectionPath))
                {
                    using (FileStream fs = File.OpenRead(decompressedReservedSectionPath))
                    {
                        byte[] savestateCheckbytes = new byte[4];
                        fs.Read(savestateCheckbytes, 0, 4);

                        // check for SAVE bytes to indicate this is a V1 set.
                        if (ParseFile.CompareSegment(savestateCheckbytes, 0, MINI2SF_SAVESTATE_ID))
                        {                            
                            decompressedSaveStatePath =
                                Path.ChangeExtension(decompressedReservedSectionPath, CompressionUtil.ZLIB_DECOMPRESS_OUTPUT_EXTENSION);
                            fs.Position = 0x0C;  // move to zlib section

                            // decompress save state
                            CompressionUtil.DecompressZlibStreamToFile(fs, decompressedSaveStatePath);

                            using (FileStream ssFs = File.OpenRead(decompressedSaveStatePath))
                            {
                                if (ssFs.Length == 0x0C) // verify save state length
                                {
                                    ret = (int)BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(ssFs, 8, 4), 0);
                                }
                            }
                            File.Delete(decompressedSaveStatePath);
                        }
                    }
                    File.Delete(decompressedReservedSectionPath);
                }
            }

            return ret;
        }

        public static void Time2sfFolder(Time2sfStruct pTime2sfStruct, out string pMessages)
        {
            Hashtable mini2sfSongNumbers = new Hashtable();
            string emptyFolderFileName;
            bool processSuccess;
            
            pMessages = String.Empty;

            // Verify paths
            if (!Directory.Exists(pTime2sfStruct.Mini2sfDirectory))
            {
                throw new DirectoryNotFoundException(String.Format("Cannot find directory <{0}>", pTime2sfStruct.Mini2sfDirectory));
            }
            
            if (!File.Exists(pTime2sfStruct.SdatPath))
            {
                throw new FileNotFoundException(String.Format("Cannot find file <{0}>", pTime2sfStruct.SdatPath));
            }

            if (Sdat.IsSdat(pTime2sfStruct.SdatPath))
            {

                // delete existing batch file
                string psfpointBatchFilePath = Path.Combine(Path.Combine(pTime2sfStruct.Mini2sfDirectory, "text"), PSFPOINT_BATCH_TXT);

                if (File.Exists(psfpointBatchFilePath))
                {
                    File.Delete(psfpointBatchFilePath);
                }

                // extract SDAT
                string extractedSdatPath = Path.Combine(Path.GetDirectoryName(pTime2sfStruct.SdatPath), Path.GetFileNameWithoutExtension(pTime2sfStruct.SdatPath));
                if (Directory.Exists(extractedSdatPath))
                {
                    extractedSdatPath += String.Format("_temp_{0}", new Random().Next().ToString());
                }

                string extractedSseqPath = Path.Combine(extractedSdatPath, "Seq");

                Sdat sdat = new Sdat();
                using (FileStream fs = File.Open(pTime2sfStruct.SdatPath, FileMode.Open, FileAccess.Read))
                {
                    sdat.Initialize(fs, pTime2sfStruct.SdatPath);
                    sdat.ExtractSseqs(fs, extractedSdatPath);
                }

                // Make SMAP
                Smap smap = new Smap(sdat);

                // Build Hashtable for mini2sfs
                int songNumber;
                foreach (string mini2sfFile in Directory.GetFiles(pTime2sfStruct.Mini2sfDirectory, "*.mini2sf"))
                {
                    songNumber = GetSongNumberForMini2sf(mini2sfFile);
                    
                    if (!mini2sfSongNumbers.ContainsKey(songNumber))
                    {
                        mini2sfSongNumbers.Add(songNumber, mini2sfFile);
                    }
                }

                // Loop through SMAP and build timing script
                string emptyFileDir = Path.Combine(pTime2sfStruct.Mini2sfDirectory, EMPTY_FILE_DIRECTORY);

                int totalSequences = smap.SseqSection.Length;
                int i = 1;

                // Initialize Bass            
                if (smap.SseqSection.Length > 0)
                {
                    Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero, null);
                }

                string rippedFileName;
                string rippedFilePath;
                string outputMessages;
                string sseqFilePath;

                foreach (Smap.SmapSeqStruct s in smap.SseqSection)
                {
                    if (mini2sfSongNumbers.ContainsKey(s.number))
                    {
                        rippedFileName = Path.GetFileName((string)mini2sfSongNumbers[s.number]);
                        rippedFilePath = (string)mini2sfSongNumbers[s.number];

                        // check if file is empty or not
                        if (s.fileID == Smap.EMPTY_FILE_ID)
                        {
                            // move to empty dir
                            if (!Directory.Exists(emptyFileDir))
                            {
                                Directory.CreateDirectory(emptyFileDir);
                            }

                            if (File.Exists(rippedFilePath))
                            {
                                emptyFolderFileName = Path.Combine(emptyFileDir, rippedFileName);
                                File.Copy(rippedFilePath, emptyFolderFileName, true);
                                File.Delete(rippedFilePath);
                            }
                        }
                        else
                        {
                            sseqFilePath = Path.Combine(extractedSseqPath, s.name);
                            
                            // convert sseq file to midi
                            processSuccess = convertSseqFile(SSEQ2MID_SOURCE_PATH, pTime2sfStruct.Mini2sfDirectory,
                                sseqFilePath, pTime2sfStruct.DoSingleLoop, out outputMessages);
                            pMessages += outputMessages;

                            // time file
                            if (processSuccess)
                            {
                                processSuccess = buildFileTimingBatch(pTime2sfStruct.Mini2sfDirectory,
                                    rippedFilePath, sseqFilePath, out outputMessages);
                                pMessages += outputMessages;
                            }
                        }                                        
                    }
                    i++;
                }
                
                // copy script
                string psfpointBatchFileDestinationPath = Path.Combine(pTime2sfStruct.Mini2sfDirectory, PSFPOINT_BATCH_TXT);
                File.Copy(psfpointBatchFilePath, psfpointBatchFileDestinationPath, true);

                // run timing script
                ExecutePsfPointBatchScript(psfpointBatchFileDestinationPath, true);

                // delete extracted SDAT
                Directory.Delete(extractedSdatPath, true);
            }
            else
            {
                throw new FormatException(String.Format("{0} is not an SDAT file.", pTime2sfStruct.SdatPath));
            } // if (Sdat.IsSdat(pTime2sfStruct.SdatPath))

        }

        private static bool convertSseqFile(string pSseq2MidToolPath, string pMini2sfPath,
            string pSseqFilePath, bool pDoSingleLoop, out string pErrorMessage)
        {
            Process ndsProcess;
            bool isSuccess;
            pErrorMessage = String.Empty;

            // convert existing sseq to mid            
            string sseqPath = Path.GetDirectoryName(pSseqFilePath);
            string sseq2MidDestinationPath = Path.Combine(sseqPath, Path.GetFileName(pSseq2MidToolPath));

            try
            {
                if (!File.Exists(sseq2MidDestinationPath))
                {
                    File.Copy(pSseq2MidToolPath, sseq2MidDestinationPath, false);
                }
                
                // pDoSingleLoop
                string arguments;

                if (pDoSingleLoop)
                {
                    arguments = String.Format(" -1 -l {0}", Path.GetFileName(pSseqFilePath));
                }
                else
                {
                    arguments = String.Format(" -2 -l {0}", Path.GetFileName(pSseqFilePath));
                }

                ndsProcess = new Process();

                ndsProcess.StartInfo = new ProcessStartInfo(sseq2MidDestinationPath, arguments);
                ndsProcess.StartInfo.WorkingDirectory = sseqPath;
                ndsProcess.StartInfo.UseShellExecute = false;
                ndsProcess.StartInfo.CreateNoWindow = true;
                ndsProcess.StartInfo.RedirectStandardOutput = true;
                isSuccess = ndsProcess.Start();
                string sseqOutputFile = ndsProcess.StandardOutput.ReadToEnd();
                ndsProcess.WaitForExit();
                ndsProcess.Close();
                ndsProcess.Dispose();

                // output redirected standard output
                string textOutputPath = Path.Combine(pMini2sfPath, "text");
                string sseq2MidOutputPath = Path.Combine(textOutputPath, SSEQ2MID_TXT);

                if (!Directory.Exists(textOutputPath)) { Directory.CreateDirectory(textOutputPath); }

                TextWriter tw = File.CreateText(sseq2MidOutputPath);
                tw.Write(sseqOutputFile);
                tw.Close();
                tw.Dispose();
            }
            catch (Exception _e)
            {
                isSuccess = false;
                pErrorMessage = _e.Message + Environment.NewLine;
            }

            return isSuccess;
        }

        private static bool buildFileTimingBatch(string pMini2sfPath,
            string p2sfFilePath, string pSseqFilePath, out string pErrorMessage)
        {
            bool isSuccess = false;
            string arguments;
            pErrorMessage = String.Empty;

            StreamWriter sw;

            StringBuilder strReturn = new StringBuilder(128);
            int minutes;
            int seconds;

            int midiStream;

            string _2sfFileName = Path.GetFileName(p2sfFilePath);

            string psfpointBatchFilePath = Path.Combine(Path.Combine(pMini2sfPath, "text"), PSFPOINT_BATCH_TXT);

            if (!File.Exists(psfpointBatchFilePath))
            {
                sw = File.CreateText(psfpointBatchFilePath);
            }
            else
            {
                sw = new StreamWriter(File.Open(psfpointBatchFilePath, FileMode.Append, FileAccess.Write));
            }

            try
            {
                string midiFilePath = pSseqFilePath + ".mid";

                if (File.Exists(midiFilePath))
                {
                    midiStream = BassMidi.BASS_MIDI_StreamCreateFile(midiFilePath, 0L, 0L, BASSFlag.BASS_DEFAULT, 0);

                    if (midiStream != 0)
                    {
                        // play the channel
                        long length = Bass.BASS_ChannelGetLength(midiStream);
                        double tlength = Bass.BASS_ChannelBytes2Seconds(midiStream, length);
                        minutes = (int)(tlength / 60);
                        seconds = (int)(tlength - (minutes * 60));

                        if (seconds > 59)
                        {
                            minutes++;
                            seconds -= 60;
                        }
                    }
                    else
                    {
                        // error
                        pErrorMessage = Environment.NewLine + String.Format("Stream error: {0}", Bass.BASS_ErrorGetCode()) +
                            Environment.NewLine;                       
                        return false;
                    }


                    // Do Fade
                    if (isLoopingTrack(pMini2sfPath, Path.GetFileName(pSseqFilePath)))
                    {
                        arguments = " -fade=\"10\" " + _2sfFileName;

                        if (minutes == 0 && seconds == 0)
                        {
                            seconds = 1;
                        }
                    }
                    else
                    {
                        arguments = " -fade=\"1\" " + _2sfFileName;
                        seconds++;
                        if (seconds > 60)
                        {
                            minutes++;
                            seconds -= 60;
                        }

                    }

                    // Add fade info to batch file
                    sw.WriteLine("psfpoint.exe" + arguments);

                    // Add length info to batch file
                    arguments = " -length=\"" + minutes + ":" + seconds.ToString().PadLeft(2, '0') + "\" " + _2sfFileName;
                    sw.WriteLine("psfpoint.exe" + arguments);
                }

                isSuccess = true;
            }
            catch (Exception e)
            {
                pErrorMessage = String.Format("Error timing {0}: {1}", p2sfFilePath, e.Message) + Environment.NewLine;
            }
            finally
            {
                sw.Close();
                sw.Dispose();
            }

            return isSuccess;
        }

        private static bool isLoopingTrack(string pMini2sfPath, string pSequenceName)
        {
            string sseq2MidOutputPath = Path.Combine(Path.Combine(pMini2sfPath, "text"), SSEQ2MID_TXT);
            string oneLineBack = String.Empty;
            string twoLinesBack = String.Empty;

            bool _ret = true;

            // Check Path
            if (File.Exists(sseq2MidOutputPath))
            {
                string inputLine = String.Empty;

                TextReader textReader = new StreamReader(sseq2MidOutputPath);
                while ((inputLine = textReader.ReadLine()) != null)
                {
                    // Check for the incoming sequence name
                    string sseqFileName = Path.GetFileName(pSequenceName);
                    if (inputLine.Trim().Contains(sseqFileName))
                    {
                        // Skip columns headers
                        textReader.ReadLine();

                        // Read until EOF or End of SEQ section (blank line)
                        while (((inputLine = textReader.ReadLine()) != null) &&
                               !inputLine.Trim().Contains(SSEQ2MID_TXT_MARKER))
                        {
                            twoLinesBack = oneLineBack;
                            oneLineBack = inputLine;
                        }

                        if (twoLinesBack.Contains(SSEQ2MID_TXT_END_OF_TRACK))
                        {
                            _ret = false;
                        }
                    }


                }

                textReader.Close();
                textReader.Dispose();
            }

            return _ret;
        }

        public static void Make2sfSet(string pRomPath, string pSdatPath, 
            int pMinIndex, int pMaxIndex, string pOutputFolder)
        {
            int read;
            byte[] data = new byte[Constants.FILE_READ_CHUNK_SIZE];
            
            string sdatPrefix = Path.GetFileNameWithoutExtension(pSdatPath);
            string libOutputPath = Path.Combine(pOutputFolder, sdatPrefix + ".2sflib");
            string compressedDataSectionPath = Path.Combine(pOutputFolder, sdatPrefix + ".data.bin");

            string dataCrc32 = String.Empty;
            UInt32 dataLength = 0;
            long totalRomSize;
            FileInfo fi;

            System.Text.Encoding enc = System.Text.Encoding.ASCII;
            byte[] tagData;

            // write compressed section to temp file
            using (FileStream dataFs = File.Open(compressedDataSectionPath, FileMode.Create, FileAccess.ReadWrite))
            {                                
                using (ZlibStream zs = new ZlibStream(dataFs, Ionic.Zlib.CompressionMode.Compress, CompressionLevel.BestCompression, true))
                {
                    fi = new FileInfo(pRomPath);
                    totalRomSize = fi.Length;
                    fi = new FileInfo(pSdatPath);
                    totalRomSize += fi.Length;

                    zs.Write(new byte[] { 0, 0, 0, 0}, 0, 4);
                    zs.Write(BitConverter.GetBytes((UInt32) totalRomSize), 0, 4);
                    
                    // write rom file
                    using (FileStream romStream = File.OpenRead(pRomPath))
                    {
                        while ((read = romStream.Read(data, 0, data.Length)) > 0)
                        {
                            zs.Write(data, 0, read);
                        }
                    }
                    
                    // write SDAT                                        
                    using (FileStream sdatStream = File.OpenRead(pSdatPath))
                    {
                        while ((read = sdatStream.Read(data, 0, data.Length)) > 0)
                        {
                            zs.Write(data, 0, read);
                        }
                        zs.Flush();                        
                    }
                }

                // output 2sflib file
                dataFs.Seek(0, SeekOrigin.Begin);
                dataLength = (UInt32)dataFs.Length;
                dataCrc32 = ChecksumUtil.GetCrc32OfFullFile(dataFs);

                using (FileStream libStream = File.OpenWrite(libOutputPath))
                {
                    using (BinaryWriter bw = new BinaryWriter(libStream))
                    {
                        bw.Write('P');
                        bw.Write('S');
                        bw.Write('F');
                        bw.Write(new byte[] { 0x24 });
                        bw.Write(BitConverter.GetBytes((UInt32)0));          // reserved size
                        bw.Write(BitConverter.GetBytes(dataLength));         // data length
                        bw.Write((UInt32)VGMToolbox.util.Encoding.GetLongFromString("0x" + dataCrc32)); // data crc32

                        // data
                        dataFs.Seek(0, SeekOrigin.Begin);
                        while ((read = dataFs.Read(data, 0, data.Length)) > 0)
                        {
                            bw.Write(data, 0, read);
                        }
                    }
                }                                
            }

            // delete compressed data section
            if (File.Exists(compressedDataSectionPath))
            {
                File.Delete(compressedDataSectionPath);
            }

            // write .mini2sfs
            string mini2sfFileName;
            int mini2sfCrc32;
            
            byte[] mini2sfData = new byte[MINI2SF_DATA_START.Length + 2];
            Array.ConstrainedCopy(MINI2SF_DATA_START, 0, mini2sfData, 0, MINI2SF_DATA_START.Length);


            for (int i = pMinIndex; i <= pMaxIndex; i++)
            {
                using (MemoryStream mini2sfMs = new MemoryStream(mini2sfData, true))
                {
                    // build data section
                    Array.ConstrainedCopy(BitConverter.GetBytes((UInt16)i), 0, mini2sfData, mini2sfData.Length - 2, 2);

                    // create compressed Stream
                    using (MemoryStream compressedMs = new MemoryStream())
                    {
                        using (ZlibStream zs = new ZlibStream(compressedMs, Ionic.Zlib.CompressionMode.Compress, CompressionLevel.None, true))
                        {
                            zs.Write(mini2sfMs.ToArray(), 0, (int)mini2sfMs.Length);
                            zs.Flush();
                        }

                        // get CRC32
                        CRC32 crc32Calc = new CRC32();
                        mini2sfCrc32 = crc32Calc.GetCrc32(new System.IO.MemoryStream(compressedMs.ToArray()));

                        // write mini2sf to disk
                        mini2sfFileName = Path.Combine(pOutputFolder, String.Format("{0}-{1}.mini2sf", sdatPrefix, i.ToString("x4")));

                        using (FileStream mini2sfFs = File.Open(mini2sfFileName, FileMode.Create, FileAccess.Write))
                        {
                            using (BinaryWriter bw = new BinaryWriter(mini2sfFs))
                            {
                                bw.Write('P');
                                bw.Write('S');
                                bw.Write('F');
                                bw.Write(new byte[] { 0x24 });
                                bw.Write(BitConverter.GetBytes((UInt32)0));                  // reserved size
                                bw.Write(BitConverter.GetBytes((UInt32)compressedMs.Length)); // data length                        
                                bw.Write(BitConverter.GetBytes((UInt32)mini2sfCrc32));       // data crc32    
                                bw.Write(compressedMs.ToArray());                            // data

                                // add [TAG]
                                tagData = enc.GetBytes(Xsf.ASCII_TAG);
                                bw.Write(tagData, 0, tagData.Length);

                                tagData = enc.GetBytes(String.Format("_lib={0}", Path.GetFileName(libOutputPath)));
                                bw.Write(tagData, 0, tagData.Length);
                                bw.Write(0x0A);

                            }
                        }
                    }
                }
            }    
        }
    }
}
