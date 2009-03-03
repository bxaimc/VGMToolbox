using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

using VGMToolbox.util.ObjectPooling;

using ICSharpCode.SharpZipLib.Checksums;

namespace VGMToolbox.util
{
    public class ParseFile
    {
        public static int MAX_BUFFER_SIZE = 70000;
        
        public static byte[] parseSimpleOffset(byte[] pBytes, int pOffset, int pLength)
        {
            byte[] ret = new byte[pLength];
            uint j = 0;

            for (int i = pOffset; i < pOffset + pLength; i++)
            {
                ret[j] = pBytes[i];
                j++;
            }

            return ret;
        }

        public static byte[] parseSimpleOffset(Stream pFileStream, int pOffset, int pLength)
        {
            byte[] ret = new byte[pLength];
            long currentStreamPosition = pFileStream.Position;

            pFileStream.Seek((long)pOffset, SeekOrigin.Begin);
            BinaryReader br = new BinaryReader(pFileStream);
            ret = br.ReadBytes((int)pLength);

            pFileStream.Position = currentStreamPosition;

            return ret;
        }

        public static byte[] parseSimpleOffset(Stream pFileStream, long pOffset, int pLength)
        {
            byte[] ret = new byte[pLength];
            long currentStreamPosition = pFileStream.Position;

            pFileStream.Seek(pOffset, SeekOrigin.Begin);
            BinaryReader br = new BinaryReader(pFileStream);
            ret = br.ReadBytes(pLength);

            pFileStream.Position = currentStreamPosition;

            return ret;
        }

        public static void parseSimpleOffset(Stream pFileStream, int pOffset, int pLength, ref ByteArray pOutputBuffer)
        {
            long currentStreamPosition = pFileStream.Position;
            pFileStream.Seek((long)pOffset, SeekOrigin.Begin);
            BinaryReader br = new BinaryReader(pFileStream);           
            br.Read(pOutputBuffer.ByArray, 0, (int) pLength);
            pFileStream.Position = currentStreamPosition;
        }

        public static int getSegmentLength(byte[] pBytes, int pOffset, byte[] pTerminator)
        {
            int ret;
            Boolean terminatorFound = false;
            int i = pOffset;

            while (i < pBytes.Length)
            {
                if (pBytes[i] == pTerminator[0])    // first char match
                {
                    if (CompareSegment(pBytes, i, pTerminator))
                    {
                        terminatorFound = true;
                        break;
                    }
                }
                i++;
            } // while (!terminatorFound)

            if (terminatorFound)
            {
                ret = i - pOffset;
            }
            else
            {
                //ret = pOffset;
                ret = 0;
            }
            
            return ret;
        }

        public static int getSegmentLength(Stream pStream, int pOffset, byte[] pTerminator)
        {
            int ret;
            Boolean terminatorFound = false;
            int i = pOffset;
            byte[] checkBytes = new byte[pTerminator.Length];

            while (i < pStream.Length)
            {
                pStream.Seek(i, SeekOrigin.Begin);
                pStream.Read(checkBytes, 0, 1);

                if (checkBytes[0] == pTerminator[0])    // first char match
                {

                    pStream.Seek(i, SeekOrigin.Begin);
                    pStream.Read(checkBytes, 0, pTerminator.Length);
                    
                    if (CompareSegment(checkBytes, 0, pTerminator))
                    {
                        terminatorFound = true;
                        break;
                    }
                }
                i++;
            } // while (!terminatorFound)

            if (terminatorFound)
            {
                ret = i - pOffset;
            }
            else
            {
                //ret = pOffset;
                ret = 0;
            }

            return ret;
        }

        public static long GetNextOffset(Stream pStream, long pOffset, byte[] pSearchBytes)
        {
            long initialStreamPosition = pStream.Position;

            bool itemFound = false;
            long absoluteOffset = pOffset;
            long relativeOffset;
            byte[] checkBytes = new byte[MAX_BUFFER_SIZE];
            byte[] compareBytes;

            long ret = -1;

            while (!itemFound && (absoluteOffset < pStream.Length))
            {
                pStream.Position = absoluteOffset;
                pStream.Read(checkBytes, 0, MAX_BUFFER_SIZE);
                relativeOffset = 0;

                while (!itemFound && (relativeOffset < MAX_BUFFER_SIZE))
                {
                    if ((relativeOffset + pSearchBytes.Length) < checkBytes.Length)
                    {
                        compareBytes = new byte[pSearchBytes.Length];
                        Array.Copy(checkBytes, relativeOffset,
                            compareBytes, 0, pSearchBytes.Length);

                        if (CompareSegment(compareBytes, 0, pSearchBytes))
                        {
                            itemFound = true;
                            ret = absoluteOffset + relativeOffset;
                            break;
                        }
                    }
                    relativeOffset++;
                }

                absoluteOffset += (MAX_BUFFER_SIZE - pSearchBytes.Length);
            }

            // return stream to incoming position
            pStream.Position = initialStreamPosition;

            return ret;
        }

        public static bool CompareSegment(byte[] pBytes, int pOffset, byte[] pTarget)
        {
            Boolean ret = true;
            uint j = 0;
            for (int i = pOffset; i < pTarget.Length; i++)
            {
                if (pBytes[i] != pTarget[j])
                {
                    ret = false;
                    break;
                }
                j++;
            }

            return ret;
        }

        public static bool CompareSegment(byte[] pBytes, long pOffset, byte[] pTarget)
        {
            Boolean ret = true;
            uint j = 0;
            for (long i = pOffset; i < pTarget.Length; i++)
            {
                if (pBytes[i] != pTarget[j])
                {
                    ret = false;
                    break;
                }
                j++;
            }

            return ret;
        }

        /// <summary>
        /// Reads data into a complete array, throwing an EndOfStreamException
        /// if the stream runs out of data first, or if an IOException
        /// naturally occurs.
        /// </summary>
        /// <param name="stream">The stream to read data from</param>
        /// <param name="data">The array to read bytes into. The array
        /// will be completely filled from the stream, so an appropriate
        /// size must be given.</param>
        public static void ReadWholeArray(Stream stream, byte[] data)
        {
            ReadWholeArray(stream, data, data.Length);            
        }

        public static void ReadWholeArray(Stream stream, byte[] data, int pLength)
        {
            int offset = 0;
            int remaining = pLength;
            while (remaining > 0)
            {
                int read = stream.Read(data, offset, remaining);
                if (read <= 0)
                    throw new EndOfStreamException
                        (String.Format("End of stream reached with {0} bytes left to read", remaining));
                remaining -= read;
                offset += read;
            }
        }

        public static void ExtractChunkToFile(Stream pStream, long pOffset, int pLength, string pFilePath)
        {
            BinaryWriter bw = null;
            string fullOutputDirectory = Path.GetDirectoryName(Path.GetFullPath(pFilePath));

            // create output folder if needed
            if (!Directory.Exists(fullOutputDirectory))
            {
                Directory.CreateDirectory(fullOutputDirectory);
            }

            try
            {
                bw = new BinaryWriter(File.Open(pFilePath, FileMode.Create, FileAccess.Write));

                int read = 0;
                int totalBytes = 0;
                byte[] bytes = new byte[4096];
                pStream.Seek((long)pOffset, SeekOrigin.Begin);

                int maxread = pLength > bytes.Length ? bytes.Length : pLength;

                while ((read = pStream.Read(bytes, 0, maxread)) > 0)
                {
                    bw.Write(bytes, 0, read);
                    totalBytes += read;

                    maxread = (pLength - totalBytes) > bytes.Length ? bytes.Length : (pLength - totalBytes);
                }
            }
            finally
            {
                if (bw != null)
                {
                    bw.Close();
                }
            }

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

        public static string ByteArrayToString(byte[] pBytes)
        {
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < pBytes.Length; i++)
            {
                sBuilder.Append(pBytes[i].ToString("X2"));
            }

            return sBuilder.ToString();
        }

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
            return ByteArrayToString(md5Hash.Hash);
        }

        public static string GetSha1OfFullFile(FileStream pFileStream)
        {
            SHA1CryptoServiceProvider sha1Hash = new SHA1CryptoServiceProvider();

            pFileStream.Seek(0, SeekOrigin.Begin);
            sha1Hash.ComputeHash(pFileStream);
            return ByteArrayToString(sha1Hash.Hash);
        }

        public static void UpdateTextField(string pFilePath, string pFieldValue, int pOffset,
            int pMaxLength)
        {
            System.Text.Encoding enc = System.Text.Encoding.ASCII;

            using (BinaryWriter bw =
                new BinaryWriter(File.Open(pFilePath, FileMode.Open, FileAccess.ReadWrite)))
            {
                byte[] newBytes = new byte[pMaxLength];
                byte[] convertedBytes = enc.GetBytes(pFieldValue);

                int numBytesToCopy = 
                    convertedBytes.Length <= pMaxLength ? convertedBytes.Length : pMaxLength;
                Array.ConstrainedCopy(convertedBytes, 0, newBytes, 0, numBytesToCopy);

                bw.Seek(pOffset, SeekOrigin.Begin);
                bw.Write(newBytes);                
            }
        }

        public static void ReplaceFileChunk(string pSourceFilePath, long pSourceOffset,
            long pLength, string pDestinationFilePath, long pDestinationOffset)
        {
            int read = 0;
            long maxread;
            int totalBytes = 0;
            byte[] bytes = new byte[MAX_BUFFER_SIZE];

            using (BinaryWriter bw =
                new BinaryWriter(File.Open(pDestinationFilePath, FileMode.Open, FileAccess.ReadWrite)))
            {
                using (BinaryReader br = 
                    new BinaryReader(File.Open(pSourceFilePath, FileMode.Open, FileAccess.Read)))                
                {
                    br.BaseStream.Position = pSourceOffset;
                    bw.BaseStream.Position = pDestinationOffset;

                    maxread = pLength > bytes.Length ? bytes.Length : pLength;

                    while ((read = br.Read(bytes, 0, (int) maxread)) > 0)
                    {
                        bw.Write(bytes, 0, read);
                        totalBytes += read;

                        maxread = (pLength - totalBytes) > bytes.Length ? bytes.Length : (pLength - totalBytes);
                    }                    
                }
            }
        }

        public static void ZeroOutFileChunk(string pPath, long pOffset, int pLength)
        {
            int bytesToWrite = pLength;
            byte[] bytes;

            int maxWrite = bytesToWrite > MAX_BUFFER_SIZE ? MAX_BUFFER_SIZE : bytesToWrite;

            using (BinaryWriter bw = 
                new BinaryWriter(File.Open(pPath, FileMode.Open, FileAccess.Write)))
            {
                bw.BaseStream.Position = pOffset;
                
                while (bytesToWrite > 0)
                {
                    bytes = new byte[maxWrite];
                    bw.Write(bytes);
                    bytesToWrite -= maxWrite;
                    maxWrite = bytesToWrite > bytes.Length ? bytes.Length : bytesToWrite;
                }
            }
        }
    }
}
