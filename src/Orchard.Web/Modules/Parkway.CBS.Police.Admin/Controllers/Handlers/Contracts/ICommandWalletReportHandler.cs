using Orchard;
using Orchard.Security.Permissions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface ICommandWalletReportHandler : IDependency
    {
        /// <summary>
        /// Get Reports VM for Command Wallet Report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        CommandReportVM GetVMForReports(CommandWalletReportSearchParams searchParams);

        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canViewRequests"></param>
        void CheckForPermission(Permission canViewOfficers);
    }
}
