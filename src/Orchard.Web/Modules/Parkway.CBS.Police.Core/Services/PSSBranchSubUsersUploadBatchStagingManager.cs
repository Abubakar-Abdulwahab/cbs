using NHibernate.Linq;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSSBranchSubUsersUploadBatchStagingManager : BaseManager<PSSBranchSubUsersUploadBatchStaging>, IPSSBranchSubUsersUploadBatchStagingManager<PSSBranchSubUsersUploadBatchStaging>
    {
        private readonly IRepository<PSSBranchSubUsersUploadBatchStaging> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public PSSBranchSubUsersUploadBatchStagingManager(IRepository<PSSBranchSubUsersUploadBatchStaging> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }

        /// <summary>
        /// Gets branch sub users batch with specified batch Id
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        public PSSBranchSubUsersUploadBatchStagingDTO GetBranchSubUsersBatchWithId(Int64 batchId)
        {
            return _transactionManager.GetSession().Query<PSSBranchSubUsersUploadBatchStaging>().Where(x => x.Id == batchId).Select(x => new PSSBranchSubUsersUploadBatchStagingDTO
            {
                TaxEntity = new CBS.Core.HelperModels.TaxEntityViewModel
                {
                    Id = x.TaxEntity.Id,
                    Recipient = x.TaxEntity.Recipient,
                    PhoneNumber = x.TaxEntity.PhoneNumber,
                    CategoryName = x.TaxEntity.TaxEntityCategory.Name,
                    Email = x.TaxEntity.Email,
                    Address = x.TaxEntity.Address,
                    PayerId = x.TaxEntity.PayerId
                },
                BatchRef = x.BatchRef,
                Status = x.Status,
                HasError = x.HasError,
                ErrorMessage = x.ErrorMessage
            }).SingleOrDefault();
        }

        /// <summary>
        /// Updates status for batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="status"></param>
        public void UpdateStatusForBatchWithId(long batchId, Models.Enums.PSSBranchSubUserUploadStatus status)
        {
            try
            {
                var queryText = $"UPDATE Parkway_CBS_Police_Core_{nameof(PSSBranchSubUsersUploadBatchStaging)} SET {nameof(PSSBranchSubUsersUploadBatchStaging.Status)} = :status WHERE {nameof(PSSBranchSubUsersUploadBatchStaging.Id)} = :batchId;";

                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("status", (int)status);
                query.SetParameter("batchId", batchId);

                query.ExecuteUpdate();
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                RollBackAllTransactions();
                throw;
            }
        }

        /// <summary>
        /// Gets payer id of tax entity attached to batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns>payer id of tax entity attached to the batch</returns>
        public string GetPayerIdForBatchTaxEntity(long batchId)
        {
            return _transactionManager.GetSession().Query<PSSBranchSubUsersUploadBatchStaging>().Where(x => x.Id == batchId).Select(x => x.TaxEntity.PayerId).SingleOrDefault();
        }
    }
}