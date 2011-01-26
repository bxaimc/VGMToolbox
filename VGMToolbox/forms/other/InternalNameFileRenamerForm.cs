using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.plugin;

namespace VGMToolbox.forms.other
{
    public partial class InternalNameFileRenamerForm : VgmtForm
    {
        public InternalNameFileRenamerForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = "Internal Name File Renamer";
            this.tbOutput.Text = "Rename files according to names contained in their headers.";
            this.btnDoTask.Hide();            
            
            InitializeComponent();

            this.rbNameLength.Checked = true;
        }

        private void doLengthRadioButtons()
        {
            this.tbNameLength.Enabled = this.rbNameLength.Checked;
            this.tbTerminatorBytes.Enabled = this.rbTerminatorBytes.Checked;
        }
        private void rbNameLength_CheckedChanged(object sender, EventArgs e)
        {
            this.doLengthRadioButtons();
        }
        private void rbTerminatorBytes_CheckedChanged(object sender, EventArgs e)
        {
            this.doLengthRadioButtons();
        }
    }
}
