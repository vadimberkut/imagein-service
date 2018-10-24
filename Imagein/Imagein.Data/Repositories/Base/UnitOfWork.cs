using Imagein.Data.DbContexts;
using Imagein.Data.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imagein.Data.Repositories.Base
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext dbContext;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void Commit()
        {
            dbContext.SaveChanges();
        }

        public void Reject()
        {
            foreach (var entry in dbContext.ChangeTracker.Entries().Where(x => x.State != EntityState.Unchanged))
            {
                switch(entry.State)
                {
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Modified:
                    case EntityState.Detached:
                        entry.Reload();
                        break;
                }
            }
        }
    }
}
