using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.DataFilters.TaxPayerReport
{
    /// <summary>
    /// Interface filter for tax profile reporting
    /// </summary>
    public interface ITaxPayerReportFilter : IDependency
    {
        /// <summary>
        /// Get tax profile report based on skip, take and search parameters provided.
        /// </summary>
        /// <param name="searchModel">TaxProfilesSearchParams</param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        IEnumerable<TaxEntity> GetReportForTaxProfiles(TaxProfilesSearchParams model, int skip, int take);

        /// <summary>
        /// Get the aggregate of tax profile that fall fall under the search properties
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns>IEnumerable{TaxProfileReportStats}</returns>
        IEnumerable<TaxProfileReportStats> GetAggregateForTaxProfiles(TaxProfilesSearchParams searchModel);


        /// <summary>
        /// Get tax report for statement of account
        /// </summary>
        /// <param name="taxPayerIdNumber"></param>
        /// <param name="fromRange"></param>
        /// <param name="endRange"></param>
        /// <param name="paymentTypeId"></param>
        /// <returns>AccountStatementModel</returns>
        AccountStatementModel GetReportForStatementOfAccount(Int64 taxPayerIdNumber, DateTime fromRange, DateTime endRange, int paymentType);

        /// <summary>
        /// Get aggregate of TotalCreditAmount and TotalBillAmount
        /// </summary>
        /// <param name="fromRange"></param>
        /// <param name="endRange"></param>
        /// <param name="paymentTypeId"></param>
        /// <param name="taxPayerIdNumber"></param>
        /// <returns>AccountStatementAggregate</returns>
        AccountStatementAggregate GetAggregateForStatementOfAccount(DateTime fromRange, DateTime endRange, int paymentTypeId, Int64 taxPayerIdNumber);
    }
}
