using System;
using System.IO;
using VGMToolbox.util;

namespace manakut
{
    class Program
    {
        static void Main(string[] args)
        {
            const string LBA_SWITCH = "/LBA";

            string inFilename;
            string outFilename;
            string startOffset;
            string cutSize;

            string fullInputPath;
            string fullOutputPath;

            long longStartOffset;
            long longCutSize;

            bool doLba = false;

            if (args.Length < 3)
            {
                Console.WriteLine("Usage: manakut.exe <infile> <outfile> <start offset> <cut size> [/lba]");
                Console.WriteLine("   or: manakut.exe <infile> <outfile> <start offset> [/lba]");
                Console.WriteLine();
                Console.WriteLine("The 4 parameter option will read from <start offset> to EOF.");
                Console.WriteLine("Use the /lba switch to multiply start offset by 0x800.");
                Console.WriteLine(String.Format("The only current limitation is that cut size cannot exceed [{0}].", int.MaxValue.ToString()));
            }
            else
            {

                inFilename = args[0];
                outFilename = args[1];
                startOffset = args[2];


                fullInputPath = Path.GetFullPath(inFilename);
                fullOutputPath = Path.GetFullPath(outFilename);

                if (File.Exists(fullInputPath))
                {

                    using (FileStream fs = File.OpenRead(fullInputPath))
                    {

                        if (startOffset.StartsWith("0x"))
                        {
                            startOffset = startOffset.Substring(2);
                            longStartOffset = long.Parse(startOffset, System.Globalization.NumberStyles.HexNumber, null);
                        }
                        else
                        {
                            longStartOffset = long.Parse(startOffset, System.Globalization.NumberStyles.Integer, null);
                        }

                        // check for LBA switch
                        if ((args.Length > 4) && (args[4].ToUpper().Equals(LBA_SWITCH)))
                        {
                            doLba = true;
                        }
                        else if ((args.Length > 3) && (args[3].ToUpper().Equals(LBA_SWITCH)))
                        {
                            doLba = true;
                        }

                        if ((args.Length > 3) && (!args[3].ToUpper().Equals(LBA_SWITCH)))
                        {
                            cutSize = args[3];

                            if (cutSize.StartsWith("0x"))
                            {
                                cutSize = cutSize.Substring(2);
                                longCutSize = long.Parse(cutSize, System.Globalization.NumberStyles.HexNumber, null);
                            }
                            else
                            {
                                longCutSize = long.Parse(cutSize, System.Globalization.NumberStyles.Integer, null);
                            }
                        }
                        else
                        {
                            longCutSize = fs.Length - longStartOffset;
                        }

                        // set LBA values
                        if (doLba)
                        {
                            longStartOffset *= 0x800;
                        }

                        if (longCutSize > (long)int.MaxValue)
                        {
                            Console.WriteLine(String.Format("Sorry that cut size is too large: {0}", longCutSize.ToString()));
                        }
                        else
                        {
                            ParseFile.ExtractChunkToFile(fs, longStartOffset, (int)longCutSize, fullOutputPath);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("File Not Found: " + fullInputPath);
                }
            }
        }
    }
}
