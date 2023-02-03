using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.Models;
namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IRegularizationUnknownOfficersDeploymentAllowancePaymentSettlementLogManager<RegularizationUnknownOfficersDeploymentAllowancePaymentSettlementLog> : IDependency, IBaseManager<RegularizationUnknownOfficersDeploymentAllowancePaymentSettlementLog>
    {
        /// <summary>
        /// Updates the transaction status and UpdatedAtUtc for <see cref="DeploymentAllowancePaymentRequestItem"/>
        /// </summary>
        /// <param name="reference"></param>
        void UpdateRegularizationUnknownOfficersDeploymentAllowancePaymentRequestItemTransactionStatusFromLog(string reference);
    }
}