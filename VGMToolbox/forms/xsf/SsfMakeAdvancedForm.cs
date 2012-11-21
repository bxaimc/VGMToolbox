using System;
using System.Collections.Generic;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.xsf;

namespace VGMToolbox.forms.xsf
{
    public partial class SsfMakeAdvancedForm : VgmtForm
    {
        public SsfMakeAdvancedForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            this.lblTitle.Text = "ssfmake Advanced Front End (note: Python must be installed and in your PATH.)";
            this.btnDoTask.Text = "Make SSFs";
            
            InitializeComponent();
        }

        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new SsfMakeWorker();
        }
        protected override string getCancelMessage()
        {
            return ConfigurationSettings.AppSettings["Form_SsfMakeFE_MessageCancel"];
        }
        protected override string getCompleteMessage()
        {
            return ConfigurationSettings.AppSettings["Form_SsfMakeFE_MessageComplete"];
        }
        protected override string getBeginMessage()
        {
            return ConfigurationSettings.AppSettings["Form_SsfMakeFE_MessageBegin"];
        }
    }
}
