using NHibernate;
using Parkway.CentralBillingScheduler.DAO.DataContext.Contracts;
using Parkway.Tools.NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CentralBillingScheduler.DAO
{
    public interface IDatabaseSessionManager
    {
        INHibernateDataContext GetSessionManager(string sessionFactoryName);

        INHibernateDataContext ReInitializeSessionManager(string sessionFactoryName);
    }
}
