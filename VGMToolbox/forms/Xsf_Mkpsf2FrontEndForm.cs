using System;
using System.ComponentModel;
using System.Windows.Forms;

using VGMToolbox.tools.xsf;

namespace VGMToolbox.forms
{
    public partial class Xsf_Mkpsf2FrontEndForm : VgmtForm
    {
        MkPsf2Worker mkPsf2Worker;
        
        public Xsf_Mkpsf2FrontEndForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = "mkpsf2 Front End";
            this.btnDoTask.Text = "Build PSF2s";
            
            this.tbOutput.Text = "Source Directory: Path to BD/HD/SQ files." + Environment.NewLine;
            this.tbOutput.Text += "Modules Directory: Path to IRX files." + Environment.NewLine + Environment.NewLine;
            this.tbOutput.Text += "Notes: Be sure matching BD/HD/SQ files have the same file name.";

            InitializeComponent();
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            base.initializeProcessing();
            toolStripStatusLabel1.Text = "Build PSF2s...Begin";

            MkPsf2Worker.MkPsf2Struct mkStruct = new MkPsf2Worker.MkPsf2Struct();
            mkStruct.sourcePath = tbSourceDirectory.Text;
            mkStruct.modulePath = tbModulesDirectory.Text;
            mkStruct.outputFolder = tbOutputFolderName.Text;

            mkStruct.depth = tbDepth.Text;
            mkStruct.reverb = tbReverb.Text;
            mkStruct.tempo = tbTempo.Text;
            mkStruct.tickInterval = tbTickInterval.Text;
            mkStruct.volume = tbVolume.Text;

            mkPsf2Worker = new MkPsf2Worker();
            mkPsf2Worker.ProgressChanged += backgroundWorker_ReportProgress;
            mkPsf2Worker.RunWorkerCompleted += MkPsf2Worker_WorkComplete;
            mkPsf2Worker.RunWorkerAsync(mkStruct);
        }

        private void MkPsf2Worker_WorkComplete(object sender,
            RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                toolStripStatusLabel1.Text = "Build PSF2s...Cancelled";
                tbOutput.Text += "Operation cancelled.";
            }
            else
            {
                toolStripStatusLabel1.Text = "Build PSF2s...Complete";
            }

            // update node color
            setNodeAsComplete();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (mkPsf2Worker != null && mkPsf2Worker.IsBusy)
            {
                tbOutput.Text += "CANCEL PENDING...";
                mkPsf2Worker.CancelAsync();
                this.errorFound = true;
            }
        }

        private void btnSourceDirectoryBrowse_Click(object sender, EventArgs e)
        {
            tbSourceDirectory.Text = base.browseForFolder(sender, e);
        }

        private void btnModulesDirectoryBrowse_Click(object sender, EventArgs e)
        {
            tbModulesDirectory.Text = base.browseForFolder(sender, e);
        }
    }
}
