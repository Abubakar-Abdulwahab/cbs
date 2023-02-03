using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSSBranchOfficersUploadBatchStagingManager : BaseManager<PSSBranchOfficersUploadBatchStaging>, IPSSBranchOfficersUploadBatchStagingManager<PSSBranchOfficersUploadBatchStaging>
    {
        private readonly ITransactionManager _transactionManager;

        public PSSBranchOfficersUploadBatchStagingManager(IRepository<PSSBranchOfficersUploadBatchStaging> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _transactionManager = orchardServices.TransactionManager;
            Logger = NullLogger.Instance;

        }

        /// <summary>
        /// Gets the tax entity profile location attached to batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        public TaxEntityProfileLocationVM GetTaxEntityProfileLocationAttachedToBatchWithId(long batchId)
        {
            return _transactionManager.GetSession().Query<PSSBranchOfficersUploadBatchStaging>()
                .Where(x => x.Id == batchId)
                .Select(x => new TaxEntityProfileLocationVM { Id = x.TaxEntityProfileLocation.Id, Name = x.TaxEntityProfileLocation.Name, TaxEntity = new TaxEntityViewModel { PhoneNumber = x.TaxEntityProfileLocation.TaxEntity.PhoneNumber, Email = x.TaxEntityProfileLocation.TaxEntity.Email, Id = x.TaxEntityProfileLocation.TaxEntity.Id, PayerId = x.TaxEntityProfileLocation.Code, CategoryName = x.TaxEntityProfileLocation.TaxEntity.TaxEntityCategory.Name, CategoryId = x.TaxEntityProfileLocation.TaxEntity.TaxEntityCategory.Id }, Address = x.TaxEntityProfileLocation.Address }).SingleOrDefault();
        }

        /// <summary>
        /// Gets the batch related to the <paramref name="batchId"/>
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        public PSSBranchOfficersUploadBatchStagingVM GetBatchByBatchId(long batchId)
        {
            return _transactionManager.GetSession().Query<PSSBranchOfficersUploadBatchStaging>().Where(x => x.Id == batchId).Select(x => new PSSBranchOfficersUploadBatchStagingVM
            {
                Id = x.Id,
                BatchReference = x.BatchRef,
                CreatedAt = x.CreatedAtUtc,
                Status = x.Status,
                HasError = x.HasError,
                ErrorMessage = x.ErrorMessage
            }).SingleOrDefault();
        }

        /// <summary>
        /// Checks if the batch status is completed for the <paramref name="batchId"/> provided
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns><see cref="true"/> if the batch process is completed, otherwise, <see cref="false"/></returns>
        public bool IsBatchProcessed(long batchId)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSSBranchOfficersUploadBatchStaging>().Count(x => x.Id == batchId && x.Status == (int)PSSBranchOfficersUploadStatus.BatchValidated) > 0;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Updates batch with specified id setting HasGeneratedInvoice flag to true
        /// </summary>
        /// <param name="batchId"></param>
        public void UpdateInvoiceGenerationStatusForBatchWithId(long batchId)
        {
            try
            {
                string tableName = "Parkway_CBS_Police_Core_" + typeof(PSSBranchOfficersUploadBatchStaging).Name;

                var queryText = $"UPDATE {tableName} SET {nameof(PSSBranchOfficersUploadBatchStaging.HasGeneratedInvoice)} = :boolval, {nameof(PSSBranchOfficersUploadBatchStaging.UpdatedAtUtc)} = :updateDate, {nameof(PSSBranchOfficersUploadBatchStaging.Status)} = :status WHERE {nameof(PSSBranchOfficersUploadBatchStaging.Id)} = :batchId";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("boolval", true);
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("batchId", batchId);
                query.SetParameter("status", (int)PSSBranchOfficersUploadStatus.Completed);

                query.ExecuteUpdate();

            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}