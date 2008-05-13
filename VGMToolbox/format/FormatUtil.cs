using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

using VGMToolbox.util;
using VGMToolbox.format;

namespace VGMToolbox.format
{
    static class FormatUtil
    {
        private const int MAX_SIGNATURE_LENGTH = 3;
        private const int HEADER_OFFSET = 0;

        private static readonly byte[] ZIP_SIGNATURE = new byte[] { 0x50, 0x4B, 0x03, 0x04 }; // PK..

        /// <summary>
        /// Loops through all classes that implement IFormat and checks the signature bytes (magic number) to determine
        /// the type
        /// </summary>
        /// <param name="pFileStream">File Stream of the file to check.</param>
        /// <returns>Type of object if it matches and exisiting format class, null otherwise</returns>
        public static Type getObjectType(Stream pFileStream)
        {
            // Grab the first few bytes, since a full file read is not needed for the header
            byte[] signatureBytes = ParseFile.parseSimpleOffset(pFileStream, HEADER_OFFSET, MAX_SIGNATURE_LENGTH);
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
    }
}
