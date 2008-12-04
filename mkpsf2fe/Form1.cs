using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

using VGMToolbox.util;

namespace mkpsf2fe
{
    public partial class Form1 : Form
    {
        MkPsf2Worker mkPsf2Worker;
        
        public Form1()
        {                        
            InitializeComponent();
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            tbOutput.Clear();
            toolStripStatusLabel1.Text = "Build PSF2s...Begin";

            MkPsf2Worker.MkPsf2Struct mkStruct = new MkPsf2Worker.MkPsf2Struct();
            mkStruct.sourcePath = tbSourceDirectory.Text;
            mkStruct.modulePath = tbModulesDirectory.Text;

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
        }

        private void backgroundWorker_ReportProgress(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage != Constants.IGNORE_PROGRESS &&
                e.ProgressPercentage != Constants.PROGRESS_MSG_ONLY)
            {
                toolStripProgressBar.Value = e.ProgressPercentage;
                this.Text = "mkpsf2FE [" + e.ProgressPercentage + "%]";
            }

            if ((e.ProgressPercentage == Constants.PROGRESS_MSG_ONLY) && e.UserState != null)
            {
                Constants.ProgressStruct vProgressStruct = (Constants.ProgressStruct)e.UserState;
                tbOutput.Text += vProgressStruct.genericMessage;
            }
            else if (e.UserState != null)
            {
                Constants.ProgressStruct vProgressStruct = (Constants.ProgressStruct)e.UserState;

                tbOutput.Text += vProgressStruct.errorMessage == null ? String.Empty : vProgressStruct.errorMessage;
            }
        }

        private void btnSourceDirectoryBrowse_Click(object sender, EventArgs e)
        {
            folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                tbSourceDirectory.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (mkPsf2Worker != null && mkPsf2Worker.IsBusy)
            {
                mkPsf2Worker.CancelAsync();
            }
        }

        private void btnModulesDirectoryBrowse_Click(object sender, EventArgs e)
        {
            folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                tbModulesDirectory.Text = folderBrowserDialog.SelectedPath;
            }
        }
    }
}
