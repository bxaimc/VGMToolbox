using System;
using System.Collections.Generic;
using System.Text;

namespace VGMToolbox.util
{
    /// <summary>
    /// Class containing static text conversion functions.
    /// </summary>
    public class Encoding
    {
        /// <summary>
        /// Codepage value for Shift JIS (Jp)
        /// </summary>
        public const int CODEPAGE_JP = 932;
        /// <summary>
        /// Codepage value for Cyrillic (US)
        /// </summary>
        public const int CODEPAGE_US = 1251;

        private Encoding() { }

        /// <summary>
        /// Get string from bytes.
        /// </summary>
        /// <param name="pBytes">Bytes to convert to a string.</param>
        /// <param name="codePage">Codepage to use in converting bytes.</param>
        /// <returns>String encoding using the input Codepage.</returns>
        public static string GetEncodedText(byte[] pBytes, int codePage)
        {
            return System.Text.Encoding.GetEncoding(codePage).GetString(pBytes);
        }

        /// <summary>
        /// Get text encoded in Shift JIS
        /// </summary>
        /// <param name="pBytes">Bytes to decode.</param>
        /// <returns>String encoded using the Shift JIS codepage.</returns>
        public static string GetJpEncodedText(byte[] pBytes)
        {
            return GetEncodedText(pBytes, CODEPAGE_JP);
        }
        /// <summary>
        /// Get text encoded in Cyrillic
        /// </summary>
        /// <param name="pBytes">Bytes to decode.</param>
        /// <returns>String encoded using the Cyrillic codepage.</returns>
        public static string GetUSEncodedText(byte[] pBytes)
        {
            return GetEncodedText(pBytes, CODEPAGE_US);
        }

        /// <summary>
        /// Convert input string to a long.  Works for Decimal and Hexidecimal (use 0x prefix).
        /// </summary>
        /// <param name="pStringNumber">String containing a Decimal and Hexidecimal number.</param>
        /// <returns>Long representing the input string.</returns>
        public static long GetLongFromString(string pStringNumber)
        {
            long ret;

            if (pStringNumber.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase))
            {
                pStringNumber = pStringNumber.Substring(2);
                ret = long.Parse(pStringNumber, System.Globalization.NumberStyles.HexNumber, null);
            }
            else
            {
                ret = long.Parse(pStringNumber, System.Globalization.NumberStyles.Integer, null);
            }

            return ret;
        }

        /// <summary>
        /// Get the UInt32 Value of the Incoming Byte Array, which is in Big Endian order.
        /// </summary>
        /// <param name="pBytes">Bytes to convert.</param>
        /// <returns>The UInt32 Value of the Incoming Byte Array.</returns>
        public static UInt32 GetUint32BigEndian(byte[] pBytes)
        {
            byte[] workingArray = new byte[pBytes.Length];
            Array.Copy(pBytes, 0, workingArray, 0, pBytes.Length); 
            
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
        public static UInt16 GetUint16BigEndian(byte[] pBytes)
        {
            byte[] workingArray = new byte[pBytes.Length];
            Array.Copy(pBytes, 0, workingArray, 0, pBytes.Length); 
            
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(workingArray);
            }
            return BitConverter.ToUInt16(workingArray, 0);
        }
    }
}
