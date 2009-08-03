using System;
using System.ComponentModel;
using System.Configuration;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.xsf;

namespace VGMToolbox.forms.xsf
{
    public partial class PsfTimerForm : AVgmtForm
    {        
        public PsfTimerForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = ConfigurationSettings.AppSettings["Form_SeqExtractor_Title"];
            this.tbOutput.Text = ConfigurationSettings.AppSettings["Form_SeqExtractor_IntroText"];
            this.btnDoTask.Hide();
            
            InitializeComponent();

            // messages
            this.BackgroundWorker = new PsxSeqExtractWorker();
            this.BeginMessage = ConfigurationSettings.AppSettings["Form_SeqExtractor_MessageBegin"];
            this.CompleteMessage = ConfigurationSettings.AppSettings["Form_SeqExtractor_MessageComplete"];
            this.CancelMessage = ConfigurationSettings.AppSettings["Form_SeqExtractor_MessageCancel"];

            this.grpSource.Text =
                ConfigurationSettings.AppSettings["Form_SeqExtractor_GroupSource"];
            this.lblDragNDrop.Text =
                ConfigurationSettings.AppSettings["Form_SeqExtractor_LblDragNDrop"];            
            this.grpOptions.Text =
                ConfigurationSettings.AppSettings["Form_SeqExtractor_GroupOptions"];
            this.cbForce2Loops.Text =
                ConfigurationSettings.AppSettings["Form_SeqExtractor_CheckBoxForce2Loops"];
            this.cbForceType.Text =
                ConfigurationSettings.AppSettings["Form_SeqExtractor_CheckBoxForceType"];
            this.rbForceSepType.Text =
                ConfigurationSettings.AppSettings["Form_SeqExtractor_RadioForceSepType"];
            this.rbForceSeqType.Text =
                ConfigurationSettings.AppSettings["Form_SeqExtractor_RadioForceSeqType"];
            this.cbLoopEntireTrack.Text =
                ConfigurationSettings.AppSettings["Form_SeqExtractor_CheckBoxLoopEntireTrack"];
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }
        private void tbSource_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            PsxSeqExtractWorker.PsxSeqExtractStruct psxStruct = new PsxSeqExtractWorker.PsxSeqExtractStruct();
            psxStruct.SourcePaths = s;
            psxStruct.force2Loops = cbForce2Loops.Checked;
            psxStruct.forceSepType = rbForceSepType.Checked;
            psxStruct.forceSeqType = rbForceSeqType.Checked;
            psxStruct.loopEntireTrack = cbLoopEntireTrack.Checked;

            base.backgroundWorker_Execute(psxStruct);
        }
        
        private void cbForceType_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cbForceType.Checked)
            {
                this.rbForceSepType.Enabled = true;
                this.rbForceSeqType.Enabled = true;
            }
            else
            {
                this.rbForceSepType.Enabled = false;
                this.rbForceSeqType.Enabled = false;

                this.rbForceSepType.Checked = false;
                this.rbForceSeqType.Checked = false;
            }
        }
    }
}
