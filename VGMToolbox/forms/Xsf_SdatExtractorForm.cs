using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.tools.nds;

namespace VGMToolbox.forms
{
    public partial class Xsf_SdatExtractorForm : VgmtForm
    {
        SdatExtractorWorker sdatExtractorWorker;

        DateTime elapsedTimeStart;
        DateTime elapsedTimeEnd;
        TimeSpan elapsedTime;

        public Xsf_SdatExtractorForm()
        {
            this.TopLevel = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Dock = DockStyle.Fill;           
            this.lblTitle.Text = "SDAT Extractor";
            
            InitializeComponent();
        }

        private void tbNDS_SdatExtractor_Source_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;
        }

        private void tbNDS_SdatExtractor_Source_DragDrop(object sender, DragEventArgs e)
        {
            this.tbOutput.Clear();
            
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
    }
}
