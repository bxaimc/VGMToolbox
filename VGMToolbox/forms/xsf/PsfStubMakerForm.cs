using System.Configuration;
using System.Windows.Forms;

using VGMToolbox.format.util;
using VGMToolbox.plugin;
using VGMToolbox.tools.xsf;

namespace VGMToolbox.forms.xsf
{
    public partial class PsfStubMakerForm : AVgmtForm
    {
        public PsfStubMakerForm(TreeNode pTreeNode): 
            base(pTreeNode)
        {
            InitializeComponent();

            this.grpSourceFiles.AllowDrop = true;

            this.lblTitle.Text = ConfigurationSettings.AppSettings["Form_PsfStubCreator_Title"];
            this.grpSourceFiles.Text = ConfigurationSettings.AppSettings["Form_Global_DropSourceFiles"];
            this.grpOptions.Text = ConfigurationSettings.AppSettings["Form_PsfStubCreator_GroupOptions"];
            this.lblDriverText.Text = ConfigurationSettings.AppSettings["Form_PsfStubCreator_LblDriverText"];
            this.tbOutput.Text = ConfigurationSettings.AppSettings["Form_PsfStubCreator_IntroText"];
            
            this.btnDoTask.Hide();            
            this.loadDefaults();

            this.cbOverrideDriverOffset.Checked = false;
            this.overrideDriverOffset();
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }

        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new PsfStubMakerWorker();
        }
        protected override string getCancelMessage()
        {
            return ConfigurationSettings.AppSettings["Form_PsfStubCreator_MessageCancel"];
        }
        protected override string getCompleteMessage()
        {
            return ConfigurationSettings.AppSettings["Form_PsfStubCreator_MessageComplete"];
        }
        protected override string getBeginMessage()
        {
            return ConfigurationSettings.AppSettings["Form_PsfStubCreator_MessageBegin"];
        }

        private void grpSourceFiles_DragDrop(object sender, DragEventArgs e)
        {
            // check for PsyQ in PATH
            if (!XsfUtil.IsPsyQPathVariablePresent())
            {
                MessageBox.Show(
                    ConfigurationSettings.AppSettings["Form_PsfStubCreator_ErrPsyQPath"],
                    ConfigurationSettings.AppSettings["Form_Global_ErrorWindowTitle"]);                           
            }
            else if (!XsfUtil.IsPsyQSdkPresent())
            {
                MessageBox.Show(
                    ConfigurationSettings.AppSettings["Form_PsfStubCreator_IntroText"],
                    ConfigurationSettings.AppSettings["Form_Global_ErrorWindowTitle"]);
            }
            else if (this.validateInputs())
            {
                string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

                PsfStubMakerStruct bwStruct = new PsfStubMakerStruct();
                bwStruct.SourcePaths = s;
                bwStruct.UseSeqFunctions = this.cbSeqFunctions.Checked;
                bwStruct.DriverText = this.tbDriverText.Text;
                bwStruct.IncludeReverb = this.cbIncludeReverb.Checked;

                bwStruct.PsfDrvLoad = this.tbPsfDrvLoad.Text;
                bwStruct.PsfDrvSize = this.tbPsfDrvSize.Text;
                bwStruct.PsfDrvParam = this.tbPsfDrvParam.Text;
                bwStruct.PsfDrvParamSize = this.tbPadDrvParamSize.Text;

                bwStruct.MySeq = this.tbMySeq.Text;
                bwStruct.MySeqSize = this.tbMySeqSize.Text;
                bwStruct.MyVh = this.tbMyVh.Text;
                bwStruct.MyVhSize = this.tbMyVhSize.Text;
                bwStruct.MyVb = this.tbMyVb.Text;
                bwStruct.MyVbSize = this.tbMyVbSize.Text;

                bwStruct.OverrideDriverLoadAddress = cbOverrideDriverOffset.Checked;

                base.backgroundWorker_Execute(bwStruct);
            }
        }

        private void loadDefaults()
        {
            this.tbPsfDrvLoad.Text = PsfStubMakerWorker.PsfDrvLoadDefault;
            this.tbPsfDrvSize.Text = PsfStubMakerWorker.PsfDrvSizeDefault;
            this.tbPsfDrvParam.Text = PsfStubMakerWorker.PsfDrvParamDefault;
            this.tbPadDrvParamSize.Text = PsfStubMakerWorker.PsfDrvParamSizeDefault;

            this.tbMySeq.Text = PsfStubMakerWorker.MySeqDefault;
            this.tbMySeqSize.Text = PsfStubMakerWorker.MySeqSizeDefault;
            this.tbMyVh.Text = PsfStubMakerWorker.MyVhDefault;
            this.tbMyVhSize.Text = PsfStubMakerWorker.MyVhSizeDefault;
            this.tbMyVb.Text = PsfStubMakerWorker.MyVbDefault;
            this.tbMyVbSize.Text = PsfStubMakerWorker.MyVbSizeDefault;        
        }

        private bool validateInputs()
        {
            bool ret = true;

            if (cbOverrideDriverOffset.Checked)
            {
                ret = ret && base.checkTextBox(this.tbPsfDrvLoad.Text, this.lblPsfDrvLoad.Text);
                ret = ret && base.checkTextBox(this.tbPsfDrvSize.Text, this.lblPsfDrvSize.Text);
                ret = ret && base.checkTextBox(this.tbPsfDrvParam.Text, this.lblPsfDrvParam.Text);
                ret = ret && base.checkTextBox(this.tbPadDrvParamSize.Text, this.lblPadDrvParamSize.Text);

                ret = ret && base.checkTextBox(this.tbMySeq.Text, this.lblMySeq.Text);
                ret = ret && base.checkTextBox(this.tbMySeqSize.Text, this.tbMySeqSize.Text);
                ret = ret && base.checkTextBox(this.tbMyVh.Text, this.lblMyVh.Text);
                ret = ret && base.checkTextBox(this.tbMyVhSize.Text, this.tbMyVhSize.Text);

                ret = ret && base.checkTextBox(this.tbMyVb.Text, this.lblMyVb.Text);
                ret = ret && base.checkTextBox(this.tbMyVbSize.Text, this.tbMyVbSize.Text);
            }
            
            return ret;
        }

        private void btnLoadDefaults_Click(object sender, System.EventArgs e)
        {
            cbOverrideDriverOffset.Checked = true;
            this.overrideDriverOffset();
            this.loadDefaults();
        }

        private void cbOverrideDriverOffset_CheckedChanged(object sender, System.EventArgs e)
        {
            this.overrideDriverOffset();
        }

        public void overrideDriverOffset()
        {
            if (!cbOverrideDriverOffset.Checked)
            {
                this.tbPsfDrvLoad.ReadOnly = true;
                this.tbPsfDrvSize.ReadOnly = true;
                this.tbPsfDrvParam.ReadOnly = true;
                this.tbPadDrvParamSize.ReadOnly = true;
                this.tbMySeq.ReadOnly = true;
                this.tbMySeqSize.ReadOnly = true;
                this.tbMyVh.ReadOnly = true;
                this.tbMyVhSize.ReadOnly = true;
                this.tbMyVb.ReadOnly = true;
                this.tbMyVbSize.ReadOnly = true;

                this.tbPsfDrvLoad.Clear();
                this.tbPsfDrvSize.Clear();
                this.tbPsfDrvParam.Clear();
                this.tbPadDrvParamSize.Clear();
                this.tbMySeq.Clear();
                this.tbMySeqSize.Clear();
                this.tbMyVh.Clear();
                this.tbMyVhSize.Clear();
                this.tbMyVb.Clear();
                this.tbMyVbSize.Clear();
            }
            else
            {
                this.tbPsfDrvLoad.ReadOnly = false;
                this.tbPsfDrvSize.ReadOnly = false;
                this.tbPsfDrvParam.ReadOnly = false;
                this.tbPadDrvParamSize.ReadOnly = false;
                this.tbMySeq.ReadOnly = false;
                this.tbMySeqSize.ReadOnly = false;
                this.tbMyVh.ReadOnly = false;
                this.tbMyVhSize.ReadOnly = false;
                this.tbMyVb.ReadOnly = false;
                this.tbMyVbSize.ReadOnly = false;                
                
                /*
                this.tbPsfDrvLoad.Text = PsfStubMakerWorker.PsfDrvLoadDefault;
                this.tbPsfDrvSize.Text = PsfStubMakerWorker.PsfDrvSizeDefault;
                this.tbPsfDrvParam.Text = PsfStubMakerWorker.PsfDrvParamDefault;
                this.tbPadDrvParamSize.Text = PsfStubMakerWorker.PsfDrvParamSizeDefault;
                this.tbMySeq.Text = PsfStubMakerWorker.MySeqDefault;
                this.tbMySeqSize.Text = PsfStubMakerWorker.MySeqSizeDefault;
                this.tbMyVh.Text = PsfStubMakerWorker.MyVhDefault;
                this.tbMyVhSize.Text = PsfStubMakerWorker.MyVhSizeDefault;
                this.tbMyVb.Text = PsfStubMakerWorker.MyVbDefault;
                this.tbMyVbSize.Text = PsfStubMakerWorker.MyVbSizeDefault;
                 */
            }        
        }

        public string GetSeqOffset()
        {
            return this.tbMySeq.Text;
        }
        public string GetSeqSize()
        {
            return this.tbMySeqSize.Text;
        }
        public string GetVhOffset()
        {
            return this.tbMyVh.Text;
        }
        public string GetVbOffset()
        {
            return this.tbMyVb.Text;
        }

        public string GetParamOffset()
        {
            return this.tbPsfDrvParam.Text;
        }
    }
}
