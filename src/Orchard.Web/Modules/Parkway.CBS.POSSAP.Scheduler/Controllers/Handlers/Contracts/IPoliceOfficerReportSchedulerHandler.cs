using Orchard;
using Orchard.Security.Permissions;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.POSSAP.Scheduler.Controllers.Handlers.Contracts
{
    public interface IPoliceOfficerReportSchedulerHandler : IDependency
    {
        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="CanViewOfficersSchedule"></param>
        void CheckForPermission(Permission CanViewOfficersSchedule);


        PoliceOfficerReportVM GetVMForReports(PoliceOfficerSearchParams searchParams);
    }
}
