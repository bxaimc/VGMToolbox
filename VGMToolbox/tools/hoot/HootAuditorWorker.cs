using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Windows.Forms;

using VGMToolbox.format.hoot;
using VGMToolbox.plugin;

namespace VGMToolbox.tools.hoot
{
    class HootAuditorWorker : AVgmtDragAndDropWorker
    {
        private const string FILE_EXTENSION_XML = ".XML";
        private const string ROM_TYPE_SHELL = "SHELL";

        public struct HootAuditorStruct : IVgmtWorkerStruct
        {
            private string[] sourcePaths; // xml paths
            public string[] SourcePaths
            {
                get { return sourcePaths; }
                set { sourcePaths = value; }
            }
            
            public string[] SetArchivePaths;
        }


        protected override void DoTaskForFile(string pPath, IVgmtWorkerStruct pHootAuditorStruct, DoWorkEventArgs e)
        {
            HootAuditorStruct hootAuditorStruct = (HootAuditorStruct)pHootAuditorStruct;
            
            if (Path.GetExtension(pPath).ToUpper().Equals(FILE_EXTENSION_XML))
            {
                TreeNode rootNode = new TreeNode(Path.GetFileNameWithoutExtension(pPath));
                Dictionary<string, string[]> archiveContents = new Dictionary<string, string[]>();

                try
                {
                    gamelist hootGames = new gamelist();
                    XmlSerializer serializer = new XmlSerializer(typeof(gamelist));
                    using (TextReader textReader = new StreamReader(pPath))
                    {
                        hootGames = (gamelist)serializer.Deserialize(textReader);
                    }

                    string gameArchiveFileName;
                    TreeNode gameNode;
                    TreeNode romNode;
                    

                    foreach (game g in hootGames.Items)
                    {
                        gameArchiveFileName = g.romlist.archive + ".zip";
                        gameNode = new TreeNode(String.Format("{0} ({1})]", gameArchiveFileName, g.name));


                        foreach (rom r in g.romlist.rom)
                        {
                            if (!r.type.ToUpper().Equals(ROM_TYPE_SHELL))
                            {
                                romNode = new TreeNode(r.Value);
                                gameNode.Nodes.Add(romNode);
                            }
                        }
                        
                        rootNode.Nodes.Add(gameNode);
                    }

                    this.progressStruct.Clear();
                    this.progressStruct.Filename = pPath;
                    this.progressStruct.NewNode = rootNode;
                    ReportProgress(this.progress, this.progressStruct);

                }
                catch (Exception ex)
                {
                    this.progressStruct.Clear();
                    this.progressStruct.ErrorMessage = String.Format("Error processing <{0}>.  Error received: ", pPath) + ex.Message;
                    ReportProgress(this.Progress, this.progressStruct);
                }
            }
        }
    }
}
