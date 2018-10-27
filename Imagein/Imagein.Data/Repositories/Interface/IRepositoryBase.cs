using Imagein.Entity.Entities.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Imagein.Data.Repositories.Interface
{
    public interface IRepositoryBase<T> where T : class, IBaseEntity
    {
        /// <summary>
        /// Get all entities of type T
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> GetAll();

        /// <summary>
        /// Get entities using predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEnumerable<T> GetMany(Func<T, bool> predicate);


        /// <summary>
        /// Get entity by predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        T Get(Func<T, bool> predicate);

        /// <summary>
        /// Get Entity by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetById(string id);


        /// <summary>
        /// Marks entity as new
        /// </summary>
        /// <param name="entity"></param>
        void Add(T entity);

        /// <summary>
        /// Marks all entities as new
        /// </summary>
        /// <param name="entities"></param>
        void AddAll(IEnumerable<T> entities);


        /// <summary>
        /// Marks entity as modified
        /// </summary>
        /// <param name="entity"></param>
        void Update(T entity);

        /// <summary>
        /// Marks all entities as modified
        /// </summary>
        /// <param name="entities"></param>
        void UpdateAll(IEnumerable<T> entities);


        /// <summary>
        /// Marks entity to be removed
        /// </summary>
        /// <param name="entity"></param>
        void Delete(T entity);

        /// <summary>
        /// Marks entities to be removed
        /// </summary>
        /// <param name="entity"></param>
        void Delete(IEnumerable<T> entities);

        /// <summary>
        /// Marks entity to be removed accordingly to predicate
        /// </summary>
        /// <param name="predicate"></param>
        void Delete(Func<T, bool> predicate);


        /// <summary>
        /// Checks entity exists based on predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool Exists(Func<T, bool> predicate);
    }
}
