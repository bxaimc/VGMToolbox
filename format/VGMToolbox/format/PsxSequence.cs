using System;
using System.Collections.Generic;
using System.Text;

namespace VGMToolbox.format
{
    public class PsxSequence
    {
        public static readonly byte[] ASCII_SIGNATURE = new byte[] {0x70, 0x51, 0x45, 0x53 }; //pQES
        public static readonly byte[] END_SEQUENCE = new byte[] { 0xFF, 0x2F, 0x00 };
        public const string FILE_EXTENSION = ".seq";
    }
}
