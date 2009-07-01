using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;

using ICSharpCode.SharpZipLib.Checksums;
using Ionic.Zlib;

namespace VGMToolbox.util
{
    public sealed class ChecksumUtil
    {
        private ChecksumUtil() { }

        public static string GetCrc32OfFullFile(Stream pFileStream)
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

        public static string GetMd5OfFullFile(Stream pFileStream)
        {
            MD5CryptoServiceProvider md5Hash = new MD5CryptoServiceProvider();

            pFileStream.Seek(0, SeekOrigin.Begin);
            md5Hash.ComputeHash(pFileStream);
            return ParseFile.ByteArrayToString(md5Hash.Hash);
        }

        public static string GetSha1OfFullFile(Stream pFileStream)
        {
            SHA1CryptoServiceProvider sha1Hash = new SHA1CryptoServiceProvider();

            pFileStream.Seek(0, SeekOrigin.Begin);
            sha1Hash.ComputeHash(pFileStream);
            return ParseFile.ByteArrayToString(sha1Hash.Hash);
        }

        public static void AddChunkToChecksum(Stream pStream, int pOffset, int pLength,
            ref Crc32 pCrc32)
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
                    throw new EndOfStreamException
                        (String.Format(CultureInfo.CurrentCulture, "End of stream reached with {0} bytes left to read", remaining));

                pCrc32.Update(data, 0, read);
                remaining -= read;
                offset += read;
            }
        }

        public static void AddChunkToChecksum(Stream pStream, int pOffset, int pLength,
            ref Crc32 pCrc32, ref CryptoStream pMd5CryptoStream, ref CryptoStream pSha1CryptoStream)
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
                    throw new EndOfStreamException
                        (String.Format(CultureInfo.CurrentCulture, "End of stream reached with {0} bytes left to read", remaining));

                pCrc32.Update(data, 0, read);
                pMd5CryptoStream.Write(data, 0, read);
                pSha1CryptoStream.Write(data, 0, read);
                remaining -= read;
                offset += read;
            }
        }
    }
}
