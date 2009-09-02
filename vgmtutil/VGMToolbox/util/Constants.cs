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
        private string startingOffset;
        private bool treatSearchStringAsHex;
        private bool cutFile;
        private string searchStringOffset;
        private string cutSize;
        private string cutSizeOffsetSize;
        private bool isCutSizeAnOffset;
        private string outputFileExtension;
        private bool isLittleEndian;
        private bool useTerminatorForCutSize;
        private string terminatorString;
        private bool treatTerminatorStringAsHex;
        private bool includeTerminatorLength;
        private string extraCutSizeBytes;

        public bool DoSearchStringModulo
        {
            set;
            get;
        }

        public string SearchStringModuloDivisor
        {
            set;
            get;
        }

        public string SearchStringModuloResult
        {
            set;
            get;
        }

        public bool DoTerminatorModulo
        {
            set;
            get;
        }

        public string TerminatorStringModuloDivisor
        {
            set;
            get;
        }

        public string TerminatorStringModuloResult
        {
            set;
            get;
        }

        public string MinimumSize
        {
            set;
            get;
        }

        public string SearchString
        {
            get { return searchString; }
            set { searchString = value; }
        }

        /// <summary>
        /// Gets or sets offset to being searching at
        /// </summary>
        public string StartingOffset
        {
            set { startingOffset = value; }
            get { return startingOffset; }
        }        

        /// <summary>
        /// Gets or sets flag to indicate search string is a hex value.
        /// </summary>
        public bool TreatSearchStringAsHex
        {
            get { return treatSearchStringAsHex; }
            set { treatSearchStringAsHex = value; }
        }
        
        /// <summary>
        /// Gets or sets flag to cut the file when the offset is found.
        /// </summary>
        public bool CutFile
        {
            get { return cutFile; }
            set { cutFile = value; }
        }
        
        /// <summary>
        /// Gets or sets offset within destination file Search String would reside.
        /// </summary>
        public string SearchStringOffset
        {
            get { return searchStringOffset; }
            set { searchStringOffset = value; }
        }
        
        /// <summary>
        /// Gets or sets size to cut from file
        /// </summary>
        public string CutSize
        {
            get { return cutSize; }
            set { cutSize = value; }
        }
        
        /// <summary>
        /// Gets or sets size of offset holding cut size
        /// </summary>
        public string CutSizeOffsetSize
        {
            get { return cutSizeOffsetSize; }
            set { cutSizeOffsetSize = value; }
        }
        
        /// <summary>
        /// Gets or sets flag indicating that cut size is an offset.
        /// </summary>
        public bool IsCutSizeAnOffset
        {
            get { return isCutSizeAnOffset; }
            set { isCutSizeAnOffset = value; }
        }
        
        /// <summary>
        /// Gets or sets file extension to use for cut files.
        /// </summary>
        public string OutputFileExtension
        {
            get { return outputFileExtension; }
            set { outputFileExtension = value; }
        }
        
        /// <summary>
        /// Gets or sets flag indicating that offset based cut size is stored in Little Endian byte order.
        /// </summary>
        public bool IsLittleEndian
        {
            get { return isLittleEndian; }
            set { isLittleEndian = value; }
        }

        /// <summary>
        /// Gets or sets flag indicating that a terminator should be used to determine the cut size.
        /// </summary>
        public bool UseTerminatorForCutSize
        {
            get { return useTerminatorForCutSize; }
            set { useTerminatorForCutSize = value; }
        }
        
        /// <summary>
        /// Gets or sets terminator string to search for.
        /// </summary>
        public string TerminatorString
        {
            get { return terminatorString; }
            set { terminatorString = value; }
        }
        
        /// <summary>
        /// Gets or sets flag indicating that Terminator String is hex.
        /// </summary>
        public bool TreatTerminatorStringAsHex
        {
            get { return treatTerminatorStringAsHex; }
            set { treatTerminatorStringAsHex = value; }
        }
        
        /// <summary>
        /// Gets or sets flag indicating that the length of the terminator should be included in the cut size.
        /// </summary>
        public bool IncludeTerminatorLength
        {
            get { return includeTerminatorLength; }
            set { includeTerminatorLength = value; }
        }
        
        /// <summary>
        /// Gets or sets additional bytes to include in the cut size.
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
        /// <summary>
        /// File name to display in progress bar.
        /// </summary>
        private string fileName;

        /// <summary>
        /// Error message to display in output window.
        /// </summary>
        private string errorMessage;

        /// <summary>
        /// Generic message to display in output window.
        /// </summary>
        private string genericMessage;

        /// <summary>
        /// New tree node to add to a TreeView.
        /// </summary>
        private TreeNode newNode;

        /// <summary>
        /// Gets or sets fileName.
        /// </summary>
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }
        
        /// <summary>
        /// Gets or sets errorMessage.
        /// </summary>
        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; }
        }
        
        /// <summary>
        /// Gets or sets genericMessage.
        /// </summary>
        public string GenericMessage
        {
            get { return genericMessage; }
            set { genericMessage = value; }
        }
        
        /// <summary>
        /// Gets or sets newNode.
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
            fileName = String.Empty;
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
        /// <summary>
        /// Class name of the Form this node will bring to focus.
        /// </summary>
        private string formClass;

        /// <summary>
        /// Object type this node represents.  
        /// </summary>
        private string objectType;

        /// <summary>
        /// File path of the file this node represents.
        /// </summary>
        private string filePath;

        /// <summary>
        /// Gets or sets formClass
        /// </summary>
        public string FormClass
        {
            get { return formClass; }
            set { formClass = value; }
        }

        /// <summary>
        /// Gets or sets objectType
        /// </summary>
        public string ObjectType
        {
            get { return objectType; }
            set { objectType = value; }
        }

        /// <summary>
        /// Gets or sets filePath
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
        public const int FileReadChunkSize = 71680;
                
        /// <summary>
        /// Constant used to send an ignore the value message to the progress bar.
        /// </summary>
        public const int IgnoreProgress = -1;
        
        /// <summary>
        /// Constant used to send a generic message to the progress bar.
        /// </summary>
        public const int ProgressMessageOnly = -2;
        
        // empty constructor
        private Constants() { }
    }
}
