using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.examine;

namespace VGMToolbox.forms.examine
{
    public partial class CrcCalculatorForm : AVgmtForm
    {
        public CrcCalculatorForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            InitializeComponent();

            this.grpSourceFiles.AllowDrop = true;
            
            this.lblTitle.Text = ConfigurationSettings.AppSettings["Form_ChecksumCalculator_Title"];
            this.tbOutput.Text = ConfigurationSettings.AppSettings["Form_ChecksumCalculator_IntroText"];

            this.grpSourceFiles.Text = ConfigurationSettings.AppSettings["Form_ChecksumCalculator_GroupSourceFiles"];
            this.grpOptions.Text = ConfigurationSettings.AppSettings["Form_ChecksumCalculator_GroupOptions"];
            this.cbDoVgmtChecksums.Text = ConfigurationSettings.AppSettings["Form_ChecksumCalculator_CheckBoxDoVgmtChecksums"];
            this.checkForDuplicatesFlag.Text = ConfigurationSettings.AppSettings["Form_ChecksumCalculator_CheckBoxCheckForDuplicates"];

            this.btnDoTask.Hide();
        }

        private void grpSourceFiles_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            ExamineChecksumGeneratorWorker.ExamineChecksumGeneratorStruct crcStruct =
                new ExamineChecksumGeneratorWorker.ExamineChecksumGeneratorStruct();
            crcStruct.SourcePaths = s;
            crcStruct.DoVgmtChecksums = cbDoVgmtChecksums.Checked;
            crcStruct.CheckForDuplicates = checkForDuplicatesFlag.Checked;

            base.backgroundWorker_Execute(crcStruct);
        }
        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }

        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new ExamineChecksumGeneratorWorker();
        }
        protected override string getCancelMessage()
        {
            return ConfigurationSettings.AppSettings["Form_ChecksumCalculator_MessageCancel"];
        }
        protected override string getCompleteMessage()
        {
            return ConfigurationSettings.AppSettings["Form_ChecksumCalculator_MessageComplete"];
        }
        protected override string getBeginMessage()
        {
            return ConfigurationSettings.AppSettings["Form_ChecksumCalculator_MessageBegin"];
        }
    }
}
