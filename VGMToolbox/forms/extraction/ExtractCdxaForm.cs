using System;
using System.ComponentModel;
using System.Configuration;
using System.Windows.Forms;

using VGMToolbox.format;
using VGMToolbox.plugin;
using VGMToolbox.tools.extract;

namespace VGMToolbox.forms.extraction
{
    public partial class ExtractCdxaForm : AVgmtForm
    {        
        public ExtractCdxaForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = ConfigurationSettings.AppSettings["Form_CdxaExtractor_Title"];
            this.tbOutput.Text = ConfigurationSettings.AppSettings["Form_CdxaExtractor_IntroText"];
            // hide the DoTask button since this is a drag and drop form
            this.btnDoTask.Hide();
            
            InitializeComponent();

            // messages
            this.BackgroundWorker = new ExtractCdxaWorker();
            this.BeginMessage = ConfigurationSettings.AppSettings["Form_CdxaExtractor_MessageBegin"];
            this.CompleteMessage = ConfigurationSettings.AppSettings["Form_CdxaExtractor_MessageComplete"];
            this.CancelMessage = ConfigurationSettings.AppSettings["Form_CdxaExtractor_MessageCancel"];

            this.grpSource.AllowDrop = true;

            this.grpSource.Text =
                ConfigurationSettings.AppSettings["Form_Global_DropSourceFiles"];
            this.grpOptions.Text =
                ConfigurationSettings.AppSettings["Form_CdxaExtractor_GroupOptions"];
            this.lblSilentBlocks.Text = ConfigurationSettings.AppSettings["Form_CdxaExtractor_LblSilentBlocks"];
            
            this.cbAddRiffHeader.Text =
                ConfigurationSettings.AppSettings["Form_CdxaExtractor_CheckBoxAddRiffHeader"];
            this.cbPatchByte0x11.Text =
                ConfigurationSettings.AppSettings["Form_CdxaExtractor_CheckBoxPatchByte0x11"];

            this.silentFrameCounter.Value = Cdxa.NUM_SILENT_FRAMES_FOR_SILENT_BLOCK;
        }

        private void tbSource_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            ExtractCdxaWorker.ExtractCdxaStruct extStruct = new ExtractCdxaWorker.ExtractCdxaStruct();
            extStruct.SourcePaths = s;
            extStruct.AddRiffHeader = cbAddRiffHeader.Checked;
            extStruct.PatchByte0x11 = cbPatchByte0x11.Checked;
            extStruct.SilentFramesCount = (uint)this.silentFrameCounter.Value;

            base.backgroundWorker_Execute(extStruct);
        }
        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }
    }
}
