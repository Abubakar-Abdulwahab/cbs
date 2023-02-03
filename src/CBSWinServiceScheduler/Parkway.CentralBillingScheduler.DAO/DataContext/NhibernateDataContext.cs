using NHibernate;
using Parkway.CentralBillingScheduler.DAO.DataContext.Contracts;
using Parkway.Tools.NHibernate;

namespace Parkway.CentralBillingScheduler.DAO.DataContext
{
    public class NHibernateDataContext : INHibernateDataContext
    {
        private SessionManager manager;

        public NHibernateDataContext(string sessionFactoryName)
        {
            try
            {
                manager = Parkway.Tools.NHibernate.SessionManager.GetInstance(sessionFactoryName, "");
                if(manager == null)
                {
                    var bmanager = SessionManager.Instance;
                    manager = Parkway.Tools.NHibernate.SessionManager.GetInstance(sessionFactoryName, "");
                }
            }
            catch (System.Exception exception)
            {

                throw;
            }
        }

        public ISession NewNHibernateSession()
        {
            return manager.NewSession();
        }
    }
}
