using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.DTO;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSSBranchSubUsersUploadBatchItemsStagingManager<PSSBranchSubUsersUploadBatchItemsStaging> : IDependency, IBaseManager<PSSBranchSubUsersUploadBatchItemsStaging>
    {
        /// <summary>
        /// Creates tax entity profile locations for all branches in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        void CreateBranches(long batchId);

        /// <summary>
        /// Resolves Tax Entity Profile Location Ids for branches that have been created in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        void ResolveTaxEntityProfileLocationIdsForCreatedBranches(long batchId);

        /// <summary>
        /// Creates cbs users for all sub users in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        void CreateSubUsersAsCBSUsers(long batchId);

        /// <summary>
        /// Attaches sub users to their respective branches in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        void AttachSubUsersToBranchLocations(long batchId);

        /// <summary>
        /// Gets PSS Branch Sub Users Upload Batch Items for batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        IEnumerable<PSSBranchSubUsersUploadBatchItemsStagingDTO> GetItems(long batchId, int skip, int take);

        /// <summary>
        /// Performs a bulk update on PSSBranchSubUsersUploadBatchItemsStaging, updating the user id for sub users in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        void UpdateUserIdsForSubUsersInBatchWithId(long batchId);
    }
}
