using System.Configuration;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.extract;

namespace VGMToolbox.forms.extraction
{
    public partial class MidiExtractorForm : AVgmtForm
    {
        public MidiExtractorForm(TreeNode pTreeNode) : 
            base(pTreeNode)

        {
            InitializeComponent();

            this.lblTitle.Text = ConfigurationSettings.AppSettings["Form_MidiExtractor_Title"];
            this.tbOutput.Text = ConfigurationSettings.AppSettings["Form_MidiExtractor_IntroText"];
            this.grpSourceFiles.Text = ConfigurationSettings.AppSettings["Form_Global_DropSourceFiles"];
            
            this.grpSourceFiles.AllowDrop = true;
            this.btnDoTask.Hide();
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }
        private void grpSourceFiles_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            ExtractMidiWorker.ExtractMidiStruct bwStruct = new ExtractMidiWorker.ExtractMidiStruct();            
            bwStruct.SourcePaths = s;
            base.backgroundWorker_Execute(bwStruct);
        }

        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new ExtractMidiWorker();
        }
        protected override string getCancelMessage()
        {
            return ConfigurationSettings.AppSettings["Form_MidiExtractor_MessageCancel"];
        }
        protected override string getCompleteMessage()
        {
            return ConfigurationSettings.AppSettings["Form_MidiExtractor_MessageComplete"];
        }
        protected override string getBeginMessage()
        {
            return ConfigurationSettings.AppSettings["Form_MidiExtractor_MessageBegin"];
        }
    }
}
