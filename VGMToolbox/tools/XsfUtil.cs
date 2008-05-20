using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;

using VGMToolbox.format;
using VGMToolbox.util;

namespace VGMToolbox.tools
{
    class XsfUtil
    {
        public static string getType(string pFileName)
        {
            FileStream fs = new FileStream(pFileName, FileMode.Open);
            Type t = FormatUtil.getObjectType(fs);
            return t.ToString();
        }

        public void buildTreeNode(string pFileName, TreeNode pParentNode)
        {
            FileStream fs = File.OpenRead(pFileName);
            Type formatType = FormatUtil.getObjectType(fs);
            
            System.Text.Encoding enc = System.Text.Encoding.UTF8;

            if (formatType != null && formatType.ToString() == "VGMToolbox.format.Xsf")
            {
                fs.Seek(0, SeekOrigin.Begin);
                Xsf vgmData = new Xsf();
                vgmData.initialize(fs);

                // Populate Tree View
                if (pParentNode != null)
                {                   
                    // create file node
                    TreeNode fileNode = new TreeNode(FileUtil.trimPath(pFileName));
                    
                    if (!fileNode.Text.Equals(pParentNode.Text))
                    {                        
                        foreach (string s in vgmData.TagHash.Keys)
                        {
                            TreeNode tagNode = new TreeNode(s + ": " + vgmData.TagHash[s]);
                            fileNode.Nodes.Add(tagNode);                            
                        }
                        pParentNode.Nodes.Add(fileNode);
                    }
                    else
                    {
                        foreach (string s in vgmData.TagHash.Keys)
                        {
                            TreeNode tagNode = new TreeNode(s + ": " + vgmData.TagHash[s]);
                            pParentNode.Nodes.Add(tagNode);
                        }
                    }
                }
            }

            fs.Close();
            fs.Dispose();
        }

        public void buildTreeNode(string[] pPaths, ref TreeNode[] pParentNodes, ref string pOutputMessages, ToolStripProgressBar pToolStripProgressBar)
        {
            ArrayList treeNodes = new ArrayList();
            TreeNode parentNode;         

            for (int i = 0; i < pPaths.Length; i++)
            {
                pToolStripProgressBar.PerformStep();
                
                try
                {
                    if (File.Exists(pPaths[i]))
                    {
                        parentNode = new TreeNode(FileUtil.trimPath(pPaths[i]));
                        buildTreeNode(pPaths[i], parentNode);
                        treeNodes.Add(parentNode);

                    }
                    else if (Directory.Exists(pPaths[i]))
                    {
                        string nodeName = pPaths[i].Substring(pPaths[i].LastIndexOf(Path.DirectorySeparatorChar) + 1);
                        parentNode = new TreeNode(nodeName);

                        foreach (string f in Directory.GetFiles(pPaths[i]))
                        {
                            buildTreeNode(f, parentNode);
                        }
                        treeNodes.Add(parentNode);
                    }
                    pParentNodes = (TreeNode[])treeNodes.ToArray(typeof(TreeNode));
                }
                catch (Exception)
                { 
                    pOutputMessages += "Error processing <" + pPaths[i] + ">, skipped." + Environment.NewLine;                
                }
            }       
        }
    }
}
