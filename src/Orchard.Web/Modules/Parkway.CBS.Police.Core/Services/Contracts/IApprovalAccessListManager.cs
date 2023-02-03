using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IApprovalAccessListManager<ApprovalAccessList> : IDependency, IBaseManager<ApprovalAccessList>
    {
        /// <summary>
        /// Checks if the user already has approval access
        /// </summary>
        /// <param name="commandId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool CheckIfCommandExistForUser(int commandId, int userId);

        /// <summary>
        /// Returns a list of CommandId and service type for ApprovalAccessRoleUser using <paramref name="userId"/>
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<ApprovalAccessListVM> GetApprovalAccessListByUserId(int userId);

        /// <summary>
        /// Returns all the services the user with the specified id has access to
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<int> GetSelectedServicesFromAccessListByuserId(int userId);

        /// <summary>
        /// Deletes the specified service for specified access role user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="serviceId"></param>
        void DeleteServiceInAccessList(int userId, int serviceId);
    }
}
