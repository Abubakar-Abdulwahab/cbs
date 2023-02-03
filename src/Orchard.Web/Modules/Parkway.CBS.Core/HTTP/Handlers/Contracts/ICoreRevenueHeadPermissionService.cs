using Orchard;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICoreRevenueHeadPermissionService : IDependency
    {

        /// <summary>
        /// Assign revenue head constraints to selected expert system
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="admin"></param>
        void TryAssignRevenueHeadPermissionsToExpertSystem(AssignRevenueHeadPermissionConstraintsVM userInput, UserPartRecord admin);


        /// <summary>
        /// This method fetches existing revenue head constraints for the expert system with the specified Id.
        /// </summary>
        /// <param name="expertSystemId"></param>
        /// <param name="permissionId"></param>
        /// <returns>IEnumerable{RevenueHeadPermissionsConstraintsVM}</returns>
        IEnumerable<RevenueHeadPermissionsConstraintsVM> GetExistingConstraints(int expertSystemId, int permissionId);


        /// <summary>
        /// Delete existing records for expert system with specified Id
        /// </summary>
        /// <param name="expertSystemId"></param>
        void DeleteExistingExpertSystemRecords(int expertSystemId);

    }
}
