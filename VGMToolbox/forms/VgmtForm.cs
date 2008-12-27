using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.util;

namespace VGMToolbox.forms
{
    public partial class VgmtForm : Form
    {
        protected DateTime elapsedTimeStart;
        protected DateTime elapsedTimeEnd;
        protected TimeSpan elapsedTime;
        
        public VgmtForm()
        {
            this.TopLevel = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Dock = DockStyle.Fill;
            
            InitializeComponent();
        }

        protected virtual void doDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;        
        }

        protected void initializeProcessing()
        {
            tbOutput.Clear();

            this.elapsedTimeStart = DateTime.Now;
            this.showElapsedTime();
        }

        protected void showElapsedTime()
        {
            this.elapsedTimeEnd = DateTime.Now;
            this.elapsedTime = new TimeSpan();
            this.elapsedTime = elapsedTimeEnd - elapsedTimeStart;
            this.lblTimeElapsed.Text = String.Format("{0:D2}:{1:D2}:{2:D2}", elapsedTime.Hours, elapsedTime.Minutes, elapsedTime.Seconds);
        }

        protected void backgroundWorker_ReportProgress(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage != Constants.IGNORE_PROGRESS &&
                e.ProgressPercentage != Constants.PROGRESS_MSG_ONLY)
            {
                this.toolStripProgressBar.Value = e.ProgressPercentage;
                this.Text = "VGMToolbox [" + e.ProgressPercentage + "%]";
            }

            if ((e.ProgressPercentage == Constants.PROGRESS_MSG_ONLY) && e.UserState != null)
            {
                Constants.ProgressStruct vProgressStruct = (Constants.ProgressStruct)e.UserState;
                tbOutput.Text += vProgressStruct.genericMessage;
            }
            else if (e.UserState != null)
            {
                Constants.ProgressStruct vProgressStruct = (Constants.ProgressStruct)e.UserState;

                lblProgressLabel.Text = vProgressStruct.filename == null ? String.Empty : vProgressStruct.filename;
                tbOutput.Text += vProgressStruct.errorMessage == null ? String.Empty : vProgressStruct.errorMessage;
            }

            this.showElapsedTime();
        }
    }
}
