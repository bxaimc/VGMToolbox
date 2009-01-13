using System;
using System.Collections.Generic;
using System.Text;

namespace VGMToolbox.format
{
    public interface IHootFormat : IFormat
    {
        string GetHootDriverAlias();
        string GetHootDriverType();
        string GetHootDriver();    
    }
}
