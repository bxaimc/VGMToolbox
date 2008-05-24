using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

using ICSharpCode.SharpZipLib.GZip;

using VGMToolbox.util;
using VGMToolbox.format;

namespace VGMToolbox.format
{
    static class FormatUtil
    {
        private const int MAX_SIGNATURE_LENGTH = 5;
        private const int HEADER_OFFSET = 0;

        private static readonly byte[] ZIP_SIGNATURE = new byte[] { 0x50, 0x4B, 0x03, 0x04 }; // PK..
        private static readonly byte[] GZIP_SIGNATURE = new byte[] { 0x1F, 0x8B };

        /// <summary>
        /// Loops through all classes that implement IFormat and checks the signature bytes (magic number) to determine
        /// the type
        /// </summary>
        /// <param name="pFileStream">File Stream of the file to check.</param>
        /// <returns>Type of object if it matches and exisiting format class, null otherwise</returns>
        public static Type getObjectType(FileStream pFileStream)
        {
            byte[] signatureBytes;
            
            // Grab the first few bytes, since a full file read is not needed for the header

            // Check for Gzip compressed (VGM: .vgz files)
            if (IsGzipFile(pFileStream))
            {
                signatureBytes = getGzippedSignatureBytes(pFileStream);
            }
            else
            {
                signatureBytes = ParseFile.parseSimpleOffset(pFileStream, HEADER_OFFSET, MAX_SIGNATURE_LENGTH);
            }
            
            Type ret = null;
                        
            // Get the assembly for this application
            Assembly asm = Assembly.GetExecutingAssembly();
            
            // Get all classes being used in this application
            Type[] asmtypes = asm.GetTypes();


            // Loop through classes checking interfaces
            foreach (Type t in asmtypes)
            {
                if (t.IsClass & t.GetInterface("IFormat") != null)
                {
                    // Create and instance of this format
                    Object o = asm.CreateInstance(t.FullName);
                    
                    // Set it to the Interface
                    IFormat format = o as IFormat;                    

                    // Check the header bytes
                    if (ParseFile.compareSegment(signatureBytes, HEADER_OFFSET, format.getAsciiSignature()))
                    {
                        ret = Type.GetType(t.FullName);
                        break;
                    }
                }
            }            
            return ret;
        }

        public static bool IsZipFile(string pFilePath)
        {
            bool ret = false;
            FileStream fs = File.OpenRead(pFilePath);
            byte[] signatureBytes = ParseFile.parseSimpleOffset(fs, HEADER_OFFSET, ZIP_SIGNATURE.Length);
            
            fs.Close();
            fs.Dispose();

            if (ParseFile.compareSegment(signatureBytes, HEADER_OFFSET, ZIP_SIGNATURE))
            {
                ret = true;
            }

            return ret;
        }

        public static bool IsGzipFile(string pFilePath)
        {
            bool ret = false;
            FileStream fs = File.OpenRead(pFilePath);
            byte[] signatureBytes = ParseFile.parseSimpleOffset(fs, HEADER_OFFSET, GZIP_SIGNATURE.Length);

            fs.Close();
            fs.Dispose();

            if (ParseFile.compareSegment(signatureBytes, HEADER_OFFSET, GZIP_SIGNATURE))
            {
                ret = true;
            }

            return ret;
        }

        public static bool IsGzipFile(Stream pFileStream)
        {
            bool ret = false;
            long currentOffset = pFileStream.Position;
            
            byte[] signatureBytes = ParseFile.parseSimpleOffset(pFileStream, HEADER_OFFSET, 
                GZIP_SIGNATURE.Length);

            if (ParseFile.compareSegment(signatureBytes, HEADER_OFFSET, GZIP_SIGNATURE))
            {
                ret = true;
            }

            pFileStream.Position = currentOffset;
            
            return ret;
        }

        private static byte[] getGzippedSignatureBytes(Stream pFileStream)
        {
            byte[] ret = null;

            long currentPosition = pFileStream.Position;

            GZipInputStream gZipInputStream = new GZipInputStream(pFileStream);
            string tempGzipFile = Path.GetTempFileName();
            FileStream gZipFileStream = new FileStream(tempGzipFile, FileMode.Open, FileAccess.ReadWrite);

            int size = 4096;
            byte[] writeData = new byte[size];
            while (true)
            {
                size = gZipInputStream.Read(writeData, 0, size);
                if (size > 0)
                {
                    gZipFileStream.Write(writeData, 0, size);
                }
                else
                {
                    break;
                }
            }

            ret = ParseFile.parseSimpleOffset(gZipFileStream, HEADER_OFFSET, MAX_SIGNATURE_LENGTH);
            gZipFileStream.Close();
            gZipFileStream.Dispose();

            File.Delete(tempGzipFile);               // delete temp file
            pFileStream.Position = currentPosition;  // return file to position on entry

            return ret;
        }
    }
}
