using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Parkway.CBS.Core.DataFilters.PAYEBatchItemReceiptReport.Contracts;
using Parkway.CBS.Core.DataFilters.PAYEBatchItemReceiptReport.SearchFilters.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using System.Collections.Generic;
using System.Dynamic;

namespace Parkway.CBS.Core.DataFilters.PAYEBatchItemReceiptReport
{
    public class PAYEBatchItemReceiptFilter : IPAYEBatchItemReceiptFilter
    {
        private readonly ITransactionManager _transactionManager;
        protected readonly IEnumerable<IPAYEBatchItemReceiptSearchFilter> _searchFilters;

        public PAYEBatchItemReceiptFilter(IOrchardServices orchardService, IEnumerable<IPAYEBatchItemReceiptSearchFilter> searchFilters)
        {
            _transactionManager = orchardService.TransactionManager;
            _searchFilters = searchFilters;
        }

        public IEnumerable<ReportStatsVM> GetAggregate(PAYEReceiptSearchParams searchParams)
        {
            var session = _transactionManager.GetSession();
            var query = GetCriteria(searchParams);
            return query.SetProjection(
                Projections.ProjectionList()
                    .Add(Projections.Count<PAYEBatchItemReceipt>(x => x.Id), "TotalRecordCount")
            ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }

        public IEnumerable<PAYEReceiptItems> GetReceipts(PAYEReceiptSearchParams searchParams)
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
                .Add(Projections.Property("TaxEntity.Recipient"), "PayerName")
                .Add(Projections.Property("PYR.ReceiptNumber"), "ReceiptNumber")
                .Add(Projections.Property("PAYEBatchItem.GrossAnnual"), "AnnualEarnings")
                .Add(Projections.Property("PAYEBatchItem.Exemptions"), "Exemptions")
                .Add(Projections.Property("PAYEBatchItem.IncomeTaxPerMonth"), "IncomeTaxValue")
                .Add(Projections.Property("TaxEntity.PayerId"), "PayerId")
                .Add(Projections.Property("PAYEBatchItem.Month"), "MonthId")
                .Add(Projections.Property("PAYEBatchItem.Year"), "Year")
                ).SetResultTransformer(Transformers.AliasToBean<PAYEReceiptItems>());

            return query.AddOrder(Order.Desc("Id"))
                .Future<PAYEReceiptItems>();
        }

        public ICriteria GetCriteria(PAYEReceiptSearchParams searchParams)
        {
            var session = _transactionManager.GetSession();
            var criteria = session.CreateCriteria<PAYEBatchItemReceipt>("PYR")
                            .CreateAlias("PYR.PAYEBatchItem", "PAYEBatchItem")
                            .CreateAlias("PYR.PAYEBatchItem.TaxEntity", "TaxEntity");

            criteria.Add(Restrictions.Between("CreatedAtUtc", searchParams.StartDate, searchParams.EndDate));

            foreach (var filter in _searchFilters)
            {
                filter.AddCriteriaRestriction(criteria, searchParams);
            }

            return criteria;
        }

        public dynamic GetReceiptViewModel(PAYEReceiptSearchParams searchParams)
        {
            dynamic returnOBJ = new ExpandoObject();
            returnOBJ.ReceiptRecords = GetReceipts(searchParams);
            returnOBJ.Aggregate = GetAggregate(searchParams);
            return returnOBJ;
        }
    }
}