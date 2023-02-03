using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models.Enums;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IAccountPaymentRequestItemManager<AccountPaymentRequestItem> : IDependency, IBaseManager<AccountPaymentRequestItem>
    {
        /// <summary>
        /// Get request items for processing
        /// </summary>
        /// <param name="paymentId"></param>
        IEnumerable<AccountWalletPaymentRequestItemDTO> GetPaymentRequestItems(string paymentId);

        /// <summary>
        /// Updates the <see cref="AccountPaymentRequestItem.TransactionStatus"/> using <paramref name="status"/>
        /// </summary>
        /// <param name="paymentRequestId"></param>
        /// <param name="status"></param>
        void UpdatePaymentRequestStatusId(long paymentRequestId, PaymentRequestStatus status);
    }
}
