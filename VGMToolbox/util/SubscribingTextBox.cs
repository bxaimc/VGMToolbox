using System;
using System.Windows.Forms;

using VGMToolbox.format;
using VGMToolbox.tools;


namespace VGMToolbox.util
{
    class SubscribingTextBox : TextBox
    {
        private MdxUtil mdxUtil;

        public SubscribingTextBox(MdxUtil pMdxUtil)
        {            
            mdxUtil = pMdxUtil;
            mdxUtil.SendMessage += new EventHandler(mdxUtil_SendMessage);
        }

        private void mdxUtil_SendMessage(object sender, EventArgs e)
        {
            if (e is FormatEventArgs)
            {
                FormatEventArgs formatArgs = e as FormatEventArgs;
                this.Text += formatArgs.Message;
                this.Show();
            }
        }
    }
}
