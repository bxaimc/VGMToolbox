using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VGMToolbox.format
{
    public class _2sf : Xsf
    {
        public static int SPEC_REVISION_1 = 1;
        public static int SPEC_REVISION_2 = 2;
        
        int specRevisionNumber;

        public override void Initialize(Stream pStream, string pFilePath)
        {
            base.Initialize(pStream, pFilePath);

            if (this.reservedSectionLength > 0)
            {
                specRevisionNumber = SPEC_REVISION_1;                    
            }
            else
            {
                specRevisionNumber = SPEC_REVISION_2;
            }
        }
    }
}
