using Orchard.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Commands;

namespace Parkway.CBS.OrchardTenantStarter.Host
{
    /// <summary>
    /// Command line specific "no-op" background service implementation.
    /// Note that we make this class "internal" so that it's not auto-registered
    /// by the Orchard Framework (it is registered explicitly by the command
    /// line host).
    /// <see cref="CommandBackgroundService"/>
    /// </summary>
    internal class CBSTenantBackgroundService : IBackgroundService
    {
        public void Sweep() { /*Don't run any background service in command line*/ }
    }
}
