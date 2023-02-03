using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.HelperModels;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSSBranchOfficersUploadBatchStagingManager<PSSBranchOfficersUploadBatchStaging> : IDependency, IBaseManager<PSSBranchOfficersUploadBatchStaging>
    {

        /// <summary>
        /// Checks if the batch status is completed for the <paramref name="batchId"/> provided
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns><see cref="true"/> if the batch process is completed, otherwise, <see cref="false"/></returns>
        bool IsBatchProcessed(long batchId);

        /// <summary>
        /// Gets the tax entity profile location attached to batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        TaxEntityProfileLocationVM GetTaxEntityProfileLocationAttachedToBatchWithId(long batchId);

        /// <summary>
        /// Gets the batch related to the <paramref name="batchId"/>
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        PSSBranchOfficersUploadBatchStagingVM GetBatchByBatchId(long batchId);

        /// <summary>
        /// Updates batch with specified id setting HasGeneratedInvoice flag to true
        /// </summary>
        /// <param name="batchId"></param>
        void UpdateInvoiceGenerationStatusForBatchWithId(long batchId);
    }
}
