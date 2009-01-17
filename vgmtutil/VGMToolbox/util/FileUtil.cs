using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VGMToolbox.util
{
    public class FileUtil
    {
        public static bool checkFilesExists(string pPath)
        {
            return File.Exists(pPath);
        }

        public static string replaceFileName(string pPath, string pFileName)
        {
            string ret = null;
            string oldFileName = pPath.Substring(0, pPath.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            ret = oldFileName +  pFileName;

            oldFileName = null;
            
            return ret;
        }

        public static string trimPath(string pFullFilePath)
        {
            return pFullFilePath.Substring(pFullFilePath.LastIndexOf(Path.DirectorySeparatorChar));
        }

        public static byte[] ReplaceNullByteWithSpace(byte[] pBytes)
        {
            for (int i = 0; i < pBytes.Length; i++)
            {
                if (pBytes[i] == 0x00)
                {
                    pBytes[i] = 0x20;
                }
            }

            return pBytes;
        }

        public static int GetFileCount(string[] pPaths)
        {
            int totalFileCount = 0;
            
            foreach (string path in pPaths)
            {
                if (File.Exists(path))
                {
                    totalFileCount++;
                }
                else if (Directory.Exists(path))
                {
                    totalFileCount += Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Length;
                }
            }

            return totalFileCount;
        }

        public static string CleanFileName(string pDirtyFileName)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                pDirtyFileName = pDirtyFileName.Replace(c, '_');
            }

            return pDirtyFileName;
        }
    }
}
