using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using VGMToolbox.util;

namespace VGMToolbox.format
{
    public class ElectronicArtsVp6Stream : MpegStream
    {
        public const string DefaultAudioExtension = ".sns";
        public const string DefaultVideoExtension = ".vp6";

        protected static readonly byte[] MVHD_BYTES = new byte[] { 0x4D, 0x56, 0x68, 0x64 };
        protected static readonly byte[] MV0F_BYTES = new byte[] { 0x4D, 0x56, 0x30, 0x46 };
        protected static readonly byte[] MV0K_BYTES = new byte[] { 0x4D, 0x56, 0x30, 0x4B };        
        protected static readonly byte[] MVxx_BYTES = new byte[] { 0x4D, 0x56 };

        protected static readonly byte[] SCHl_BYTES = new byte[] { 0x53, 0x43, 0x48, 0x6C };
        protected static readonly byte[] SCCl_BYTES = new byte[] { 0x53, 0x43, 0x43, 0x6C };
        protected static readonly byte[] SCDl_BYTES = new byte[] { 0x53, 0x43, 0x44, 0x6C };
        protected static readonly byte[] SCLl_BYTES = new byte[] { 0x53, 0x43, 0x4C, 0x6C };
        protected static readonly byte[] SCEl_BYTES = new byte[] { 0x53, 0x43, 0x45, 0x6C };
        protected static readonly byte[] SCxx_BYTES = new byte[] { 0x53, 0x43 };

        protected static readonly byte[] SHEN_BYTES = new byte[] { 0x53, 0x48, 0x45, 0x4E };
        protected static readonly byte[] SCEN_BYTES = new byte[] { 0x53, 0x43, 0x45, 0x4E };
        protected static readonly byte[] SDEN_BYTES = new byte[] { 0x53, 0x44, 0x45, 0x4E };
        protected static readonly byte[] SEEN_BYTES = new byte[] { 0x53, 0x45, 0x45, 0x4E };
        protected static readonly byte[] SHxx_BYTES = new byte[] { 0x53, 0x48 };


        public ElectronicArtsVp6Stream(string path)
            : base(path)
        {
            this.UsesSameIdForMultipleAudioTracks = false;
            this.BlockSizeIsLittleEndian = true;
            this.FileExtensionAudio = DefaultAudioExtension;
            this.FileExtensionVideo = DefaultVideoExtension;

            base.BlockIdDictionary.Clear();
            
            // video blocks
            base.BlockIdDictionary[BitConverter.ToUInt32(MVHD_BYTES, 0)] = new BlockSizeStruct(PacketSizeType.SizeBytes, 4); 
            base.BlockIdDictionary[BitConverter.ToUInt32(MV0F_BYTES, 0)] = new BlockSizeStruct(PacketSizeType.SizeBytes, 4); 
            base.BlockIdDictionary[BitConverter.ToUInt32(MV0K_BYTES, 0)] = new BlockSizeStruct(PacketSizeType.SizeBytes, 4); 
            
            // audio blocks            
            base.BlockIdDictionary[BitConverter.ToUInt32(SCHl_BYTES, 0)] = new BlockSizeStruct(PacketSizeType.SizeBytes, 4); 
            base.BlockIdDictionary[BitConverter.ToUInt32(SCCl_BYTES, 0)] = new BlockSizeStruct(PacketSizeType.SizeBytes, 4); 
            base.BlockIdDictionary[BitConverter.ToUInt32(SCDl_BYTES, 0)] = new BlockSizeStruct(PacketSizeType.SizeBytes, 4); 
            base.BlockIdDictionary[BitConverter.ToUInt32(SCLl_BYTES, 0)] = new BlockSizeStruct(PacketSizeType.SizeBytes, 4); 
            base.BlockIdDictionary[BitConverter.ToUInt32(SCEl_BYTES, 0)] = new BlockSizeStruct(PacketSizeType.SizeBytes, 4); 

            base.BlockIdDictionary[BitConverter.ToUInt32(SHEN_BYTES, 0)] = new BlockSizeStruct(PacketSizeType.SizeBytes, 4); 
            base.BlockIdDictionary[BitConverter.ToUInt32(SCEN_BYTES, 0)] = new BlockSizeStruct(PacketSizeType.SizeBytes, 4); 
            base.BlockIdDictionary[BitConverter.ToUInt32(SDEN_BYTES, 0)] = new BlockSizeStruct(PacketSizeType.SizeBytes, 4); 
            base.BlockIdDictionary[BitConverter.ToUInt32(SEEN_BYTES, 0)] = new BlockSizeStruct(PacketSizeType.SizeBytes, 4); 
        }

        protected override byte[] GetPacketStartBytes() { return ElectronicArtsVp6Stream.MVHD_BYTES; }

        protected override int GetAudioPacketHeaderSize(Stream readStream, long currentOffset)
        {
            return 0;
        }
        protected override int GetVideoPacketHeaderSize(Stream readStream, long currentOffset)
        {
            return 0;
        }

        protected override bool IsThisAnAudioBlock(byte[] blockToCheck)
        {
            return (ParseFile.CompareSegment(blockToCheck, 0, ElectronicArtsVp6Stream.SCxx_BYTES) ||
                    ParseFile.CompareSegment(blockToCheck, 0, ElectronicArtsVp6Stream.SHxx_BYTES));
        }
        protected override bool IsThisAVideoBlock(byte[] blockToCheck)
        {
            // not sure if specific blocks will be needed
            return ParseFile.CompareSegment(blockToCheck, 0, ElectronicArtsVp6Stream.MVxx_BYTES);
        }

        protected override void DoFinalTasks(FileStream sourceFileStream, Dictionary<uint, FileStream> outputFiles, bool addHeader)
        {
            foreach (uint streamId in outputFiles.Keys)
            {
                outputFiles[streamId].Close();
                outputFiles[streamId].Dispose();
            }
        }

    }

}
