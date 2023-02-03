using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.DataFilters.GenerateRequestWithoutOfficersUploadBatchStagingReport.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Parkway.CBS.Police.Core.DataFilters.GenerateRequestWithoutOfficersUploadBatchStagingReport
{
    public class GenerateRequestWithoutOfficersUploadBatchItemsStagingFilter : IGenerateRequestWithoutOfficersUploadBatchItemsStagingFilter
    {
        private readonly ITransactionManager _transactionManager;

        public GenerateRequestWithoutOfficersUploadBatchItemsStagingFilter(IOrchardServices orchardService)
        {
            _transactionManager = orchardService.TransactionManager;
        }

        /// <summary>
        /// Get veiw model for PSS Branch Sub Users Upload Batch Items
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, Aggregate, ValidItemsAggregate }</returns>
        public dynamic GetReportViewModel(GenerateRequestWithoutOfficersUploadBatchItemsStagingReportSearchParams searchParams)
        {
            dynamic returnOBJ = new ExpandoObject();

            try
            {
                returnOBJ.ReportRecords = GetReport(searchParams);
                returnOBJ.Aggregate = GetTotalRecordCount(searchParams);
                returnOBJ.ValidItemsAggregate = GetTotalValidRecordCount(searchParams);
                return returnOBJ;
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// Get the aggregate of the batch items
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        private IEnumerable<ReportStatsVM> GetTotalRecordCount(GenerateRequestWithoutOfficersUploadBatchItemsStagingReportSearchParams searchParams)
        {
            var query = GetCriteria(searchParams);

            return query
                .SetProjection(
               Projections.ProjectionList()
                    .Add(Projections.Count<GenerateRequestWithoutOfficersUploadBatchItemsStaging>(x => x.Id), "TotalRecordCount")
               ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }


        /// Get the aggregate of the valid batch items
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        private IEnumerable<FileUploadReport> GetTotalValidRecordCount(GenerateRequestWithoutOfficersUploadBatchItemsStagingReportSearchParams searchParams)
        {
            var query = GetCriteria(searchParams);

            return query.Add(Restrictions.Eq(nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.HasError), false))
                .SetProjection(
               Projections.ProjectionList()
                    .Add(Projections.Count<GenerateRequestWithoutOfficersUploadBatchItemsStaging>(x => x.Id), nameof(FileUploadReport.NumberOfValidRecords))
               ).SetResultTransformer(Transformers.AliasToBean<FileUploadReport>()).Future<FileUploadReport>();
        }


        /// <summary>
        /// Get Generate Request Without Officers Upload Batch Items Staging report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <returns>IEnumerable{GenerateRequestWithoutOfficersUploadBatchItemsStagingDTO}</returns>
        private IEnumerable<GenerateRequestWithoutOfficersUploadBatchItemsStagingDTO> GetReport(GenerateRequestWithoutOfficersUploadBatchItemsStagingReportSearchParams searchParams)
        {
            var query = GetCriteria(searchParams);

            if (searchParams.PageData)
            {
                query
                .SetFirstResult(searchParams.Skip)
                .SetMaxResults(searchParams.Take);
            }

            return query
                .AddOrder(Order.Asc("Id"))
                .SetProjection(Projections.ProjectionList()
                .Add(Projections.Property(nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.BranchCode)), nameof(GenerateRequestWithoutOfficersUploadBatchItemsStagingDTO.BranchCode))
                .Add(Projections.Property(nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.NumberOfOfficers)), nameof(GenerateRequestWithoutOfficersUploadBatchItemsStagingDTO.NumberOfOfficers))
                .Add(Projections.Property(nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.NumberOfOfficersValue)), nameof(GenerateRequestWithoutOfficersUploadBatchItemsStagingDTO.NumberOfOfficersValue))
                .Add(Projections.Property(nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.CommandCode)), nameof(GenerateRequestWithoutOfficersUploadBatchItemsStagingDTO.CommandCode))
                .Add(Projections.Property($"{nameof(Command)}.{nameof(Command.Name)}"), nameof(GenerateRequestWithoutOfficersUploadBatchItemsStagingDTO.CommandName))
                .Add(Projections.Property(nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.CommandTypeValue)), nameof(GenerateRequestWithoutOfficersUploadBatchItemsStagingDTO.CommandTypeValue))
                .Add(Projections.Property(nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.DayTypeValue)), nameof(GenerateRequestWithoutOfficersUploadBatchItemsStagingDTO.DayTypeValue))
                .Add(Projections.Property(nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.HasError)), nameof(GenerateRequestWithoutOfficersUploadBatchItemsStagingDTO.HasError))
                .Add(Projections.Property(nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.ErrorMessage)), nameof(GenerateRequestWithoutOfficersUploadBatchItemsStagingDTO.ErrorMessage))
                ).SetResultTransformer(Transformers.AliasToBean<GenerateRequestWithoutOfficersUploadBatchItemsStagingDTO>())
                .Future<GenerateRequestWithoutOfficersUploadBatchItemsStagingDTO>();
        }


        public ICriteria GetCriteria(GenerateRequestWithoutOfficersUploadBatchItemsStagingReportSearchParams searchParams)
        {
            var session = _transactionManager.GetSession();
            var criteria = session.CreateCriteria<GenerateRequestWithoutOfficersUploadBatchItemsStaging>(nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging))
                .CreateAlias(nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.GenerateRequestWithoutOfficersUploadBatchStaging), nameof(GenerateRequestWithoutOfficersUploadBatchStaging))
                .CreateAlias(nameof(GenerateRequestWithoutOfficersUploadBatchItemsStaging.Command), nameof(Command), NHibernate.SqlCommand.JoinType.LeftOuterJoin);

            criteria
                .Add(Restrictions.Eq($"{nameof(GenerateRequestWithoutOfficersUploadBatchStaging)}.{nameof(GenerateRequestWithoutOfficersUploadBatchStaging.Id)}", searchParams.GenerateRequestWithoutOfficersUploadBatchStagingId));

            return criteria;
        }
    }
}