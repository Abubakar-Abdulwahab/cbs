using Orchard;
using Orchard.Security.Permissions;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IAccountWalletPaymentApprovalHandler : IDependency
    {
        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canViewWalletPaymentApprovalReport"></param>
        void CheckForPermission(Permission canViewWalletPaymentApprovalReport);

        /// <summary>
        /// Gets view model for the view 
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        AccountWalletPaymentApprovalRequestVM GetPaymentApprovalRequestVM(AccountWalletPaymentApprovalSearchParams searchParams);

        /// <summary>
        /// Gets the view model for the details view
        /// </summary>
        /// <param name="paymentId"></param>
        /// <returns></returns>
        WalletPaymentRequestApprovalDetailVM GetViewDetailVM(string paymentId);

        /// <summary>
        /// Approves or Declines a request based on the value of <paramref name="approveRequest"/>
        /// And sends to payment provider if it's an authorization
        /// </summary>
        /// <param name="paymentId"></param>
        /// <param name="approveRequest"></param>
        /// <exception cref="NoRecordFoundException"></exception>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        string ProcessPaymentRequest(string paymentId, bool approveRequest);
    }
}
