using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.DataFilters.PSSBranchSubUsersUpload.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Parkway.CBS.Police.Core.DataFilters.PSSBranchSubUsersUpload
{
    public class PSSBranchSubUsersUploadBatchItemsStagingFilter : IPSSBranchSubUsersUploadBatchItemsStagingFilter
    {
        private readonly ITransactionManager _transactionManager;

        public PSSBranchSubUsersUploadBatchItemsStagingFilter(IOrchardServices orchardService)
        {
            _transactionManager = orchardService.TransactionManager;
        }

        /// <summary>
        /// Get veiw model for PSS Branch Sub Users Upload Batch Items
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, Aggregate, ValidItemsAggregate }</returns>
        public dynamic GetReportViewModel(PSSBranchSubUsersUploadBatchItemsSearchParams searchParams)
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


        /// Get the aggregate of the branch sub user batch items
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        private IEnumerable<ReportStatsVM> GetTotalRecordCount(PSSBranchSubUsersUploadBatchItemsSearchParams searchParams)
        {
            var query = GetCriteria(searchParams);

            return query
                .SetProjection(
               Projections.ProjectionList()
                    .Add(Projections.Count<PSSBranchSubUsersUploadBatchItemsStaging>(x => x.Id), "TotalRecordCount")
               ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }

        /// Get the aggregate of the valid branch sub user batch items
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        private IEnumerable<FileUploadReport> GetTotalValidRecordCount(PSSBranchSubUsersUploadBatchItemsSearchParams searchParams)
        {
            var query = GetCriteria(searchParams);

            return query.Add(Restrictions.Eq(nameof(PSSBranchSubUsersUploadBatchItemsStaging.HasError), false))
                .SetProjection(
               Projections.ProjectionList()
                    .Add(Projections.Count<PSSBranchSubUsersUploadBatchItemsStaging>(x => x.Id), nameof(FileUploadReport.NumberOfValidRecords))
               ).SetResultTransformer(Transformers.AliasToBean<FileUploadReport>()).Future<FileUploadReport>();
        }


        /// <summary>
        /// Get pss branch sub users items report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <returns>IEnumerable{PSSBranchSubUsersUploadBatchItemsStagingDTO}</returns>
        private IEnumerable<PSSBranchSubUsersUploadBatchItemsStagingDTO> GetReport(PSSBranchSubUsersUploadBatchItemsSearchParams searchParams)
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
                .Add(Projections.Property(nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchStateValue)), nameof(PSSBranchSubUsersUploadBatchItemsStagingDTO.BranchStateValue))
                .Add(Projections.Property(nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchLGAValue)), nameof(PSSBranchSubUsersUploadBatchItemsStagingDTO.BranchLGAValue))
                .Add(Projections.Property(nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchAddress)), nameof(PSSBranchSubUsersUploadBatchItemsStagingDTO.BranchAddress))
                .Add(Projections.Property(nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchName)), nameof(PSSBranchSubUsersUploadBatchItemsStagingDTO.BranchName))
                .Add(Projections.Property(nameof(PSSBranchSubUsersUploadBatchItemsStaging.SubUserName)), nameof(PSSBranchSubUsersUploadBatchItemsStagingDTO.SubUserName))
                .Add(Projections.Property(nameof(PSSBranchSubUsersUploadBatchItemsStaging.SubUserEmail)), nameof(PSSBranchSubUsersUploadBatchItemsStagingDTO.SubUserEmail))
                .Add(Projections.Property(nameof(PSSBranchSubUsersUploadBatchItemsStaging.SubUserPhoneNumber)), nameof(PSSBranchSubUsersUploadBatchItemsStagingDTO.SubUserPhoneNumber))
                .Add(Projections.Property(nameof(PSSBranchSubUsersUploadBatchItemsStaging.HasError)), nameof(PSSBranchSubUsersUploadBatchItemsStagingDTO.HasError))
                .Add(Projections.Property(nameof(PSSBranchSubUsersUploadBatchItemsStaging.ErrorMessage)), nameof(PSSBranchSubUsersUploadBatchItemsStagingDTO.ErrorMessage))
                ).SetResultTransformer(Transformers.AliasToBean<PSSBranchSubUsersUploadBatchItemsStagingDTO>())
                .Future<PSSBranchSubUsersUploadBatchItemsStagingDTO>();
        }


        public ICriteria GetCriteria(PSSBranchSubUsersUploadBatchItemsSearchParams searchParams)
        {
            var session = _transactionManager.GetSession();
            var criteria = session.CreateCriteria<PSSBranchSubUsersUploadBatchItemsStaging>(nameof(PSSBranchSubUsersUploadBatchItemsStaging))
                .CreateAlias(nameof(PSSBranchSubUsersUploadBatchItemsStaging.PSSBranchSubUsersUploadBatchStaging), nameof(PSSBranchSubUsersUploadBatchStaging));

            criteria
                .Add(Restrictions.Eq($"{nameof(PSSBranchSubUsersUploadBatchItemsStaging.PSSBranchSubUsersUploadBatchStaging)}.{nameof(PSSBranchSubUsersUploadBatchStaging.Id)}", searchParams.BatchId));

            return criteria;
        }
    }
}