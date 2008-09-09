using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using VGMToolbox.format;

namespace VGMToolbox.tools
{
    class NsfeToM3u
    {
        public NsfeToM3u() { }
        
        public static void BuildNezPlugM3uFiles(string[] pPaths)
        {
            foreach (string path in pPaths)
            {
                if (File.Exists(path))
                { 
                    buildNezPlugM3u(path);
                }
            }
        
        }

        private static void buildNezPlugM3u(string pPath)
        {
            FileStream fs = File.OpenRead(pPath);
            Type formatType = FormatUtil.getObjectType(fs);

            if (formatType != null && formatType.Name.Equals("Nsfe"))
            {
                StreamWriter sw;
                string filename = Path.GetFileName(pPath);
                                
                Nsfe nsfeData = new Nsfe();
                fs.Seek(0, SeekOrigin.Begin);
                nsfeData.Initialize(fs);



                string[] playlist = nsfeData.Playlist.Split(',');                

                string outputFile = pPath.Substring(0, pPath.Length - Path.GetExtension(pPath).Length) + ".m3u";
                if (File.Exists(outputFile))
                {
                    File.Delete(outputFile);
                }
                sw = File.CreateText(outputFile);
                
                sw.WriteLine("#######################################################");
                sw.WriteLine("#");
                sw.WriteLine("# Game: " + nsfeData.SongName);
                sw.WriteLine("# Artist: " + nsfeData.SongArtist);
                sw.WriteLine("# Copyright: " + nsfeData.SongCopyright);
                sw.WriteLine("#");
                sw.WriteLine("#######################################################");
                sw.WriteLine();
                
                for (int i = 0; i < playlist.Length; i++)
                {
                    int index = int.Parse(playlist[i].Trim());
                    
                    int tempTime = nsfeData.Times[index];
                    int timeMinutes = tempTime / 60000;
                    int timeSeconds = ((tempTime - (timeMinutes * 60000)) % 60000) / 1000;

                    int tempFade = nsfeData.Fades[index];
                    int fadeMinutes = tempFade / 60000;
                    int fadeSeconds = ((tempFade - (fadeMinutes * 60000)) % 60000) / 1000;                    

                    sw.WriteLine(Path.GetFileNameWithoutExtension(pPath) + ".nsf" + "::NSF," +
                        (index + 1) + "," +
                        nsfeData.TrackLabels[index].Trim() + "," +
                        timeMinutes + ":" + timeSeconds.ToString("d2") + "," +
                        "," +
                        fadeMinutes + ":" + fadeSeconds.ToString("d2") + ",");                

                }




                sw.Close();
                sw.Dispose();
            }
        }
    }
}
