using System;
using System.Collections.Generic;
using System.Text;

namespace Imagein.Entity.Entities.Base
{
    public interface ICreatable
    {
        /// <summary>
        /// UTC
        /// </summary>
        DateTime CreatedOnUtc { get; set; }
    }
}
