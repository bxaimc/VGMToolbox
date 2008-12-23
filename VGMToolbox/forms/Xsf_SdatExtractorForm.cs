using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace VGMToolbox.forms
{
    public partial class Xsf_SdatExtractorForm : VgmtForm
    {
        public Xsf_SdatExtractorForm()
        {
            this.TopLevel = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Dock = DockStyle.Fill;           
            this.lblTitle.Text = "SDAT Extractor";
            
            InitializeComponent();
        }
    }
}
