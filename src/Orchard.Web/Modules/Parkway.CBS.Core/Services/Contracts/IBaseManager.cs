using Orchard.Users.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IBaseManager<M>
    {

        /// <summary>
        /// Start UOW transaction
        /// </summary>
        void StartUOW();   

        /// <summary>
        /// End UOW transaction
        /// </summary>
        void EndUOW();       

        /// <summary>
        /// Roll back all transactions
        /// </summary>
        void RollBackAllTransactions();

        /// <summary>
        /// Get user by TaxEntityId
        /// </summary>
        /// <param name="id"></param>
        /// <returns>UserPartRecord</returns>
        UserPartRecord User(int id);

        /// <summary>
        /// Get user by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns>UserPartRecord</returns>
        UserPartRecord User(string email);


        /// <summary>
        /// Evict object from cache
        /// </summary>
        /// <param name="entity"></param>
        void Evict(M entity);

        /// <summary>
        /// Clear session
        /// </summary>
        void ClearSession();

        /// <summary>
        /// Get model with the given id
        /// </summary>
        /// <param name="id">TaxEntityId of the record</param>
        /// <returns><paramref name="M"/>Model</returns>
        M Get(int id);

        /// <summary>
        /// Get model
        /// </summary>
        /// <param name="lambda"></param>
        /// <returns><typeparamref name="M"/>Model</returns>
        M Get(Expression<Func<M, bool>> lambda);

        int Count(Expression<Func<M, bool>> lambda);

        /// <summary>
        /// Updaate the model
        /// </summary>
        /// <param name="model"></param>
        /// <returns>bool</returns>
        bool Update(M model);

        /// <summary>
        /// Get model based where column = variable
        /// <para>Must always resolve to a string value. That is it can only query columns of string data types 
        /// e.g Code column stores variaables as string, User_Id doesn't.</para>
        /// </summary>
        /// <param name="column">Name of the column the select operation is done on</param>
        /// <param name="variable">Comparison value.</param>
        /// <returns><typeparamref name="M"/>Model</returns>
        M Get(string column, string variable);

        /// <summary>
        /// Save the model object
        /// </summary>
        /// <param name="model"></param>
        /// <returns>bool</returns>
        bool Save(M model);

        /// <summary>
        /// Save a large bunch
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="tableName"></param>
        bool SaveBundle(DataTable dataTable, string tableName);

        /// <summary>
        /// Save a list. This is transactionless and relies on the main app blanket transaction
        /// </summary>
        /// <param name="collection"></param>
        /// <returns>bool</returns>
        bool SaveBundleUnCommit(ICollection<M> collection);


        /// <summary>
        /// Save a bunch of Models of the same type
        /// <para>This method doesn't commit immediately. It returns the index of the item that failed to insert</para>
        /// </summary>
        /// <param name="collection"></param>
        /// <returns>int</returns>
        int SaveBundleUnCommitStatelessWithErrors(ICollection<M> collection);


        /// Save a bunch of Models of the same type
        /// </summary>
        /// <param name="collection"></param>
        /// <returns>bool</returns>
        bool SaveBundleUnCommitStateless(ICollection<M> collection);


        /// <summary>
        /// Save a bunch of Models of the same type
        /// </summary>
        /// <param name="collection"></param>
        /// <returns>bool</returns>
        bool SaveBundle(ICollection<M> collection);// where M : CBSModel;


        /// <summary>
        /// Get collection of M
        /// </summary>
        /// <returns>IEnumerable{M}</returns>
        IEnumerable<M> GetCollection();

        /// <summary>
        /// Get a collection of model in ascending order
        /// </summary>
        /// <param name="lambda">lambda expression</param>
        /// <param name="order">Ordering</param>
        /// <returns>IEnumerable{M}</returns>
        IEnumerable<M> GetCollection(Expression<Func<M, bool>> lambda, string order);

        /// <summary>
        /// Get a collection of model
        /// </summary>
        /// <param name="lambda">lambda expression</param>
        /// <returns>IEnumerable{M}</returns>  
        IEnumerable<M> GetCollection(Expression<Func<M, bool>> lambda);


        IEnumerable<M> GetCollection(Expression<Func<M, bool>> lambda, int take, int skip = 0);
    }
}