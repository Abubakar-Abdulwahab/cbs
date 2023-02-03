using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Police.Core.DataFilters.PSSBranchOfficersUploadBatchStagingReport.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Parkway.CBS.Police.Core.DataFilters.PSSBranchOfficersUploadBatchStagingReport
{
    public class PSSBranchOfficersUploadBatchStagingFilter : IPSSBranchOfficersUploadBatchStagingFilter
    {
        private readonly ITransactionManager _transactionManager;

        public PSSBranchOfficersUploadBatchStagingFilter(IOrchardServices orchardService)
        {
            _transactionManager = orchardService.TransactionManager;
        }

        /// <summary>
        /// Get aggreagate for the search param
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>IEnumerable<ReportStatsVM></returns>
        public IEnumerable<ReportStatsVM> GetAggregate(PSSBranchOfficersUploadBatchStagingReportSearchParams searchParams)
        {
            return GetCriteria(searchParams).SetProjection(
                Projections.ProjectionList()
                    .Add(Projections.Count<PSSBranchOfficersUploadBatchStaging>(x => x.Id), nameof(ReportStatsVM.TotalRecordCount))
            ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }


        /// <summary>
        /// Get branch officers upload report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>IEnumerable<PSSBranchOfficersUploadBatchStagingVM></returns>
        public IEnumerable<PSSBranchOfficersUploadBatchStagingVM> GetReport(PSSBranchOfficersUploadBatchStagingReportSearchParams searchParams)
        {
            ICriteria criteria = GetCriteria(searchParams);

            if (!searchParams.DontPageData)
            {
                criteria.SetFirstResult(searchParams.Skip).SetMaxResults(searchParams.Take);
            }

            return criteria.AddOrder(Order.Desc("Id"))
                .SetProjection(Projections.ProjectionList()
                .Add(Projections.Property(nameof(PSSBranchOfficersUploadBatchStaging.CreatedAtUtc)), nameof(PSSBranchOfficersUploadBatchStagingVM.CreatedAt))
                .Add(Projections.Property(nameof(PSSBranchOfficersUploadBatchStaging.BatchRef)), nameof(PSSBranchOfficersUploadBatchStagingVM.BatchReference))
                .Add(Projections.Property(nameof(PSSBranchOfficersUploadBatchStaging.Status)), nameof(PSSBranchOfficersUploadBatchStagingVM.Status))
                .Add(Projections.Property(nameof(PSSBranchOfficersUploadBatchStaging.Id)), nameof(PSSBranchOfficersUploadBatchStagingVM.Id))
                .Add(Projections.Property($"{nameof(PSSBranchOfficersUploadBatchStaging.TaxEntityProfileLocation)}.{nameof(PSSBranchOfficersUploadBatchStaging.TaxEntityProfileLocation.Id)}"), nameof(PSSBranchOfficersUploadBatchStagingVM.TaxProfileLocationId))
                .Add(Projections.Property(nameof(PSSBranchOfficersUploadBatchStaging.HasGeneratedInvoice)), nameof(PSSBranchOfficersUploadBatchStagingVM.HasGeneratedInvoice))
                ).SetResultTransformer(Transformers.AliasToBean<PSSBranchOfficersUploadBatchStagingVM>())
                .Future<PSSBranchOfficersUploadBatchStagingVM>();
        }


        /// <summary>
        /// Get report model
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, Aggregate }</returns>
        public dynamic GetReportViewModel(PSSBranchOfficersUploadBatchStagingReportSearchParams searchParams)
        {
            dynamic returnOBJ = new ExpandoObject();

            try
            {
                returnOBJ.ReportRecords = GetReport(searchParams);
                returnOBJ.Aggregate = GetAggregate(searchParams);
            }
            catch (NoRecordFoundException)
            {
                returnOBJ.ReportRecords = Enumerable.Empty<PSSBranchOfficersUploadBatchStaging>();
            }
            return returnOBJ;
        }


        /// <summary>
        /// Creates base criteria query
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public ICriteria GetCriteria(PSSBranchOfficersUploadBatchStagingReportSearchParams searchParams)
        {
            var criteria = _transactionManager.GetSession().CreateCriteria<PSSBranchOfficersUploadBatchStaging>(nameof(PSSBranchOfficersUploadBatchStaging))
                .CreateAlias(nameof(PSSBranchOfficersUploadBatchStaging.TaxEntityProfileLocation), nameof(PSSBranchOfficersUploadBatchStaging.TaxEntityProfileLocation));

            criteria.Add(Restrictions.Eq(nameof(TaxEntityProfileLocation) + "." + nameof(PSSBranchOfficersUploadBatchStaging.TaxEntityProfileLocation.Id), searchParams.ProfileLocationId));

            return criteria;
        }
    }
}