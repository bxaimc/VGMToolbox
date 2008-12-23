using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.forms;
using VGMToolbox.util;

namespace VGMToolbox
{
    public partial class Form2 : Form
    {
        Font treeviewBoldFont;
        
        public Form2()
        {
            InitializeComponent();

            buildMenuNodes();
        }

        private void buildMenuNodes()
        {
            this.treeviewBoldFont = new Font(this.tvMenuTree.Font, FontStyle.Bold);
            
            TreeNode rootNode = new TreeNode("VGMToolbox");
            rootNode.NodeFont = this.treeviewBoldFont;

            Constants.NodeTagStruct nodeTag = new Constants.NodeTagStruct();

            ////////////
            // Auditing
            ////////////
            TreeNode audting_RootNode = new TreeNode("Auditing Tools");
            TreeNode audting_DatafileCreatorNode = new TreeNode("Datafile Creator");
            TreeNode audting_RebuilderNode = new TreeNode("Rebuilder");
            TreeNode audting_DatafileCheckerNode = new TreeNode("Datafile Checker");
            audting_RootNode.NodeFont = this.treeviewBoldFont;

            audting_RootNode.Nodes.Add(audting_DatafileCreatorNode);
            audting_RootNode.Nodes.Add(audting_RebuilderNode);
            audting_RootNode.Nodes.Add(audting_DatafileCheckerNode);

            rootNode.Nodes.Add(audting_RootNode);

            ///////////
            // Examine
            ///////////
            TreeNode examine_RootNode = new TreeNode("Examine/Exploration Tools");
            
            // Tag Viewer
            TreeNode examine_TagViewerNode = new TreeNode("Tag/Info Viewer");
            Examine_TagViewerForm examine_TagViewerForm = new Examine_TagViewerForm();
            this.splitContainer1.Panel2.Controls.Add(examine_TagViewerForm);

            nodeTag.formClass = examine_TagViewerForm.GetType().Name;
            examine_TagViewerNode.Tag = nodeTag;
            
            
            
            // MDX Checker
            TreeNode examine_MDXCheckerNode = new TreeNode("MDX Checker");
            
            
            
            
            
            examine_RootNode.NodeFont = this.treeviewBoldFont;

            examine_RootNode.Nodes.Add(examine_TagViewerNode);
            examine_RootNode.Nodes.Add(examine_MDXCheckerNode);

            rootNode.Nodes.Add(examine_RootNode);

            /////////
            // Tools
            /////////
            TreeNode tools_RootNode = new TreeNode("Misc. Tools");
            tools_RootNode.NodeFont = this.treeviewBoldFont;

            // Hoot
            TreeNode hoot_RootNode = new TreeNode("Hoot Tools");
            TreeNode hoot_CsvDatafileNode = new TreeNode("CSV to Datafile");
            TreeNode hoot_XmlBuilderNode = new TreeNode("Xml Builder");
            hoot_RootNode.NodeFont = this.treeviewBoldFont;

            hoot_RootNode.Nodes.Add(hoot_CsvDatafileNode);
            hoot_RootNode.Nodes.Add(hoot_XmlBuilderNode);
            
            tools_RootNode.Nodes.Add(hoot_RootNode);

            // NSF
            TreeNode nsf_RootNode = new TreeNode("NSF Tools");
            TreeNode nsf_NsfeM3uNode = new TreeNode("NSFE to NSF + M3U");
            nsf_RootNode.NodeFont = this.treeviewBoldFont;

            nsf_RootNode.Nodes.Add(nsf_NsfeM3uNode);
            
            tools_RootNode.Nodes.Add(nsf_RootNode);

            // GBS
            TreeNode gbs_RootNode = new TreeNode("GBS Tools");
            TreeNode gbs_M3uNode = new TreeNode("GBS M3U Creator");
            gbs_RootNode.NodeFont = this.treeviewBoldFont;

            gbs_RootNode.Nodes.Add(gbs_M3uNode);
            
            tools_RootNode.Nodes.Add(gbs_RootNode);

            // xSF
            TreeNode xsf_RootNode = new TreeNode("xSF Tools");
            TreeNode xsf_xsf2ExeNode = new TreeNode("xSF2EXE");
            
            TreeNode xsf_2sfRipperNode = new TreeNode("2SF Ripper");
            nodeTag.formClass = "pnlXsf_2sfRipper";
            xsf_2sfRipperNode.Tag = nodeTag;

            TreeNode xsf_2sfTimerNode = new TreeNode("2SF Timer");
            TreeNode xsf_MkPsf2FENode = new TreeNode("mkpsf2 Front End");
            TreeNode xsf_UnPsf2FENode = new TreeNode("unpsf2 Front End");
            xsf_RootNode.NodeFont = this.treeviewBoldFont;


            xsf_RootNode.Nodes.Add(xsf_xsf2ExeNode);
            xsf_RootNode.Nodes.Add(xsf_2sfRipperNode);
            xsf_RootNode.Nodes.Add(xsf_2sfTimerNode);
            xsf_RootNode.Nodes.Add(xsf_MkPsf2FENode);
            xsf_RootNode.Nodes.Add(xsf_UnPsf2FENode);

            tools_RootNode.Nodes.Add(xsf_RootNode);

            /*****************************
             * NDS
             ******************************/            
            TreeNode nds_RootNode = new TreeNode("NDS Tools");

            // SDAT Extractor
            TreeNode nds_SdatExtractorNode = new TreeNode("SDAT Extractor");
            nds_RootNode.NodeFont = this.treeviewBoldFont;

            Xsf_SdatExtractorForm xsf_SdatExtractorForm = new Xsf_SdatExtractorForm();
            this.splitContainer1.Panel2.Controls.Add(xsf_SdatExtractorForm);

            nodeTag.formClass = xsf_SdatExtractorForm.GetType().Name;
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
                
                showForm(this.splitContainer1.Panel2.Controls, nts.formClass);
            }
        }

        private void showForm(Control.ControlCollection pControls, string pFormClass)
        {
            foreach (Control ctrl in pControls)
            {
                if ((ctrl is Form)&& (!String.IsNullOrEmpty(ctrl.Name)))
                {
                    if ((!String.IsNullOrEmpty(pFormClass)) && (pFormClass.Equals(ctrl.Name)))
                    {
                        ctrl.Show();
                        ctrl.Visible = true;
                        ctrl.BringToFront();
                    }
                    else
                    {
                        ctrl.Visible = false;
                    }
                }

                // showForm(ctrl.Controls, pFormClass);
            }        
        }
    }
}
