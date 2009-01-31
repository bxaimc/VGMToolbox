using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace VGMToolbox.forms
{
    public partial class Xsf_SdatFinderForm : VgmtForm
    {
        public Xsf_SdatFinderForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = "Nintendo DS SDAT Finder";
            this.tbOutput.Text = "Find and Extract SDATs from files.";

            // hide the DoTask button since this is a drag and drop form
            this.btnDoTask.Hide();

            InitializeComponent();
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }


    }
}
