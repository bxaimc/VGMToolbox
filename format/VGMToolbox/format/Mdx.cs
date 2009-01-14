using System;
using System.Collections.Generic;
using System.IO;

using ICSharpCode.SharpZipLib.Checksums;

using VGMToolbox.util;
using VGMToolbox.util.ObjectPooling;

namespace VGMToolbox.format
{
    public class Mdx : IFormat
    {
        private const string FORMAT_ABBREVIATION = "MDX";
        
        public static readonly byte[] TITLE_TERMINATOR = new byte[3] { 0x0D, 0x0A, 0x1A };
        public static readonly byte[] PDX_TERMINATOR = new byte[1] { 0x00 };
        public static readonly string MDX_FILE_EXTENSION = ".MDX";
        public static readonly string PDX_FILE_EXTENSION = ".PDX";
        public static readonly string EXCEPTION_PDX_MISSING = "PDX file for this file was not found.";

        private string filePath;
        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }

        private string title = String.Empty;
        private string pdxFileName = String.Empty;

        private int titleLength;

        private int pdxOffset;
        private int pdxLength;

        private int dataOffset;
        private int dataLength;

        Dictionary<string, string> tagHash = new Dictionary<string, string>();

        public Mdx() { }

        #region METHODS

        public Mdx(byte[] pBytes)
        {
            initialize(pBytes);
        }

        public string Title { get { return title; } }
        public string PdxFileName { get { return pdxFileName; } }


        public void initialize(byte[] pBytes)
        {
            // int _titleLength;
            // int _pdxLength;

            // Title
            this.titleLength = ParseFile.getSegmentLength(pBytes, 0, TITLE_TERMINATOR);

            if (this.titleLength > 0)
            {
                title = VGMToolbox.util.Encoding.getJpEncodedText(ParseFile.parseSimpleOffset(pBytes, 0,
                         this.titleLength));
            }

            // PDX
            this.pdxOffset = this.titleLength + TITLE_TERMINATOR.Length;
            this.pdxLength = ParseFile.getSegmentLength(pBytes,
                            (this.titleLength + TITLE_TERMINATOR.Length), PDX_TERMINATOR);
            if (this.pdxLength > 0)
            {
                pdxFileName = VGMToolbox.util.Encoding.getJpEncodedText(ParseFile.parseSimpleOffset(pBytes,
                                (this.titleLength + TITLE_TERMINATOR.Length), this.pdxLength));
            }

            this.dataOffset = this.titleLength + TITLE_TERMINATOR.Length +
                this.pdxLength + PDX_TERMINATOR.Length;
            this.dataLength = (int)(pBytes.Length - this.dataOffset);
        }
        public void Initialize(Stream pStream)
        {
            // Title
            this.titleLength = ParseFile.getSegmentLength(pStream, 0, TITLE_TERMINATOR);
            
            if (this.titleLength > 0)
            {
                this.title = VGMToolbox.util.Encoding.getJpEncodedText(ParseFile.parseSimpleOffset(pStream, 0,
                         this.titleLength));
            }

            // PDX
            this.pdxOffset = this.titleLength + TITLE_TERMINATOR.Length;
            this.pdxLength = ParseFile.getSegmentLength(pStream, this.pdxOffset, PDX_TERMINATOR);
            
            if (this.pdxOffset > 0)
            {
                this.pdxFileName = VGMToolbox.util.Encoding.getJpEncodedText(ParseFile.parseSimpleOffset(pStream,
                                this.pdxOffset, this.pdxLength));
                string pdxExtension = Path.GetExtension(this.pdxFileName);

                if (pdxExtension.Equals(String.Empty))
                {
                    this.pdxFileName += PDX_FILE_EXTENSION;
                }
            }

            this.initializeTagHash();

            this.dataOffset = this.titleLength + TITLE_TERMINATOR.Length +
                this.pdxLength + PDX_TERMINATOR.Length;
            this.dataLength = (int)(pStream.Length - this.dataOffset);
        }

        private void initializeTagHash()
        {
            tagHash.Add("Title", this.title);

            if (pdxFileName != null && !this.pdxFileName.Equals(String.Empty))
            {
                tagHash.Add("PDX", pdxFileName);
            }
        }

        public void GetDatFileCrc32(string pPath, ref Dictionary<string, ByteArray> pLibHash,
            ref Crc32 pChecksum, bool pUseLibHash)
        {
            int read;
            byte[] data;
            FileStream fs;

            pChecksum.Reset();

            // Add data segment
            if (this.dataLength > 0)
            {
                data = new byte[4096];

                fs = new FileStream(pPath, FileMode.Open, FileAccess.Read);
                fs.Seek(this.dataOffset, SeekOrigin.Begin);

                while ((read = fs.Read(data, 0, data.Length)) > 0)
                {
                    pChecksum.Update(data, 0, read);
                }

                fs.Close();
                fs.Dispose();
            }

            // Add PDX if found
            if (pdxFileName != null && !this.pdxFileName.Equals(String.Empty))
            {
                string pdxPath = Path.GetDirectoryName(pPath) + Path.DirectorySeparatorChar + 
                    this.pdxFileName;

                if (File.Exists(pdxPath))
                {
                    data = new byte[4096];

                    fs = new FileStream(pPath, FileMode.Open, FileAccess.Read);
                    fs.Seek(this.dataOffset, SeekOrigin.Begin);

                    while ((read = fs.Read(data, 0, 4096)) > 0)
                    {
                        pChecksum.Update(data, 0, read);
                    }

                    fs.Close();
                    fs.Dispose();
                }
                else  // Cannot be sure of unique value, raise exception to cause
                {     //   fall back to full file checksum

                    pChecksum.Reset();
                    throw new IOException(EXCEPTION_PDX_MISSING);
                }
            }
        }

        public byte[] GetAsciiSignature()
        {
            return null;
        }

        public string GetFileExtensions()
        {
            return MDX_FILE_EXTENSION;
        }
        
        public string GetFormatAbbreviation()
        {
            return FORMAT_ABBREVIATION;
        }

        public bool IsFileLibrary(string pPath)
        {
            return false;
        }

        public bool HasMultipleFileExtensions()
        {
            return false;
        }

        public Dictionary<string, string> GetTagHash()
        {
            return this.tagHash;
        }

        public bool UsesLibraries() { return true; }
        public bool IsLibraryPresent(string pFilePath) 
        {
            bool ret = false;

            if (!String.IsNullOrEmpty(this.pdxFileName))
            {
                string searchFile =
                    Path.Combine(Path.GetDirectoryName(Path.GetFullPath(pFilePath)), this.pdxFileName);

                if (File.Exists(searchFile))
                {
                    ret = true;
                }
            }
            else
            {
                ret = true;
            }
            return ret;
        }

        #endregion
    }    
}
