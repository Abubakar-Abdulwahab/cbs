using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.DataFilters.PSSSettlementReportParty.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.DataFilters.PSSSettlementReportParty
{
    public class PSSSettlementReportPartyFilter : IPSSSettlementReportPartyFilter
    {
        private readonly ITransactionManager _transactionManager;

        public PSSSettlementReportPartyFilter(IOrchardServices orchardService)
        {
            _transactionManager = orchardService.TransactionManager;
        }

        /// <summary>
        /// Get view model for settlement report parties
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, TotalNumberOfFeeParties, TotalAmountSettled }</returns>
        public dynamic GetRequestReportViewModel(PSSSettlementReportPartySearchParams searchParams)
        {
            dynamic returnOBJ = new ExpandoObject();

            try
            {
                returnOBJ.ReportRecords = GetReport(searchParams);
                returnOBJ.TotalNumberOfFeeParties = GetTotalNumberOfFeeParties(searchParams);
                returnOBJ.TotalAmountSettled = GetTotalAmountSettled(searchParams);
                return returnOBJ;
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Gets the total amount settled
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <returns></returns>
        private IEnumerable<ReportStatsVM> GetTotalAmountSettled(PSSSettlementReportPartySearchParams searchParams)
        {
            var query = GetCriteria(searchParams);

            return query.SetProjection(
               Projections.ProjectionList()
                    .Add(Projections.Sum<PSSSettlementFeePartyBatchAggregate>(x => x.TotalSettlementAmount), "TotalAmount")
               ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }


        /// <summary>
        /// Get the aggregate of the total number of fee parties for the settlement
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <returns></returns>
        private IEnumerable<ReportStatsVM> GetTotalNumberOfFeeParties(PSSSettlementReportPartySearchParams searchParams)
        {
            var query = GetCriteria(searchParams);

            return query.SetProjection(
               Projections.ProjectionList()
                    .Add(Projections.Count<PSSSettlementFeePartyBatchAggregate>(x => x.Id), "TotalRecordCount")
               ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }


        /// <summary>
        /// Get the list of settlement report fee parties
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <returns>IEnumerable{PSSSettlementFeePartyBatchAggregateVM}</returns>
        private IEnumerable<PSSSettlementFeePartyBatchAggregateVM> GetReport(PSSSettlementReportPartySearchParams searchParams)
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
                .Add(Projections.Property(nameof(PSSSettlementFeePartyBatchAggregate.Id)), nameof(PSSSettlementFeePartyBatchAggregateVM.Id))
                .Add(Projections.Property($"{nameof(PSSFeeParty)}.{nameof(PSSFeeParty.Id)}"), nameof(PSSSettlementFeePartyBatchAggregateVM.FeePartyId))
                .Add(Projections.Property(nameof(PSSSettlementFeePartyBatchAggregate.FeePartyName)), nameof(PSSSettlementFeePartyBatchAggregateVM.FeePartyName))
                .Add(Projections.Property($"{nameof(PSSSettlementBatch)}.{nameof(PSSSettlementBatch.SettlementDate)}"), nameof(PSSSettlementFeePartyBatchAggregateVM.TransactionDate))
                .Add(Projections.Property(nameof(PSSSettlementFeePartyBatchAggregate.TotalSettlementAmount)), nameof(PSSSettlementFeePartyBatchAggregateVM.TotalSettlementAmount))
                .Add(Projections.Property(nameof(PSSSettlementFeePartyBatchAggregate.Percentage)), nameof(PSSSettlementFeePartyBatchAggregateVM.Percentage))
                .Add(Projections.Property(nameof(PSSSettlementFeePartyBatchAggregate.BankName)), nameof(PSSSettlementFeePartyBatchAggregateVM.BankName))
                .Add(Projections.Property(nameof(PSSSettlementFeePartyBatchAggregate.BankAccountNumber)), nameof(PSSSettlementFeePartyBatchAggregateVM.BankAccountNumber))
                .Add(Projections.Property($"{nameof(Command)}.{nameof(Command.Id)}"), nameof(PSSSettlementFeePartyBatchAggregateVM.CommandId))
                ).SetResultTransformer(Transformers.AliasToBean<PSSSettlementFeePartyBatchAggregateVM>())
                .Future<PSSSettlementFeePartyBatchAggregateVM>();
        }


        public ICriteria GetCriteria(PSSSettlementReportPartySearchParams searchParams)
        {
            var session = _transactionManager.GetSession();
            var criteria = session.CreateCriteria<PSSSettlementFeePartyBatchAggregate>(nameof(PSSSettlementFeePartyBatchAggregate))
                .CreateAlias(nameof(PSSSettlementFeePartyBatchAggregate.Batch), nameof(PSSSettlementBatch))
                .CreateAlias(nameof(PSSSettlementFeePartyBatchAggregate.FeeParty), nameof(PSSFeeParty))
                .CreateAlias(nameof(PSSSettlementFeePartyBatchAggregate.Command), nameof(Command), NHibernate.SqlCommand.JoinType.LeftOuterJoin);
            criteria
                .Add(Restrictions.Eq($"{nameof(PSSSettlementBatch)}.{nameof(PSSSettlementBatch.BatchRef)}", searchParams.BatchRef));

            return criteria;
        }
    }
}