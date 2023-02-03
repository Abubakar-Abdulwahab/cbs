using Orchard;
using Orchard.Environment;
using Orchard.Environment.ShellBuilders;
using Orchard.HostContext;
using Parkway.CBS.OrchardTenantStarter.HostContext;
using Parkway.CBS.OrchardTenantStarter.HostContext.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.OrchardTenantStarter
{
    public class CBSOrchardHost
    {
        private readonly ICBSHostContextProvider _tenantHostContextProvider;
        protected CBSTenantHostContext context;

        public CBSOrchardHost()
        { _tenantHostContextProvider = new CBSHostContextProviderImpl(); }


        public void SetHostContextFromHostProvider()
        { context = _tenantHostContextProvider.CreateContext(); }


        public void ProcessIPPIS(string implementingClassName, string tenant, string fullFilePath, int month, int year)
        {
            context.CBSTenantHost.ProcessIPPISFile(implementingClassName, tenant, fullFilePath, month, year);
        }
    }
}
