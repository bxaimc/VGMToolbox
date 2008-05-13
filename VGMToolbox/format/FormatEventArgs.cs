using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGMToolbox.format
{
    class FormatEventArgs : EventArgs
    {
        private string message;

        public string Message 
        {
            get
            {
                return this.message;
            }
            set
            {
                this.message = value;
            }
        }
    }
}
