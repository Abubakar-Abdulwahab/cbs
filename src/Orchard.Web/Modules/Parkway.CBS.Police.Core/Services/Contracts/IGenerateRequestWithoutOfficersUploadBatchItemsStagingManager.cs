using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.DTO;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IGenerateRequestWithoutOfficersUploadBatchItemsStagingManager<GenerateRequestWithoutOfficersUploadBatchItemsStaging> : IDependency, IBaseManager<GenerateRequestWithoutOfficersUploadBatchItemsStaging>
    {
        /// <summary>
        /// Gets items in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        IEnumerable<GenerateRequestWithoutOfficersUploadBatchItemsStagingDTO> GetItems(long batchId);


        /// <summary>
        /// Get total number of requested officers in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        int GetTotalNumberOfRequestedOfficersInBatch(long batchId);
    }
}
