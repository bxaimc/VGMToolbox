using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using VGMToolbox.format.sdat;

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
    }
}
