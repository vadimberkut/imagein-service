﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Imagein.Data.Repositories.Interface
{
    public interface IUnitOfWork
    {
        IFileRepository FileRepository { get; }


        void Commit();
        void Reject();
    }
}
