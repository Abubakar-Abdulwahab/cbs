using Orchard.HostContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.OrchardTenantStarter.HostContext.Contracts
{
    /// <summary>
    /// <see cref="ICommandHostContextProvider"/>
    /// </summary>
    public interface ICBSHostContextProvider
    {
        CBSTenantHostContext CreateContext();
        //H CreateContext<H>() where H : CBSTenantHostContext;
        void Shutdown(CBSTenantHostContext context);
    }
}
