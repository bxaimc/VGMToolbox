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

            // messages
            this.BackgroundWorker = new ExamineChecksumGeneratorWorker();
            this.BeginMessage = ConfigurationSettings.AppSettings["Form_ChecksumCalculator_MessageBegin"];
            this.CompleteMessage = ConfigurationSettings.AppSettings["Form_ChecksumCalculator_MessageComplete"];
            this.CancelMessage = ConfigurationSettings.AppSettings["Form_ChecksumCalculator_MessageCancel"];

            this.grpSourceFiles.AllowDrop = true;
            
            this.lblTitle.Text = ConfigurationSettings.AppSettings["Form_ChecksumCalculator_Title"];
            this.tbOutput.Text = ConfigurationSettings.AppSettings["Form_ChecksumCalculator_IntroText"];

            this.grpSourceFiles.Text = ConfigurationSettings.AppSettings["Form_ChecksumCalculator_GroupSourceFiles"];
            this.grpOptions.Text = ConfigurationSettings.AppSettings["Form_ChecksumCalculator_GroupOptions"];
            this.cbDoVgmtChecksums.Text = ConfigurationSettings.AppSettings["Form_ChecksumCalculator_CheckBoxDoVgmtChecksums"];

            this.btnDoTask.Hide();
        }

        private void grpSourceFiles_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            ExamineChecksumGeneratorWorker.ExamineChecksumGeneratorStruct crcStruct =
                new ExamineChecksumGeneratorWorker.ExamineChecksumGeneratorStruct();
            crcStruct.SourcePaths = s;
            crcStruct.DoVgmtChecksums = cbDoVgmtChecksums.Checked;

            base.backgroundWorker_Execute(crcStruct);
        }
        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }
    }
}
