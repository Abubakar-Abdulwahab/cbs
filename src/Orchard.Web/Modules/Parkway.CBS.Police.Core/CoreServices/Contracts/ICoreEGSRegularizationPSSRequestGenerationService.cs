using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.CoreServices.Contracts
{
    public interface ICoreEGSRegularizationPSSRequestGenerationService : IDependency
    {
        /// <summary>
        /// Generates a request using details in <paramref name="requestVM"/> for the configured duration and tax entity profile location <paramref name="taxEntityProfileLocationId"/>
        /// using officers in batch with id <paramref name="PSSBranchOfficersUploadBatchStagingId"/>
        /// </summary>
        /// <param name="requestVM"></param>
        /// <param name="taxEntityProfileLocationId"></param>
        /// <param name="PSSBranchOfficersUploadBatchStagingId"></param>
        /// <returns></returns>
        InvoiceGenerationResponse GenerateTimeSpecificRequest(EscortRequestVM requestVM, int taxEntityProfileLocationId, long PSSBranchOfficersUploadBatchStagingId);
    }
}
