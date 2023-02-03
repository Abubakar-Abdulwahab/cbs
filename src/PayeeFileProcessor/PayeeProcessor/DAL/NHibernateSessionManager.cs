using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PayeeProcessor.DAL
{
    public class NHibernateSessionManager
    {
        private Configuration Config;
        private ISessionFactory SessionFactory;
        private ISession Session;

        private IStatelessSession _statelessSession;

        public NHibernateSessionManager()
        {
            Config = new Configuration();
            Configure(Config);
            SessionFactory = Config.BuildSessionFactory();
        }

        private void Configure(Configuration cfg)
        { 
            cfg.DataBaseIntegration(x => {
                x.ConnectionStringName = "default";
                x.Driver<SqlClientDriver>();
                x.Dialect<MsSql2008Dialect>();
                x.IsolationLevel = IsolationLevel.Serializable;
                 
                
            });
            cfg.AddAssembly(Assembly.GetExecutingAssembly());
 
        }

        /// <summary>
        /// Opens a session
        /// </summary>
        /// <returns></returns>
        public ISession OpenSession()
        {
            if (Session == null)
            {
                Session = SessionFactory.OpenSession();

            }
            return Session;
        }

        /// <summary>
        /// Close a Session 
        /// </summary>
        public void CloseSession()
        {
            if (Session != null && Session.IsOpen)
            {
                Session.Flush();
                Session.Close();
            }
            Session = null;
        }

        /// <summary>
        /// Opens a Stateless Session 
        /// </summary>
        /// <returns></returns>
        public IStatelessSession OpenStatelessSession()
        {
            if(_statelessSession == null)
            {
                _statelessSession = SessionFactory.OpenStatelessSession();
            }
            return _statelessSession;
        }
    }
}
