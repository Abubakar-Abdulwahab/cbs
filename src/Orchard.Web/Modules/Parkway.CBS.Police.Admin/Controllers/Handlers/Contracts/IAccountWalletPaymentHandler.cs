using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IAccountWalletPaymentHandler : IDependency
    {
        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canInitateAccountWalletPayment"></param>
        void CheckForPermission(Orchard.Security.Permissions.Permission canInitateAccountWalletPayment);

        /// <summary>
        /// Get the view model for adding account wallet configuration
        /// </summary>
        /// <returns><see cref="InitiateAccountWalletPaymentVM"/></returns>
        InitiateAccountWalletPaymentVM GetInitiateWalletPaymentVM();

        /// <summary>
        /// Validates and Initates wallet payment request
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="model"></param>
        void InitiateWalletPaymentRequest(ref List<ErrorModel> errors, InitiateAccountWalletPaymentVM model);
    }
}
