using Imagein.Data.DbContexts;
using Imagein.Data.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imagein.Data.Repositories.Base
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        private ApplicationDbContext dbContext;
        protected DbSet<T> dbSet;

        public RepositoryBase(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
            this.dbSet = this.dbContext.Set<T>();
        }


        #region Interface implementation

        public virtual IEnumerable<T> GetAll()
        {
            return dbSet.AsNoTracking().AsEnumerable().ToList();
        }

        public virtual IEnumerable<T> GetMany(Func<T, bool> predicate)
        {
            return dbSet.AsNoTracking().Where(predicate).ToList();
        }


        public virtual T Get(Func<T, bool> predicate)
        {
            try
            {
                return dbSet.AsNoTracking().First(predicate);
            }
            catch(InvalidCastException ex)
            {
                // No items in collection
                // TODO - use own exception system
                throw new Exception("", ex);
            }
        }

        public virtual T GetById(string id)
        {
            var existing = dbSet.Find(id);

            if(existing == null)
            {
                // No items in collection
                // TODO - use own exception system
                throw new Exception("");
            }

            return existing;
        }

        public virtual void Add(T entity)
        {
            dbSet.Add(entity);
            // DbContext.Entry<T>(entity).State = EntityState.Added;
        }

        public virtual void AddAll(IEnumerable<T> entities)
        {
            dbSet.AddRange(entities);
        }

        public virtual void Update(T entity)
        {
            dbSet.Update(entity);
        }

        public virtual void UpdateAll(IEnumerable<T> entities)
        {
            dbSet.UpdateRange(entities);
        }

        public virtual void Delete(T entity)
        {
            dbSet.Remove(entity);
        }

        public virtual void Delete(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
        }

        public virtual void Delete(Func<T, bool> predicate)
        {
            var entities = dbSet.AsNoTracking().Where(predicate).ToList();
            dbSet.RemoveRange(entities);
        }

        public virtual bool Exists(Func<T, bool> predicate)
        {
            return this.dbSet.AsNoTracking().Any(predicate);
        }

        #endregion
    }
}
