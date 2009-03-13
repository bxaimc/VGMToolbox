using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using VGMToolbox.format.sdat;
using VGMToolbox.util;

namespace sdatfind
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                usage();
                return;
            }
            
            string filePath = Path.GetFullPath(args[0]);
            string filePrefix = Path.GetFileNameWithoutExtension(filePath) + "_sdatfind";
            string outputPath;

            int sdatIndex = 0;
            long sdatOffset;
            long previousOffset;

            byte[] sdatSizeBytes;
            int sdatSize;

            // get file path and check it exists
            Console.WriteLine("Checking if file exists.");

            filePath = Path.GetFullPath(args[0]);

            if (!File.Exists(filePath))
            {
                Console.WriteLine(String.Format("File <{0}> not found.", filePath));
                return;
            }

            // open file and extract the sdat
            Console.WriteLine("Extracting SDATs.");

            try
            {
                using (FileStream fs = File.Open(Path.GetFullPath(filePath), FileMode.Open, FileAccess.Read))
                {
                    previousOffset = 0;

                    while ((sdatOffset = ParseFile.GetNextOffset(fs, previousOffset, Sdat.ASCII_SIGNATURE)) != -1)
                    {
                        sdatSizeBytes = ParseFile.parseSimpleOffset(fs, sdatOffset + 8, 4);
                        sdatSize = BitConverter.ToInt32(sdatSizeBytes, 0);

                        outputPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(filePath), Path.Combine(filePrefix,
                            String.Format("sound_data_{0}.sdat", sdatIndex++.ToString("X2")))));

                        ParseFile.ExtractChunkToFile(fs, sdatOffset, sdatSize, outputPath);

                        previousOffset = sdatOffset + sdatSize;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("Error processing <{0}>.  Error received: ", filePath) + ex.Message);
            }                                
        }

        private static void usage()
        {
            Console.WriteLine("sdatfind.exe filename");
        }
    }
}
