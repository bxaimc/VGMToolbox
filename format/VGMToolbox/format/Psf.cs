using System;
using System.Collections.Generic;
using System.Text;

namespace VGMToolbox.format
{
    public class Psf: Xsf
    {
        public const uint MIN_TEXT_SECTION_OFFSET = 0x80010000;
        public const uint MAX_TEXT_SECTION_OFFSET = 0x801F0000;
        public const uint PC_OFFSET_CORRECTION = 0x800;
        public const uint TEXT_SIZE_OFFSET = 0x1C;

        public const int MINIPSF_INITIAL_PC_OFFSET = 0x10;
        public const int MINIPSF_TEXT_SECTION_OFFSET = 0x18;
        public const int MINIPSF_TEXT_SECTION_SIZE_OFFSET = 0x1C;       
    }
}
