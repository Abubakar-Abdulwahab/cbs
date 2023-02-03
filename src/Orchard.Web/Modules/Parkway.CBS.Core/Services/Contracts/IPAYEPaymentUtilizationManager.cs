using Orchard;
using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IPAYEPaymentUtilizationManager<PAYEPaymentUtilization> : IDependency, IBaseManager<PAYEPaymentUtilization>
    {
        /// <summary>
        /// Get the total amount paid for a batch using the batch record id
        /// </summary>
        /// <param name="batchRecordId"></param>
        /// <returns>decimal</returns>
        decimal GetBatchRecordAmountPaid(long batchRecordId);

        /// <summary>
        /// Get receipts utilized for the schedule with the specified batch record Id
        /// </summary>
        /// <param name="batchRecordId"></param>
        /// <returns></returns>
        IEnumerable<PAYEReceiptVM> GetUtilizedReceiptsForBatchRecord(long batchRecordId);

        /// <summary>
        /// Get receipts utilized for the schedule with the specified batch ref
        /// </summary>
        /// <param name="batchRef"></param>
        /// <returns></returns>
        IEnumerable<PAYEReceiptVM> GetUtilizedReceiptsForBatchRecord(string batchRef);
    }
}
