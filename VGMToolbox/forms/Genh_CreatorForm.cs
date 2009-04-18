using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.dbutil;
using VGMToolbox.format.util;
using VGMToolbox.plugin;
using VGMToolbox.tools.genh;

namespace VGMToolbox.forms
{
    public partial class Genh_CreatorForm : VgmtForm
    {
        private static readonly string DB_PATH =
            Path.Combine(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "db"), "collection.s3db");
                
        public Genh_CreatorForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            InitializeComponent();

            this.lblTitle.Text = "Create GENHs";
            this.tbSourceDirectory.Text = "<Source Directory>";
            this.btnDoTask.Text = "Create GENHs";

            this.loadFormats();
        }
             
        private void loadFormats()
        {
            this.comboFormat.DataSource = SqlLiteUtil.GetSimpleDataTable(DB_PATH, "GenhFormats", "GenhFormatDescription");
            this.comboFormat.DisplayMember = "GenhFormatDescription";
            this.comboFormat.ValueMember = "GenhFormatId";
        }

        private void comboFormat_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
        private void comboFormat_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void btnBrowseDirectory_Click(object sender, EventArgs e)
        {
            this.tbSourceDirectory.Text = base.browseForFolder(sender, e);
        }
        private void tbSourceDirectory_TextChanged(object sender, EventArgs e)
        {
            this.lbFiles.Items.Clear();
            
            if (Directory.Exists(this.tbSourceDirectory.Text))
            {                                
                foreach (string f in Directory.GetFiles(this.tbSourceDirectory.Text))
                {
                    this.lbFiles.Items.Add(Path.GetFileName(f));
                }
            }
            else
            {
                this.lbFiles.Items.Clear();
            }
        }        
        private void comboFormat_SelectedValueChanged(object sender, EventArgs e)
        {
            DataRowView drv = (DataRowView)this.comboFormat.SelectedItem;

            if ((Convert.ToUInt16(drv.Row.ItemArray[0]) == 0) || 
                (Convert.ToUInt16(drv.Row.ItemArray[0]) == 14))
            {
                this.cbFindLoop.Enabled = true;
            }
            else
            {
                this.cbFindLoop.Enabled = false;
            }

            if (Convert.ToUInt16(drv.Row.ItemArray[0]) == 12)
            {
                lblRightCoef.Show();
                tbRightCoef.Show();
                lblLeftCoef.Show();
                tbLeftCoef.Show();
                cbCapcomHack.Show();
            }
            else
            {
                lblRightCoef.Hide();
                tbRightCoef.Hide();
                lblLeftCoef.Hide();
                tbLeftCoef.Hide();
                cbCapcomHack.Hide();
            }

        }
        private void lbFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbFiles.SelectedIndices.Count > 1)
            {
                this.tbLoopEnd.Text = String.Empty;
            }
        }
        private void cbLoopFileEnd_CheckedChanged(object sender, EventArgs e)
        {
            if (cbFindLoop.Checked || cbLoopFileEnd.Checked)
            {
                this.tbLoopEnd.Text = String.Empty;
                this.tbLoopEnd.ReadOnly = true;
            }
            else
            {
                this.tbLoopEnd.ReadOnly = false;
            }
            
        }
        private void cbFindLoop_CheckedChanged(object sender, EventArgs e)
        {
            if (cbFindLoop.Checked || cbLoopFileEnd.Checked)
            {
                this.tbLoopEnd.Text = String.Empty;
                this.tbLoopEnd.ReadOnly = true;

                this.tbLoopStart.Text = String.Empty;
                this.tbLoopStart.ReadOnly = true;

            }
            else
            {
                this.tbLoopEnd.ReadOnly = false;
                this.tbLoopStart.ReadOnly = false;
            }
        }
        private void cbNoLoops_CheckedChanged(object sender, EventArgs e)
        {
            if (cbNoLoops.Checked)
            {
                this.cbFindLoop.Checked = false;
                this.cbFindLoop.Enabled = false;
                this.cbLoopFileEnd.Checked = false;
                this.cbLoopFileEnd.Enabled = false;

                this.tbLoopStart.ReadOnly = true;
                this.tbLoopEnd.ReadOnly = true;
            }
            else
            {
                this.cbLoopFileEnd.Enabled = true;
                this.comboFormat_SelectedValueChanged(sender, e);

                this.tbLoopStart.ReadOnly = false;
                this.tbLoopEnd.ReadOnly = false;
            }
        }

        private void btnDoTask_Click(object sender, EventArgs e)
        {
            string errorMessages;

            this.initializeProcessing();
            DataRowView drv = (DataRowView)this.comboFormat.SelectedItem;

            GenhCreatorWorker.GenhCreatorStruct genhStruct = new GenhCreatorWorker.GenhCreatorStruct();            
            genhStruct.Format = Convert.ToUInt16(drv.Row.ItemArray[0]).ToString();
            genhStruct.HeaderSkip = this.tbHeaderSkip.Text;
            genhStruct.Interleave = this.tbInterleave.Text;
            genhStruct.Channels = this.tbChannels.Text;
            genhStruct.Frequency = this.tbFrequency.Text;
            genhStruct.LoopStart = this.tbLoopStart.Text;
            genhStruct.LoopEnd = this.tbLoopEnd.Text;
            genhStruct.UseFileEnd = this.cbLoopFileEnd.Checked;
            genhStruct.FindLoop = this.cbFindLoop.Checked;
            genhStruct.CoefRightChannel = this.tbRightCoef.Text;
            genhStruct.CoefLeftChannel = this.tbLeftCoef.Text;
            genhStruct.CapcomHack = this.cbCapcomHack.Checked;

            genhStruct.SourcePaths = new string[this.lbFiles.SelectedIndices.Count];

            int j = 0;
            foreach (int i in this.lbFiles.SelectedIndices)
            {
                genhStruct.SourcePaths[j++] = Path.Combine(this.tbSourceDirectory.Text, this.lbFiles.Items[i].ToString());
            }

            if (!ValidateInputs(genhStruct, out errorMessages))
            {
                MessageBox.Show(errorMessages, "Errors found in input");
                this.errorFound = true;
                this.setNodeAsComplete();
            }
            else
            {
                base.backgroundWorker_Execute(genhStruct);
            }
        }

        public static bool ValidateInputs(GenhCreatorWorker.GenhCreatorStruct pGenhCreatorStruct,
            out string pErrorMessages)
        {
            bool isValid = true;
            StringBuilder errorBuffer = new StringBuilder();

            // Paths
            if (pGenhCreatorStruct.SourcePaths.GetLength(0) < 1)
            {
                isValid = false;
                errorBuffer.Append("No Input Files are selected.");
                errorBuffer.Append(Environment.NewLine);
            }

            // Header Skip
            if (String.IsNullOrEmpty(pGenhCreatorStruct.HeaderSkip.Trim()))
            {
                isValid = false;
                errorBuffer.Append("'Header Skip' is a required field.");
                errorBuffer.Append(Environment.NewLine);
            }

            // Interleave
            if (String.IsNullOrEmpty(pGenhCreatorStruct.Interleave.Trim()))
            {
                isValid = false;
                errorBuffer.Append("'Interleave' is a required field.");
                errorBuffer.Append(Environment.NewLine);
            }

            // Channels
            if (String.IsNullOrEmpty(pGenhCreatorStruct.Channels.Trim()))
            {
                isValid = false;
                errorBuffer.Append("'Channels' is a required field.");
                errorBuffer.Append(Environment.NewLine);
            }

            // Frequency
            if (String.IsNullOrEmpty(pGenhCreatorStruct.Frequency.Trim()))
            {
                isValid = false;
                errorBuffer.Append("'Frequency' is a required field.");
                errorBuffer.Append(Environment.NewLine);
            }

            // Loop Start
            if (!pGenhCreatorStruct.NoLoops &&
                !pGenhCreatorStruct.FindLoop &&
                String.IsNullOrEmpty(pGenhCreatorStruct.LoopStart.Trim()))
            {
                isValid = false;
                errorBuffer.Append("'Loop Start' is a required field.");
                errorBuffer.Append(Environment.NewLine);
            }

            // Loop End
            if (!pGenhCreatorStruct.NoLoops && 
                !pGenhCreatorStruct.UseFileEnd && 
                !pGenhCreatorStruct.FindLoop &&
                String.IsNullOrEmpty(pGenhCreatorStruct.LoopEnd.Trim()))
            {
                isValid = false;
                errorBuffer.Append("'Loop End' is required if 'Use File End' or 'Find Loop' are unchecked.");
                errorBuffer.Append(Environment.NewLine);
            }

            // Right Coef
            if (pGenhCreatorStruct.Format.Equals("12") &&
                String.IsNullOrEmpty(pGenhCreatorStruct.CoefRightChannel.Trim()))
            {
                isValid = false;
                errorBuffer.Append("'Coef: Right Channel' is a required field for the selected format.");
                errorBuffer.Append(Environment.NewLine);
            }

            // Left Coef
            if (pGenhCreatorStruct.Format.Equals("12") &&
                String.IsNullOrEmpty(pGenhCreatorStruct.CoefLeftChannel.Trim()))
            {
                isValid = false;
                errorBuffer.Append("'Coef: Left Channel' is a required field for the selected format.");
                errorBuffer.Append(Environment.NewLine);
            }

            pErrorMessages = errorBuffer.ToString();
            return isValid;
        }

        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new GenhCreatorWorker();
        }
        protected override string getCancelMessage()
        {
            return "Create GENH(s)...Cancelled";
        }
        protected override string getCompleteMessage()
        {
            return "Create GENH(s)...Complete";
        }
        protected override string getBeginMessage()
        {
            return "Create GENH(s)...Begin";
        }        
    }
}
