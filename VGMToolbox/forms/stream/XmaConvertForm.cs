using System;
using System.IO;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.stream;
using VGMToolbox.util;

namespace VGMToolbox.forms.stream
{
    public partial class XmaConvertForm : AVgmtForm
    {
        public XmaConvertForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = "XMA to WAV Converter";
            
            // hide the DoTask button since this is a drag and drop form
            this.btnDoTask.Hide();

            InitializeComponent();

            this.tbOutput.Text = String.Format("Convert XMA to WAV using external tools{0}{0}", Environment.NewLine);
            this.tbOutput.Text += String.Format("* Note: This tool requires 'ToWav.exe' by Xplorer and 'xma_test.exe' (xma_parse) by hcs.{0}", Environment.NewLine);
            this.tbOutput.Text += String.Format("  Please download both files and place in the following directory: <{0}>{1}", Path.GetDirectoryName(XmaConverterWorker.TOWAV_FULL_PATH), Environment.NewLine);

            this.initializeXmaParseInputSection();
            this.initializeRiffSection();
            this.initializeToWavSection();
        }

        //----------------
        // initialization
        //----------------
        private void initializeXmaParseInputSection()
        {
            this.cbDoXmaParse.Checked = true;
            
            // XMA Type
            this.comboXmaParseInputType.Items.Add("1");
            this.comboXmaParseInputType.Items.Add("2");
            
            this.comboXmaParseInputType.SelectedItem = "2";

            // Defaults
            this.tbXmaParseStartOffset.Text = "0x00";
            this.tbXmaParseBlockSize.Text = "0x8000";
            this.cbXmaParseIgnoreErrors.Checked = true;

            // radio buttons
            this.rbXmaParseStartOffsetStatic.Checked = true;
            this.rbXmaParseBlockSizeStatic.Checked = true;
            this.rbXmaParseDataSizeStatic.Checked = true;

            this.doXmaParseStartOffsetRadios();
            this.doXmaParseBlockSizeRadios();
            this.doXmaParseDataSizeRadios();
        }

        private void initializeRiffSection()
        {
            this.cbAddRiffHeader.Checked = true;
            
            // RIFF Frequency
            this.comboRiffFrequency.Items.Add("22050");
            this.comboRiffFrequency.Items.Add("24000");
            this.comboRiffFrequency.Items.Add("32000");
            this.comboRiffFrequency.Items.Add("44100");
            this.comboRiffFrequency.Items.Add("48000");

            this.comboRiffFrequency.SelectedItem = "48000";

            // RIFF Channels
            this.comboRiffChannels.Items.Add(XmaConverterWorker.RIFF_CHANNELS_1);
            this.comboRiffChannels.Items.Add(XmaConverterWorker.RIFF_CHANNELS_2);

            this.comboRiffChannels.SelectedItem = XmaConverterWorker.RIFF_CHANNELS_2;
        }

        private void initializeToWavSection()
        {
            this.cbDoToWav.Checked = true;
        }
                  
        //-----------------------------
        // drag and drop functionality
        //-----------------------------
        private void tbOutputFolder_DragEnter(object sender, DragEventArgs e)
        {
            this.doDragEnter(sender, e);
        }

        private void tbOutputFolder_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            if ((s.Length == 1) && (Directory.Exists(s[0])))
            {
                this.tbOutputFolder.Text = s[0];
            }
        }        

        private void XmaConvertForm_DragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }

        private void XmaConvertForm_DragDrop(object sender, DragEventArgs e)
        {            
            // validate inputs and check for prerequisites
            if (this.validateAll())
            {                
                XmaConverterWorker.XmaConverterStruct taskStruct = new XmaConverterWorker.XmaConverterStruct();
                
                // paths
                string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                taskStruct.SourcePaths = s;

                // OUTPUT
                if (!String.IsNullOrEmpty(this.tbOutputFolder.Text))
                {
                    taskStruct.OutputFolder = this.tbOutputFolder.Text.Trim();
                }

                // XMA PARSE
                taskStruct.DoXmaParse = this.cbDoXmaParse.Checked;
                taskStruct.XmaParseDoRebuildMode = this.cbXmaParseDoRebuild.Checked;
                taskStruct.XmaParseIgnoreErrors = this.cbXmaParseIgnoreErrors.Checked;
                taskStruct.XmaParseXmaType = this.comboXmaParseInputType.Text;

                taskStruct.XmaParseStartOffsetIsStatic = this.rbXmaParseStartOffsetStatic.Checked;
                taskStruct.XmaParseStartOffset = this.tbXmaParseStartOffset.Text;
                taskStruct.XmaParseStartOffsetOffsetInfo = this.XmaParseStartOffsetOffsetDescription.GetOffsetValues();

                taskStruct.XmaParseBlockSizeIsStatic = this.rbXmaParseBlockSizeStatic.Checked;
                taskStruct.XmaParseBlockSize = this.tbXmaParseBlockSize.Text;
                taskStruct.XmaParseBlockSizeOffsetInfo = this.XmaParseBlockSizeOffsetDescription.GetOffsetValues();

                taskStruct.XmaParseDataSizeIsStatic = this.rbXmaParseDataSizeStatic.Checked;
                taskStruct.XmaParseDataSize = this.tbXmaParseDataSize.Text;
                taskStruct.XmaParseDataSizeOffsetInfo = this.XmaParseDataSizeOffsetDescription.GetOffsetValues();

                // RIFF
                taskStruct.DoRiffHeader = this.cbAddRiffHeader.Checked;
                taskStruct.RiffFrequency = this.comboRiffFrequency.Text;

                switch (this.comboRiffChannels.Text)
                {
                    case XmaConverterWorker.RIFF_CHANNELS_1:
                        taskStruct.RiffChannelCount = "1";
                        break;
                    case XmaConverterWorker.RIFF_CHANNELS_2:
                        taskStruct.RiffChannelCount = "2";
                        break;
                    default:
                        taskStruct.RiffChannelCount = "2";
                        break;
                }

                // TOWAV
                taskStruct.DoToWav = this.cbDoToWav.Checked;

                // OTHER                
                taskStruct.ShowExeOutput = this.cbShowAllExeOutput.Checked;
                taskStruct.KeepIntermediateFiles = this.cbKeepTempFiles.Checked;

                base.backgroundWorker_Execute(taskStruct);
            }
        }

        //------------
        // validation
        //------------
        private bool validateAll()
        {
            bool isValid = true;

            isValid &= this.checkPrerequisites();
            isValid &= this.validateOutputOptions();
            isValid &= this.validateXmaParseOptions();
            
            return isValid;
        }

        private bool checkPrerequisites()
        {
            bool isValid = true;
            string errorMessage;

            if (!File.Exists(XmaConverterWorker.TOWAV_FULL_PATH))
            {
                errorMessage = 
                    String.Format(
                        "Cannot find \"{0}\".  \"{0}\" is not included and should be placed in the following directory: <{1}>",
                        Path.GetFileName(XmaConverterWorker.TOWAV_FULL_PATH),
                        Path.GetDirectoryName(XmaConverterWorker.TOWAV_FULL_PATH));

                MessageBox.Show(errorMessage, "Missing File");

                isValid = false;
            }
            
            if (!File.Exists(XmaConverterWorker.XMAPARSE_FULL_PATH))
            {
                errorMessage =
                    String.Format(
                        "Cannot find \"{0}\".  \"{0}\" is not included and should be placed in the following directory: <{1}>",
                        Path.GetFileName(XmaConverterWorker.XMAPARSE_FULL_PATH),
                        Path.GetDirectoryName(XmaConverterWorker.XMAPARSE_FULL_PATH));

                MessageBox.Show(errorMessage, "Missing File");

                isValid = false;
            }
                        
            return isValid;
        }

        private bool validateOutputOptions()
        {
            bool isValid = true;

            if (!String.IsNullOrEmpty(this.tbOutputFolder.Text))
            {
                isValid &= base.checkFolderExists(this.tbOutputFolder.Text, this.lblOutputFolder.Text);
            }

            return isValid;
        }

        private bool validateXmaParseOptions()
        {
            bool isValid = true;

            isValid &= base.checkTextBox(this.comboXmaParseInputType.Text, this.lblXmaParseInputType.Text);

            if (this.rbXmaParseStartOffsetStatic.Checked)
            {
                isValid &= base.checkIfTextIsParsableAsLong(this.tbXmaParseStartOffset.Text, this.rbXmaParseStartOffsetStatic.Text);
            }
            
            if (this.rbXmaParseBlockSizeStatic.Checked)
            {
                isValid &= base.checkIfTextIsParsableAsLong(this.tbXmaParseBlockSize.Text, this.rbXmaParseBlockSizeStatic.Text);
            }

            if (this.rbXmaParseDataSizeStatic.Checked)
            {
                isValid &= base.checkIfTextIsParsableAsLong(this.tbXmaParseDataSize.Text, this.rbXmaParseDataSizeStatic.Text);
            }

            if (this.rbXmaParseStartOffsetOffset.Checked)
            {
                isValid &= base.checkTextBox(this.XmaParseStartOffsetOffsetDescription.OffsetValue, this.rbXmaParseStartOffsetOffset.Text);
                isValid &= base.checkIfTextIsParsableAsLong(this.XmaParseStartOffsetOffsetDescription.OffsetValue, this.rbXmaParseStartOffsetOffset.Text);
            }

            if (this.rbXmaParseBlockSizeOffset.Checked)
            {
                isValid &= base.checkTextBox(this.XmaParseBlockSizeOffsetDescription.OffsetValue, this.rbXmaParseBlockSizeOffset.Text);
                isValid &= base.checkIfTextIsParsableAsLong(this.XmaParseBlockSizeOffsetDescription.OffsetValue, this.rbXmaParseBlockSizeOffset.Text);
            }

            if (this.rbXmaParseDataSizeOffset.Checked)
            {
                isValid &= base.checkTextBox(this.XmaParseDataSizeOffsetDescription.OffsetValue, this.rbXmaParseDataSizeOffset.Text);
                isValid &= base.checkIfTextIsParsableAsLong(this.XmaParseDataSizeOffsetDescription.OffsetValue, this.rbXmaParseDataSizeOffset.Text);
            }

            return isValid;
        }

        //----------------------------
        // background worker defaults
        //----------------------------
        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new XmaConverterWorker();
        }
        protected override string getCancelMessage()
        {
            return "Converting XMA...Cancelled";
        }
        protected override string getCompleteMessage()
        {
            return "Converting XMA...Complete";
        }
        protected override string getBeginMessage()
        {
            return "Converting XMA...Begin";
        }

        //--------------------
        // combo box handlers
        //--------------------
        private void comboXmaParseInputType_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
        private void comboXmaParseInputType_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void comboRiffFrequency_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
        private void comboRiffFrequency_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void comboRiffChannels_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
        private void comboRiffChannels_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        //---------------
        // output folder
        //---------------
        private void btnBrowseOutputFolder_Click(object sender, EventArgs e)
        {
            this.tbOutputFolder.Text = base.browseForFolder(sender, e);
        }

        //--------------------------
        // checkboxes/radio buttons
        //--------------------------
        private void cbDoXmaParse_CheckedChanged(object sender, EventArgs e)
        {
            this.comboXmaParseInputType.Enabled = this.cbDoXmaParse.Checked;
            this.cbXmaParseDoRebuild.Enabled = this.cbDoXmaParse.Checked;
            this.cbXmaParseIgnoreErrors.Enabled = this.cbDoXmaParse.Checked;

            this.rbXmaParseStartOffsetStatic.Enabled = this.cbDoXmaParse.Checked;
            this.rbXmaParseStartOffsetOffset.Enabled = this.cbDoXmaParse.Checked;

            this.rbXmaParseBlockSizeStatic.Enabled = this.cbDoXmaParse.Checked;
            this.rbXmaParseBlockSizeOffset.Enabled = this.cbDoXmaParse.Checked;

            this.rbXmaParseDataSizeStatic.Enabled = this.cbDoXmaParse.Checked;
            this.rbXmaParseDataSizeOffset.Enabled = this.cbDoXmaParse.Checked;

            this.doXmaParseStartOffsetRadios();
            this.doXmaParseBlockSizeRadios();
            this.doXmaParseDataSizeRadios();
        }

        private void cbAddRiffHeader_CheckedChanged(object sender, EventArgs e)
        {
            this.comboRiffFrequency.Enabled = this.cbAddRiffHeader.Checked;
            this.comboRiffChannels.Enabled = this.cbAddRiffHeader.Checked;
        }

        // xma_parse - start offset
        private void doXmaParseStartOffsetRadios()
        {
            this.tbXmaParseStartOffset.Enabled = this.rbXmaParseStartOffsetStatic.Checked && this.rbXmaParseStartOffsetStatic.Enabled;
            this.XmaParseStartOffsetOffsetDescription.Enabled = this.rbXmaParseStartOffsetOffset.Checked && this.rbXmaParseStartOffsetOffset.Enabled;
        }
        private void rbXmaParseStartOffsetOffset_CheckedChanged(object sender, EventArgs e)
        {
            this.doXmaParseStartOffsetRadios();
        }
        private void rbXmaParseStartOffsetStatic_CheckedChanged(object sender, EventArgs e)
        {
            this.doXmaParseStartOffsetRadios();
        }

        // xma parse - block size
        private void doXmaParseBlockSizeRadios()
        {
            this.tbXmaParseBlockSize.Enabled = this.rbXmaParseBlockSizeStatic.Checked && this.rbXmaParseBlockSizeStatic.Enabled;
            this.XmaParseBlockSizeOffsetDescription.Enabled = this.rbXmaParseBlockSizeOffset.Checked && this.rbXmaParseBlockSizeOffset.Enabled;
        }
        private void rbXmaParseBlockSizeOffset_CheckedChanged(object sender, EventArgs e)
        {
            this.doXmaParseBlockSizeRadios();
        }
        private void rbXmaParseBlockSizeStatic_CheckedChanged(object sender, EventArgs e)
        {
            this.doXmaParseBlockSizeRadios();
        }

        // xma parse - data size
        private void doXmaParseDataSizeRadios()
        {
            this.tbXmaParseDataSize.Enabled = this.rbXmaParseDataSizeStatic.Checked && this.rbXmaParseDataSizeStatic.Enabled;
            this.XmaParseDataSizeOffsetDescription.Enabled = this.rbXmaParseDataSizeOffset.Checked && this.rbXmaParseDataSizeOffset.Enabled;
        }
        private void rbXmaParseDataSizeOffset_CheckedChanged(object sender, EventArgs e)
        {
            this.doXmaParseDataSizeRadios();
        }
        private void rbXmaParseDataSizeStatic_CheckedChanged(object sender, EventArgs e)
        {
            this.doXmaParseDataSizeRadios();
        }     
    }
}
