using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using VGMToolbox.util;

namespace VGMToolbox.format
{
    public class CriAcbFile : CriUtfTable
    {
        public const string EXTRACTION_FOLDER_FORMAT = "VGMT_ACB_EXT_{0}";
        
        public const byte WAVEFORM_ENCODE_TYPE_ADX = 0;
        public const byte WAVEFORM_ENCODE_TYPE_HCA = 2;
        public const byte WAVEFORM_ENCODE_TYPE_ATRAC3 = 8;
        public const byte WAVEFORM_ENCODE_TYPE_BCWAV = 9;
        public const byte WAVEFORM_ENCODE_TYPE_NINTENDO_DSP = 13;

        public const string AWB_FORMAT1 = "{0}_streamfiles.awb";
        public const string AWB_FORMAT2 = "{0}.awb";

        protected enum AwbToExtract { Internal, External };

        public CriAcbFile(FileStream fs, long offset)
        {
            this.Initialize(fs, offset);

            // @TODO: Make CueId field a parameter
            this.GetCueNameToWaveformMap(fs, true);

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

        

        protected void GetCueNameToWaveformMap(FileStream fs, bool includeCueIdInFileName)
        {
            this.CueNamesToWaveforms = new Dictionary<string, ushort>();

            long fileSize = fs.Length;

            // CueName
            string cueName;
            ushort? cueIndex = ushort.MaxValue;

            // Cue
            uint? cueId;
            byte? referenceType;
            ushort? referenceId;

            // Synth
            ulong referenceItemsOffset;
            ulong referenceCorrection;
            ushort? waveformIndex;

            // Waveform
            ushort? awbId;
            byte? encodeType;

            CriUtfTable cueTableUtf = new CriUtfTable();
            cueTableUtf.Initialize(fs, (long)this.CueTableOffset);

            CriUtfTable cueNameTableUtf = new CriUtfTable();
            cueNameTableUtf.Initialize(fs, (long)this.CueNameTableOffset);

            CriUtfTable waveformTableUtf = new CriUtfTable();
            waveformTableUtf.Initialize(fs, (long)this.WaveformTableOffset);

            CriUtfTable synthTableUtf = new CriUtfTable();
            synthTableUtf.Initialize(fs, (long)this.SynthTableOffset);

            // Loop over cue names
            for (int i = 0; i < cueNameTableUtf.NumberOfRows; i++)
            {
                // this is the basic mapping
                // WaveFormTable[(ushort)*(&SynthTable[CueTable[CueNameTable.CueIndex].ReferenceIndex].ReferenceItems + referenceCorrection)].Id

                try
                {
                    // get cue index and cue name
                    cueIndex = (ushort?)CriUtfTable.GetUtfFieldForRow(cueNameTableUtf, i, "CueIndex");
                    cueName = (string)CriUtfTable.GetUtfFieldForRow(cueNameTableUtf, i, "CueName");

                    if (cueIndex.HasValue && !String.IsNullOrEmpty(cueName))
                    {
                        // lookup cue index and get reference id
                        cueId = (uint?)CriUtfTable.GetUtfFieldForRow(cueTableUtf, cueIndex.Value, "CueId");
                        referenceType = (byte?)CriUtfTable.GetUtfFieldForRow(cueTableUtf, cueIndex.Value, "ReferenceType");
                        referenceId = (ushort?)CriUtfTable.GetUtfFieldForRow(cueTableUtf, cueIndex.Value, "ReferenceIndex");

                        if (cueId.HasValue && referenceType.HasValue && referenceId.HasValue)
                        {
                            // lookup reference items offset for corresponding synth
                            referenceItemsOffset = (ulong)CriUtfTable.GetOffsetForUtfFieldForRow(synthTableUtf, referenceId.Value, "ReferenceItems");

                            if (referenceItemsOffset > 0)
                            {
                                switch (referenceType)
                                {
                                    case 2:
                                    case 8:
                                        referenceCorrection = 6;
                                        break;
                                    case 3:
                                        referenceCorrection = 2;
                                        break;
                                    default:
                                        throw new FormatException(String.Format("  Unexpected ReferenceType: '{0}' for CueIndex: '{1}.'  Please report to VGMToolbox thread at hcs64.com forums, see link in 'Other' menu item.", referenceType.Value.ToString("D"), cueIndex.Value.ToString("D")));
                                        break;
                                }

                                // @TODO: This should be in ReferenceItems.Value
                                waveformIndex = ParseFile.ReadUshortBE(fs, (long)(referenceItemsOffset + referenceCorrection));
                                // waveformIndex = ParseFile.ReadUshortBE((byte[])CriUtfTable.GetUtfFieldForRow(synthTableUtf, referenceId.Value, "ReferenceItems"), (long)referenceCorrection);

                                // get awb id and encode type from corresponding waveform
                                awbId = (ushort?)CriUtfTable.GetUtfFieldForRow(waveformTableUtf, waveformIndex.Value, "Id");
                                encodeType = (byte?)CriUtfTable.GetUtfFieldForRow(waveformTableUtf, waveformIndex.Value, "EncodeType");

                                if (awbId.HasValue && encodeType.HasValue)
                                {
                                    // build cue name
                                    cueName += CriAcbFile.GetFileExtensionForEncodeType(encodeType.Value);

                                    if (includeCueIdInFileName)
                                    {
                                        cueName = String.Format("{0}_{1}", cueId.Value.ToString("D5"), cueName);
                                    }

                                    // add to Dictionary
                                    CueNamesToWaveforms.Add(cueName, awbId.Value);

                                }
                                else
                                {
                                    //this.progressStruct.Clear();
                                    //this.progressStruct.GenericMessage = String.Format("  Warning, WaveformIndex entry not found for CueIndex: '{0}' ({1}) with ReferenceItemsOffset and Reference Type: '0x{2}', '{3}'...Skipping.{4}", cueIndex.Value.ToString("D"), cueName, referenceItemsOffset.ToString("X8"), referenceType.Value.ToString("D"), Environment.NewLine);
                                    //ReportProgress(Constants.ProgressMessageOnly, this.progressStruct);
                                } // if (waveformIndex.HasValue)
                            }
                            else
                            {
                                //this.progressStruct.Clear();
                                //this.progressStruct.GenericMessage = String.Format("  Warning, SynthTable entry not found for CueIndex: '{0}' ({1}) with ReferenceId '{2}'...Skipping.{3}", cueIndex.Value.ToString("D"), cueName, referenceId.Value.ToString("D"), Environment.NewLine);
                                //ReportProgress(Constants.ProgressMessageOnly, this.progressStruct);

                            } // if (referenceItemsOffset.HasValue)
                        }
                        else
                        {
                            //this.progressStruct.Clear();
                            //this.progressStruct.GenericMessage = String.Format("  Warning, CueTable entry not found for CueIndex: '{0}' ({1})...Skipping.{2}", cueIndex.Value.ToString("D"), cueName, Environment.NewLine);
                            //ReportProgress(Constants.ProgressMessageOnly, this.progressStruct);

                        } // if (cueId != null && referenceType != null && referenceId != null)
                    }
                    else
                    {
                        //this.progressStruct.Clear();
                        //this.progressStruct.GenericMessage = String.Format("  Warning, CueIndex or CueName entry not found for CueNameTable index: '{0}'...Skipping.{1}", i.ToString("D"), Environment.NewLine);
                        //ReportProgress(Constants.ProgressMessageOnly, this.progressStruct);
                    } // if (cueIndex.HasValue && !String.IsNullOrEmpty(cueName))
                }
                catch (Exception ex)
                {
                    throw new FormatException(String.Format("  Error mapping CueName to Waveform for CueIndex: {0}: {1}", cueIndex.Value.ToString("D"), ex.Message));
                }
            }
        }

        protected void ExtractInternalAwb(string destinationFolder)
        {
            // check internal awb
            if (this.InternalAwb != null)
            {
                if (this.StreamAwbAfs2HeaderSize == 0)
                {
                    this.ExtractAwbWithCueNames(destinationFolder, AwbToExtract.Internal);
                }
                else
                { 
                    // use AFS2 IDs
                    this.InternalAwb.ExtractAllRaw(destinationFolder);
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
