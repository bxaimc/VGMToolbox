using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using VGMToolbox.util;

namespace VGMToolbox.plugin
{
    public abstract partial class AVgmtForm : Form
    {
        protected DateTime elapsedTimeStart;
        protected DateTime elapsedTimeEnd;
        protected TimeSpan elapsedTime;
        
        protected TreeNode menuTreeNode;

        protected bool errorFound;

        protected AVgmtForm()
        {
            menuTreeNode = null;

            this.TopLevel = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            InitializeComponent();
        }

        protected AVgmtForm(TreeNode pTreeNode)
        {
            menuTreeNode = pTreeNode;
            
            this.TopLevel = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            InitializeComponent();
        }

        protected static void ResetNodeColor(TreeNode pTreeNode)
        {
            // reset colors to indicate a fresh status
            pTreeNode.BackColor = Color.White;
            pTreeNode.ForeColor = Color.Black;
        }

        protected virtual void DoDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;        
        }

        protected virtual void InitializeProcessing()
        {
            errorFound = false;
            
            this.tbOutput.Clear();

            this.SetNodeAsWorking();

            this.elapsedTimeStart = DateTime.Now;
            this.ShowElapsedTime();
        }

        protected virtual void BackgroundWorker_ReportProgress(object sender, ProgressChangedEventArgs e)
        {
            VGMToolbox.util.ProgressStruct vProgressStruct = (VGMToolbox.util.ProgressStruct)e.UserState;

            if (e.ProgressPercentage != Constants.IGNORE_PROGRESS &&
                e.ProgressPercentage != Constants.PROGRESS_MSG_ONLY)
            {
                this.toolStripProgressBar.Value = e.ProgressPercentage;
                // this.Text = "VGMToolbox [" + e.ProgressPercentage + "%]";

                if (!String.IsNullOrEmpty(vProgressStruct.GenericMessage))
                {
                    this.tbOutput.Text += vProgressStruct.GenericMessage;
                }
            }

            if ((e.ProgressPercentage == Constants.PROGRESS_MSG_ONLY) && e.UserState != null)
            {
                tbOutput.Text += vProgressStruct.GenericMessage;
            }
            else if (e.UserState != null)
            {
                lblProgressLabel.Text = vProgressStruct.Filename == null ? String.Empty : vProgressStruct.Filename;

                if (!String.IsNullOrEmpty(vProgressStruct.ErrorMessage))
                {
                    tbOutput.Text += vProgressStruct.ErrorMessage;
                    errorFound = true;
                }
            }

            this.ShowElapsedTime();
        }

        protected void SetNodeAsWorking()
        {
            // set colors to indicate a working status
            menuTreeNode.BackColor = Color.Yellow;
            menuTreeNode.ForeColor = Color.Black;        
        }

        protected void SetNodeAsComplete()
        {
            if (errorFound)
            {
                this.SetNodeAsError();            
            }
            else
            {
                // set colors to indicate a complete status
                menuTreeNode.BackColor = Color.Green;
                menuTreeNode.ForeColor = Color.White;
            }
        }

        protected void SetNodeAsError()
        {            
            // set colors to indicate a error status
            menuTreeNode.BackColor = Color.Red;
            menuTreeNode.ForeColor = Color.White;
        }

        protected void ShowElapsedTime()
        {
            this.elapsedTimeEnd = DateTime.Now;
            this.elapsedTime = new TimeSpan();
            this.elapsedTime = elapsedTimeEnd - elapsedTimeStart;
            this.lblTimeElapsed.Text = String.Format("{0:D2}:{1:D2}:{2:D2}", elapsedTime.Hours, elapsedTime.Minutes, elapsedTime.Seconds);
        }

        protected string BrowseForFile(object sender, EventArgs e)
        {
            string filename = String.Empty;

            openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filename = openFileDialog1.FileName;
            }

            return filename;
        }

        protected string BrowseForFolder(object sender, EventArgs e)
        {
            string foldername = String.Empty;

            folderBrowserDialog1 = new FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                foldername = folderBrowserDialog1.SelectedPath;
            }

            return foldername;
        }

        protected bool CheckTextBox(string pText, string pFieldName)
        {
            bool ret = true;
            
            if (pText.Trim().Length == 0)
            {
                MessageBox.Show(String.Format("{0} cannot be empty.", pFieldName), "Required Field Missing.");
                ret = false;
            }
            return ret;
        }
        
        protected bool CheckFileExists(string pPath, string pLabel)
        {
            bool ret = true;

            if (!File.Exists(pPath))
            {
                ret = false;
                MessageBox.Show(String.Format("Cannot find the file selected for \"{0}\": <{1}>", pLabel, pPath), "File Not Found.");
            }

            return ret;
        }

        protected bool CheckFolderExists(string pPath, string pLabel)
        {
            bool ret = true;

            if (!Directory.Exists(pPath))
            {
                ret = false;
                MessageBox.Show(String.Format("Cannot find the directory selected for \"{0}\": <{1}>", pLabel, pPath), "Directory Not Found.");
            }

            return ret;
        }
                
        private void tbOutput_DoubleClick(object sender, EventArgs e)
        {
            string tempFileName = Path.GetTempFileName();
            
            // write output to a temp file
            using (StreamWriter sw = new StreamWriter(File.Open(tempFileName, FileMode.Open, FileAccess.Write)))
            {
                sw.Write(tbOutput.Text);
            }

            File.Move(tempFileName, Path.ChangeExtension(tempFileName, ".txt"));
            Process.Start(Path.ChangeExtension(tempFileName, ".txt"));
        }
    }
}
