using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using System.Linq;
using Orchard.Logging;

namespace Parkway.CBS.Police.Core.Services
{
    public class GenerateRequestWithoutOfficersUploadBatchStagingManager : BaseManager<GenerateRequestWithoutOfficersUploadBatchStaging>, IGenerateRequestWithoutOfficersUploadBatchStagingManager<GenerateRequestWithoutOfficersUploadBatchStaging>
    {
        private readonly IRepository<GenerateRequestWithoutOfficersUploadBatchStaging> _repo;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public GenerateRequestWithoutOfficersUploadBatchStagingManager(IRepository<GenerateRequestWithoutOfficersUploadBatchStaging> repo, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repo, user, orchardServices)
        {
            _repo = repo;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Gets GenerateRequestWithoutOfficersUploadBatch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        public GenerateRequestWithoutOfficersUploadBatchStagingDTO GetGenerateRequestWithoutOfficersUploadBatchWithId(Int64 batchId)
        {
            return _transactionManager.GetSession().Query<GenerateRequestWithoutOfficersUploadBatchStaging>().Where(x => x.Id == batchId).Select(x => new GenerateRequestWithoutOfficersUploadBatchStagingDTO
            {
                TaxEntityProfileLocation = new CBS.Core.HelperModels.TaxEntityProfileLocationVM
                {
                    TaxEntity = new CBS.Core.HelperModels.TaxEntityViewModel
                    {
                        Id = x.TaxEntityProfileLocation.TaxEntity.Id,
                        Recipient = x.TaxEntityProfileLocation.TaxEntity.Recipient,
                        PhoneNumber = x.TaxEntityProfileLocation.TaxEntity.PhoneNumber,
                        CategoryName = x.TaxEntityProfileLocation.TaxEntity.TaxEntityCategory.Name,
                        Email = x.TaxEntityProfileLocation.TaxEntity.Email,
                        Address = x.TaxEntityProfileLocation.TaxEntity.Address,
                        PayerId = x.TaxEntityProfileLocation.TaxEntity.PayerId,
                        CategoryId = x.TaxEntityProfileLocation.TaxEntity.TaxEntityCategory.Id
                    },
                    Id = x.TaxEntityProfileLocation.Id,
                    Name = x.TaxEntityProfileLocation.Name,
                    Address = x.TaxEntityProfileLocation.Address,
                    Code = x.TaxEntityProfileLocation.Code
                },
                BatchRef = x.BatchRef,
                Status = x.Status,
                HasError = x.HasError,
                ErrorMessage = x.ErrorMessage
            }).SingleOrDefault();
        }


        /// <summary>
        /// Gets GenerateRequestWithoutOfficersUploadBatch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        public GenerateRequestWithoutOfficersUploadBatchStagingDTO GetGenerateRequestWithoutOfficersUploadBatchStatusInfoWithId(Int64 batchId)
        {
            return _transactionManager.GetSession().Query<GenerateRequestWithoutOfficersUploadBatchStaging>().Where(x => x.Id == batchId).Select(x => new GenerateRequestWithoutOfficersUploadBatchStagingDTO
            {
                Status = x.Status,
                HasError = x.HasError,
                ErrorMessage = x.ErrorMessage
            }).SingleOrDefault();
        }


        /// <summary>
        /// Gets tax entity profile location id and tax entity id for batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        public GenerateRequestWithoutOfficersUploadBatchStagingDTO GetTaxEntityProfileLocationIdAndTaxEntityIdForBatch(Int64 batchId)
        {
            return _transactionManager.GetSession().Query<GenerateRequestWithoutOfficersUploadBatchStaging>().Where(x => x.Id == batchId).Select(x => new GenerateRequestWithoutOfficersUploadBatchStagingDTO
            {
                TaxEntityProfileLocation = new CBS.Core.HelperModels.TaxEntityProfileLocationVM
                {
                    Id = x.TaxEntityProfileLocation.Id,
                    TaxEntity = new CBS.Core.HelperModels.TaxEntityViewModel
                    {
                        Id = x.TaxEntityProfileLocation.TaxEntity.Id
                    }
                }
            }).SingleOrDefault();
        }


        /// <summary>
        /// Updates batch with specified id setting HasGeneratedInvoice flag to true and status to completed
        /// </summary>
        /// <param name="batchId"></param>
        public void UpdateInvoiceGenerationStatusForBatchWithId(long batchId)
        {
            try
            {
                string tableName = "Parkway_CBS_Police_Core_" + typeof(GenerateRequestWithoutOfficersUploadBatchStaging).Name;

                var queryText = $"UPDATE {tableName} SET {nameof(GenerateRequestWithoutOfficersUploadBatchStaging.HasGeneratedInvoice)} = :boolval, {nameof(GenerateRequestWithoutOfficersUploadBatchStaging.UpdatedAtUtc)} = :updateDate, {nameof(GenerateRequestWithoutOfficersUploadBatchStaging.Status)} = :status WHERE {nameof(GenerateRequestWithoutOfficersUploadBatchStaging.Id)} = :batchId";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("boolval", true);
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("batchId", batchId);
                query.SetParameter("status", (int)GenerateRequestWithoutOfficersUploadStatus.Completed);

                query.ExecuteUpdate();

            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}