using Orchard;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IPAYEBatchRecordStagingManager<PAYEBatchRecordStaging> : IDependency, IBaseManager<PAYEBatchRecordStaging>
    {

        /// <summary>
        /// Get PAYEBatchRecordStaging with Id and taxEntityId
        /// </summary>
        /// <param name="id"></param>
        /// <param name="taxEntityId"></param>
        /// <returns>IEnumerable{PAYEBatchRecordStaging}</returns>
        IEnumerable<PAYEBatchRecordStaging> GetBatchStagingId(long id, long taxEntityId);


        /// <summary>
        /// Move PAYE staging to main table
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="invoiceId"></param>
        /// <param name="surcharge">Surcharge for this assessment</param>
        /// <returns>string | this method returns the batch ref of the batch record</returns>
        string InvoiceConfirmedMovePAYE(long batchId, long invoiceId, decimal surcharge);


        /// <summary>
        /// Attach batch record with specified batchRecordId to invoice with specified invoiceId. This method should be used when generating an invoice for an existing batch.
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="invoiceId"></param>
        void InvoiceConfirmedAttachExisitngBatch(long batchRecordId, long invoiceId);


        /// <summary>
        /// Get VM for Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>PAYEBatchRecordStagingVM</returns>
        PAYEBatchRecordStagingVM GetVM(long id);

    }

}
