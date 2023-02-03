using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Parkway.CBS.Core.DataFilters.SettlementRules.SearchFilters;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using System.Collections.Generic;
using System.Dynamic;

namespace Parkway.CBS.Core.DataFilters.SettlementRules
{
    public class SettlementRulesFilter : ISettlementRulesFilter
    {

        private readonly ITransactionManager _transactionManager;
        protected readonly IEnumerable<ISettlementRulesSearchFilter> _searchFilters;

        public SettlementRulesFilter(IOrchardServices orchardService, IEnumerable<ISettlementRulesSearchFilter> searchFilters)
        {
            _transactionManager = orchardService.TransactionManager;
            _searchFilters = searchFilters;
        }


        public IEnumerable<ReportStatsVM> GetAggregate(SettlementRulesSearchParams searchParams)
        {
            var session = _transactionManager.GetSession();
            var query = GetCriteria(searchParams);
            return query.SetProjection(
                Projections.ProjectionList()
                    .Add(Projections.Count<SettlementRule>(x => x.Id), "TotalRecordCount")
            ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }


        public IEnumerable<SettlementRuleLite> GetSettlementRules(SettlementRulesSearchParams searchParams)
        {
            var query = GetCriteria(searchParams);

            if (!searchParams.DontPageData)
            {
                query
                .SetFirstResult(searchParams.Skip)
                .SetMaxResults(searchParams.Take);
            }

            query.SetProjection(
                Projections.ProjectionList()
                .Add(Projections.Property<SettlementRule>(SR => SR.Id), "Id")
                .Add(Projections.Property<SettlementRule>(SR => SR.Name), "Name")
                .Add(Projections.Property<SettlementRule>(SR => SR.SettlementEngineRuleIdentifier), "RuleIdentifier")
                .Add(Projections.Property<SettlementRule>(SR => SR.NextScheduleDate), "NextScheduleDate")
                ).SetResultTransformer(Transformers.AliasToBean<SettlementRuleLite>());


            return query.AddOrder(Order.Desc("Id"))
                .Future<SettlementRuleLite>();
        }


        public dynamic GetSettlementRulesListViewModel(SettlementRulesSearchParams searchParams)
        {
            dynamic returnOBJ = new ExpandoObject();
            returnOBJ.RulesRecords = GetSettlementRules(searchParams);
            returnOBJ.Aggregate = GetAggregate(searchParams);
            return returnOBJ;
        }


        public ICriteria GetCriteria(SettlementRulesSearchParams searchParams)
        {
            var session = _transactionManager.GetSession();
            var criteria = session.CreateCriteria<SettlementRule>("SR");

            //criteria.Add(Restrictions.Between(nameof(SettlementRule.CreatedAtUtc), searchParams.StartDate, searchParams.EndDate));
            criteria.Add(Restrictions.Eq("AddedBy", searchParams.Admin));

            foreach (var filter in _searchFilters)
            {
                filter.AddCriteriaRestriction(criteria, searchParams);
            }

            return criteria;
        }
    }
}