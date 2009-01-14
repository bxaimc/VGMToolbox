using System.Collections.Generic;
using System.IO;

using ICSharpCode.SharpZipLib.Checksums;

using VGMToolbox.util.ObjectPooling;

namespace VGMToolbox.format
{
    public interface IFormat
    {
        string FilePath { get; set; }
        
        byte[] GetAsciiSignature();
        string GetFileExtensions();  // Should return values only if getAsciiSignature returns NULL

        void Initialize(Stream pStream, string pFilePath);
               
        string GetFormatAbbreviation();
        bool IsFileLibrary(string pPath);
        bool HasMultipleFileExtensions();

        bool UsesLibraries();
        bool IsLibraryPresent();

        Dictionary<string, string> GetTagHash();

        void GetDatFileCrc32(string pPath, ref Dictionary<string, ByteArray> pLibHash,
            ref Crc32 pChecksum, bool pUseLibHash);

        // void GetDatFileCrc32(ref Crc32 pChecksum);



        //void getDatFileCrc32(string pPath, ref Dictionary<string, ByteArray> pLibHash, 
        //    ref Crc32 pChecksum, ref CryptoStream pMd5CryptoStream, ref CryptoStream pSha1CryptoStream, 
        //    bool pUseLibHash);
    }
}
