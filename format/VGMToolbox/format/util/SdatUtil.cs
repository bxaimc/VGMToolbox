using System;
using System.Collections;
using System.IO;

using VGMToolbox.format.sdat;
using VGMToolbox.util;

namespace VGMToolbox.format.util
{
    public class SdatUtil
    {
        public static string ExtractSdat(string pSdatPath)
        {
            string fullPath = Path.GetFullPath(pSdatPath);
            string outputPath = null;

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException(String.Format("Cannot find file <{0}>", fullPath));
            }
            else
            {
                using (FileStream fs = File.OpenRead(fullPath))
                {
                    Type dataType = FormatUtil.getObjectType(fs);
                    
                    if (dataType != null && dataType.Name.Equals("Sdat"))
                    {
                        string filePrefix = Path.GetFileNameWithoutExtension(fullPath);
                        outputPath = Path.Combine(Path.GetDirectoryName(fullPath), filePrefix);
                        
                        Sdat sdat = new Sdat();
                        sdat.Initialize(fs, fullPath);
                        sdat.ExtractSseqs(fs, outputPath);
                        sdat.ExtractStrms(fs, outputPath);
                        sdat.BuildSmap(outputPath, filePrefix);
                    }
                }                                
            }
            return outputPath;
        }

        public static string[] ExtractSdatsFromFile(string pPath, string pDirectorySuffix)
        {
            ArrayList extractedSdatPaths = new ArrayList();

            string fullPath = Path.GetFullPath(pPath);
            string outputPath;
            string filePrefix = Path.GetFileNameWithoutExtension(fullPath);
            if (!String.IsNullOrEmpty(pDirectorySuffix))
            {
                filePrefix += pDirectorySuffix;
            }

            int sdatIndex = 0;
            long sdatOffset;
            long previousOffset;

            byte[] sdatSizeBytes;
            int sdatSize;

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException(String.Format("Cannot find file <{0}>", fullPath));
            }
            else
            {
                using (FileStream fs = File.Open(fullPath, FileMode.Open, FileAccess.Read))
                {
                    previousOffset = 0;

                    while ((sdatOffset = ParseFile.GetNextOffset(fs, previousOffset, Sdat.ASCII_SIGNATURE)) != -1)
                    {
                        sdatSizeBytes = ParseFile.parseSimpleOffset(fs, sdatOffset + 8, 4);
                        sdatSize = BitConverter.ToInt32(sdatSizeBytes, 0);

                        outputPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(pPath), Path.Combine(filePrefix,
                            String.Format("sound_data_{0}.sdat", sdatIndex++.ToString("X2")))));

                        ParseFile.ExtractChunkToFile(fs, sdatOffset, sdatSize, outputPath);

                        extractedSdatPaths.Add(outputPath);

                        previousOffset = sdatOffset + sdatSize;
                    }
                }
            }

            // return paths of extracted SDATs
            if (extractedSdatPaths.Count > 0)
            {
                return (string[])extractedSdatPaths.ToArray(typeof(string));
            }
            else
            {
                return null;
            }
        }
    }
}
