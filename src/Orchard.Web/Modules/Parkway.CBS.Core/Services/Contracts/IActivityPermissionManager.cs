using Orchard;
using System.Collections.Generic;
using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IActivityPermissionManager<ActivityPermission> : IDependency, IBaseManager<ActivityPermission>
    {

        /// <summary>
        /// Gets the permission preference
        /// </summary>
        /// <param name="mdaId"></param>
        /// <param name="permissionName"></param>
        /// <param name="revenueHeadId"></param>
        /// <returns>Dictionary{int, bool}</returns>
        Dictionary<int, bool> GetPermissionPreference(CBSPermissionName permissionName, int revenueHeadId, int mdaId);

    }
}