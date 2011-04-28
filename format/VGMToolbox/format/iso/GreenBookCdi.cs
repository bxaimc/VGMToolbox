using System;
using System.Collections.Generic;
using System.Text;

namespace VGMToolbox.format.iso
{
    public class GreenBookCdi : Iso9660
    {
        public static readonly byte[] VOLUME_DESCRIPTOR_IDENTIFIER = new byte[] { 0x01, 0x43, 0x44, 0x2D, 0x49, 0x20 };
        public static string FORMAT_DESCRIPTION_STRING = "CDI";
    }

    public class GreenBookCdiVolume : Iso9660Volume
    { 
    
    }
}
