using System;
using System.ComponentModel;

namespace VGMToolbox.plugin
{
    public interface IVgmtBackgroundWorker
    {
        bool IsBusy { get; }
        event ProgressChangedEventHandler ProgressChanged;
        event RunWorkerCompletedEventHandler RunWorkerCompleted;

        void RunWorkerAsync(object argument);
        void CancelAsync();
    }
}
