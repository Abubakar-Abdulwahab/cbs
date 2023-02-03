using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Orchard.Data;
using Parkway.CBS.Core.DataFilters.DirectAssessmentReport.Contracts;
using Parkway.CBS.Core.DataFilters.DirectAssessmentReport.SearchFilters.Contracts;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using System.Collections.Generic;
using System.Dynamic;

namespace Parkway.CBS.Core.DataFilters.DirectAssessmentReport
{
    public class DirectAssessmentRequestReportFilter : IDirectAssessmentRequestReportFilter
    {
        private readonly ITransactionManager _transactionManager;
        protected readonly IEnumerable<IDirectAssessmentRequestSearchFilter> _searchFilters;

        public DirectAssessmentRequestReportFilter(ITransactionManager transactionManager, IEnumerable<IDirectAssessmentRequestSearchFilter> searchFilters)
        {
            _transactionManager = transactionManager;
            _searchFilters = searchFilters;
        }

        /// <summary>
        /// Get aggreagate for the search param
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>IEnumerable<ReportStatsVM></returns>
        public IEnumerable<ReportStatsVM> GetAggregate(DirectAssessmentSearchParams searchParams)
        {
            return GetCriteria(searchParams).SetProjection(
                           Projections.ProjectionList()
                               .Add(Projections.Sum<PAYEDirectAssessmentRecord>(x => x.Invoice.Amount), nameof(ReportStatsVM.TotalAmount))
                               .Add(Projections.Count<PAYEDirectAssessmentRecord>(x => x.Id), nameof(ReportStatsVM.TotalRecordCount))
                       ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }

        /// <summary>
        /// Get direct assessment request report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>IEnumerable<TCCRequestVM></returns>
        public IEnumerable<DirectAssessmentReportItemsVM> GetReport(DirectAssessmentSearchParams searchParams)
        {
            ICriteria criteria = GetCriteria(searchParams);
            criteria.AddOrder(Order.Desc("Id"))
                         .SetProjection(Projections.ProjectionList()
                         .Add(Projections.Property<PAYEDirectAssessmentRecord>(x => x.Invoice.Amount), nameof(DirectAssessmentReportItemsVM.InvoiceAmount))
                         .Add(Projections.Property<PAYEDirectAssessmentRecord>(x => x.Invoice.Status), nameof(DirectAssessmentReportItemsVM.InvoiceStatusId))
                         .Add(Projections.Property<PAYEDirectAssessmentRecord>(x => x.Invoice.InvoiceNumber), nameof(DirectAssessmentReportItemsVM.InvoiceNo))
                         .Add(Projections.Property<PAYEDirectAssessmentRecord>(x => x.Comment), nameof(DirectAssessmentReportItemsVM.Comments))
                         .Add(Projections.Property<PAYEDirectAssessmentRecord>(x => x.AssessmentType.Name), nameof(DirectAssessmentReportItemsVM.DirectAssessmentType))
                         .Add(Projections.Property<PAYEDirectAssessmentRecord>(x => x.Year), nameof(DirectAssessmentReportItemsVM.Year))
                         .Add(Projections.Property<PAYEDirectAssessmentRecord>(x => x.Month), nameof(DirectAssessmentReportItemsVM.Month))
                         .Add(Projections.Property("TaxPayer.PayerId"), nameof(DirectAssessmentReportItemsVM.StateTIN))
                         .Add(Projections.Property("TaxPayer.Recipient"), nameof(DirectAssessmentReportItemsVM.PayerName))
                         .Add(Projections.Property<PAYEDirectAssessmentRecord>(x => x.AssessedBy.NormalizedUserName), nameof(DirectAssessmentReportItemsVM.AssessedBy))
                         .Add(Projections.Property<PAYEDirectAssessmentRecord>(x => x.AssessedBy.CreatedUtc), nameof(DirectAssessmentReportItemsVM.AssessmentDate)));

            if (!searchParams.DontPageData)
            {
                criteria.SetFirstResult(searchParams.Skip).SetMaxResults(searchParams.Take);
            }

            return criteria.SetResultTransformer(Transformers.AliasToBean<DirectAssessmentReportItemsVM>())
                          .Future<DirectAssessmentReportItemsVM>();
        }

        /// <summary>
        /// Get report view model
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, Aggregate }</returns>
        public dynamic GetReportViewModel(DirectAssessmentSearchParams searchParams)
        {
            dynamic returnOBJ = new ExpandoObject();

            try
            {
                returnOBJ.ReportRecords = GetReport(searchParams);
                returnOBJ.Aggregate = GetAggregate(searchParams);
            }
            catch (NoRecordFoundException)
            {
                returnOBJ.ReportRecords = new List<PAYEDirectAssessmentReportVM> { };
            }
            return returnOBJ;
        }

        /// <summary>
        /// Get search query Criterias
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>ICriteria</returns>
        public ICriteria GetCriteria(DirectAssessmentSearchParams searchParams)
        {
            var criteria = _transactionManager.GetSession().CreateCriteria<PAYEDirectAssessmentRecord>(nameof(PAYEDirectAssessmentRecord))
                .CreateAlias("PAYEDirectAssessmentRecord.Invoice", nameof(Invoice))
                .CreateAlias("PAYEDirectAssessmentRecord.AssessedBy", "AssessedBy")
                .CreateAlias("PAYEDirectAssessmentRecord.AssessmentType", "AssessmentType")
                .CreateAlias("Invoice.TaxPayer", "TaxPayer");

            criteria.Add(Restrictions.Between(nameof(PAYEDirectAssessmentRecord.CreatedAtUtc), searchParams.StartDate, searchParams.EndDate));

            foreach (var filter in _searchFilters)
            {
                filter.AddCriteriaRestriction(criteria, searchParams);
            }

            return criteria;
        }
    }
}