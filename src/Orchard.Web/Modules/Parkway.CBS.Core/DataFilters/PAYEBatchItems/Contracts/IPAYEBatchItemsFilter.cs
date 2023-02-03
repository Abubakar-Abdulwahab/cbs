using Orchard;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Core.DataFilters.PAYEBatchItems.Contracts
{
    public interface IPAYEBatchItemsFilter : IDependency
    {
        /// <summary>
        /// Get PAYE Batch Items for batch using search params
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        IEnumerable<PAYEBatchItemsVM> GetPAYEBatchItemsForBatch(PAYEBatchItemSearchParams searchParams);

        /// <summary>
        /// Get aggegrate of batch items
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        IEnumerable<ReportStatsVM> GetAggregate(PAYEBatchItemSearchParams searchParams);

        /// <summary>
        /// Get batch items
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        dynamic GetBatchItems(PAYEBatchItemSearchParams searchParams);


    }
}
