using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace VGMToolbox.util
{        
    public sealed class ParseFile
    {
        private ParseFile() { }
                
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
            byte[] checkBytes = new byte[Constants.FILE_READ_CHUNK_SIZE];
            byte[] compareBytes;

            long ret = -1;

            while (!itemFound && (absoluteOffset < pStream.Length))
            {
                pStream.Position = absoluteOffset;
                pStream.Read(checkBytes, 0, Constants.FILE_READ_CHUNK_SIZE);
                relativeOffset = 0;

                while (!itemFound && (relativeOffset < Constants.FILE_READ_CHUNK_SIZE))
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

                absoluteOffset += (Constants.FILE_READ_CHUNK_SIZE - pSearchBytes.Length);
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

        public static string ByteArrayToString(byte[] pBytes)
        {
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < pBytes.Length; i++)
            {
                sBuilder.Append(pBytes[i].ToString("X2", CultureInfo.InvariantCulture));
            }

            return sBuilder.ToString();
        }

        public static string FindOffsetAndCutFile(string pPath, FindOffsetStruct pFindOffsetStruct, out string pMessages)
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
            if (pFindOffsetStruct.TreatSearchStringAsHex)
            {
                searchBytes = new byte[pFindOffsetStruct.SearchString.Length / 2];

                // convert the search string to bytes
                for (i = 0; i < pFindOffsetStruct.SearchString.Length; i += 2)
                {
                    searchBytes[j] = BitConverter.GetBytes(Int16.Parse(pFindOffsetStruct.SearchString.Substring(i, 2), System.Globalization.NumberStyles.AllowHexSpecifier, CultureInfo.CurrentCulture))[0];
                    j++;
                }
            }
            else
            {
                searchBytes = enc.GetBytes(pFindOffsetStruct.SearchString);
            }

            // create terminator bytes
            if (pFindOffsetStruct.TreatTerminatorStringAsHex)
            {
                terminatorBytes = new byte[pFindOffsetStruct.TerminatorString.Length / 2];

                // convert the search string to bytes
                for (i = 0; i < pFindOffsetStruct.SearchString.Length; i += 2)
                {
                    terminatorBytes[j] = BitConverter.GetBytes(Int16.Parse(pFindOffsetStruct.TerminatorString.Substring(i, 2), System.Globalization.NumberStyles.AllowHexSpecifier, CultureInfo.CurrentCulture))[0];
                    j++;
                }
            }
            else if (!String.IsNullOrEmpty(pFindOffsetStruct.TerminatorString))
            {
                terminatorBytes = enc.GetBytes(pFindOffsetStruct.TerminatorString);
            }
            
            FileInfo fi = new FileInfo(pPath);

            using (FileStream fs = File.Open(Path.GetFullPath(pPath), FileMode.Open, FileAccess.Read))
            {
                ret.AppendFormat("[{0}]", pPath);
                ret.Append(Environment.NewLine);

                previousOffset = 0;
                outputFolder = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(pPath),
                    Path.GetFileNameWithoutExtension(pPath) + "_CUT"));

                while ((offset = ParseFile.GetNextOffset(fs, previousOffset, searchBytes)) != -1)
                {
                    if (pFindOffsetStruct.CutFile)
                    {
                        skipCut = false;

                        cutStart = offset - VGMToolbox.util.Encoding.GetIntFromString(pFindOffsetStruct.SearchStringOffset);

                        if (pFindOffsetStruct.IsCutSizeAnOffset)
                        {
                            cutSizeOffset = cutStart + VGMToolbox.util.Encoding.GetIntFromString(pFindOffsetStruct.CutSize);
                            previousPosition = fs.Position;
                            cutSizeBytes = ParseFile.parseSimpleOffset(fs, cutSizeOffset,
                                (int)VGMToolbox.util.Encoding.GetIntFromString(pFindOffsetStruct.CutSizeOffsetSize));
                            fs.Position = previousPosition;

                            if (!pFindOffsetStruct.IsLittleEndian)
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
                        else if (pFindOffsetStruct.UseTerminatorForCutsize)
                        {
                            if (cutStart >= 0)
                            {
                                cutSize = GetNextOffset(fs, offset, terminatorBytes) - cutStart;

                                if (pFindOffsetStruct.IncludeTerminatorLength)
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
                            cutSize = VGMToolbox.util.Encoding.GetIntFromString(pFindOffsetStruct.CutSize);
                        }

                        outputFile = String.Format(CultureInfo.InvariantCulture, "{0}_{1}{2}", Path.GetFileNameWithoutExtension(pPath), chunkCount.ToString("X8", CultureInfo.InvariantCulture), pFindOffsetStruct.OutputFileExtension);

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
                            if (!String.IsNullOrEmpty(pFindOffsetStruct.ExtraCutSizeBytes))
                            {
                                cutSize += (long)VGMToolbox.util.Encoding.GetIntFromString(pFindOffsetStruct.ExtraCutSizeBytes);
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
