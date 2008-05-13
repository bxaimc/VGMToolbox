using System;
using System.Collections.Generic;
using System.Text;

using VGMToolbox.util;

namespace VGMToolbox.format
{
    class Mdx
    {
        public static readonly byte[] TITLE_TERMINATOR = new byte[3] { 0x0D, 0x0A, 0x1A };
        public static readonly byte[] PDX_TERMINATOR = new byte[1] { 0x00 };
        public static readonly string MDX_FILE_EXTENSION = ".MDX";
        public static readonly string PDX_FILE_EXTENSION = ".PDX";

        private string title;
        private string pdxFileName;
        private uint toneDataOffset;
        private uint mmlDataOffset;

        public Mdx() { }
        
        public Mdx(byte[] pBytes)
        {
            initialize(pBytes);
        }

        public string Title
        {
            get { return title; }
        }
        public string PdxFileName
        {
            get { return pdxFileName; }
        }
        public uint ToneDataOffset
        {
            get { return toneDataOffset; }
        }
        public uint MmlDataOffset
        {
            get { return mmlDataOffset; }
        }

        public void initialize(byte[] pBytes)
        {
            int _titleLength;
            int _pdxLength;

            // Title
            _titleLength = ParseFile.getSegmentLength(pBytes, 0, TITLE_TERMINATOR);
            if (_titleLength > 0)
            {
                title = VGMToolbox.util.Encoding.getJpEncodedText(ParseFile.parseSimpleOffset(pBytes, 0,
                         _titleLength));
            }

            // PDX
            _pdxLength = ParseFile.getSegmentLength(pBytes,
                            (_titleLength + TITLE_TERMINATOR.Length), PDX_TERMINATOR);
            if (_pdxLength > 0)
            {
                pdxFileName = VGMToolbox.util.Encoding.getJpEncodedText(ParseFile.parseSimpleOffset(pBytes,
                                (_titleLength + TITLE_TERMINATOR.Length), _pdxLength));
            }

            toneDataOffset = 0;
            mmlDataOffset = 0;        
        }
    }
}
