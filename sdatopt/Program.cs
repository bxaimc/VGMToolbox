using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using VGMToolbox.format;
using VGMToolbox.format.sdat;
using VGMToolbox.util;

namespace sdatopt
{
    class Program
    {
        static void Main(string[] args)
        {
            if ((args.Length > 3) || (args.Length < 2))
            {
                Console.WriteLine("usage: sdatopt.exe filename start_sequence end_sequence");
                Console.WriteLine("       sdatopt.exe filename ALL");
            }
            else
            {
                Sdat sdat = null;
                
                string sdatDirectory;
                string sdatOptimizingFileName;
                string sdatOptimizingPath;

                string sdatCompletedFileName;
                string sdatCompletedPath;
                
                string filename = Path.GetFullPath(args[0]);
                int startSequence = Sdat.NO_SEQUENCE_RESTRICTION;
                int endSequence = Sdat.NO_SEQUENCE_RESTRICTION;

                sdatDirectory = Path.GetDirectoryName(filename);
                sdatOptimizingFileName = String.Format("{0}_OPTIMIZING.sdat",
                    Path.GetFileNameWithoutExtension(filename));
                sdatOptimizingPath = Path.Combine(sdatDirectory, sdatOptimizingFileName);

                sdatCompletedFileName = String.Format("{0}_OPTIMIZED.sdat",
                    Path.GetFileNameWithoutExtension(filename));
                sdatCompletedPath = Path.Combine(sdatDirectory, sdatCompletedFileName);

                File.Copy(filename, sdatOptimizingPath, true);

                using (FileStream fs = File.Open(sdatOptimizingPath, FileMode.Open, FileAccess.ReadWrite))
                {
                    Type dataType = FormatUtil.getObjectType(fs);

                    if (dataType != null && dataType.Name.Equals("Sdat"))
                    {
                        sdat = new Sdat();
                        sdat.Initialize(fs, sdatOptimizingPath);
                    }
                }

                if (sdat != null)
                {
                    if (!args[1].Trim().Equals("ALL"))
                    {
                        if (!String.IsNullOrEmpty(args[1]))
                        {
                            startSequence = (int)VGMToolbox.util.Encoding.GetIntFromString(args[1]);
                        }

                        if (!String.IsNullOrEmpty(args[2]))
                        {
                            endSequence = (int)VGMToolbox.util.Encoding.GetIntFromString(args[2]);
                        }
                    }
                    sdat.OptimizeForZlib(startSequence, endSequence);
                }

                File.Copy(sdatOptimizingPath, sdatCompletedPath, true);
                File.Delete(sdatOptimizingPath);

            }
        }
    }
}
