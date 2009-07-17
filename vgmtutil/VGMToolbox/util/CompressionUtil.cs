using System;
using System.IO;
using System.Reflection;
using SevenZip;

using Ionic.Zip;
using Ionic.Zlib;

namespace VGMToolbox.util
{
    public class CompressionUtil
    {
        public static string ZLIB_DECOMPRESS_OUTPUT_EXTENSION = ".zlibx";
        public static string ZLIB_COMPRESS_OUTPUT_EXTENSION = ".zlib";
        public static readonly string SEVEN_ZIP_DLL = 
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "7z.dll");
        
        public static string[] GetFileList(string pPath)
        {
            string[] filenames = null;
            SevenZipExtractor sevenZipExtractor = null;

            if (File.Exists(pPath))
            {
                SevenZipExtractor.SetLibraryPath(SEVEN_ZIP_DLL);

                try
                {
                    sevenZipExtractor = new SevenZipExtractor(pPath);
                    filenames = new string[sevenZipExtractor.ArchiveFileNames.Count];

                    int i = 0;
                    foreach (string f in sevenZipExtractor.ArchiveFileNames)
                    {
                        filenames[i++] = f;
                    }
                }
                catch (System.ArgumentException)
                {
                    // ignore unsupported formats
                }
                finally
                {
                    if (sevenZipExtractor != null)
                    {
                        sevenZipExtractor.Dispose();
                    }
                }
            }

            return filenames;
        }

        public static string[] GetUpperCaseFileList(string pPath)
        {
            string[] filenames = null;
            SevenZipExtractor sevenZipExtractor = null;

            if (File.Exists(pPath))
            {
                SevenZipExtractor.SetLibraryPath(SEVEN_ZIP_DLL);

                try
                {
                    sevenZipExtractor = new SevenZipExtractor(pPath);
                    filenames = new string[sevenZipExtractor.ArchiveFileNames.Count];

                    int i = 0;
                    foreach (string f in sevenZipExtractor.ArchiveFileNames)
                    {
                        filenames[i++] = f.ToUpper();
                    }
                }
                catch (System.ArgumentException)
                {
                    // ignore unsupported formats
                }
                finally
                {
                    if (sevenZipExtractor != null)
                    {
                        sevenZipExtractor.Dispose();
                    }
                }
            }

            return filenames;
        }

        public static void ExtractFileFromArchive(string pArchivePath, string pFileName, string pOutputPath)
        {
            SevenZipExtractor sevenZipExtractor = null;
            string outputDir;
            
            if (File.Exists(pArchivePath))
            {
                try
                {
                    if (!String.IsNullOrEmpty(pOutputPath))
                    {
                        outputDir = pOutputPath;
                    }
                    else
                    {
                        outputDir = Path.Combine(Path.GetDirectoryName(pArchivePath), Path.GetFileNameWithoutExtension(pArchivePath));
                    }

                    if (!Directory.Exists(outputDir))
                    {
                        Directory.CreateDirectory(outputDir);
                    }

                    SevenZipExtractor.SetLibraryPath(SEVEN_ZIP_DLL);
                    sevenZipExtractor = new SevenZipExtractor(pArchivePath);
                    sevenZipExtractor.ExtractFile(pFileName, outputDir, true);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    if (sevenZipExtractor != null)
                    {
                        sevenZipExtractor.Dispose();
                    }
                }                
            }
        }
        
        public static void CompressFolderWith7zip(string pSourcePath, string pArchiveName)
        {
            SevenZipCompressor.SetLibraryPath(SEVEN_ZIP_DLL);
            SevenZipCompressor compressor = new SevenZipCompressor();
            compressor.CompressionLevel = SevenZip.CompressionLevel.Ultra;
            compressor.CompressDirectory(pSourcePath, pArchiveName, true);
        }

        public static void ExtractAllFilesFromZipFile(string pZipFilePath, string pOutputFolder)
        {
            using (ZipFile zip = ZipFile.Read(pZipFilePath))
            {
                zip.ExtractAll(pOutputFolder);
            }
        }

        public static void AddFileToZipFile(string pZipFileName, string pNewEntrySourceFileName, string pNewEntryDestinationName)
        {            
            ZipFile zf;

            // create or open zip file
            if (File.Exists(pZipFileName))
            {
                zf = ZipFile.Read(pZipFileName);
            }
            else
            {
                zf = new ZipFile(pZipFileName);
            }

            zf.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;

            using (FileStream fs = File.OpenRead(pNewEntrySourceFileName))
            {
                zf.AddEntry(Path.GetFileName(pNewEntryDestinationName), Path.GetDirectoryName(pNewEntryDestinationName), fs);
                zf.Save();
            }           
        }

        public static void DecompressZlibStreamToFile(Stream pStream, string pOutputFilePath)
        { 
            DecompressZlibStreamToFile(pStream, pOutputFilePath, 0);
        }

        public static void DecompressZlibStreamToFile(Stream pStream, string pOutputFilePath, long pStartingOffset)
        {
            using (FileStream outFs = new FileStream(pOutputFilePath, FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter bw = new BinaryWriter(outFs))
                {
                    pStream.Position = pStartingOffset;
                    
                    using (ZlibStream zs = new ZlibStream(pStream, CompressionMode.Decompress, true))
                    {
                        int read;
                        byte[] data = new byte[Constants.FILE_READ_CHUNK_SIZE];

                        while ((read = zs.Read(data, 0, data.Length)) > 0)
                        {
                            bw.Write(data, 0, read);
                        }
                    }
                }
            }
        }

        public static void CompressStreamToZlibFile(Stream pStream, string pOutputFilePath)
        { 
            CompressStreamToZlibFile(pStream, pOutputFilePath, 0);
        }
        
        public static void CompressStreamToZlibFile(Stream pStream, string pOutputFilePath, long pStartingOffset)
        {
            using (FileStream outFs = new FileStream(pOutputFilePath, FileMode.Create, FileAccess.Write))
            {
                using (BinaryReader br = new BinaryReader(pStream))
                {
                    pStream.Position = pStartingOffset;

                    using (ZlibStream zs = new ZlibStream(outFs, CompressionMode.Compress, Ionic.Zlib.CompressionLevel.BestCompression, true))
                    {
                        int read;
                        byte[] data = new byte[Constants.FILE_READ_CHUNK_SIZE];

                        while ((read = br.Read(data, 0, data.Length)) > 0)
                        {
                            zs.Write(data, 0, read);
                        }

                        zs.Flush();
                    }
                }
            }
        }

        public static void GzipEntireFile(string pFileName)
        {
            string tempFileName;
                        
            if (File.Exists(pFileName))
            {
                using (FileStream fs = File.OpenRead(pFileName))
                {
                    tempFileName = Path.GetTempFileName();

                    using (FileStream outFs = File.OpenWrite(tempFileName))
                    {
                        using (GZipStream gs = new GZipStream(outFs, CompressionMode.Compress, 
                            Ionic.Zlib.CompressionLevel.BestCompression))
                        {
                            int read;
                            byte[] data = new byte[Constants.FILE_READ_CHUNK_SIZE];

                            while ((read = fs.Read(data, 0, data.Length)) > 0)
                            {
                                gs.Write(data, 0, read);
                            }
                        }
                    }
                }

                File.Delete(pFileName);
                File.Move(tempFileName, pFileName);
            }
        }
    }
}
