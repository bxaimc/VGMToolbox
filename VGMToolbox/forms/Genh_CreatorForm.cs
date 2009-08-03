using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.dbutil;
using VGMToolbox.format.util;
using VGMToolbox.plugin;
using VGMToolbox.tools.genh;
using VGMToolbox.util;

namespace VGMToolbox.forms
{
    public partial class Genh_CreatorForm : AVgmtForm
    {
        private static readonly string DB_PATH =
            Path.Combine(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "db"), "collection.s3db");
                
        public Genh_CreatorForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            InitializeComponent();

            // messages
            this.BackgroundWorker = new GenhCreatorWorker();
            this.BeginMessage = ConfigurationSettings.AppSettings["Form_GenhCreator_MessageBegin"];
            this.CompleteMessage = ConfigurationSettings.AppSettings["Form_GenhCreator_MessageComplete"];
            this.CancelMessage = ConfigurationSettings.AppSettings["Form_GenhCreator_MessageCancel"];

            this.lblTitle.Text = ConfigurationSettings.AppSettings["Form_GenhCreator_Title"];            
            this.tbOutput.Text = ConfigurationSettings.AppSettings["Form_GenhCreator_IntroText"];
            
            this.grpSourceFiles.Text = ConfigurationSettings.AppSettings["Form_GenhCreator_GroupSourceFiles"];
            this.tbSourceDirectory.Text = ConfigurationSettings.AppSettings["Form_GenhCreator_tbSourceDirectory"];
            this.lblFilenameFilter.Text = ConfigurationSettings.AppSettings["Form_GenhCreator_LblFilenameFilter"];
            this.grpFormat.Text = ConfigurationSettings.AppSettings["Form_GenhCreator_GroupFormat"];
            this.grpOptions.Text = ConfigurationSettings.AppSettings["Form_GenhCreator_GroupOptions"];
            this.lblHeaderSkip.Text = ConfigurationSettings.AppSettings["Form_GenhCreator_LblHeaderSkip"];
            this.lblInterleave.Text = ConfigurationSettings.AppSettings["Form_GenhCreator_LblInterleave"];
            this.lblChannels.Text = ConfigurationSettings.AppSettings["Form_GenhCreator_LblChannels"];
            this.lblFrequency.Text = ConfigurationSettings.AppSettings["Form_GenhCreator_LblFrequency"];
            this.lblLoopStart.Text = ConfigurationSettings.AppSettings["Form_GenhCreator_LblLoopStart"];
            this.lblLoopEnd.Text = ConfigurationSettings.AppSettings["Form_GenhCreator_LblLoopEnd"];
            this.cbNoLoops.Text = ConfigurationSettings.AppSettings["Form_GenhCreator_CheckBoxNoLoops"];
            this.cbLoopFileEnd.Text = ConfigurationSettings.AppSettings["Form_GenhCreator_CheckBoxFileEnd"];
            this.cbFindLoop.Text = ConfigurationSettings.AppSettings["Form_GenhCreator_CheckBoxFindLoop"];
            this.lblRightCoef.Text = ConfigurationSettings.AppSettings["Form_GenhCreator_LblRightCoef"];
            this.lblLeftCoef.Text = ConfigurationSettings.AppSettings["Form_GenhCreator_LblLeftCoef"];
            this.cbCapcomHack.Text = ConfigurationSettings.AppSettings["Form_GenhCreator_CheckBoxCapcomHack"];
            this.cbHeaderOnly.Text = ConfigurationSettings.AppSettings["Form_GenhCreator_CheckBoxHeaderOnly"];

            this.loadFormats();

            rbCreate.Checked = true;
            this.updateFormForTask();

            rbEdit.Hide();
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
            this.reloadFiles();
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

                this.tbLoopStart.Text = String.Empty;
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

            GenhCreatorStruct genhStruct = new GenhCreatorStruct();
            genhStruct.DoCreation = this.rbCreate.Checked;
            genhStruct.DoEdit = this.rbEdit.Checked;
            genhStruct.DoExtract = this.rbExtract.Checked;
            genhStruct.Format = Convert.ToUInt16(drv.Row.ItemArray[0]).ToString();
            genhStruct.HeaderSkip = this.tbHeaderSkip.Text;
            genhStruct.Interleave = this.tbInterleave.Text;
            genhStruct.Channels = this.tbChannels.Text;
            genhStruct.Frequency = this.tbFrequency.Text;
            genhStruct.LoopStart = this.tbLoopStart.Text;
            genhStruct.LoopEnd = this.tbLoopEnd.Text;
            genhStruct.NoLoops = this.cbNoLoops.Checked;
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

            if ((!rbExtract.Checked) && (!ValidateInputs(genhStruct, out errorMessages)))
            {
                MessageBox.Show(errorMessages, ConfigurationSettings.AppSettings["Form_GenhCreator_MessageErrors"]);
                this.errorFound = true;
                this.setNodeAsComplete();
            }
            else
            {
                base.backgroundWorker_Execute(genhStruct);
            }
        }

        public static bool ValidateInputs(GenhCreatorStruct pGenhCreatorStruct,
            out string pErrorMessages)
        {
            bool isValid = true;
            StringBuilder errorBuffer = new StringBuilder();

            // Paths
            if (pGenhCreatorStruct.SourcePaths.GetLength(0) < 1)
            {
                isValid = false;
                errorBuffer.Append(ConfigurationSettings.AppSettings["Form_GenhCreator_MessageNoInputs"]);
                errorBuffer.Append(Environment.NewLine);
            }

            // Header Skip
            if (String.IsNullOrEmpty(pGenhCreatorStruct.HeaderSkip.Trim()))
            {
                isValid = false;
                errorBuffer.Append(ConfigurationSettings.AppSettings["Form_GenhCreator_MessageHeaderSkip"]);
                errorBuffer.Append(Environment.NewLine);
            }

            // Interleave
            if (String.IsNullOrEmpty(pGenhCreatorStruct.Interleave.Trim()))
            {
                isValid = false;
                errorBuffer.Append(ConfigurationSettings.AppSettings["Form_GenhCreator_MessageInterleave"]);
                errorBuffer.Append(Environment.NewLine);
            }

            // Channels
            if (String.IsNullOrEmpty(pGenhCreatorStruct.Channels.Trim()))
            {
                isValid = false;
                errorBuffer.Append(ConfigurationSettings.AppSettings["Form_GenhCreator_MessageChannels"]);
                errorBuffer.Append(Environment.NewLine);
            }

            // Frequency
            if (String.IsNullOrEmpty(pGenhCreatorStruct.Frequency.Trim()))
            {
                isValid = false;
                errorBuffer.Append(ConfigurationSettings.AppSettings["Form_GenhCreator_MessageFrequency"]);
                errorBuffer.Append(Environment.NewLine);
            }

            // Loop Start
            if (!pGenhCreatorStruct.NoLoops &&
                !pGenhCreatorStruct.FindLoop &&
                String.IsNullOrEmpty(pGenhCreatorStruct.LoopStart.Trim()))
            {
                isValid = false;
                errorBuffer.Append(ConfigurationSettings.AppSettings["Form_GenhCreator_MessageLoopStart1"]);
                errorBuffer.Append(Environment.NewLine);
            }
            else if (!pGenhCreatorStruct.NoLoops &&
                     !pGenhCreatorStruct.FindLoop &&
                     VGMToolbox.util.Encoding.GetLongValueFromString(pGenhCreatorStruct.LoopStart.Trim()) < 0)
            {
                isValid = false;
                errorBuffer.Append(ConfigurationSettings.AppSettings["Form_GenhCreator_MessageLoopStart2"]);
                errorBuffer.Append(Environment.NewLine);            
            }

            // Loop End
            if (!pGenhCreatorStruct.NoLoops && 
                !pGenhCreatorStruct.UseFileEnd && 
                !pGenhCreatorStruct.FindLoop &&
                String.IsNullOrEmpty(pGenhCreatorStruct.LoopEnd.Trim()))
            {
                isValid = false;
                errorBuffer.Append(ConfigurationSettings.AppSettings["Form_GenhCreator_MessageLoopEnd"]);
                errorBuffer.Append(Environment.NewLine);
            }

            // Right Coef
            if (pGenhCreatorStruct.Format.Equals("12") &&
                String.IsNullOrEmpty(pGenhCreatorStruct.CoefRightChannel.Trim()))
            {
                isValid = false;
                errorBuffer.Append(ConfigurationSettings.AppSettings["Form_GenhCreator_MessageCoefRight"]);
                errorBuffer.Append(Environment.NewLine);
            }

            // Left Coef
            if (pGenhCreatorStruct.Format.Equals("12") &&
                String.IsNullOrEmpty(pGenhCreatorStruct.CoefLeftChannel.Trim()))
            {
                isValid = false;
                errorBuffer.Append(ConfigurationSettings.AppSettings["Form_GenhCreator_MessageCoefLeft"]);
                errorBuffer.Append(Environment.NewLine);
            }

            // Find Loop End
            if (pGenhCreatorStruct.UseFileEnd && pGenhCreatorStruct.Format.Equals("8"))
            {
                isValid = false;
                errorBuffer.Append(ConfigurationSettings.AppSettings["Form_GenhCreator_MessageFileEnd"]);
                errorBuffer.Append(Environment.NewLine);            
            }

            pErrorMessages = errorBuffer.ToString();
            return isValid;
        }

        private void reloadFiles()
        {            
            string fileName;

            this.lbFiles.Items.Clear();

            if (Directory.Exists(this.tbSourceDirectory.Text))
            {
                string[] fileList = Directory.GetFiles(this.tbSourceDirectory.Text);
                Array.Sort(fileList);

                foreach (string f in fileList)
                {                    
                    if (rbCreate.Checked)
                    {
                        if (!f.ToUpper().EndsWith("GENH") &&
                            !f.ToUpper().EndsWith("EXE"))
                        {
                            if (!String.IsNullOrEmpty(this.tbFilenameFilter.Text))
                            {
                                fileName = Path.GetFileName(f);
                                
                                if (fileName.ToUpper().Contains(this.tbFilenameFilter.Text.ToUpper()))
                                {
                                    this.lbFiles.Items.Add(Path.GetFileName(f));
                                }
                            }
                            else
                            {
                                this.lbFiles.Items.Add(Path.GetFileName(f));
                            }
                        }
                    }
                    else
                    {
                        if (f.ToUpper().EndsWith("GENH"))
                        {
                            this.lbFiles.Items.Add(Path.GetFileName(f));
                        }
                    }
                }
            }
            else
            {
                this.lbFiles.Items.Clear();
            }        
        }
        private void updateFormForTask()
        {
            if (rbCreate.Checked)
            {
                this.btnDoTask.Text = ConfigurationSettings.AppSettings["Form_GenhCreator_BtnDoTaskCreate"];
                grpOptions.Show();
                grpFormat.Show();
                cbHeaderOnly.Show();
            }
            else if (rbEdit.Checked)
            {
                this.btnDoTask.Text = ConfigurationSettings.AppSettings["Form_GenhCreator_BtnDoTaskEdit"];
                grpOptions.Show();
                grpFormat.Show();
                cbHeaderOnly.Show();
            }
            else if (rbExtract.Checked)
            {
                this.btnDoTask.Text = ConfigurationSettings.AppSettings["Form_GenhCreator_BtnDoTaskExtract"];
                grpOptions.Hide();
                grpFormat.Hide();
                cbHeaderOnly.Hide();
            }
        }

        private void rbCreate_CheckedChanged(object sender, EventArgs e)
        {
            this.reloadFiles();
            this.updateFormForTask();
        }
        private void rbEdit_CheckedChanged(object sender, EventArgs e)
        {
            this.reloadFiles();
            this.updateFormForTask();
        }
        private void rbExtract_CheckedChanged(object sender, EventArgs e)
        {
            this.reloadFiles();
            this.updateFormForTask();
        }

        private void lbFiles_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Point where the mouse is clicked.
                Point p = new Point(e.X, e.Y);

                // show menu
                contextMenuRefresh.Show(lbFiles, p);
            }
        }
        private void refreshFileListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.reloadFiles();
        }

        private void tbFilenameFilter_TextChanged(object sender, EventArgs e)
        {
            this.reloadFiles();
        }

    }
}
