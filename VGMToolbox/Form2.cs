using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.util;

namespace VGMToolbox
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();

            buildMenuNodes();
        }

        private void buildMenuNodes()
        {
            TreeNode rootNode = new TreeNode("VGMToolbox");
            Constants.NodeTagStruct nodeTag = new Constants.NodeTagStruct();

            ////////////
            // Auditing
            ////////////
            TreeNode audting_RootNode = new TreeNode("Auditing Tools");
            TreeNode audting_DatafileCreatorNode = new TreeNode("Datafile Creator");
            TreeNode audting_RebuilderNode = new TreeNode("Rebuilder");
            TreeNode audting_DatafileCheckerNode = new TreeNode("Datafile Checker");
            
            audting_RootNode.Nodes.Add(audting_DatafileCreatorNode);
            audting_RootNode.Nodes.Add(audting_RebuilderNode);
            audting_RootNode.Nodes.Add(audting_DatafileCheckerNode);

            rootNode.Nodes.Add(audting_RootNode);

            ///////////
            // Examine
            ///////////
            TreeNode examine_RootNode = new TreeNode("Examine/Exploration Tools");
            TreeNode examine_TagViewerNode = new TreeNode("Tag/Info Viewer");
            TreeNode examine_MDXCheckerNode = new TreeNode("MDX Checker");

            examine_RootNode.Nodes.Add(examine_TagViewerNode);
            examine_RootNode.Nodes.Add(examine_MDXCheckerNode);

            rootNode.Nodes.Add(examine_RootNode);

            /////////
            // Tools
            /////////
            TreeNode tools_RootNode = new TreeNode("Misc. Tools");

            // Hoot
            TreeNode hoot_RootNode = new TreeNode("Hoot Tools");
            TreeNode hoot_CsvDatafileNode = new TreeNode("CSV to Datafile");
            TreeNode hoot_XmlBuilderNode = new TreeNode("Xml Builder");

            hoot_RootNode.Nodes.Add(hoot_CsvDatafileNode);
            hoot_RootNode.Nodes.Add(hoot_XmlBuilderNode);
            
            tools_RootNode.Nodes.Add(hoot_RootNode);

            // NSF
            TreeNode nsf_RootNode = new TreeNode("NSF Tools");
            TreeNode nsf_NsfeM3uNode = new TreeNode("NSFE to NSF + M3U");

            nsf_RootNode.Nodes.Add(nsf_NsfeM3uNode);
            
            tools_RootNode.Nodes.Add(nsf_RootNode);

            // GBS
            TreeNode gbs_RootNode = new TreeNode("GBS Tools");
            TreeNode gbs_M3uNode = new TreeNode("GBS M3U Creator");

            gbs_RootNode.Nodes.Add(gbs_M3uNode);
            
            tools_RootNode.Nodes.Add(gbs_RootNode);

            // xSF
            TreeNode xsf_RootNode = new TreeNode("xSF Tools");
            TreeNode xsf_xsf2ExeNode = new TreeNode("xSF2EXE");
            
            TreeNode xsf_2sfRipperNode = new TreeNode("2SF Ripper");
            nodeTag.panel = "pnlXsf_2sfRipper";
            xsf_2sfRipperNode.Tag = nodeTag;

            TreeNode xsf_2sfTimerNode = new TreeNode("2SF Timer");
            TreeNode xsf_MkPsf2FENode = new TreeNode("mkpsf2 Front End");
            TreeNode xsf_UnPsf2FENode = new TreeNode("unpsf2 Front End");

            xsf_RootNode.Nodes.Add(xsf_xsf2ExeNode);
            xsf_RootNode.Nodes.Add(xsf_2sfRipperNode);
            xsf_RootNode.Nodes.Add(xsf_2sfTimerNode);
            xsf_RootNode.Nodes.Add(xsf_MkPsf2FENode);
            xsf_RootNode.Nodes.Add(xsf_UnPsf2FENode);

            tools_RootNode.Nodes.Add(xsf_RootNode);

            // NDS
            TreeNode nds_RootNode = new TreeNode("NDS Tools");
            TreeNode nds_SdatExtractorNode = new TreeNode("SDAT Extractor");

            nodeTag.panel = "pnlNds_SdatExtractor";
            nds_SdatExtractorNode.Tag = nodeTag;

            nds_RootNode.Nodes.Add(nds_SdatExtractorNode);

            tools_RootNode.Nodes.Add(nds_RootNode);

            // add Tools node to Root
            rootNode.Nodes.Add(tools_RootNode);

            tvMenuTree.Nodes.Add(rootNode);
            tvMenuTree.ExpandAll();

            tvMenuTree.NodeMouseClick += tvMenuTree_doClick;
        }

        private void tvMenuTree_doClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag != null)
            {
                Constants.NodeTagStruct nts = (Constants.NodeTagStruct)e.Node.Tag;
                
                // e.Node.ForeColor = Color.Red; // use this when a process is running?
                
                showPanel(Controls, nts.panel);
            }
        }

        private void showPanel(Control.ControlCollection pControls, string pPanelName)
        {
            foreach (Control ctrl in pControls)
            {
                if ((ctrl is Panel) && (!String.IsNullOrEmpty(ctrl.Name)))
                {
                    if ((!String.IsNullOrEmpty(pPanelName)) && (pPanelName.Equals(ctrl.Name)))
                    {
                        ctrl.Visible = true;
                        ctrl.BringToFront();
                    }
                    else
                    {
                        ctrl.Visible = false;
                    }
                }

                showPanel(ctrl.Controls, pPanelName);
            }        
        }
    }
}
