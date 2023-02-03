using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.DTO;
using System;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSSBranchSubUsersUploadBatchStagingManager<PSSBranchSubUsersUploadBatchStaging> : IDependency, IBaseManager<PSSBranchSubUsersUploadBatchStaging>
    {
        /// <summary>
        /// Gets branch sub users batch with specified batch Id
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        PSSBranchSubUsersUploadBatchStagingDTO GetBranchSubUsersBatchWithId(Int64 batchId);

        /// <summary>
        /// Updates status for batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="status"></param>
        void UpdateStatusForBatchWithId(long batchId, Models.Enums.PSSBranchSubUserUploadStatus status);

        /// <summary>
        /// Gets payer id of tax entity attached to batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns>payer id of tax entity attached to the batch</returns>
        string GetPayerIdForBatchTaxEntity(long batchId);
    }
}
