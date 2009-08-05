using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.nsf;

namespace VGMToolbox.forms
{
    public partial class Nsf_Nsfe2NsfM3uForm : AVgmtForm
    {        
        public Nsf_Nsfe2NsfM3uForm(TreeNode pTreeNode) : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = 
                ConfigurationSettings.AppSettings["Form_Nsfe2M3u_Title"];
            // hide the DoTask button since this is a drag and drop form
            this.btnDoTask.Hide();
            
            InitializeComponent();

            this.grpSourceFiles.AllowDrop = true;

            this.grpSourceFiles.Text =
                ConfigurationSettings.AppSettings["Form_Global_DropSourceFiles"];
            this.grpOptions.Text =
                ConfigurationSettings.AppSettings["Form_Nsfe2M3u_GroupOptions"];
            this.cbNSFE_OneM3uPerTrack.Text =
                ConfigurationSettings.AppSettings["Form_Nsfe2M3u_CheckBoxOneM3uPerTrack"];
        }

        private void tbNSF_nsfe2m3uSource_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            NsfeM3uBuilderWorker.NsfeM3uBuilderStruct nsfeStruct = new NsfeM3uBuilderWorker.NsfeM3uBuilderStruct();
            nsfeStruct.SourcePaths = s;
            nsfeStruct.OnePlaylistPerFile = cbNSFE_OneM3uPerTrack.Checked;

            base.backgroundWorker_Execute(nsfeStruct);
        }
        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }

        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new NsfeM3uBuilderWorker();
        }
        protected override string getCancelMessage()
        {
            return ConfigurationSettings.AppSettings["Form_Nsfe2M3u_MessageCancel"];
        }
        protected override string getCompleteMessage()
        {
            return ConfigurationSettings.AppSettings["Form_Nsfe2M3u_MessageComplete"];
        }
        protected override string getBeginMessage()
        {
            return ConfigurationSettings.AppSettings["Form_Nsfe2M3u_MessageBegin"];
        }
    }
}
