using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

using VGMToolbox.format;
using VGMToolbox.format.util;
using VGMToolbox.plugin;

namespace VGMToolbox.tools.hoot
{
    class HootXmlBuilderWorker : AVgmtDragAndDropWorker, IVgmtBackgroundWorker
    {
        private ArrayList hootGamesArrayList = new ArrayList();

        public struct HootXmlBuilderStruct : IVgmtWorkerStruct
        {
            public bool combineOutput;
            public bool splitOutput;

            private string[] sourcePaths;
            public string[] SourcePaths
            {
                get { return sourcePaths; }
                set { sourcePaths = value; }
            }
        }

        public HootXmlBuilderWorker() : 
            base()
        {
            hootGamesArrayList = new ArrayList();
        }

        protected override void DoTaskForFile(string pPath, IVgmtWorkerStruct pHootXmlBuilderStruct, DoWorkEventArgs e)
        {          
            VGMToolbox.format.hoot.game hootGame = null;

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
                        hootGame = new VGMToolbox.format.hoot.game();

                        string setName = "GAME NAME";
                        if (!String.IsNullOrEmpty(vgmData.GetSongName()))
                        {
                            setName = vgmData.GetSongName();
                        }
                        hootGame.name = String.Format("[{0}] {1} ({2})",
                            vgmData.GetHootDriverAlias(), setName, "JP PLACE HOLDER");

                        hootGame.driver = new VGMToolbox.format.hoot.driver();
                        hootGame.driver.type = vgmData.GetHootDriverType();
                        hootGame.driver.Value = vgmData.GetHootDriver();

                        hootGame.driveralias = new VGMToolbox.format.hoot.driveralias();
                        hootGame.driveralias.type = vgmData.GetHootDriverAlias();
                        hootGame.driveralias.Value = "COMPANY";

                        hootGame.romlist = new VGMToolbox.format.hoot.romlist();
                        hootGame.romlist.archive = "INSERT ARCHIVE NAME HERE";
                        hootGame.romlist.rom = new VGMToolbox.format.hoot.rom[1];
                        hootGame.romlist.rom[0] = new VGMToolbox.format.hoot.rom();
                        hootGame.romlist.rom[0].type = "code";
                        hootGame.romlist.rom[0].Value = Path.GetFileName(pPath);

                        hootGame.titlelist = new VGMToolbox.format.hoot.title[vgmData.GetTotalSongs()];
                        int j = 0;
                        int totalSongs = vgmData.GetTotalSongs();
                        VGMToolbox.format.hoot.title hootTitle;

                        for (int i = 0; i < totalSongs; i++)
                        {
                            hootTitle = new VGMToolbox.format.hoot.title();
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
                this.progressStruct.ErrorMessage = String.Format("Error processing <{0}>.  Error received: ", pPath) + ex.Message;
                ReportProgress(this.Progress, this.progressStruct);
            }            
        }    

        protected override void OnDoWork(DoWorkEventArgs e)
        {            
            base.OnDoWork(e);
            
            HootXmlBuilderStruct hootXmlBuilderStruct = (HootXmlBuilderStruct)e.Argument;
            VGMToolbox.format.hoot.game[] hootGames = 
                (VGMToolbox.format.hoot.game[])this.hootGamesArrayList.ToArray(typeof(VGMToolbox.format.hoot.game));
            
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
                    foreach (VGMToolbox.format.hoot.game g in hootGames)
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
