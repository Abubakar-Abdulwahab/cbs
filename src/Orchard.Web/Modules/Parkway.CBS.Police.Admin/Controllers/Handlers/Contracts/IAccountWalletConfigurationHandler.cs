using Orchard;
using Orchard.Security.Permissions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IAccountWalletConfigurationHandler : IDependency
    {
        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canAddWalletConfiguration"></param>
        void CheckForPermission(Permission canAddWalletConfiguration);

        /// <summary>
        /// Get the view model for adding account wallet configuration
        /// </summary>
        /// <returns><see cref="AddOrRemoveAccountWalletConfigurationVM"/></returns>
        AddOrRemoveAccountWalletConfigurationVM GetAddOrRemoveWalletConfigurationVM(int walletAcctId);

        /// <summary>
        /// Saves the configuration to <see cref="PSServiceRequestFlowApprover"/>
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="model"></param>
        void AddOrRemoveWalletConfiguration(ref List<ErrorModel> errors, AddOrRemoveAccountWalletConfigurationVM model);
    }
}
