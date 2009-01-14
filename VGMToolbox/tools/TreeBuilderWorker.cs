using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using VGMToolbox.format;
using VGMToolbox.util;

namespace VGMToolbox.tools
{
    class TreeBuilderWorker : BackgroundWorker
    {
        private int fileCount = 0;
        private int maxFiles = 0;

        public struct TreeBuilderStruct
        {
            public string[] pPaths;
            public int totalFiles;
            public bool checkForLibs;
        }

        public TreeBuilderWorker()
        {
            fileCount = 0;
            maxFiles = 0;
            
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
        }

        private void buildTree(TreeBuilderStruct pTreeBuilderStruct, DoWorkEventArgs e)
        {
            Constants.ProgressStruct vProgressStruct;
            string examineOutputPath = "." + Path.DirectorySeparatorChar + "examine" + 
                Path.DirectorySeparatorChar + "examine.txt";
            StreamWriter sw = null;
            sw = File.CreateText(examineOutputPath);

            foreach (string path in pTreeBuilderStruct.pPaths)
            {
                TreeNode t = null;

                if (File.Exists(path))
                {
                    if (!CancellationPending)
                    {
                        t = this.getFileNode(path, sw, pTreeBuilderStruct.checkForLibs, e);
                    }
                    else 
                    {
                        e.Cancel = true;

                        if (sw != null)
                        {
                            sw.Close();
                            sw.Dispose();
                        }
                        return;
                    }
                }
                else if (Directory.Exists(path))
                {
                    t = new TreeNode(Path.GetFileName(path));
                    this.getDirectoryNode(path, t, sw, pTreeBuilderStruct.checkForLibs, e);

                    if (CancellationPending)
                    {
                        e.Cancel = true;

                        if (sw != null)
                        {
                            sw.Close();
                            sw.Dispose();
                        }
                        
                        return;                        
                    }
                }
                
                // Add node, but ignore progress
                vProgressStruct = new Constants.ProgressStruct();
                vProgressStruct.newNode = (TreeNode) t.Clone();
                ReportProgress(Constants.IGNORE_PROGRESS, vProgressStruct);                
            }

            sw.Close();
            sw.Dispose();
        }

        private void getDirectoryNode(string pDirectory, TreeNode pTreeNode, StreamWriter pOutputFileStream, 
            bool pCheckForLibs, DoWorkEventArgs e)
        {
            foreach (string d in Directory.GetDirectories(pDirectory))
            {
                if (!CancellationPending)
                {
                    TreeNode t = new TreeNode(Path.GetFileName(d));
                    this.getDirectoryNode(d, t, pOutputFileStream, pCheckForLibs, e);
                    pTreeNode.Nodes.Add(t);

                    if (t.ForeColor == Color.Red)
                    {
                        pTreeNode.ForeColor = Color.Red;
                    }
                }
                else
                {
                    e.Cancel = true;
                    break;
                }
            }                        
            foreach (string f in Directory.GetFiles(pDirectory))
            {
                if (!CancellationPending)
                {
                    TreeNode t = this.getFileNode(f, pOutputFileStream, pCheckForLibs, e);
                    pTreeNode.Nodes.Add(t);

                    if (t.ForeColor == Color.Red)
                    {
                        pTreeNode.ForeColor = Color.Red;
                    }
                }
                else
                {
                    e.Cancel = true;
                    break;
                }
            }
        }

        private TreeNode getFileNode(string pFileName, StreamWriter pOutputFileStream, 
            bool pCheckForLibs, DoWorkEventArgs e)
        {
            int progress = (++fileCount * 100) / maxFiles;
            Constants.ProgressStruct vProgressStruct = new Constants.ProgressStruct();
            vProgressStruct.newNode = null;
            vProgressStruct.filename = pFileName;
            ReportProgress(progress, vProgressStruct);

            TreeNode ret = new TreeNode(Path.GetFileName(pFileName));            
            Constants.NodeTagStruct nodeTag = new Constants.NodeTagStruct();

            using (FileStream fs = File.OpenRead(pFileName))
            {
                try
                {
                    Type dataType = FormatUtil.getObjectType(fs);

                    if (dataType != null)
                    {
                        pOutputFileStream.WriteLine(pFileName);

                        IFormat vgmData = (IFormat)Activator.CreateInstance(dataType);
                        vgmData.Initialize(fs, pFileName);
                        Dictionary<string, string> tagHash = vgmData.GetTagHash();
                        
                        // Add Path for possible future use.
                        tagHash.Add("Path", vgmData.FilePath);

                        // check for libs
                        if (pCheckForLibs && vgmData.UsesLibraries())
                        {
                            if (!vgmData.IsLibraryPresent())
                            {
                                ret.ForeColor = Color.Red;
                                ret.Text += " (Missing Library)";
                            }
                        }

                        char[] trimNull = new char[] { '\0' };

                        foreach (string s in tagHash.Keys)
                        {
                            TreeNode tagNode = new TreeNode(s + ": " + tagHash[s]);
                            ret.Nodes.Add(tagNode);

                            pOutputFileStream.WriteLine(s + ": " + tagHash[s].TrimEnd(trimNull));
                        }

                        pOutputFileStream.WriteLine(Environment.NewLine);

                        // add classname to nodeTag
                        nodeTag.objectType = dataType.AssemblyQualifiedName;
                    }
                }
                catch (Exception ex)
                {
                    vProgressStruct = new Constants.ProgressStruct();
                    vProgressStruct.newNode = null;
                    vProgressStruct.errorMessage = String.Format("Error processing <{0}>.  Error received: ", pFileName) + ex.Message;
                    ReportProgress(progress, vProgressStruct);
                }
            } // using (FileStream fs = File.OpenRead(pFileName))

            nodeTag.filePath = pFileName;
            ret.Tag = nodeTag;

            return ret;
        }
     
        protected override void OnDoWork(DoWorkEventArgs e)
        {
            TreeBuilderStruct treeBuilderStruct = (TreeBuilderStruct)e.Argument;
            maxFiles = treeBuilderStruct.totalFiles;
            this.buildTree(treeBuilderStruct, e);
        }    
    }
}
