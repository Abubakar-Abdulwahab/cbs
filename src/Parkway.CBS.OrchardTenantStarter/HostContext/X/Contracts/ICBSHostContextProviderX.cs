using Parkway.CBS.OrchardTenantStarter.Host.X;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.OrchardTenantStarter.HostContext.X.Contracts
{
    public interface ICBSHostContextProviderX
    {
        CBSTenantHostContextX CreateContext();
        //H CreateContext<H>() where H : CBSTenantHostContext;
        void Shutdown(CBSTenantHostContextX context);
    }
}
