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
            else
            {
                string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

                PsfStubMakerStruct bwStruct = new PsfStubMakerStruct();
                bwStruct.SourcePaths = s;
                bwStruct.DriverText = this.tbDriverText.Text;
                bwStruct.IncludeReverb = this.cbIncludeReverb.Checked;

                base.backgroundWorker_Execute(bwStruct);
            }
        }
    }
}
