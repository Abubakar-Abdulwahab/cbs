using Orchard;
using Parkway.CBS.Police.Core.Models.Enums;

namespace Parkway.CBS.Police.Core.PaymentRequestSettlement.Contracts
{
    public interface IPaymentRequestService : IDependency
    {
        /// <summary>
        /// Send request to settlement engine for processing
        /// </summary>
        /// <param name="paymentId"></param>
        string BeginRequestPaymentProcess(string paymentId);

        /// <summary>
        /// Updates the <see cref="AccountPaymentRequest.FlowDefinitionLevel"/> and <see cref="AccountPaymentRequest.PaymentRequestStatus"/> 
        /// Updates the <see cref="AccountPaymentRequestItem.FlowDefinitionLevel"/> and <see cref="AccountPaymentRequestItem.PaymentRequestStatus"/> 
        /// </summary>
        /// <param name="paymentRequestId"></param>
        /// <param name="currentDefinintionLevelPosition"></param>
        /// <param name="definitionId"></param>
        /// <param name="status"></param>
        void UpdatePaymentRequestFlow(long paymentRequestId, int currentDefinintionLevelPosition, int definitionId, PaymentRequestStatus status);
    }
}
