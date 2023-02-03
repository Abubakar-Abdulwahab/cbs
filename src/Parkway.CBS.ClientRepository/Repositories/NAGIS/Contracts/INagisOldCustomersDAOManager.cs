using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.ClientRepository.Repositories.NAGIS.Models;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientRepository.Repositories.NAGIS.Contracts
{
    public interface INagisOldCustomersDAOManager : IRepository<NagisOldCustomers>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="chunkSize"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        List<NAGISDataGenerateInvoiceModel> GetChunkedBatchCustomer(long batchId, int chunkSize, int skip);

        /// <summary>
        /// Create the Tax Entity records where the Tax Entity Category, PhoneNumber, customername and address does not match what is in the NAGIS Old Invoices records.
        /// </summary>
        /// <param name="batchId"></param>
        void CreateNAGISCustomers(long batchId);

        /// <summary>
        /// Update the Tax Entity Id in NAGIS Customer table where the customerId and Id matches what is in the NagisOldInvoiceStagingHelper records.
        /// </summary>
        /// <param name="batchId"></param>
        void UpdateNAGISCustomerRecordsTaxEntityId(long batchId);

    }
}
