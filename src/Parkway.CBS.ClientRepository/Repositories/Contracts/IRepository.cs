using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace Parkway.CBS.ClientRepository.Repositories.Contracts
{
    public interface IRepository<TEntity> where TEntity : class
    {
        TEntity Get(int id);

        IEnumerable<TEntity> GetAll();

        /// <summary>
        /// Get count
        /// </summary>
        /// <returns>Int64</returns>
        Int64 Count();

        /// <summary>
        /// Get long count
        /// </summary>
        /// <returns>Int64</returns>
        Int64 Count(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Get int count
        /// </summary>
        /// <returns>int</returns>
        int IntCount(Expression<Func<TEntity, bool>> predicate);


        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);

        // This method was not in the videos, but I thought it would be useful to add.
        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate);

        void Add(TEntity entity);

        void AddRange(IEnumerable<TEntity> entities);

        void Remove(TEntity entity);

        void RemoveRange(IEnumerable<TEntity> entities);


        bool SaveBundle(List<DataTable> listOfDataTables, string tableName);
    }
}
