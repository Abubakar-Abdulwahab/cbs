using Orchard;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IPSSEndOfficerDeploymentHandler : IDependency
    {

        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canViewRequests"></param>
        void CheckForPermission(Orchard.Security.Permissions.Permission permission);


        /// <summary>
        /// Get officer details to end the deployment using specified deployment Id
        /// </summary>
        /// <param name="deploymentId"></param>
        /// <returns></returns>
        EndOfficerDeploymentVM GetDeployedOfficerDetails(int deploymentId);


        /// <summary>
        /// End the deployment of an officer
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="adminUserId"></param>
        void EndOfficerDeployment(EndOfficerDeploymentVM userInput, int adminUserId);
    }
}
