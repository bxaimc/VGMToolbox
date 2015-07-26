using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
//using System.Linq;
using System.Text;

using VGMToolbox.format;
using VGMToolbox.plugin;
using VGMToolbox.util;

namespace VGMToolbox.tools.extract
{
    class ExtractCriAcbAwbWorker : AVgmtDragAndDropWorker, IVgmtBackgroundWorker
    {
        public const string CRI_ACBAWB_EXTRACTION_FOLDER = "VGMT_ACB-AWB_EXTRACT_{0}";
        
        public const string AWB_FORMAT1 = "{0}_streamfiles.awb";
        public const string AWB_FORMAT2 = "{0}.awb";

        public const string CUE_NAME_TABLE = "CueNameTable";

        public struct ExtractCriAcbAwbStruct : IVgmtWorkerStruct
        {
            public string[] SourcePaths { set; get; }
            public bool IncludeCueIdInFileName { set; get; }
        }

        public ExtractCriAcbAwbWorker() :
            base() { }

        protected override void DoTaskForFile(string pPath, IVgmtWorkerStruct pExtractStruct, DoWorkEventArgs e)
        {
            ExtractCriAcbAwbStruct extractStruct = (ExtractCriAcbAwbStruct)pExtractStruct;

            Dictionary<string, ushort> cueNamesToWaveforms;

            string extractionDirectoryBase;
            string extractionDirectory;

            using (FileStream fs = File.Open(pPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                this.progressStruct.Clear();
                this.progressStruct.GenericMessage = String.Format("Processing: '{0}'{1}", Path.GetFileName(pPath), Environment.NewLine);
                ReportProgress(Constants.ProgressMessageOnly, this.progressStruct);
                
                // initialize ACB
                CriAcbFile acb = new CriAcbFile(fs, 0);

                // parse Cue Name Table
                if (acb.CueNameTableOffset > 0)
                {
                    // build cue name to waveform map
                    cueNamesToWaveforms = this.getCueNameToWaveformMap(fs, acb, extractStruct);

                    // open AWB file
                    CriAfs2Archive afs2 = this.getAfsArchiveForAcb(acb);
                                        
                    // extract file
                    using (FileStream afs2Fs = File.Open(afs2.SourceFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        extractionDirectoryBase = Path.GetDirectoryName(afs2.SourceFile);
                        extractionDirectory = Path.Combine(extractionDirectoryBase, 
                            String.Format(CRI_ACBAWB_EXTRACTION_FOLDER, Path.GetFileNameWithoutExtension(afs2.SourceFile)));

                        this.progressStruct.Clear();
                        this.progressStruct.GenericMessage = String.Format("  Extracting files.{0}", Environment.NewLine);
                        ReportProgress(Constants.ProgressMessageOnly, this.progressStruct);                        

                        foreach (string fileName in cueNamesToWaveforms.Keys)
                        {
                            ParseFile.ExtractChunkToFile(afs2Fs,
                                afs2.Files[cueNamesToWaveforms[fileName]].FileOffsetByteAligned,
                                afs2.Files[cueNamesToWaveforms[fileName]].FileLength,
                                Path.Combine(extractionDirectory, fileName));

                            //this.progressStruct.Clear();
                            //this.progressStruct.GenericMessage = String.Format("  Extracting: {0}{1}",
                            //    fileName, Environment.NewLine);
                            //ReportProgress(Constants.ProgressMessageOnly, this.progressStruct);                        
                        
                        }
                    }
                }
                else
                {
                    throw new FormatException(String.Format("Error parsing ACB file, <{0}>, cannot find '{1}'", pPath, CUE_NAME_TABLE));
                }

            }            
        }

        protected CriAfs2Archive getAfsArchiveForAcb(CriAcbFile acb)
        {
            CriAfs2Archive afs2 = null;

            string awbDirectory;
            string awbMask;
            string acbBaseFileName;
            string[] awbFiles;

            byte[] awbMd5Calculated;
            string awbMd5HashFromAcb;

            awbDirectory = Path.GetDirectoryName(acb.SourceFile);

            // try format 1
            acbBaseFileName = Path.GetFileNameWithoutExtension(acb.SourceFile);
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
            this.progressStruct.Clear();
            this.progressStruct.GenericMessage = String.Format("  Validating AWB file against ACB checksum.{0}", Environment.NewLine);
            ReportProgress(Constants.ProgressMessageOnly, this.progressStruct);                        
            
            using (FileStream fs = File.Open(awbFiles[0], FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                awbMd5Calculated = ByteConversion.GetBytesFromHexString(ChecksumUtil.GetMd5OfFullFile(fs));

                if (ParseFile.CompareSegment(awbMd5Calculated, 0, acb.StreamAwbHash))
                {
                    this.progressStruct.Clear();
                    this.progressStruct.GenericMessage = String.Format("  AWB checksum OK.{0}", Environment.NewLine);
                    ReportProgress(Constants.ProgressMessageOnly, this.progressStruct);                        

                    afs2 = new CriAfs2Archive(fs, 0);
                }
                else
                {
                    throw new FormatException(String.Format("AWB file, <{0}>, did not match MD5 checksum inside ACB file.", Path.GetFileName(fs.Name)));
                }
            }

            return afs2;
        }

        protected Dictionary<string, ushort> getCueNameToWaveformMap(FileStream fs, CriAcbFile acb, 
            ExtractCriAcbAwbStruct extractStruct)
        {
            Dictionary<string, ushort> cueNameToWaveformDictionary = new Dictionary<string, ushort>();

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
            cueTableUtf.Initialize(fs, (long)acb.CueTableOffset);

            CriUtfTable cueNameTableUtf = new CriUtfTable();
            cueNameTableUtf.Initialize(fs, (long)acb.CueNameTableOffset);

            CriUtfTable waveformTableUtf = new CriUtfTable();
            waveformTableUtf.Initialize(fs, (long)acb.WaveformTableOffset);

            CriUtfTable synthTableUtf = new CriUtfTable();
            synthTableUtf.Initialize(fs, (long)acb.SynthTableOffset);

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
                                        referenceCorrection = 6;
                                        break;
                                    case 3:
                                        referenceCorrection = 2;
                                        break;
                                    default:
                                        throw new FormatException(String.Format("  Unexpected ReferenceType: '{0}' for CueIndex: '{1}.'  Pleae report to VGMToolbox thread at hcs64.com forums, see link in 'Other' menu item.", referenceType.Value.ToString("D"), cueIndex.Value.ToString("D")));
                                        break;
                                }

                                // @TODO: This should be in ReferenceItems.Value
                                waveformIndex = ParseFile.ReadUshortBE(fs, (long)(referenceItemsOffset + referenceCorrection));

                                // get awb id and encode type from corresponding waveform
                                awbId = (ushort?)CriUtfTable.GetUtfFieldForRow(waveformTableUtf, waveformIndex.Value, "Id");
                                encodeType = (byte?)CriUtfTable.GetUtfFieldForRow(waveformTableUtf, waveformIndex.Value, "EncodeType");

                                if (awbId.HasValue && encodeType.HasValue)
                                {
                                    // build cue name
                                    cueName += CriAcbFile.GetFileExtensionForEncodeType(encodeType.Value);

                                    if (extractStruct.IncludeCueIdInFileName)
                                    {
                                        cueName = String.Format("{0}_{1}", cueId.Value.ToString("D5"), cueName);
                                    }

                                    // add to Dictionary
                                    cueNameToWaveformDictionary.Add(cueName, awbId.Value);

                                }
                                else
                                {
                                    this.progressStruct.Clear();
                                    this.progressStruct.GenericMessage = String.Format("  Warning, WaveformIndex entry not found for CueIndex: '{0}' ({1}) with ReferenceItemsOffset and Reference Type: '0x{2}', '{3}'...Skipping.{4}", cueIndex.Value.ToString("D"), cueName, referenceItemsOffset.ToString("X8"), referenceType.Value.ToString("D"), Environment.NewLine);
                                    ReportProgress(Constants.ProgressMessageOnly, this.progressStruct);
                                } // if (waveformIndex.HasValue)
                            }
                            else
                            {
                                this.progressStruct.Clear();
                                this.progressStruct.GenericMessage = String.Format("  Warning, SynthTable entry not found for CueIndex: '{0}' ({1}) with ReferenceId '{2}'...Skipping.{3}", cueIndex.Value.ToString("D"), cueName, referenceId.Value.ToString("D"), Environment.NewLine);
                                ReportProgress(Constants.ProgressMessageOnly, this.progressStruct);

                            } // if (referenceItemsOffset.HasValue)
                        }
                        else
                        {
                            this.progressStruct.Clear();
                            this.progressStruct.GenericMessage = String.Format("  Warning, CueTable entry not found for CueIndex: '{0}' ({1})...Skipping.{2}", cueIndex.Value.ToString("D"), cueName, Environment.NewLine);
                            ReportProgress(Constants.ProgressMessageOnly, this.progressStruct);

                        } // if (cueId != null && referenceType != null && referenceId != null)
                    }
                    else
                    {
                        this.progressStruct.Clear();
                        this.progressStruct.GenericMessage = String.Format("  Warning, CueIndex or CueName entry not found for CueNameTable index: '{0}'...Skipping.{1}", i.ToString("D"), Environment.NewLine);
                        ReportProgress(Constants.ProgressMessageOnly, this.progressStruct);
                    } // if (cueIndex.HasValue && !String.IsNullOrEmpty(cueName))
                }
                catch (Exception ex)
                {
                    throw new FormatException(String.Format("  Error mapping CueName to Waveform for CueIndex: {0}: {1}", cueIndex.Value.ToString("D"), ex.Message));
                }
            }

            return cueNameToWaveformDictionary;
        }

        
    }
}
