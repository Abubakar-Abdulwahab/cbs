using NHibernate;
using Parkway.Tools.NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientRepository
{
    public interface IUoW : IDisposable
    {
        ISession Session { get; }

        /// <summary>
        /// Commit transaction
        /// </summary>
        void Commit();

        /// <summary>
        /// Start a stateless transaction
        /// 
        /// </summary>
        IUoW BeginStatelessTransaction();


        /// <summary>
        /// Start a stateful transaction
        /// </summary>
        IUoW BeginTransaction();

        
        void Rollback();
    }
}
