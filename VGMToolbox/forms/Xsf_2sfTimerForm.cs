using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace VGMToolbox.forms
{
    public partial class Xsf_2sfTimerForm : VgmtForm
    {
        public Xsf_2sfTimerForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = "2SF Timer";

            // hide the DoTask button since this is a drag and drop form
            this.btnDoTask.Text = "Time 2SFs";

            InitializeComponent();
        }
    }
}
