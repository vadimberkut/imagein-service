using System;
using System.Collections.Generic;
using System.Text;

namespace Imagein.Entity.Enums
{
    public enum FileStoreType
    {
        Unknown = 0,

        /// <summary>
        /// File is stored manually by us
        /// </summary>
        Local = 1,

        /// <summary>
        /// File is stored somewhere else. E.g. in cloud
        /// </summary>
        Remote = 2,
    }
}
