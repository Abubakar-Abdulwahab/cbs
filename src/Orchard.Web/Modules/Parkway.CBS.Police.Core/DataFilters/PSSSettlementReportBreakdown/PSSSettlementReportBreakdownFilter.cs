using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Police.Core.DataFilters.PSSSettlementReportBreakdown.Contracts;
using Parkway.CBS.Police.Core.DataFilters.PSSSettlementReportBreakdown.SearchFilters;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Parkway.CBS.Police.Core.DataFilters.PSSSettlementReportBreakdown
{
    public class PSSSettlementReportBreakdownFilter : IPSSSettlementReportBreakdownFilter
    {
        private readonly ITransactionManager _transactionManager;
        private readonly IEnumerable<IPSSSettlementReportBreakdownFilters> _searchFilters;

        public PSSSettlementReportBreakdownFilter(IOrchardServices orchardService, IEnumerable<IPSSSettlementReportBreakdownFilters> searchFilters)
        {
            _transactionManager = orchardService.TransactionManager;
            _searchFilters = searchFilters;
        }

        /// <summary>
        /// Get view model for settlement report breakdown
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, TotalRecordCount, TotalReportAmount }</returns>
        public dynamic GetReportViewModel(PSSSettlementReportBreakdownSearchParams searchParams)
        {
            dynamic returnOBJ = new ExpandoObject();

            try
            {
                returnOBJ.ReportRecords = GetReport(searchParams);
                returnOBJ.TotalRecordCount = GetTotalRecordCount(searchParams);
                returnOBJ.TotalReportAmount = GetTotalReportAmount(searchParams);
                return returnOBJ;
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Get the aggregate of the total number of items
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <returns></returns>
        private IEnumerable<ReportStatsVM> GetTotalRecordCount(PSSSettlementReportBreakdownSearchParams searchParams)
        {
            var query = GetCriteria(searchParams);

            return query.SetProjection(
               Projections.ProjectionList()
                    .Add(Projections.Count<PSSSettlementBatchItems>(x => x.Id), "TotalRecordCount")
               ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }


        /// <summary>
        /// Get total amount settled
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <returns></returns>
        private IEnumerable<ReportStatsVM> GetTotalReportAmount(PSSSettlementReportBreakdownSearchParams searchParams)
        {
            var query = GetCriteria(searchParams);

            return query.SetProjection(
               Projections.ProjectionList()
                    .Add(Projections.Sum<PSSSettlementBatchItems>(x => x.AmountSettled), "TotalAmount")
               ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }


        /// <summary>
        /// Get the full settlement report breakdown
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <returns>IEnumerable{PSSSettlementBatchItemsVM}</returns>
        private IEnumerable<PSSSettlementBatchItemsVM> GetReport(PSSSettlementReportBreakdownSearchParams searchParams)
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
                .Add(Projections.Property(nameof(PSSSettlementBatchItems.TransactionDate)), nameof(PSSSettlementBatchItemsVM.TransactionDate))
                .Add(Projections.Property($"{nameof(PSSRequest)}.{nameof(PSSRequest.FileRefNumber)}"), nameof(PSSSettlementBatchItemsVM.FileNumber))
                .Add(Projections.Property($"{nameof(Invoice)}.{nameof(Invoice.InvoiceNumber)}"), nameof(PSSSettlementBatchItemsVM.InvoiceNumber))
                .Add(Projections.Property($"{nameof(PSService)}.{nameof(PSService.Name)}"), nameof(PSSSettlementBatchItemsVM.ServiceName))
                .Add(Projections.Property($"{nameof(PSSSettlementFeePartyBatchAggregate)}.{nameof(PSSSettlementFeePartyBatchAggregate.FeePartyName)}"), nameof(PSSSettlementBatchItemsVM.SettlementParty))
                .Add(Projections.Property(nameof(PSSSettlementBatchItems.AmountSettled)), nameof(PSSSettlementBatchItemsVM.SettlementAmount))
                ).SetResultTransformer(Transformers.AliasToBean<PSSSettlementBatchItemsVM>())
                .Future<PSSSettlementBatchItemsVM>();
        }


        public ICriteria GetCriteria(PSSSettlementReportBreakdownSearchParams searchParams)
        {
            var session = _transactionManager.GetSession();
            var criteria = session.CreateCriteria<PSSSettlementBatchItems>(nameof(PSSSettlementBatchItems))
                .CreateAlias(nameof(PSSSettlementBatchItems.Request), nameof(PSSRequest))
                .CreateAlias(nameof(PSSSettlementBatchItems.Invoice), nameof(Invoice))
                .CreateAlias(nameof(PSSSettlementBatchItems.FeeParty), nameof(PSSFeeParty))
                .CreateAlias(nameof(PSSSettlementBatchItems.Service), nameof(PSService))
                .CreateAlias(nameof(PSSSettlementBatchItems.SettlementFeePartyBatchAggregate), nameof(PSSSettlementFeePartyBatchAggregate));

            criteria
                .Add(Restrictions.Between(nameof(PSSSettlementBatchItems.CreatedAtUtc), searchParams.StartDate, searchParams.EndDate));

            foreach (var filter in _searchFilters)
            {
                filter.AddCriteriaRestriction(criteria, searchParams);
            }

            return criteria;
        }
    }
}