using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.xsf;

namespace VGMToolbox.forms.xsf
{
    public partial class PsfDataFinderForm : AVgmtForm
    {
        public PsfDataFinderForm(TreeNode pTreeNode) :
            base(pTreeNode)
        {
            InitializeComponent();

            this.btnDoTask.Hide();
            this.grpSource.AllowDrop = true;

            this.lblTitle.Text = "PSF Data Extractor";
            this.tbOutput.Text = "- Extract SEQ/VH/VB data from files." + Environment.NewLine;
            this.tbOutput.Text += "- VH/VB should always be correctly paired." + Environment.NewLine;
            this.tbOutput.Text += "- WARNING: VB DETECTION IS VERY SLOW, PLEASE BE PATIENT..." + Environment.NewLine;
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }

        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new PsfDataFinderWorker();
        }
        protected override string getCancelMessage()
        {
            return "Extracting PSF Data...Cancelled.";
        }
        protected override string getCompleteMessage()
        {
            return "Extracting PSF Data...Complete.";
        }
        protected override string getBeginMessage()
        {
            return "Extracting PSF Data...Begin";
        }

        private void grpSource_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            
            PsfDataFinderWorker.PsfDataFinderStruct bwStruct = new PsfDataFinderWorker.PsfDataFinderStruct();
            bwStruct.SourcePaths = s;
            bwStruct.UseSeqMinimumSize = cbUseMinimum.Checked;
            bwStruct.ReorderSeqFiles = cbRenameSeq.Checked;
            bwStruct.UseZeroOffsetForVb = cbVbStartAt0x00.Checked;
            bwStruct.RelaxVbEofRestrictions = cbRelaxVbEof.Checked;

            if (cbUseMinimum.Checked)
            {
                bwStruct.MinimumSize = int.Parse(this.tbMinimumSize.Text);
            }

            base.backgroundWorker_Execute(bwStruct);
        }
    }
}
