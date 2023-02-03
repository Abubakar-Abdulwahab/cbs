using Orchard;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.PSSBranchSubUsersUpload.Contracts
{
    public interface IPSSBranchSubUsersUploadBatchItemsStagingFilter : IDependency
    {
        /// <summary>
        /// Get veiw model for PSS Branch Sub Users Upload Batch Items
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, Aggregate, ValidItemsAggregate }</returns>
        dynamic GetReportViewModel(PSSBranchSubUsersUploadBatchItemsSearchParams searchParams);
    }
}
