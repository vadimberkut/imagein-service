using NUlid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Imagein.Entity.Entities.Base
{
    public interface IBaseEntity
    {
        string Id { get; set; }
        Ulid IdAsUlid { get; }
    }
}
