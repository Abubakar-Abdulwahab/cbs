using Orchard.Commands;
using Orchard.Environment;
using Orchard.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.OrchardTenantStarter.Host
{
    /// <summary>
    /// <see cref="CommandHostEnvironment"/>
    /// </summary>
    class CBSTenantHostEnvironment : HostEnvironment
    {
        public CBSTenantHostEnvironment()
        {
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public override void RestartAppDomain()
        {
            throw new OrchardCommandHostRetryException(T("A change of configuration requires the session to be restarted."));
        }
    }
}
