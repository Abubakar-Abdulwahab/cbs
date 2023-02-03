using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IAccountPaymentRequestManager<AccountPaymentRequest> : IDependency, IBaseManager<AccountPaymentRequest>
    {
        /// <summary>
        /// Gets the account payment details using the <paramref name="paymentId"/>
        /// </summary>
        /// <param name="paymentId"></param>
        /// <returns></returns>
        AccountPaymentRequestVM GetWalletPaymentDetailByPaymentId(string paymentId);

        /// <summary>
        /// Get details for sending request to the settlement engine
        /// </summary>
        /// <param name="paymentId"></param>
        /// <returns></returns>
        AccountPaymentSettlementRequestVM GetWalletPaymentDetailForSettlementByPaymentId(string paymentId);

        /// <summary>
        /// Gets the payment reference 
        /// </summary>
        /// <param name="paymentRequestId"></param>
        /// <returns></returns>
        string GetWalletPaymentReference(long paymentRequestId);

        /// <summary>
        /// Gets the view model for the approval detail view
        /// </summary>
        /// <param name="paymentId"></param>
        /// <returns></returns>
        WalletPaymentRequestApprovalDetailVM GetWalletPaymentRequestApprovalDetailVM(string paymentId);

        /// <summary>
        /// Moves the payment to next flow definition level
        /// </summary>
        /// <param name="paymentRequestId"></param>
        /// <param name="newDefinitionLevelId"></param>
        /// <param name="status"></param>
        void UpdatePaymentRequestFlowId(long paymentRequestId, int newDefinitionLevelId, PaymentRequestStatus status);
    }
}
