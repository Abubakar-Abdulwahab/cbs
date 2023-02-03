using Orchard;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.AdminUserReport.Contracts
{
    public interface IAdminUserReportFilter : IDependency
    {
        /// <summary>
        /// Gets view model for admin user report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        dynamic GetAdminUserReportViewModel(AdminUserReportSearchParams searchParams);
    }
}
