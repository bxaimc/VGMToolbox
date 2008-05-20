using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGMToolbox.format
{
    class Nsf
    {
        private static readonly byte[] ASCII_SIGNATURE = new byte[] { 0x4E, 0x45, 0x53, 0x4D, 0x1A }; // PSF
        private const string FORMAT_ABBREVIATION = "NSF";

        private const int SIG_OFFSET = 0x00;
        private const int SIG_LENGTH = 0x05;

        private const int VERSION_OFFSET = 0x05;
        private const int VERSION_LENGTH = 0x01;        
        
        
        
        
        
        public string getFormatAbbreviation()
        {
            return FORMAT_ABBREVIATION;
        }
        
        bool IsFileLibrary(string pPath)
        {
            return false;
        }
    }
}
