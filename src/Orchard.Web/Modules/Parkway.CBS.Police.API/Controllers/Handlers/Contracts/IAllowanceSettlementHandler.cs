using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.HelperModels;

namespace Parkway.CBS.Police.API.Controllers.Handlers.Contracts
{
    public interface IAllowanceSettlementHandler : IDependency
    {
        /// <summary>
        /// Update the payment status for a particular deployment allowance request
        /// </summary>
        /// <param name="model"></param>
        /// <returns>APIResponse</returns>
        APIResponse UpdateDeploymentAllowanceStatus(DeploymentAllowancePaymentNotificationModel model);
    }
}
