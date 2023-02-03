using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace CBSPay.Core.Interfaces
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> Table { get; }
        int Count(Expression<Func<TEntity, bool>> predicate);
        void Delete(object id, bool saveNow = true);
        void Delete(TEntity entity, bool saveNow = true);
        void DeleteRange(IEnumerable<TEntity> entities, bool save = true);
        IQueryable<TEntity> Fetch(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>,
            IOrderedQueryable<TEntity>> orderBy = null, int? page = default(int?), int? pageSize = default(int?));
        TEntity Find(Expression<Func<TEntity, bool>> predicate);
        TEntity Find(object id);
        TEntity Find(params object[] keyValues);
        void Insert(TEntity entity, bool saveNow = true);
        void InsertRange(IEnumerable<TEntity> entities, bool saveNow = true);
        IQueryable<TEntity> Queryable();
        int SaveChanges();
        void Update(TEntity entity, bool saveNow = true);
        void UpdateRange(IEnumerable<TEntity> entities, bool save = true);
        void SaveBundle(IEnumerable<TEntity> collection);
        bool SaveBundle(ICollection<TEntity> collection);
    }
}
