namespace Parkway.CBS.POSSAP.EGSRegularization.Implementations.Contracts
{
    public interface IGenerateRequestWithoutOfficersFileUpload
    {
        /// <summary>
        /// Process GenerateRequestWithoutOfficers line items for file upload as a background job.
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="tenantName"></param>
        void ProcessGenerateRequestWithoutOfficersFileUpload(long batchId, string tenantName);
    }
}
