using Parkway.Cashflow.Ng.Models;
using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Entities.VMs;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Parkway.CBS.ClientRepository.Repositories.ReferenceDataImpl.Contracts
{
    public interface IBatchInvoiceResponseDAOManager : IRepository<BatchInvoiceResponse>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listOfProcessedInvoices"></param>
        /// <param name="batchId"></param>
        void SaveBundle(List<CashFlowCreateCustomerAndInvoiceResponse> listOfProcessedInvoices, string batchId);

        /// <summary>
        /// Save the results of the invoice generation process.
        /// The list contains the results of invoice generation models chunked up
        /// </summary>
        /// <param name="listOfProcessedInvoices"></param>
        /// <param name="batchId"></param>
        void SaveBundle(List<ConcurrentStack<NAGISGenerateInvoiceResult>> listOfProcessedInvoices, long batchId);
    }
}
