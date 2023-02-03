using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Parkway.CBS.Core.DataFilters.PAYEReceiptUtilizationReport.Contracts;
using Parkway.CBS.Core.DataFilters.PAYEReceiptUtilizationReport.SearchFilters.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using System.Collections.Generic;
using System.Dynamic;

namespace Parkway.CBS.Core.DataFilters.PAYEReceiptUtilizationReport
{
    public class PAYEReceiptUtilizationFilter : IPAYEReceiptUtilizationFilter
    {
        private readonly ITransactionManager _transactionManager;
        protected readonly IEnumerable<IPAYEReceiptUtilizationSearchFilter> _searchFilters;

        public PAYEReceiptUtilizationFilter(IOrchardServices orchardService, IEnumerable<IPAYEReceiptUtilizationSearchFilter> searchFilters)
        {
            _transactionManager = orchardService.TransactionManager;
            _searchFilters = searchFilters;
        }

        /// <summary>
        /// Get aggregate report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>IEnumerable<ReportStatsVM></returns>
        public IEnumerable<ReportStatsVM> GetAggregate(PAYEReceiptSearchParams searchParams)
        {
            var session = _transactionManager.GetSession();
            var query = GetCriteria(searchParams);
            return query.SetProjection(
                Projections.ProjectionList()
                    .Add(Projections.Count<PAYEReceipt>(x => x.Id), nameof(ReportStatsVM.TotalRecordCount))
            ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }

        /// <summary>
        /// Get PAYE receipts list
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>IEnumerable<PAYEReceiptVM></returns>
        public IEnumerable<PAYEReceiptVM> GetReceipts(PAYEReceiptSearchParams searchParams)
        {
            var query = GetCriteria(searchParams);

            if (!searchParams.DontPageData)
            {
                query
                .SetFirstResult(searchParams.Skip)
                .SetMaxResults(searchParams.Take);
            }

          return query
                .AddOrder(Order.Desc(nameof(PAYEReceipt.Id))).SetProjection(
                Projections.ProjectionList()
                .Add(Projections.Property($"{nameof(PAYEReceipt.Receipt)}.{nameof(PAYEReceipt.Receipt.CreatedAtUtc)}"), nameof(PAYEReceiptVM.PaymentDate))
                .Add(Projections.Property($"{nameof(PAYEReceipt.Receipt)}.{nameof(PAYEReceipt.Receipt.ReceiptNumber)}"), nameof(PAYEReceiptVM.ReceiptNumber))
                .Add(Projections.Property($"{nameof(PAYEReceipt)}.{nameof(PAYEReceipt.UtilizationStatusId)}"), nameof(PAYEReceiptVM.Status))
                .Add(Projections.Property($"{nameof(PAYEReceipt.Receipt.Invoice.TaxPayer)}.{nameof(PAYEReceipt.Receipt.Invoice.TaxPayer.Recipient)}"), nameof(PAYEReceiptVM.PayerName))
                .Add(Projections.Property($"{nameof(PAYEReceipt.Receipt.Invoice.TaxPayer)}.{nameof(PAYEReceipt.Receipt.Invoice.TaxPayer.PayerId)}"), nameof(PAYEReceiptVM.PayerId))
                .Add(Projections.Property($"{nameof(PAYEReceipt.Receipt.Invoice.TaxPayer)}.{nameof(PAYEReceipt.Receipt.Invoice.TaxPayer.PhoneNumber)}"), nameof(PAYEReceiptVM.PhoneNo))
                ).SetResultTransformer(Transformers.AliasToBean<PAYEReceiptVM>()).Future<PAYEReceiptVM>();                
        }

        /// <summary>
        /// Prepare query criteria for PAYE receipt
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public ICriteria GetCriteria(PAYEReceiptSearchParams searchParams)
        {
            var session = _transactionManager.GetSession();
            var criteria = session.CreateCriteria<PAYEReceipt>(nameof(PAYEReceipt))
                            .CreateAlias($"{nameof(PAYEReceipt)}.{nameof(PAYEReceipt.Receipt)}", nameof(PAYEReceipt.Receipt))
                            .CreateAlias($"{nameof(PAYEReceipt)}.{nameof(PAYEReceipt.Receipt)}.{nameof(PAYEReceipt.Receipt.Invoice)}", nameof(PAYEReceipt.Receipt.Invoice))
                            .CreateAlias($"{nameof(PAYEReceipt)}.{nameof(PAYEReceipt.Receipt)}.{nameof(PAYEReceipt.Receipt.Invoice)}.{nameof(PAYEReceipt.Receipt.Invoice.TaxPayer)}", nameof(PAYEReceipt.Receipt.Invoice.TaxPayer));

            criteria.Add(Restrictions.Between(nameof(PAYEReceipt.CreatedAtUtc), searchParams.StartDate, searchParams.EndDate));

            foreach (var filter in _searchFilters)
            {
                filter.AddCriteriaRestriction(criteria, searchParams);
            }

            return criteria;
        }

        /// <summary>
        /// Get PAYE receipts and report aggregate object
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>dynamic</returns>
        public dynamic GetReceiptViewModel(PAYEReceiptSearchParams searchParams)
        {
            dynamic returnOBJ = new ExpandoObject();
            returnOBJ.ReceiptRecords = GetReceipts(searchParams);
            returnOBJ.Aggregate = GetAggregate(searchParams);
            return returnOBJ;
        }

        /// <summary>
        /// Gets receipt utilizations report for a specified receipt number
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <returns>IEnumerable<PAYEReceiptUtilizationReportVM></returns>
        public IEnumerable<PAYEReceiptUtilizationReportVM> GetReceiptUtilizations(string receiptNumber)
        {
            var criteria = _transactionManager.GetSession().CreateCriteria<PAYEPaymentUtilization>(nameof(PAYEPaymentUtilization))
                            .CreateAlias($"{nameof(PAYEPaymentUtilization)}.{nameof(PAYEPaymentUtilization.PAYEReceipt)}", nameof(PAYEPaymentUtilization.PAYEReceipt))
                            .CreateAlias($"{nameof(PAYEPaymentUtilization)}.{nameof(PAYEPaymentUtilization.PAYEBatchRecord)}", nameof(PAYEPaymentUtilization.PAYEBatchRecord))
                            .CreateAlias($"{nameof(PAYEPaymentUtilization)}.{nameof(PAYEPaymentUtilization.PAYEReceipt)}.{nameof(PAYEPaymentUtilization.PAYEReceipt.Receipt)}", nameof(PAYEPaymentUtilization.PAYEReceipt.Receipt)).Add(Restrictions.Eq($"{nameof(PAYEPaymentUtilization.PAYEReceipt.Receipt)}.{nameof(PAYEPaymentUtilization.PAYEReceipt.Receipt.ReceiptNumber)}", receiptNumber));

            criteria
            .SetProjection(
                Projections.ProjectionList()
                .Add(Projections.Property(nameof(PAYEPaymentUtilization.CreatedAtUtc)), nameof(PAYEReceiptUtilizationReportVM.UtilizedDate))
                .Add(Projections.Property(nameof(PAYEPaymentUtilization.UtilizedAmount)), nameof(PAYEReceiptUtilizationReportVM.UtilizedAmount))
                .Add(Projections.Property($"{nameof(PAYEPaymentUtilization.PAYEBatchRecord)}.{nameof(PAYEPaymentUtilization.PAYEBatchRecord.BatchRef)}"), nameof(PAYEReceiptUtilizationReportVM.BatchRef))
                ).SetResultTransformer(Transformers.AliasToBean<PAYEReceiptUtilizationReportVM>());

            return criteria.AddOrder(Order.Desc(nameof(PAYEPaymentUtilization.Id)))
                .Future<PAYEReceiptUtilizationReportVM>();
        }
    }
}