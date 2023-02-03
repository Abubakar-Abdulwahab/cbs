using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Entities.VMs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientRepository.Repositories.ReferenceDataImpl.Contracts
{
    public interface IReferenceDataRecordsInvoiceDAOManager : IRepository<ReferenceDataRecordsInvoice>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listOfProcessedInvoices"></param>
        /// <param name="batchId"></param>
        /// <param name="invoiceModel"></param>
        void SaveBundle(List<ConcurrentStack<Entities.VMs.CashFlowBatchInvoiceResponse>> listOfProcessedInvoices, long batchId, string invoiceModel);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchId"></param>
        void UpdateInvoiceStagingWithCashFlowResponse(long batchId);
    }
}
