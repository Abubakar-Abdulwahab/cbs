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
    public interface INagisOldInvoiceSummaryDAOManager : IRepository<NagisOldInvoiceSummary>
    {
        /// <summary>
        /// Get chunk of records for invoice generation
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="chunkSize"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        List<NAGISDataGenerateInvoiceModel> GetChunkedBatch(long batchId, int chunkSize, int skip);


        /// <summary>
        /// Get all records for invoice generation
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        List<NAGISDataGenerateInvoiceModel> GetBatch(long batchId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchId"></param>
        void UpdateInvoiceStagingWithCashFlowResponse(long batchId);

    }
}
