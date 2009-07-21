using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace VGMToolbox.util
{
    /// <summary>
    /// Struct containing criteria used to find offsets.
    /// </summary>
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

        /// <summary>
        /// String to search for.
        /// </summary>
        public string SearchString
        {
            get { return searchString; }
            set { searchString = value; }
        }
        /// <summary>
        /// Flag to indicate search string is a hex value.
        /// </summary>
        public bool TreatSearchStringAsHex
        {
            get { return treatSearchStringAsHex; }
            set { treatSearchStringAsHex = value; }
        }
        /// <summary>
        /// Flag to cut the file when the offset is found.
        /// </summary>
        public bool CutFile
        {
            get { return cutFile; }
            set { cutFile = value; }
        }
        /// <summary>
        /// Offset within destination file Search String would reside.
        /// </summary>
        public string SearchStringOffset
        {
            get { return searchStringOffset; }
            set { searchStringOffset = value; }
        }
        /// <summary>
        /// Size to cut from file
        /// </summary>
        public string CutSize
        {
            get { return cutSize; }
            set { cutSize = value; }
        }
        /// <summary>
        /// Size of offset holding cut size
        /// </summary>
        public string CutSizeOffsetSize
        {
            get { return cutSizeOffsetSize; }
            set { cutSizeOffsetSize = value; }
        }
        /// <summary>
        /// Flag indicating that cut size is an offset.
        /// </summary>
        public bool IsCutSizeAnOffset
        {
            get { return isCutSizeAnOffset; }
            set { isCutSizeAnOffset = value; }
        }
        /// <summary>
        /// File extension to use for cut files.
        /// </summary>
        public string OutputFileExtension
        {
            get { return outputFileExtension; }
            set { outputFileExtension = value; }
        }
        /// <summary>
        /// Flag indicating that offset based cut size is stored in Little Endian byte order.
        /// </summary>
        public bool IsLittleEndian
        {
            get { return isLittleEndian; }
            set { isLittleEndian = value; }
        }

        /// <summary>
        /// Flag indicating that a terminator should be used to determine the cut size.
        /// </summary>
        public bool UseTerminatorForCutsize
        {
            get { return useTerminatorForCutsize; }
            set { useTerminatorForCutsize = value; }
        }
        /// <summary>
        /// Terminator string to search for.
        /// </summary>
        public string TerminatorString
        {
            get { return terminatorString; }
            set { terminatorString = value; }
        }
        /// <summary>
        /// Flag indicating that Terminator String is hex.
        /// </summary>
        public bool TreatTerminatorStringAsHex
        {
            get { return treatTerminatorStringAsHex; }
            set { treatTerminatorStringAsHex = value; }
        }
        /// <summary>
        /// Flag indicating that the length of the terminator should be included in the cut size.
        /// </summary>
        public bool IncludeTerminatorLength
        {
            get { return includeTerminatorLength; }
            set { includeTerminatorLength = value; }
        }
        /// <summary>
        /// Additional bytes to include in the cut size.
        /// </summary>
        public string ExtraCutSizeBytes
        {
            get { return extraCutSizeBytes; }
            set { extraCutSizeBytes = value; }
        }
    }

    /// <summary>
    /// Struct used to send messages conveying progress.
    /// </summary>
    public struct ProgressStruct
    {
        private string filename;
        private string errorMessage;
        private string genericMessage;
        private TreeNode newNode;

        /// <summary>
        /// File name to display in progress bar.
        /// </summary>
        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }
        /// <summary>
        /// Error message to display in output window.
        /// </summary>
        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; }
        }
        /// <summary>
        /// Generic message to display in output window.
        /// </summary>
        public string GenericMessage
        {
            get { return genericMessage; }
            set { genericMessage = value; }
        }
        /// <summary>
        /// New tree node to add to a TreeView.
        /// </summary>
        public TreeNode NewNode
        {
            get { return newNode; }
            set { newNode = value; }
        }

        /// <summary>
        /// Reset this node's values
        /// </summary>
        public void Clear()
        {
            filename = String.Empty;
            errorMessage = String.Empty;
            genericMessage = String.Empty;
            newNode = null;
        }
    }

    /// <summary>
    /// Struct used to allow TreeView to select a specific form and modify the originating node upon completion of a task.
    /// </summary>
    public struct NodeTagStruct
    {
        private string formClass;
        private string objectType;
        private string filePath;

        /// <summary>
        /// Class name of the Form this node will bring to focus.
        /// </summary>
        public string FormClass
        {
            get { return formClass; }
            set { formClass = value; }
        }
        /// <summary>
        /// Object type this node represents.  
        /// </summary>
        public string ObjectType
        {
            get { return objectType; }
            set { objectType = value; }
        }
        /// <summary>
        /// File path of the file this node represents.
        /// </summary>
        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }
    } 

    /// <summary>
    /// Class containing universal constants.
    /// </summary>
    public sealed class Constants
    {
        /// <summary>
        /// Chunk size to use when reading from files.  Used to grab maximum buffer 
        /// size without using the large object heap which has poor collection.
        /// </summary>
        public const int FILE_READ_CHUNK_SIZE = 71680;
                
        /// <summary>
        /// Constant used to send an ignore the value message to the progress bar.
        /// </summary>
        public const int IGNORE_PROGRESS = -1;
        /// <summary>
        /// Constant used to send a generic message to the progress bar.
        /// </summary>
        public const int PROGRESS_MSG_ONLY = -2;
        
        // empty constructor
        private Constants() { }
    }
}
