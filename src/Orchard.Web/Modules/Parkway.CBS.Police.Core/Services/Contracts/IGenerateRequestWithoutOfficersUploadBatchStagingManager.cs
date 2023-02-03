using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.DTO;
using System;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IGenerateRequestWithoutOfficersUploadBatchStagingManager<GenerateRequestWithoutOfficersUploadBatchStaging> : IDependency, IBaseManager<GenerateRequestWithoutOfficersUploadBatchStaging>
    {
        /// <summary>
        /// Gets GenerateRequestWithoutOfficersUploadBatch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        GenerateRequestWithoutOfficersUploadBatchStagingDTO GetGenerateRequestWithoutOfficersUploadBatchWithId(Int64 batchId);


        /// <summary>
        /// Gets GenerateRequestWithoutOfficersUploadBatch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        GenerateRequestWithoutOfficersUploadBatchStagingDTO GetGenerateRequestWithoutOfficersUploadBatchStatusInfoWithId(Int64 batchId);


        /// <summary>
        /// Gets tax entity profile location id and tax entity id for batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        GenerateRequestWithoutOfficersUploadBatchStagingDTO GetTaxEntityProfileLocationIdAndTaxEntityIdForBatch(Int64 batchId);


        /// <summary>
        /// Updates batch with specified id setting HasGeneratedInvoice flag to true and status to completed
        /// </summary>
        /// <param name="batchId"></param>
        void UpdateInvoiceGenerationStatusForBatchWithId(long batchId);
    }
}
