using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.plugin;

namespace VGMToolbox.forms
{
    public partial class Extract_MidiExtractorForm : VgmtForm
    {
        public Extract_MidiExtractorForm(TreeNode pTreeNode) : 
            base(pTreeNode)

        {
            InitializeComponent();

            this.grpSourceFiles.AllowDrop = true;
            this.btnDoTask.Hide();
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }

        private void grpSourceFiles_DragDrop(object sender, DragEventArgs e)
        {

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
