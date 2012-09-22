using System;
using System.Collections;
using System.IO;
using System.Text;

namespace VGMToolbox.format
{
    public class NintendoU8Archive
    {
        public static readonly byte[] STANDARD_IDENTIFIER = new byte[] { 0x55, 0xAA, 0x38, 0x2D };
        public const uint IDENTIFIER_OFFSET = 0x00;
        public const string FORMAT_DESCRIPTION_STRING = "Nintendo U8";
        public const string EXTRACTION_FOLDER = "VGMT_U8_EXTRACT";

        public struct u8Node
        {
            public ushort NodeType { set; get; }
            public ushort NameOffset { set; get; }
            public uint DataOffset { set; get; }
            public uint DataSize { set; get; }
        }

        public string SourceFileName { set; get; }

        public uint RootNodeOffset { set; get; }
        public uint HeaderOffset { set; get; }
        public uint DataOffset { set; get; }

        public u8Node[] NodeList { set; get; }
    }
}
