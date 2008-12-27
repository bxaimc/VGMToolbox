using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.tools.nds;
using VGMToolbox.util;

namespace VGMToolbox.forms
{
    public partial class Xsf_SdatExtractorForm : VgmtForm
    {
        SdatExtractorWorker sdatExtractorWorker;

        public Xsf_SdatExtractorForm()
        {            
            // setup specific input
            this.lblTitle.Text = "SDAT Extractor";
            this.btnDoTask.Hide();

            InitializeComponent();
        }

        private void tbNDS_SdatExtractor_Source_DragDrop(object sender, DragEventArgs e)
        {
            toolStripStatusLabel1.Text = "SDAT Extraction...Begin";

            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            int totalFileCount = 0;

            foreach (string path in s)
            {
                if (File.Exists(path))
                {
                    totalFileCount++;
                }
                else if (Directory.Exists(path))
                {
                    totalFileCount += Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Length;
                }
            }

            SdatExtractorWorker.SdatExtractorStruct sdexStruct = new SdatExtractorWorker.SdatExtractorStruct();
            sdexStruct.pPaths = s;
            sdexStruct.totalFiles = totalFileCount;

            sdatExtractorWorker = new SdatExtractorWorker();
            sdatExtractorWorker.ProgressChanged += backgroundWorker_ReportProgress;
            sdatExtractorWorker.RunWorkerCompleted += SdatExtractorWorker_WorkComplete;
            sdatExtractorWorker.RunWorkerAsync(sdexStruct);
        }

        private void SdatExtractorWorker_WorkComplete(object sender,
                     RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                toolStripStatusLabel1.Text = "SDAT Extraction...Cancelled";
                tbOutput.Text += "Operation cancelled.";
            }
            else
            {
                lblProgressLabel.Text = String.Empty;
                toolStripStatusLabel1.Text = "SDAT Extraction...Complete";
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (sdatExtractorWorker != null && sdatExtractorWorker.IsBusy)
            {
                tbOutput.Text += "CANCEL PENDING...";
                sdatExtractorWorker.CancelAsync();
            }
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }
    }
}