using Orchard;
using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.DataFilters.PAYEBatchItemReceiptReport.Contracts
{
    public interface IPAYEBatchItemReceiptFilter : IDependency
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>IEnumerable<PAYEReceiptItems></returns>
        IEnumerable<PAYEReceiptItems> GetReceipts(PAYEReceiptSearchParams searchParams);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        dynamic GetReceiptViewModel(PAYEReceiptSearchParams searchParams);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>IEnumerable<ReportStatsVM></returns>
        IEnumerable<ReportStatsVM> GetAggregate(PAYEReceiptSearchParams searchParams);
    }
}
