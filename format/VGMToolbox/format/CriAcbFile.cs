using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using VGMToolbox.util;

namespace VGMToolbox.format
{
    public class CriAcbCueRecord
    {
        public uint CueId { set; get; }
        public byte ReferenceType { set; get; }
        public ushort ReferenceIndex { set; get; }

        public bool IsWaveformIdentified { set; get; }
        public ushort WaveformIndex { set; get; }
        public ushort WaveformId { set; get; }
        public byte EncodeType { set; get; }

        public string CueName { set; get; }
    }
    
    public class CriAcbFile : CriUtfTable
    {
        public const string EXTRACTION_FOLDER_FORMAT = "VGMT_ACB_EXT_{0}";
        
        public const byte WAVEFORM_ENCODE_TYPE_ADX = 0;
        public const byte WAVEFORM_ENCODE_TYPE_HCA = 2;
        public const byte WAVEFORM_ENCODE_TYPE_VAG = 7;
        public const byte WAVEFORM_ENCODE_TYPE_ATRAC3 = 8;
        public const byte WAVEFORM_ENCODE_TYPE_BCWAV = 9;
        public const byte WAVEFORM_ENCODE_TYPE_NINTENDO_DSP = 13;

        public const string AWB_FORMAT1 = "{0}_streamfiles.awb";
        public const string AWB_FORMAT2 = "{0}.awb";

        protected enum AwbToExtract { Internal, External };

        public CriAcbFile(FileStream fs, long offset, bool includeCueIdInFileName)
        {
            // initialize UTF
            this.Initialize(fs, offset);
            
            // initialize ACB specific items
            this.InitializeCueList(fs);
            this.InitializeCueNameToWaveformMap(fs, includeCueIdInFileName);
                       
            // initialize internal AWB
            if (this.InternalAwbFileSize > 0)
            {
                this.InternalAwb = new CriAfs2Archive(fs, (long)this.InternalAwbFileOffset);
            }

            // initialize external AWB
            if (this.StreamAwbAfs2HeaderSize > 0)
            {
                this.ExternalAwb = this.InitializeExternalAwbArchive();
            }
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
        public ulong InternalAwbFileSize
        {
            get
            {
                return (ulong)CriUtfTable.GetSizeForUtfFieldForRow(this, 0, "AwbFile");
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

        public CriAcbCueRecord[] CueList { set; get; }
        public Dictionary<string, ushort> CueNamesToWaveforms { set; get; }

        public byte[] AcfMd5Hash
        {
            get
            {
                return (byte[])CriUtfTable.GetUtfFieldForRow(this, 0, "AcfMd5Hash");
            }        
        }
        public ulong AwbFileOffset
        {
            get
            {
                return CriUtfTable.GetOffsetForUtfFieldForRow(this, 0, "AwbFile");
            }        
        }
        public CriAfs2Archive InternalAwb { set; get; }

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
        public CriAfs2Archive ExternalAwb { set; get; }

        public void ExtractAll()
        { 
            string baseExtractionFolder = Path.Combine(Path.GetDirectoryName(this.SourceFile),
                                                       String.Format(EXTRACTION_FOLDER_FORMAT, Path.GetFileNameWithoutExtension(this.SourceFile)));
        
            string acbExtractionFolder = Path.Combine(baseExtractionFolder, "acb");
            string internalAwbExtractionFolder = Path.Combine(acbExtractionFolder, "awb");
            string awbExtractionFolder = Path.Combine(baseExtractionFolder, "awb");
        
            
            // this.ExtractInternalAwb(internalAwbExtractionFolder);
            
            // extract internal awb
            if (this.InternalAwb != null)
            {
                if (this.ExternalAwb == null)
                {
                    // use cue names since no AWB exists
                    this.ExtractAwbWithCueNames(internalAwbExtractionFolder, AwbToExtract.Internal);
                }
                else
                {
                    // use AFS2 IDs, since cue names will apply to external AWB (@TODO: Does a flag exist?)
                    this.InternalAwb.ExtractAllRaw(internalAwbExtractionFolder);
                }
            }

            if (this.ExternalAwb != null)
            {
                this.ExtractAwbWithCueNames(awbExtractionFolder, AwbToExtract.External);
            }
        }

        
        protected void InitializeCueNameToWaveformMap(FileStream fs, bool includeCueIdInFileName)
        {
            ushort cueIndex;
            string cueName;
                        
            CriUtfTable cueNameTableUtf = new CriUtfTable();
            cueNameTableUtf.Initialize(fs, (long)this.CueNameTableOffset);

            for (int i = 0; i < cueNameTableUtf.NumberOfRows; i++)
            {
                cueIndex = (ushort)CriUtfTable.GetUtfFieldForRow(cueNameTableUtf, i, "CueIndex");

                // skip cues with unidentified waveforms (see 'vc05_0140.acb, vc10_0372.acb' in Kidou_Senshi_Gundam_AGE_Universe_Accel (PSP))
                if (this.CueList[cueIndex].IsWaveformIdentified)
                {
                    cueName = (string)CriUtfTable.GetUtfFieldForRow(cueNameTableUtf, i, "CueName");

                    this.CueList[cueIndex].CueName = cueName;
                    this.CueList[cueIndex].CueName += CriAcbFile.GetFileExtensionForEncodeType(this.CueList[cueIndex].EncodeType);

                    if (includeCueIdInFileName)
                    {
                        this.CueList[cueIndex].CueName = String.Format("{0}_{1}",
                            this.CueList[cueIndex].CueId.ToString("D5"), this.CueList[cueIndex].CueName);
                    }

                    this.CueNamesToWaveforms.Add(this.CueList[cueIndex].CueName, this.CueList[cueIndex].WaveformId);
                }
            }              
        }

        protected void InitializeCueList(FileStream fs)
        {
            this.CueNamesToWaveforms = new Dictionary<string, ushort>();

            ulong referenceItemsOffset = 0;
            ulong referenceItemsSize = 0;
            ulong referenceCorrection = 0;

            CriUtfTable cueTableUtf = new CriUtfTable();
            cueTableUtf.Initialize(fs, (long)this.CueTableOffset);

            CriUtfTable waveformTableUtf = new CriUtfTable();
            waveformTableUtf.Initialize(fs, (long)this.WaveformTableOffset);

            CriUtfTable synthTableUtf = new CriUtfTable();
            synthTableUtf.Initialize(fs, (long)this.SynthTableOffset);

            this.CueList = new CriAcbCueRecord[cueTableUtf.NumberOfRows];

            for (int i = 0; i < cueTableUtf.NumberOfRows; i++)
            {
                this.CueList[i] = new CriAcbCueRecord();
                this.CueList[i].IsWaveformIdentified = false;

                this.CueList[i].CueId = (uint)CriUtfTable.GetUtfFieldForRow(cueTableUtf, i, "CueId");
                this.CueList[i].ReferenceType = (byte)CriUtfTable.GetUtfFieldForRow(cueTableUtf, i, "ReferenceType");
                this.CueList[i].ReferenceIndex = (ushort)CriUtfTable.GetUtfFieldForRow(cueTableUtf, i, "ReferenceIndex");

                switch (this.CueList[i].ReferenceType)
                {
                    case 2:
                        referenceItemsOffset = (ulong)CriUtfTable.GetOffsetForUtfFieldForRow(synthTableUtf, this.CueList[i].ReferenceIndex, "ReferenceItems");
                        referenceItemsSize = CriUtfTable.GetSizeForUtfFieldForRow(synthTableUtf, this.CueList[i].ReferenceIndex, "ReferenceItems");
                        referenceCorrection = referenceItemsSize + 2;
                        break;
                    case 3:
                    case 8:
                        if (i == 0) 
                        {
                            referenceItemsOffset = (ulong)CriUtfTable.GetOffsetForUtfFieldForRow(synthTableUtf, 0, "ReferenceItems");
                            referenceItemsSize = CriUtfTable.GetSizeForUtfFieldForRow(synthTableUtf, 0, "ReferenceItems");
                            referenceCorrection = referenceItemsSize - 2; // samples found have only had a '01 00' record => Always Waveform[0]?.                       
                        }
                        else
                        {
                            referenceCorrection += 4; // relative to previous offset, do not lookup
                        }
                        break;
                    default:
                        throw new FormatException(String.Format("  Unexpected ReferenceType: '{0}' for CueIndex: '{1}.'  Please report to VGMToolbox thread at hcs64.com forums, see link in 'Other' menu item.", this.CueList[i].ReferenceType.ToString("D"), i.ToString("D")));
                        break;
                }

                if (referenceItemsSize != 0)
                {
                    // get wave form info
                    this.CueList[i].WaveformIndex = ParseFile.ReadUshortBE(fs, (long)(referenceItemsOffset + referenceCorrection));

                    // get waveform id and encode type from corresponding waveform
                    this.CueList[i].WaveformId = (ushort)CriUtfTable.GetUtfFieldForRow(waveformTableUtf, this.CueList[i].WaveformIndex, "Id");
                    this.CueList[i].EncodeType = (byte)CriUtfTable.GetUtfFieldForRow(waveformTableUtf, this.CueList[i].WaveformIndex, "EncodeType");
                    
                    // update flag
                    this.CueList[i].IsWaveformIdentified = true;
                }                
            }
        }

        protected void ExtractAwbWithCueNames(string destinationFolder, AwbToExtract whichAwb)
        {
            CriUtfTable waveformTableUtf;
            byte encodeType;
            string rawFileName;
            
            CriAfs2Archive awb;
            string rawFileFormat = "{0}_{1}{2}";

            if (whichAwb == AwbToExtract.Internal)
            {
                awb = this.InternalAwb;
            }
            else
            {
                awb = this.ExternalAwb;
            }

            using (FileStream fs = File.Open(awb.SourceFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                // use files names for internal AWB
                foreach (string key in this.CueNamesToWaveforms.Keys)
                {
                    // extract file
                    ParseFile.ExtractChunkToFile64(fs,
                        (ulong)awb.Files[this.CueNamesToWaveforms[key]].FileOffsetByteAligned,
                        (ulong)awb.Files[this.CueNamesToWaveforms[key]].FileLength,
                        Path.Combine(destinationFolder, FileUtil.CleanFileName(key)), false, false);
                }

                //-------------------------------------
                // extract any items without a CueName
                //-------------------------------------
                using (FileStream acbStream = File.Open(this.SourceFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    waveformTableUtf = new CriUtfTable();
                    waveformTableUtf.Initialize(acbStream, (long)this.WaveformTableOffset);
                }

                // get list of unextracted files
                var unextractedFiles = awb.Files.Keys.Where(x => !this.CueNamesToWaveforms.ContainsValue(x));
                foreach (ushort key in unextractedFiles)
                {
                    encodeType = (byte)CriUtfTable.GetUtfFieldForRow(waveformTableUtf, key, "EncodeType");

                    rawFileName = String.Format(rawFileFormat,
                        Path.GetFileNameWithoutExtension(awb.SourceFile), key.ToString("D5"), 
                        CriAcbFile.GetFileExtensionForEncodeType(encodeType));

                    // extract file
                    ParseFile.ExtractChunkToFile64(fs,
                        (ulong)awb.Files[key].FileOffsetByteAligned,
                        (ulong)awb.Files[key].FileLength,
                        Path.Combine(destinationFolder, rawFileName), false, false);
                }
            }
        }

        protected CriAfs2Archive InitializeExternalAwbArchive()
        {
            CriAfs2Archive afs2 = null;

            string awbDirectory;
            string awbMask;
            string acbBaseFileName;
            string[] awbFiles;

            byte[] awbMd5Calculated;

            awbDirectory = Path.GetDirectoryName(this.SourceFile);

            // try format 1
            acbBaseFileName = Path.GetFileNameWithoutExtension(this.SourceFile);
            awbMask = String.Format(AWB_FORMAT1, acbBaseFileName);
            awbFiles = Directory.GetFiles(awbDirectory, awbMask, SearchOption.TopDirectoryOnly);

            if (awbFiles.Length < 1)
            {
                // try format 2
                awbMask = String.Format(AWB_FORMAT2, acbBaseFileName);
                awbFiles = Directory.GetFiles(awbDirectory, awbMask, SearchOption.TopDirectoryOnly);
            }

            // file not found
            if (awbFiles.Length < 1)
            {
                throw new FileNotFoundException(String.Format("Cannot find AWB file. Please verify corresponding AWB file is named '{0}' or '{1}'.",
                    String.Format(AWB_FORMAT1, acbBaseFileName), String.Format(AWB_FORMAT2, acbBaseFileName)));
            }

            if (awbFiles.Length > 1)
            {
                throw new FileNotFoundException(String.Format("More than one matching AWB file for this ACB. Please verify only one AWB file is named '{1}' or '{2}'.",
                    String.Format(AWB_FORMAT1, acbBaseFileName), String.Format(AWB_FORMAT2, acbBaseFileName)));
            }

            // initialize AFS2 file                        
            using (FileStream fs = File.Open(awbFiles[0], FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                // validate MD5 checksum
                awbMd5Calculated = ByteConversion.GetBytesFromHexString(ChecksumUtil.GetMd5OfFullFile(fs));

                if (ParseFile.CompareSegment(awbMd5Calculated, 0, this.StreamAwbHash))
                {
                    afs2 = new CriAfs2Archive(fs, 0);
                }
                else
                {
                    throw new FormatException(String.Format("AWB file, <{0}>, did not match MD5 checksum inside ACB file.", Path.GetFileName(fs.Name)));
                }
            }

            return afs2;
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
                case WAVEFORM_ENCODE_TYPE_VAG:
                    ext = ".vag";
                    break;
                case WAVEFORM_ENCODE_TYPE_BCWAV:
                    ext = ".bcwav";
                    break;
                case WAVEFORM_ENCODE_TYPE_NINTENDO_DSP:
                    ext = ".dsp";
                    break;
                default:
                    ext = String.Format(".EncodeType-{0}.bin", encodeType.ToString("D2"));
                    break;
            }

            return ext;
        }
    }
}
