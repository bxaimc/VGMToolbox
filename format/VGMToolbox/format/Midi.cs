using System;
using System.Collections.Generic;
using System.Text;

namespace VGMToolbox.format
{
    class Midi  
    {
        private static readonly byte[] ASCII_SIGNATURE = new byte[] { 0x4D, 0x54, 0x68, 0x64 }; // MThd
        private const string FORMAT_ABBREVIATION = "MIDI";

        private string filePath;
        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }

        Dictionary<string, string> tagHash = new Dictionary<string, string>();

        #region METHODS

        public void Initialize(Stream pStream, string pFilePath)
        {
            this.filePath = pFilePath;
        }

        public byte[] GetAsciiSignature()
        {
            return ASCII_SIGNATURE;
        }

        public string GetFileExtensions()
        {
            return null;
        }

        public string GetFormatAbbreviation()
        {
            return FORMAT_ABBREVIATION;
        }

        public bool IsFileLibrary() { return false; }

        public bool HasMultipleFileExtensions()
        {
            return false;
        }

        public Dictionary<string, string> GetTagHash()
        {
            return this.tagHash;
        }

        public bool UsesLibraries() { return false; }
        public bool IsLibraryPresent() { return true; }

        #endregion
    }
}
