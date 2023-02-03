using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CentralBillingScheduler.DAO.DataContext.Contracts
{
    public interface INHibernateDataContext
    {
        ISession NewNHibernateSession();
    }
}
