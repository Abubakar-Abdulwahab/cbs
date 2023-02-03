using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSSBranchSubUsersUploadBatchItemsStagingManager : BaseManager<PSSBranchSubUsersUploadBatchItemsStaging>, IPSSBranchSubUsersUploadBatchItemsStagingManager<PSSBranchSubUsersUploadBatchItemsStaging>
    {
        private readonly IRepository<PSSBranchSubUsersUploadBatchItemsStaging> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public PSSBranchSubUsersUploadBatchItemsStagingManager(IRepository<PSSBranchSubUsersUploadBatchItemsStaging> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }

        /// <summary>
        /// Creates tax entity profile locations for all branches in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        public void CreateBranches(long batchId)
        {
            try
            {
                var queryText = $"INSERT INTO Parkway_CBS_Core_{nameof(TaxEntityProfileLocation)}({nameof(TaxEntityProfileLocation.Name)}, {nameof(TaxEntityProfileLocation.State)}_Id, {nameof(TaxEntityProfileLocation.LGA)}_Id, {nameof(TaxEntityProfileLocation.Address)}, {nameof(TaxEntityProfileLocation.TaxEntity)}_Id, {nameof(TaxEntityProfileLocation.CreatedAtUtc)}, {nameof(TaxEntityProfileLocation.UpdatedAtUtc)}, {nameof(TaxEntityProfileLocation.IsDefault)}) " +
                    $"SELECT DISTINCT T1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchName)}, T1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchState)}_Id, T1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchLGA)}_Id, T1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchAddress)}, " +
                    $"T2.{nameof(PSSBranchSubUsersUploadBatchStaging.TaxEntity)}_Id, GETDATE(), GETDATE(), 0 " +
                    $"FROM Parkway_CBS_Police_Core_{nameof(PSSBranchSubUsersUploadBatchItemsStaging)} AS T1 INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSBranchSubUsersUploadBatchStaging)} AS T2 " +
                    $"ON T1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.PSSBranchSubUsersUploadBatchStaging)}_Id = T2.{nameof(PSSBranchSubUsersUploadBatchStaging.Id)} " +
                    $"WHERE T1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.PSSBranchSubUsersUploadBatchStaging)}_Id = :batchId AND " +
                    $"T1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.HasError)} = :hasError AND T1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.TaxEntityProfileLocation)}_Id IS NULL";

                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("hasError", false);
                query.SetParameter("batchId", batchId);

                query.ExecuteUpdate();
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Resolves Tax Entity Profile Location Ids for branches that have been created in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        public void ResolveTaxEntityProfileLocationIdsForCreatedBranches(long batchId)
        {
            try
            {
                var queryText = $"UPDATE T1 SET T1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.TaxEntityProfileLocation)}_Id = T2.{nameof(TaxEntityProfileLocation.Id)} " +
                    $"FROM Parkway_CBS_Police_Core_{nameof(PSSBranchSubUsersUploadBatchItemsStaging)} AS T1 INNER JOIN Parkway_CBS_Core_{nameof(TaxEntityProfileLocation)} AS " +
                    $"T2 ON T1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchName)} = T2.{nameof(TaxEntityProfileLocation.Name)} AND " +
                    $"T1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchState)}_Id = T2.{nameof(TaxEntityProfileLocation.State)}_Id AND " +
                    $"T1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchLGA)}_Id = T2.{nameof(TaxEntityProfileLocation.LGA)}_Id WHERE " +
                    $"T1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.PSSBranchSubUsersUploadBatchStaging)}_Id = :batchId AND " +
                    $"T1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.HasError)} = :hasError;";

                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("hasError", false);
                query.SetParameter("batchId", batchId);

                query.ExecuteUpdate();
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Creates cbs users for all sub users in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        public void CreateSubUsersAsCBSUsers(long batchId)
        {
            try
            {
                var queryText = $"INSERT INTO Parkway_CBS_Core_{nameof(CBSUser)}({nameof(CBSUser.Name)}, {nameof(CBSUser.UserPartRecord)}_Id, {nameof(CBSUser.TaxEntity)}_Id, " +
                    $"{nameof(CBSUser.CreatedAtUtc)}, {nameof(CBSUser.UpdatedAtUtc)}, {nameof(CBSUser.PhoneNumber)}, {nameof(CBSUser.Email)}, {nameof(CBSUser.Address)}, " +
                    $"{nameof(CBSUser.IsAdministrator)}) SELECT DISTINCT T1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.SubUserName)}, " +
                    $"T1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.User)}_Id, T2.{nameof(PSSBranchSubUsersUploadBatchStaging.TaxEntity)}_Id, GETDATE(), GETDATE(), " +
                    $"T1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.SubUserPhoneNumber)}, T1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.SubUserEmail)}, " +
                    $"T1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.BranchAddress)}, 0 FROM Parkway_CBS_Police_Core_{nameof(PSSBranchSubUsersUploadBatchItemsStaging)} AS T1 " +
                    $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSBranchSubUsersUploadBatchStaging)} AS T2 ON " +
                    $"T1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.PSSBranchSubUsersUploadBatchStaging)}_Id = T2.{nameof(PSSBranchSubUsersUploadBatchStaging.Id)} " +
                    $"WHERE T1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.HasError)} = :hasError AND " +
                    $"T1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.PSSBranchSubUsersUploadBatchStaging)}_Id = :batchId ";

                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("hasError", false);
                query.SetParameter("batchId", batchId);

                query.ExecuteUpdate();
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Attaches sub users to their respective branches in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        public void AttachSubUsersToBranchLocations(long batchId)
        {
            try
            {
                var queryText = $"INSERT INTO Parkway_CBS_Core_{nameof(CBSUserTaxEntityProfileLocation)}({nameof(CBSUserTaxEntityProfileLocation.CBSUser)}_Id, {nameof(CBSUserTaxEntityProfileLocation.TaxEntityProfileLocation)}_Id, {nameof(CBSUserTaxEntityProfileLocation.CreatedAtUtc)}, {nameof(CBSUserTaxEntityProfileLocation.UpdatedAtUtc)}) SELECT DISTINCT T2.{nameof(CBSUser.Id)}, T1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.TaxEntityProfileLocation)}_Id, GETDATE(), GETDATE() FROM Parkway_CBS_Police_Core_{nameof(PSSBranchSubUsersUploadBatchItemsStaging)} AS T1 INNER JOIN Parkway_CBS_Core_{nameof(CBSUser)} AS T2 ON T1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.User)}_Id = T2.{nameof(CBSUser.UserPartRecord)}_Id WHERE T1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.PSSBranchSubUsersUploadBatchStaging)}_Id = :batchId AND T1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.HasError)} = :hasError;";

                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("hasError", false);
                query.SetParameter("batchId", batchId);

                query.ExecuteUpdate();
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Gets PSS Branch Sub Users Upload Batch Items for batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public IEnumerable<PSSBranchSubUsersUploadBatchItemsStagingDTO> GetItems(long batchId, int skip, int take)
        {
            return _transactionManager.GetSession().Query<PSSBranchSubUsersUploadBatchItemsStaging>()
                .Where(x => x.PSSBranchSubUsersUploadBatchStaging.Id == batchId && !x.HasError)
                .Skip(skip)
                .Take(take)
                .Select(x => new PSSBranchSubUsersUploadBatchItemsStagingDTO { Id = x.Id, SubUserEmail = x.SubUserEmail, SubUserPhoneNumber = x.SubUserPhoneNumber });
        }

        /// <summary>
        /// Performs a bulk update on PSSBranchSubUsersUploadBatchItemsStaging, updating the user id for sub users in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        public void UpdateUserIdsForSubUsersInBatchWithId(long batchId)
        {
            try
            {
                var queryText = $"UPDATE T1 SET T1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.User)}_Id = T2.{nameof(UserPartRecord.Id)} FROM Parkway_CBS_Police_Core_{nameof(PSSBranchSubUsersUploadBatchItemsStaging)} AS T1 INNER JOIN Orchard_Users_{nameof(UserPartRecord)} AS T2 ON T1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.SubUserEmail)} = T2.{nameof(UserPartRecord.Email)} WHERE T1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.PSSBranchSubUsersUploadBatchStaging)}_Id = :batchId AND T1.{nameof(PSSBranchSubUsersUploadBatchItemsStaging.HasError)} = :hasError";

                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("hasError", false);
                query.SetParameter("batchId", batchId);
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