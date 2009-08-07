using System;
using System.Collections.Generic;
using System.Text;

namespace VGMToolbox.util
{    
    /// <summary>
    /// Class containing static text conversion functions.
    /// </summary>
    public sealed class Encoding
    {
        /// <summary>
        /// Codepage value for Shift JIS (Jp)
        /// </summary>
        public const int CodePageJapan = 932;
        /// <summary>
        /// Codepage value for Cyrillic (US)
        /// </summary>
        public const int CodePageUnitedStates = 1251;

        /// <summary>
        /// Codepage value for OEM DOS
        /// </summary>
        public const int CodePageOEM = 437;

        private Encoding() { }

        /// <summary>
        /// Get string from bytes.
        /// </summary>
        /// <param name="pBytes">Bytes to convert to a string.</param>
        /// <param name="codePage">Codepage to use in converting bytes.</param>
        /// <returns>String encoding using the input Codepage.</returns>
        public static string GetEncodedText(byte[] value, int codePage)
        {
            return System.Text.Encoding.GetEncoding(codePage).GetString(value);
        }

        /// <summary>
        /// Get text encoded in Shift JIS
        /// </summary>
        /// <param name="pBytes">Bytes to decode.</param>
        /// <returns>String encoded using the Shift JIS codepage.</returns>
        public static string GetJapaneseEncodedText(byte[] value)
        {
            return GetEncodedText(value, CodePageJapan);
        }
        /// <summary>
        /// Get text encoded in Cyrillic
        /// </summary>
        /// <param name="pBytes">Bytes to decode.</param>
        /// <returns>String encoded using the Cyrillic codepage.</returns>
        public static string GetUnitedStatesEncodedText(byte[] value)
        {
            return GetEncodedText(value, CodePageUnitedStates);
        }

        /// <summary>
        /// Get text encoded in ASCII
        /// </summary>
        /// <param name="pBytes">Bytes to decode.</param>
        /// <returns>String encoded using ASCII.</returns>
        public static string GetAsciiText(byte[] value)
        {
            System.Text.Encoding ascii = System.Text.Encoding.ASCII;
            return ascii.GetString(value);
        }

        /// <summary>
        /// Convert input string to a long.  Works for Decimal and Hexidecimal (use 0x prefix).
        /// </summary>
        /// <param name="pStringNumber">String containing a Decimal and Hexidecimal number.</param>
        /// <returns>Long representing the input string.</returns>
        public static long GetLongValueFromString(string value)
        {
            long ret;

            if (value.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase))
            {
                value = value.Substring(2);
                ret = long.Parse(value, System.Globalization.NumberStyles.HexNumber, null);
            }
            else
            {
                ret = long.Parse(value, System.Globalization.NumberStyles.Integer, null);
            }

            return ret;
        }

        /// <summary>
        /// Get the UInt32 Value of the Incoming Byte Array, which is in Big Endian order.
        /// </summary>
        /// <param name="pBytes">Bytes to convert.</param>
        /// <returns>The UInt32 Value of the Incoming Byte Array.</returns>
        public static UInt32 GetUInt32BigEndian(byte[] value)
        {
            byte[] workingArray = new byte[value.Length];
            Array.Copy(value, 0, workingArray, 0, value.Length); 
            
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(workingArray);
            }
            return BitConverter.ToUInt32(workingArray, 0);
        }

        /// <summary>
        /// Get the UInt16 Value of the Incoming Byte Array, which is in Big Endian order.
        /// </summary>
        /// <param name="pBytes">Bytes to convert.</param>
        /// <returns>The UInt16 Value of the Incoming Byte Array.</returns>
        public static UInt16 GetUInt16BigEndian(byte[] value)
        {
            byte[] workingArray = new byte[value.Length];
            Array.Copy(value, 0, workingArray, 0, value.Length); 
            
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(workingArray);
            }
            return BitConverter.ToUInt16(workingArray, 0);
        }

        /// <summary>
        /// Predicts Code Page between Cyrillic and Shift-JIS based on whether high ASCII is included or not.
        /// </summary>
        /// <param name="tagBytes">Bytes containing the tags in an unknown language.</param>
        /// <returns>Integer representing the predicted code page.</returns>
        public static int GetPredictedCodePageForTags(byte[] tagBytes)
        {
            int predictedCodePage = CodePageUnitedStates;

            foreach (byte b in tagBytes)
            {
                if ((int)b > 0x7F)
                {
                    predictedCodePage = CodePageJapan;
                    break;
                }
            }

            return predictedCodePage;
        }
    }
}
