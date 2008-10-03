using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

using ICSharpCode.SharpZipLib.Checksums;

using VGMToolbox.util.ObjectPooling;

namespace VGMToolbox.format
{
    interface IFormat
    {
        byte[] GetAsciiSignature();
        string GetFileExtensions();  // Should return values only if getAsciiSignature returns NULL
        
        void GetDatFileCrc32(string pPath, ref Dictionary<string, ByteArray> pLibHash,
            ref Crc32 pChecksum, bool pUseLibHash);        
        //void getDatFileCrc32(string pPath, ref Dictionary<string, ByteArray> pLibHash, 
        //    ref Crc32 pChecksum, ref CryptoStream pMd5CryptoStream, ref CryptoStream pSha1CryptoStream, 
        //    bool pUseLibHash);
        string GetFormatAbbreviation();
        //void initialize(byte[] pBytes);
        void Initialize(Stream pStream);
        bool IsFileLibrary(string pPath);
        bool HasMultipleFileExtensions();

        int GetStartingSong();
        int GetTotalSongs();
        string GetSongName();

        string GetHootDriverAlias();
        string GetHootDriverType();
        string GetHootDriver();

        Dictionary<string, string> GetTagHash();
    }
}
