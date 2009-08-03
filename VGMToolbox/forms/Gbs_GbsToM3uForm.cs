using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.gbs;

namespace VGMToolbox.forms
{
    public partial class Gbs_GbsToM3uForm : AVgmtForm
    {        
        public Gbs_GbsToM3uForm(TreeNode pTreeNode): base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = 
                ConfigurationSettings.AppSettings["Form_GbsM3u_Title"];
            // hide the DoTask button since this is a drag and drop form
            this.btnDoTask.Hide();
            
            InitializeComponent();

            // messages
            this.BackgroundWorker = new GbsM3uBuilderWorker();
            this.BeginMessage = ConfigurationSettings.AppSettings["Form_GbsM3u_MessageBegin"];
            this.CompleteMessage = ConfigurationSettings.AppSettings["Form_GbsM3u_MessageComplete"];
            this.CancelMessage = ConfigurationSettings.AppSettings["Form_GbsM3u_MessageCancel"];

            this.grpSource.AllowDrop = true;

            this.grpSource.Text =
                ConfigurationSettings.AppSettings["Form_Global_DropSourceFiles"];
            this.grpOptions.Text =
                ConfigurationSettings.AppSettings["Form_GbsM3u_GroupOptions"];
            this.cbGBS_OneM3uPerTrack.Text =
                ConfigurationSettings.AppSettings["Form_GbsM3u_CheckBoxOneM3uPerTrack"];
        }

        private void tbGBS_gbsm3uSource_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            GbsM3uBuilderWorker.GbsM3uWorkerStruct gbStruct = new GbsM3uBuilderWorker.GbsM3uWorkerStruct();
            gbStruct.SourcePaths = s;
            gbStruct.onePlaylistPerFile = cbGBS_OneM3uPerTrack.Checked;

            base.backgroundWorker_Execute(gbStruct);
        }        
        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }
    }
}
