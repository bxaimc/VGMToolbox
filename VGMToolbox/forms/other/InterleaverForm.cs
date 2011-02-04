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
    public partial class InterleaverForm : VgmtForm
    {
        public InterleaverForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            InitializeComponent();

            this.tbOutput.Text = "Interleave files according to the options.";
            
            this.btnDoTask.Text = "Interleave";
        }
    }
}
