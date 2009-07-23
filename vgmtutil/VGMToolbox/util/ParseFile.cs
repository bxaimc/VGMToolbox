using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace VGMToolbox.util
{        
    public sealed class ParseFile
    {
        private ParseFile() { }
                
        /// <summary>
        /// Extract a section from the incoming byte array.
        /// </summary>
        /// <param name="pBytes">Bytes to extract from.</param>
        /// <param name="pOffset">Offset to begin cutting from.</param>
        /// <param name="pLength">Number of bytes to cut.</param>
        /// <returns>Byte array containing the extracted section.</returns>
        public static byte[] ParseSimpleOffset(byte[] sourceArray, int offset, int length)
        {
            byte[] ret = new byte[length];
            uint j = 0;

            for (int i = offset; i < offset + length; i++)
            {
                ret[j] = sourceArray[i];
                j++;
            }

            return ret;
        }

        /// <summary>
        /// Extract a section from the incoming stream.
        /// </summary>
        /// <param name="pFileStream">Stream to extract the chunk from.</param>
        /// <param name="pOffset">Offset to begin cutting from.</param>
        /// <param name="pLength">Number of bytes to cut.</param>
        /// <returns>Byte array containing the extracted section.</returns>
        public static byte[] ParseSimpleOffset(Stream pFileStream, int pOffset, int pLength)
        {
            byte[] ret = new byte[pLength];
            long currentStreamPosition = pFileStream.Position;

            pFileStream.Seek((long)pOffset, SeekOrigin.Begin);
            BinaryReader br = new BinaryReader(pFileStream);
            ret = br.ReadBytes((int)pLength);

            pFileStream.Position = currentStreamPosition;

            return ret;
        }

        /// <summary>
        /// Extract a section from the incoming stream.
        /// </summary>
        /// <param name="pFileStream">Stream to extract the chunk from.</param>
        /// <param name="pOffset">Offset to begin cutting from.</param>
        /// <param name="pLength">Number of bytes to cut.</param>
        /// <returns>Byte array containing the extracted section.</returns>
        public static byte[] ParseSimpleOffset(Stream pFileStream, long pOffset, int pLength)
        {
            byte[] ret = new byte[pLength];
            long currentStreamPosition = pFileStream.Position;

            pFileStream.Seek(pOffset, SeekOrigin.Begin);
            BinaryReader br = new BinaryReader(pFileStream);
            ret = br.ReadBytes(pLength);

            pFileStream.Position = currentStreamPosition;

            return ret;
        }

        /// <summary>
        /// Get the length from the input offset to the location of the input terminator bytes or zero.
        /// </summary>
        /// <param name="pBytes">Bytes to check.</param>
        /// <param name="pOffset">Offset at which to begin searching for the terminator bytes.</param>
        /// <param name="pTerminator">Bytes to search for.</param>
        /// <returns>Length of distance between offset and terminator or zero if not found.</returns>
        public static int GetSegmentLength(byte[] pBytes, int pOffset, byte[] pTerminator)
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

        /// <summary>
        /// Get the length from the input offset to the location of the input terminator bytes or zero.
        /// </summary>
        /// <param name="pStream">Stream to check.</param>
        /// <param name="pOffset">Offset at which to begin searching for the terminator bytes.</param>
        /// <param name="pTerminator">Bytes to search for.</param>
        /// <returns>Length of distance between offset and terminator or zero if not found.</returns>
        public static int GetSegmentLength(Stream pStream, int pOffset, byte[] pTerminator)
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

        /// <summary>
        /// Get the offset of the first instance of pSearchBytes after the input offset.
        /// </summary>
        /// <param name="pStream">Stream to search.</param>
        /// <param name="pOffset">Offset to begin searching from.</param>
        /// <param name="pSearchBytes">Bytes to search for.</param>
        /// <returns>Returns the offset of the first instance of pSearchBytes after the input offset or -1 otherwise.</returns>
        public static long GetNextOffset(Stream pStream, long pOffset, byte[] pSearchBytes)
        {
            long initialStreamPosition = pStream.Position;

            bool itemFound = false;
            long absoluteOffset = pOffset;
            long relativeOffset;
            byte[] checkBytes = new byte[Constants.FileReadChunkSize];
            byte[] compareBytes;

            long ret = -1;

            while (!itemFound && (absoluteOffset < pStream.Length))
            {
                pStream.Position = absoluteOffset;
                pStream.Read(checkBytes, 0, Constants.FileReadChunkSize);
                relativeOffset = 0;

                while (!itemFound && (relativeOffset < Constants.FileReadChunkSize))
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

                absoluteOffset += (Constants.FileReadChunkSize - pSearchBytes.Length);
            }

            // return stream to incoming position
            pStream.Position = initialStreamPosition;

            return ret;
        }

        /// <summary>
        /// Get the offset of the first instance of pSearchBytes after the input offset.
        /// </summary>
        /// <param name="pBufferToSearch">Byte array to search.</param>
        /// <param name="pOffset">Offset to begin searching from.</param>
        /// <param name="pSearchBytes">Bytes to search for.</param>
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
        /// <param name="pStream">Stream to search.</param>
        /// <param name="pOffset">Offset to begin searching from.</param>
        /// <param name="pSearchBytes">Bytes to search for.</param>
        /// <returns>Returns the offset of the first instance of pSearchBytes before the input offset or -1 otherwise.</returns>
        public static long GetPreviousOffset(Stream pStream, long offset, byte[] pSearchBytes)
        {
            long initialStreamPosition = pStream.Position;

            bool itemFound = false;
            long relativeOffset;
            byte[] checkBytes = new byte[Constants.FileReadChunkSize];
            byte[] compareBytes;

            long ret = -1;

            long absoluteOffset = offset - (Constants.FileReadChunkSize) + pSearchBytes.Length;
            while (!itemFound && (absoluteOffset > -1))
            {
                pStream.Position = absoluteOffset;
                relativeOffset = pStream.Read(checkBytes, 0, Constants.FileReadChunkSize);

                while (!itemFound && (relativeOffset > -1))
                {
                    if ((relativeOffset + pSearchBytes.Length) <= checkBytes.Length)
                    {
                        compareBytes = new byte[pSearchBytes.Length];
                        Array.Copy(checkBytes, relativeOffset,
                            compareBytes, 0, pSearchBytes.Length);

                        if (CompareSegment(compareBytes, 0, pSearchBytes))
                        {
                            itemFound = true;
                            // ret = absoluteOffset + relativeOffset - pSearchBytes.Length;
                            ret = absoluteOffset + relativeOffset;

                            if (ret == offset)
                            {
                                ret -= pSearchBytes.Length;
                            }

                            break;
                        }
                    }
                    relativeOffset--;
                }

                absoluteOffset = absoluteOffset - Constants.FileReadChunkSize + pSearchBytes.Length;
            }

            // return stream to incoming position
            pStream.Position = initialStreamPosition;

            return ret;
        }

        /// <summary>
        /// Compare bytes at input offset to target bytes.
        /// </summary>
        /// <param name="pBytes">Bytes to compare.</param>
        /// <param name="pOffset">Offset to begin comparison of pBytes to pTarget.</param>
        /// <param name="pTarget">Target bytes to compare.</param>
        /// <returns>True if the bytes at pOffset match the pTarget bytes.</returns>
        public static bool CompareSegment(byte[] sourceArray, int offset, byte[] target)
        {
            Boolean ret = true;
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
        /// <param name="pBytes">Bytes to compare.</param>
        /// <param name="pOffset">Offset to begin comparison of pBytes to pTarget.</param>
        /// <param name="pTarget">Target bytes to compare.</param>
        /// <returns>True if the bytes at pOffset match the pTarget bytes.</returns>
        public static bool CompareSegment(byte[] sourceArray, long offset, byte[] target)
        {
            Boolean ret = true;
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

        /// <summary>
        /// Extracts a section of the incoming stream to a file.
        /// </summary>
        /// <param name="pStream">Stream to extract from.</param>
        /// <param name="pOffset">Offset to begin the cut.</param>
        /// <param name="pLength">Number of bytes to cut.</param>
        /// <param name="pFilePath">File path to output the extracted chunk to.</param>
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

        /// <summary>
        /// Convert the input bytes to a string containing the hex values.
        /// </summary>
        /// <param name="pBytes">Bytes to convert to a string.</param>
        /// <returns>String of hex values that represent the incoming byte array.</returns>
        public static string ByteArrayToString(byte[] value)
        {
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < value.Length; i++)
            {
                sBuilder.Append(value[i].ToString("X2", CultureInfo.InvariantCulture));
            }

            return sBuilder.ToString();
        }

        /// <summary>
        /// Find an offset and cut the file based on incoming criteria.
        /// </summary>
        /// <param name="pPath">Path of file to search.</param>
        /// <param name="pFindOffsetStruct">Struct containing search criteria.</param>
        /// <param name="pMessages">Output messages.</param>
        /// <returns>Directory that extracted files were output into.</returns>
        public static string FindOffsetAndCutFile(string sourcePath, FindOffsetStruct searchCriteria, out string pMessages)
        {
            int i;
            int j = 0;
            byte[] searchBytes;
            byte[] terminatorBytes = null;
            System.Text.Encoding enc = System.Text.Encoding.ASCII;

            long cutStart;
            long cutSize;
            long cutSizeOffset;
            byte[] cutSizeBytes;

            long previousPosition;
            string outputFolder;
            string outputFile;
            int chunkCount = 0;

            long offset;
            long previousOffset;

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

                previousOffset = 0;
                outputFolder = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(sourcePath),
                    Path.GetFileNameWithoutExtension(sourcePath) + "_CUT"));

                while ((offset = ParseFile.GetNextOffset(fs, previousOffset, searchBytes)) != -1)
                {
                    if (searchCriteria.CutFile)
                    {
                        skipCut = false;

                        cutStart = offset - VGMToolbox.util.Encoding.GetLongValueFromString(searchCriteria.SearchStringOffset);

                        if (searchCriteria.IsCutSizeAnOffset)
                        {
                            cutSizeOffset = cutStart + VGMToolbox.util.Encoding.GetLongValueFromString(searchCriteria.CutSize);
                            previousPosition = fs.Position;
                            cutSizeBytes = ParseFile.ParseSimpleOffset(fs, cutSizeOffset,
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
                        else if (searchCriteria.UseTerminatorForCutSize)
                        {
                            if (cutStart >= 0)
                            {
                                cutSize = GetNextOffset(fs, offset + 1, terminatorBytes) - cutStart;

                                if (searchCriteria.IncludeTerminatorLength)
                                {
                                    cutSize += terminatorBytes.Length;
                                }
                            }
                            else
                            {
                                cutSize = 0;
                            }
                        }
                        else
                        {
                            cutSize = VGMToolbox.util.Encoding.GetLongValueFromString(searchCriteria.CutSize);
                        }

                        outputFile = String.Format(CultureInfo.InvariantCulture, "{0}_{1}{2}", Path.GetFileNameWithoutExtension(sourcePath), chunkCount.ToString("X8", CultureInfo.InvariantCulture), searchCriteria.OutputFileExtension);

                        if (cutStart < 0)
                        {
                            ret.AppendFormat(CultureInfo.CurrentCulture, "  Warning: For string found at: 0x{0}, cut begin is less than 0 ({1})...Skipping",
                                offset.ToString("X8", CultureInfo.InvariantCulture), cutStart.ToString("X8", CultureInfo.InvariantCulture));
                            ret.Append(Environment.NewLine);

                            skipCut = true;
                        }
                        else if (cutSize < 1)
                        {
                            ret.AppendFormat(CultureInfo.CurrentCulture, "  Warning: For string found at: 0x{0}, cut size is less than 1 ({1})...Skipping",
                                offset.ToString("X8", CultureInfo.InvariantCulture), cutSize.ToString("X8", CultureInfo.InvariantCulture));
                            ret.Append(Environment.NewLine);

                            skipCut = true;
                        }
                        else if ((cutStart + cutSize) > fi.Length)
                        {
                            ret.AppendFormat(CultureInfo.CurrentCulture, "  Warning: For string found at: 0x{0}, total file end will go past the end of the file ({1})",
                                offset.ToString("X8", CultureInfo.InvariantCulture), (cutStart + cutSize).ToString("X8", CultureInfo.InvariantCulture));
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
                            
                            ParseFile.ExtractChunkToFile(fs, cutStart, (int)cutSize, Path.Combine(outputFolder, outputFile));
                            
                            ret.AppendFormat(CultureInfo.CurrentCulture, "  Extracted [{3}] begining at 0x{0}, for string found at: 0x{1}, with size 0x{2}",
                                cutStart.ToString("X8", CultureInfo.InvariantCulture), offset.ToString("X8", CultureInfo.InvariantCulture), cutSize.ToString("X8", CultureInfo.InvariantCulture), outputFile);
                            ret.Append(Environment.NewLine);

                            previousOffset = cutStart + cutSize;
                            chunkCount++;
                        }
                    }
                    else
                    {
                        // just append the offset
                        ret.AppendFormat(CultureInfo.CurrentCulture, "  String found at: 0x{0}", offset.ToString("X8", CultureInfo.InvariantCulture));
                        ret.Append(Environment.NewLine);

                        previousOffset = offset + searchBytes.Length;
                    }
                }
            }

            pMessages = ret.ToString();
            return outputFolder;
        }
    }
}
