using Orchard;
using Orchard.Security.Permissions;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IExpenditureHeadReportHandler : IDependency
    {
        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canViewExpenditureHeadReport"></param>
        void CheckForPermission(Permission canViewExpenditureHeadReport);

        /// <summary>
        /// Get Reports VM for Expenditure head Report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        ExpenditureHeadReportVM GetVMForReports(ExpenditureHeadReportSearchParams searchParams);
    }
}
