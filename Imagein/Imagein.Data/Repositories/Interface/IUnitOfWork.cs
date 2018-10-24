using System;
using System.Collections.Generic;
using System.Text;

namespace Imagein.Data.Repositories.Interface
{
    public interface IUnitOfWork
    {
        void Commit();
        void Reject();
    }
}
