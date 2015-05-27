using System;
using System.Collections.Generic;
using System.IO;

using VGMToolbox.util;

namespace VGMToolbox.format
{
    public class SonyPspMpsStream : SonyPmfStream
    {
        public SonyPspMpsStream(string path)
            : base(path) { }

        protected override long GetStartOffset(Stream readStream, long currentOffset)
        {
            return 0;
        }
    }
}
