using Orchard;
using Parkway.CBS.Police.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Police.Core.DataFilters.GenerateRequestWithoutOfficersUploadBatchStagingReport.Contracts
{
    public interface IGenerateRequestWithoutOfficersUploadBatchStagingFilter : IDependency
    {
        /// <summary>
        /// Get Generate Request Without Officers Upload Batch Staging report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, Aggregate }</returns>
        dynamic GetReportViewModel(GenerateRequestWithoutOfficersUploadBatchStagingReportSearchParams searchParams);
    }
}
