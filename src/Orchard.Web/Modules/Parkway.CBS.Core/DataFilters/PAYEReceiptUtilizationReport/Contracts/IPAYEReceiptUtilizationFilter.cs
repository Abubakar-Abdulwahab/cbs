using Orchard;
using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.DataFilters.PAYEReceiptUtilizationReport.Contracts
{
    public interface IPAYEReceiptUtilizationFilter : IDependency
    {
        /// <summary>
        /// Get PAYE receipts list
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>IEnumerable<PAYEReceiptVM></returns>
        IEnumerable<PAYEReceiptVM> GetReceipts(PAYEReceiptSearchParams searchParams);

        /// <summary>
        /// Get PAYE receipts and report aggregate object
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>dynamic</returns>
        dynamic GetReceiptViewModel(PAYEReceiptSearchParams searchParams);

        /// <summary>
        /// Get aggregate report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>IEnumerable<ReportStatsVM></returns>
        IEnumerable<ReportStatsVM> GetAggregate(PAYEReceiptSearchParams searchParams);

        /// <summary>
        /// Gets receipt utilizations report for a specified receipt number
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <returns>IEnumerable<PAYEReceiptUtilizationReportVM></returns>
        IEnumerable<PAYEReceiptUtilizationReportVM> GetReceiptUtilizations(string receiptNumber);
    }
}
