using Orchard;
using Orchard.Host;
using Orchard.HostContext;
using Orchard.Users.Models;
using Parkway.CBS.OrchardTenantStarter.Host;
using System;

namespace Parkway.CBS.OrchardTenantStarter.HostContext
{
    /// <summary>
    /// Central Billing System's implementation of the CommandHostContext
    /// <see cref="CommandHostContext"/>
    /// </summary>
    public class CBSTenantHostContext : CommandHostContext
    {
        public CBSTenantHost CBSTenantHost { get; set; }

        private new  CommandHost CommandHost { get; set; }
    }    
}
