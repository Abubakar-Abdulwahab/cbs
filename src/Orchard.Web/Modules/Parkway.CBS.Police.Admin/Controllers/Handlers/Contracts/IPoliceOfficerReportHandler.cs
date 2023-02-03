using Orchard;
using Orchard.Security.Permissions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IPoliceOfficerReportHandler : IDependency
    {
        /// <summary>
        /// Get Reports VM for Police Officer Report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        PoliceOfficerReportVM GetVMForReports(PoliceOfficerSearchParams searchParams);

        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canViewRequests"></param>
        void CheckForPermission(Permission canViewRequests);


        /// <summary>
        /// Get police officers of the rank with specified rankId that belong to the command with the specified commandId
        /// </summary>
        /// <param name="commandId"></param>
        /// <param name="rankId"></param>
        /// <returns></returns>
        APIResponse GetPoliceOfficersByCommandAndRankId(int commandId, long rankId);
    }
}
