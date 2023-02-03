using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.DataFilters.PSSSettlementReportAggregate.Contracts;
using Parkway.CBS.Police.Core.DataFilters.PSSSettlementReportAggregate.SearchFilters;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Parkway.CBS.Police.Core.DataFilters.PSSSettlementReportAggregate
{
    public class PSSSettlementReportAggregateFilter : IPSSSettlementReportAggregateFilter
    {
        private readonly ITransactionManager _transactionManager;
        private readonly IEnumerable<IPSSSettlementReportAggregateFilters> _searchFilters;

        public PSSSettlementReportAggregateFilter(IOrchardServices orchardService, IEnumerable<IPSSSettlementReportAggregateFilters> searchFilters)
        {
            _transactionManager = orchardService.TransactionManager;
            _searchFilters = searchFilters;
        }


        /// <summary>
        /// Get veiw model for settlement report aggregate
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, TotalNumberOfSettlements }</returns>
        public dynamic GetRequestReportViewModel(PSSSettlementReportAggregateSearchParams searchParams)
        {
            dynamic returnOBJ = new ExpandoObject();

            try
            {
                returnOBJ.ReportRecords = GetReport(searchParams);
                returnOBJ.TotalNumberOfSettlements = GetTotalNumberOfSettlements(searchParams);
                return returnOBJ;
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// Get the aggregate of the settlements
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <returns></returns>
        private IEnumerable<ReportStatsVM> GetTotalNumberOfSettlements(PSSSettlementReportAggregateSearchParams searchParams)
        {
            var query = GetCriteria(searchParams);

            return query
                .SetProjection(
               Projections.ProjectionList()
                    .Add(Projections.Count<PSSSettlementBatch>(x => x.Id), "TotalRecordCount")
               ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }


        /// <summary>
        /// Get settlement batch aggregate report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <returns>IEnumerable{PSSSettlementBatchVM}</returns>
        private IEnumerable<PSSSettlementBatchVM> GetReport(PSSSettlementReportAggregateSearchParams searchParams)
        {
            var query = GetCriteria(searchParams);

            if (searchParams.PageData)
            {
                query
                .SetFirstResult(searchParams.Skip)
                .SetMaxResults(searchParams.Take);
            }

            return query
                .AddOrder(Order.Desc("Id"))
                .SetProjection(Projections.ProjectionList()
                .Add(Projections.Property($"{nameof(PSSSettlementBatch.PSSSettlement)}.{nameof(PSSSettlement.Name)}"), nameof(PSSSettlementBatchVM.SettlementName))
                .Add(Projections.Property(nameof(PSSSettlementBatch.SettlementDate)), nameof(PSSSettlementBatchVM.TransactionDate))
                .Add(Projections.Property(nameof(PSSSettlementBatch.SettlementAmount)), nameof(PSSSettlementBatchVM.SettlementAmount))
                .Add(Projections.Property(nameof(PSSSettlementBatch.BatchRef)), nameof(PSSSettlementBatchVM.SettlementBatchRef))
                .Add(Projections.Property(nameof(PSSSettlementBatch.StatusMessage)), nameof(PSSSettlementBatchVM.StatusMessage))
                ).SetResultTransformer(Transformers.AliasToBean<PSSSettlementBatchVM>())
                .Future<PSSSettlementBatchVM>();
        }


        public ICriteria GetCriteria(PSSSettlementReportAggregateSearchParams searchParams)
        {
            var session = _transactionManager.GetSession();
            var criteria = session.CreateCriteria<PSSSettlementBatch>(nameof(PSSSettlementBatch)).CreateAlias(nameof(PSSSettlementBatch.PSSSettlement), "PSSSettlement");

            criteria
                .Add(Restrictions.Between(nameof(PSSSettlementBatch.CreatedAtUtc), searchParams.StartDate, searchParams.EndDate));

            foreach (var filter in _searchFilters)
            {
                filter.AddCriteriaRestriction(criteria, searchParams);
            }

            return criteria;
        }
    }
}