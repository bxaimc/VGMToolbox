using System;
using System.Collections.Generic;
using System.Text;

namespace VGMToolbox.util
{
    public class Encoding
    {
        public const int CODEPAGE_JP = 932;
        public const int CODEPAGE_US = 1251;
        
        public static string getEncodedText(byte[] pBytes, int codePage)
        {
            return System.Text.Encoding.GetEncoding(codePage).GetString(pBytes);
        }

        public static string getJpEncodedText(byte[] pBytes)
        {
            return getEncodedText(pBytes, CODEPAGE_JP);
        }
        public static string getUSEncodedText(byte[] pBytes)
        {
            return getEncodedText(pBytes, CODEPAGE_US);
        }

        public static long GetIntFromString(string pStringNumber)
        {
            long ret;

            if (pStringNumber.StartsWith("0x"))
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
    }
}
