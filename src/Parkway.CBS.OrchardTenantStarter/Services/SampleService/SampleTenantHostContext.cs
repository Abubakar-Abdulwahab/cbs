using Orchard;
using Orchard.Users.Models;
using Parkway.CBS.OrchardTenantStarter.HostContext;
using System;

namespace Parkway.CBS.OrchardTenantStarter.Services.SampleService
{
    public class SampleTenantHostContext : CBSTenantHostContext
    {

        public void CreateStandaloneEnvironment(string tenant)
        {
            //this.CBSTenantHost.GetAgent();
            using (var sds = (IWorkContextScope)this.CBSTenantHost.GetAgent().GetType().GetMethod("CreateStandaloneEnvironment").Invoke(this.CBSTenantHost.GetAgent(), new object[] { tenant }))
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

        public SampleTenantHost SampleTenantHost { get; set; }
    }
}
