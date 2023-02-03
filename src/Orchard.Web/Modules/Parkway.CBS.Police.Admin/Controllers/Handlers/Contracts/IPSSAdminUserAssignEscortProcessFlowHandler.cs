using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IPSSAdminUserAssignEscortProcessFlowHandler : IDependency
    {
        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="permission"></param>
        void CheckForPermission(Orchard.Security.Permissions.Permission permission);

        /// <summary>
        /// Gets Assign Escort Process Flow VM
        /// </summary>
        /// <returns></returns>
        AssignEscortProcessFlowVM GetAssignEscortProcessFlowVM();

        /// <summary>
        /// Assigns selected users to specified escort process stage definitions
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="userId"></param>
        /// <param name="errors"></param>
        void AssignProcessFlowsToUsers(AssignEscortProcessFlowVM userInput, int userId, ref List<ErrorModel> errors);
    }
}
