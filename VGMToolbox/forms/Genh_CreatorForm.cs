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
        public const int NO_LABEL_SELECTED = -1;
        public const int LOOP_START_LABEL_SELECTED = 1;
        public const int LOOP_END_LABEL_SELECTED = 2;
        
        private int selectedLabel;

        private static readonly string DB_PATH =
            Path.Combine(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "db"), "collection.s3db");
                
        public Genh_CreatorForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            InitializeComponent();
            
            this.lblTitle.Text = ConfigurationSettings.AppSettings["Form_GenhCreator_Title"];            
            this.tbOutput.Text = ConfigurationSettings.AppSettings["Form_GenhCreator_IntroText"];
            
            //this.grpSourceFiles.Text = ConfigurationSettings.AppSettings["Form_GenhCreator_GroupSourceFiles"];
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
            this.cbManualEntry.Text = ConfigurationSettings.AppSettings["Form_GenhCreator_CheckBoxManualEntry"];
            this.cbNoLoops.Text = ConfigurationSettings.AppSettings["Form_GenhCreator_CheckBoxNoLoops"];
            this.cbLoopFileEnd.Text = ConfigurationSettings.AppSettings["Form_GenhCreator_CheckBoxFileEnd"];
            this.cbFindLoop.Text = ConfigurationSettings.AppSettings["Form_GenhCreator_CheckBoxFindLoop"];
            this.lblRightCoef.Text = ConfigurationSettings.AppSettings["Form_GenhCreator_LblRightCoef"];
            this.lblLeftCoef.Text = ConfigurationSettings.AppSettings["Form_GenhCreator_LblLeftCoef"];
            this.cbCapcomHack.Text = ConfigurationSettings.AppSettings["Form_GenhCreator_CheckBoxCapcomHack"];
            this.cbHeaderOnly.Text = ConfigurationSettings.AppSettings["Form_GenhCreator_CheckBoxHeaderOnly"];

            this.loadFormats();
            this.loadHeaderSkip();
            this.loadInterleave();
            this.loadChannels();
            this.loadFrequencies();

            rbCreate.Checked = true;
            this.updateFormForTask();

            // hide edit button
            rbEdit.Hide();

            // hide coefficients
            this.grpCoefOptions.Hide();

            // setup looping section
            this.doLoopCheckboxes();

            this.selectedLabel = NO_LABEL_SELECTED;
        }

        #region INITIALIZE FUNCTIONS
        
        private void loadFormats()
        {
            this.comboFormat.DataSource = SqlLiteUtil.GetSimpleDataTable(DB_PATH, "GenhFormats", "GenhFormatDescription");
            this.comboFormat.DisplayMember = "GenhFormatDescription";
            this.comboFormat.ValueMember = "GenhFormatId";
        }
        private void loadHeaderSkip()
        {
            cbHeaderSkip.Items.Add("0");

            for (int i = 4; i < 15; i++)
            {
                cbHeaderSkip.Items.Add(
                    String.Format("0x{0}", ((int)Math.Pow(2, i)).ToString("X2")));    
            }
        }
        private void loadInterleave()
        {
            cbHeaderSkip.Items.Add(String.Empty);

            for (int i = 4; i < 17; i++)
            {
                cbInterleave.Items.Add(
                    String.Format("0x{0}", ((int)Math.Pow(2, i)).ToString("X2")));
            }
        }
        private void loadChannels()
        {
            this.cbChannels.Items.Add(String.Empty);
            
            for (int i = 1; i < 9; i++)
            {
                this.cbChannels.Items.Add(i.ToString());

            }
        }
        private void loadFrequencies()
        {
            this.cbFrequency.Items.Add(String.Empty);
            this.cbFrequency.Items.Add("8000");
            this.cbFrequency.Items.Add("11025");
            this.cbFrequency.Items.Add("16000");
            this.cbFrequency.Items.Add("18900");
            this.cbFrequency.Items.Add("22050");
            this.cbFrequency.Items.Add("24000");
            this.cbFrequency.Items.Add("32000");
            this.cbFrequency.Items.Add("32075");
            this.cbFrequency.Items.Add("37800");
            this.cbFrequency.Items.Add("44100");
            this.cbFrequency.Items.Add("48000");

        }
        
        #endregion

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
                this.grpCoefOptions.Show();
                lblRightCoef.Show();
                tbRightCoef.Show();
                lblLeftCoef.Show();
                tbLeftCoef.Show();
                cbCapcomHack.Show();
            }
            else
            {
                this.grpCoefOptions.Hide();
                lblRightCoef.Hide();
                tbRightCoef.Hide();
                lblLeftCoef.Hide();
                tbLeftCoef.Hide();
                cbCapcomHack.Hide();
            }

            this.showLoopPointsForSelectedFile();

        }
        private void lbFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbFiles.SelectedIndices.Count > 1)
            {
                this.tbLoopEnd.Text = String.Empty;

                if (this.cbFindLoop.Checked)
                {
                    this.tbLoopStart.Text = String.Empty;
                }
            }
            
            this.showLoopPointsForSelectedFile();
        }

        #region LOOPING GUI

        private void doLoopCheckboxes()
        {
            if (cbManualEntry.Checked)
            {
                this.tbLoopStart.ReadOnly = false;
                this.cbUseLoopStartOffset.Enabled = true;

                this.tbLoopEnd.ReadOnly = false;
                this.cbUseLoopEndOffset.Enabled = true;
            }
            else if (cbNoLoops.Checked || cbFindLoop.Checked)
            {
                this.tbLoopStart.Clear();
                this.tbLoopStart.ReadOnly = true;
                this.cbUseLoopStartOffset.Checked = false;
                this.cbUseLoopStartOffset.Enabled = false;

                this.tbLoopEnd.Clear();
                this.tbLoopEnd.ReadOnly = true;
                this.cbUseLoopEndOffset.Checked = false;
                this.cbUseLoopEndOffset.Enabled = false;
            }
            else if (cbLoopFileEnd.Checked)
            {
                this.tbLoopStart.ReadOnly = false;
                this.cbUseLoopStartOffset.Enabled = true;

                this.tbLoopEnd.Clear();
                this.tbLoopEnd.ReadOnly = true;                
                this.cbUseLoopEndOffset.Checked = false;
                this.cbUseLoopEndOffset.Enabled = false;
            }

            this.doLoopStartCheckbox();
            this.doLoopEndCheckbox();
            this.showLoopPointsForSelectedFile();
        }

        private void doLoopStartCheckbox()
        {
            this.loopStartOffsetDescription.Enabled = cbUseLoopStartOffset.Checked;
            this.tbLoopStart.Enabled = !cbUseLoopStartOffset.Checked;

        }
        private void cbUseLoopStartOffset_CheckedChanged(object sender, EventArgs e)
        {
            this.doLoopStartCheckbox();
        }

        private void doLoopEndCheckbox()
        {
            this.loopEndOffsetDescription.Enabled = cbUseLoopEndOffset.Checked;
            this.tbLoopEnd.Enabled = !cbUseLoopEndOffset.Checked;

        }
        private void cbUseLoopEndOffset_CheckedChanged(object sender, EventArgs e)
        {
            this.doLoopEndCheckbox();
        }


        #endregion

        private GenhCreatorStruct getGenhParameters()
        {
            GenhCreatorStruct genhStruct = new GenhCreatorStruct();
            DataRowView drv = (DataRowView)this.comboFormat.SelectedItem;
            
            genhStruct.DoCreation = this.rbCreate.Checked;
            genhStruct.DoEdit = this.rbEdit.Checked;
            genhStruct.DoExtract = this.rbExtract.Checked;
            genhStruct.Format = Convert.ToUInt16(drv.Row.ItemArray[0]).ToString();
            genhStruct.HeaderSkip = this.cbHeaderSkip.Text;
            genhStruct.Interleave = this.cbInterleave.Text;
            genhStruct.Channels = this.cbChannels.Text;
            genhStruct.Frequency = this.cbFrequency.Text;
            genhStruct.LoopStart = this.tbLoopStart.Text;
            genhStruct.LoopEnd = this.tbLoopEnd.Text;
            genhStruct.NoLoops = this.cbNoLoops.Checked;
            genhStruct.UseFileEnd = this.cbLoopFileEnd.Checked;
            genhStruct.FindLoop = this.cbFindLoop.Checked;
            genhStruct.CoefRightChannel = this.tbRightCoef.Text;
            genhStruct.CoefLeftChannel = this.tbLeftCoef.Text;
            genhStruct.CapcomHack = this.cbCapcomHack.Checked;
            genhStruct.OutputHeaderOnly = this.cbHeaderOnly.Checked;

            genhStruct.SourcePaths = new string[this.lbFiles.SelectedIndices.Count];

            int j = 0;
            foreach (int i in this.lbFiles.SelectedIndices)
            {
                genhStruct.SourcePaths[j++] = Path.Combine(this.tbSourceDirectory.Text, this.lbFiles.Items[i].ToString());
            }

            return genhStruct;
        }

        private void btnDoTask_Click(object sender, EventArgs e)
        {
            string errorMessages;

            this.initializeProcessing();

            GenhCreatorStruct genhStruct = this.getGenhParameters();

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
                     VGMToolbox.util.ByteConversion.GetLongValueFromString(pGenhCreatorStruct.LoopStart.Trim()) < 0)
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

        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new GenhCreatorWorker();
        }
        protected override string getCancelMessage()
        {
            return ConfigurationSettings.AppSettings["Form_GenhCreator_MessageCancel"];
        }
        protected override string getCompleteMessage()
        {
            return ConfigurationSettings.AppSettings["Form_GenhCreator_MessageComplete"];
        }
        protected override string getBeginMessage()
        {
            return ConfigurationSettings.AppSettings["Form_GenhCreator_MessageBegin"];
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
        private void tbSourceDirectory_Click(object sender, EventArgs e)
        {
            if (tbSourceDirectory.Text.Equals(ConfigurationSettings.AppSettings["Form_GenhCreator_tbSourceDirectory"]))
            {
                tbSourceDirectory.Clear();
            }
        }

        private void cbManualEntry_CheckedChanged(object sender, EventArgs e)
        {
            this.doLoopCheckboxes();
        }
        private void cbNoLoops_CheckedChanged(object sender, EventArgs e)
        {
            this.doLoopCheckboxes();
        }
        private void cbLoopFileEnd_CheckedChanged(object sender, EventArgs e)
        {
            this.doLoopCheckboxes();
        }
        private void cbFindLoop_CheckedChanged(object sender, EventArgs e)
        {
            this.doLoopCheckboxes();
        }

        private void showLoopPointsForSelectedFile()
        {
            if (lbFiles.SelectedIndices.Count == 1)
            {
                string dummy;
                string loopStartFound = this.tbLoopStart.Text;
                string loopEndFound = this.tbLoopEnd.Text;
                GenhCreatorStruct genhStruct = this.getGenhParameters();

                if (ValidateInputs(genhStruct, out dummy))
                {
                    try
                    {
                        if (genhStruct.UseFileEnd)
                        {
                            loopEndFound = GenhUtil.GetFileEndLoopEnd(genhStruct.SourcePaths[0], genhStruct.ToGenhCreationStruct());
                        }
                        else if (genhStruct.FindLoop)
                        {
                            if ((!isPs2AdpcmSelected()) || 
                                (!GenhUtil.GetPsAdpcmLoop(genhStruct.SourcePaths[0], genhStruct.ToGenhCreationStruct(), out loopStartFound, out loopEndFound)))
                            {
                                loopStartFound = String.Empty;
                                loopEndFound = String.Empty;
                            }
                        }

                        this.tbLoopStart.Text = loopStartFound;
                        this.tbLoopEnd.Text = loopEndFound;
                    }
                    catch (Exception ex)
                    {
                        this.tbLoopStart.Clear();
                        this.tbLoopEnd.Clear();
                        this.tbOutput.Clear();
                        this.tbOutput.Text += String.Format("{0}{1}", ex.Message, Environment.NewLine);
                    }
                }
            }
        }

        private void cbHeaderSkip_SelectedValueChanged(object sender, EventArgs e)
        {
            this.showLoopPointsForSelectedFile();
        }
        private void cbInterleave_SelectedValueChanged(object sender, EventArgs e)
        {
            this.showLoopPointsForSelectedFile();
        }
        private void cbChannels_SelectedValueChanged(object sender, EventArgs e)
        {
            this.showLoopPointsForSelectedFile();
        }
        private void cbFrequency_SelectedValueChanged(object sender, EventArgs e)
        {
            this.showLoopPointsForSelectedFile();
        }

        #region BYTES TO SAMPLES

        private bool isPs2AdpcmSelected()
        {
            bool ret = false;

            DataRowView drv = (DataRowView)this.comboFormat.SelectedItem;
            UInt16 formatId = Convert.ToUInt16(drv.Row.ItemArray[0]);

            if (formatId == 0 || formatId == 0xE)
            {
                ret = true;
            }

            return ret;
        }
        private void lblLoopStart_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Point where the mouse is clicked.
                Point p = new Point(e.X, e.Y);

                if (tbLoopStart.Enabled && !tbLoopStart.ReadOnly &&
                    !String.IsNullOrEmpty(this.tbLoopStart.Text) && isPs2AdpcmSelected())
                {
                    this.selectedLabel = LOOP_START_LABEL_SELECTED;
                    
                    // show menu
                    contextMenuBytesToSamples.Show(lblLoopStart, p);
                }
            }
        }
        private void lblLoopEnd_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Point where the mouse is clicked.
                Point p = new Point(e.X, e.Y);

                if (tbLoopEnd.Enabled && !tbLoopEnd.ReadOnly &&
                    !String.IsNullOrEmpty(this.tbLoopEnd.Text) && isPs2AdpcmSelected())
                {
                    this.selectedLabel = LOOP_END_LABEL_SELECTED;
                    
                    // show menu
                    contextMenuBytesToSamples.Show(lblLoopEnd, p);
                }
            }
        }
        private void bytesToSamplesToolStripMenuItem_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                try
                {
                    long loopValue = 0;
                    long headerValue = 0;

                    if (this.selectedLabel == LOOP_START_LABEL_SELECTED)
                    {
                        loopValue = ByteConversion.GetLongValueFromString(this.tbLoopStart.Text);
                    }
                    else if (this.selectedLabel == LOOP_END_LABEL_SELECTED)
                    {
                        loopValue = ByteConversion.GetLongValueFromString(this.tbLoopEnd.Text);
                    }

                    if (!String.IsNullOrEmpty(this.cbHeaderSkip.Text))
                    {
                        headerValue = ByteConversion.GetLongValueFromString(this.cbHeaderSkip.Text);
                        loopValue -= headerValue;
                    }


                    if (loopValue > 0)
                    {
                        long channelCount = ByteConversion.GetLongValueFromString(this.cbChannels.Text);
                        long samplesValue = (loopValue / 16 / channelCount * 28);

                        if (this.selectedLabel == LOOP_START_LABEL_SELECTED)
                        {
                            this.tbLoopStart.Text = samplesValue.ToString();
                        }
                        else if (this.selectedLabel == LOOP_END_LABEL_SELECTED)
                        {
                            this.tbLoopEnd.Text = samplesValue.ToString();
                        }

                    }
                }
                catch
                { 
                
                }
            }
        }

        #endregion
    }
}
