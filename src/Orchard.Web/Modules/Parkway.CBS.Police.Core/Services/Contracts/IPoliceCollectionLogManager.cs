using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using System;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPoliceCollectionLogManager<PoliceCollectionLog> : IDependency, IBaseManager<PoliceCollectionLog>
    {
        /// <summary>
        /// Get collection log details using the transaction log id
        /// </summary>
        /// <param name="transactionLogId"></param>
        /// <returns></returns>
        PoliceCollectionLog GetCollectionLogDetails(Int64 transactionLogId);

        /// <summary>
        /// Get payment details from transaction log for a particular invoice and save in police collection log table
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="requestId"></param>
        void SaveCollectionLogPayment(long invoiceId, long requestId);

        /// <summary>
        /// Get receipt details using the specified receipt number
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <returns>USSDValidateReceiptVM</returns>
        USSDValidateReceiptVM GetReceiptInfo(string receiptNumber);
    }
}
