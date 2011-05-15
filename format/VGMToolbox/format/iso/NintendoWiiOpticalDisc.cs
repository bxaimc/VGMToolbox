using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.util;

namespace VGMToolbox.format.iso
{
    public class NintendoWiiOpticalDisc
    {
        public static readonly byte[] STANDARD_IDENTIFIER = new byte[] { 0x5D, 0x1C, 0x9E, 0xA3 };
        public const uint IDENTIFIER_OFFSET = 0x18;
        public static string FORMAT_DESCRIPTION_STRING = "Wii Optical Disc";

        private static readonly string PROGRAMS_FOLDER =
            Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "external"));
        public static readonly string WII_EXTERNAL_FOLDER = Path.Combine(PROGRAMS_FOLDER, "wii");
        public static readonly string COMMON_KEY_PATH = Path.Combine(WII_EXTERNAL_FOLDER, "ckey.bin");
        public static readonly string KOREAN_KEY_PATH = Path.Combine(WII_EXTERNAL_FOLDER, "kkey.bin");

        public static byte[] GetKeyFromFile(string path)
        {
            byte[] keyArray = null;

            if (!File.Exists(path))
            {
                throw new FileNotFoundException(String.Format("Key not found: {0}.{1}", path, Environment.NewLine));
            }
            else
            {
                keyArray = new byte[0x10];
                
                using (FileStream fs = File.OpenRead(path))
                {
                    fs.Read(keyArray, 0, 0x10);
                }
            }

            return keyArray;
        }
    }

    public class NintendoWiiOpticalDiscVolume : IVolume
    {
        public struct PartitionEntry
        {
            public long PartitionOffset { set; get; }
            public byte[] PartitionType { set; get; }

            public byte[] TitleId { set; get; }
            public byte[] EncryptedTitleKey { set; get; }
            public byte[] DecryptedTitleKey { set; get; }
            public byte CommonKeyIndex { set; get; }

            public long RelativeDataOffset { set; get; }
            public long DataSize { set; get; }
        }

        public struct Partition
        {
            public uint PartitionCount { set; get; }
            public long PartitionTableOffset { set; get; }
            public PartitionEntry[] PartitionEntries { set; get; }
        }

        public string VolumeIdentifier { set; get; }
        public string FormatDescription { set; get; }

        public string WiiDiscId { set; get; }
        public string GameCode { set; get; }
        public string RegionCode { set; get; }
        public byte[] MakerCode { set; get; }

        public long VolumeBaseOffset { set; get; }
        public bool IsRawDump { set; get; }

        public byte[] CommonKey { set; get; }
        public byte[] KoreanCommonKey { set; get; }

        public Partition[] Partitions { set; get; }

        public ArrayList DirectoryStructureArray { set; get; }
        public IDirectoryStructure[] Directories
        {
            set { Directories = value; }
            get
            {
                DirectoryStructureArray.Sort();
                return (IDirectoryStructure[])DirectoryStructureArray.ToArray(typeof(NintendoGameCubeDirectoryStructure));
            }
        }

        public long RootDirectoryOffset { set; get; }
        public DateTime ImageCreationTime { set; get; }

        public long NameTableOffset { set; get; }

        public void Initialize(FileStream isoStream, long offset, bool isRawDump)
        {
            byte[] volumeIdentifierBytes;

            this.VolumeBaseOffset = offset;
            this.FormatDescription = NintendoWiiOpticalDisc.FORMAT_DESCRIPTION_STRING;

            this.WiiDiscId = ByteConversion.GetAsciiText(ParseFile.ParseSimpleOffset(isoStream, this.VolumeBaseOffset, 1));
            this.GameCode = ByteConversion.GetAsciiText(ParseFile.ParseSimpleOffset(isoStream, this.VolumeBaseOffset + 1, 2));
            this.RegionCode = ByteConversion.GetAsciiText(ParseFile.ParseSimpleOffset(isoStream, this.VolumeBaseOffset + 3, 1));
            this.MakerCode = ParseFile.ParseSimpleOffset(isoStream, this.VolumeBaseOffset + 4, 2);
            
            this.IsRawDump = isRawDump;
            this.DirectoryStructureArray = new ArrayList();

            // get identifier
            volumeIdentifierBytes = ParseFile.ParseSimpleOffset(isoStream, this.VolumeBaseOffset + 0x20, 64);
            volumeIdentifierBytes = FileUtil.ReplaceNullByteWithSpace(volumeIdentifierBytes);
            this.VolumeIdentifier = ByteConversion.GetEncodedText(volumeIdentifierBytes, ByteConversion.GetPredictedCodePageForTags(volumeIdentifierBytes)).Trim(); ;

            // initialize partition info
            this.InitializePartitions(isoStream);

            // add dummy time
            this.ImageCreationTime = new DateTime();

            // get offset of file table
            //this.RootDirectoryOffset = (long)ByteConversion.GetUInt32BigEndian(ParseFile.ParseSimpleOffset(isoStream, this.VolumeBaseOffset + 0x424, 4));

            //this.LoadDirectories(isoStream);
        }

        public void ExtractAll(FileStream isoStream, string destinationFolder, bool extractAsRaw)
        {
            foreach (NintendoGameCubeDirectoryStructure ds in this.DirectoryStructureArray)
            {
                ds.Extract(isoStream, destinationFolder, extractAsRaw);
            }
        }

        public void InitializePartitions(FileStream isoStream)
        {
            byte[] encryptedPartitionCluster;
            byte[] decryptedClusterDataSection;
            byte[] clusterIV;
            byte[] encryptedClusterDataSection;

            this.CommonKey = null;
            this.KoreanCommonKey = null;
            this.Partitions = new Partition[4];

            for (int i = 0; i < 4; i++)
            {
                this.Partitions[i] = new Partition();
                this.Partitions[i].PartitionCount = ByteConversion.GetUInt32BigEndian(ParseFile.ParseSimpleOffset(isoStream, this.VolumeBaseOffset + 0x40000 + (i * 8), 4));
                this.Partitions[i].PartitionEntries = new PartitionEntry[this.Partitions[i].PartitionCount];

                this.Partitions[i].PartitionTableOffset = ByteConversion.GetUInt32BigEndian(ParseFile.ParseSimpleOffset(isoStream, this.VolumeBaseOffset + 0x40004 + (i * 8), 4));
                this.Partitions[i].PartitionTableOffset <<= 2;

                if (this.Partitions[i].PartitionTableOffset > 0)
                {
                    // set absolute offset of partition
                    this.Partitions[i].PartitionTableOffset += this.VolumeBaseOffset;

                    for (int j = 0; j < this.Partitions[i].PartitionCount; j++)
                    {
                        this.Partitions[i].PartitionEntries[j] = new PartitionEntry();

                        // get offset to this partition
                        this.Partitions[i].PartitionEntries[j].PartitionOffset =
                            ByteConversion.GetUInt32BigEndian(ParseFile.ParseSimpleOffset(isoStream, this.Partitions[i].PartitionTableOffset + (j * 8), 4));
                        this.Partitions[i].PartitionEntries[j].PartitionOffset <<= 2;
                        this.Partitions[i].PartitionEntries[j].PartitionOffset += this.VolumeBaseOffset;

                        // get partition type
                        this.Partitions[i].PartitionEntries[j].PartitionType =
                            ParseFile.ParseSimpleOffset(isoStream, this.Partitions[i].PartitionTableOffset + 4 + (i * 8), 4);

                        // get relative offset partition's data section
                        this.Partitions[i].PartitionEntries[j].RelativeDataOffset = ByteConversion.GetUInt32BigEndian(ParseFile.ParseSimpleOffset(isoStream, this.Partitions[i].PartitionEntries[j].PartitionOffset + 0x02B8, 4));
                        this.Partitions[i].PartitionEntries[j].RelativeDataOffset <<= 2;
                        this.Partitions[i].PartitionEntries[j].RelativeDataOffset += this.VolumeBaseOffset;

                        // get the size of partition's data section
                        this.Partitions[i].PartitionEntries[j].DataSize = ByteConversion.GetUInt32BigEndian(ParseFile.ParseSimpleOffset(isoStream, this.Partitions[i].PartitionEntries[j].PartitionOffset + 0x02BC, 4));
                        this.Partitions[i].PartitionEntries[j].DataSize <<= 2;

                        //---------------------------
                        // parse this entry's ticket
                        //---------------------------
                        this.Partitions[i].PartitionEntries[j].TitleId = new byte[0x10];
                        Array.Copy(
                            ParseFile.ParseSimpleOffset(isoStream, this.Partitions[i].PartitionEntries[j].PartitionOffset + 0x01DC, 8),
                            0,
                            this.Partitions[i].PartitionEntries[j].TitleId,
                            0,
                            8);

                        this.Partitions[i].PartitionEntries[j].EncryptedTitleKey = ParseFile.ParseSimpleOffset(isoStream, this.Partitions[i].PartitionEntries[j].PartitionOffset + 0x01BF, 0x10);
                        this.Partitions[i].PartitionEntries[j].CommonKeyIndex = ParseFile.ParseSimpleOffset(isoStream, this.Partitions[i].PartitionEntries[j].PartitionOffset + 0x01F1, 1)[0];

                        //---------------------
                        // decrypt the TitleId
                        //---------------------
                        switch (this.Partitions[i].PartitionEntries[j].CommonKeyIndex)
                        {
                            case 0:
                                if (this.CommonKey == null)
                                {
                                    this.CommonKey = NintendoWiiOpticalDisc.GetKeyFromFile(NintendoWiiOpticalDisc.COMMON_KEY_PATH);
                                }

                                this.Partitions[i].PartitionEntries[j].DecryptedTitleKey =
                                    AESEngine.Decrypt(this.Partitions[i].PartitionEntries[j].EncryptedTitleKey,
                                        this.CommonKey, this.Partitions[i].PartitionEntries[j].TitleId,
                                        CipherMode.CBC, PaddingMode.Zeros);

                                break;
                            case 1:
                                if (this.KoreanCommonKey == null)
                                {
                                    this.KoreanCommonKey = NintendoWiiOpticalDisc.GetKeyFromFile(NintendoWiiOpticalDisc.KOREAN_KEY_PATH);
                                }

                                this.Partitions[i].PartitionEntries[j].DecryptedTitleKey =
                                    AESEngine.Decrypt(this.Partitions[i].PartitionEntries[j].EncryptedTitleKey,
                                        this.KoreanCommonKey, this.Partitions[i].PartitionEntries[j].TitleId,
                                        CipherMode.CBC, PaddingMode.Zeros);

                                break;
                        } // switch (this.Partitions[i].PartitionEntries[j].CommonKeyIndex)


                        string outFile = Path.Combine(Path.GetDirectoryName(isoStream.Name), String.Format("{0}-{1}.bin", i.ToString(), j.ToString()));                                                
                        long currentOffset = 0;

                        using (FileStream outStream = File.OpenWrite(outFile))
                        {

                            while (currentOffset < this.Partitions[i].PartitionEntries[j].DataSize)
                            {
                                encryptedPartitionCluster = ParseFile.ParseSimpleOffset(isoStream,
                                    this.Partitions[i].PartitionEntries[j].PartitionOffset + this.Partitions[i].PartitionEntries[j].RelativeDataOffset + currentOffset,
                                    0x8000);

                                clusterIV = ParseFile.ParseSimpleOffset(encryptedPartitionCluster, 0x03D0, 0x10);
                                encryptedClusterDataSection = ParseFile.ParseSimpleOffset(encryptedPartitionCluster, 0x400, 0x7C00);
                                decryptedClusterDataSection = AESEngine.Decrypt(encryptedClusterDataSection,
                                                this.Partitions[i].PartitionEntries[j].DecryptedTitleKey, clusterIV,
                                                CipherMode.CBC, PaddingMode.Zeros);

                                outStream.Write(decryptedClusterDataSection, 0, decryptedClusterDataSection.Length);
                                
                                currentOffset += 0x8000;
                            }
                        }

                    } // for (int j = 0; j < this.Partitions[i].PartitionCount; j++)
                } // if (this.Partitions[i].PartitionTableOffset > 0)
            }        
        }

        public void LoadDirectories(FileStream isoStream)
        {
            byte[] rootDirectoryBytes;
            NintendoGameCubeDirectoryRecord rootDirectoryRecord;
            NintendoGameCubeDirectoryStructure rootDirectory;

            // Get name table offset
            rootDirectoryBytes = ParseFile.ParseSimpleOffset(isoStream, this.RootDirectoryOffset, 0xC);
            rootDirectoryRecord = new NintendoGameCubeDirectoryRecord(rootDirectoryBytes);
            this.NameTableOffset = this.RootDirectoryOffset + ((long)rootDirectoryRecord.FileSize * 0xC);

            rootDirectory = new NintendoGameCubeDirectoryStructure(isoStream,
                isoStream.Name, rootDirectoryRecord, this.ImageCreationTime, this.VolumeBaseOffset,
                this.RootDirectoryOffset, this.RootDirectoryOffset, this.NameTableOffset, String.Empty, String.Empty);

            this.DirectoryStructureArray.Add(rootDirectory);
        }
    }

    public class AESEngine
    {
        // Decrypt a byte array into a byte array using a key and an IV 
        public static byte[] Decrypt(byte[] cipherData, byte[] Key, byte[] IV, CipherMode cipherMode, PaddingMode paddingMode)
        {
            byte[] decryptedData = null;
            
            // Create a MemoryStream that is going to accept the
            // decrypted bytes 
            using (MemoryStream ms = new MemoryStream())
            {

                // Create a symmetric algorithm. 
                // We are going to use Rijndael because it is strong and
                // available on all platforms. 
                // You can use other algorithms, to do so substitute the next
                // line with something like 
                //     TripleDES alg = TripleDES.Create(); 

                Rijndael alg = Rijndael.Create();

                // Now set the key and the IV. 
                // We need the IV (Initialization Vector) because the algorithm
                // is operating in its default 
                // mode called CBC (Cipher Block Chaining). The IV is XORed with
                // the first block (8 byte) 
                // of the data after it is decrypted, and then each decrypted
                // block is XORed with the previous 
                // cipher block. This is done to make encryption more secure. 
                // There is also a mode called ECB which does not need an IV,
                // but it is much less secure. 

                alg.KeySize = 128;
                alg.Mode = cipherMode;
                alg.Padding = paddingMode;
                alg.Key = Key;
                alg.IV = IV;

                // Create a CryptoStream through which we are going to be
                // pumping our data. 
                // CryptoStreamMode.Write means that we are going to be
                // writing data to the stream 
                // and the output will be written in the MemoryStream
                // we have provided. 

                CryptoStream cs = new CryptoStream(ms,
                    alg.CreateDecryptor(), CryptoStreamMode.Write);

                // Write the data and make it do the decryption 
                cs.Write(cipherData, 0, cipherData.Length);

                // Close the crypto stream (or do FlushFinalBlock). 
                // This will tell it that we have done our decryption
                // and there is no more data coming in, 
                // and it is now a good time to remove the padding
                // and finalize the decryption process. 

                cs.Close();

                // Now get the decrypted data from the MemoryStream. 
                // Some people make a mistake of using GetBuffer() here,
                // which is not the right way. 

                decryptedData = ms.ToArray();
            }

            return decryptedData;
        }

        // Decrypt a file into another file using a password 
        public static void Decrypt(string fileIn, string fileOut, string Password, CipherMode cipherMode, PaddingMode paddingMode)
        {

            // First we are going to open the file streams 

            FileStream fsIn = new FileStream(fileIn,
                        FileMode.Open, FileAccess.Read);
            FileStream fsOut = new FileStream(fileOut,
                        FileMode.OpenOrCreate, FileAccess.Write);

            // Then we are going to derive a Key and an IV from
            // the Password and create an algorithm 

            PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password,
                new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 
            0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});
            Rijndael alg = Rijndael.Create();

            alg.Mode = cipherMode;
            alg.Padding = paddingMode;
            alg.Key = pdb.GetBytes(32);
            alg.IV = pdb.GetBytes(16);

            // Now create a crypto stream through which we are going
            // to be pumping data. 
            // Our fileOut is going to be receiving the Decrypted bytes. 

            CryptoStream cs = new CryptoStream(fsOut, alg.CreateDecryptor(), CryptoStreamMode.Write);

            // Now will will initialize a buffer and will be 
            // processing the input file in chunks. 
            // This is done to avoid reading the whole file (which can be
            // huge) into memory. 

            int bufferLen = 4096;
            byte[] buffer = new byte[bufferLen];
            int bytesRead;

            do
            {
                // read a chunk of data from the input file 
                bytesRead = fsIn.Read(buffer, 0, bufferLen);

                // Decrypt it 
                cs.Write(buffer, 0, bytesRead);

            } while (bytesRead != 0);

            // close everything 
            cs.Close(); // this will also close the unrelying fsOut stream 
            fsIn.Close();
        }
    }
}
