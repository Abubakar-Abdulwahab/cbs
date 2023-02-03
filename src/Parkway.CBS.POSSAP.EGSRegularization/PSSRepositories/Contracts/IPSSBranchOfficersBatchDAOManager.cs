using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.POSSAP.EGSRegularization.HelperModel;

namespace Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories.Contracts
{
    public interface IPSSBranchOfficersBatchDAOManager : IRepository<PSSBranchOfficersUploadBatchStaging>
    {
        /// <summary>
        /// Gets processing status and filepath of PSSBranchOfficers upload batch with specified id.
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns>PSSBranchOfficersBatchDetailsVM</returns>
        PSSBranchOfficersBatchDetailsVM GetPSSBranchOfficersUploadBatchStatusAndFilePath(long batchId);

        /// <summary>
        /// Update processing status for PSSBranchOfficers upload with specified batch id using the specified processing status.
        /// </summary>
        /// <param name="batchId"></param>
        void UpdatePSSBranchOfficersUploadBatchStatus(long batchId, string errorMessage);

        /// <summary>
        /// Update processing status for PSSBranchOfficers upload with specified batch id using the specified processing status.
        /// </summary>
        /// <param name="processingStatus"></param>
        /// <param name="batchId"></param>
        /// <param name="errorMessage"></param>
        void UpdatePSSBranchOfficersUploadBatchStatus(PSSBranchOfficersUploadStatus processingStatus, long batchId, string errorMessage = "");
    }
}
