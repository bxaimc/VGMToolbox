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
        public uint TitleMetaDataSize { set; get; }
        public uint DataSize { set; get; }
        public uint FooterSize { set; get; }
        
        // offsets
        public uint CertificateChainOffset { set; get; }
        public uint TicketOffset { set; get; }
        public uint TitleMetaDataOffset { set; get; }
        public uint DataOffset { set; get; }
        public uint FooterOffset { set; get; }

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
                    this.TitleMetaDataSize = ParseFile.ReadUintBE(fs, 0x14);
                    this.DataSize = ParseFile.ReadUintBE(fs, 0x18);
                    this.FooterSize = ParseFile.ReadUintBE(fs, 0x1C);
                    
                    // offsets
                    this.CertificateChainOffset = this.PadValue(this.HeaderSize, "Certificate Chain Offset");
                    this.TicketOffset = this.PadValue(this.CertificateChainOffset + this.CertificateChainSize, "Ticket Offset");
                    this.TitleMetaDataOffset = this.PadValue(this.TicketOffset + this.TicketSize, "Title Meta Data Offset");
                    this.DataOffset = this.PadValue(this.TitleMetaDataOffset + this.TitleMetaDataSize, "Data Offset");
                    this.FooterOffset = this.PadValue(this.DataOffset + this.DataSize, "Footer Offset");


                } // using (FileStream fs = File.OpenRead(sourceFile))
            }
            else
            {
                throw new FormatException("Nintendo WAD magic bytes not found at offset 0x03.");
            }
        }

        /// <summary>
        /// Extracts all data from the WAD object.
        /// </summary>
        /// <returns>Array containing the paths of all extracted files.</returns>
        public string[] ExtractAll()
        {
            ArrayList extractedFiles = new ArrayList();
            string footerFile;

            extractedFiles.Add(this.ExtractCertificateChain());
            extractedFiles.Add(this.ExtractTicket());
            extractedFiles.Add(this.ExtractTitleMetaData());

            // optional footer
            footerFile = this.ExtractFooter();

            if (!String.IsNullOrEmpty(footerFile))
            {
                extractedFiles.Add(footerFile);
            }
            
            
            return (string[])extractedFiles.ToArray(typeof(string));
        }

        /// <summary>
        /// Extract the certificate chain.
        /// </summary>
        /// <returns>File name of extracted file.</returns>
        public string ExtractCertificateChain()
        {
            string destinationFile = Path.Combine(
                                        this.GetUnpackFolder(),
                                        (this.TitleId.ToString("X8") + ".cert"));
            

            using (FileStream fs = File.OpenRead(this.SourceFileName))
            {
                ParseFile.ExtractChunkToFile(fs, this.CertificateChainOffset, this.CertificateChainSize, destinationFile);
            }

            return destinationFile;
        }

        /// <summary>
        /// Extract the ticket.
        /// </summary>
        /// <returns>File name of extracted file.</returns>
        public string ExtractTicket()
        {
            string destinationFile = Path.Combine(
                                        this.GetUnpackFolder(),
                                        (this.TitleId.ToString("X8") + ".tik"));


            using (FileStream fs = File.OpenRead(this.SourceFileName))
            {
                ParseFile.ExtractChunkToFile(fs, this.TicketOffset, this.TicketSize, destinationFile);
            }

            return destinationFile;
        }

        /// <summary>
        /// Extract the title meta data.
        /// </summary>
        /// <returns>File name of extracted file.</returns>
        public string ExtractTitleMetaData()
        {
            string destinationFile = Path.Combine(
                                        this.GetUnpackFolder(),
                                        (this.TitleId.ToString("X8") + ".tmd"));


            using (FileStream fs = File.OpenRead(this.SourceFileName))
            {
                ParseFile.ExtractChunkToFile(fs, this.TitleMetaDataOffset, this.TitleMetaDataSize, destinationFile);
            }

            return destinationFile;
        }

        /// <summary>
        /// Extract optional footer.
        /// </summary>
        /// <returns>File name of extracted file or String.Empty if footer is not present.</returns>
        public string ExtractFooter()
        {
            string destinationFile = String.Empty;

            if (this.FooterSize > 0)
            {
                destinationFile = Path.Combine(
                                    this.GetUnpackFolder(), 
                                    (this.TitleId.ToString("X8") + ".trailer"));

                using (FileStream fs = File.OpenRead(this.SourceFileName))
                {
                    ParseFile.ExtractChunkToFile(fs, this.FooterOffset, this.FooterSize, destinationFile);
                }
            }
            
            return destinationFile;
        }

        /// <summary>
        /// Pads input value to next 0x40 byte alignment.
        /// </summary>
        /// <param name="value">Value to pad.</param>
        /// <param name="textDescription">Text description of value used for error output.</param>
        /// <returns>Value padded to next 0x40 byte alignment.</returns>
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

        /// <summary>
        /// Builds the output folder path for unpacking.
        /// </summary>
        /// <returns>Full path of folder for unpacking.</returns>
        private string GetUnpackFolder()
        {
            string unpackFolder = Path.Combine(
                Path.GetDirectoryName(this.SourceFileName),
                String.Format("{0}_{1}", NintendoWad.EXTRACTION_FOLDER, Path.GetFileNameWithoutExtension(this.SourceFileName)));

            return unpackFolder;
        }

        /// <summary>
        /// Checks for WAD file Magic Bytes.
        /// </summary>
        /// <param name="sourceFile">Full path to file to check.</param>
        /// <returns>Boolean value indicating if input file has WAD magic bytes.</returns>
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
