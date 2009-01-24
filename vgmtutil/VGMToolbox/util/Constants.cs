using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace VGMToolbox.util
{
    public class Constants
    {
        public const int FILE_READ_CHUNK_SIZE = 71680;
        
        
        // Progress Bar items
        public static readonly int IGNORE_PROGRESS = -1;
        public static readonly int PROGRESS_MSG_ONLY = -2;

        public struct ProgressStruct
        {
            public string filename;
            public string errorMessage;
            public string genericMessage;
            public TreeNode newNode;

            public void Clear()
            {
                filename = String.Empty;
                errorMessage = String.Empty;
                genericMessage = String.Empty;
                newNode = null;
            }
        }    

        // Node Tags
        public struct NodeTagStruct
        {
            public string formClass;
            public string objectType;
            public string filePath;
        } 

    }
}
