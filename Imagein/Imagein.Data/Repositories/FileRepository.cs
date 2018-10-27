using Imagein.Data.DbContexts;
using Imagein.Data.Repositories.Base;
using Imagein.Data.Repositories.Interface;
using Imagein.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Imagein.Data.Repositories
{
    public class FileRepository : RepositoryBase<FileEntity>, IFileRepository
    {

        public FileRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }

        #region Overrides

        #endregion

    }
}
