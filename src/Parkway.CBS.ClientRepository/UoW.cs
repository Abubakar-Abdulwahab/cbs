using NHibernate;
using Parkway.Tools.NHibernate;
using System;

namespace Parkway.CBS.ClientRepository
{
    public class UoW : IUoW
    {
        private ITransaction _transaction;

        private SessionManager manager;

        public UoW(string sessionFactoryName, string sessionKeyContext)
        {
            manager = SessionManager.GetInstance(sessionFactoryName, sessionKeyContext);
            Session = manager.NewSession();
        }


        /// <summary>
        /// Get the session for this unit of work
        /// </summary>
        public ISession Session { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        public void OpenNewSession()
        {
            if (Session.IsOpen) { Session.Dispose(); }
            Session = manager.NewSession();
        }

        /// <summary>
        /// Start a stateless transaction
        /// </summary>
        public IUoW BeginStatelessTransaction()
        {
            if (IsTransactionActive()) { throw new Exception("An active transaction found"); }
            _transaction = Session.SessionFactory.OpenStatelessSession().BeginTransaction();
            return this;
        }


        /// <summary>
        /// Start a stateful transaction
        /// </summary>
        public IUoW BeginTransaction()
        {
            if (IsTransactionActive()) { throw new Exception("An active transaction found"); }
            _transaction = Session.BeginTransaction();
            return this;
        }


        /// <summary>
        /// check if the instance wide transaction is active
        /// </summary>
        /// <returns>bool</returns>
        protected bool IsTransactionActive()
        {
            if (_transaction != null && _transaction.IsActive) { return true; }
            return false;
        }


        /// <summary>
        /// Commit transaction
        /// </summary>
        public void Commit()
        {
            bool isTransactionActive = IsTransactionActive();
            try
            {
                if (!isTransactionActive)
                    throw new Exception("No active transaction found");

                _transaction.Commit();
            }
            catch (Exception)
            { throw; }
            finally
            { if (isTransactionActive) _transaction.Dispose(); }
        }


        /// <summary>
        /// Roll back changes
        /// </summary>
        public void Rollback()
        {
            try
            {
                if (!IsTransactionActive())
                    throw new Exception("No active transaction found");

                _transaction.Rollback();
            }
            finally { _transaction.Dispose(); }
        }


        /// <summary>
        /// Dispose the session value
        /// </summary>
        public void Dispose()
        { if (Session != null) { Session.Dispose(); } }

    }
}
