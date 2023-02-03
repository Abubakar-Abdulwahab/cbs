using Orchard;
using Orchard.Security.Permissions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface ICommandWalletHandler : IDependency
    {
        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canCreateCommandWallet"></param>
        void CheckForPermission(Permission canCreateCommandWallet);

        /// <summary>
        /// Gets the view model for the get add command wallet
        /// </summary>
        /// <returns></returns>
        AddCommandWalletVM GetAddCommandWalletVM();

        /// <summary>
        /// Validates and Creates command wallet in <see cref="CommandWalletDetails"/>
        /// </summary>
        /// <param name="errors"> Validation errors</param>
        /// <param name="model">User input model</param>
        void AddCommandWallet(ref List<ErrorModel> errors, AddCommandWalletVM model);
    }
}
