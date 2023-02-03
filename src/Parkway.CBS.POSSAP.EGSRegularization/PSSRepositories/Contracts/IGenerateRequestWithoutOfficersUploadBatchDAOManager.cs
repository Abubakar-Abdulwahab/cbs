using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.POSSAP.EGSRegularization.HelperModels;

namespace Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories.Contracts
{
    public interface IGenerateRequestWithoutOfficersUploadBatchDAOManager : IRepository<GenerateRequestWithoutOfficersUploadBatchStaging>
    {
        /// <summary>
        /// Gets processing status and filepath of GenerateRequestWithoutOfficers upload batch with specified id.
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns>GenerateRequestWithoutOfficersUploadBatchDetailsVM</returns>
        GenerateRequestWithoutOfficersUploadBatchDetailsVM GetGenerateRequestWithoutOfficersUploadBatchStatusAndFilePath(long batchId);

        /// <summary>
        /// Update processing status for GenerateRequestWithoutOfficers upload with specified batch id using the specified processing status.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="batchId"></param>
        /// <param name="errorMessage"></param>
        void UpdateGenerateRequestWithoutOfficersUploadBatchStatus(GenerateRequestWithoutOfficersUploadStatus status, long batchId, string errorMessage = "");
    }
}
