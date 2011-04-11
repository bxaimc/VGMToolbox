using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

using VGMToolbox.format.iso;
using VGMToolbox.plugin;
using VGMToolbox.util;

namespace VGMToolbox.tools.extract
{
    class IsoExtractorWorker : AVgmtDragAndDropWorker, IVgmtBackgroundWorker
    {
        public struct IsoExtractorStruct : IVgmtWorkerStruct
        {
            public string[] SourcePaths { set; get; }
        }
        
        public IsoExtractorWorker() :
            base() { }

        protected override void DoTaskForFile(string pPath, IVgmtWorkerStruct pTaskStruct, DoWorkEventArgs e)
        {
            IsoExtractorStruct taskStruct = (IsoExtractorStruct)pTaskStruct;

            Iso9660Volume volume;
            long volumeOffset;
            string outputPath;

            using (FileStream fs = File.OpenRead(pPath))
            {
                volumeOffset = ParseFile.GetNextOffset(fs, 0, Iso9660.VOLUME_DESCRIPTOR_IDENTIFIER);

                while (volumeOffset > -1)
                {
                    volume = new Iso9660Volume();
                    volume.Initialize(fs, volumeOffset);

                    outputPath = Path.Combine(Path.Combine(Path.GetDirectoryName(pPath),
                        String.Format("VOLUME_{0}", volume.VolumeSequenceNumber.ToString("X4"))), volume.VolumeIdentifier);
                    
                    volume.ExtractAll(fs, outputPath);

                    volumeOffset = ParseFile.GetNextOffset(fs, volume.VolumeBaseOffset + (volume.VolumeSpaceSize *  volume.LogicalBlockSize), Iso9660.VOLUME_DESCRIPTOR_IDENTIFIER);
                }                                
            }
        }    
    }
}
