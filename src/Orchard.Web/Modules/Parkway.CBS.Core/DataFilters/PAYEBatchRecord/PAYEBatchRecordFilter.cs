using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Parkway.CBS.Core.DataFilters.PAYEBatchRecord.Contracts;
using Parkway.CBS.Core.DataFilters.PAYEBatchRecord.SearchFilters.Contracts;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;
using System.Dynamic;

namespace Parkway.CBS.Core.DataFilters.PAYEBatchRecord
{
    public class PAYEBatchRecordFilter : IPAYEBatchRecordFilter
    {
        private readonly ITransactionManager _transactionManager;
        protected readonly IEnumerable<IPAYEBatchRecordSearchFilter> _searchFilters;

        public PAYEBatchRecordFilter(IOrchardServices orchardService, IEnumerable<IPAYEBatchRecordSearchFilter> searchFilters)
        {
            _transactionManager = orchardService.TransactionManager;
            _searchFilters = searchFilters;
        }

        public IEnumerable<ReportStatsVM> GetAggregate(PAYEBatchRecordSearchParams searchParams)
        {
            var session = _transactionManager.GetSession();
            var query = GetCriteria(searchParams);
            return query.SetProjection(
                Projections.ProjectionList()
                    .Add(Projections.Count<Models.PAYEBatchRecord>(x => x.Id), "TotalRecordCount")
            ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }

        public IEnumerable<PAYEBatchRecordVM> GetBatchRecords(PAYEBatchRecordSearchParams searchParams)
        {
            var query = GetCriteria(searchParams);

            if (!searchParams.DontPageData)
            {
                query
                .SetFirstResult(searchParams.Skip)
                .SetMaxResults(searchParams.Take);
            }

            query
            .SetProjection(
                Projections.ProjectionList()
                .Add(Projections.Property("PBR.Id"), "BatchRecordId")
                .Add(Projections.Property("PBR.BatchRef"), "BatchRef")
                .Add(Projections.Property("PBR.PaymentCompleted"), "PaymentCompleted")
                .Add(Projections.Property("PBR.RevenueHeadSurCharge"), "RevenueHeadSurCharge")
                ).SetResultTransformer(Transformers.AliasToBean<PAYEBatchRecordVM>());

            return query.AddOrder(Order.Desc("Id"))
                .Future<PAYEBatchRecordVM>();
        }

        public ICriteria GetCriteria(PAYEBatchRecordSearchParams searchParams)
        {
            var session = _transactionManager.GetSession();
            var criteria = session.CreateCriteria<Models.PAYEBatchRecord>("PBR")
                            .CreateAlias("PBR.TaxEntity", "TaxEntity");

            criteria.Add(Restrictions.Between("CreatedAtUtc", searchParams.StartDate, searchParams.EndDate));

            foreach (var filter in _searchFilters)
            {
                filter.AddCriteriaRestriction(criteria, searchParams);
            }

            return criteria;
        }

        public dynamic GetBatchRecordsViewModel(PAYEBatchRecordSearchParams searchParams)
        {
            dynamic returnOBJ = new ExpandoObject();
            returnOBJ.BatchRecords = GetBatchRecords(searchParams);
            returnOBJ.Aggregate = GetAggregate(searchParams);
            return returnOBJ;
        }
    }
}