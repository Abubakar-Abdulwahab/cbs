using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Police.Core.DataFilters.GenerateRequestWithoutOfficersUploadBatchStagingReport.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.DataFilters.GenerateRequestWithoutOfficersUploadBatchStagingReport
{
    public class GenerateRequestWithoutOfficersUploadBatchStagingFilter : IGenerateRequestWithoutOfficersUploadBatchStagingFilter
    {
        private readonly ITransactionManager _transactionManager;

        public GenerateRequestWithoutOfficersUploadBatchStagingFilter(IOrchardServices orchardService/*, IEnumerable<ITaxEntityProfileLocationSearchFilter> searchFilters*/)
        {
            _transactionManager = orchardService.TransactionManager;
        }


        /// <summary>
        /// Get aggreagate for the search param
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>IEnumerable<ReportStatsVM></returns>
        public IEnumerable<ReportStatsVM> GetAggregate(GenerateRequestWithoutOfficersUploadBatchStagingReportSearchParams searchParams)
        {
            return GetCriteria(searchParams).SetProjection(
                Projections.ProjectionList()
                    .Add(Projections.Count<GenerateRequestWithoutOfficersUploadBatchStaging>(x => x.Id), nameof(ReportStatsVM.TotalRecordCount))
            ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }


        /// <summary>
        /// Get Generate Request Without Officers Upload Batch Staging report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>IEnumerable<TaxEntityProfileLocationVM></returns>
        public IEnumerable<GenerateRequestWithoutOfficersUploadBatchStagingDTO> GetReport(GenerateRequestWithoutOfficersUploadBatchStagingReportSearchParams searchParams)
        {
            ICriteria criteria = GetCriteria(searchParams);

            if (searchParams.PageData)
            {
                criteria.SetFirstResult(searchParams.Skip).SetMaxResults(searchParams.Take);
            }

            return criteria.AddOrder(Order.Desc("Id"))
                .SetProjection(Projections.ProjectionList()
                .Add(Projections.Property(nameof(GenerateRequestWithoutOfficersUploadBatchStaging.CreatedAtUtc)), nameof(GenerateRequestWithoutOfficersUploadBatchStagingDTO.CreatedAt))
                .Add(Projections.Property(nameof(GenerateRequestWithoutOfficersUploadBatchStaging.BatchRef)), nameof(GenerateRequestWithoutOfficersUploadBatchStagingDTO.BatchRef))
                .Add(Projections.Property(nameof(GenerateRequestWithoutOfficersUploadBatchStaging.Id)), nameof(GenerateRequestWithoutOfficersUploadBatchStagingDTO.Id))
                .Add(Projections.Property(nameof(GenerateRequestWithoutOfficersUploadBatchStaging.HasGeneratedInvoice)), nameof(GenerateRequestWithoutOfficersUploadBatchStagingDTO.HasGeneratedInvoice))
                ).SetResultTransformer(Transformers.AliasToBean<GenerateRequestWithoutOfficersUploadBatchStagingDTO>())
                .Future<GenerateRequestWithoutOfficersUploadBatchStagingDTO>();
        }


        /// <summary>
        /// Get report model
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, Aggregate }</returns>
        public dynamic GetReportViewModel(GenerateRequestWithoutOfficersUploadBatchStagingReportSearchParams searchParams)
        {
            dynamic returnOBJ = new ExpandoObject();

            try
            {
                returnOBJ.ReportRecords = GetReport(searchParams);
                returnOBJ.Aggregate = GetAggregate(searchParams);
            }
            catch (NoRecordFoundException)
            {
                returnOBJ.ReportRecords = new List<GenerateRequestWithoutOfficersUploadBatchStagingDTO> { };
            }
            return returnOBJ;
        }


        /// <summary>
        /// Creates base criteria query
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public ICriteria GetCriteria(GenerateRequestWithoutOfficersUploadBatchStagingReportSearchParams searchParams)
        {
            var criteria = _transactionManager.GetSession().CreateCriteria<GenerateRequestWithoutOfficersUploadBatchStaging>(nameof(GenerateRequestWithoutOfficersUploadBatchStaging))
                .CreateAlias(nameof(GenerateRequestWithoutOfficersUploadBatchStaging.TaxEntityProfileLocation), nameof(TaxEntityProfileLocation));

            criteria.Add(Restrictions.Eq(nameof(TaxEntityProfileLocation) + "." + nameof(TaxEntityProfileLocation.Id), searchParams.TaxEntityProfileLocationId));

            return criteria;
        }
    }
}