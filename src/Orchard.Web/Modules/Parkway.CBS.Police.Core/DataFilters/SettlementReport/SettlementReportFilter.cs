using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.DataFilters.SettlementReport.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Parkway.CBS.Police.Core.DataFilters.SettlementReport
{
    public class SettlementReportFilter : ISettlementReportFilter
    {
        private readonly ITransactionManager _transactionManager;
        public SettlementReportFilter(IOrchardServices orchardService)
        {
            _transactionManager = orchardService.TransactionManager;
        }

        /// <summary>
        /// Get view model for settlement reports
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, TotalNumberOfActiveSettlement }</returns>
        public dynamic GetSettlementReportViewModel(SettlementReportSearchParams searchParams)
        {
            dynamic returnOBJ = new ExpandoObject();

            try
            {
                returnOBJ.ReportRecords = GetReport(searchParams);
                returnOBJ.TotalNumberOfActiveSettlements = GetTotalNumberOfActiveSettlements(searchParams);
                return returnOBJ;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private IEnumerable<PSSSettlementVM> GetReport(SettlementReportSearchParams searchParams)
        {
            var query = GetCriteria(searchParams);

            if (!searchParams.DontPageData)
            {
                query
                .SetFirstResult(searchParams.Skip)
                .SetMaxResults(searchParams.Take);
            }

            return query
                .AddOrder(Order.Desc(nameof(PSSSettlement.UpdatedAtUtc)))
                .Add(Restrictions.Eq(nameof(PSSSettlement.IsActive), true))
                .SetProjection(Projections.ProjectionList()
                .Add(Projections.Property(nameof(PSSSettlement.Name)), nameof(PSSSettlementVM.Name))
                .Add(Projections.Property(nameof(PSSSettlement.Id)), nameof(PSSSettlementVM.Id))
                .Add(Projections.Property(nameof(PSSSettlement.IsActive)), nameof(PSSSettlementVM.IsActive)))
                .SetResultTransformer(Transformers.AliasToBean<PSSSettlementVM>())
                .Future<PSSSettlementVM>();
        }

        /// <summary>
        /// Get the total number of active settlements
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        private IEnumerable<ReportStatsVM> GetTotalNumberOfActiveSettlements(SettlementReportSearchParams searchParams)
        {
            var query = GetCriteria(searchParams);

            return query
                .Add(Restrictions.Eq(nameof(PSSSettlement.IsActive), true))
                .SetProjection(
               Projections.ProjectionList()
                    .Add(Projections.CountDistinct<PSSSettlement>(x => x.Id), nameof(ReportStatsVM.TotalRecordCount))
               ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }


        public ICriteria GetCriteria(SettlementReportSearchParams searchParams)
        {
            var session = _transactionManager.GetSession();
            var criteria = session.CreateCriteria<PSSSettlement>(nameof(PSSSettlement));

            return criteria;
        }
    }
}