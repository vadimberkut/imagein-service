using System;
using System.Collections.Generic;
using System.Text;

namespace Imagein.Entity.Entities.Base
{
    public interface IUpdatable
    {
        /// <summary>
        /// UTC
        /// </summary>
        DateTime UpdatedOnUtc { get; set; }
    }
}
