using NHibernate;
using NHibernate.Engine;
using Parkway.Tools.NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.PayeeProcessor
{
    public class NHibernateSessionManager
    { 
        /// <summary>
        /// Get a NHibernate Session for persistence to database
        /// </summary>
        /// <param name="session_factory"></param>
        /// <param name="sessionkey"></param>
        /// <returns></returns>
        public static ISession GetSession(string session_factory , string sessionkey)
        {           
            SessionManager mgr = SessionManager.GetInstance(session_factory, sessionkey);
            if (mgr != null)
                return mgr.GetSession();
            else               
                throw new Exception($"Could not get session for session with factory {session_factory} and Session Key {sessionkey}");
        } 

        /// <summary>
        /// GEt a NHibernate Stateless session for persistence to database
        /// </summary>
        /// <param name="session_factory"></param>
        /// <param name="sessionkey"></param>
        /// <returns></returns>

        public static IStatelessSession GetStatelessSession(string session_factory, string sessionkey)
        { 
            var _sessionFactory = (ISessionFactoryImplementor)SessionManager.GetInstance(session_factory, sessionkey)
                                                                            .GetSession().SessionFactory; 
            return _sessionFactory.OpenStatelessSession();

        }

    }
}
