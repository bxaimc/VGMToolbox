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

            // messages
            this.BackgroundWorker = new ExtractMidiWorker();
            this.BeginMessage = ConfigurationSettings.AppSettings["Form_MidiExtractor_MessageBegin"];
            this.CompleteMessage = ConfigurationSettings.AppSettings["Form_MidiExtractor_MessageComplete"];
            this.CancelMessage = ConfigurationSettings.AppSettings["Form_MidiExtractor_MessageCancel"];

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
    }
}
