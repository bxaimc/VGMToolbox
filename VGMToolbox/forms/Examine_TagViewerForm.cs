using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace VGMToolbox.forms
{
    public partial class Examine_TagViewerForm : VgmtTreeviewForm
    {
        public Examine_TagViewerForm()
        {
            this.TopLevel = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Dock = DockStyle.Fill;
            this.lblTitle.Text = "Tag Viewer";            

            InitializeComponent();
        }
    }
}
