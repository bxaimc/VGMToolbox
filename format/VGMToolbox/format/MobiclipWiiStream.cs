using System;
using System.IO;
using System.Text;

using VGMToolbox.util;

namespace VGMToolbox.format
{
    public class MobiclipWiiStream : MobiclipStream
    {
        public static readonly byte[] StreamTypeBytes = new byte[] { 0x43, 0x35 }; //C5

        public MobiclipWiiStream(string path)
        {
            this.FilePath = path;
        }

        protected override long GetBlockSize(FileStream inStream, long currentOffset)
        {
            long blockSize = (long)BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(inStream, currentOffset, 4), 0);

            if (blockSize > 0)
            {
                blockSize += 4;

                if ((blockSize % 4) != 0)
                {
                    blockSize -= (blockSize % 4);
                }
            }
            return blockSize;
        }

        protected override byte[] GetVideoChunk(FileStream inStream, long currentOffset)
        {
            long chunkSize = (long)BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(inStream, currentOffset + 4, 4), 0);
            byte[] videoChunk = ParseFile.ParseSimpleOffset(inStream, currentOffset + 8, (int)chunkSize);
            return videoChunk;        
        }

        protected override byte[] GetAudioChunk(FileStream inStream, long currentOffset, long blockSize, long videoChunkSize)
        {
            long absoluteOffset = currentOffset + 8 + videoChunkSize + 4;
            byte[] audioChunk = ParseFile.ParseSimpleOffset(inStream, absoluteOffset, (int)(blockSize - videoChunkSize - 8 - 4));
            return audioChunk;
        }
    }
}
