using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace VGMToolbox.format
{
    public class CriAcbFile : CriUtfTable
    {
        public const byte WAVEFORM_ENCODE_TYPE_ADX = 0;
        public const byte WAVEFORM_ENCODE_TYPE_HCA = 2;
        public const byte WAVEFORM_ENCODE_TYPE_ATRAC3 = 8;
        public const byte WAVEFORM_ENCODE_TYPE_BCWAV = 9;
        
        public CriAcbFile(FileStream fs, long offset)
        {
            this.Initialize(fs, offset);
        }

        public string Name 
        {
            get 
            { 
                return (string)CriUtfTable.GetUtfFieldForRow(this, 0, "Name"); 
            }
        }
        public string VersionString
        {
            get
            {
                return (string)CriUtfTable.GetUtfFieldForRow(this, 0, "VersionString");
            }
        }

        public ulong InternalAwbFileOffset
        {
            get 
            {
                return (ulong)CriUtfTable.GetOffsetForUtfFieldForRow(this, 0, "AwbFile");                
            }
        
        }

        public ulong CueTableOffset
        {
            get
            {
                return CriUtfTable.GetOffsetForUtfFieldForRow(this, 0, "CueTable");
            }
        }
        public ulong CueNameTableOffset
        {
            get
            {
                return CriUtfTable.GetOffsetForUtfFieldForRow(this, 0, "CueNameTable");
            }
        }
        public ulong WaveformTableOffset
        {
            get
            {
                return CriUtfTable.GetOffsetForUtfFieldForRow(this, 0, "WaveformTable");
            }
        }
        public ulong SynthTableOffset
        {
            get
            {
                return CriUtfTable.GetOffsetForUtfFieldForRow(this, 0, "SynthTable");
            }
        }

        public byte[] StreamAwbHash
        {
            get 
            {
                return (byte[])CriUtfTable.GetUtfFieldForRow(this, 0, "StreamAwbHash");
            }
        }
        public ulong StreamAwbAfs2HeaderOffset
        {
            get
            {
                return (ulong)CriUtfTable.GetOffsetForUtfFieldForRow(this, 0, "StreamAwbAfs2Header");
            }
        }
        public ulong StreamAwbAfs2HeaderSize
        {
            get
            {
                return (ulong)CriUtfTable.GetSizeForUtfFieldForRow(this, 0, "StreamAwbAfs2Header");
            }
        }

        public static string GetFileExtensionForEncodeType(byte encodeType)
        {
            string ext;

            switch (encodeType)
            {
                case WAVEFORM_ENCODE_TYPE_ADX:
                    ext = ".adx";
                    break;
                case WAVEFORM_ENCODE_TYPE_HCA:
                    ext = ".hca";
                    break;
                case WAVEFORM_ENCODE_TYPE_ATRAC3:
                    ext = ".at3";
                    break;
                case WAVEFORM_ENCODE_TYPE_BCWAV:
                    ext = ".bcwav";
                    break;
                default:
                    ext = String.Format(".EncodeType-{0}.bin", encodeType.ToString("D2"));
                    break;
            }

            return ext;
        }
    }
}
