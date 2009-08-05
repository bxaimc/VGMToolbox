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

            this.grpSourceFiles.AllowDrop = true;
            this.btnDoTask.Hide();
            
            this.grpSourceFiles.Text = ConfigurationSettings.AppSettings["Form_Global_DropSourceFiles"];
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }

        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new VabSplitterWorker();
        }
        protected override string getCancelMessage()
        {
            return ConfigurationSettings.AppSettings["Form_VabSplitter_MessageCancel"];
        }
        protected override string getCompleteMessage()
        {
            return ConfigurationSettings.AppSettings["Form_VabSplitter_MessageComplete"];
        }
        protected override string getBeginMessage()
        {
            return ConfigurationSettings.AppSettings["Form_VabSplitter_MessageBegin"];
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
