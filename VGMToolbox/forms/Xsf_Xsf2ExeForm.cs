using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.xsf;

namespace VGMToolbox.forms
{
    public partial class Xsf_Xsf2ExeForm : AVgmtForm
    {        
        public Xsf_Xsf2ExeForm(TreeNode pTreeNode) : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = 
                ConfigurationSettings.AppSettings["Form_Xsf2Exe_Title"];

            // hide the DoTask button since this is a drag and drop form
            this.btnDoTask.Hide();            
            
            InitializeComponent();

            this.grpXsfPsf2Exe_Source.Text =
                ConfigurationSettings.AppSettings["Form_Xsf2Exe_GroupSourceFiles"];
            this.lblDragNDrop.Text =
                ConfigurationSettings.AppSettings["Form_Xsf2Exe_LblDragNDrop"];
            this.grpOptions.Text =
                ConfigurationSettings.AppSettings["Form_Xsf2Exe_GroupOptions"];
            this.cbExtractReservedSection.Text =
                ConfigurationSettings.AppSettings["Form_Xsf2Exe_CheckBoxExtractReservedSection"];
            this.cbXsfPsf2Exe_IncludeOrigExt.Text =
                ConfigurationSettings.AppSettings["Form_Xsf2Exe_CheckBoxIncludeOriginalExtension"];
            this.cbXsfPsf2Exe_StripGsfHeader.Text =
                ConfigurationSettings.AppSettings["Form_Xsf2Exe_CheckBoxStripGsfHeader"];
        }

        private void tbXsfPsf2Exe_Source_DragDrop(object sender, DragEventArgs e)
        {            
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            XsfCompressedProgramExtractorWorker.XsfCompressedProgramExtractorStruct xcpeStruct =
                new XsfCompressedProgramExtractorWorker.XsfCompressedProgramExtractorStruct();
            xcpeStruct.SourcePaths = s;
            xcpeStruct.includeExtension = cbXsfPsf2Exe_IncludeOrigExt.Checked;
            xcpeStruct.stripGsfHeader = cbXsfPsf2Exe_StripGsfHeader.Checked;
            xcpeStruct.extractReservedSection = cbExtractReservedSection.Checked;

            base.backgroundWorker_Execute(xcpeStruct);
        }
        
        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }

        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new XsfCompressedProgramExtractorWorker();
        }
        protected override string getCancelMessage()
        {
            return ConfigurationSettings.AppSettings["Form_Xsf2Exe_MessageCancel"];
        }
        protected override string getCompleteMessage()
        {
            return ConfigurationSettings.AppSettings["Form_Xsf2Exe_MessageComplete"];
        }
        protected override string getBeginMessage()
        {
            return ConfigurationSettings.AppSettings["Form_Xsf2Exe_MessageBegin"];
        }
    }
}
