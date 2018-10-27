using NUlid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Imagein.Entity.Entities.Base
{
    public class BaseEntity
    {
        public BaseEntity()
        {
            Id = Ulid.NewUlid().ToString();
        }

        public string Id { get; set; }

        public Ulid IdAsUlid => new Ulid(Id);
    }
}
