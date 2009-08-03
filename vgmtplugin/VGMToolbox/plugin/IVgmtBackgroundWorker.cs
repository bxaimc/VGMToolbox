using System;
using System.ComponentModel;

namespace VGMToolbox.plugin
{    
    /// <summary>
    /// Interface defining a generic background worker for use with VGMT.
    /// </summary>
    public interface IVgmtBackgroundWorker
    {
        event ProgressChangedEventHandler ProgressChanged;

        event RunWorkerCompletedEventHandler RunWorkerCompleted;
        
        /// <summary>
        /// Gets a value indicating whether or not worker is busy.
        /// </summary>
        bool IsBusy { get; }
        
        void RunWorkerAsync(object argument);
        
        void CancelAsync();
    }
}
