using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

using VGMToolbox.format;
using VGMToolbox.util;

namespace VGMToolbox.format.util
{
    public class XsfUtil
    {
        public const int INVALID_DATA = -1;
        
        static readonly string UNPKPSF2_SOURCE_PATH =
            Path.Combine(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "external"), "psf2"), "unpkpsf2.exe");
        static readonly string BIN2PSF_SOURCE_PATH =
            Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "external"), "bin2psf.exe");
        
        public struct Xsf2ExeStruct
        {
            public bool IncludeExtension;
            public bool StripGsfHeader;        
        }

        public struct TimePsf2Struct
        {
            public bool IncludeExtension;
            public bool StripGsfHeader;
        }

        public static bool IsPythonPresentInPath()
        {
            bool ret = false;
            
            string pathVariable = Environment.GetEnvironmentVariable("PATH");
            string[] paths = pathVariable.Split(new char[] { ';' });

            foreach (string p in paths)
            {
                string[] files = Directory.GetFiles(p, "python.exe");

                if (files.Length > 0)
                {
                    ret = true;
                    break;
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
                        BinaryWriter bw;
                        outputFile = Path.GetDirectoryName(pPath) + Path.DirectorySeparatorChar;
                        outputFile += (pXsf2ExeStruct.IncludeExtension ? Path.GetFileName(pPath) : Path.GetFileNameWithoutExtension(pPath)) + ".data.bin";


                        bw = new BinaryWriter(File.Create(outputFile));

                        InflaterInputStream inflater;
                        int read;
                        byte[] data = new byte[4096];

                        fs.Seek((long)(Xsf.RESERVED_SECTION_OFFSET + vgmData.ReservedSectionLength), SeekOrigin.Begin);
                        inflater = new InflaterInputStream(fs);

                        while ((read = inflater.Read(data, 0, data.Length)) > 0)
                        {
                            bw.Write(data, 0, read);
                        }

                        bw.Close();
                        inflater.Close();
                        inflater.Dispose();

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

        public static string UnpackPsf2(string pPath, ref string pStandardOutput, ref string pStandardError)
        {
            string filePath;
            string outputDir = null;
            
            Process unpkPsf2Process = null;

            string formatString = GetXsfFormatString(pPath);

            if (!String.IsNullOrEmpty(formatString) && formatString.Equals(Xsf.FORMAT_NAME_PSF2))
            {
                using (FileStream fs = File.OpenRead(pPath))
                {

                    filePath = Path.GetFullPath(pPath);
                    outputDir = Path.Combine(Path.GetDirectoryName(filePath),
                        Path.GetFileNameWithoutExtension(filePath));

                    // call unpkpsf2.exe
                    string arguments = String.Format(" \"{0}\" \"{1}\"", filePath, outputDir);
                    unpkPsf2Process = new Process();
                    unpkPsf2Process.StartInfo = new ProcessStartInfo(UNPKPSF2_SOURCE_PATH, arguments);
                    unpkPsf2Process.StartInfo.UseShellExecute = false;
                    unpkPsf2Process.StartInfo.CreateNoWindow = true;

                    unpkPsf2Process.StartInfo.RedirectStandardError = true;
                    unpkPsf2Process.StartInfo.RedirectStandardOutput = true;

                    bool isSuccess = unpkPsf2Process.Start();
                    pStandardOutput = unpkPsf2Process.StandardOutput.ReadToEnd();
                    pStandardError = unpkPsf2Process.StandardError.ReadToEnd();

                    unpkPsf2Process.WaitForExit();
                    unpkPsf2Process.Close();
                    unpkPsf2Process.Dispose();
                }
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
                            ret = (int)BitConverter.ToUInt16(ParseFile.parseSimpleOffset(fs, 8, 2), 0);
                        }
                    }
                }
            }

            return ret;
        }
    }
}
