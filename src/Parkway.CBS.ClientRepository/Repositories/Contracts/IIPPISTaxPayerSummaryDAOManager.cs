using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Entities.DTO;
using System.Collections.Generic;

namespace Parkway.CBS.ClientRepository.Repositories.Contracts
{
    public interface IIPPISTaxPayerSummaryDAOManager : IRepository<IPPISTaxPayerSummary>
    {
        /// <summary>
        /// Get the records from IPPISTaxPayerSummary that need invoices to be generated for them 
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns>List{IPPISGenerateInvoiceModel}</returns>
        List<IPPISGenerateInvoiceModel> GetChunkedBatch(long batchId, int chunkSize, int skip);
    }
}
