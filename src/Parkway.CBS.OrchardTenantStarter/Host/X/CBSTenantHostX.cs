using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using Orchard;
using Orchard.Host;
using Orchard.Users.Models;

namespace Parkway.CBS.OrchardTenantStarter.Host.X
{
    public class CBSTenantHostX : MarshalByRefObject, IRegisteredObject
    {
        //protected object _agent;

        public CBSTenantHostX()
        {
            HostingEnvironment.RegisterObject(this);
        }


        //public object GetAgent() { return _agent; }


        [SecurityCritical]
        public override object InitializeLifetimeService()
        {
            // never expire the cross-AppDomain lease on this object
            return null;
        }


        //public void CreateStandaloneEnvironment(string tenant)
        //{
        //    using (var sds = (IWorkContextScope)_agent.GetType().GetMethod("CreateStandaloneEnvironment").Invoke(_agent, new object[] { tenant }))
        //    {
        //        try
        //        {
        //            var result = sds.Resolve<IOrchardServices>().TransactionManager.GetSession().Get<UserPartRecord>(2);
        //        }
        //        catch (Exception)
        //        {
        //            throw;
        //        }
        //    }
        //}

        //internal CommandReturnCodes StartSession()
        //{
        //    _agent = CreateAgent();
        //    return StartHost(_agent);
        //}

        //protected object CreateAgent()
        //{
        //    return Activator.CreateInstance("Parkway.CBS.OrchardTenantStarter", "Parkway.CBS.OrchardTenantStarter.Host.X.CBSTenantHostAgentX").Unwrap();
        //}


        [SecuritySafeCritical]
        public void Stop(bool immediate)
        {
            HostingEnvironment.UnregisterObject(this);
        }

        protected CommandReturnCodes StopHost(object agent)
        {
            return (CommandReturnCodes)agent.GetType().GetMethod("StopHost").Invoke(agent, new object[] { });
        }

        protected CommandReturnCodes StartHost(object agent)
        {
            return (CommandReturnCodes)agent.GetType().GetMethod("StartHost").Invoke(agent, new object[] { });
        }
    }
}