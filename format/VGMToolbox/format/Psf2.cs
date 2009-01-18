using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VGMToolbox.format
{
    public class Psf2 : Xsf
    {
        public const string SQ_FILE = "SQ.IRX";
        
        public struct Psf2IniSqIrxStruct
        {
            public string SqFileName;
            public string HdFileName;
            public string BdFileName;

            public string SequenceNumber;
            public string TimerTickInterval;
            public string Reverb;
            public string Depth;
            public string Tempo;
            public string Volume;
        }

        public static Psf2IniSqIrxStruct ParseClsIniFile(Stream pStream)
        {
            string currentLine;
            string[] splitLine;
            string[] splitItem;
            char[] splitDelimeters = new char[] {' '};
            char[] splitItemDelimeter = new char[] {'='};

            Psf2IniSqIrxStruct ret = new Psf2IniSqIrxStruct();

            // get original stream position in case needed by caller
            long originalStreamPosition = pStream.Position;
            
            pStream.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(pStream);

            while ((currentLine = sr.ReadLine()) != null)
            { 
                // check for the SQ.IRX line
                if (currentLine.Trim().ToUpper().StartsWith(SQ_FILE))
                {
                    splitLine = currentLine.Trim().Split(splitDelimeters, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string s in splitLine)
                    {
                        if (s.Contains("=")) // we have an option
                        {
                            splitItem = s.Trim().Split(splitItemDelimeter, StringSplitOptions.RemoveEmptyEntries);

                            switch (splitItem[0])
                            { 
                                case "-s":
                                    ret.SqFileName = splitItem[1].ToUpper();
                                    break;
                                case "-h":
                                    ret.HdFileName = splitItem[1].ToUpper();
                                    break;
                                case "-b":
                                    ret.BdFileName = splitItem[1].ToUpper();
                                    break;
                                case "-n":
                                    ret.SequenceNumber = splitItem[1];
                                    break;
                                case "-u":
                                    ret.TimerTickInterval = splitItem[1];
                                    break;
                                case "-r":
                                    ret.Reverb = splitItem[1];
                                    break;
                                case "-d":
                                    ret.Depth = splitItem[1];
                                    break;
                                case "-t":
                                    ret.Tempo = splitItem[1];
                                    break;
                                case "-v":
                                    ret.Volume = splitItem[1];
                                    break;
                            }
                        }
                    }

                    // add defaults if needed
                    if (String.IsNullOrEmpty(ret.BdFileName)) 
                    { 
                        ret.BdFileName = "DEFAULT.BD"; 
                    }
                    if (String.IsNullOrEmpty(ret.HdFileName)) 
                    { 
                        ret.BdFileName = "DEFAULT.HD"; 
                    }
                    if (String.IsNullOrEmpty(ret.SqFileName)) 
                    { 
                        ret.BdFileName = "DEFAULT.SQ"; 
                    }

                    break;
                }
            }

            // return to incoming position
            pStream.Seek(originalStreamPosition, SeekOrigin.Begin);

            return ret;
        }

        public static void WriteClsIniFile(Psf2IniSqIrxStruct pPsf2IniSqIrxStruct, string pOutputPath)
        {
            StringBuilder sqArguments;
            
            using (StreamWriter sw = new StreamWriter(File.Open(pOutputPath, FileMode.Create, FileAccess.Write)))
            {
                sw.WriteLine("libsd.irx");
                sw.WriteLine("modhsyn.irx");
                sw.WriteLine("modmidi.irx");

                sqArguments = new StringBuilder();

                // build sq.irx arguments                    
                sqArguments.Append(String.IsNullOrEmpty(pPsf2IniSqIrxStruct.Reverb) ?
                    " -r=5" : String.Format(" -r={0}", pPsf2IniSqIrxStruct.Reverb.Trim()));
                sqArguments.Append(String.IsNullOrEmpty(pPsf2IniSqIrxStruct.Depth) ?
                    " -d=16383" : String.Format(" -d={0}", pPsf2IniSqIrxStruct.Depth.Trim()));

                sqArguments.Append(String.IsNullOrEmpty(pPsf2IniSqIrxStruct.SequenceNumber) ?
                    String.Empty : String.Format(" -n={0}", pPsf2IniSqIrxStruct.SequenceNumber.Trim()));
                sqArguments.Append(String.IsNullOrEmpty(pPsf2IniSqIrxStruct.TimerTickInterval) ?
                    String.Empty : String.Format(" -u={0}", pPsf2IniSqIrxStruct.TimerTickInterval.Trim()));
                sqArguments.Append(String.IsNullOrEmpty(pPsf2IniSqIrxStruct.Tempo) ?
                    String.Empty : String.Format(" -t={0}", pPsf2IniSqIrxStruct.Tempo.Trim()));
                sqArguments.Append(String.IsNullOrEmpty(pPsf2IniSqIrxStruct.Volume) ?
                    String.Empty : String.Format(" -v={0}", pPsf2IniSqIrxStruct.Volume.Trim()));

                sqArguments.Append(String.Format(" -s={0} -h={1} -b={2}",
                    pPsf2IniSqIrxStruct.SqFileName, pPsf2IniSqIrxStruct.HdFileName, 
                    pPsf2IniSqIrxStruct.BdFileName));

                sw.WriteLine(String.Format("sq.irx {0}", sqArguments.ToString()));
                
                sw.Write(Environment.NewLine);
                sw.Close();
            }
        }
    }
}
