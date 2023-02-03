using Orchard;
using Orchard.Users.Models;
using Parkway.CBS.OrchardTenantStarter.Host;
using System;
using System.Web.Hosting;

namespace Parkway.CBS.OrchardTenantStarter.Services.SampleService
{
    public class SampleTenantHost : CBSTenantHost
    {
        //public SampleTenantHost()
        //{
        //    HostingEnvironment.RegisterObject(this);
        //}

        //public void DoSome(CBSTenantHost host, string tenant)
        //{
        //    using (var sds = (IWorkContextScope)host.GetAgent().GetType().GetMethod("CreateStandaloneEnvironment").Invoke(host.GetAgent(), new object[] { tenant }))
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

        //public void Stop(bool immediate)
        //{
        //}
    }
}