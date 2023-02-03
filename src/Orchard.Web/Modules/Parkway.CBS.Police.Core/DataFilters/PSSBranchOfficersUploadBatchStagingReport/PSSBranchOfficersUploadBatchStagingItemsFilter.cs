using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.DataFilters.PSSBranchOfficersUploadBatchStagingReport.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Parkway.CBS.Police.Core.DataFilters.PSSBranchOfficersUploadBatchItemsStagingReport
{
    public class PSSBranchOfficersUploadBatchItemsStagingFilter : IPSSBranchOfficersUploadBatchItemsStagingFilter
    {
        private readonly ITransactionManager _transactionManager;

        public PSSBranchOfficersUploadBatchItemsStagingFilter(IOrchardServices orchardService)
        {
            _transactionManager = orchardService.TransactionManager;
        }

        /// <summary>
        /// Get aggreagate for the search param
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>IEnumerable<ReportStatsVM></returns>
        public IEnumerable<ReportStatsVM> GetAggregate(PSSBranchOfficersUploadBatchItemsStagingReportSearchParams searchParams)
        {
            return GetCriteria(searchParams).SetProjection(
                Projections.ProjectionList()
                    .Add(Projections.Count<PSSBranchOfficersUploadBatchItemsStaging>(x => x.Id), nameof(ReportStatsVM.TotalRecordCount))
            ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }


        /// <summary>
        /// Get branch officers upload report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>IEnumerable<PSSBranchOfficersUploadBatchItemsStagingVM></returns>
        public IEnumerable<PSSBranchOfficersUploadBatchItemsStagingVM> GetReport(PSSBranchOfficersUploadBatchItemsStagingReportSearchParams searchParams)
        {
            ICriteria criteria = GetCriteria(searchParams);

            if (!searchParams.DontPageData)
            {
                criteria.SetFirstResult(searchParams.Skip).SetMaxResults(searchParams.Take);
            }

            return criteria.AddOrder(Order.Asc("Id"))
                .SetProjection(Projections.ProjectionList()
                .Add(Projections.Property(nameof(PSSBranchOfficersUploadBatchItemsStaging.CreatedAtUtc)), nameof(PSSBranchOfficersUploadBatchItemsStagingVM.CreatedAt))
                 .Add(Projections.Property(nameof(PSSBranchOfficersUploadBatchItemsStaging.ErrorMessage)), nameof(PSSBranchOfficersUploadBatchItemsStagingVM.ErrorMessage))
                 .Add(Projections.Property(nameof(PSSBranchOfficersUploadBatchItemsStaging.HasError)), nameof(PSSBranchOfficersUploadBatchItemsStagingVM.HasError))
                 .Add(Projections.Property(nameof(PSSBranchOfficersUploadBatchItemsStaging.APNumber)), nameof(PSSBranchOfficersUploadBatchItemsStagingVM.ServiceNumber))
                 .Add(Projections.Property(nameof(PSSBranchOfficersUploadBatchItemsStaging.OfficerName)), nameof(PSSBranchOfficersUploadBatchItemsStagingVM.Name))
                 .Add(Projections.Property(nameof(PSSBranchOfficersUploadBatchItemsStaging.OfficerCommandValue)), nameof(PSSBranchOfficersUploadBatchItemsStagingVM.Command))
                  .Add(Projections.Property(nameof(PSSBranchOfficersUploadBatchItemsStaging.Id)), nameof(PSSBranchOfficersUploadBatchItemsStagingVM.Id))
                ).SetResultTransformer(Transformers.AliasToBean<PSSBranchOfficersUploadBatchItemsStagingVM>())
                .Future<PSSBranchOfficersUploadBatchItemsStagingVM>();
        }


        /// <summary>
        /// Get report model
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, Aggregate }</returns>
        public dynamic GetReportViewModel(PSSBranchOfficersUploadBatchItemsStagingReportSearchParams searchParams)
        {
            dynamic returnOBJ = new ExpandoObject();

            try
            {
                returnOBJ.ReportRecords = GetReport(searchParams);
                returnOBJ.Aggregate = GetAggregate(searchParams);
                returnOBJ.ValidItemsAggregate = GetTotalValidRecordCount(searchParams);

            }
            catch (NoRecordFoundException)
            {
                returnOBJ.ReportRecords = Enumerable.Empty<PSSBranchOfficersUploadBatchItemsStaging>();
                throw;
            }
            return returnOBJ;
        }

        /// Get the aggregate of the valid branch officer batch items
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        private IEnumerable<FileUploadReport> GetTotalValidRecordCount(PSSBranchOfficersUploadBatchItemsStagingReportSearchParams searchParams)
        {
            var query = GetCriteria(searchParams);

            return query.Add(Restrictions.Eq(nameof(PSSBranchOfficersUploadBatchItemsStaging.HasError), false))
                .SetProjection(
               Projections.ProjectionList()
                    .Add(Projections.Count<PSSBranchOfficersUploadBatchItemsStaging>(x => x.Id), nameof(FileUploadReport.NumberOfValidRecords))
               ).SetResultTransformer(Transformers.AliasToBean<FileUploadReport>()).Future<FileUploadReport>();
        }


        /// <summary>
        /// Creates base criteria query
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public ICriteria GetCriteria(PSSBranchOfficersUploadBatchItemsStagingReportSearchParams searchParams)
        {
            var criteria = _transactionManager.GetSession().CreateCriteria<PSSBranchOfficersUploadBatchItemsStaging>(nameof(PSSBranchOfficersUploadBatchItemsStaging))
                .CreateAlias(nameof(PSSBranchOfficersUploadBatchItemsStaging.PSSBranchOfficersUploadBatchStaging), nameof(PSSBranchOfficersUploadBatchItemsStaging.PSSBranchOfficersUploadBatchStaging));

            criteria.Add(Restrictions.Eq(nameof(PSSBranchOfficersUploadBatchStaging) + "." + nameof(PSSBranchOfficersUploadBatchStaging.Id), searchParams.BatchId));

            return criteria;
        }
    }
}