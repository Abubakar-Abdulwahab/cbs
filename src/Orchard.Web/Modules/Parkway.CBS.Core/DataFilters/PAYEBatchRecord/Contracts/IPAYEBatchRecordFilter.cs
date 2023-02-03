using Orchard;
using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.DataFilters.PAYEBatchRecord.Contracts
{
    public interface IPAYEBatchRecordFilter : IDependency
    {
        /// <summary>
        /// Get batch records
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        IEnumerable<PAYEBatchRecordVM> GetBatchRecords(PAYEBatchRecordSearchParams searchParams);

        /// <summary>
        /// Get batch records view model
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        dynamic GetBatchRecordsViewModel(PAYEBatchRecordSearchParams searchParams);

        /// <summary>
        /// Get aggregate of batch records
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        IEnumerable<ReportStatsVM> GetAggregate(PAYEBatchRecordSearchParams searchParams);
    }
}
