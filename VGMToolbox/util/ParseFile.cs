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

            pFileStream.Seek((long)pOffset, SeekOrigin.Begin);
            BinaryReader br = new BinaryReader(pFileStream);
            ret = br.ReadBytes((int)pLength);

            return ret;
        }

        public static void parseSimpleOffset(Stream pFileStream, int pOffset, int pLength, ref ByteArray pOutputBuffer)
        {
            pFileStream.Seek((long)pOffset, SeekOrigin.Begin);
            BinaryReader br = new BinaryReader(pFileStream);           
            br.Read(pOutputBuffer.ByArray, 0, (int) pLength);
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
                    if (compareSegment(pBytes, i, pTerminator))
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

        public static bool compareSegment(byte[] pBytes, int pOffset, byte[] pTarget)
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
