using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace VGMToolbox.forms
{
    public partial class TreeViewVgmtForm : VgmtForm
    {
        public TreeViewVgmtForm() : base() 
        {
            InitializeComponent();
        }
        
        public TreeViewVgmtForm(TreeNode pTreeNode) : base(pTreeNode)
        {
            InitializeComponent();
        }
    }
}
