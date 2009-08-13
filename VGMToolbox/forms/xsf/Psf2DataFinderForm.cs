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
    public partial class Psf2DataFinderForm : AVgmtForm
    {
        public Psf2DataFinderForm(TreeNode pTreeNode) :
            base(pTreeNode)
        {
            InitializeComponent();

            this.btnDoTask.Hide();
            this.grpSource.AllowDrop = true;

            this.lblTitle.Text = "PSF2 Data Extractor";
            this.tbOutput.Text = "- Extract SQ/HD/BD data from files." + Environment.NewLine;
            this.tbOutput.Text += "- HD/BD should always be correctly paired." + Environment.NewLine;
            this.tbOutput.Text += "- WARNING: BD DETECTION IS VERY SLOW, PLEASE BE PATIENT..." + Environment.NewLine;
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }

        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new Psf2DataFinderWorker();
        }
        protected override string getCancelMessage()
        {
            return "Extracting PSF2 Data...Cancelled.";
        }
        protected override string getCompleteMessage()
        {
            return "Extracting PSF2 Data...Complete.";
        }
        protected override string getBeginMessage()
        {
            return "Extracting PSF2 Data...Begin";
        }

        private void grpSource_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            Psf2DataFinderWorker.Psf2DataFinderStruct bwStruct = new Psf2DataFinderWorker.Psf2DataFinderStruct();
            bwStruct.SourcePaths = s;
            bwStruct.UseSeqMinimumSize = cbUseMinimum.Checked;
            
            if (cbUseMinimum.Checked)
            {
                bwStruct.MinimumSize = int.Parse(this.tbMinimumSize.Text);
            }

            base.backgroundWorker_Execute(bwStruct);
        }
    }
}
