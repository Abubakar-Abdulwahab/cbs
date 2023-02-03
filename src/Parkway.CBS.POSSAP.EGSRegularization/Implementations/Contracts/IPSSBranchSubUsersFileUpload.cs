
namespace Parkway.CBS.POSSAP.EGSRegularization.Implementations.Contracts
{
    public interface IPSSBranchSubUsersFileUpload
    {
        /// <summary>
        /// Process PSSBranchSubUsers line items for file upload as a background job.
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="tenantName"></param>
        void ProcessPSSBranchSubUsersFileUpload(long batchId, string tenantName);
    }
}
