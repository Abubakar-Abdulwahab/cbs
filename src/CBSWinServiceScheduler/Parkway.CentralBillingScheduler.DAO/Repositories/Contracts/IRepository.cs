
using Parkway.CentralBillingScheduler.DAO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using System.Data;

namespace Parkway.CentralBillingScheduler.DAO.Repositories.Contracts
{
    public interface IRepository<M> where M : CBSBaseModel
    {
        /// <summary>
        /// Get T by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        M Get(int id);


        /// <summary>
        /// Get collection
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>IList{M}</returns>
        IList<M> Get(Expression<Func<M, bool>> predicate);


        /// <summary>
        /// Add T
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>T</returns>
        bool Add(M entity);

        bool Add(IEnumerable<M> items);

        bool Update(M entity);

        bool Delete(M entity);

        bool Delete(IEnumerable<M> entities);

        /// <summary>
        /// Save bundle
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="tableName"></param>
        /// <returns>bool</returns>
        bool SaveBundle(DataTable dataTable, string tableName);
    }
}
