using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using VGMToolbox.format;
using VGMToolbox.util;

namespace VGMToolbox.tools.hoot
{
    class HootXmlBuilderWorker : BackgroundWorker
    {
        private int fileCount = 0;
        private int maxFiles = 0;
        ArrayList hootGamesArrayList = new ArrayList();

        public struct HootXmlBuilderStruct
        {
            public string[] pPaths;
            public int totalFiles;
            public bool combineOutput;
            public bool splitOutput;
        }

        public HootXmlBuilderWorker()
        {
            fileCount = 0;
            maxFiles = 0;
            hootGamesArrayList = new ArrayList();
            
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
        }


        private void buildXmlFiles(HootXmlBuilderStruct pHootXmlBuilderStruct, DoWorkEventArgs e)
        {            
            foreach (string path in pHootXmlBuilderStruct.pPaths)
            {                
                if (File.Exists(path))
                {
                    if (!CancellationPending)
                    {
                        this.buildFileHootGame(path, e);
                    }
                    else 
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                else if (Directory.Exists(path))
                {
                    this.buildDirectoryHootGame(path, e);

                    if (CancellationPending)
                    {
                        e.Cancel = true;
                        return;                        
                    }
                }                               
            }

            return;
        }

        private void buildDirectoryHootGame(string pPath, DoWorkEventArgs e)
        {
            foreach (string d in Directory.GetDirectories(pPath))
            {
                if (!CancellationPending)
                {
                    this.buildDirectoryHootGame(d, e);
                }
                else
                {
                    e.Cancel = true;
                    break;
                }
            }
            foreach (string f in Directory.GetFiles(pPath))
            {
                if (!CancellationPending)
                {
                    this.buildFileHootGame(f, e);
                    // fileCount++;
                }
                else
                {
                    e.Cancel = true;
                    break;
                }
            }                
        }

        private void buildFileHootGame(string pPath, DoWorkEventArgs e)
        {
            // Report Progress
            int progress = (++fileCount * 100) / maxFiles;
            Constants.ProgressStruct vProgressStruct = new Constants.ProgressStruct();
            vProgressStruct.newNode = null;
            vProgressStruct.filename = pPath;
            ReportProgress(progress, vProgressStruct);

          
            VGMToolbox.tools.hoot.game hootGame = null;

            try
            {
                FileStream fs = File.OpenRead(pPath);
                Type dataType = FormatUtil.getObjectType(fs);

                if (dataType != null)
                {
                    IFormat vgmData = (IFormat)Activator.CreateInstance(dataType);
                    vgmData.Initialize(fs);

                    if (!String.IsNullOrEmpty(vgmData.GetHootDriver()))
                    {
                        hootGame = new VGMToolbox.tools.hoot.game();

                        string setName = "GAME NAME";
                        if (!String.IsNullOrEmpty(vgmData.GetSongName()))
                        {
                            setName = vgmData.GetSongName();
                        }
                        hootGame.name = String.Format("[{0}] {1} ({2})",
                            vgmData.GetHootDriverAlias(), setName, "JP PLACE HOLDER");

                        hootGame.driver = new VGMToolbox.tools.hoot.driver();
                        hootGame.driver.type = vgmData.GetHootDriverType();
                        hootGame.driver.Value = vgmData.GetHootDriver();

                        hootGame.driveralias = new VGMToolbox.tools.hoot.driveralias();
                        hootGame.driveralias.type = vgmData.GetHootDriverAlias();
                        hootGame.driveralias.Value = "COMPANY";

                        hootGame.romlist = new VGMToolbox.tools.hoot.romlist();
                        hootGame.romlist.archive = "INSERT ARCHIVE NAME HERE";
                        hootGame.romlist.rom = new VGMToolbox.tools.hoot.rom[1];
                        hootGame.romlist.rom[0] = new VGMToolbox.tools.hoot.rom();
                        hootGame.romlist.rom[0].type = "code";
                        hootGame.romlist.rom[0].Value = Path.GetFileName(pPath);

                        hootGame.titlelist = new VGMToolbox.tools.hoot.title[vgmData.GetTotalSongs()];
                        int j = 0;
                        int totalSongs = vgmData.GetTotalSongs();
                        // for (int i = vgmData.GetStartingSong(); i < (vgmData.GetStartingSong() + vgmData.GetTotalSongs()); i++)
                        for (int i = 0; i < totalSongs; i++)
                        {
                            VGMToolbox.tools.hoot.title hootTitle = new VGMToolbox.tools.hoot.title();
                            hootTitle.code = "0x" + i.ToString("X2");
                            hootTitle.Value = "BGM #" + i.ToString("X2");

                            hootGame.titlelist[j] = hootTitle;
                            j++;
                        }

                        hootGamesArrayList.Add(hootGame);
                    }
                }

                fs.Close();
                fs.Dispose();
            }
            catch (Exception ex)
            {
                vProgressStruct = new Constants.ProgressStruct();
                vProgressStruct.newNode = null;
                vProgressStruct.errorMessage = String.Format("Error processing <{0}>.  Error received: ", pPath) + ex.Message;
                ReportProgress(progress, vProgressStruct);
            }            
        }    

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            HootXmlBuilderStruct hootXmlBuilderStruct = (HootXmlBuilderStruct)e.Argument;
            maxFiles = hootXmlBuilderStruct.totalFiles;
            
            this.buildXmlFiles(hootXmlBuilderStruct, e);
            VGMToolbox.tools.hoot.game[] hootGames = (VGMToolbox.tools.hoot.game[])hootGamesArrayList.ToArray(typeof(VGMToolbox.tools.hoot.game));
            
            if (hootGames.Length > 0)
            {
                string outputPath;
                
                // Use to suppress namespace attributes
                XmlSerializerNamespaces namespaceSerializer = new XmlSerializerNamespaces();
                namespaceSerializer.Add("", "");

                if (hootXmlBuilderStruct.combineOutput)
                {
                    outputPath = "." + Path.DirectorySeparatorChar + "hoot" +
                        Path.DirectorySeparatorChar + "[combined] hootgame.xml";
                    
                    XmlSerializer serializer = new XmlSerializer(hootGames.GetType());
                    TextWriter textWriter = new StreamWriter(outputPath);
                    serializer.Serialize(textWriter, hootGames, namespaceSerializer);
                    textWriter.Close();
                    textWriter.Dispose();
                }

                if (hootXmlBuilderStruct.splitOutput)
                {
                    foreach (VGMToolbox.tools.hoot.game g in hootGames)
                    {
                        outputPath = "." + Path.DirectorySeparatorChar + "hoot" +
                            Path.DirectorySeparatorChar + g.romlist.rom[0].Value + ".xml";

                        XmlSerializer serializer = new XmlSerializer(g.GetType());
                        TextWriter textWriter = new StreamWriter(outputPath);
                        serializer.Serialize(textWriter, g, namespaceSerializer);
                        textWriter.Close();
                        textWriter.Dispose();
                    }
                
                }
            }
        }        
    }
}
