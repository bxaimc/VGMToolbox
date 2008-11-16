using System;
using System.IO;
using System.Text;

using VGMToolbox.format.sdat;

namespace sdatext
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath;
            string fileName;

            if (args.Length < 1)
            { 
                usage();
                return;
            }

            // get file path and check it exists
            Console.WriteLine("Checking if file exists.");
            
            filePath = args[0];

            if (!File.Exists(filePath))
            {
                Console.WriteLine(String.Format("File <{0}> not found.", filePath));
                return;
            }

            // open file and extract the sdat
            Console.WriteLine("Extracting SDAT.");
            
            FileStream fs = null;
            fileName = Path.GetFileNameWithoutExtension(filePath);
            string outputPath = Path.Combine(Path.GetDirectoryName(filePath), fileName);

            try
            {
                fs = File.OpenRead(filePath);
                Sdat sdat = new Sdat();

                sdat.Initialize(fs);
                sdat.BuildSmap(outputPath, fileName);
                sdat.ExtractSseqs(fs, outputPath);
                sdat.ExtractStrms(fs, outputPath);


                fs.Close();
                fs.Dispose();                           
            }
            catch (Exception _e)
            {
                Console.WriteLine(_e.Message);

                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }

            Console.WriteLine("Done!");
        }

        private static void usage()
        {
            Console.WriteLine("sdatext sdatfile");
        }
    }
}
