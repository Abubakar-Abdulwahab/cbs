using Orchard;
using System;
using System.Collections.Generic;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IStatsManager : IDependency
    {

        /// <summary>
        /// Get expected income on invoices that have not been paid between date range
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <returns>IEnumerable{DashboardStats}</returns>
        IEnumerable<DashboardStats> GetExpectedIncomeOnInvoices(DashboardStatsSearchParams searchParams, bool applyAccessRestrictions, DateTime startDate, DateTime endDate);


        /// <summary>
        /// Get the number of invoices generated in range
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns>IEnumerable{DashboardStats}</returns>
        IEnumerable<DashboardStats> GetNumberOfInvoicesGenerated(DashboardStatsSearchParams searchParams, bool applyAccessRestrictions, DateTime startDate, DateTime endDate);


        /// <summary>
        /// Get the summation of all invoice amounts generated for the given time frame
        /// and also the count of these invoices that were summed
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <returns>IEnumerable{DashboardStats}</returns>
        IEnumerable<DashboardStats> GetSumAndCountOfInvoicesGenerated(DashboardStatsSearchParams searchParams, bool applyAccessRestrictions, DateTime startDate, DateTime endDate);


        /// <summary>
        /// Get the income received in this date range
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <returns>IEnumerable{DashboardStats}</returns>
        IEnumerable<DashboardStats> GetIncomeReceivedAndCount(DashboardStatsSearchParams searchParams, bool applyAccessRestrictions, DateTime startDate, DateTime endDate);


        IList<StatsPerMonth> GetStats(DateTime? startDate, DateTime? endDate);


        IList<StatsPerMonth> GetStatsForRevenueHead(RevenueHead revenueHead, DateTime? startDate, DateTime? endDate);


        IList<StatsPerMonth> GetStatsPerQuarter(DateTime? startDate, DateTime? endDate);


        IList<StatsPerMonth> GetStatsForPieChart(DateTime startDate, DateTime endDate, string mdaSelected, MDA mda = null);
        
    }
}
