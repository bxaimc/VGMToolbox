using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace VGMToolbox.auditing
{
    class AuditingUtil
    {
        private const string ROM_SPACER = "  ";         // Simple spacer for formatting output lists

        public static readonly int IGNORE_PROGRESS = -1;
     
        private Hashtable haveList = new Hashtable();   // Stores list of present files
        private Hashtable missList = new Hashtable();   // Stores list of missing files
        private Datafile datafile;                      // Datafile used for building lists
        private ArrayList checksumCache = new ArrayList();
        private Hashtable checksumHash = new Hashtable();

        public Hashtable ChecksumHash { get { return checksumHash; } }

        #region CONSTRUCTORS
        
        /// <summary>
        /// Simple constructor taking input pDatafile
        /// </summary>
        /// <param name="pDatafile">Datafile to use for building lists.</param>
        public AuditingUtil(Datafile pDatafile)
        {
            datafile = pDatafile;
            checksumHash = this.BuildChecksumHash();
            // this.addCachedChecksumsToHash();
        }
        
        # endregion

        #region STRUCTS

        /// <summary>
        /// Simple struct used in rebuilding.
        /// </summary>
        public struct ChecksumStruct
        {
            public string game;
            public string rom;
            public bool isFilePresent;
        }

        public struct ProgressStruct
        {
            public string filename;
            public string errorMessage;
        }
         
        #endregion

        #region DATFILE TEXT OUTPUT FUNCTIONS

        /// <summary>
        /// Function to build a text list of the files in the datafile.
        /// </summary>
        /// <returns>String list of all games and roms in the datafile.</returns>
        public string BuildDatafileList()
        {
            string ret = "";

            foreach (game g in datafile.game)
            {
                ret += buildGameList(g);
            }

            return ret;
        }

        private string BuildDatafileList(Datafile pDatafile)
        {
            string ret = String.Empty;

            foreach (game g in pDatafile.game)
            {
                ret += buildGameList(g);
            }

            return ret;
        }

        /// <summary>
        /// Function to build a text list of the files in the incoming game.
        /// </summary>
        /// <param name="pGame">Game to parse and build text file of.</param>
        /// <returns>String list of the game name and all roms in the game.</returns>
        private string buildGameList(game pGame)
        {
            string ret = System.Environment.NewLine + pGame.name + System.Environment.NewLine;

            foreach (rom r in pGame.rom)
            {
                ret += buildRomOutput(r);
            }

            return ret;
        }

        /// <summary>
        /// Function to build a text string of the incoming rom.
        /// </summary>
        /// <param name="pRom">Rom to create string of.</param>
        /// <returns>String of the incoming rom.</returns>
        private string buildRomOutput(rom pRom)
        {
            return ROM_SPACER + pRom.name + System.Environment.NewLine;
        }

        # endregion

        #region HAVE/MISS LIST METHODS

        /// <summary>
        /// Function to parse the datafile object and create a hashtable using the
        /// checksums as a key and the game/rom paths as the item.
        /// </summary>
        /// <returns>Hashtable with checksum keys for each game/rom pair.</returns>
        public Hashtable BuildChecksumHash()
        {
            Hashtable checksumHash = new Hashtable();
            ChecksumStruct checksumItem;            
            ArrayList list;
            string checksumKey = String.Empty;

            foreach (game set in datafile.game)
            {
                if (set.rom != null && set.rom.Length > 0)
                {
                    foreach (rom file in set.rom)
                    {
                        // @TODO Add MD5/SHA1 to make checksum hash correct String(CRC32 + MD5 + SHA1)
                        checksumKey = file.crc;

                        if (!checksumHash.ContainsKey(checksumKey))
                        {
                            list = new ArrayList();
                            checksumItem = new ChecksumStruct();
                            checksumItem.game = set.name;
                            checksumItem.rom = file.name;
                            checksumItem.isFilePresent = false;
                            list.Add(checksumItem);
                        }
                        else
                        {
                            list = (ArrayList)checksumHash[checksumKey];
                            checksumItem = new ChecksumStruct();
                            checksumItem.game = set.name;
                            checksumItem.rom = file.name;
                            checksumItem.isFilePresent = false;
                            list.Add(checksumItem);
                            checksumHash.Remove(checksumKey);
                        }
                        
                        checksumHash.Add(checksumKey, list);
                    }
                }
            }
            return checksumHash;
        }
        
        /// <summary>
        /// Function to write out the have and miss lists to a file.
        /// </summary>
        /// <param name="pPath">Directory to write the Have and Miss lists to.</param>
        public void WriteHaveMissLists(string pPath)
        {
            StreamWriter sw;
            
            string havePath = pPath + "_HAVE.TXT";
            string missPath = pPath + "_MISS.TXT";
            string fixDatPath = pPath + "Fix_Datafile.xml";

            string haveListText = String.Empty;
            string missListText = String.Empty;

            Datafile haveList = new Datafile();
            Datafile missList = new Datafile();

            game newHaveGame = new game();
            game newMissGame = new game();

            ArrayList haveGameList = new ArrayList();
            ArrayList missGameList = new ArrayList();
            
            foreach (game g in datafile.game)
            {                
                ArrayList haveRomList = new ArrayList();
                ArrayList missRomList = new ArrayList();
                
                foreach (rom r in g.rom)
                {
                    if (checksumCache.Contains(r.crc))
                    {
                        haveRomList.Add(r);
                    }
                    else 
                    {
                        missRomList.Add(r);
                    }
                }

                if (haveRomList.Count > 0)
                {
                    newHaveGame = g.DeepCopy();
                    newHaveGame.rom = (rom[]) haveRomList.ToArray(typeof(rom));
                    haveGameList.Add(newHaveGame);
                }

                if (missRomList.Count > 0)
                {
                    newMissGame = g.DeepCopy();
                    newMissGame.rom = (rom[])missRomList.ToArray(typeof(rom));
                    missGameList.Add(newMissGame);
                }
            }

            haveList.game = (game[])haveGameList.ToArray(typeof(game));
            missList.game = (game[])missGameList.ToArray(typeof(game));

            haveListText = BuildDatafileList(haveList);
            missListText = BuildDatafileList(missList);

            haveListText = String.Format("You have {0} of {1} {2} sets,", haveGameList.Count,
                datafile.game.Length, datafile.header.description) + Environment.NewLine +
                Environment.NewLine + haveListText;
            missListText = String.Format("You are missing {0} of {1} {2} sets,", missGameList.Count,
                datafile.game.Length, datafile.header.description) + Environment.NewLine +
                Environment.NewLine + missListText;
            
            if (File.Exists(havePath))
            {
                File.Delete(havePath);
            }

            sw = File.CreateText(havePath);
            sw.Write(haveListText);
            sw.Close();

            if (File.Exists(missPath))
            {
                File.Delete(missPath);
            }

            sw = File.CreateText(missPath);
            sw.Write(missListText);

            sw.Close();
            sw.Dispose();

            // Fix Datafile
            if (File.Exists(fixDatPath))
            {
                File.Delete(fixDatPath);
            }

            if (missList.game.Length > 0)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Datafile));
                TextWriter textWriter = new StreamWriter(fixDatPath);
                serializer.Serialize(textWriter, missList);
                textWriter.Close();
                textWriter.Dispose();
            }
        }

        # endregion

        # region CACHE METHODS

        public void AddChecksumToCache(string pChecksum)
        {
            if (pChecksum != String.Empty && !checksumCache.Contains(pChecksum))
            {
                checksumCache.Add(pChecksum);
            }        
        }

        public void ReadChecksumHashFromFile(FileStream pFileStream)
        {
            if (pFileStream.Length > 0)
            {
                pFileStream.Seek(0, SeekOrigin.Begin);
                using (InflaterInputStream inflaterInputStream = new InflaterInputStream(pFileStream, new Inflater()))
                {
                    BinaryFormatter b = new BinaryFormatter();
                    checksumCache = new ArrayList((string[])b.Deserialize(inflaterInputStream));
                }
            }
        }

        public void WriteChecksumHashToFile(FileStream pFileStream)
        {
            pFileStream.Seek(0, SeekOrigin.Begin);
            using (DeflaterOutputStream deflaterOutputStream = new DeflaterOutputStream(pFileStream, new Deflater(Deflater.BEST_COMPRESSION)))
            {
                BinaryFormatter b = new BinaryFormatter();
                b.Serialize(deflaterOutputStream, (string[])checksumCache.ToArray(typeof(string)));
            }
        }
        
        # endregion

        #region STATIC METHODS

        public static string ByteArrayToString(byte[] pBytes)
        {
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < pBytes.Length; i++)
            {
                sBuilder.Append(pBytes[i].ToString("X2"));
            }

            return sBuilder.ToString();
        }

        public static string GetMd5OfFullFile(FileStream pFileStream)
        {
            MD5CryptoServiceProvider md5Hash = new MD5CryptoServiceProvider();

            pFileStream.Seek(0, SeekOrigin.Begin);
            md5Hash.ComputeHash(pFileStream);
            return ByteArrayToString(md5Hash.Hash);
        }

        # endregion
    }
}
