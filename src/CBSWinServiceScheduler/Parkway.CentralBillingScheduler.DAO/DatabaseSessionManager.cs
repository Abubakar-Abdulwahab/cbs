using NHibernate;
using Parkway.CentralBillingScheduler.DAO.DataContext;
using Parkway.CentralBillingScheduler.DAO.DataContext.Contracts;
//using Parkway.Tools.NHibernate;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CentralBillingScheduler.DAO
{
    public class DatabaseSessionManager : IDatabaseSessionManager
    {
        private static ConcurrentDictionary<string, INHibernateDataContext> SessionManagers = new ConcurrentDictionary<string, INHibernateDataContext>();

        public INHibernateDataContext GetSessionManager(string sessionFactoryName)
        {
            //check if session factory already has an instance
            var manager = SessionManagers.Where(mng => mng.Key == sessionFactoryName).Select(mng => mng.Value).FirstOrDefault();
            
            if (manager == null)
            {
                manager = new NHibernateDataContext(sessionFactoryName);
                SessionManagers.TryAdd(sessionFactoryName, manager);
            }
            return manager;
        }

        public INHibernateDataContext ReInitializeSessionManager(string sessionFactoryName)
        {
            throw new Exception();
            //var manager = SessionManagers.Where(mng => mng.Key == sessionFactoryName).Select(mng => mng.Value).FirstOrDefault();
            //manager = new NHibernateDataContext(sessionFactoryName);
            //SessionManagers.(sessionFactoryName, manager);
            
        }
    }
}
