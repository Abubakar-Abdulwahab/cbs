using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.DataFilters.SettlementFeePartiesReport.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Parkway.CBS.Police.Core.DataFilters.SettlementFeePartiesReport
{
    public class SettlementFeePartiesReportFilter : ISettlementFeePartiesReportFilter
    {
        private readonly ITransactionManager _transactionManager;
        public SettlementFeePartiesReportFilter(IOrchardServices orchardService)
        {
            _transactionManager = orchardService.TransactionManager;
        }

        /// <summary>
        /// Get view model for settlement reports
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, TotalNumberOfActiveSettlementFeeParties }</returns>
        public dynamic GetSettlementReportViewModel(SettlementFeePartiesSearchParams searchParams)
        {
            dynamic returnOBJ = new ExpandoObject();

            try
            {
                returnOBJ.ReportRecords = GetReport(searchParams);
                returnOBJ.TotalNumberOfActiveSettlementFeeParties = GetTotalNumberOfActiveSettlementFeeParties(searchParams);
                return returnOBJ;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private IEnumerable<PSSSettlementFeePartyVM> GetReport(SettlementFeePartiesSearchParams searchParams)
        {
            var query = GetCriteria(searchParams);

            if (!searchParams.DontPageData)
            {
                query
                .SetFirstResult(searchParams.Skip)
                .SetMaxResults(searchParams.Take);
            }

            return query
                .Add(Restrictions.Eq(nameof(PSSSettlementFeeParty.IsActive), true))
                .Add(Restrictions.Eq(nameof(PSSSettlementFeeParty.IsDeleted), false))
                .AddOrder(Order.Desc(nameof(PSSSettlementFeeParty.DeductionValue)))
                .SetProjection(Projections.ProjectionList()
                .Add(Projections.Property($"{nameof(PSSSettlementFeeParty.FeeParty)}.{nameof(PSSSettlementFeeParty.FeeParty.Name)}"), nameof(PSSSettlementFeePartyVM.SettlementFeePartyName))
                .Add(Projections.Property($"{nameof(PSSSettlementFeeParty.FeeParty)}.{nameof(PSSSettlementFeeParty.FeeParty.Id)}"), nameof(PSSSettlementFeePartyVM.FeePartyId))
                .Add(Projections.Property(nameof(PSSSettlementFeeParty.DeductionValue)), nameof(PSSSettlementFeePartyVM.DeductionValue))
                .Add(Projections.Property(nameof(PSSSettlementFeeParty.AdditionalSplitValue)), nameof(PSSSettlementFeePartyVM.AdapterName)))
                .SetResultTransformer(Transformers.AliasToBean<PSSSettlementFeePartyVM>())
                .Future<PSSSettlementFeePartyVM>();
        }

        /// <summary>
        /// Get the total number of active settlements
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        private IEnumerable<ReportStatsVM> GetTotalNumberOfActiveSettlementFeeParties(SettlementFeePartiesSearchParams searchParams)
        {
            var query = GetCriteria(searchParams);

            return query
                .Add(Restrictions.Eq(nameof(PSSSettlementFeeParty.IsActive), true))
                .Add(Restrictions.Eq(nameof(PSSSettlementFeeParty.IsDeleted), false))
                .SetProjection(
               Projections.ProjectionList()
                    .Add(Projections.CountDistinct<PSSSettlementFeeParty>(x => x.Id), nameof(ReportStatsVM.TotalRecordCount))
               ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }


        public ICriteria GetCriteria(SettlementFeePartiesSearchParams searchParams)
        {
            var session = _transactionManager.GetSession();
            var criteria = session.CreateCriteria<PSSSettlementFeeParty>(nameof(PSSSettlementFeeParty)).CreateAlias($"{nameof(PSSSettlementFeeParty)}.{nameof(PSSSettlementFeeParty.Settlement)}", nameof(PSSSettlementFeeParty.Settlement)).CreateAlias($"{nameof(PSSSettlementFeeParty)}.{nameof(PSSSettlementFeeParty.FeeParty)}", nameof(PSSSettlementFeeParty.FeeParty)).Add(Restrictions.Eq($"{nameof(PSSSettlementFeeParty.Settlement)}.{nameof(PSSSettlementFeeParty.Settlement.Id)}", searchParams.SettlementId));


            return criteria;
        }
    }
}