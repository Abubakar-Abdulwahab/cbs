using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.DTO;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSSBranchOfficersUploadBatchItemsStagingManager<PSSBranchOfficersUploadBatchItemsStaging> : IDependency, IBaseManager<PSSBranchOfficersUploadBatchItemsStaging>
    {
        /// <summary>
        /// Gets PSS branch officers upload batch items in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        IEnumerable<PSSBranchOfficersUploadBatchItemsStagingDTO> GetItemsInBatchWithId(long batchId);
    }
}
