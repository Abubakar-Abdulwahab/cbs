using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models.Enums;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IDeploymentAllowancePaymentRequestManager<DeploymentAllowancePaymentRequest> : IDependency, IBaseManager<DeploymentAllowancePaymentRequest>
    {
        /// <summary>
        /// Gets the payment reference 
        /// </summary>
        /// <param name="paymentRequestId"></param>
        /// <returns></returns>
        string GetWalletPaymentReference(long paymentRequestId);


        /// <summary>
        /// Gets deployment allowance payment request details with specified payment reference for settlement engine api call
        /// </summary>
        /// <param name="paymentRef"></param>
        /// <returns></returns>
        DeploymentAllowancePaymentRequestDTO GetDeploymentAllowancePaymentRequestDetailsWithPaymentRefForSettlement(string paymentRef);


        /// <summary>
        /// Moves the payment to next flow definition level
        /// </summary>
        /// <param name="paymentRequestId"></param>
        /// <param name="newDefinitionLevelId"></param>
        /// <param name="status"></param>
        void UpdatePaymentRequestFlowId(long paymentRequestId, int newDefinitionLevelId, PaymentRequestStatus status);
    }
}
