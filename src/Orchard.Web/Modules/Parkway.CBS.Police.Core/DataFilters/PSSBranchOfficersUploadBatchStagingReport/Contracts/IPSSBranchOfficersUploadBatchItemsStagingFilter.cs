﻿using Orchard;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Police.Core.DataFilters.PSSBranchOfficersUploadBatchStagingReport.Contracts
{
    public interface IPSSBranchOfficersUploadBatchItemsStagingFilter : IDependency
    {
        /// <summary>
        /// Get view model for branch officer report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, TotalRecordCount }</returns>
        dynamic GetReportViewModel(PSSBranchOfficersUploadBatchItemsStagingReportSearchParams searchParams);
    }
}
