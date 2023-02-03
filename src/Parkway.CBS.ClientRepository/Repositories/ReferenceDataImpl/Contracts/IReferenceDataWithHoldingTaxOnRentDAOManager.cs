using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientRepository.Repositories.ReferenceDataImpl.Contracts
{
    public interface IReferenceDataWithHoldingTaxOnRentDAOManager : IRepository<ReferenceDataWithHoldingTaxOnRent>
    {
        /// <summary>
        /// Get chunk of records for invoice generation
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="chunkSize"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        List<ReferenceDataGenerateInvoiceModel> GetChunkedBatch(long batchId, int chunkSize, int skip);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        List<ReferenceDataGenerateInvoiceModel> GetBatch(long batchId);


    }
}
