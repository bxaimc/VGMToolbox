using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.tools.hoot;

namespace VGMToolbox.forms
{
    public partial class Hoot_CsvDatafileForm : VgmtForm
    {
        public Hoot_CsvDatafileForm(TreeNode pTreeNode) : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = "Hoot CSV to Datafile";

            this.btnDoTask.Text = "Add Info";
            this.btnCancel.Hide();
            
            InitializeComponent();
        }
    }
}
