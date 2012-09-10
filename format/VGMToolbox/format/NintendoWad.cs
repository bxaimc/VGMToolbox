using System;
using System.Collections;
using System.IO;
using System.Text;

using VGMToolbox.util;

namespace VGMToolbox.format
{
    public class NintendoWad
    {
        public static readonly byte[] STANDARD_IDENTIFIER = new byte[] { 0x20, 0x49, 0x73, 0x00, 0x00 };
        public const uint IDENTIFIER_OFFSET = 0x03;
        public const string FORMAT_DESCRIPTION_STRING = "Nintendo WAD";
        public const string EXTRACTION_FOLDER = "VGMT_WAD_EXTRACT";

        public string SourceFileName { set; get; }

        public uint HeaderSize { set; get; }
        public uint WadType { set; get; }
        public uint CertificateChainSize { set; get; }
        public uint Reserved { set; get; }
        public uint TicketSize { set; get; }
        public uint TmdSize { set; get; }
        public uint DataSize { set; get; }
        public uint FooterSize { set; get; }
        
        // CERT
        public uint CertificateChainOffset { set; get; }

        // TITLE
        public ulong TitleId { set; get; }

        public NintendoWad(string sourceFile)
        { 
            // check magic bytes
            if (NintendoWad.IsWadFile(sourceFile))
            {
                // read header
                using (FileStream fs = File.OpenRead(sourceFile))
                {
                    // set source file
                    this.SourceFileName = sourceFile; 

                    // parse header
                    this.HeaderSize = ParseFile.ReadUintBE(fs, 0);
                    this.WadType = ParseFile.ReadUintBE(fs, 4);
                    this.CertificateChainSize = ParseFile.ReadUintBE(fs, 8);
                    this.Reserved = ParseFile.ReadUintBE(fs, 0xC);
                    this.TicketSize = ParseFile.ReadUintBE(fs, 0x10);
                    this.TmdSize = ParseFile.ReadUintBE(fs, 0x14);
                    this.DataSize = ParseFile.ReadUintBE(fs, 0x18);
                    this.FooterSize = ParseFile.ReadUintBE(fs, 0x1C);
                    
                    // CERT
                    this.CertificateChainOffset = this.PadValue(this.HeaderSize, "Certificate Chain Offset");

                } // using (FileStream fs = File.OpenRead(sourceFile))
            }
            else
            {
                throw new FormatException("Nintendo WAD magic bytes not found at offset 0x03.");
            }
        }

        public string[] ExtractAll()
        {
            ArrayList extractedFiles = new ArrayList();

            extractedFiles.Add(this.ExtractCert());

            
            
            
            
            return (string[])extractedFiles.ToArray(typeof(string));
        }

        public string ExtractCert()
        {
            string destinationFile = Path.Combine(
                                        Path.Combine(Path.GetDirectoryName(this.SourceFileName), NintendoWad.EXTRACTION_FOLDER),
                                        (this.TitleId.ToString("X8") + ".cert"));
            

            using (FileStream fs = File.OpenRead(this.SourceFileName))
            {
                ParseFile.ExtractChunkToFile(fs, this.CertificateChainOffset, this.CertificateChainSize, destinationFile);
            }

            return destinationFile;
        }





        private uint PadValue(uint value, string textDescription)
        {
            uint paddedValue;
            Int64 byteAlignedValue;

            byteAlignedValue = MathUtil.RoundUpToByteAlignment((long)value, 0x40);

            if (byteAlignedValue == -1)
            {
                throw new Exception(String.Format("Error padding value for \"{0}.\"", textDescription));
            }
            else
            {
                paddedValue = (uint)byteAlignedValue;
            }
            
            return paddedValue;
        }

        public static bool IsWadFile(string sourceFile)
        {
            bool isWad = false;
            byte[] magicBytes = new byte[NintendoWad.STANDARD_IDENTIFIER.Length];

            using (FileStream fs = File.OpenRead(sourceFile))
            {
                magicBytes = ParseFile.ParseSimpleOffset(fs, NintendoWad.IDENTIFIER_OFFSET, NintendoWad.STANDARD_IDENTIFIER.Length);

                if (ParseFile.CompareSegment(magicBytes, 0, NintendoWad.STANDARD_IDENTIFIER))
                {
                    isWad = true;
                }
            }

            return isWad;
        }

    }
}
