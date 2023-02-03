using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models.Enums;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IDeploymentAllowancePaymentRequestItemManager<DeploymentAllowancePaymentRequestItem> : IDependency, IBaseManager<DeploymentAllowancePaymentRequestItem>
    {
        /// <summary>
        /// Get deployment allowance payment request items for deployment allowance payment request with specified request invoice id and account wallet configuration with specified command id
        /// </summary>
        /// <param name="requestInvoiceId"></param>
        /// <param name="commandId"></param>
        /// <returns></returns>
        List<DeploymentAllowancePaymentRequestItemDTO> GetDeploymentAllowancePaymentRequestItemsForBatchWithRequestInvoiceId(long requestInvoiceId, int commandId);


        /// <summary>
        /// Gets items in deployment allowance payment request with specified payment reference
        /// </summary>
        /// <param name="paymentRef">Deployment Allowance Payment Request Payment Reference</param>
        /// <returns>IEnumerable<DeploymentAllowancePaymentRequestItemDTO></returns>
        IEnumerable<DeploymentAllowancePaymentRequestItemDTO> GetItemsInDeploymentAllowancePaymentRequestWithPaymentRef(string paymentRef);


        /// <summary>
        /// Updates the <see cref="DeploymentAllowancePaymentRequestItem.TransactionStatus"/> using <paramref name="status"/>
        /// </summary>
        /// <param name="paymentRequestId"></param>
        /// <param name="status"></param>
        void UpdatePaymentRequestStatusId(long paymentRequestId, PaymentRequestStatus status);
    }
}
