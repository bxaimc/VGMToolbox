using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace VGMToolbox.forms
{
    public partial class VgmtTreeviewForm : VgmtForm
    {
        public VgmtTreeviewForm()
        {
            this.treeView1 = new TreeView();            
            this.treeView1.Dock = DockStyle.Left;
            this.treeView1.BringToFront();

            InitializeComponent();
        }
    }
}
