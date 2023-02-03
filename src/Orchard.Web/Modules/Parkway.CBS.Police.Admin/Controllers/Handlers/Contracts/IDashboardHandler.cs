using Orchard;
using Parkway.CBS.Police.Admin.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IDashboardHandler : IDependency
    {
        /// <summary>
        /// Get POSSAP dashboard view
        /// </summary>
        /// <returns>PSSDashboardViewModel</returns>
        PSSDashboardViewModel GetDashboardView();
    }
}
