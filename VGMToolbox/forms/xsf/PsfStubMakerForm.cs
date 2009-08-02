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

namespace VGMToolbox.forms.xsf
{
    public partial class PsfStubMakerForm : AVgmtForm
    {
        public PsfStubMakerForm(TreeNode pTreeNode): 
            base(pTreeNode)
        {
            InitializeComponent();

            this.grpSourceFiles.AllowDrop = true;
            this.lblTitle.Text = "PSF Driver Stub Creator";
            this.tbOutput.Text = "PsyQ SDK must be installed." + Environment.NewLine;
            this.tbOutput.Text += "Be sure to add your PSYQ and PSYQ\\BIN folders to your PATH environment variable." + Environment.NewLine;
            this.tbOutput.Text += "Also please create a PSYQ_PATH environment variable pointing to the top level PSYQ folder." + Environment.NewLine;
            
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
                MessageBox.Show("Required PsyQ SDK executables (CCPSX.EXE, PSYLINK.EXE) not found in any of the directories listed in your PATH environment variable." +
                    "  Please update your PATH environment variables or install the PsyQ SDK.", "ERROR");
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
