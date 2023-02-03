using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Entities.VMs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientRepository.Repositories.NAGIS.Contracts
{
    public interface INagisOldInvoiceCustomerResponseDAOManager : IRepository<NagisOldInvoiceCustomerResponse>
    {
        /// <summary>
        /// Save the results of the customer generation process.
        /// </summary>
        /// <param name="listOfProcessedInvoices"></param>
        /// <param name="batchId"></param>
        void SaveBundle(List<ConcurrentStack<NAGISGenerateCustomerResult>> listOfProcessedCustomers, long batchId);
    }
}
