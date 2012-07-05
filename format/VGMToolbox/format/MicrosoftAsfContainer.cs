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


        public const ushort CodecTypeAudio = 1;
        public const ushort CodecTypeVideo = 2;
        public const ushort CodecTypeUnknown = 0xFFFF;

        public struct AsfHeaderObject
        {
            public Guid ObjectGuid { set; get; }
            public ulong ObjectSize { set; get; }
            public uint NumberOfHeaderObjects { set; get; }
            public byte Reserved1 { set; get; }
            public byte Reserved2 { set; get; }

            public AsfFilePropertiesObject FileProperties { set; get; }
            public AsfStreamPropertiesObject StreamProperties { set; get; }
            public AsfHeaderExtensionObject HeaderExtension { set; get; }
            public AsfCodecListObject CodecList { set; get; }

            public byte[] RawBlock { set; get; }
        }
        
        public struct AsfObjectDefinition
        {
            public Guid ObjectGuid { set; get; }
            public UInt64 ObjectSize { set; get; }
            public byte[] ObjectData { set; get; }
        }

        public struct AsfFilePropertiesObject
        {
            public Guid ObjectGuid { set; get; }
            public ulong ObjectSize { set; get; }
            public Guid FileId { set; get; }
            public ulong FileSize { set; get; }

            public ulong DataPacketsCount { set; get; }

            public byte[] RawBlock { set; get; }
        }

        public struct AsfStreamPropertiesObject
        {
            public Guid ObjectGuid { set; get; }
            public ulong ObjectSize { set; get; }
            public Guid StreamType { set; get; }
            public byte StreamId { set; get; }

            public byte[] RawBlock { set; get; }
        }

        public struct AsfHeaderExtensionObject
        {
            public Guid ObjectGuid { set; get; }
            public ulong ObjectSize { set; get; }
            public byte Reserved1 { set; get; }
            public byte Reserved2 { set; get; }
            public uint HeaderExtensionDataSize { set; get; }
            public byte[] HeaderExtensionData { set; get; }

            public byte[] RawBlock { set; get; }
        }

        public struct AsfCodecListObject
        {
            public Guid ObjectGuid { set; get; }
            public ulong ObjectSize { set; get; }
            public Guid Reserved { set; get; }
            public uint CodecEntriesCount { set; get; }
            public AsfCodecEntryObject[] CodecEntries { set; get; }

            public byte[] RawBlock { set; get; }
        }

        public struct AsfCodecEntryObject
        {
            public ushort CodecType { set; get; }
            public ulong ObjectSize { set; get; }

            public ushort CodecNameLength { set; get; }
            public byte[] CodecNameBytes { set; get; }
            public string CodecName { set; get; }

            public ushort CodecDescriptionLength { set; get; }
            public byte[] CodecDescriptionBytes { set; get; }
            public string CodecDescription { set; get; }

            public ushort CodecInformationLength { set; get; }
            public byte[] CodecInformationBytes { set; get; }
            public string CodecInformation { set; get; }
        }

        public struct AsfDataObject
        {
            public Guid ObjectGuid { set; get; }
            public ulong ObjectSize { set; get; }
            public Guid FileGuid { set; get; }
            public ulong TotalDataPackets { set; get; }
            public ushort Reserved { set; get; }

            public bool IsErrorCorrectionPresent { set; get; }
            public byte ErrorCorrectionLength { set; get; }

            public AsfDataPacketPaylodParsingObject PacketPayloadParsingInfo { set; get; }

            public byte[] RawBlock { set; get; }
        }

        public struct AsfDataPacketPaylodParsingObject
        {
            public byte LengthTypeFlags { set; get; }
            public bool MultiplePayloadsPresent { set; get; }                        
            
            public byte PropertyFlags { set; get; }

            public uint PacketLength { set; get; }
            public uint Sequence { set; get; }
            public uint PaddingLength { set; get; }

            public uint SendTime { set; get; }
            public ushort Duration { set; get; }

            public byte[] RawBlock { set; get; }
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

        public AsfHeaderObject Header { set; get; }
        public AsfDataObject DataOject { set; get; }

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

        private AsfFilePropertiesObject parseAsfFilePropertiesObject(Stream inStream, long currentOffset)
        {
            AsfFilePropertiesObject fileProperties = new AsfFilePropertiesObject();

            fileProperties.ObjectGuid = new Guid(ParseFile.ParseSimpleOffset(inStream, currentOffset, 0x10));
            fileProperties.ObjectSize = BitConverter.ToUInt64(ParseFile.ParseSimpleOffset(inStream, (currentOffset + 0x10), 8), 0);
            fileProperties.FileId = new Guid(ParseFile.ParseSimpleOffset(inStream, currentOffset + 0x18, 0x10));
            fileProperties.FileSize = BitConverter.ToUInt64(ParseFile.ParseSimpleOffset(inStream, (currentOffset + 0x28), 8), 0);
            fileProperties.DataPacketsCount = BitConverter.ToUInt64(ParseFile.ParseSimpleOffset(inStream, (currentOffset + 0x38), 8), 0);
            fileProperties.RawBlock = ParseFile.ParseSimpleOffset(inStream, currentOffset, (int)fileProperties.ObjectSize);

            return fileProperties;
        }

        private AsfStreamPropertiesObject parseAsfStreamPropertiesObject(Stream inStream, long currentOffset)
        {
            AsfStreamPropertiesObject streamProperties = new AsfStreamPropertiesObject();

            streamProperties.ObjectGuid = new Guid(ParseFile.ParseSimpleOffset(inStream, currentOffset, 0x10));
            streamProperties.ObjectSize = BitConverter.ToUInt64(ParseFile.ParseSimpleOffset(inStream, (currentOffset + 0x10), 8), 0);
            streamProperties.StreamType = new Guid(ParseFile.ParseSimpleOffset(inStream, currentOffset + 0x10, 0x10));
            streamProperties.StreamId = (byte)(ParseFile.ParseSimpleOffset(inStream, currentOffset + 0x48, 1)[0] & 0x7E);

            streamProperties.RawBlock = ParseFile.ParseSimpleOffset(inStream, currentOffset, (int)streamProperties.ObjectSize);

            return streamProperties;
        }

        private AsfHeaderExtensionObject parseAsfHeaderExtensionObject(Stream inStream, long currentOffset)
        {
            AsfHeaderExtensionObject headerExtension = new AsfHeaderExtensionObject();

            headerExtension.ObjectGuid = new Guid(ParseFile.ParseSimpleOffset(inStream, currentOffset, 0x10));
            headerExtension.ObjectSize = BitConverter.ToUInt64(ParseFile.ParseSimpleOffset(inStream, (currentOffset + 0x10), 8), 0);

            headerExtension.RawBlock = ParseFile.ParseSimpleOffset(inStream, currentOffset, (int)headerExtension.ObjectSize);

            return headerExtension;
        }

        private AsfCodecListObject parseAsfCodecListObject(Stream inStream, long currentOffset)
        {
            AsfCodecListObject codecList = new AsfCodecListObject();

            codecList.ObjectGuid = new Guid(ParseFile.ParseSimpleOffset(inStream, currentOffset, 0x10));
            codecList.ObjectSize = BitConverter.ToUInt64(ParseFile.ParseSimpleOffset(inStream, (currentOffset + 0x10), 8), 0);
            codecList.RawBlock = ParseFile.ParseSimpleOffset(inStream, currentOffset, (int)codecList.ObjectSize);

            codecList.CodecEntriesCount = BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(inStream, (currentOffset + 0x28), 4), 0);
            codecList.CodecEntries = new AsfCodecEntryObject[codecList.CodecEntriesCount];

            currentOffset += 0x2C;

            for (uint i = 0; i < codecList.CodecEntriesCount; i++)
            {
                codecList.CodecEntries[i] = new AsfCodecEntryObject();
                codecList.CodecEntries[i].CodecType = BitConverter.ToUInt16(ParseFile.ParseSimpleOffset(inStream, currentOffset, 2), 0);

                codecList.CodecEntries[i].CodecNameLength = BitConverter.ToUInt16(ParseFile.ParseSimpleOffset(inStream, currentOffset + 2, 2), 0);
                codecList.CodecEntries[i].CodecNameBytes = ParseFile.ParseSimpleOffset(inStream, currentOffset + 4, (int)(2 * codecList.CodecEntries[i].CodecNameLength));
                codecList.CodecEntries[i].CodecName = Encoding.Unicode.GetString(codecList.CodecEntries[i].CodecNameBytes);

                codecList.CodecEntries[i].CodecDescriptionLength = BitConverter.ToUInt16(ParseFile.ParseSimpleOffset(inStream, (currentOffset + 4 + (2 * codecList.CodecEntries[i].CodecNameLength)), 2), 0);
                codecList.CodecEntries[i].CodecDescriptionBytes = ParseFile.ParseSimpleOffset(inStream, (currentOffset + 6 + (2 * codecList.CodecEntries[i].CodecNameLength)), (int)(2 * codecList.CodecEntries[i].CodecDescriptionLength));
                codecList.CodecEntries[i].CodecDescription = Encoding.Unicode.GetString(codecList.CodecEntries[i].CodecDescriptionBytes);

                codecList.CodecEntries[i].CodecInformationLength = BitConverter.ToUInt16(ParseFile.ParseSimpleOffset(inStream, (currentOffset + 6 + (2 * codecList.CodecEntries[i].CodecNameLength) + (2 * codecList.CodecEntries[i].CodecDescriptionLength)), 2), 0);
                codecList.CodecEntries[i].CodecInformationBytes = ParseFile.ParseSimpleOffset(inStream, (currentOffset + 8 + (2 * codecList.CodecEntries[i].CodecNameLength) + (2 * codecList.CodecEntries[i].CodecDescriptionLength)), (int)codecList.CodecEntries[i].CodecInformationLength);
                // codecList.CodecEntries[i].CodecInformation = Encoding.Unicode.GetString(codecList.CodecEntries[i].CodecInformationBytes);

                codecList.CodecEntries[i].ObjectSize = (8 + (ulong)(2 * codecList.CodecEntries[i].CodecNameLength) +
                                                            (ulong)(2 * codecList.CodecEntries[i].CodecDescriptionLength) +
                                                            (ulong)codecList.CodecEntries[i].CodecInformationLength);

                currentOffset += (long)codecList.CodecEntries[i].ObjectSize;
            }

            return codecList;
        }

        private void parseHeader(Stream inStream, long currentOffset)
        {
            Guid checkGuid;
            ulong checkObjectSize;
            ulong endOfHeaderBlockOffset;

            AsfHeaderObject tempHeader = new AsfHeaderObject();

            tempHeader.ObjectGuid = new Guid(ParseFile.ParseSimpleOffset(inStream, currentOffset, 0x10));
            tempHeader.ObjectSize = BitConverter.ToUInt64(ParseFile.ParseSimpleOffset(inStream, (currentOffset + 0x10), 8), 0);
            tempHeader.NumberOfHeaderObjects = BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(inStream, (currentOffset + 0x18), 4), 0);
            tempHeader.Reserved1 = ParseFile.ParseSimpleOffset(inStream, (currentOffset + 0x1C), 1)[0];
            tempHeader.Reserved2 = ParseFile.ParseSimpleOffset(inStream, (currentOffset + 0x1D), 1)[0];
            tempHeader.RawBlock = ParseFile.ParseSimpleOffset(inStream, currentOffset, (int)tempHeader.ObjectSize);
            endOfHeaderBlockOffset = (ulong)currentOffset + tempHeader.ObjectSize;

            // get additional header objects
            currentOffset += 0x1E;
            
            for (uint i = 0; i < tempHeader.NumberOfHeaderObjects; i++)
            {
                checkGuid = new Guid(ParseFile.ParseSimpleOffset(inStream, currentOffset, 0x10));
                checkObjectSize = BitConverter.ToUInt64(ParseFile.ParseSimpleOffset(inStream, (currentOffset + 0x10), 8), 0);

                if (checkGuid.Equals(MicrosoftAsfContainer.ASF_File_Properties_Object))
                { 
                    tempHeader.FileProperties = this.parseAsfFilePropertiesObject(inStream, currentOffset);
                }
                else if (checkGuid.Equals(MicrosoftAsfContainer.ASF_Stream_Properties_Object))
                { 
                    tempHeader.StreamProperties = this.parseAsfStreamPropertiesObject(inStream, currentOffset);        
                }
                else if (checkGuid.Equals(MicrosoftAsfContainer.ASF_Header_Extension_Object))
                { 
                    // not used at this time
                    tempHeader.HeaderExtension = this.parseAsfHeaderExtensionObject(inStream, currentOffset);
                }
                else if (checkGuid.Equals(MicrosoftAsfContainer.ASF_Codec_List_Object))
                {
                    tempHeader.CodecList = this.parseAsfCodecListObject(inStream, currentOffset);
                }
                
                currentOffset += (long)checkObjectSize;

                // verify we have not gone past the end of the header
                if ((ulong)currentOffset > endOfHeaderBlockOffset)
                {
                    throw new IndexOutOfRangeException(String.Format("Error parsing header, object ending at 0x{0} exceeds header size.{1}", currentOffset.ToString("X"), Environment.NewLine));
                }
            }
            
            // public AsfCodecListObject CodecList { set; get; }

            this.Header = tempHeader;
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
                                this.parseHeader(fs, currentOffset);
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
