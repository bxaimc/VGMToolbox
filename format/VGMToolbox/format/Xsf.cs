using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

using VGMToolbox.format.util;
using VGMToolbox.util;

namespace VGMToolbox.format
{
    public class Xsf : IFormat, IXsfTagFormat
    {
        // Fields
        private static readonly byte[] ASCII_SIGNATURE = new byte[] { 0x50, 0x53, 0x46 }; // PSF
        private const string FORMAT_ABBREVIATION = "PSF";

        private const int READ_CHUNK_SIZE = 71680;

        public const ushort VERSION_2SF = 0x24;
        public const ushort VERSION_DSF = 0x12;
        public const ushort VERSION_GSF = 0x22;
        public const ushort VERSION_MDSF = 0x13;
        public const ushort VERSION_PSF1 = 0x01;
        public const ushort VERSION_PSF2 = 0x02;
        public const ushort VERSION_QSF = 0x41;
        public const ushort VERSION_SNSF = 0x23;
        public const ushort VERSION_SSF = 0x11;
        public const ushort VERSION_USF = 0x21;

        public const string FORMAT_NAME_PSF = "PSF";
        public const string FORMAT_NAME_PSF2 = "PSF2";
        public const string FORMAT_NAME_2SF = "2SF";

        private const int SIG_OFFSET = 0x00;
        private const int SIG_LENGTH = 0x03;

        private const int VERSION_OFFSET = 0x03;
        private const int VERSION_LENGTH = 0x01;

        private const int RESERVED_SIZE_OFFSET = 0x04;
        private const int RESERVED_SIZE_LENGTH = 0x04;

        private const int COMPRESSED_SIZE_OFFSET = 0x08;
        private const int COMPRESSED_SIZE_LENGTH = 0x04;

        private const int CRC32_OFFSET = 0x0C;
        private const int CRC32_LENGTH = 0x04;

        public const int RESERVED_SECTION_OFFSET = 0x10;                
        
        public const string ASCII_TAG = "[TAG]";
        private const string TAG_UTF8_INDICATOR = "utf8=";

        private string filePath;
        public string FilePath 
        {
            get { return filePath; }
            set { filePath = value; }
        }

        private byte[] asciiSignature;
        private byte[] versionByte;
        private uint reservedSectionLength;
        private uint compressedProgramLength;
        private byte[] compressedProgramCrc32;
        private byte[] reservedSection;
        private byte[] compressedProgram;

        private Dictionary<string, string> tagHash;
        private Hashtable formatHash;

        // Properties
        public byte[] AsciiSignature { get { return this.asciiSignature; } }
        public byte[] CompressedProgram { get { return this.compressedProgram; } }
        public byte[] CompressedProgramCrc32 { get { return this.compressedProgramCrc32; } }
        public uint CompressedProgramLength { get { return this.compressedProgramLength; } }
        public byte[] ReservedSection { get { return this.reservedSection; } }
        public uint ReservedSectionLength { get { return this.reservedSectionLength; } }
        public byte[] VersionByte { get { return this.versionByte; } }
        public Dictionary<string, string> TagHash { get { return this.tagHash; } }

        // Methods
        public Xsf() {}
        
        public Xsf(byte[] pBytes)
        {
            this.initialize(pBytes);
        }

        public byte[] GetAsciiSignature()
        {
            return ASCII_SIGNATURE;
        }

        public string GetFileExtensions()
        {
            return null;
        }

        protected byte[] getCompressedProgram(byte[] pBytes)
        {
            return ParseFile.parseSimpleOffset(pBytes, (int) (RESERVED_SECTION_OFFSET + this.getReservedSectionLength(pBytes)), (int) this.getCompressedProgramLength(pBytes));
        }

        private byte[] getCompressedProgramCrc32(byte[] pBytes)
        {
            return ParseFile.parseSimpleOffset(pBytes, CRC32_OFFSET, CRC32_LENGTH);
        }

        private byte[] getCompressedProgramCrc32(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, CRC32_OFFSET, CRC32_LENGTH);
        }

        private uint getCompressedProgramLength(byte[] pBytes)
        {
            return BitConverter.ToUInt32(ParseFile.parseSimpleOffset(pBytes, COMPRESSED_SIZE_OFFSET, COMPRESSED_SIZE_LENGTH), 0);
        }

        private uint getCompressedProgramLength(Stream pStream)
        {
            return BitConverter.ToUInt32(ParseFile.parseSimpleOffset(pStream, COMPRESSED_SIZE_OFFSET, COMPRESSED_SIZE_LENGTH), 0);
        }

        public void GetDatFileCrc32(ref Crc32 pChecksum)
        {
            using (FileStream fs = File.OpenRead(this.filePath))
            {
                // Reserved Section
                ChecksumUtil.AddChunkToChecksum(fs, (int)RESERVED_SECTION_OFFSET,
                    (int)this.reservedSectionLength, ref pChecksum);

                // Compressed Program
                addDecompressedProgramChecksum(fs, ref pChecksum);
            }

            // Libs
            string[] libPaths = this.GetLibPathArray();

            foreach (string f in libPaths)
            {
                using (FileStream lfs = File.OpenRead(f))
                {
                    Xsf libXsf = new Xsf();
                    libXsf.Initialize(lfs, f);
                    libXsf.GetDatFileCrc32(ref pChecksum);
                    libXsf = null;
                }                
            }
        }

        public void GetDatFileChecksums(ref Crc32 pChecksum,
            ref CryptoStream pMd5CryptoStream, ref CryptoStream pSha1CryptoStream)
        {
            using (FileStream fs = File.OpenRead(this.filePath))
            {
                // Reserved Section
                ChecksumUtil.AddChunkToChecksum(fs, (int)RESERVED_SECTION_OFFSET,
                    (int)this.reservedSectionLength, ref pChecksum, ref pMd5CryptoStream,
                    ref pSha1CryptoStream);

                // Compressed Program
                addDecompressedProgramChecksum(fs, ref pChecksum, ref pMd5CryptoStream,
                    ref pSha1CryptoStream);
            }

            // Libs
            string[] libPaths = this.GetLibPathArray();

            foreach (string f in libPaths)
            {
                using (FileStream lfs = File.OpenRead(f))
                {
                    Xsf libXsf = new Xsf();
                    libXsf.Initialize(lfs, f);
                    libXsf.GetDatFileChecksums(ref pChecksum, ref pMd5CryptoStream,
                        ref pSha1CryptoStream);
                    libXsf = null;
                }
            }
        }

        protected void addDecompressedProgramChecksum(FileStream pFileStream, ref Crc32 pChecksum)
        {
            if (this.compressedProgramLength > 0)
            {
                InflaterInputStream inflater;
                int read;
                byte[] data = new byte[READ_CHUNK_SIZE];

                pFileStream.Seek((long)(RESERVED_SECTION_OFFSET + this.reservedSectionLength), SeekOrigin.Begin);
                inflater = new InflaterInputStream(pFileStream);

                while ((read = inflater.Read(data, 0, READ_CHUNK_SIZE)) > 0)
                {
                    pChecksum.Update(data, 0, read);
                }

                inflater.Close();
                inflater.Dispose();
            }
        }
        
        protected void addDecompressedProgramChecksum(FileStream pFileStream, ref Crc32 pChecksum, 
            ref CryptoStream pMd5CryptoStream, ref CryptoStream pSha1CryptoStream)
        { 
            if (this.compressedProgramLength > 0)
            {
                InflaterInputStream inflater;
                int read;
                byte[] data = new byte[READ_CHUNK_SIZE];

                pFileStream.Seek((long)(RESERVED_SECTION_OFFSET + this.reservedSectionLength), SeekOrigin.Begin);
                inflater = new InflaterInputStream(pFileStream);

                while ((read = inflater.Read(data, 0, READ_CHUNK_SIZE)) > 0)
                {
                    pChecksum.Update(data, 0, read);
                    pMd5CryptoStream.Write(data, 0, read);
                    pSha1CryptoStream.Write(data, 0, read);
                }

                inflater.Close();
                inflater.Dispose();
            }                        
        }

        /*
        public void getDecompressedProgram(FileStream pFileStream, ref ByteArray pOutputBuffer)
        {
            if (this.compressedProgramLength > 0)
            {
                int compressedProgramInstance = -1;
                ByteArray compressedProgram = ObjectPooler.Instance.GetFreeByteArray(ref compressedProgramInstance);

                // Get Compressed Program
                pFileStream.Seek(0, SeekOrigin.Begin);
                ParseFile.parseSimpleOffset(pFileStream, (int) (RESERVED_SECTION_OFFSET + this.reservedSectionLength),
                    (int) this.compressedProgramLength, ref compressedProgram);
                compressedProgram.ArrayLength = (int)this.compressedProgramLength;

                // Inflate
                Inflater inflater = new Inflater();
                inflater.SetInput(compressedProgram.ByArray, 0, compressedProgram.ArrayLength);
                inflater.Inflate(pOutputBuffer.ByArray);
                pOutputBuffer.ArrayLength = (int)inflater.TotalOut;

                if (!inflater.IsFinished)
                    throw new Exception(String.Format("Error inflating {0}, output incomplete", pFileStream.Name));

                // Return Byte Array
                ObjectPooler.Instance.DoneWithByteArray(compressedProgramInstance);
            }
            else
            {
                pOutputBuffer.ArrayLength = 0;
            }
        }
        */
        public string getFormat()
        {
            return this.formatHash[(ushort)this.versionByte[0]].ToString();
        }

        public string GetFormatAbbreviation()
        {
            return FORMAT_ABBREVIATION;
        }

        public string[] GetLibPathArray()
        {
            ArrayList libPaths = new ArrayList();
            string libPath = String.Empty;
            string[] libPathStrings;
            bool libsFound = true;
            int i = 2;

            // Grab the _lib files from the tags.  Sort them and get the checksum bytes of each one.
            if (tagHash.ContainsKey("_lib"))
            {
                libPath = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(this.filePath)), tagHash["_lib"]);
                libPaths.Add(libPath.Trim().ToUpper());

                while (libsFound)
                {
                    if (tagHash.ContainsKey("_lib" + i.ToString()))
                    {
                        libPaths.Add(Path.Combine(Path.GetDirectoryName(Path.GetFullPath(this.filePath)), tagHash["_lib" + i.ToString()]));
                        i++;
                    }
                    else
                    {
                        libsFound = false;
                    }
                }
            }

            // libPaths.Sort();
            libPathStrings = new string[libPaths.Count];
            libPathStrings = (string[])libPaths.ToArray(typeof(string));

            return libPathStrings;
        }

        protected byte[] getReservedSection(byte[] pBytes)
        {
            return ParseFile.parseSimpleOffset(pBytes, RESERVED_SECTION_OFFSET, (int) this.getReservedSectionLength(pBytes));
        }

        protected uint getReservedSectionLength(byte[] pBytes)
        {
            return BitConverter.ToUInt32(ParseFile.parseSimpleOffset(pBytes, RESERVED_SIZE_OFFSET, RESERVED_SIZE_LENGTH), 0);
        }

        protected uint getReservedSectionLength(Stream pStream)
        {
            return BitConverter.ToUInt32(ParseFile.parseSimpleOffset(pStream, RESERVED_SIZE_OFFSET, RESERVED_SIZE_LENGTH), 0);
        }

        protected byte[] getSignatureTag(byte[] pBytes)
        {
            return ParseFile.parseSimpleOffset(pBytes, SIG_OFFSET, SIG_LENGTH);
        }

        protected byte[] getSignatureTag(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, SIG_OFFSET, SIG_LENGTH);
        }

        protected byte[] getVersionTag(byte[] pBytes)
        {
            return ParseFile.parseSimpleOffset(pBytes, VERSION_OFFSET, VERSION_LENGTH);
        }

        protected byte[] getVersionTag(Stream pStream)
        {
            return ParseFile.parseSimpleOffset(pStream, VERSION_OFFSET, VERSION_LENGTH);
        }

        protected Dictionary<string, string> getTags(byte[] pBytes)
        {
            int tagOffset = (int) (RESERVED_SECTION_OFFSET + this.reservedSectionLength + this.compressedProgramLength + ASCII_TAG.Length);
            return getTags(pBytes, tagOffset, pBytes.Length);
        }

        /*
        protected Dictionary<string, string> getTags(ByteArray pBytes)
        {
            int tagOffset = (int) (RESERVED_SECTION_OFFSET + this.reservedSectionLength + this.compressedProgramLength + ASCII_TAG.Length);
            return getTags(pBytes.ByArray, tagOffset, pBytes.ArrayLength);
        }
        */
        protected Dictionary<string, string> getTags(Stream pStream)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();

            int tagOffset = (int) (RESERVED_SECTION_OFFSET + this.reservedSectionLength + this.compressedProgramLength + ASCII_TAG.Length);
            int tagLength = (int) pStream.Length - tagOffset;

            if ((tagLength > 0))
            {
                byte[] tagBytes = ParseFile.parseSimpleOffset(pStream, tagOffset, tagLength);
                ret = getTags(tagBytes, 0, (int)tagLength);
            }

            return ret;
        }

        protected Dictionary<string, string> getTags(byte[] pBytes, int pTagOffset, int pLength)
        {
            Dictionary<string, string> tags = 
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (pTagOffset < pLength)
            {
                string tagsString;
                byte[] tagBytes;

                int tagLength = pLength - pTagOffset;

                if (pTagOffset != 0)
                {
                    tagBytes = ParseFile.parseSimpleOffset(pBytes, pTagOffset, tagLength);
                }
                else
                {
                    tagBytes = pBytes;
                }

                /* This is a hack for improper tags, see Lost Vikings II SNSF set.  Official spec
                 *   does not allow null values (0x00) */
                tagBytes = FileUtil.ReplaceNullByteWithSpace(tagBytes);
                
                System.Text.Encoding enc = System.Text.Encoding.ASCII;
                tagsString = enc.GetString(tagBytes);
                
                // check for utf8 tag and reencode bytes if needed
                if (tagsString.IndexOf(TAG_UTF8_INDICATOR) > -1)
                {
                    enc = System.Text.Encoding.UTF8;
                    tagsString = enc.GetString(tagBytes);
                }

                string[] splitTags = tagsString.Trim().Split((char)0x0A);
                string[] tag;

                foreach (string s in splitTags)
                {
                    tag = s.Split((char)0x3D);

                    if (tag.Length >= 2)
                    {
                        if (!tags.ContainsKey(tag[0]))
                        {
                            tags.Add(tag[0].Trim(), tag[1].Trim());
                        }
                        else
                        {
                            string oldTag = tags[tag[0]] + Environment.NewLine;
                            tags.Remove(tag[0]);
                            tags.Add(tag[0], oldTag + tag[1]);
                        }
                    }
                } //foreach (string s in splitTags)
            }
            return tags;
        }

        public void initialize(byte[] pBytes)
        {
            this.asciiSignature = this.getSignatureTag(pBytes);
            this.versionByte = this.getVersionTag(pBytes);
            this.reservedSectionLength = this.getReservedSectionLength(pBytes);
            this.compressedProgramLength = this.getCompressedProgramLength(pBytes);
            this.compressedProgramCrc32 = this.getCompressedProgramCrc32(pBytes);
            this.populateFormatHash();
            this.tagHash = this.getTags(pBytes);         
        }

        public void Initialize(Stream pStream, string pFilePath)                 
        {
            this.filePath = pFilePath;
            this.asciiSignature = this.getSignatureTag(pStream);
            this.versionByte = this.getVersionTag(pStream);
            this.reservedSectionLength = this.getReservedSectionLength(pStream);
            this.compressedProgramLength = this.getCompressedProgramLength(pStream);
            this.compressedProgramCrc32 = this.getCompressedProgramCrc32(pStream);
            this.populateFormatHash();
            this.tagHash = this.getTags(pStream);
        }

        protected void populateFormatHash()
        {
            this.formatHash = new Hashtable();
            this.formatHash.Add((ushort)1, FORMAT_NAME_PSF);
            this.formatHash.Add((ushort)2, FORMAT_NAME_PSF2);
            this.formatHash.Add((ushort)0x11, "SSF");
            this.formatHash.Add((ushort)0x12, "DSF");
            this.formatHash.Add((ushort)0x13, "MDSF");
            this.formatHash.Add((ushort)0x21, "USF");
            this.formatHash.Add((ushort)0x22, "GSF");
            this.formatHash.Add((ushort)0x23, "SNSF");
            this.formatHash.Add((ushort)0x24, "2SF");
            this.formatHash.Add((ushort)0x41, "QSF");
        }

        public bool verifyChecksum()
        {
            bool flag = false;
            if (this.compressedProgram != null)
            {
                Crc32 checksum = new Crc32();
                checksum.Update(this.compressedProgram);
                flag = BitConverter.ToUInt32(this.compressedProgramCrc32, 0) == checksum.Value;
            }
            return flag;
        }

        public bool IsFileLibrary()
        {
            bool ret = false;

            FileStream fs;
            Type formatType;
            Xsf checkFile;
            ArrayList libPathArray;

            string libDirectory = Path.GetDirectoryName(Path.GetFullPath(this.filePath));

            foreach (string f in Directory.GetFiles(libDirectory))
            {
                try
                {
                    fs = File.OpenRead(f);
                    formatType = FormatUtil.getObjectType(fs);

                    if (formatType == this.GetType())
                    {
                        fs.Seek(0, SeekOrigin.Begin);
                        checkFile = new Xsf();
                        checkFile.Initialize(fs, f);
                        libPathArray = new ArrayList(checkFile.GetLibPathArray());

                        if (libPathArray.Contains(this.filePath.ToUpper()))
                        {
                            ret = true;
                            fs.Close();
                            fs.Dispose();
                            break;
                        }

                    }
                    fs.Close();
                    fs.Dispose();
                }
                catch (Exception)
                { 
                    // Do nothing for now, if the file cannot be read than it cannot need a lib
                }
            }

            return ret;
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
        public bool IsLibraryPresent() 
        {
            bool ret = true;

            string libDirectory = Path.GetDirectoryName(Path.GetFullPath(this.filePath));
            string[] libPathArray = this.GetLibPathArray();

            foreach (string s in libPathArray)
            {
                if (!File.Exists(s))
                {
                    ret = false;
                    break;
                }
            }

            return ret;
        }

        #region IXsfTagFormat FUNCTIONS

        public bool CanUpdateXsfTags() { return true; }

        private string GetSimpleTag(string pTagKey)
        {
            string ret = String.Empty;

            if (this.tagHash.ContainsKey(pTagKey))
            {
                ret = tagHash[pTagKey];
            }
            return ret;
        }
        public string GetTitleTag() { return GetSimpleTag("title"); }
        public string GetArtistTag() { return GetSimpleTag("artist"); }
        public string GetGameTag() { return GetSimpleTag("game"); }
        public string GetYearTag() { return GetSimpleTag("year"); }
        public string GetGenreTag() { return GetSimpleTag("genre"); }
        public string GetCommentTag() { return GetSimpleTag("comment"); }
        public string GetCopyrightTag() { return GetSimpleTag("copyright"); }        
        public string GetVolumeTag() { return GetSimpleTag("volume"); }
        public string GetLengthTag() { return GetSimpleTag("length"); }
        public string GetFadeTag() { return GetSimpleTag("fade"); }
        public string GetSystemTag() { return GetSimpleTag("system"); }
        public string GetXsfByTag() 
        {
            string format = this.getFormat();

            if (format.Equals(FORMAT_NAME_PSF2))
            {
                format = FORMAT_NAME_PSF;
            }

            return GetSimpleTag(format + "by");         
        }

        private void SetSimpleTag(string pKey, string pNewValue)
        {
            if (!String.IsNullOrEmpty(pNewValue) && !String.IsNullOrEmpty(pNewValue.Trim()))
            {
                this.tagHash[pKey] = pNewValue.Trim();
            }
            else if (tagHash.ContainsKey(pKey))
            {
                tagHash.Remove(pKey);
            }
        }
        public void SetTitleTag(string pNewValue) { SetSimpleTag("title", pNewValue); }
        public void SetArtistTag(string pNewValue) { SetSimpleTag("artist", pNewValue); }
        public void SetGameTag(string pNewValue) { SetSimpleTag("game", pNewValue); }
        public void SetYearTag(string pNewValue) { SetSimpleTag("year", pNewValue); }
        public void SetGenreTag(string pNewValue) { SetSimpleTag("genre", pNewValue); }
        public void SetCommentTag(string pNewValue) { SetSimpleTag("comment", pNewValue); }
        public void SetCopyrightTag(string pNewValue) { SetSimpleTag("copyright", pNewValue); }        
        public void SetVolumeTag(string pNewValue) { SetSimpleTag("volume", pNewValue); }
        public void SetLengthTag(string pNewValue) { SetSimpleTag("length", pNewValue); }
        public void SetFadeTag(string pNewValue) { SetSimpleTag("fade", pNewValue); }
        public void SetSystemTag(string pNewValue) { SetSimpleTag("system", pNewValue); }
        public void SetXsfByTag(string pNewValue) 
        { 
            string format = this.getFormat();

            if (format.Equals(FORMAT_NAME_PSF2))
            {
                format = FORMAT_NAME_PSF;
            }

            SetSimpleTag(format.ToLower() + "by", pNewValue); 
        }

        public void UpdateTags()
        {
            int actualFileEnd;
            string retaggingFilePath;
            string[] splitValue;
            string[] splitParam = new string[] { Environment.NewLine };

            try
            {
                actualFileEnd = (int)(RESERVED_SECTION_OFFSET + this.reservedSectionLength + this.compressedProgramLength);
                retaggingFilePath = Path.Combine(Path.GetDirectoryName(this.filePath),
                    String.Format("{0}_RETAG_{1}{2}", Path.GetFileNameWithoutExtension(this.filePath), new Random().Next().ToString(), Path.GetExtension(this.filePath)));

                // extract file without tags
                using (FileStream fs = File.OpenRead(this.filePath))
                {
                    ParseFile.ExtractChunkToFile(fs, 0, actualFileEnd, retaggingFilePath);
                }

                // add new tags
                using (FileStream fs = File.Open(retaggingFilePath, FileMode.Append, FileAccess.Write))
                {
                    System.Text.Encoding enc = System.Text.Encoding.ASCII;
                    byte[] dataToWrite;

                    // write [TAG]
                    dataToWrite = enc.GetBytes(ASCII_TAG);
                    fs.Write(dataToWrite, 0, dataToWrite.Length);
                    
                    // change to UTF8 encoding
                    enc = System.Text.Encoding.UTF8;
                    
                    // add or update utf8=1 tag
                    this.tagHash["utf8"] = "1";
                    
                    foreach (string key in tagHash.Keys)
                    {
                        splitValue = tagHash[key].Split(splitParam, StringSplitOptions.None);

                        foreach (string valueItem in splitValue)
                        {
                            dataToWrite = enc.GetBytes(String.Format("{0}={1}", key, valueItem));
                            fs.Write(dataToWrite, 0, dataToWrite.Length);
                            fs.WriteByte(0x0A);
                        }
                    } // foreach (string key in tagHash.Keys)
                } // using (FileStream fs = File.Open(this.filePath, FileMode.Append, FileAccess.Write))

                File.Delete(this.filePath);
                File.Move(retaggingFilePath, this.filePath);
            }
            catch (Exception _ex)
            {
                throw new Exception(String.Format("Error updating tags for <{0}>", this.filePath), _ex);
            }
        }

        #endregion

    }
}
