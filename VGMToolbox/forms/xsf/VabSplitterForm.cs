using System.Configuration;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.xsf;

namespace VGMToolbox.forms.xsf
{
    public partial class VabSplitterForm : AVgmtForm
    {
        public VabSplitterForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            this.lblTitle.Text = ConfigurationSettings.AppSettings["Form_VabSplitter_Title"];
            this.tbOutput.Text = ConfigurationSettings.AppSettings["Form_VabSplitter_IntroText"];

            InitializeComponent();

            // messages
            this.BackgroundWorker = new VabSplitterWorker();
            this.BeginMessage = ConfigurationSettings.AppSettings["Form_VabSplitter_MessageBegin"];
            this.CompleteMessage = ConfigurationSettings.AppSettings["Form_VabSplitter_MessageComplete"];
            this.CancelMessage = ConfigurationSettings.AppSettings["Form_VabSplitter_MessageCancel"];

            this.grpSourceFiles.AllowDrop = true;
            this.btnDoTask.Hide();
            
            this.grpSourceFiles.Text = ConfigurationSettings.AppSettings["Form_Global_DropSourceFiles"];
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }

        private void grpSourceFiles_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            VabSplitterWorker.VabSplitterStruct bwStruct = new VabSplitterWorker.VabSplitterStruct();
            bwStruct.SourcePaths = s;

            base.backgroundWorker_Execute(bwStruct);
        }
    }
}
