using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.ClientRepository.Repositories.Contracts
{
    public interface IIPPISBatchDAOManager : IRepository<IPPISBatch>
    {
        /// <summary>
        /// Get the eneity if any that has been processed for the given month and year
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns>IPPISBatch</returns>
        IPPISBatch GetRecordForMonthAndYear(int month, int year);


        /// <summary>
        /// Get batch record for this Id
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns>IPPISBatch</returns>
        IPPISBatch GetBatchRecord(long batchId);
    }
}
