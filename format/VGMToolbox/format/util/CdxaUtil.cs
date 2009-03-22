using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using VGMToolbox.format;
using VGMToolbox.util;

namespace VGMToolbox.format.util
{
    public class CdxaUtil
    {
        public struct ExtractXaStruct
        {
            public string Path;
            public bool AddRiffHeader;
            public bool PatchByte0x11;
        }

        public static void ExtractXaFiles(ExtractXaStruct pExtractXaStruct)
        { 
            Dictionary<UInt32, BinaryWriter> bwDictionary = new Dictionary<UInt32, BinaryWriter>();

            long offset;
            byte[] trackId;
            byte[] buffer = new byte[Cdxa.XA_BLOCK_SIZE];
            UInt32 trackKey;
            UInt32 xaFileSize;

            string outputFileName;
            string outputDirectory = Path.Combine(Path.GetDirectoryName(pExtractXaStruct.Path),
                Path.GetFileNameWithoutExtension(pExtractXaStruct.Path));

            using (FileStream fs = File.OpenRead(pExtractXaStruct.Path))
            {                
                // get first offset to start the party
                offset = ParseFile.GetNextOffset(fs, 0, Cdxa.XA_SIG);

                if (offset != -1) // we have found an XA sig
                {
                    if (!Directory.Exists(outputDirectory))
                    {
                        Directory.CreateDirectory(outputDirectory);
                    }

                    while ((offset + Cdxa.XA_BLOCK_SIZE) <= fs.Length)
                    {
                        trackId = ParseFile.parseSimpleOffset(fs, offset + Cdxa.XA_TRACK_OFFSET, Cdxa.XA_TRACK_SIZE);
                        trackKey = BitConverter.ToUInt32(trackId, 0);

                        if (!bwDictionary.ContainsKey(trackKey))
                        {
                            outputFileName = Path.Combine(outputDirectory, Path.GetFileNameWithoutExtension(pExtractXaStruct.Path) + "_" + ParseFile.ByteArrayToString(trackId) + Cdxa.XA_FILE_EXTENSION);
                            // outputFileName = Path.Combine(outputDirectory, ParseFile.ByteArrayToString(trackId) + Cdxa.XA_FILE_EXTENSION);
                            bwDictionary.Add(trackKey, new BinaryWriter(File.Open(outputFileName, FileMode.Create, FileAccess.Write)));

                            if (pExtractXaStruct.AddRiffHeader)
                            {
                                bwDictionary[trackKey].Write(Cdxa.XA_RIFF_HEADER);
                            }
                        }

                        if (!pExtractXaStruct.PatchByte0x11)
                        {
                            bwDictionary[trackKey].Write(ParseFile.parseSimpleOffset(fs, offset, Cdxa.XA_BLOCK_SIZE));
                        }
                        else
                        {
                            buffer = ParseFile.parseSimpleOffset(fs, offset, Cdxa.XA_BLOCK_SIZE);
                            buffer[0x11] = 0x00;
                            bwDictionary[trackKey].Write(buffer);
                        }

                        offset += Cdxa.XA_BLOCK_SIZE;
                    }

                    // fix header and close writers
                    foreach (UInt32 keyname in bwDictionary.Keys)
                    {
                        if (pExtractXaStruct.AddRiffHeader)
                        {
                            xaFileSize = (UInt32)bwDictionary[keyname].BaseStream.Length;

                            // add file size
                            bwDictionary[keyname].BaseStream.Position = Cdxa.FILESIZE_OFFSET;
                            bwDictionary[keyname].Write(BitConverter.GetBytes(xaFileSize - 8));

                            // add data size
                            bwDictionary[keyname].BaseStream.Position = Cdxa.DATA_OFFSET;
                            bwDictionary[keyname].Write(BitConverter.GetBytes(xaFileSize - Cdxa.XA_RIFF_HEADER.Length));
                        }

                        bwDictionary[keyname].Close();
                    }
                }
                else
                {
                    // Console.WriteLine("XA Signature bytes not found");
                }
            }                
        }    
    }
}
