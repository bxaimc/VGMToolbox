using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using VGMToolbox.util;

namespace VGMToolbox.format
{
    public class MicrosoftAsfContainer
    {
        const string DefaultFileExtensionAudio = ".wma";
        const string DefaultFileExtensionVideo = ".wmv";
        
        public static readonly byte[] ASF_HEADER_BYTES = new byte[] { 0x30, 0x26, 0xB2, 0x75, 
                                                                      0x8E, 0x66, 0xCF, 0x11, 
                                                                      0xA6, 0xD9, 0x00, 0xAA, 
                                                                      0x00, 0x62, 0xCE, 0x6C};
        
        public static readonly Guid ASF_Header_Object = new Guid(MicrosoftAsfContainer.ASF_HEADER_BYTES);

        public static readonly Guid ASF_File_Properties_Object = new Guid(new byte[] { 0xA1, 0xDC, 0xAB, 0x8C, 
                                                                                       0x47, 0xA9, 0xCF, 0x11, 
                                                                                       0x8E, 0xE4, 0x00, 0xC0, 
                                                                                       0x0C, 0x20, 0x53, 0x65 });

        public static readonly Guid ASF_Header_Extension_Object = new Guid(new byte[] { 0xB5, 0x03, 0xBF, 0x5F, 
                                                                                        0x2E, 0xA9, 0xCF, 0x11, 
                                                                                        0x8E, 0xE3, 0x00, 0xC0, 
                                                                                        0x0C, 0x20, 0x53, 0x65 });

        public static readonly Guid ASF_Stream_Properties_Object = new Guid(new byte[] { 0x91, 0x07, 0xDC, 0xB7, 
                                                                                         0xB7, 0xA9, 0xCF, 0x11, 
                                                                                         0x8E, 0xE6, 0x00, 0xC0, 
                                                                                         0x0C, 0x20, 0x53, 0x65 });        
        
        public static readonly Guid ASF_Codec_List_Object = new Guid(new byte[] { 0x40, 0x52, 0xD1, 0x86, 
                                                                                  0x1D, 0x31, 0xD0, 0x11, 
                                                                                  0xA3, 0xA4, 0x00, 0xA0, 
                                                                                  0xC9, 0x03, 0x48, 0xF6 });

        
        public static readonly Guid ASF_Data_Object = new Guid(new byte[] { 0x36, 0x26, 0xB2, 0x75, 
                                                                            0x8E, 0x66, 0xCF, 0x11, 
                                                                            0xA6, 0xD9, 0x00, 0xAA, 
                                                                            0x00, 0x62, 0xCE, 0x6C });

             
        struct AsfHeaderObject
        {
            public Guid ObjectGuid { set; get; }
            public UInt64 ObjectSize { set; get; }
            public UInt32 NumberOfHeaderObjects { set; get; }
            public byte Reserved1 { set; get; }
            public byte Reserved2 { set; get; }
        }
        
        struct AsfObjectDefinition
        {
            public Guid ObjectGuid { set; get; }
            public UInt64 ObjectSize { set; get; }
            public byte[] ObjectData { set; get; }
        }

        public MicrosoftAsfContainer(string filePath)
        {
            this.FilePath = filePath;
            this.FileExtensionAudio = MicrosoftAsfContainer.DefaultFileExtensionAudio;
            this.FileExtensionVideo = MicrosoftAsfContainer.DefaultFileExtensionVideo;
        }

        public string FilePath { get; set; }
        public string FileExtensionAudio { get; set; }
        public string FileExtensionVideo { get; set; }

        private void writeChunkToStream(
            byte[] chunk,
            uint chunkId,
            Dictionary<uint, FileStream> streamWriters,
            string fileExtension)
        {
            string destinationFile;

            if (!streamWriters.ContainsKey(chunkId))
            {
                destinationFile = Path.Combine(Path.GetDirectoryName(this.FilePath),
                    String.Format("{0}_{1}{2}", Path.GetFileNameWithoutExtension(this.FilePath), chunkId.ToString("X4"), fileExtension));
                streamWriters[chunkId] = File.Open(destinationFile, FileMode.Create, FileAccess.ReadWrite);
            }

            streamWriters[chunkId].Write(chunk, 0, chunk.Length);
        }

        public virtual void DemultiplexStreams(MpegStream.DemuxOptionsStruct demuxOptions)
        {
            long currentOffset = 0;
            long fileSize;
            
            Dictionary<uint, FileStream> streamOutputWriters = new Dictionary<uint, FileStream>();

            Guid checkGuid;
            ulong blockSize;

            try
            {
                using (FileStream fs = File.OpenRead(this.FilePath))
                {
                    fileSize = fs.Length;
                    currentOffset = 0;

                    currentOffset = ParseFile.GetNextOffset(fs, 0, MicrosoftAsfContainer.ASF_HEADER_BYTES);

                    if (currentOffset > -1)
                    {
                        while (currentOffset < fileSize)
                        {
                            // get guid
                            checkGuid = new Guid(ParseFile.ParseSimpleOffset(fs, currentOffset, 0x10));
                            blockSize = BitConverter.ToUInt64(ParseFile.ParseSimpleOffset(fs, (currentOffset + 0x10), 8), 0);

                            // process block
                            if (checkGuid.Equals(MicrosoftAsfContainer.ASF_Header_Object))
                            {
                                // parse header
                            }
                            else if (checkGuid.Equals(MicrosoftAsfContainer.ASF_Data_Object))
                            {
                                // write data to file
                            }

                            // increment counter
                            currentOffset += (long)blockSize;
                          
                        } // while
                    }
                    else
                    {
                        throw new Exception(String.Format("ASF/WMV Header not found.{0}", Environment.NewLine));    
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
                // this.DoFinalTasks(streamOutputWriters, demuxOptions);
            }
        }
    }
}
