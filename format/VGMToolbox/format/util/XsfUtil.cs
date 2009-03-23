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

        public static string ExtractCompressedDataSection(string pPath, Xsf2ExeStruct pXsf2ExeStruct)
        {
            string outputFile = null;
            
            using (FileStream fs = File.OpenRead(pPath))
            {
                Type dataType = FormatUtil.getObjectType(fs);

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

            return outputFile;
        }

        public static string ExtractReservedSection(string pPath, Xsf2ExeStruct pXsf2ExeStruct)
        {
            string outputFile = null;

            using (FileStream fs = File.OpenRead(pPath))
            {
                Type dataType = FormatUtil.getObjectType(fs);

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

            using (FileStream fs = File.OpenRead(pPath))
            {
                Type dataType = FormatUtil.getObjectType(fs);

                if (dataType != null && dataType.Name.Equals("Xsf"))
                {
                    Psf2 psf2File = new Psf2();
                    psf2File.Initialize(fs, pPath);

                    if (psf2File.getFormat().Equals(Psf2.FORMAT_NAME_PSF2))
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
            }

            return outputDir;
        }

        // PSF
        public static bool IsPsxSepFile(string pPath)
        {
            return false;        
        }
        
        public static string ExtractPsxSequences(string pPath)
        {
            string outputPath = null;

            long previousOffset = 0;
            long offsetLocation = -1;
            long seqEndLocation = -1;

            using (FileStream fs = File.Open(pPath, FileMode.Open, FileAccess.Read))
            {
                while ((offsetLocation = ParseFile.GetNextOffset(fs, previousOffset, PsxSequence.ASCII_SIGNATURE)) > -1)
                {
                    // can modify to use spec values
                    seqEndLocation = ParseFile.GetNextOffset(fs, offsetLocation, PsxSequence.END_SEQUENCE);

                    previousOffset = seqEndLocation + PsxSequence.ASCII_SIGNATURE.Length;
                }
            }

            return outputPath;
        }

    }
}
