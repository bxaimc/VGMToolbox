using System;
using System.Drawing;
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

            // add empty form
            ZZZ_NotYetImplemented zzz_NotYetImplemented = new ZZZ_NotYetImplemented();
            this.splitContainer1.Panel2.Controls.Add(zzz_NotYetImplemented);

            buildMenuNodes();
        }

        private void buildMenuNodes()
        {
            Constants.NodeTagStruct nodeTag = new Constants.NodeTagStruct();
            this.treeviewBoldFont = new Font(this.tvMenuTree.Font, FontStyle.Bold);

            // add empty form
            EmptyForm emptyForm = new EmptyForm();
            this.splitContainer1.Panel2.Controls.Add(emptyForm);

            // set tag for displaying the empty form
            nodeTag.formClass = emptyForm.GetType().Name;

            TreeNode rootNode = new TreeNode("VGMToolbox");
            rootNode.NodeFont = this.treeviewBoldFont;
            rootNode.Tag = nodeTag;

            ////////////
            // Auditing
            ////////////
            TreeNode auditing_RootNode = buildAuditingTreeNode();
            auditing_RootNode.Tag = nodeTag;
            rootNode.Nodes.Add(auditing_RootNode);            

            ///////////
            // Examine
            ///////////
            TreeNode examine_RootNode = buildExamineTreeNode();
            examine_RootNode.Tag = nodeTag;
            rootNode.Nodes.Add(examine_RootNode);

            /////////
            // Tools
            /////////
            TreeNode tools_RootNode = new TreeNode("Misc. Tools");
            tools_RootNode.Tag = nodeTag;
            tools_RootNode.NodeFont = this.treeviewBoldFont;

            // Hoot
            TreeNode hoot_RootNode = buildHootTreeNode();
            hoot_RootNode.Tag = nodeTag;
            tools_RootNode.Nodes.Add(hoot_RootNode);

            // NSF
            TreeNode nsf_RootNode = buildNsfTreeNode();
            nsf_RootNode.Tag = nodeTag;
            tools_RootNode.Nodes.Add(nsf_RootNode);

            // GBS
            TreeNode gbs_RootNode = buildGbsTreeNode();
            gbs_RootNode.Tag = nodeTag;
            tools_RootNode.Nodes.Add(gbs_RootNode);

            // xSF
            TreeNode xsf_RootNode = buildXsfTreeNode();
            xsf_RootNode.Tag = nodeTag;
            tools_RootNode.Nodes.Add(xsf_RootNode);
            
            // EXTRACTION                        
            TreeNode ext_RootNode = buildExtractionTreeNode();
            ext_RootNode.Tag = nodeTag;
            tools_RootNode.Nodes.Add(ext_RootNode);

            // add Tools node to Root
            rootNode.Nodes.Add(tools_RootNode);

            // add Root node to tree
            tvMenuTree.Nodes.Add(rootNode);
            // tvMenuTree.ExpandAll();
            rootNode.Expand();
            

            tvMenuTree.NodeMouseClick += tvMenuTree_doClick;
        }

        private TreeNode buildAuditingTreeNode()
        {
            Constants.NodeTagStruct nodeTag = new Constants.NodeTagStruct();
            TreeNode auditing_RootNode = new TreeNode("Auditing Tools");
            
            ////////////////////
            // Datafile Creator
            ////////////////////
            TreeNode auditing_DatafileCreatorNode = new TreeNode("Datafile Creator");

            // add form
            Auditing_DatafileCreatorForm auditing_DatafileCreatorForm = 
                new Auditing_DatafileCreatorForm(auditing_DatafileCreatorNode);
            this.splitContainer1.Panel2.Controls.Add(auditing_DatafileCreatorForm);

            // set tag for displaying the form
            nodeTag.formClass = auditing_DatafileCreatorForm.GetType().Name;
            auditing_DatafileCreatorNode.Tag = nodeTag;

            /////////////
            // Rebuilder
            /////////////
            TreeNode auditing_RebuilderNode = new TreeNode("Rebuilder");

            // add form
            Auditing_RebuilderForm auditing_RebuilderForm =
                new Auditing_RebuilderForm(auditing_RebuilderNode);
            this.splitContainer1.Panel2.Controls.Add(auditing_RebuilderForm);

            // set tag for displaying the form
            nodeTag.formClass = auditing_RebuilderForm.GetType().Name;
            auditing_RebuilderNode.Tag = nodeTag;

            ////////////////////
            // Datafile Checker
            ////////////////////
            TreeNode auditing_DatafileCheckerNode = new TreeNode("Datafile Checker");

            // add form
            Auditing_DatafileCheckerForm auditing_DatafileCheckerForm =
                new Auditing_DatafileCheckerForm(auditing_DatafileCheckerNode);
            this.splitContainer1.Panel2.Controls.Add(auditing_DatafileCheckerForm);

            // set tag for displaying the form
            nodeTag.formClass = auditing_DatafileCheckerForm.GetType().Name;
            auditing_DatafileCheckerNode.Tag = nodeTag;            
            
            auditing_RootNode.NodeFont = this.treeviewBoldFont;

            auditing_RootNode.Nodes.Add(auditing_DatafileCreatorNode);
            auditing_RootNode.Nodes.Add(auditing_RebuilderNode);
            auditing_RootNode.Nodes.Add(auditing_DatafileCheckerNode);

            return auditing_RootNode;
        }

        private TreeNode buildExamineTreeNode()
        {
            Constants.NodeTagStruct nodeTag = new Constants.NodeTagStruct();
            TreeNode examine_RootNode = new TreeNode("Examine/Exploration Tools");

            // Tag Viewer
            TreeNode examine_TagViewerNode = new TreeNode("Tag/Info Viewer");
            Examine_TagViewerForm examine_TagViewerForm = 
                new Examine_TagViewerForm(examine_TagViewerNode);
            this.splitContainer1.Panel2.Controls.Add(examine_TagViewerForm);

            nodeTag.formClass = examine_TagViewerForm.GetType().Name;
            examine_TagViewerNode.Tag = nodeTag;

            examine_RootNode.NodeFont = this.treeviewBoldFont;

            examine_RootNode.Nodes.Add(examine_TagViewerNode);

            return examine_RootNode;
        }

        private TreeNode buildHootTreeNode()
        {
            Constants.NodeTagStruct nodeTag = new Constants.NodeTagStruct();
            TreeNode hoot_RootNode = new TreeNode("Hoot Tools");
            
            TreeNode hoot_CsvDatafileNode = new TreeNode("CSV to Datafile");
            
            ///////////////
            // XML Builder
            ///////////////
            TreeNode hoot_XmlBuilderNode = new TreeNode("XML Builder");

            // add form
            Hoot_XmlBuilderForm hoot_XmlBuilderForm = new Hoot_XmlBuilderForm(hoot_XmlBuilderNode);
            this.splitContainer1.Panel2.Controls.Add(hoot_XmlBuilderForm);

            // set tag for displaying the form
            nodeTag.formClass = hoot_XmlBuilderForm.GetType().Name;
            hoot_XmlBuilderNode.Tag = nodeTag;
            
            hoot_RootNode.NodeFont = this.treeviewBoldFont;

            hoot_RootNode.Nodes.Add(hoot_CsvDatafileNode);
            hoot_RootNode.Nodes.Add(hoot_XmlBuilderNode);

            return hoot_RootNode;
        }

        private TreeNode buildNsfTreeNode()
        {
            Constants.NodeTagStruct nodeTag = new Constants.NodeTagStruct();
            
            TreeNode nsf_RootNode = new TreeNode("NSF Tools");
            TreeNode nsf_NsfeM3uNode = new TreeNode("NSFE to NSF + M3U");
            nsf_RootNode.NodeFont = this.treeviewBoldFont;

            // add form
            Nsf_Nsfe2NsfM3uForm nsf_Nsfe2NsfM3uForm = new Nsf_Nsfe2NsfM3uForm(nsf_NsfeM3uNode);
            this.splitContainer1.Panel2.Controls.Add(nsf_Nsfe2NsfM3uForm);

            // set tag for displaying the form
            nodeTag.formClass = nsf_Nsfe2NsfM3uForm.GetType().Name;
            nsf_NsfeM3uNode.Tag = nodeTag;

            nsf_RootNode.Nodes.Add(nsf_NsfeM3uNode);

            return nsf_RootNode;        
        }

        private TreeNode buildGbsTreeNode()
        {
            Constants.NodeTagStruct nodeTag = new Constants.NodeTagStruct();
            
            TreeNode gbs_RootNode = new TreeNode("GBS Tools");
            TreeNode gbs_M3uNode = new TreeNode("GBS M3U Creator");
            gbs_RootNode.NodeFont = this.treeviewBoldFont;

            // add form
            Gbs_GbsToM3uForm gbs_GbsToM3uForm = new Gbs_GbsToM3uForm(gbs_M3uNode);
            this.splitContainer1.Panel2.Controls.Add(gbs_GbsToM3uForm);

            // set tag for displaying the form
            nodeTag.formClass = gbs_GbsToM3uForm.GetType().Name;
            gbs_M3uNode.Tag = nodeTag;

            gbs_RootNode.Nodes.Add(gbs_M3uNode);

            return gbs_RootNode;
        }

        private TreeNode buildXsfTreeNode()
        {
            Constants.NodeTagStruct nodeTag = new Constants.NodeTagStruct();
            EmptyForm emptyForm = new EmptyForm();
            TreeNode xsf_RootNode = new TreeNode("xSF Tools");
            TreeNode _2sf_RootNode = new TreeNode("2SF");
            TreeNode psf_RootNode = new TreeNode("PSF");
            TreeNode psf2_RootNode = new TreeNode("PSF2");


            //////////////
            // 2SF Ripper
            //////////////
            TreeNode xsf_2sfRipperNode = new TreeNode("2SF Ripper (Old Format)");
            
            // Add 2SF Ripper Form
            Xsf_2sfRipperForm xsf_2sfRipperForm = new Xsf_2sfRipperForm(xsf_2sfRipperNode);
            this.splitContainer1.Panel2.Controls.Add(xsf_2sfRipperForm);

            // Set Tag for displaying the Form
            nodeTag.formClass = xsf_2sfRipperForm.GetType().Name;
            xsf_2sfRipperNode.Tag = nodeTag;

            //////////////
            // 2SF Timer
            //////////////
            TreeNode xsf_2sfTimerNode = new TreeNode("2SF Timer");

            Xsf_2sfTimerForm xsf_2sfTimerForm = new Xsf_2sfTimerForm(xsf_2sfTimerNode);
            this.splitContainer1.Panel2.Controls.Add(xsf_2sfTimerForm);

            nodeTag.formClass = xsf_2sfTimerForm.GetType().Name;
            xsf_2sfTimerNode.Tag = nodeTag;

            ////////////
            // MKPSF2FE
            ////////////
            TreeNode xsf_MkPsf2FENode = new TreeNode("mkpsf2 Front End");

            // Add UnpkPsf2 Ripper Form
            Xsf_Mkpsf2FrontEndForm xsf_Mkpsf2FrontEndForm =
                new Xsf_Mkpsf2FrontEndForm(xsf_MkPsf2FENode);
            this.splitContainer1.Panel2.Controls.Add(xsf_Mkpsf2FrontEndForm);

            // Set Tag for displaying the Form
            nodeTag.formClass = xsf_Mkpsf2FrontEndForm.GetType().Name;
            xsf_MkPsf2FENode.Tag = nodeTag;
            
            //////////////
            // UNPKPSF2FE
            //////////////
            TreeNode xsf_UnPsf2FENode = new TreeNode("unpksf2 Front End");

            // Add UnpkPsf2 Ripper Form
            Xsf_Unpkpsf2FrontEndForm xsf_Unpkpsf2FrontEndForm =
                new Xsf_Unpkpsf2FrontEndForm(xsf_UnPsf2FENode);
            this.splitContainer1.Panel2.Controls.Add(xsf_Unpkpsf2FrontEndForm);

            // Set Tag for displaying the Form
            nodeTag.formClass = xsf_Unpkpsf2FrontEndForm.GetType().Name;
            xsf_UnPsf2FENode.Tag = nodeTag;
            
            /////////////////
            // PSF2TOPSF2LIB
            /////////////////
            TreeNode xsf_Psf2ToPsf2LibNode = new TreeNode("PSF2 to PSF2LIB");

            // Add UnpkPsf2 Ripper Form
            Xsf_Psf2ToPsf2LibForm xsf_Psf2ToPsf2LibForm =
                new Xsf_Psf2ToPsf2LibForm(xsf_Psf2ToPsf2LibNode);
            this.splitContainer1.Panel2.Controls.Add(xsf_Psf2ToPsf2LibForm);

            // Set Tag for displaying the Form
            nodeTag.formClass = xsf_Psf2ToPsf2LibForm.GetType().Name;
            xsf_Psf2ToPsf2LibNode.Tag = nodeTag;

            /////////////////
            // PSF2SQEXTRACTOR
            /////////////////
            TreeNode xsf_Psf2SqExtractorNode = new TreeNode("PSF2 SQ Extractor");

            // Add UnpkPsf2 Ripper Form
            Xsf_Psf2SqExtractorForm xsf_Psf2SqExtractorForm =
                new Xsf_Psf2SqExtractorForm(xsf_Psf2SqExtractorNode);
            this.splitContainer1.Panel2.Controls.Add(xsf_Psf2SqExtractorForm);

            // Set Tag for displaying the Form
            nodeTag.formClass = xsf_Psf2SqExtractorForm.GetType().Name;
            xsf_Psf2SqExtractorNode.Tag = nodeTag;

            //////////////
            // BIN2PSF FE
            //////////////
            TreeNode xsf_Bin2PsfFENode = new TreeNode("bin2psf Front End");

            // Add UnpkPsf2 Ripper Form
            Xsf_Bin2PsfFrontEndForm xsf_Bin2PsfFrontEndForm =
                new Xsf_Bin2PsfFrontEndForm(xsf_Bin2PsfFENode);
            this.splitContainer1.Panel2.Controls.Add(xsf_Bin2PsfFrontEndForm);

            // Set Tag for displaying the Form
            nodeTag.formClass = xsf_Bin2PsfFrontEndForm.GetType().Name;
            xsf_Bin2PsfFENode.Tag = nodeTag;
                        
            // SSFMAKE
            /*
            TreeNode xsf_SsfMakeFENode = new TreeNode("ssfmake Front End");
            
            Xsf_SsfMakeFrontEndForm xsf_SsfMakeFrontEndForm = 
                new Xsf_SsfMakeFrontEndForm(xsf_SsfMakeFENode);
            this.splitContainer1.Panel2.Controls.Add(xsf_SsfMakeFrontEndForm);

            nodeTag.formClass = xsf_SsfMakeFrontEndForm.GetType().Name;
            xsf_SsfMakeFENode.Tag = nodeTag;
            */

            ///////////
            // XSF2EXE
            ///////////
            TreeNode xsf_xsf2ExeNode = new TreeNode("xSF2EXE");

            // Add Xsf2Exe Form
            Xsf_Xsf2ExeForm xsf_Xsf2ExeForm = new Xsf_Xsf2ExeForm(xsf_xsf2ExeNode);
            this.splitContainer1.Panel2.Controls.Add(xsf_Xsf2ExeForm);

            // Set Tag for displaying the Form
            nodeTag.formClass = xsf_Xsf2ExeForm.GetType().Name;
            xsf_xsf2ExeNode.Tag = nodeTag;

            nodeTag.formClass = emptyForm.GetType().Name;
            xsf_RootNode.NodeFont = this.treeviewBoldFont;

            xsf_RootNode.Nodes.Add(xsf_xsf2ExeNode);

            _2sf_RootNode.NodeFont = this.treeviewBoldFont;
            _2sf_RootNode.Tag = nodeTag;
            _2sf_RootNode.Nodes.Add(xsf_2sfRipperNode);
            _2sf_RootNode.Nodes.Add(xsf_2sfTimerNode);
            xsf_RootNode.Nodes.Add(_2sf_RootNode);

            psf_RootNode.NodeFont = this.treeviewBoldFont;
            psf_RootNode.Tag = nodeTag;
            psf_RootNode.Nodes.Add(xsf_Bin2PsfFENode);                        
            xsf_RootNode.Nodes.Add(psf_RootNode);

            psf2_RootNode.NodeFont = this.treeviewBoldFont;
            psf2_RootNode.Tag = nodeTag;
            psf2_RootNode.Nodes.Add(xsf_MkPsf2FENode);
            psf2_RootNode.Nodes.Add(xsf_UnPsf2FENode);
            psf2_RootNode.Nodes.Add(xsf_Psf2ToPsf2LibNode);
            psf2_RootNode.Nodes.Add(xsf_Psf2SqExtractorNode);            
            xsf_RootNode.Nodes.Add(psf2_RootNode);            

            // xsf_RootNode.Nodes.Add(xsf_SsfMakeFENode);


            return xsf_RootNode;
        }
        
        private TreeNode buildExtractionTreeNode()
        {
            Constants.NodeTagStruct nodeTag = new Constants.NodeTagStruct();
            TreeNode ext_RootNode = new TreeNode("Extraction Tools");
            ext_RootNode.NodeFont = this.treeviewBoldFont;

            ///////
            // NDS
            ///////
            TreeNode ext_NdsNode = new TreeNode("Nintendo DS");
            ext_NdsNode.NodeFont = this.treeviewBoldFont;

            nodeTag = new Constants.NodeTagStruct();
            EmptyForm emptyForm = new EmptyForm();
            nodeTag.formClass = emptyForm.GetType().Name;
            ext_NdsNode.Tag = nodeTag;

            // SDAT Extractor
            TreeNode ext_SdatExtractorNode = new TreeNode("SDAT Extractor");            

            // Add SDAT Extractor Form
            Xsf_SdatExtractorForm xsf_SdatExtractorForm = new Xsf_SdatExtractorForm(ext_SdatExtractorNode);
            this.splitContainer1.Panel2.Controls.Add(xsf_SdatExtractorForm);

            // Set Tag for displaying the Form
            nodeTag.formClass = xsf_SdatExtractorForm.GetType().Name;
            ext_SdatExtractorNode.Tag = nodeTag;

            // SDAT Finder
            TreeNode ext_SdatFinderNode = new TreeNode("SDAT Finder");

            // Add SDAT Extractor Form
            Xsf_SdatFinderForm xsf_SdatFinderForm = new Xsf_SdatFinderForm(ext_SdatFinderNode);
            this.splitContainer1.Panel2.Controls.Add(xsf_SdatFinderForm);

            // Set Tag for displaying the Form
            nodeTag.formClass = xsf_SdatFinderForm.GetType().Name;
            ext_SdatFinderNode.Tag = nodeTag;

            ///////
            // PC
            ///////
            TreeNode ext_PcNode = new TreeNode("PC");
            ext_PcNode.NodeFont = this.treeviewBoldFont;

            nodeTag = new Constants.NodeTagStruct();
            emptyForm = new EmptyForm();
            nodeTag.formClass = emptyForm.GetType().Name;
            ext_PcNode.Tag = nodeTag;


            ext_NdsNode.Nodes.Add(ext_SdatExtractorNode);
            ext_NdsNode.Nodes.Add(ext_SdatFinderNode);            
            ext_RootNode.Nodes.Add(ext_NdsNode);
            ext_RootNode.Nodes.Add(ext_PcNode);

            return ext_RootNode;
        }

        private void tvMenuTree_doClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag != null)
            {
                Constants.NodeTagStruct nts = (Constants.NodeTagStruct)e.Node.Tag;

                // need to fix this so it only changes if the form is not "running"
                //VgmtForm.ResetNodeColor(e.Node); 

                showForm(this.splitContainer1.Panel2.Controls, nts.formClass);
            }
            else
            {
                showForm(this.splitContainer1.Panel2.Controls, "ZZZ_NotYetImplemented");
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

        private void tvMenuTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.IsSelected && e.Node.Tag != null)
            {
                Constants.NodeTagStruct nts = (Constants.NodeTagStruct)e.Node.Tag;

                // need to fix this so it only changes if the form is not "running"
                //VgmtForm.ResetNodeColor(e.Node); 

                showForm(this.splitContainer1.Panel2.Controls, nts.formClass);
            }
            else
            {
                showForm(this.splitContainer1.Panel2.Controls, "ZZZ_NotYetImplemented");
            }           
        }
    }
}
