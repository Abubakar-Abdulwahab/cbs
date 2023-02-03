using System.Collections.Concurrent;
using System.Collections.Generic;
using Parkway.Cashflow.Ng.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Entities.VMs;

namespace Parkway.CBS.ClientRepository.Repositories.Contracts
{
    public interface IIPPISBatchRecordsInvoiceDAOManager : IRepository<IPPISBatchRecordsInvoice>
    {
        /// <summary>
        /// Save the results of the invoice generation process.
        /// The list contains the results of invoice generation models chunked up
        /// </summary>
        /// <param name="listOfProcessedInvoices"></param>
        /// <param name="batchId"></param>
        /// <param name="invoiceModel"></param>
        void SaveBundle(List<ConcurrentStack<IPPISGenerateInvoiceResult>> listOfProcessedInvoices, long batchId, string invoiceModel);
    }
}
