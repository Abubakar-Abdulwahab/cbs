using System;
using Orchard;
using System.Collections.Generic;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IPAYEBatchRecordInvoiceManager<PAYEBatchRecordInvoice> : IDependency, IBaseManager<PAYEBatchRecordInvoice>
    {

        /// <summary>
        /// Get the batch record id using the invoice id
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns>List{PAYEBatchRecordVM}</returns>
        /// <exception cref="Exception"></exception>
        List<PAYEBatchRecordVM> GetBatchRecordsWithIncompletePayment(long invoiceId);

        // <summary>
        /// Get invoice number of unpaid invoice for batch with specified batch record id
        /// </summary>
        /// <param name="batchRecordId"></param>
        /// <returns></returns>
        IEnumerable<string> GetUnpaidInvoiceForBatchWithId(Int64 batchRecordId);

    }
}
