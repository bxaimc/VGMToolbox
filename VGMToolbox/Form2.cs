using System;
using System.Configuration;
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

            this.Text = ConfigurationSettings.AppSettings["VersionString"];

            // add empty form
            ZZZ_NotYetImplemented zzz_NotYetImplemented = new ZZZ_NotYetImplemented();
            this.splitContainer1.Panel2.Controls.Add(zzz_NotYetImplemented);

            buildMenuNodes();
        }

        private void buildMenuNodes()
        {
            VGMToolbox.util.NodeTagStruct nodeTag = new VGMToolbox.util.NodeTagStruct();
            this.treeviewBoldFont = new Font(this.tvMenuTree.Font, FontStyle.Bold);

            // add empty form
            EmptyForm emptyForm = new EmptyForm();
            this.splitContainer1.Panel2.Controls.Add(emptyForm);

            // set tag for displaying the empty form
            nodeTag.FormClass = emptyForm.GetType().Name;

            TreeNode rootNode = 
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_RootNode"]);
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
            // GENH
            ////////
            TreeNode genh_RootNode = buildGenhTreeNode();
            genh_RootNode.Tag = nodeTag;
            rootNode.Nodes.Add(genh_RootNode);

            /////////
            // Tools
            /////////
            TreeNode tools_RootNode = 
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_MiscellaneousToolsNode"]);
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

            // VGM
            TreeNode vgm_RootNode = buildVgmTreeNode();
            vgm_RootNode.Tag = nodeTag;
            tools_RootNode.Nodes.Add(vgm_RootNode);

            // xSF
            TreeNode xsf_RootNode = buildXsfTreeNode();
            xsf_RootNode.Tag = nodeTag;
            tools_RootNode.Nodes.Add(xsf_RootNode);

            // EXTRACTION                        
            TreeNode ext_RootNode = buildExtractionTreeNode();
            ext_RootNode.Tag = nodeTag;
            tools_RootNode.Nodes.Add(ext_RootNode);

            // Compression                        
            TreeNode comp_RootNode = buildCompressionTreeNode();
            comp_RootNode.Tag = nodeTag;
            tools_RootNode.Nodes.Add(comp_RootNode);

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
            VGMToolbox.util.NodeTagStruct nodeTag = new VGMToolbox.util.NodeTagStruct();
            TreeNode auditing_RootNode =
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_AuditingRootNode"]);
            
            ////////////////////
            // Datafile Creator
            ////////////////////
            TreeNode auditing_DatafileCreatorNode =
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_DatafileCreatorNode"]);

            // add form
            Auditing_DatafileCreatorForm auditing_DatafileCreatorForm = 
                new Auditing_DatafileCreatorForm(auditing_DatafileCreatorNode);
            this.splitContainer1.Panel2.Controls.Add(auditing_DatafileCreatorForm);

            // set tag for displaying the form
            nodeTag.FormClass = auditing_DatafileCreatorForm.GetType().Name;
            auditing_DatafileCreatorNode.Tag = nodeTag;

            /////////////
            // Rebuilder
            /////////////
            TreeNode auditing_RebuilderNode =
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_RebuilderNode"]);

            // add form
            Auditing_RebuilderForm auditing_RebuilderForm =
                new Auditing_RebuilderForm(auditing_RebuilderNode);
            this.splitContainer1.Panel2.Controls.Add(auditing_RebuilderForm);

            // set tag for displaying the form
            nodeTag.FormClass = auditing_RebuilderForm.GetType().Name;
            auditing_RebuilderNode.Tag = nodeTag;

            ////////////////////
            // Datafile Checker
            ////////////////////
            TreeNode auditing_DatafileCheckerNode =
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_DatafileCheckerNode"]);

            // add form
            Auditing_DatafileCheckerForm auditing_DatafileCheckerForm =
                new Auditing_DatafileCheckerForm(auditing_DatafileCheckerNode);
            this.splitContainer1.Panel2.Controls.Add(auditing_DatafileCheckerForm);

            // set tag for displaying the form
            nodeTag.FormClass = auditing_DatafileCheckerForm.GetType().Name;
            auditing_DatafileCheckerNode.Tag = nodeTag;            
            
            //////////////////////
            // Collection Builder
            //////////////////////
            
            /*
            TreeNode auditing_CollectionBuilderNode =
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_CollectionBuilderNode"]);

            // add form
            Auditing_CollectionBuilderForm auditing_CollectionBuilderForm =
                new Auditing_CollectionBuilderForm(auditing_CollectionBuilderNode);
            this.splitContainer1.Panel2.Controls.Add(auditing_CollectionBuilderForm);

            // set tag for displaying the form
            nodeTag.formClass = auditing_CollectionBuilderForm.GetType().Name;
            auditing_CollectionBuilderNode.Tag = nodeTag;            
            */

            auditing_RootNode.NodeFont = this.treeviewBoldFont;

            auditing_RootNode.Nodes.Add(auditing_DatafileCreatorNode);
            auditing_RootNode.Nodes.Add(auditing_RebuilderNode);
            auditing_RootNode.Nodes.Add(auditing_DatafileCheckerNode);
            // auditing_RootNode.Nodes.Add(auditing_CollectionBuilderNode);

            return auditing_RootNode;
        }

        private TreeNode buildExamineTreeNode()
        {
            VGMToolbox.util.NodeTagStruct nodeTag = new VGMToolbox.util.NodeTagStruct();
            TreeNode examine_RootNode =
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_ExamineRootNode"]);

            // Tag Viewer
            TreeNode examine_TagViewerNode =
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_TagViewerNode"]);
            Examine_TagViewerForm examine_TagViewerForm = 
                new Examine_TagViewerForm(examine_TagViewerNode);
            this.splitContainer1.Panel2.Controls.Add(examine_TagViewerForm);

            nodeTag.FormClass = examine_TagViewerForm.GetType().Name;
            examine_TagViewerNode.Tag = nodeTag;

            // Crc Calculator
            TreeNode examine_CrcCalculatorNode =
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_ChecksumNode"]);
            Examine_CrcCalculatorForm examine_CrcCalculatorForm =
                new Examine_CrcCalculatorForm(examine_CrcCalculatorNode);
            this.splitContainer1.Panel2.Controls.Add(examine_CrcCalculatorForm);

            nodeTag.FormClass = examine_CrcCalculatorForm.GetType().Name;
            examine_CrcCalculatorNode.Tag = nodeTag;

            // Search for Files
            TreeNode examine_SearchForFilesNode = new TreeNode(ConfigurationSettings.AppSettings["MenuTree_SearchForFilesNode"]);
            Examine_SearchForFileForm examine_SearchForFileForm =
                new Examine_SearchForFileForm(examine_SearchForFilesNode);
            this.splitContainer1.Panel2.Controls.Add(examine_SearchForFileForm);

            nodeTag.FormClass = examine_SearchForFileForm.GetType().Name;
            examine_SearchForFilesNode.Tag = nodeTag;

            examine_RootNode.NodeFont = this.treeviewBoldFont;
            examine_RootNode.Nodes.Add(examine_TagViewerNode);
            examine_RootNode.Nodes.Add(examine_CrcCalculatorNode);
            examine_RootNode.Nodes.Add(examine_SearchForFilesNode);

            return examine_RootNode;
        }

        private TreeNode buildHootTreeNode()
        {
            VGMToolbox.util.NodeTagStruct nodeTag = new VGMToolbox.util.NodeTagStruct();
            TreeNode hoot_RootNode =
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_HootRootNode"]);

            //TreeNode hoot_CsvDatafileNode = 
            //    new TreeNode(ConfigurationSettings.AppSettings["MenuTree_CsvDatafileNode"]);
            
            ///////////////
            // XML Builder
            ///////////////
            TreeNode hoot_XmlBuilderNode =
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_XmlBuilderNode"]);

            // add form
            Hoot_XmlBuilderForm hoot_XmlBuilderForm = new Hoot_XmlBuilderForm(hoot_XmlBuilderNode);
            this.splitContainer1.Panel2.Controls.Add(hoot_XmlBuilderForm);

            // set tag for displaying the form
            nodeTag.FormClass = hoot_XmlBuilderForm.GetType().Name;
            hoot_XmlBuilderNode.Tag = nodeTag;

            ///////////////
            // Auditor
            ///////////////
            TreeNode hoot_AuditorNode =
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_HootAuditorNode"]);

            // add form
            Hoot_AuditorForm hoot_AuditorForm = new Hoot_AuditorForm(hoot_AuditorNode);
            this.splitContainer1.Panel2.Controls.Add(hoot_AuditorForm);

            // set tag for displaying the form
            nodeTag.FormClass = hoot_AuditorForm.GetType().Name;
            hoot_AuditorNode.Tag = nodeTag;


            hoot_RootNode.NodeFont = this.treeviewBoldFont;

            // hoot_RootNode.Nodes.Add(hoot_CsvDatafileNode);
            hoot_RootNode.Nodes.Add(hoot_XmlBuilderNode);
            hoot_RootNode.Nodes.Add(hoot_AuditorNode);

            return hoot_RootNode;
        }

        private TreeNode buildNsfTreeNode()
        {
            VGMToolbox.util.NodeTagStruct nodeTag = new VGMToolbox.util.NodeTagStruct();

            TreeNode nsf_RootNode = 
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_NsfRootNode"]);
            TreeNode nsf_NsfeM3uNode = 
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_NsfeM3uNode"]);
            nsf_RootNode.NodeFont = this.treeviewBoldFont;

            // add form
            Nsf_Nsfe2NsfM3uForm nsf_Nsfe2NsfM3uForm = new Nsf_Nsfe2NsfM3uForm(nsf_NsfeM3uNode);
            this.splitContainer1.Panel2.Controls.Add(nsf_Nsfe2NsfM3uForm);

            // set tag for displaying the form
            nodeTag.FormClass = nsf_Nsfe2NsfM3uForm.GetType().Name;
            nsf_NsfeM3uNode.Tag = nodeTag;

            nsf_RootNode.Nodes.Add(nsf_NsfeM3uNode);

            return nsf_RootNode;        
        }

        private TreeNode buildGbsTreeNode()
        {
            VGMToolbox.util.NodeTagStruct nodeTag = new VGMToolbox.util.NodeTagStruct();

            TreeNode gbs_RootNode = 
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_GbsRootNode"]);
            TreeNode gbs_M3uNode = 
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_GbsM3uNode"]);
            gbs_RootNode.NodeFont = this.treeviewBoldFont;

            // add form
            Gbs_GbsToM3uForm gbs_GbsToM3uForm = new Gbs_GbsToM3uForm(gbs_M3uNode);
            this.splitContainer1.Panel2.Controls.Add(gbs_GbsToM3uForm);

            // set tag for displaying the form
            nodeTag.FormClass = gbs_GbsToM3uForm.GetType().Name;
            gbs_M3uNode.Tag = nodeTag;

            gbs_RootNode.Nodes.Add(gbs_M3uNode);

            return gbs_RootNode;
        }

        private TreeNode buildXsfTreeNode()
        {
            VGMToolbox.util.NodeTagStruct nodeTag = new VGMToolbox.util.NodeTagStruct();
            EmptyForm emptyForm = new EmptyForm();
            TreeNode xsf_RootNode = 
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_XsfRootNode"]);
            TreeNode _2sf_RootNode = 
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_2sfRootNode"]);
            TreeNode psf_RootNode = 
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_PsfRootNode"]);
            TreeNode psf2_RootNode = 
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_Psf2RootNode"]);
            TreeNode ssf_RootNode = 
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_SsfRootNode"]);


            //////////////
            // 2SF Ripper
            //////////////
            //TreeNode xsf_2sfRipperNode = 
            //    new TreeNode(ConfigurationSettings.AppSettings["MenuTree_2sfRipperOldNode"]);
            
            //// Add 2SF Ripper Form
            //Xsf_2sfRipperForm xsf_2sfRipperForm = new Xsf_2sfRipperForm(xsf_2sfRipperNode);
            //this.splitContainer1.Panel2.Controls.Add(xsf_2sfRipperForm);

            //// Set Tag for displaying the Form
            //nodeTag.formClass = xsf_2sfRipperForm.GetType().Name;
            //xsf_2sfRipperNode.Tag = nodeTag;

            TreeNode xsf_Make2sfNode = new TreeNode(ConfigurationSettings.AppSettings["MenuTree_Make2sfNode"]);

            // Add Make 2SF Form
            Xsf_Mk2sfForm xsf_Mk2sfForm = new Xsf_Mk2sfForm(xsf_Make2sfNode);
            this.splitContainer1.Panel2.Controls.Add(xsf_Mk2sfForm);

            // Set Tag for displaying the Form
            nodeTag.FormClass = xsf_Mk2sfForm.GetType().Name;
            xsf_Make2sfNode.Tag = nodeTag;


            //////////////
            // 2SF Timer
            //////////////
            TreeNode xsf_2sfTimerNode = 
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_2sfTimerNode"]);

            Xsf_2sfTimerForm xsf_2sfTimerForm = new Xsf_2sfTimerForm(xsf_2sfTimerNode);
            this.splitContainer1.Panel2.Controls.Add(xsf_2sfTimerForm);

            nodeTag.FormClass = xsf_2sfTimerForm.GetType().Name;
            xsf_2sfTimerNode.Tag = nodeTag;

            //////////////////
            // SDAT Optimizer
            //////////////////
            TreeNode xsf_SdatOptimizerNode =
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_SdatOptimizerNode"]);

            Xsf_2sfSdatOptimizerForm xsf_2sfSdatOptimizerForm = new Xsf_2sfSdatOptimizerForm(xsf_SdatOptimizerNode);
            this.splitContainer1.Panel2.Controls.Add(xsf_2sfSdatOptimizerForm);

            nodeTag.FormClass = xsf_2sfSdatOptimizerForm.GetType().Name;
            xsf_SdatOptimizerNode.Tag = nodeTag;

            ////////////////////
            // 2SF TAG COPIER
            ////////////////////
            TreeNode xsf_2sfTagMigratorNode =
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_V1toV2CopyNode"]);

            Xsf_2sfTagsMigratorForm xsf_2sfTagsMigratorForm = new Xsf_2sfTagsMigratorForm(xsf_2sfTagMigratorNode);
            this.splitContainer1.Panel2.Controls.Add(xsf_2sfTagsMigratorForm);

            nodeTag.FormClass = xsf_2sfTagsMigratorForm.GetType().Name;
            xsf_2sfTagMigratorNode.Tag = nodeTag;

            ////////////
            // MKPSF2FE
            ////////////
            TreeNode xsf_MkPsf2FENode = 
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_MkPsf2FENode"]);

            // Add UnpkPsf2 Ripper Form
            Xsf_Mkpsf2FrontEndForm xsf_Mkpsf2FrontEndForm =
                new Xsf_Mkpsf2FrontEndForm(xsf_MkPsf2FENode);
            this.splitContainer1.Panel2.Controls.Add(xsf_Mkpsf2FrontEndForm);

            // Set Tag for displaying the Form
            nodeTag.FormClass = xsf_Mkpsf2FrontEndForm.GetType().Name;
            xsf_MkPsf2FENode.Tag = nodeTag;
            
            //////////////
            // UNPKPSF2FE
            //////////////
            TreeNode xsf_UnPsf2FENode = 
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_UnPkPsf2FENode"]);

            // Add UnpkPsf2 Ripper Form
            Xsf_Unpkpsf2FrontEndForm xsf_Unpkpsf2FrontEndForm =
                new Xsf_Unpkpsf2FrontEndForm(xsf_UnPsf2FENode);
            this.splitContainer1.Panel2.Controls.Add(xsf_Unpkpsf2FrontEndForm);

            // Set Tag for displaying the Form
            nodeTag.FormClass = xsf_Unpkpsf2FrontEndForm.GetType().Name;
            xsf_UnPsf2FENode.Tag = nodeTag;
            
            /////////////////
            // PSF2TOPSF2LIB
            /////////////////
            TreeNode xsf_Psf2ToPsf2LibNode = 
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_Psf2ToPsf2LibNode"]);

            // Add UnpkPsf2 Ripper Form
            Xsf_Psf2ToPsf2LibForm xsf_Psf2ToPsf2LibForm =
                new Xsf_Psf2ToPsf2LibForm(xsf_Psf2ToPsf2LibNode);
            this.splitContainer1.Panel2.Controls.Add(xsf_Psf2ToPsf2LibForm);

            // Set Tag for displaying the Form
            nodeTag.FormClass = xsf_Psf2ToPsf2LibForm.GetType().Name;
            xsf_Psf2ToPsf2LibNode.Tag = nodeTag;

            /////////////////
            // PSF2SQEXTRACTOR
            /////////////////
            TreeNode xsf_Psf2SqExtractorNode = 
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_Psf2SqExtractorNode"]);

            // Add UnpkPsf2 Ripper Form
            Xsf_Psf2SqExtractorForm xsf_Psf2SqExtractorForm =
                new Xsf_Psf2SqExtractorForm(xsf_Psf2SqExtractorNode);
            this.splitContainer1.Panel2.Controls.Add(xsf_Psf2SqExtractorForm);

            // Set Tag for displaying the Form
            nodeTag.FormClass = xsf_Psf2SqExtractorForm.GetType().Name;
            xsf_Psf2SqExtractorNode.Tag = nodeTag;

            /////////////////
            // PSF2TIMER
            /////////////////
            TreeNode xsf_Psf2TimerNode =
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_Psf2TimerNode"]);

            // Add UnpkPsf2 Ripper Form
            Xsf_Psf2TimerForm xsf_Psf2TimerForm = new Xsf_Psf2TimerForm(xsf_Psf2TimerNode);
            this.splitContainer1.Panel2.Controls.Add(xsf_Psf2TimerForm);

            // Set Tag for displaying the Form
            nodeTag.FormClass = xsf_Psf2TimerForm.GetType().Name;
            xsf_Psf2TimerNode.Tag = nodeTag;

            //////////////
            // BIN2PSF FE
            //////////////
            TreeNode xsf_Bin2PsfFENode = 
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_Bin2PsfFENode"]);

            // Add UnpkPsf2 Ripper Form
            Xsf_Bin2PsfFrontEndForm xsf_Bin2PsfFrontEndForm =
                new Xsf_Bin2PsfFrontEndForm(xsf_Bin2PsfFENode);
            this.splitContainer1.Panel2.Controls.Add(xsf_Bin2PsfFrontEndForm);

            // Set Tag for displaying the Form
            nodeTag.FormClass = xsf_Bin2PsfFrontEndForm.GetType().Name;
            xsf_Bin2PsfFENode.Tag = nodeTag;
             
            //////////////////////
            // PSF SEQ Extractor
            //////////////////////
            TreeNode xsf_SeqExtractNode =
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_SeqExtractorNode"]);

            // Add Form
            Xsf_PsxSeqExtractForm xsf_PsxSeqExtractForm =
                new Xsf_PsxSeqExtractForm(xsf_SeqExtractNode);
            this.splitContainer1.Panel2.Controls.Add(xsf_PsxSeqExtractForm);

            // Set Tag for displaying the Form
            nodeTag.FormClass = xsf_PsxSeqExtractForm.GetType().Name;
            xsf_SeqExtractNode.Tag = nodeTag;


            // SSFMAKE
            TreeNode xsf_SsfMakeFENode = 
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_SsfMakeFENode"]);
            
            Xsf_SsfMakeFrontEndForm xsf_SsfMakeFrontEndForm = 
                new Xsf_SsfMakeFrontEndForm(xsf_SsfMakeFENode);
            this.splitContainer1.Panel2.Controls.Add(xsf_SsfMakeFrontEndForm);

            nodeTag.FormClass = xsf_SsfMakeFrontEndForm.GetType().Name;
            xsf_SsfMakeFENode.Tag = nodeTag;
            
            // SEQ/TON Extractors
            TreeNode xsf_SsfSeqTonExtFENode = 
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_SsfSeqTonExtFENode"]);

            Xsf_SsfSeqTonExtForm xsf_SsfSeqTonExtForm =
                new Xsf_SsfSeqTonExtForm(xsf_SsfSeqTonExtFENode);
            this.splitContainer1.Panel2.Controls.Add(xsf_SsfSeqTonExtForm);

            nodeTag.FormClass = xsf_SsfSeqTonExtForm.GetType().Name;
            xsf_SsfSeqTonExtFENode.Tag = nodeTag;


            ///////////
            // XSF2EXE
            ///////////
            TreeNode xsf_xsf2ExeNode = 
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_Xsf2ExeNode"]);

            // Add Xsf2Exe Form
            Xsf_Xsf2ExeForm xsf_Xsf2ExeForm = new Xsf_Xsf2ExeForm(xsf_xsf2ExeNode);
            this.splitContainer1.Panel2.Controls.Add(xsf_Xsf2ExeForm);

            // Set Tag for displaying the Form
            nodeTag.FormClass = xsf_Xsf2ExeForm.GetType().Name;
            xsf_xsf2ExeNode.Tag = nodeTag;

            //////////////////
            // XSFRECOMPRESS
            /////////////////
            TreeNode xsf_xsfRecompressNode = 
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_XsfRecompressNode"]);

            // Add XsfRecompress Form
            Xsf_RecompressDataForm xsf_RecompressDataForm = new Xsf_RecompressDataForm(xsf_xsfRecompressNode);
            this.splitContainer1.Panel2.Controls.Add(xsf_RecompressDataForm);

            // Set Tag for displaying the Form
            nodeTag.FormClass = xsf_RecompressDataForm.GetType().Name;
            xsf_xsfRecompressNode.Tag = nodeTag;

            //////////////////
            // TAG EDITOR
            /////////////////
            TreeNode xsf_xsfTagEditorNode = 
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_XsfTagEditorNode"]);

            // Add XsfRecompress Form
            Xsf_XsfTagEditorForm xsf_XsfTagEditorForm = new Xsf_XsfTagEditorForm(xsf_xsfTagEditorNode);
            this.splitContainer1.Panel2.Controls.Add(xsf_XsfTagEditorForm);

            // Set Tag for displaying the Form
            nodeTag.FormClass = xsf_XsfTagEditorForm.GetType().Name;
            xsf_xsfTagEditorNode.Tag = nodeTag;

            nodeTag.FormClass = emptyForm.GetType().Name;
            xsf_RootNode.NodeFont = this.treeviewBoldFont;

            xsf_RootNode.Nodes.Add(xsf_xsf2ExeNode);
            xsf_RootNode.Nodes.Add(xsf_xsfRecompressNode);
            xsf_RootNode.Nodes.Add(xsf_xsfTagEditorNode);

            _2sf_RootNode.NodeFont = this.treeviewBoldFont;
            _2sf_RootNode.Tag = nodeTag;
            //_2sf_RootNode.Nodes.Add(xsf_2sfRipperNode);
            _2sf_RootNode.Nodes.Add(xsf_Make2sfNode);            
            _2sf_RootNode.Nodes.Add(xsf_2sfTimerNode);
            _2sf_RootNode.Nodes.Add(xsf_SdatOptimizerNode);
            _2sf_RootNode.Nodes.Add(xsf_2sfTagMigratorNode);
            
            xsf_RootNode.Nodes.Add(_2sf_RootNode);

            psf_RootNode.NodeFont = this.treeviewBoldFont;
            psf_RootNode.Tag = nodeTag;
            psf_RootNode.Nodes.Add(xsf_Bin2PsfFENode);
            psf_RootNode.Nodes.Add(xsf_SeqExtractNode);
            xsf_RootNode.Nodes.Add(psf_RootNode);

            psf2_RootNode.NodeFont = this.treeviewBoldFont;
            psf2_RootNode.Tag = nodeTag;
            psf2_RootNode.Nodes.Add(xsf_MkPsf2FENode);
            psf2_RootNode.Nodes.Add(xsf_UnPsf2FENode);
            psf2_RootNode.Nodes.Add(xsf_Psf2ToPsf2LibNode);
            psf2_RootNode.Nodes.Add(xsf_Psf2SqExtractorNode);
            psf2_RootNode.Nodes.Add(xsf_Psf2TimerNode);
            xsf_RootNode.Nodes.Add(psf2_RootNode);

            ssf_RootNode.NodeFont = this.treeviewBoldFont;
            ssf_RootNode.Tag = nodeTag;
            ssf_RootNode.Nodes.Add(xsf_SsfMakeFENode);
            ssf_RootNode.Nodes.Add(xsf_SsfSeqTonExtFENode);            
            xsf_RootNode.Nodes.Add(ssf_RootNode);

            return xsf_RootNode;
        }
        
        private TreeNode buildExtractionTreeNode()
        {
            VGMToolbox.util.NodeTagStruct nodeTag = new VGMToolbox.util.NodeTagStruct();
            TreeNode ext_RootNode = 
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_ExtractionRootNode"]);
            ext_RootNode.NodeFont = this.treeviewBoldFont;

            ///////////
            // GENERIC
            ///////////
            TreeNode ext_GenericNode =
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_ExtractionGenericRootNode"]);
            ext_GenericNode.NodeFont = this.treeviewBoldFont;

            nodeTag = new VGMToolbox.util.NodeTagStruct();
            EmptyForm emptyForm = new EmptyForm();
            nodeTag.FormClass = emptyForm.GetType().Name;
            ext_GenericNode.Tag = nodeTag;

            // Snakebite
            TreeNode ext_SimpleCutterNode = 
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_SimpleCutterNode"]);

            // Add Offset Finder Form
            Extract_SnakebiteGuiForm extract_SnakebiteGuiForm = new Extract_SnakebiteGuiForm(ext_SimpleCutterNode);
            this.splitContainer1.Panel2.Controls.Add(extract_SnakebiteGuiForm);

            // Set Tag for displaying the Form
            nodeTag.FormClass = extract_SnakebiteGuiForm.GetType().Name;
            ext_SimpleCutterNode.Tag = nodeTag;


            // Offset Finder
            TreeNode ext_OffsetFinderNode =
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_OffsetFinderNode"]);

            // Add Offset Finder Form
            Extract_OffsetFinderForm extract_OffsetFinderForm = new Extract_OffsetFinderForm(ext_OffsetFinderNode);
            this.splitContainer1.Panel2.Controls.Add(extract_OffsetFinderForm);

            // Set Tag for displaying the Form
            nodeTag.FormClass = extract_OffsetFinderForm.GetType().Name;
            ext_OffsetFinderNode.Tag = nodeTag;


            // CD-XA Extractor
            TreeNode ext_ExtractCdxaNode =
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_ExtractCdxaNode"]);

            // Add Cdxa Extractor Form
            Extract_ExtractCdxaForm extract_ExtractCdxaForm = new Extract_ExtractCdxaForm(ext_ExtractCdxaNode);
            this.splitContainer1.Panel2.Controls.Add(extract_ExtractCdxaForm);

            // Set Tag for displaying the Form
            nodeTag.FormClass = extract_ExtractCdxaForm.GetType().Name;
            ext_ExtractCdxaNode.Tag = nodeTag;

            ///////
            // NDS
            ///////
            TreeNode ext_NdsNode =
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_ExtractionNdsRootNode"]);
            ext_NdsNode.NodeFont = this.treeviewBoldFont;

            nodeTag = new VGMToolbox.util.NodeTagStruct();
            nodeTag.FormClass = emptyForm.GetType().Name;
            ext_NdsNode.Tag = nodeTag;

            // SDAT Extractor
            TreeNode ext_SdatExtractorNode = 
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_SdatExtractorNode"]);            

            // Add SDAT Extractor Form
            Xsf_SdatExtractorForm xsf_SdatExtractorForm = new Xsf_SdatExtractorForm(ext_SdatExtractorNode);
            this.splitContainer1.Panel2.Controls.Add(xsf_SdatExtractorForm);

            // Set Tag for displaying the Form
            nodeTag.FormClass = xsf_SdatExtractorForm.GetType().Name;
            ext_SdatExtractorNode.Tag = nodeTag;

            // SDAT Finder
            TreeNode ext_SdatFinderNode = 
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_SdatFinderNode"]);

            // Add SDAT Extractor Form
            Xsf_SdatFinderForm xsf_SdatFinderForm = new Xsf_SdatFinderForm(ext_SdatFinderNode);
            this.splitContainer1.Panel2.Controls.Add(xsf_SdatFinderForm);

            // Set Tag for displaying the Form
            nodeTag.FormClass = xsf_SdatFinderForm.GetType().Name;
            ext_SdatFinderNode.Tag = nodeTag;

            ///////
            // PC
            ///////
            TreeNode ext_PcNode = 
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_ExtractionPcRootNode"]);
            ext_PcNode.NodeFont = this.treeviewBoldFont;

            nodeTag = new VGMToolbox.util.NodeTagStruct();
            emptyForm = new EmptyForm();
            nodeTag.FormClass = emptyForm.GetType().Name;
            ext_PcNode.Tag = nodeTag;

            ext_GenericNode.Nodes.Add(ext_SimpleCutterNode);
            ext_GenericNode.Nodes.Add(ext_OffsetFinderNode);
            ext_GenericNode.Nodes.Add(ext_ExtractCdxaNode);
                               
            ext_RootNode.Nodes.Add(ext_GenericNode);
            
            ext_NdsNode.Nodes.Add(ext_SdatExtractorNode);
            ext_NdsNode.Nodes.Add(ext_SdatFinderNode);            
            ext_RootNode.Nodes.Add(ext_NdsNode);
            
            // ext_RootNode.Nodes.Add(ext_PcNode);

            return ext_RootNode;
        }

        private TreeNode buildGenhTreeNode()
        {
            VGMToolbox.util.NodeTagStruct nodeTag = new VGMToolbox.util.NodeTagStruct();

            TreeNode genh_RootNode = new TreeNode(ConfigurationSettings.AppSettings["MenuTree_GenhRootNode"]);
            genh_RootNode.NodeFont = this.treeviewBoldFont;

            // GENH Creator
            TreeNode genh_CreatorNode =
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_GenhCreationRootNode"]);

            // Add GENH Creator Form
            Genh_CreatorForm genh_CreatorForm = new Genh_CreatorForm(genh_CreatorNode);
            this.splitContainer1.Panel2.Controls.Add(genh_CreatorForm);

            // Set Tag for displaying the Form
            nodeTag.FormClass = genh_CreatorForm.GetType().Name;
            genh_CreatorNode.Tag = nodeTag;

            genh_RootNode.Nodes.Add(genh_CreatorNode);

            return genh_RootNode;
        }

        private TreeNode buildVgmTreeNode()
        {
            VGMToolbox.util.NodeTagStruct nodeTag = new VGMToolbox.util.NodeTagStruct();

            TreeNode vgm_RootNode = new TreeNode(ConfigurationSettings.AppSettings["MenuTree_VgmRootNode"]);
            vgm_RootNode.NodeFont = this.treeviewBoldFont;

            // VGM Tagger
            TreeNode vgm_TaggerNode = new TreeNode(ConfigurationSettings.AppSettings["MenuTree_VgmTagEditorNode"]);

            // Add  Form
            Vgm_VgmTagEditorForm vgm_VgmTagEditorForm = new Vgm_VgmTagEditorForm(vgm_TaggerNode);
            this.splitContainer1.Panel2.Controls.Add(vgm_VgmTagEditorForm);

            // Set Tag for displaying the Form
            nodeTag.FormClass = vgm_VgmTagEditorForm.GetType().Name;
            vgm_TaggerNode.Tag = nodeTag;

            vgm_RootNode.Nodes.Add(vgm_TaggerNode);

            return vgm_RootNode;
        }

        private TreeNode buildCompressionTreeNode()
        {
            VGMToolbox.util.NodeTagStruct nodeTag = new VGMToolbox.util.NodeTagStruct();
            TreeNode comp_RootNode = 
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_CompressionRootNode"]);
            comp_RootNode.NodeFont = this.treeviewBoldFont;

            nodeTag = new VGMToolbox.util.NodeTagStruct();
            EmptyForm emptyForm = new EmptyForm();
            nodeTag.FormClass = emptyForm.GetType().Name;

            // GZIP
            TreeNode ext_ExtractGzipNode =
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_ExtractGzipNode"]);

            // Add Form
            Compression_GzipCompressionForm compression_GzipCompressionForm =
                new Compression_GzipCompressionForm(ext_ExtractGzipNode);
            this.splitContainer1.Panel2.Controls.Add(compression_GzipCompressionForm);

            // Set Tag for displaying the Form
            nodeTag.FormClass = compression_GzipCompressionForm.GetType().Name;
            ext_ExtractGzipNode.Tag = nodeTag;

            // ZLIB
            TreeNode ext_ExtractZlibNode = 
                new TreeNode(ConfigurationSettings.AppSettings["MenuTree_ExtractZlibNode"]);

            // Add Form
            Extract_ZlibExtractForm extract_ZlibExtractForm = new Extract_ZlibExtractForm(ext_ExtractZlibNode);
            this.splitContainer1.Panel2.Controls.Add(extract_ZlibExtractForm);

            // Set Tag for displaying the Form
            nodeTag.FormClass = extract_ZlibExtractForm.GetType().Name;
            ext_ExtractZlibNode.Tag = nodeTag;

            comp_RootNode.Nodes.Add(ext_ExtractGzipNode);
            comp_RootNode.Nodes.Add(ext_ExtractZlibNode);

            return comp_RootNode;
        }

        private void tvMenuTree_doClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag != null)
            {
                VGMToolbox.util.NodeTagStruct nts = (VGMToolbox.util.NodeTagStruct)e.Node.Tag;

                // need to fix this so it only changes if the form is not "running"
                //VgmtForm.ResetNodeColor(e.Node); 

                showForm(this.splitContainer1.Panel2.Controls, nts.FormClass);
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
                VGMToolbox.util.NodeTagStruct nts = (VGMToolbox.util.NodeTagStruct)e.Node.Tag;

                // need to fix this so it only changes if the form is not "running"
                //VgmtForm.ResetNodeColor(e.Node); 

                showForm(this.splitContainer1.Panel2.Controls, nts.FormClass);
            }
            else
            {
                showForm(this.splitContainer1.Panel2.Controls, "ZZZ_NotYetImplemented");
            }           
        }
    }
}
