using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.CoreServices.Contracts
{
    public interface ICorePSSAdminUserService : IDependency
    {
        /// <summary>
        /// Create an admin user
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errors"></param>
        /// <returns>bool</returns>
        PSSAdminUsers TryCreateAdminUser(AdminUserCreationVM model, ref List<ErrorModel> errors);

        /// <summary>
        /// Edit an admin user
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errors"></param>
        /// <returns>bool</returns>
        AdminUserVM TryEditAdminUser(AdminUserCreationVM model, ref List<ErrorModel> errors);

        /// <summary>
        /// Get command
        /// </summary>
        /// <param name="adminId"></param>
        /// <returns></returns>
        CommandVM GetCommandForAdmin();
    }
}
