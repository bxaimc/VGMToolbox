using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace VGMToolbox.util
{
    public struct FindOffsetStruct
    {
        private string searchString;
        private bool treatSearchStringAsHex;
        private bool cutFile;
        private string searchStringOffset;
        private string cutSize;
        private string cutSizeOffsetSize;
        private bool isCutSizeAnOffset;
        private string outputFileExtension;
        private bool isLittleEndian;
        private bool useTerminatorForCutsize;
        private string terminatorString;
        private bool treatTerminatorStringAsHex;
        private bool includeTerminatorLength;
        private string extraCutSizeBytes;

        public string SearchString
        {
            get { return searchString; }
            set { searchString = value; }
        }
        public bool TreatSearchStringAsHex
        {
            get { return treatSearchStringAsHex; }
            set { treatSearchStringAsHex = value; }
        }
        public bool CutFile
        {
            get { return cutFile; }
            set { cutFile = value; }
        }
        public string SearchStringOffset
        {
            get { return searchStringOffset; }
            set { searchStringOffset = value; }
        }
        public string CutSize
        {
            get { return cutSize; }
            set { cutSize = value; }
        }
        public string CutSizeOffsetSize
        {
            get { return cutSizeOffsetSize; }
            set { cutSizeOffsetSize = value; }
        }
        public bool IsCutSizeAnOffset
        {
            get { return isCutSizeAnOffset; }
            set { isCutSizeAnOffset = value; }
        }
        public string OutputFileExtension
        {
            get { return outputFileExtension; }
            set { outputFileExtension = value; }
        }
        public bool IsLittleEndian
        {
            get { return isLittleEndian; }
            set { isLittleEndian = value; }
        }

        public bool UseTerminatorForCutsize
        {
            get { return useTerminatorForCutsize; }
            set { useTerminatorForCutsize = value; }
        }
        public string TerminatorString
        {
            get { return terminatorString; }
            set { terminatorString = value; }
        }
        public bool TreatTerminatorStringAsHex
        {
            get { return treatTerminatorStringAsHex; }
            set { treatTerminatorStringAsHex = value; }
        }
        public bool IncludeTerminatorLength
        {
            get { return includeTerminatorLength; }
            set { includeTerminatorLength = value; }
        }
        public string ExtraCutSizeBytes
        {
            get { return extraCutSizeBytes; }
            set { extraCutSizeBytes = value; }
        }
    }

    public struct ProgressStruct
    {
        private string filename;
        private string errorMessage;
        private string genericMessage;
        private TreeNode newNode;

        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }
        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; }
        }
        public string GenericMessage
        {
            get { return genericMessage; }
            set { genericMessage = value; }
        }
        public TreeNode NewNode
        {
            get { return newNode; }
            set { newNode = value; }
        }

        public void Clear()
        {
            filename = String.Empty;
            errorMessage = String.Empty;
            genericMessage = String.Empty;
            newNode = null;
        }
    }

    public struct NodeTagStruct
    {
        private string formClass;
        private string objectType;
        private string filePath;

        public string FormClass
        {
            get { return formClass; }
            set { formClass = value; }
        }

        public string ObjectType
        {
            get { return objectType; }
            set { objectType = value; }
        }

        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }
    } 

    public sealed class Constants
    {
        public const int FILE_READ_CHUNK_SIZE = 71680;
                
        // Progress Bar items
        public static readonly int IGNORE_PROGRESS = -1;
        public static readonly int PROGRESS_MSG_ONLY = -2;
        
        // empty constructor
        private Constants() { }
    }
}
