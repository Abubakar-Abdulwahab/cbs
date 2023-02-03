using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.FileUpload.NAGISImplementation.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientRepository.Repositories.NAGIS.Contracts
{
    public interface INagisOldInvoicesDAOManager : IRepository<NagisOldInvoices>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="recordId"></param>
        /// <param name="nagisDataLineRecords"></param>
        /// <returns></returns>
        int SaveRecords(string tenantName, long recordId, ConcurrentStack<NAGISDataLineRecordModel> nagisDataLineRecords);


        /// Update the Tax Entity Staging records operationTypeId where the Tax Entity Category, PhoneNumber, customername and address matches what is in the NAGIS Old Invoices records.
        /// </summary>
        /// <param name="batchId"></param>
        void UpdateNagisOldInvoicesStagingRecordsOperationType(long batchId);

        /// <summary>
        /// Create the Tax Entity records where the Tax Entity Category, PhoneNumber, customername and address does not match what is in the NAGIS Old Invoices records.
        /// </summary>
        /// <param name="batchId"></param>
        void CreateTaxEntityWithNAGISDataRecords(long batchId);

        /// <summary>
        /// Update the Tax Entity records where the Tax Entity Category, PhoneNumber, customername and address matches what is in the NAGIS records.
        /// </summary>
        /// <param name="batchId"></param>
        void UpdateTaxEntityWithNAGISDataRecords(long batchId);

        /// <summary>
        /// Update the Tax Entity Id in NAGIS Old Invoices table where the Tax Entity Category, PhoneNumber, customername and address matches what is in the Tax Entity Data records.
        /// </summary>
        /// <param name="batchId"></param>
        void UpdateNAGISDataStagingRecordsTaxEntityId(long batchId);

        /// <summary>
        /// This method categorizes the records store into distinct NAGIS Invoice Number and with Amount Due greater than zero and save in NAGISOldInvoiceSummary table
        /// </summary>
        /// <param name="batchId"></param>
        void GroupRecordsFromNAGISOldInvoicesRecordsTableByNAGISInvoiceNumber(long batchId);

        /// <summary>
        /// Update the NagisOldInvoiceSummary Id in NAGIS Old Invoices table where the NagisInvoiceNumber matches what is in the NagisOldInvoiceSummary Data records.
        /// </summary>
        /// <param name="batchId"></param>
        void UpdateNAGISDataStagingRecordsNagisOldInvoiceSummaryId(long batchId);

        /// <summary>
        /// Create the Tax Entity records where the Tax Entity Category, PhoneNumber, customername and address does not match what is in the NAGIS Old Invoices records and output the Ids into Staging Helper table.
        /// </summary>
        /// <param name="batchId"></param>
        void CreateTaxEntityWithNAGISDataRecordsUsingStagingHelper(long batchId);

    }
}
