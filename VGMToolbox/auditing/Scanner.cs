using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Windows.Forms;

using VGMToolbox.format.auditing;

namespace VGMToolbox.auditing
{
    class Scanner
    {
        datafile dataFile;
        ArrayList datafileSetList = new ArrayList();
        string[] datafileSets;
        ArrayList unknownFolderList = new ArrayList();


        private void buildDatafileSetList()
        {
            foreach (game g in dataFile.game)
            {
                datafileSetList.Add(g.name);
            }

            datafileSetList.Sort();
            datafileSets = (string[])datafileSetList.ToArray(typeof(string));
        }

        private void buildUnknownFolderList(string pPath)
        {
            if (datafileSetList.Count == 0)
            {
                this.buildDatafileSetList();
            }
            
            foreach (string d in Directory.GetDirectories(pPath))
            {
                if (!datafileSetList.Contains(d))
                {
                    unknownFolderList.Add(d);
                }
            }
        }

        private void loadDatafile(string pDatafilePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(datafile));
            TextReader textReader = new StreamReader(pDatafilePath);
            dataFile = (datafile)serializer.Deserialize(textReader);
        }

        private void initialize(string pPath)
        {
            this.loadDatafile(pPath);
            this.buildDatafileSetList();
            this.buildUnknownFolderList(pPath);        
        }

        public void ScanAllDirs(string pPath, TreeView pTreeView, ToolStripProgressBar pToolStripProgressBar)
        {
            this.initialize(pPath);
            this.ScanAllDirs(pPath, 0, pTreeView, pToolStripProgressBar);
        }

        public void ScanAllDirs(string pPath, int pDepth, TreeView pTreeView, ToolStripProgressBar pToolStripProgressBar)
        {
            foreach (game g in dataFile.game)
            {
            
            
            }
        }

        private string buildFilePath(string pSetName, string pFilePath)
        {
            return (pSetName + Path.DirectorySeparatorChar + pFilePath);
        }
    }
}
