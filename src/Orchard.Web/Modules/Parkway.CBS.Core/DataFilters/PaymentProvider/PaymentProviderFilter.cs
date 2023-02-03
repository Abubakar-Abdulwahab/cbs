using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Parkway.CBS.Core.DataFilters.PaymentProvider.SearchFilters;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using System.Collections.Generic;
using System.Dynamic;

namespace Parkway.CBS.Core.DataFilters.PaymentProvider
{
    public class PaymentProviderFilter : IPaymentProviderFilter
    {

        private readonly ITransactionManager _transactionManager;
        protected readonly IEnumerable<IPaymentProviderSearchFilter> _searchFilters;

        public PaymentProviderFilter(IOrchardServices orchardService, IEnumerable<IPaymentProviderSearchFilter> searchFilters)
        {
            _transactionManager = orchardService.TransactionManager;
            _searchFilters = searchFilters;
        }


        public IEnumerable<ReportStatsVM> GetAggregate(PaymentProviderSearchParams searchParams)
        {
            var session = _transactionManager.GetSession();
            var query = GetCriteria(searchParams);
            return query.SetProjection(
                Projections.ProjectionList()
                    .Add(Projections.Count<ExternalPaymentProvider>(x => x.Id), nameof(ReportStatsVM.TotalRecordCount))
            ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }


        public IEnumerable<ExternalPaymentProviderVM> GetProviders(PaymentProviderSearchParams searchParams)
        {
            var query = GetCriteria(searchParams);
            query
              .CreateAlias(nameof(ExternalPaymentProvider)+"."+nameof(ExternalPaymentProvider.AddedBy), nameof(ExternalPaymentProvider.AddedBy));
            if (!searchParams.DontPageData)
            {
                query
                .SetFirstResult(searchParams.Skip)
                .SetMaxResults(searchParams.Take);
            }

            query.SetProjection(
                Projections.ProjectionList()
                .Add(Projections.Property<ExternalPaymentProvider>(epp => epp.Id), "Id")
                .Add(Projections.Property<ExternalPaymentProvider>(epp => epp.Name), "Name")
                .Add(Projections.Property<ExternalPaymentProvider>(epp => epp.CreatedAtUtc), "CreatedAt")
                .Add(Projections.Property<ExternalPaymentProvider>(epp => epp.ClientID), "ClientID")
                .Add(Projections.Property<ExternalPaymentProvider>(epp => epp.AddedBy.UserName), "AddedBy")
                ).SetResultTransformer(Transformers.AliasToBean<ExternalPaymentProviderVM>());


            return query.AddOrder(Order.Desc("Id"))
                .Future<ExternalPaymentProviderVM>();
        }


        public dynamic GetPaymentProvidersViewModel(PaymentProviderSearchParams searchParams)
        {
            dynamic returnOBJ = new ExpandoObject();
            returnOBJ.ProviderRecords = GetProviders(searchParams);
            returnOBJ.Aggregate = GetAggregate(searchParams);
            return returnOBJ;
        }


        public ICriteria GetCriteria(PaymentProviderSearchParams searchParams)
        {
            var session = _transactionManager.GetSession();
            var criteria = session.CreateCriteria<ExternalPaymentProvider>(nameof(ExternalPaymentProvider));

            //criteria.Add(Restrictions.Between(nameof(ExternalPaymentProvider.CreatedAtUtc), searchParams.StartDate, searchParams.EndDate));

            foreach (var filter in _searchFilters)
            {
                filter.AddCriteriaRestriction(criteria, searchParams);
            }

            return criteria;
        }
    }
}