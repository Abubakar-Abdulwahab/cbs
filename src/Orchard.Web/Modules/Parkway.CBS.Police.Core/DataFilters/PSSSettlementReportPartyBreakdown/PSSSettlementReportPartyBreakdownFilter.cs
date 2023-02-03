using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Police.Core.DataFilters.PSSSettlementReportPartyBreakdown.Contracts;
using Parkway.CBS.Police.Core.DataFilters.PSSSettlementReportPartyBreakdown.SearchFilters;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Parkway.CBS.Police.Core.DataFilters.PSSSettlementReportPartyBreakdown
{
    public class PSSSettlementReportPartyBreakdownFilter : IPSSSettlementReportPartyBreakdownFilter
    {
        private readonly ITransactionManager _transactionManager;
        private readonly IEnumerable<IPSSSettlementReportPartyBreakdownFilters> _searchFilters;

        public PSSSettlementReportPartyBreakdownFilter(IOrchardServices orchardService, IEnumerable<IPSSSettlementReportPartyBreakdownFilters> searchFilters)
        {
            _transactionManager = orchardService.TransactionManager;
            _searchFilters = searchFilters;
        }

        /// <summary>
        /// Get view model for settlement report party breakdown
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, TotalNumberOfBatchItems, AmountSettled }</returns>
        public dynamic GetReportViewModel(PSSSettlementReportPartyBreakdownSearchParams searchParams)
        {
            dynamic returnOBJ = new ExpandoObject();

            try
            {
                returnOBJ.ReportRecords = GetReport(searchParams);
                returnOBJ.TotalNumberOfBatchItems = GetTotalNumberOfBatchItems(searchParams);
                returnOBJ.AmountSettled = GetAmountSettled(searchParams);
                return returnOBJ;
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Gets the amount settled for the fee party
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <returns></returns>
        private IEnumerable<ReportStatsVM> GetAmountSettled(PSSSettlementReportPartyBreakdownSearchParams searchParams)
        {
            var query = GetCriteria(searchParams);

            return query.SetProjection(
               Projections.ProjectionList()
                    .Add(Projections.Sum<PSSSettlementBatchItems>(x => x.AmountSettled), "TotalAmount")
               ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }


        /// <summary>
        /// Get the aggregate of the total number of fee parties for the settlement
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <returns></returns>
        private IEnumerable<ReportStatsVM> GetTotalNumberOfBatchItems(PSSSettlementReportPartyBreakdownSearchParams searchParams)
        {
            var query = GetCriteria(searchParams);

            return query.SetProjection(
               Projections.ProjectionList()
                    .Add(Projections.Count<PSSSettlementBatchItems>(x => x.Id), "TotalRecordCount")
               ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }


        /// <summary>
        /// Get the settlement report fee party breakdown containing list of batch items for specified fee party
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <returns>IEnumerable{PSSSettlementBatchItemsVM}</returns>
        private IEnumerable<PSSSettlementBatchItemsVM> GetReport(PSSSettlementReportPartyBreakdownSearchParams searchParams)
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
                .Add(Projections.Property(nameof(PSSSettlementBatchItems.AmountSettled)), nameof(PSSSettlementBatchItemsVM.SettlementAmount))
                .Add(Projections.Property($"{nameof(PSSSettlementBatchItems.GeneratedByCommand)}.{nameof(Command.Name)}"), nameof(PSSSettlementBatchItemsVM.GeneratedByCommandName))
                ).SetResultTransformer(Transformers.AliasToBean<PSSSettlementBatchItemsVM>())
                .Future<PSSSettlementBatchItemsVM>();
        }


        public ICriteria GetCriteria(PSSSettlementReportPartyBreakdownSearchParams searchParams)
        {
            var session = _transactionManager.GetSession();
            var criteria = session.CreateCriteria<PSSSettlementBatchItems>(nameof(PSSSettlementBatchItems))
                    .CreateAlias(nameof(PSSSettlementBatchItems.FeeParty), nameof(PSSFeeParty))
                    .CreateAlias(nameof(PSSSettlementBatchItems.Batch), nameof(PSSSettlementBatch))
                    .CreateAlias(nameof(PSSSettlementBatchItems.Invoice), nameof(Invoice))
                    .CreateAlias(nameof(PSSSettlementBatchItems.Request), nameof(PSSRequest))
                    .CreateAlias(nameof(PSSSettlementBatchItems.Service), nameof(PSService))
                    .CreateAlias(nameof(PSSSettlementBatchItems.SettlementFeePartyBatchAggregate), nameof(PSSSettlementFeePartyBatchAggregate))
                    .CreateAlias(nameof(PSSSettlementBatchItems.GeneratedByCommand), nameof(PSSSettlementBatchItems.GeneratedByCommand), NHibernate.SqlCommand.JoinType.LeftOuterJoin);

            criteria
                .Add(Restrictions.And(Restrictions.Eq($"{nameof(PSSSettlementBatch)}.{nameof(PSSSettlementBatch.BatchRef)}", searchParams.BatchRef), Restrictions.Eq($"{nameof(PSSSettlementFeePartyBatchAggregate)}.{nameof(PSSSettlementFeePartyBatchAggregate.Id)}", searchParams.FeePartyBatchAggregateId)));

            return criteria;
        }
    }
}