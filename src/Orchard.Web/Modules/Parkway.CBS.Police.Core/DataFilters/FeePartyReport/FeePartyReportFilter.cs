using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.DataFilters.FeePartyReport.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Parkway.CBS.Police.Core.DataFilters.SettlementFeePartyReport
{
    public class FeePartyReportFilter : IFeePartyReportFilter
    {
        private readonly ITransactionManager _transactionManager;

        public FeePartyReportFilter(IOrchardServices orchardService)
        {
            _transactionManager = orchardService.TransactionManager;
        }

        /// <summary>
        /// Get view model for settlement fee party configurations reports
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, TotalNumberOfSettlementFeePartyConfiguration }</returns>
        public dynamic GetFeePartyReportViewModel(FeePartyReportSearchParams searchParams)
        {
            dynamic returnOBJ = new ExpandoObject();

            try
            {
                returnOBJ.ReportRecords = GetReport(searchParams);
                returnOBJ.TotalNumberOfSettlementFeePartyConfiguration = GetTotalNumberOfActiveFeePartyConfiguration(searchParams);
                return returnOBJ;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get the list of settlement fee party configurations
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns><see cref="IEnumerable{SettlementFeePartyVM}"/></returns>
        private IEnumerable<SettlementFeePartyVM> GetReport(FeePartyReportSearchParams searchParams)
        {
            var query = GetCriteria(searchParams);

            if (!searchParams.DontPageData)
            {
                query
                .SetFirstResult(searchParams.Skip)
                .SetMaxResults(searchParams.Take);
            }

            return query
                .Add(Restrictions.Eq(nameof(PSSFeeParty.IsActive), true))
                .AddOrder(Order.Desc("UpdatedAtUtc"))
                .SetProjection(Projections.ProjectionList()
                .Add(Projections.Property(nameof(PSSFeeParty.AccountNumber)), nameof(SettlementFeePartyVM.AccountNumber))
                .Add(Projections.Property(nameof(PSSFeeParty.Name)), nameof(SettlementFeePartyVM.Name))
                .Add(Projections.Property($"{nameof(PSSFeeParty.Bank)}.{nameof(PSSFeeParty.Bank.Code)}"), nameof(SettlementFeePartyVM.BankCode)))
                .SetResultTransformer(Transformers.AliasToBean<SettlementFeePartyVM>())
                .Future<SettlementFeePartyVM>();
        }

        public ICriteria GetCriteria(FeePartyReportSearchParams searchParams)
        {
            ISession session = _transactionManager.GetSession();
            return session.CreateCriteria<PSSFeeParty>(nameof(PSSFeeParty)).CreateAlias($"{nameof(PSSFeeParty)}.{nameof(PSSFeeParty.Bank)}", $"{nameof(PSSFeeParty.Bank)}", NHibernate.SqlCommand.JoinType.LeftOuterJoin);
        }

        /// <summary>
        /// Get total number of active settlementfeePartyConfiguration
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        private IEnumerable<ReportStatsVM> GetTotalNumberOfActiveFeePartyConfiguration(FeePartyReportSearchParams searchParams)
        {
            var query = GetCriteria(searchParams);

            return query
                .Add(Restrictions.Eq(nameof(PSSFeeParty.IsActive), true))
                .SetProjection(
               Projections.ProjectionList()
                    .Add(Projections.CountDistinct<PSSFeeParty>(x => x.Id), nameof(ReportStatsVM.TotalRecordCount))
               ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }
    }
}