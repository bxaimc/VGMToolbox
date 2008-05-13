using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGMToolbox.util.ObjectPooling
{
    public sealed class ObjectPooler
    {
        static readonly PoolByteArrays instance = new PoolByteArrays(5, 104857600);        

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static ObjectPooler() {}

        ObjectPooler() { }

        public static PoolByteArrays Instance
        {
            get
            {
                return instance;
            }
        }
    }
}
