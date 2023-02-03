using Orchard;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Module.Controllers.Handlers.Contracts
{
    public interface IRevenueHeadPermissionsHandler : IDependency
    {

        /// <summary>
        /// Get revenue head permission constraints VM
        /// </summary>
        /// <param name="expertSystemsId"></param>
        /// <param name="permissionId"></param>
        /// <returns>AssignRevenueHeadPermissionConstraintsVM</returns>
        AssignRevenueHeadPermissionConstraintsVM GetRevenueHeadPermissionConstraintsVM(int expertSystemsId, int permissionId);


        /// <summary>
        /// Assign selected revenue heads constraints to expert system
        /// </summary>
        /// <param name="userInput"></param>
        void AssignExpertSystemToSelectedRevenueHeads(AssignRevenueHeadPermissionConstraintsVM userInput);

        /// <summary>
        /// Get a list of revenue heads for the mdas with the specified Id
        /// </summary>
        /// <param name="mdaId">If the request takes too long to complete before the user initiates another this will contain multiple MDA ids otherwise it would have just one</param>
        /// <returns></returns>
        dynamic GetRevenueHeadsPerMda(string mdaIds);

        /// <summary>
        /// Gets MDAs restricted to the specified Access Type
        /// </summary>
        /// <param name="accessType"></param>
        /// <returns></returns>
        List<MDAVM> GetMDAsForAccessType(string accessType);
    }
}
