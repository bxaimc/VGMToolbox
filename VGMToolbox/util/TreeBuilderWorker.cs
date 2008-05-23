using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.auditing;
using VGMToolbox.format;

namespace VGMToolbox.util
{
    class TreeBuilderWorker : BackgroundWorker
    {
        private int fileCount = 0;
        private int maxFiles = 0;

        public struct TreeBuilderStruct
        {
            public string[] pPaths;
            public int totalFiles;
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
            AuditingUtil.ProgressStruct vProgressStruct;

            foreach (string path in pTreeBuilderStruct.pPaths)
            {
                TreeNode t = null;

                if (File.Exists(path))
                {
                    if (!CancellationPending)
                    {
                        t = this.getFileNode(path, e);
                    }
                    else 
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                else if (Directory.Exists(path))
                {
                    t = new TreeNode(Path.GetFileName(path));
                    this.getDirectoryNode(path, t, e);

                    if (CancellationPending)
                    {
                        e.Cancel = true;
                        return;                        
                    }
                }
                
                // Add node, but ignore progress
                vProgressStruct = new AuditingUtil.ProgressStruct();
                vProgressStruct.newNode = (TreeNode) t.Clone();
                ReportProgress(AuditingUtil.IGNORE_PROGRESS, vProgressStruct);                
            }
        }

        private void getDirectoryNode(string pDirectory, TreeNode pTreeNode, DoWorkEventArgs e)
        {
            foreach (string d in Directory.GetDirectories(pDirectory))
            {
                if (!CancellationPending)
                {
                    TreeNode t = new TreeNode(Path.GetFileName(d));
                    this.getDirectoryNode(d, t, e);
                    pTreeNode.Nodes.Add(t);
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
                    TreeNode t = this.getFileNode(f, e);
                    pTreeNode.Nodes.Add(t);
                    // fileCount++;
                }
                else
                {
                    e.Cancel = true;
                    break;
                }
            }
        }

        private TreeNode getFileNode(string pFileName, DoWorkEventArgs e)
        {
            int progress = (++fileCount * 100) / maxFiles;
            AuditingUtil.ProgressStruct vProgressStruct = new AuditingUtil.ProgressStruct();
            vProgressStruct.newNode = null;
            vProgressStruct.filename = pFileName;
            ReportProgress(progress, vProgressStruct);

            TreeNode ret = new TreeNode(Path.GetFileName(pFileName));
            FileStream fs = File.OpenRead(pFileName);                                    
            try
            {
                Type dataType = FormatUtil.getObjectType(fs);

                if (dataType != null)
                {
                    IFormat vgmData = (IFormat)Activator.CreateInstance(dataType);
                    vgmData.initialize(fs);
                    Dictionary<string, string> tagHash = vgmData.GetTagHash();

                    foreach (string s in tagHash.Keys)
                    {
                        TreeNode tagNode = new TreeNode(s + ": " + tagHash[s]);
                        ret.Nodes.Add(tagNode);
                    }
                }
            }
            catch (Exception ex) 
            { 
                vProgressStruct = new AuditingUtil.ProgressStruct();
                vProgressStruct.newNode = null;
                vProgressStruct.errorMessage = String.Format("Error processing <{0}>.  Error received: ", pFileName) + ex.Message;
                ReportProgress(progress, vProgressStruct);
            }
            
            fs.Close();
            fs.Dispose();

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
