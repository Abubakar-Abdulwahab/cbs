using Orchard;
using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IPAYEBatchItemReceiptManager<PAYEBatchItemReceipt> : IDependency, IBaseManager<PAYEBatchItemReceipt>
    {
        /// <summary>
        /// Get the VM for a receipt
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <returns>ReceiptViewModel</returns>
        PAYEBatchItemReceiptViewModel GetReceiptDetails(string receiptNumber);

        /// <summary>
        /// Create receipt items for batch items for a particular batch record id
        /// </summary>
        /// <param name="batchRecordId"></param>
        void AddPAYEReceiptItems(long batchRecordId);

    }
}
