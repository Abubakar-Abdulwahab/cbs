using Orchard;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IPSSAdminUserReportHandler : IDependency
    {
        /// <summary>
        /// Gets admin user view model
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        PSSAdminUserReportVM GetVMForReports(AdminUserReportSearchParams adminUserReportSearchParams);

        /// <summary>
        /// Toggles <see cref="PSSAdminUsers.IsActive"/> with the value from <paramref name="isActive"/>
        /// </summary>
        /// <param name="adminUserId"></param>
        /// <param name="isActive"></param>
        /// <returns>The user's username </returns>
        string ToggleIsActiveAdminUser(int adminUserId, bool isActive);
    }
}
