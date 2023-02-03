using Parkway.CBS.OrchardTenantStarter.HostContext.X;
using Parkway.CBS.OrchardTenantStarter.HostContext.X.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.OrchardTenantStarter
{
    public class CBSOrchardHostX
    {
        private readonly ICBSHostContextProviderX _tenantHostContextProvider;
        //protected H context;
        protected CBSTenantHostContextX context;

        public CBSOrchardHostX()
        { _tenantHostContextProvider = new CBSHostContextProviderImplX(); }


        public void SetHostContextFromHostProvider()
        { context = _tenantHostContextProvider.CreateContext(); }
        //{ context = _tenantHostContextProvider.CreateContext<H>(); }

        public void InstantiateAgent() { context.StartSession(); }


        public void DoSth()
        {
            context.CreateStandaloneEnvironment("Default");
        }


    }
}
