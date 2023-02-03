using Orchard;
using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IPSSAdminUserAssignEscortProcessFlowAJAXHandler : IDependency
    {
        /// <summary>
        /// Gets details of admin user with specified username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        APIResponse GetAdminUser(string username);


        /// <summary>
        /// Gets active escort process stage definitions for specified command type
        /// </summary>
        /// <param name="commandTypeId"></param>
        /// <returns></returns>
        APIResponse GetEscortProcessStageDefinitions(int commandTypeId);
    }
}
