
namespace Parkway.CBS.POSSAP.EGSRegularization.Implementations.Contracts
{
    public interface IPSSBranchOfficersFileUpload
    {
        /// <summary>
        /// Process PSSBranchOfficers line items for file upload as a background job.
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="tenantName"></param>
        void ProcessPSSBranchOfficersFileUpload(long batchId, string tenantName);
    }
}
