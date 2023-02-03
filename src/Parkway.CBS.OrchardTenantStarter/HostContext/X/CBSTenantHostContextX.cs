using System;
using System.IO;
using System.Web.Hosting;
using Orchard;
using Orchard.Host;
using Orchard.HostContext;
using Orchard.Users.Models;
using Parkway.CBS.OrchardTenantStarter.Host.X;

namespace Parkway.CBS.OrchardTenantStarter.HostContext.X
{
    public class CBSTenantHostContextX : MarshalByRefObject, IRegisteredObject
    {
        public CommandReturnCodes StartSessionResult { get; set; }
        public CommandReturnCodes RetryResult { get; set; }

        public OrchardParameters Arguments { get; set; }

        public DirectoryInfo OrchardDirectory { get; set; }

        public bool DisplayUsageHelp { get; set; }

        public CommandHost CommandHost { get; set; }

        public Logger Logger { get; set; }

        public CBSTenantHostContextX()
        {
            HostingEnvironment.RegisterObject(this);
        }
        public object Agent { get; set; }

        public CBSTenantHostX CBSTenantHostX { get; set; }

        internal CommandReturnCodes StartSession()
        {
            Agent = CreateAgent();
            return StartHost(Agent);
        }


        protected object CreateAgent()
        {
            return Activator.CreateInstance("Parkway.CBS.OrchardTenantStarter", "Parkway.CBS.OrchardTenantStarter.Host.X.CBSTenantHostAgentX").Unwrap();
        }


        protected CommandReturnCodes StopHost(object agent)
        {
            return (CommandReturnCodes)agent.GetType().GetMethod("StopHost").Invoke(agent, new object[] { });
        }


        protected CommandReturnCodes StartHost(object agent)
        {
            return (CommandReturnCodes)agent.GetType().GetMethod("StartHost").Invoke(agent, new object[] { });
        }

        public void Stop(bool immediate)
        {
            if (Agent != null)
            {
                StopHost(Agent);
                Agent = null;
            }
        }

        public void StopSession()
        {
            if (Agent != null)
            {
                StopHost(Agent);
                Agent = null;
            }
        }


        public void CreateStandaloneEnvironment(string tenant)
        {
            using (var sds = (IWorkContextScope)Agent.GetType().GetMethod("CreateStandaloneEnvironment").Invoke(Agent, new object[] { tenant }))
            {
                try
                {
                    var result = sds.Resolve<IOrchardServices>().TransactionManager.GetSession().Get<UserPartRecord>(2);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
