namespace VGMToolbox.util
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Security.Cryptography;

    using ICSharpCode.SharpZipLib.Checksums;
    using Ionic.Zlib;    
            
    /// <summary>
    /// Class containing static functions related to checksum generation.
    /// </summary>
    public sealed class ChecksumUtil
    {
        /// <summary>
        /// Prevents a default instance of the ChecksumUtil class from being created.
        /// </summary>
        private ChecksumUtil() 
        { 
        }

        /// <summary>
        /// Get the CRC32 checksum of the input stream.
        /// </summary>
        /// <param name="pFileStream">File Stream for which to generate the checksum.</param>
        /// <returns>String containing the hexidecimal representation of the CRC32 of the input stream.</returns>
        public static string GetCrc32OfFullFile(FileStream pFileStream)
        {
            // get incoming stream position
            long initialStreamPosition = pFileStream.Position;
            
            // move to zero position
            pFileStream.Seek(0, SeekOrigin.Begin);
            
            // calculate CRC32
            CRC32 crc32 = new CRC32();
            int ret = crc32.GetCrc32(pFileStream);

            // return stream to incoming position
            pFileStream.Position = initialStreamPosition;

            return ret.ToString("X8", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Get the MD5 checksum of the input stream.
        /// </summary>
        /// <param name="pFileStream">File Stream for which to generate the checksum.</param>
        /// <returns>String containing the hexidecimal representation of the MD5 of the input stream.</returns>
        public static string GetMd5OfFullFile(FileStream pFileStream)
        {
            MD5CryptoServiceProvider md5Hash = new MD5CryptoServiceProvider();

            pFileStream.Seek(0, SeekOrigin.Begin);
            md5Hash.ComputeHash(pFileStream);
            return ParseFile.ByteArrayToString(md5Hash.Hash);
        }

        /// <summary>
        /// Get the SHA1 checksum of the input stream.
        /// </summary>
        /// <param name="pFileStream">File Stream for which to generate the checksum.</param>
        /// <returns>String containing the hexidecimal representation of the SHA1 of the input stream.</returns>
        public static string GetSha1OfFullFile(FileStream pFileStream)
        {
            SHA1CryptoServiceProvider sha1Hash = new SHA1CryptoServiceProvider();

            pFileStream.Seek(0, SeekOrigin.Begin);
            sha1Hash.ComputeHash(pFileStream);
            return ParseFile.ByteArrayToString(sha1Hash.Hash);
        }

        /// <summary>
        /// Adds a chunk of data to the input CRC32 generator.
        /// </summary>
        /// <param name="pStream">Stream to read data from.</param>
        /// <param name="pOffset">Offset to begin reading from.</param>
        /// <param name="pLength">Number of bytes to read.</param>
        /// <param name="pCrc32">CRC32 generator.</param>
        public static void AddChunkToChecksum(Stream pStream, int pOffset, int pLength, ref Crc32 pCrc32)
        {
            int remaining = pLength;
            byte[] data = new byte[4096];
            int read;
            int offset = pOffset;

            pStream.Seek((long)pOffset, SeekOrigin.Begin);

            while (remaining > 0)
            {
                if (remaining < 4096)
                {
                    read = pStream.Read(data, 0, remaining);
                }
                else
                {
                    read = pStream.Read(data, 0, 4096);
                }

                if (read <= 0)
                {
                    throw new EndOfStreamException(
                        String.Format(
                            CultureInfo.CurrentCulture, 
                            "End of stream reached with {0} bytes left to read", 
                            remaining));
                }
                
                pCrc32.Update(data, 0, read);
                remaining -= read;
                offset += read;
            }
        }

        /// <summary>
        /// Adds a chunk of data to the input CRC32/MD5/SHA1 generator.
        /// </summary>
        /// <param name="sourceStream">Stream to read data from.</param>
        /// <param name="pOffset">Offset to begin reading from.</param>
        /// <param name="pLength">Number of bytes to read.</param>
        /// <param name="pCrc32">CRC32 generator.</param>
        /// <param name="pMd5CryptoStream">MD5 generator.</param>
        /// <param name="pSha1CryptoStream">SHA1 generator.</param>
        public static void AddChunkToChecksum(
            Stream sourceStream, 
            int pOffset, 
            int pLength,
            ref Crc32 pCrc32, 
            ref CryptoStream pMd5CryptoStream, 
            ref CryptoStream pSha1CryptoStream)
        {
            int remaining = pLength;
            byte[] data = new byte[4096];
            int read;
            int offset = pOffset;

            sourceStream.Seek((long)pOffset, SeekOrigin.Begin);

            while (remaining > 0)
            {
                if (remaining < 4096)
                {
                    read = sourceStream.Read(data, 0, remaining);
                }
                else
                {
                    read = sourceStream.Read(data, 0, 4096);
                }

                if (read <= 0)
                {
                    throw new EndOfStreamException(
                        String.Format(
                            CultureInfo.CurrentCulture, 
                            "End of stream reached with {0} bytes left to read", 
                            remaining));
                }

                pCrc32.Update(data, 0, read);
                pMd5CryptoStream.Write(data, 0, read);
                pSha1CryptoStream.Write(data, 0, read);
                remaining -= read;
                offset += read;
            }
        }
    }
}
