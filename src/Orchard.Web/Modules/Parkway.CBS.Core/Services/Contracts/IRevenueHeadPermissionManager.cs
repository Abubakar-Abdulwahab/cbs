using Orchard;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IRevenueHeadPermissionManager<RevenueHeadPermission> : IDependency, IBaseManager<RevenueHeadPermission>
    {
        /// <summary>
        /// Get revenue head permissions
        /// </summary>
        /// <returns></returns>
        IEnumerable<RevenueHeadPermissionVM> GetRevenueHeadPermissions();
    }
}
