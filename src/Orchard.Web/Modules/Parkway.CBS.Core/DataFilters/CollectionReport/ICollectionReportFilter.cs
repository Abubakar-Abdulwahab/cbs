using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.DataFilters.CollectionReport
{
    public interface ICollectionReportFilter : IDependency
    {
        IEnumerable<TransactionLog> GetReport(CollectionSearchParams searchParams, bool applyAccessRestrictions);

        /// <summary>
        /// Get view model for collection report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>dynamic</returns>
        dynamic GetReportViewModel(CollectionSearchParams searchParams, bool applyAccessRestrictions);


        IEnumerable<CollectionReportStats> GetAggregate(CollectionSearchParams searchParams, bool applyAccessRestrictions);
        
    }
}
