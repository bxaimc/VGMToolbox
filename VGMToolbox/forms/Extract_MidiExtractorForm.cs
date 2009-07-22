using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.extract;

namespace VGMToolbox.forms
{
    public partial class Extract_MidiExtractorForm : AVgmtForm
    {
        public Extract_MidiExtractorForm(TreeNode pTreeNode) : 
            base(pTreeNode)

        {
            InitializeComponent();

            this.lblTitle.Text = "MIDI Extractor";
            this.tbOutput.Text = "Extract MIDI files embedded in other files.";
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
            return "Extracting MIDI Data...Cancelled";
        }
        protected override string getCompleteMessage()
        {
            return "Extracting MIDI Data...Complete";
        }
        protected override string getBeginMessage()
        {
            return "Extracting MIDI Data...Begin";
        }
    }
}
