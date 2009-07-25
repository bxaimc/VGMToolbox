using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.format.util;
using VGMToolbox.plugin;
using VGMToolbox.tools.xsf;

namespace VGMToolbox.forms
{
    public partial class PsfStubMakerForm : AVgmtForm
    {
        public PsfStubMakerForm(TreeNode pTreeNode): 
            base(pTreeNode)
        {
            InitializeComponent();

            this.grpSourceFiles.AllowDrop = true;
            this.lblTitle.Text = "PSF Driver Stub Creator";
            this.btnDoTask.Hide();
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }

        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new PsfStubMakerWorker();
        }
        protected override string getCancelMessage()
        {
            return "Building PSF Driver Stub...Cancelled";
        }
        protected override string getCompleteMessage()
        {
            return "Building PSF Driver Stub...Complete";
        }
        protected override string getBeginMessage()
        {
            return "Building PSF Driver Stub...Begin";
        }

        private void grpSourceFiles_DragDrop(object sender, DragEventArgs e)
        {
            // check for PsyQ in PATH
            if (!XsfUtil.IsPsyQPathVariablePresent())
            {
                MessageBox.Show("PSYQ_PATH environment variable not found.", "ERROR");                           
            }
            else if (!XsfUtil.IsPsyQSdkPresent())
            {
                MessageBox.Show("PsyQ SDK not found in the directory represented by the environment variable %PSYQ_PATH%\\BIN." +
                    "  Please update the variable or install the PsyQ SDK.", "ERROR");
            }
            else
            {
                string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

                PsfStubMakerStruct bwStruct = new PsfStubMakerStruct();
                bwStruct.SourcePaths = s;
                bwStruct.DriverText = this.tbDriverText.Text;

                base.backgroundWorker_Execute(bwStruct);
            }
        }
    }
}
