using System;
using System.IO;
using System.Reflection;
using SevenZip;

namespace VGMToolbox.util
{
    public class CompressionUtil
    {
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

        public static void ExtractFileFromArchive(string pArchivePath, string pFileName)
        {
            SevenZipExtractor sevenZipExtractor = null;
            string outputDir;
            
            if (File.Exists(pArchivePath))
            {
                try
                {
                    outputDir = Path.Combine(Path.GetDirectoryName(pArchivePath), Path.GetFileNameWithoutExtension(pArchivePath));
                    
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
    }
}
