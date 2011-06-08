using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using VGMToolbox.util;

namespace VGMToolbox.format
{
    public abstract class MobiclipStream
    {
        public enum MovieType
        { 
            NintendoDs,
            Wii,
            Unknown
        }
        
        public static readonly byte[] MagicBytes = new byte[] { 0x4D, 0x4F };
        public static readonly byte[] DataStartBytes = new byte[] { 0x48, 0x45, 0x00, 0x00 };
        
        public long HeaderSize { set; get; }

        public int VideoStreamCount { set; get; }
        public int AudioStreamCount { set; get; }

        public int AudioBitrate { set; get; }

        public string FilePath { get; set; }
        public string FileExtensionAudio { get; set; }
        public string FileExtensionVideo { get; set; }

        protected abstract long GetBlockSize(FileStream inStream, long currentOffset);

        protected abstract byte[] GetVideoChunk(FileStream inStream, long currentOffset);

        protected abstract byte[] GetAudioChunk(FileStream inStream, long currentOffset, long blockSize, long videoChunkSize);

        protected virtual void DoFinalTasks(Dictionary<uint, FileStream> streamWriters)
        {
            foreach (uint key in streamWriters.Keys)
            {
                if (streamWriters[key] != null &&
                    streamWriters[key].CanRead)
                {
                    streamWriters[key].Close();
                    streamWriters[key].Dispose();
                }
            }
        }

        private void writeChunkToStream(byte[] chunk, uint chunkId, Dictionary<uint, FileStream> streamWriters)
        {
            string destinationFile;
            
            if (!streamWriters.ContainsKey(chunkId))
            {
                destinationFile = Path.Combine(Path.GetDirectoryName(this.FilePath), 
                    String.Format("{0}_{1}.bin", Path.GetFileNameWithoutExtension(this.FilePath), chunkId.ToString("X2")));
                streamWriters[chunkId] = File.Open(destinationFile, FileMode.Create, FileAccess.ReadWrite);
            }

            streamWriters[chunkId].Write(chunk, 0, chunk.Length);

        }

        public virtual void DemultiplexStreams(MpegStream.DemuxOptionsStruct demuxOptions)
        {
            long currentOffset = 0;
            long fileSize;

            long blockSize;

            Dictionary<uint, FileStream> streamOutputWriters = new Dictionary<uint, FileStream>();
            byte[] currentChunk;

            try
            {
                using (FileStream fs = File.OpenRead(this.FilePath))
                {
                    fileSize = fs.Length;
                    currentOffset = ParseFile.GetNextOffset(fs, currentOffset, DataStartBytes) + 4;

                    while (currentOffset < fileSize)
                    {

                        blockSize = this.GetBlockSize(fs, currentOffset);

                        if (blockSize > 0)
                        {
                            // write video block to stream                        
                            currentChunk = this.GetVideoChunk(fs, currentOffset);

                            if (demuxOptions.ExtractVideo)
                            {
                                this.writeChunkToStream(currentChunk, 0, streamOutputWriters);
                            }
                            // NDS -- currentBlockId = (uint)(BitConverter.ToUInt16(ParseFile.ParseSimpleOffset(fs, currentOffset, 2), 0) & 0x00FF);


                            // write audio block to stream
                            if (demuxOptions.ExtractAudio)
                            {
                                currentChunk = this.GetAudioChunk(fs, currentOffset, blockSize, currentChunk.Length);
                                this.writeChunkToStream(currentChunk, 1, streamOutputWriters);
                            }

                            // move offset
                            currentOffset += blockSize;
                        }
                        else
                        {
                            break;
                        }
                    }

                } // using (FileStream fs = File.OpenRead(this.FilePath))
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Exception processing block at offset 0x{0}: {1}{2}", currentOffset.ToString("X"), ex.Message, Environment.NewLine));
            }
            finally
            {
                // clean up
                this.DoFinalTasks(streamOutputWriters);
            }
        }
        

        public static MovieType GetCriMovie2StreamType(string path)
        {
            MovieType streamType = MovieType.Unknown;

            using (FileStream fs = File.OpenRead(path))
            {
                byte[] typeBytes = ParseFile.ParseSimpleOffset(fs, 2, 2);

                if (ParseFile.CompareSegment(typeBytes, 0, MobiclipNdsStream.StreamTypeBytes))
                {
                    streamType = MovieType.NintendoDs;
                }
                else if (ParseFile.CompareSegment(typeBytes, 0, MobiclipWiiStream.StreamTypeBytes))
                {
                    streamType = MovieType.Wii;
                }
            }

            return streamType;
        }        
    }
}
