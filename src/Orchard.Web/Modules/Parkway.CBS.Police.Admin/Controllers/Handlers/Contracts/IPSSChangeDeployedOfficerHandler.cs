using Orchard;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IPSSChangeDeployedOfficerHandler : IDependency
    {

        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canViewRequests"></param>
        void CheckForPermission(Orchard.Security.Permissions.Permission permission);


        /// <summary>
        /// Get ChangeDeployedOfficerVM using specified deployment Id
        /// </summary>
        /// <param name="deploymentId"></param>
        /// <returns></returns>
        ChangeDeployedOfficerVM GetChangeDeployedOfficerVM(int deploymentId);


        /// <summary>
        /// Change deployed police officer
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="errorMessage"></param>
        void ChangeDeployedOfficer(ChangeDeployedOfficerVM userInput, ref string errorMessage);
    }
}
