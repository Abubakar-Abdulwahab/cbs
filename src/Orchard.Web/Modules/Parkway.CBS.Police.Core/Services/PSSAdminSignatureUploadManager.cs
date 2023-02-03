using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSSAdminSignatureUploadManager : BaseManager<PSSAdminSignatureUpload>, IPSSAdminSignatureUploadManager<PSSAdminSignatureUpload>
    {
        private readonly IRepository<PSSAdminSignatureUpload> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public ILogger Logger { get; set; }

        public PSSAdminSignatureUploadManager(IRepository<PSSAdminSignatureUpload> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }


        /// <summary>
        /// Gets all admin signatures
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public IEnumerable<PSSAdminSignatureUploadVM> GetAdminSignatures(int userId, int take, int skip, DateTime start, DateTime end)
        {
            return _transactionManager.GetSession().Query<PSSAdminSignatureUpload>().Where(x => (x.AddedBy.Id == userId) && (x.CreatedAtUtc >= start) && (x.CreatedAtUtc <= end))
                .OrderByDescending(x => x.Id)
                .Skip(skip)
                .Take(take)
                .Select(x => new PSSAdminSignatureUploadVM { SignatureBlob = x.SignatureBlob, SignatureContentType = x.SignatureContentType, SignatureFileName = x.SignatureFileName, createdAt = x.CreatedAtUtc, IsActive = x.IsActive, Id = x.Id, UpdatedAt = x.UpdatedAtUtc });
        }


        /// <summary>
        /// Deactivates all the uploaded signatures for the admin with the specified user id
        /// </summary>
        /// <param name="userId"></param>
        public void DeactivateExisitingSignaturesForAdmin(int userId)
        {
            try
            {
                string tableName = "Parkway_CBS_Police_Core_" + typeof(PSSAdminSignatureUpload).Name;
                string addedByColumnName = nameof(PSSAdminSignatureUpload.AddedBy)+"_Id";
                string updatedAtColumnName = nameof(PSSAdminSignatureUpload.UpdatedAtUtc);
                string IsActiveColumnName = nameof(PSSAdminSignatureUpload.IsActive);

                var queryText = $"UPDATE {tableName} SET {IsActiveColumnName} = :isActive, {updatedAtColumnName} = :updateDate WHERE {addedByColumnName} = :adminId";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("adminId", userId);
                query.SetParameter("isActive", false);

                query.ExecuteUpdate();
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Gets total number of signatures uploaded by admin
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public int GetNumberOfUploadedSignatures(int userId, DateTime start, DateTime end)
        {
            return _transactionManager.GetSession().Query<PSSAdminSignatureUpload>().Count(x => (x.AddedBy.Id == userId) && (x.CreatedAtUtc >= start) && (x.CreatedAtUtc <= end));
        }


        /// <summary>
        /// Updates the status of admin signature with the specified id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="adminSignatureId"></param>
        /// <param name="status"></param>
        public void UpdateSignatureStatus(int userId, int adminSignatureId, bool status)
        {
            try
            {
                string tableName = "Parkway_CBS_Police_Core_" + typeof(PSSAdminSignatureUpload).Name;
                string addedByColumnName = nameof(PSSAdminSignatureUpload.AddedBy) + "_Id";
                string updatedAtColumnName = nameof(PSSAdminSignatureUpload.UpdatedAtUtc);
                string IsActiveColumnName = nameof(PSSAdminSignatureUpload.IsActive);
                string IdColumnName = nameof(PSSAdminSignatureUpload.Id);

                var queryText = $"UPDATE {tableName} SET {IsActiveColumnName} = :isActive, {updatedAtColumnName} = :updateDate WHERE {addedByColumnName} = :adminId AND {IdColumnName} = :signatureId";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("adminId", userId);
                query.SetParameter("isActive", status);
                query.SetParameter("signatureId", adminSignatureId);

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Gets the active signature of admin with specified id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PSSAdminSignatureUploadVM GetActiveAdminSignature(int userId)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSSAdminSignatureUpload>()
                    .Where(x => (x.IsActive) && (x.AddedBy.Id == userId))
                    .Select(x => new PSSAdminSignatureUploadVM
                    {
                        SignatureBlob = x.SignatureBlob,
                        SignatureContentType = x.SignatureContentType
                    }).SingleOrDefault();
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}