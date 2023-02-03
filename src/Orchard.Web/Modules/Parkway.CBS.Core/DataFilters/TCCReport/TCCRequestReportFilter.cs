using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Parkway.CBS.Core.DataFilters.TCCReport.Contracts;
using Parkway.CBS.Core.DataFilters.TCCReport.SearchFilters.Contracts;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.DataFilters.TCCReport
{
    public class TCCRequestReportFilter : ITCCRequestReportFilter
    {
        private readonly ITransactionManager _transactionManager;
        protected readonly IEnumerable<ITCCRequestSearchFilter> _searchFilters;

        public TCCRequestReportFilter(IOrchardServices orchardService, IEnumerable<ITCCRequestSearchFilter> searchFilters)
        {
            _transactionManager = orchardService.TransactionManager;
            _searchFilters = searchFilters;
        }

        /// <summary>
        /// Get aggreagate for the search param
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>IEnumerable<ReportStatsVM></returns>
        public IEnumerable<ReportStatsVM> GetAggregate(TCCReportSearchParams searchParams)
        {
            return GetCriteria(searchParams).SetProjection(
                Projections.ProjectionList()
                    .Add(Projections.CountDistinct<TaxClearanceCertificateRequest>(x => x.Id), nameof(ReportStatsVM.TotalRecordCount))
            ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }

        /// <summary>
        /// Get TCC request report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>IEnumerable<TaxClearanceCertificateRequest></returns>
        public IEnumerable<TCCRequestVM> GetReport(TCCReportSearchParams searchParams)
        {
            ICriteria criteria = GetCriteria(searchParams);

            if (!searchParams.DontPageData)
            {
                criteria.SetFirstResult(searchParams.Skip).SetMaxResults(searchParams.Take);
            }

            return criteria.AddOrder(Order.Desc("Id"))
                .SetProjection(Projections.ProjectionList()
                .Add(Projections.Property(nameof(TaxClearanceCertificateRequest.Id)), nameof(TCCRequestVM.Id))
                .Add(Projections.Property(nameof(TaxClearanceCertificateRequest.ApplicantName)), nameof(TCCRequestVM.ApplicantName))
                .Add(Projections.Property(nameof(TaxClearanceCertificateRequest.Status)), nameof(TCCRequestVM.StatusValue))
                .Add(Projections.Property(nameof(TaxClearanceCertificateRequest.ResidentialAddress)), nameof(TCCRequestVM.ResidentialAddress))
                .Add(Projections.Property(nameof(TaxClearanceCertificateRequest.OfficeAddress)), nameof(TCCRequestVM.OfficeAddress))
                .Add(Projections.Property(nameof(TaxEntity)+"."+nameof(TaxEntity.PayerId)), nameof(TCCRequestVM.PayerId))
                .Add(Projections.Property(nameof(TaxClearanceCertificateRequest.IsRentedApartment)), nameof(TCCRequestVM.IsRentedApartmentValue))
                .Add(Projections.Property(nameof(TaxClearanceCertificateRequest.CreatedAtUtc)), nameof(TCCRequestVM.RequestDateValue))
                .Add(Projections.Property(nameof(TaxClearanceCertificateRequest.RequestReason)), nameof(TCCRequestVM.RequestReason))
                .Add(Projections.Property(nameof(TaxClearanceCertificateRequest.ApplicationNumber)), nameof(TCCRequestVM.ApplicationNumber))
                .Add(Projections.Property(nameof(TaxClearanceCertificateRequest.ExemptionCategory)), nameof(TCCRequestVM.ExemptionTypeValue))
                .Add(Projections.Property(nameof(TaxClearanceCertificateRequest.TCCNumber)), nameof(TCCRequestVM.TCCNumber))
                ).SetResultTransformer(Transformers.AliasToBean<TCCRequestVM>())
                .Future<TCCRequestVM>();            
        }

        /// <summary>
        /// Get report model
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, Aggregate }</returns>
        public dynamic GetReportViewModel(TCCReportSearchParams searchParams)
        {
            dynamic returnOBJ = new ExpandoObject();

            try
            {
                returnOBJ.ReportRecords = GetReport(searchParams);
                returnOBJ.Aggregate = GetAggregate(searchParams);
            }
            catch (NoRecordFoundException)
            {
                returnOBJ.ReportRecords = new List<TaxClearanceCertificateRequest> { };
            }
            return returnOBJ;
        }

        public ICriteria GetCriteria(TCCReportSearchParams searchParams)
        {
            var criteria = _transactionManager.GetSession().CreateCriteria<TaxClearanceCertificateRequest>(nameof(TaxClearanceCertificateRequest))
                .CreateAlias(nameof(TaxClearanceCertificateRequest.TaxEntity), nameof(TaxEntity));

            criteria.Add(Restrictions.Between(nameof(TaxClearanceCertificateRequest.CreatedAtUtc), searchParams.StartDate, searchParams.EndDate));
                 

            foreach (var filter in _searchFilters)
            {
                filter.AddCriteriaRestriction(criteria, searchParams);
            }

            return criteria;
        }

    }
}