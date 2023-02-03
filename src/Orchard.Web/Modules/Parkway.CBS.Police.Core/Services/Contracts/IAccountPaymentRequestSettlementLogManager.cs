using Orchard;
using Parkway.CBS.Core.Services.Contracts;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IAccountPaymentRequestSettlementLogManager<AccountPaymentRequestSettlementLog> : IDependency, IBaseManager<AccountPaymentRequestSettlementLog>
    {
        /// <summary>
        /// Updates the transaction status for <see cref="AccountPaymentRequestItem"/>
        /// </summary>
        /// <param name="reference"></param>
        void UpdatePaymentRequestItemTransactionStatusFromLog(string reference);
    }
}
