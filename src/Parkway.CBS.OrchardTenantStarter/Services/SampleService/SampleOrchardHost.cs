using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.OrchardTenantStarter.Services.SampleService
{
    public class SampleOrchardHost : CBSOrchardHost<SampleTenantHostContext>
    {
        public void DoSample(string tenantName)
        {
            context.CreateStandaloneEnvironment(tenantName);
        }

        public void DoP()
        {
            context.SampleTenantHost = new SampleTenantHost { };
            //context.SampleTenantHost.DoSome(context.CBSTenantHost, "Default");

        }

        //public override void CreateAppDomain()
        //{
        //    context.SampleTenantHost = _tenantHostContextProvider.CreateAppDomain
        //    throw new NotImplementedException();
        //}

        //public override void StartSession()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
