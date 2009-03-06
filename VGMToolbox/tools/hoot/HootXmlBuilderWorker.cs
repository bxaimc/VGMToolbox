using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Forms;

using VGMToolbox.format;
using VGMToolbox.format.util;
using VGMToolbox.util;

namespace VGMToolbox.tools.hoot
{
    class HootXmlBuilderWorker : BackgroundWorker
    {
        private int fileCount = 0;
        private int maxFiles = 0;
        private ArrayList hootGamesArrayList = new ArrayList();
        private Constants.ProgressStruct progressStruct;

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
            progressStruct = new Constants.ProgressStruct();
            
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
            this.progressStruct.Clear();
            this.progressStruct.filename = pPath;
            ReportProgress(progress, this.progressStruct);
          
            VGMToolbox.tools.hoot.game hootGame = null;

            try
            {
                FileStream fs = File.OpenRead(pPath);
                Type dataType = FormatUtil.getHootObjectType(fs);

                if (dataType != null)
                {
                    IHootFormat vgmData = (IHootFormat)Activator.CreateInstance(dataType);
                    vgmData.Initialize(fs, pPath);

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
                        VGMToolbox.tools.hoot.title hootTitle;

                        // for (int i = vgmData.GetStartingSong(); i < (vgmData.GetStartingSong() + vgmData.GetTotalSongs()); i++)
                        for (int i = 0; i < totalSongs; i++)
                        {
                            hootTitle = new VGMToolbox.tools.hoot.title();
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
                this.progressStruct.Clear();
                this.progressStruct.errorMessage = String.Format("Error processing <{0}>.  Error received: ", pPath) + ex.Message;
                ReportProgress(progress, this.progressStruct);
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
                    outputPath = Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + "hoot" +
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
                        outputPath = Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + "hoot" +
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
