using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.POSSAP.EGSRegularization.HelperModel;

namespace Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories.Contracts
{
    public interface IPSSBranchSubUsersBatchDAOManager : IRepository<PSSBranchSubUsersUploadBatchStaging>
    {
        /// <summary>
        /// Gets processing status and filepath of PSSBranchSubUsers upload batch with specified id.
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns>PSSBranchSubUsersBatchDetailsVM</returns>
        PSSBranchSubUsersBatchDetailsVM GetPSSBranchSubUsersUploadBatchStatusAndFilePath(long batchId);

        /// <summary>
        /// Update processing status for PSSBranchSubUsers upload with specified batch id using the specified processing status.
        /// </summary>
        /// <param name="processingStatus"></param>
        /// <param name="batchId"></param>
        /// <param name="errorMessage"></param>
        void UpdatePSSBranchSubUsersUploadBatchStatus(PSSBranchSubUserUploadStatus processingStatus, long batchId, string errorMessage = "");
    }
}
