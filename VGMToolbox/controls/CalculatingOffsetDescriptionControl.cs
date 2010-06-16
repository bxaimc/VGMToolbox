using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace VGMToolbox.controls
{
    public partial class CalculatingOffsetDescriptionControl : OffsetDescriptionControl
    {
        public CalculatingOffsetDescriptionControl()
        {
            InitializeComponent();
        }

        //------------
        // properties
        //------------
        public string CalculationValue
        {
            get { return this.tbCalculation.Text; }
            set { this.tbCalculation.Text = value; }
        }

        public new bool Enabled
        {
            set
            {
                base.Enabled = value;
                this.tbCalculation.Enabled = value;
            }
        }
    }
}
