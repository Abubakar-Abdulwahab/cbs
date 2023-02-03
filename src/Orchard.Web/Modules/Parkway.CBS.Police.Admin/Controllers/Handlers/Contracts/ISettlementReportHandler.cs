using Orchard;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface ISettlementReportHandler : IDependency
    {
        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="permission"></param>
        void CheckForPermission(Orchard.Security.Permissions.Permission permission);

        /// <summary>
        /// Gets the view model for settlement report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        PSSSettlementReportVM GetVMForReports(SettlementReportSearchParams searchParams);
    }
}
