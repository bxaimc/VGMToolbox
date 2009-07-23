using System;
using System.IO;
using System.Reflection;
using SevenZip;

using Ionic.Zip;
using Ionic.Zlib;

namespace VGMToolbox.util
{
    /// <summary>
    /// Class containing static functions for compresson related tasks
    /// </summary>
    public class CompressionUtil
    {
        /// <summary>
        /// File extension used to output decompressed zlib data.
        /// </summary>
        public static string ZLIB_DECOMPRESS_OUTPUT_EXTENSION = ".zlibx";
        /// <summary>
        /// File extension used to output compressed zlib data.
        /// </summary>
        public static string ZLIB_COMPRESS_OUTPUT_EXTENSION = ".zlib";

        /// <summary>
        /// File extension used to output decompressed gzip data.
        /// </summary>
        public static string GZIP_DECOMPRESS_OUTPUT_EXTENSION = ".gzipx";
        /// <summary>
        /// File extension used to output compressed gzip data.
        /// </summary>
        public static string GZIP_COMPRESS_OUTPUT_EXTENSION = ".gz";
        
        /// <summary>
        /// Path to the included 7z.dll file.
        /// </summary>
        public static readonly string SEVEN_ZIP_DLL = 
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "7z.dll");
        
        /// <summary>
        /// Get a list of files inside an archive file.
        /// </summary>
        /// <param name="pPath">Path to the archive file.</param>
        /// <returns>An array of strings containing a list of files inside the archive.</returns>
        /// <remarks>Must be an archive supported by 7z.dll.</remarks>
        public static string[] GetFileList(string pPath)
        {
            string archivePath = Path.GetFullPath(pPath);
            string[] filenames = null;
            SevenZipExtractor sevenZipExtractor = null;


            if (File.Exists(archivePath))
            {
                SevenZipExtractor.SetLibraryPath(SEVEN_ZIP_DLL);

                try
                {
                    sevenZipExtractor = new SevenZipExtractor(archivePath);
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

        /// <summary>
        /// Get an uppercase list of files inside an archive file.  Must be an archive supported by 7z.dll.
        /// </summary>
        /// <param name="pPath">Path to the archive file.</param>
        /// <returns>An array of strings containing an uppercase list of files inside the archive.</returns>
        /// <remarks>Must be an archive supported by 7z.dll.</remarks>
        public static string[] GetUpperCaseFileList(string pPath)
        {
            string archivePath = Path.GetFullPath(pPath);
            string[] filenames = null;
            SevenZipExtractor sevenZipExtractor = null;

            if (File.Exists(archivePath))
            {
                SevenZipExtractor.SetLibraryPath(SEVEN_ZIP_DLL);

                try
                {
                    sevenZipExtractor = new SevenZipExtractor(archivePath);
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

        /// <summary>
        /// Extracts a file from an archive.  The file will be output to a subfolder in the archive's directory; 
        /// named with the original archive name.
        /// </summary>
        /// <param name="pArchivePath">Path to the archive file.</param>
        /// <param name="pFileName">Name of file to extract.</param>
        /// <remarks>Must be an archive supported by 7z.dll.</remarks>
        public static void ExtractFileFromArchive(string pArchivePath, string pFileName)
        { 
            ExtractFileFromArchive(pArchivePath, pFileName, String.Empty);
        }

        /// <summary>
        /// Extracts a file from an archive.
        /// </summary>
        /// <param name="pArchivePath">Path to the archive file.</param>
        /// <param name="pFileName">Name of file to extract.</param>
        /// <param name="pOutputPath">Folder to output the file to.  If empty or null, 
        /// the file will be output to a subfolder in the archive's directory; 
        /// named with the original archive name.</param>
        /// <remarks>Must be an archive supported by 7z.dll.</remarks>
        public static void ExtractFileFromArchive(string pArchivePath, string pFileName, string pOutputPath)
        {
            string archivePath = Path.GetFullPath(pArchivePath);
            SevenZipExtractor sevenZipExtractor = null;
            string outputDir;

            if (File.Exists(archivePath))
            {
                try
                {
                    if (!String.IsNullOrEmpty(pOutputPath))
                    {
                        outputDir = pOutputPath;
                    }
                    else
                    {
                        outputDir = Path.Combine(Path.GetDirectoryName(archivePath), Path.GetFileNameWithoutExtension(archivePath));
                    }

                    if (!Directory.Exists(outputDir))
                    {
                        Directory.CreateDirectory(outputDir);
                    }

                    SevenZipExtractor.SetLibraryPath(SEVEN_ZIP_DLL);
                    sevenZipExtractor = new SevenZipExtractor(archivePath);
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
        
        /// <summary>
        /// Compress input folder (recursively) with 7zip Ultra compression.
        /// </summary>
        /// <param name="pSourcePath">Source directory to compress.</param>
        /// <param name="pArchiveName">Fully rooted output archive name.</param>
        public static void CompressFolderWith7zip(string pSourcePath, string pArchiveName)
        {
            SevenZipCompressor.SetLibraryPath(SEVEN_ZIP_DLL);
            SevenZipCompressor compressor = new SevenZipCompressor();
            compressor.CompressionLevel = SevenZip.CompressionLevel.Ultra;
            compressor.CompressDirectory(pSourcePath, pArchiveName, true);
        }

        /// <summary>
        /// Extract all files from the input .zip compressed file.
        /// </summary>
        /// <param name="pZipFilePath">Fully rooted path to the .zip file.</param>
        /// <param name="pOutputFolder">Fully rooted path to the output folder to place the deompressed files.</param>
        public static void ExtractAllFilesFromZipFile(string pZipFilePath, string pOutputFolder)
        {
            using (ZipFile zip = ZipFile.Read(pZipFilePath))
            {
                zip.ExtractAll(pOutputFolder);
            }
        }

        /// <summary>
        /// Add a file to the input .zip file.
        /// </summary>
        /// <param name="pZipFileName">Fully rooted path to the .zip file to add files to.</param>
        /// <param name="pNewEntrySourceFileName">Fully rooted path to the file to insert.</param>
        /// <param name="pNewEntryDestinationName">Path within the .zip file to insert the new file as.</param>
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

        /// <summary>
        /// Decompress a zlib compressed section of a stream to file.  Data must begin at offset 0.
        /// </summary>
        /// <param name="pStream">Stream containing zlib compressed bytes.</param>
        /// <param name="pOutputFilePath">Fully rooted output file name to output the decompressed data to.</param>
        public static void DecompressZlibStreamToFile(Stream pStream, string pOutputFilePath)
        { 
            DecompressZlibStreamToFile(pStream, pOutputFilePath, 0);
        }

        /// <summary>
        /// Decompress a zlib compressed section of a stream to file.
        /// </summary>
        /// <param name="pStream">Stream containing zlib compressed bytes.</param>
        /// <param name="pOutputFilePath">Fully rooted output file name to output the decompressed data to.</param>
        /// <param name="pStartingOffset">Offset to begin reading data from.</param>
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
                        byte[] data = new byte[Constants.FileReadChunkSize];

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
                        byte[] data = new byte[Constants.FileReadChunkSize];

                        while ((read = br.Read(data, 0, data.Length)) > 0)
                        {
                            zs.Write(data, 0, read);
                        }

                        zs.Flush();
                    }
                }
            }
        }

        /// <summary>
        /// Compress an entire file using gzip compression.
        /// </summary>
        /// <param name="pFileName">Fully rooted file name of file to compress.</param>
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
                            byte[] data = new byte[Constants.FileReadChunkSize];

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

        /// <summary>
        /// Decompress a gzip compressed section of a stream to file.
        /// </summary>
        /// <param name="pStream">Stream containing gzip compressed bytes.</param>
        /// <param name="pOutputFilePath">Fully rooted output file name to output the decompressed data to.</param>
        /// <param name="pStartingOffset">Offset to begin reading data from.</param>
        public static void DecompressGzipStreamToFile(Stream pStream, string pOutputFilePath, long pStartingOffset)
        {
            using (FileStream outFs = new FileStream(pOutputFilePath, FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter bw = new BinaryWriter(outFs))
                {
                    pStream.Position = pStartingOffset;

                    using (GZipStream gs = new GZipStream(pStream, CompressionMode.Decompress, true))
                    {
                        int read;
                        byte[] data = new byte[Constants.FileReadChunkSize];

                        while ((read = gs.Read(data, 0, data.Length)) > 0)
                        {
                            bw.Write(data, 0, read);
                        }
                    }
                }
            }
        }

        public static void CompressStreamToGzipFile(Stream pStream, string pOutputFilePath, long pStartingOffset)
        {
            using (FileStream outFs = new FileStream(pOutputFilePath, FileMode.Create, FileAccess.Write))
            {
                using (BinaryReader br = new BinaryReader(pStream))
                {
                    pStream.Position = pStartingOffset;

                    using (GZipStream gs = new GZipStream(outFs, CompressionMode.Compress, Ionic.Zlib.CompressionLevel.BestCompression, true))
                    {
                        int read;
                        byte[] data = new byte[Constants.FileReadChunkSize];

                        while ((read = br.Read(data, 0, data.Length)) > 0)
                        {
                            gs.Write(data, 0, read);
                        }

                        gs.Flush();
                    }
                }
            }
        }
    }
}
