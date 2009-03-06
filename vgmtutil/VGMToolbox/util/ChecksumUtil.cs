using System;
using System.IO;
using System.Security.Cryptography;

using ICSharpCode.SharpZipLib.Checksums;

namespace VGMToolbox.util
{
    public class ChecksumUtil
    {
        public static string GetCrc32OfFullFile(FileStream pFileStream)
        {
            Crc32 crc32Generator = new Crc32();

            long remaining = pFileStream.Length;
            byte[] data = new byte[Constants.FILE_READ_CHUNK_SIZE];
            int read;

            pFileStream.Seek(0, SeekOrigin.Begin);
            crc32Generator.Reset();

            while ((read = pFileStream.Read(data, 0, data.Length)) > 0)
            {
                crc32Generator.Update(data, 0, read);
            }

            return crc32Generator.Value.ToString("X8");
        }

        public static string GetMd5OfFullFile(FileStream pFileStream)
        {
            MD5CryptoServiceProvider md5Hash = new MD5CryptoServiceProvider();

            pFileStream.Seek(0, SeekOrigin.Begin);
            md5Hash.ComputeHash(pFileStream);
            return ParseFile.ByteArrayToString(md5Hash.Hash);
        }

        public static string GetSha1OfFullFile(FileStream pFileStream)
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
                        (String.Format("End of stream reached with {0} bytes left to read", remaining));

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
                        (String.Format("End of stream reached with {0} bytes left to read", remaining));

                pCrc32.Update(data, 0, read);
                pMd5CryptoStream.Write(data, 0, read);
                pSha1CryptoStream.Write(data, 0, read);
                remaining -= read;
                offset += read;
            }
        }
    }
}
