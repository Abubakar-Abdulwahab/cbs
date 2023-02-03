namespace Parkway.CBS.Services.PAYEBatchItems.Contracts
{
    public interface IPAYEBatchItemsValidatorJob
    {
        /// <summary>
        /// Queues up the validation of the items in hangfire
        /// </summary>
        /// <param name="batchStagingRecordId">Batch record Id for the collection of items to be validated</param>
        /// <param name="tenantName">Tenant name</param>
        void ValidateItemsByBatchRecordId(string tenantName, long batchStagingRecordId);
    }
}
