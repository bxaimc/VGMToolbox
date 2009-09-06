using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace VGMToolbox.util
{    
    /// <summary>
    /// Class for Parsing Files.
    /// </summary>
    public sealed class ParseFile
    {
        public const string LogFileName = "vgmt_extraction_log.txt";
        public const string SnakeBiteBatchFileName = "vgmt_extraction_log.bat";
        
        /// <summary>
        /// Prevents a default instance of the ParseFile class from being created.
        /// </summary>
        private ParseFile() 
        { 
        }
                
        /// <summary>
        /// Extract a section from the incoming byte array.
        /// </summary>
        /// <param name="sourceArray">Bytes to extract from.</param>
        /// <param name="startingOffset">Offset to begin cutting from.</param>
        /// <param name="lengthToCut">Number of bytes to cut.</param>
        /// <returns>Byte array containing the extracted section.</returns>
        public static byte[] ParseSimpleOffset(byte[] sourceArray, int startingOffset, int lengthToCut)
        {
            byte[] ret = new byte[lengthToCut];
            uint j = 0;

            for (int i = startingOffset; i < startingOffset + lengthToCut; i++)
            {
                ret[j] = sourceArray[i];
                j++;
            }

            return ret;
        }

        /// <summary>
        /// Extract a section from the incoming stream.
        /// </summary>
        /// <param name="stream">Stream to extract the chunk from.</param>
        /// <param name="startingOffset">Offset to begin cutting from.</param>
        /// <param name="lengthToCut">Number of bytes to cut.</param>
        /// <returns>Byte array containing the extracted section.</returns>
        public static byte[] ParseSimpleOffset(Stream stream, int startingOffset, int lengthToCut)
        {
            byte[] ret = new byte[lengthToCut];
            long currentStreamPosition = stream.Position;

            stream.Seek((long)startingOffset, SeekOrigin.Begin);
            BinaryReader br = new BinaryReader(stream);
            ret = br.ReadBytes((int)lengthToCut);

            stream.Position = currentStreamPosition;

            return ret;
        }

        /// <summary>
        /// Extract a section from the incoming stream.
        /// </summary>
        /// <param name="stream">Stream to extract the chunk from.</param>
        /// <param name="startingOffset">Offset to begin cutting from.</param>
        /// <param name="lengthToCut">Number of bytes to cut.</param>
        /// <returns>Byte array containing the extracted section.</returns>
        public static byte[] ParseSimpleOffset(Stream stream, long startingOffset, int lengthToCut)
        {
            byte[] ret = new byte[lengthToCut];
            long currentStreamPosition = stream.Position;

            stream.Seek(startingOffset, SeekOrigin.Begin);
            BinaryReader br = new BinaryReader(stream);
            ret = br.ReadBytes(lengthToCut);

            stream.Position = currentStreamPosition;

            return ret;
        }

        /// <summary>
        /// Get the length from the input offset to the location of the input terminator bytes or zero.
        /// </summary>
        /// <param name="sourceBytes">Bytes to check.</param>
        /// <param name="searchOffset">Offset at which to begin searching for the terminator bytes.</param>
        /// <param name="terminatorBytes">Bytes to search for.</param>
        /// <returns>Length of distance between offset and terminator or zero if not found.</returns>
        public static int GetSegmentLength(byte[] sourceBytes, int searchOffset, byte[] terminatorBytes)
        {
            int ret;
            bool terminatorFound = false;
            int i = searchOffset;

            while (i < sourceBytes.Length)
            {
                // first char match
                if (sourceBytes[i] == terminatorBytes[0])
                {
                    if (CompareSegment(sourceBytes, i, terminatorBytes))
                    {
                        terminatorFound = true;
                        break;
                    }
                }
                
                i++;
            } // while (!terminatorFound)

            if (terminatorFound)
            {
                ret = i - searchOffset;
            }
            else
            {
                ret = 0;
            }
            
            return ret;
        }

        /// <summary>
        /// Get the length from the input offset to the location of the input terminator bytes or zero.
        /// </summary>
        /// <param name="stream">Stream to check.</param>
        /// <param name="searchOffset">Offset at which to begin searching for the terminator bytes.</param>
        /// <param name="terminatorBytes">Bytes to search for.</param>
        /// <returns>Length of distance between offset and terminator or zero if not found.</returns>
        public static int GetSegmentLength(Stream stream, int searchOffset, byte[] terminatorBytes)
        {
            int ret;
            bool terminatorFound = false;
            int i = searchOffset;
            byte[] checkBytes = new byte[terminatorBytes.Length];

            while (i < stream.Length)
            {
                stream.Seek(i, SeekOrigin.Begin);
                stream.Read(checkBytes, 0, 1);

                // first char match
                if (checkBytes[0] == terminatorBytes[0])
                {
                    stream.Seek(i, SeekOrigin.Begin);
                    stream.Read(checkBytes, 0, terminatorBytes.Length);
                    
                    if (CompareSegment(checkBytes, 0, terminatorBytes))
                    {
                        terminatorFound = true;
                        break;
                    }
                }
                
                i++;
            } // while (!terminatorFound)

            if (terminatorFound)
            {
                ret = i - searchOffset;
            }
            else
            {
                ret = 0;
            }

            return ret;
        }

        /// <summary>
        /// Get the offset of the first instance of pSearchBytes after the input offset.
        /// </summary>
        /// <param name="stream">Stream to search.</param>
        /// <param name="startingOffset">Offset to begin searching from.</param>
        /// <param name="searchBytes">Bytes to search for.</param>
        /// <returns>Returns the offset of the first instance of pSearchBytes after the input offset or -1 otherwise.</returns>
        public static long GetNextOffset(Stream stream, long startingOffset, byte[] searchBytes)
        {
            return GetNextOffset(stream, startingOffset, searchBytes, true);
        }

        public static long GetNextOffset(Stream stream, long startingOffset, 
            byte[] searchBytes, bool returnStreamToIncomingPosition)
        {
            long initialStreamPosition = 0;

            if (returnStreamToIncomingPosition)
            {
                initialStreamPosition = stream.Position;
            }

            bool itemFound = false;
            long absoluteOffset = startingOffset;
            long relativeOffset;
            byte[] checkBytes = new byte[Constants.FileReadChunkSize];
            byte[] compareBytes;

            long ret = -1;

            while (!itemFound && (absoluteOffset < stream.Length))
            {
                stream.Position = absoluteOffset;
                stream.Read(checkBytes, 0, Constants.FileReadChunkSize);
                relativeOffset = 0;

                while (!itemFound && (relativeOffset < Constants.FileReadChunkSize))
                {
                    if ((relativeOffset + searchBytes.Length) < checkBytes.Length)
                    {
                        compareBytes = new byte[searchBytes.Length];
                        Array.Copy(checkBytes, relativeOffset, compareBytes, 0, searchBytes.Length);

                        if (CompareSegment(compareBytes, 0, searchBytes))
                        {
                            itemFound = true;
                            ret = absoluteOffset + relativeOffset;
                            break;
                        }
                    }

                    relativeOffset++;
                }

                absoluteOffset += Constants.FileReadChunkSize - searchBytes.Length;
            }

            // return stream to incoming position
            if (returnStreamToIncomingPosition)
            {
                stream.Position = initialStreamPosition;
            }

            return ret;
        }

        public static long GetNextOffset(Stream stream, long startingOffset,
            byte[] searchBytes, bool doOffsetModulo, long offsetModuloDivisor, 
            long offsetModuloResult)
        { 
            return GetNextOffset(stream, startingOffset, searchBytes,
                doOffsetModulo, offsetModuloDivisor, offsetModuloResult, true);
        }

        public static long GetNextOffset(Stream stream, long startingOffset,
            byte[] searchBytes, bool doOffsetModulo, long offsetModuloDivisor, 
            long offsetModuloResult, bool returnStreamToIncomingPosition)
        {
            long initialStreamPosition = 0;

            if (returnStreamToIncomingPosition)
            {
                initialStreamPosition = stream.Position;
            }

            bool itemFound = false;
            long absoluteOffset = startingOffset;
            long relativeOffset;
            long actualOffset;
            byte[] checkBytes = new byte[Constants.FileReadChunkSize];
            byte[] compareBytes;

            long ret = -1;

            while (!itemFound && (absoluteOffset < stream.Length))
            {
                stream.Position = absoluteOffset;
                stream.Read(checkBytes, 0, Constants.FileReadChunkSize);
                relativeOffset = 0;

                while (!itemFound && (relativeOffset < Constants.FileReadChunkSize))
                {
                    actualOffset = absoluteOffset + relativeOffset;

                    if ((!doOffsetModulo) ||
                        (actualOffset % offsetModuloDivisor == offsetModuloResult))
                    {
                        if ((relativeOffset + searchBytes.Length) < checkBytes.Length)
                        {
                            compareBytes = new byte[searchBytes.Length];
                            Array.Copy(checkBytes, relativeOffset, compareBytes, 0, searchBytes.Length);

                            if (CompareSegment(compareBytes, 0, searchBytes))
                            {
                                itemFound = true;
                                ret = actualOffset;
                                break;
                            }
                        }
                    }
                    
                    relativeOffset++;
                }

                absoluteOffset += Constants.FileReadChunkSize - searchBytes.Length;
            }

            // return stream to incoming position
            if (returnStreamToIncomingPosition)
            {
                stream.Position = initialStreamPosition;
            }

            return ret;
        }

        public static Dictionary<byte[], long[]> GetAllOffsets(Stream stream, long startingOffset,
            byte[][] searchByteArrays, bool returnStreamToIncomingPosition)
        {
            long initialStreamPosition = 0;
            
            if (returnStreamToIncomingPosition)
            {
                initialStreamPosition = stream.Position;
            }

            int maxSearchBytesLength = 0;
            long absoluteOffset = startingOffset;
            long relativeOffset;
            byte[] checkBytes = new byte[Constants.FileReadChunkSize];
            byte[] compareBytes;

            Dictionary<byte[], ArrayList> tempOutput = new Dictionary<byte[], ArrayList>();
            Dictionary<byte[], long[]> ret = new Dictionary<byte[], long[]>();

            foreach (byte[] b in searchByteArrays)
            {
                tempOutput.Add(b, new ArrayList());

                if (b.Length > maxSearchBytesLength)
                {
                    maxSearchBytesLength = b.Length;
                }
            }

            while (absoluteOffset < stream.Length)
            {
                stream.Position = absoluteOffset;
                stream.Read(checkBytes, 0, Constants.FileReadChunkSize);
                relativeOffset = 0;

                while (relativeOffset < Constants.FileReadChunkSize)
                {
                    foreach (byte[] searchBytes in searchByteArrays)
                    {
                        if ((relativeOffset + searchBytes.Length) < checkBytes.Length)
                        {
                            compareBytes = new byte[searchBytes.Length];
                            Array.Copy(checkBytes, relativeOffset, compareBytes, 0, searchBytes.Length);

                            if (CompareSegment(compareBytes, 0, searchBytes))
                            {
                                tempOutput[searchBytes].Add(absoluteOffset + relativeOffset);
                                break;
                            }
                        }
                    } // foreach (byte[] searchBytes in searchByteArrays)

                    relativeOffset++;
                }

                absoluteOffset += Constants.FileReadChunkSize - maxSearchBytesLength;
            }

            // return stream to incoming position
            if (returnStreamToIncomingPosition)
            {
                stream.Position = initialStreamPosition;
            }

            // sort offsets and add to output
            foreach (byte[] key in tempOutput.Keys)
            {
                tempOutput[key].Sort();
                ret.Add(key, (long[])tempOutput[key].ToArray(typeof(long)));
            }

            return ret;
        }

        /// <summary>
        /// Get the offset of the first instance of pSearchBytes after the input offset.
        /// </summary>
        /// <param name="bufferToSearch">Byte array to search.</param>
        /// <param name="offset">Offset to begin searching from.</param>
        /// <param name="searchValue">Bytes to search for.</param>
        /// <returns>Returns the offset of the first instance of pSearchBytes after the input offset or -1 otherwise.</returns>
        public static long GetNextOffset(byte[] bufferToSearch, long offset, byte[] searchValue)
        {
            bool itemFound = false;
            long absoluteOffset = offset;
            byte[] compareBytes;

            long ret = -1;

            while (!itemFound && (absoluteOffset < (bufferToSearch.Length - searchValue.Length)))
            {
                compareBytes = new byte[searchValue.Length];
                Array.Copy(bufferToSearch, absoluteOffset, compareBytes, 0, searchValue.Length);

                if (CompareSegment(compareBytes, 0, searchValue))
                {
                    itemFound = true;
                    ret = absoluteOffset;
                    break;
                }

                absoluteOffset++;
            }

            return ret;
        }

        /// <summary>
        /// Get the offset of the first instance of pSearchBytes before the input offset.
        /// </summary>
        /// <param name="stream">Stream to search.</param>
        /// <param name="offset">Offset to begin searching from.</param>
        /// <param name="searchBytes">Bytes to search for.</param>
        /// <returns>Returns the offset of the first instance of pSearchBytes before the input offset or -1 otherwise.</returns>
        public static long GetPreviousOffset(Stream stream, long offset, byte[] searchBytes)
        {
            long initialStreamPosition = stream.Position;

            bool itemFound = false;
            long relativeOffset;
            byte[] checkBytes = new byte[Constants.FileReadChunkSize];
            byte[] compareBytes;

            long ret = -1;

            long absoluteOffset = offset - Constants.FileReadChunkSize + searchBytes.Length;
            while (!itemFound && (absoluteOffset > -1))
            {
                stream.Position = absoluteOffset;
                relativeOffset = stream.Read(checkBytes, 0, Constants.FileReadChunkSize);

                while (!itemFound && (relativeOffset > -1))
                {
                    if ((relativeOffset + searchBytes.Length) <= checkBytes.Length)
                    {
                        compareBytes = new byte[searchBytes.Length];
                        Array.Copy(checkBytes, relativeOffset, compareBytes, 0, searchBytes.Length);

                        if (CompareSegment(compareBytes, 0, searchBytes))
                        {
                            itemFound = true;
                            ret = absoluteOffset + relativeOffset;

                            if (ret == offset)
                            {
                                ret -= searchBytes.Length;
                            }

                            break;
                        }
                    }
                    
                    relativeOffset--;
                }

                absoluteOffset = absoluteOffset - Constants.FileReadChunkSize + searchBytes.Length;
            }

            // return stream to incoming position
            stream.Position = initialStreamPosition;

            return ret;
        }

        /// <summary>
        /// Compare bytes at input offset to target bytes.
        /// </summary>
        /// <param name="sourceArray">Bytes to compare.</param>
        /// <param name="offset">Offset to begin comparison of pBytes to pTarget.</param>
        /// <param name="target">Target bytes to compare.</param>
        /// <returns>True if the bytes at pOffset match the pTarget bytes.</returns>
        public static bool CompareSegment(byte[] sourceArray, int offset, byte[] target)
        {
            bool ret = true;
            uint j = 0;
            for (int i = offset; i < target.Length; i++)
            {
                if (sourceArray[i] != target[j])
                {
                    ret = false;
                    break;
                }
                
                j++;
            }

            return ret;
        }

        /// <summary>
        /// Compare bytes at input offset to target bytes.
        /// </summary>
        /// <param name="sourceArray">Bytes to compare.</param>
        /// <param name="offset">Offset to begin comparison of pBytes to pTarget.</param>
        /// <param name="target">Target bytes to compare.</param>
        /// <returns>True if the bytes at pOffset match the pTarget bytes.</returns>
        public static bool CompareSegment(byte[] sourceArray, long offset, byte[] target)
        {
            bool ret = true;
            uint j = 0;
            for (long i = offset; i < target.Length; i++)
            {
                if (sourceArray[i] != target[j])
                {
                    ret = false;
                    break;
                }
                
                j++;
            }

            return ret;
        }

        public static void ExtractChunkToFile(Stream stream, long startingOffset, int length, 
            string filePath)
        {
            ExtractChunkToFile(stream, startingOffset, length, filePath, false, false);
        }

        public static void ExtractChunkToFile(Stream stream, long startingOffset, int length,
            string filePath, bool outputLogFile)
        {
            ExtractChunkToFile(stream, startingOffset, length, filePath, outputLogFile, false);
        }

        /// <summary>
        /// Extracts a section of the incoming stream to a file.
        /// </summary>
        /// <param name="stream">Stream to extract from.</param>
        /// <param name="startingOffset">Offset to begin the cut.</param>
        /// <param name="length">Number of bytes to cut.</param>
        /// <param name="filePath">File path to output the extracted chunk to.</param>
        public static void ExtractChunkToFile(
            Stream stream, 
            long startingOffset, 
            int length, 
            string filePath,
            bool outputLogFile,
            bool outputSnakebiteBatchFile)
        {
            bool makeBatchFile = (outputSnakebiteBatchFile && (stream is FileStream));
            StringBuilder logInfo = new StringBuilder();
            StringBuilder snakeBiteBatch = new StringBuilder();
            BinaryWriter bw = null;
            string fullOutputDirectory = Path.GetDirectoryName(Path.GetFullPath(filePath));

            // create output folder if needed
            if (!Directory.Exists(fullOutputDirectory))
            {
                Directory.CreateDirectory(fullOutputDirectory);

                if (outputLogFile)
                {
                    logInfo.AppendLine("Created Directory: " + fullOutputDirectory);
                }
            }

            try
            {
                bw = new BinaryWriter(File.Open(filePath, FileMode.Create, FileAccess.Write));

                int read = 0;
                int totalBytes = 0;
                byte[] bytes = new byte[4096];
                stream.Seek((long)startingOffset, SeekOrigin.Begin);

                int maxread = length > bytes.Length ? bytes.Length : length;

                while ((read = stream.Read(bytes, 0, maxread)) > 0)
                {
                    bw.Write(bytes, 0, read);
                    totalBytes += read;

                    maxread = (length - totalBytes) > bytes.Length ? bytes.Length : (length - totalBytes);
                }

                if (outputLogFile)
                {
                    logInfo.AppendLine(
                        String.Format("Extracted - Offset: 0x{0}    Length: 0x{1}    File: {2}",
                            startingOffset.ToString("X8"),
                            length.ToString("X8"),
                            Path.GetFileName(filePath)));

                    using (StreamWriter logWriter = new StreamWriter(Path.Combine(fullOutputDirectory, ParseFile.LogFileName), true))
                    {
                        logWriter.Write(logInfo.ToString());
                    }
                }

                if (makeBatchFile)
                {
                    snakeBiteBatch.AppendLine(
                        String.Format("snakebite.exe \"{0}\" \"{1}\" 0x{2} 0x{3}",
                            Path.GetFileName(((FileStream)stream).Name),
                            Path.GetFileNameWithoutExtension(((FileStream)stream).Name) + Path.DirectorySeparatorChar + Path.GetFileName(filePath),                                                
                            startingOffset.ToString("X8"),
                            (startingOffset + length - 1).ToString("X8")));                    
                    
                    using (StreamWriter batchWriter = new StreamWriter(Path.Combine(fullOutputDirectory, ParseFile.SnakeBiteBatchFileName), true))
                    {
                        batchWriter.Write(snakeBiteBatch.ToString());
                    }                
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

        /// <summary>
        /// Convert the input bytes to a string containing the hex values.
        /// </summary>
        /// <param name="value">Bytes to convert to a string.</param>
        /// <returns>String of hex values that represent the incoming byte array.</returns>
        public static string ByteArrayToString(byte[] value)
        {
            StringBuilder checksum = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < value.Length; i++)
            {
                checksum.Append(value[i].ToString("X2", CultureInfo.InvariantCulture));
            }

            return checksum.ToString();
        }

        /// <summary>
        /// Find an offset and cut the file based on incoming criteria.
        /// </summary>
        /// <param name="sourcePath">Path of file to search.</param>
        /// <param name="searchCriteria">Struct containing search criteria.</param>
        /// <param name="messages">Output messages.</param>
        /// <returns>Directory that extracted files were output into.</returns>
        public static string FindOffsetAndCutFile(string sourcePath, FindOffsetStruct searchCriteria, out string messages, bool outputLog, bool outputBatchFile)
        {
            int i;
            int j = 0;
            byte[] searchBytes;
            byte[] terminatorBytes = null;
            System.Text.Encoding enc = System.Text.Encoding.ASCII;

            long cutStart;
            long cutSize = 0;
            long cutSizeOffset;
            byte[] cutSizeBytes;
            long minimumCutSize = -1;

            long previousPosition;
            string outputFolder;
            string outputFile;
            int chunkCount = 0;

            long offset;
            long searchStringModuloDivisor = 0;
            long searchStringModuloResult = 0;
            long previousOffset;

            long terminatorOffset;
            long terminatorModuloDivisor = 0;
            long terminatorModuloResult = 0;

            bool skipCut;

            StringBuilder ret = new StringBuilder();

            // create search bytes
            if (searchCriteria.TreatSearchStringAsHex)
            {
                searchBytes = new byte[searchCriteria.SearchString.Length / 2];

                // convert the search string to bytes
                for (i = 0; i < searchCriteria.SearchString.Length; i += 2)
                {
                    searchBytes[j] = BitConverter.GetBytes(Int16.Parse(searchCriteria.SearchString.Substring(i, 2), System.Globalization.NumberStyles.AllowHexSpecifier, CultureInfo.CurrentCulture))[0];
                    j++;
                }
            }
            else
            {
                searchBytes = enc.GetBytes(searchCriteria.SearchString);
            }

            // parse minimum cut size
            if (!String.IsNullOrEmpty(searchCriteria.MinimumSize))
            {
                minimumCutSize = VGMToolbox.util.Encoding.GetLongValueFromString(searchCriteria.MinimumSize);
            }

            // parse Search String modulo information
            if (searchCriteria.DoSearchStringModulo)
            {
                searchStringModuloDivisor = VGMToolbox.util.Encoding.GetLongValueFromString(searchCriteria.SearchStringModuloDivisor);
                searchStringModuloResult = VGMToolbox.util.Encoding.GetLongValueFromString(searchCriteria.SearchStringModuloResult);
            }

            // create terminator bytes
            j = 0;
            if (searchCriteria.TreatTerminatorStringAsHex)
            {
                terminatorBytes = new byte[searchCriteria.TerminatorString.Length / 2];

                // convert the search string to bytes
                for (i = 0; i < searchCriteria.TerminatorString.Length; i += 2)
                {
                    terminatorBytes[j] = BitConverter.GetBytes(Int16.Parse(searchCriteria.TerminatorString.Substring(i, 2), System.Globalization.NumberStyles.AllowHexSpecifier, CultureInfo.CurrentCulture))[0];
                    j++;
                }
            }
            else if (!String.IsNullOrEmpty(searchCriteria.TerminatorString))
            {
                terminatorBytes = enc.GetBytes(searchCriteria.TerminatorString);
            }

            FileInfo fi = new FileInfo(sourcePath);

            using (FileStream fs = File.Open(Path.GetFullPath(sourcePath), FileMode.Open, FileAccess.Read))
            {
                ret.AppendFormat("[{0}]", sourcePath);
                ret.Append(Environment.NewLine);

                // setup starting offset
                previousOffset = 
                    String.IsNullOrEmpty(searchCriteria.StartingOffset) ? 0 : VGMToolbox.util.Encoding.GetLongValueFromString(searchCriteria.StartingOffset);
                
                // build output folder path
                if (String.IsNullOrEmpty(searchCriteria.OutputFolder))
                {
                    outputFolder = Path.GetFullPath(
                        Path.Combine(
                            Path.GetDirectoryName(sourcePath),
                            Path.GetFileNameWithoutExtension(sourcePath) + "_CUT"));
                }
                else
                {
                    outputFolder = searchCriteria.OutputFolder;
                }

                // search for our string
                // while ((offset = ParseFile.GetNextOffset(fs, previousOffset, searchBytes)) != -1)
                while ((offset = ParseFile.GetNextOffset(fs, previousOffset, searchBytes, 
                    searchCriteria.DoSearchStringModulo, searchStringModuloDivisor, 
                    searchStringModuloResult)) != -1)
                {
                    // do cut file tasks
                    if (searchCriteria.CutFile)
                    {
                        skipCut = false;

                        cutStart = offset - VGMToolbox.util.Encoding.GetLongValueFromString(searchCriteria.SearchStringOffset);

                        // determine cut size from value at offset
                        if (searchCriteria.IsCutSizeAnOffset)
                        {
                            cutSizeOffset = cutStart + VGMToolbox.util.Encoding.GetLongValueFromString(searchCriteria.CutSize);
                            previousPosition = fs.Position;
                            cutSizeBytes = ParseFile.ParseSimpleOffset(
                                fs, 
                                cutSizeOffset,
                                (int)VGMToolbox.util.Encoding.GetLongValueFromString(searchCriteria.CutSizeOffsetSize));
                            fs.Position = previousPosition;

                            if (!searchCriteria.IsLittleEndian)
                            {
                                Array.Reverse(cutSizeBytes);
                            }

                            switch (cutSizeBytes.Length)
                            {
                                case 1:
                                    cutSize = cutSizeBytes[0];
                                    break;
                                case 2:
                                    cutSize = BitConverter.ToInt16(cutSizeBytes, 0);
                                    break;
                                case 4:
                                    cutSize = BitConverter.ToInt32(cutSizeBytes, 0);
                                    break;
                                default:
                                    cutSize = 0;
                                    break;
                            }
                        }
                        else if (searchCriteria.UseTerminatorForCutSize) // look for terminator
                        {
                            if (cutStart >= 0)
                            {                                                                
                                if (searchCriteria.DoTerminatorModulo)
                                {
                                    terminatorModuloDivisor = VGMToolbox.util.Encoding.GetLongValueFromString(searchCriteria.TerminatorStringModuloDivisor);
                                    terminatorModuloResult = VGMToolbox.util.Encoding.GetLongValueFromString(searchCriteria.TerminatorStringModuloResult);
                                }

                                terminatorOffset = GetNextOffset(fs, offset + 1, terminatorBytes,
                                    searchCriteria.DoTerminatorModulo, terminatorModuloDivisor,
                                    terminatorModuloResult);
                                
                                 cutSize = terminatorOffset - cutStart;

                                if (searchCriteria.IncludeTerminatorLength)
                                {
                                    cutSize += terminatorBytes.Length;
                                }                          
                            }
                        }
                        else // static size
                        {
                            cutSize = VGMToolbox.util.Encoding.GetLongValueFromString(searchCriteria.CutSize);
                        }

                        outputFile = String.Format(CultureInfo.InvariantCulture, "{0}_{1}{2}", Path.GetFileNameWithoutExtension(sourcePath), chunkCount.ToString("X8", CultureInfo.InvariantCulture), searchCriteria.OutputFileExtension);

                        if (cutStart < 0)
                        {
                            ret.AppendFormat(
                                CultureInfo.CurrentCulture, 
                                "  Warning: For string found at: 0x{0}, cut begin is less than 0 ({1})...Skipping",
                                offset.ToString("X8", CultureInfo.InvariantCulture), 
                                cutStart.ToString("X8", CultureInfo.InvariantCulture));
                            ret.Append(Environment.NewLine);

                            skipCut = true;
                        }
                        else if (cutSize < 1)
                        {
                            ret.AppendFormat(
                                CultureInfo.CurrentCulture, 
                                "  Warning: For string found at: 0x{0}, cut size is less than 1 ({1})...Skipping",
                                offset.ToString("X8", CultureInfo.InvariantCulture), 
                                cutSize.ToString("X8", CultureInfo.InvariantCulture));
                            ret.Append(Environment.NewLine);

                            skipCut = true;
                        }
                        else if ((cutStart + cutSize) > fi.Length)
                        {
                            ret.AppendFormat(
                                CultureInfo.CurrentCulture, 
                                "  Warning: For string found at: 0x{0}, total file end will go past the end of the file ({1})",
                                offset.ToString("X8", CultureInfo.InvariantCulture), 
                                (cutStart + cutSize).ToString("X8", CultureInfo.InvariantCulture));
                            ret.Append(Environment.NewLine);
                        }

                        if (skipCut)
                        {
                            previousOffset = offset + 1;
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(searchCriteria.ExtraCutSizeBytes))
                            {
                                cutSize += (long)VGMToolbox.util.Encoding.GetLongValueFromString(searchCriteria.ExtraCutSizeBytes);
                            }

                            // check minimum cut size
                            if (((minimumCutSize > 0) && (cutSize >= minimumCutSize)) ||
                                (minimumCutSize < 1))
                            {
                                ParseFile.ExtractChunkToFile(fs, cutStart, (int)cutSize, Path.Combine(outputFolder, outputFile), outputLog, outputBatchFile);

                                ret.AppendFormat(
                                    CultureInfo.CurrentCulture,
                                    "  Extracted [{3}] begining at 0x{0}, for string found at: 0x{1}, with size 0x{2}",
                                    cutStart.ToString("X8", CultureInfo.InvariantCulture),
                                    offset.ToString("X8", CultureInfo.InvariantCulture),
                                    cutSize.ToString("X8", CultureInfo.InvariantCulture),
                                    outputFile);
                                ret.Append(Environment.NewLine);

                                chunkCount++;
                            }

                            previousOffset = cutStart + cutSize;                            
                        }
                    }
                    else // just output text
                    {
                        // just append the offset
                        ret.AppendFormat(CultureInfo.CurrentCulture, "  String found at: 0x{0}", offset.ToString("X8", CultureInfo.InvariantCulture));
                        ret.Append(Environment.NewLine);

                        previousOffset = offset + searchBytes.Length;
                    }
                }
            }

            messages = ret.ToString();
            return outputFolder;
        }
    }
}
