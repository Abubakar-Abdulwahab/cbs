using Orchard;
using Orchard.Environment.ShellBuilders;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.StateConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ReadyCash.Go.Controllers.Handlers.Contracts
{
    public interface IRenderFormFieldsHandler : IDependency
    {
        /// <summary>
        /// Get RevenueHead form controls using the revenueheadid, payerId and tenant sitename
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <param name="payerId"></param>
        /// <param name="fileSiteName"></param>
        /// <returns>IEnumerable<FormControlViewModel></returns>
        IEnumerable<FormControlViewModel> GetFormControlsForRevenueHead(int revenueHeadId, string payerId, string fileSiteName);


        /// <summary>
        /// Get tenant config settings
        /// </summary>
        /// <param name="vendorCode"></param>
        /// <returns>StateConfig</returns>
        StateConfig GetTenantConfig(string vendorCode);

        /// <summary>
        /// Get shell context for thsi operation
        /// </summary>
        /// <param name="siteName"></param>
        /// <returns>ShellContext</returns>
        ShellContext GetShellContext(string siteName);
    }
}
