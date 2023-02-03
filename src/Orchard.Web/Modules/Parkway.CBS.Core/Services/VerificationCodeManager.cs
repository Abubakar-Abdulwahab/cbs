using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Core.Services
{
    public class VerificationCodeManager : BaseManager<VerificationCode>, IVerificationCodeManager<VerificationCode>
    {
        private readonly IRepository<VerificationCode> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public VerificationCodeManager(IRepository<VerificationCode> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _transactionManager = orchardServices.TransactionManager;
            _repository = repository;
            _orchardServices = orchardServices;
            _user = user;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Get model for token regeneration
        /// </summary>
        /// <param name="verId"></param>
        /// <returns>ModelForTokenRegeneration</returns>
        public ModelForTokenRegeneration GetCBSUserDetailsWithVerificationId(long verId)
        {
            return _transactionManager.GetSession().Query<VerificationCode>()
                 .Where(u => u.Id == verId).Take(1)
                 .Select(u => new ModelForTokenRegeneration
                 {
                     TaxPayerProfileVM = new TaxEntityViewModel
                     {
                         Recipient = u.CBSUser.TaxEntity.Recipient,
                         PhoneNumber = u.CBSUser.TaxEntity.PhoneNumber,
                         Email = u.CBSUser.TaxEntity.Email
                     },
                     CBSUserVM = new CBSUserVM { Name = u.CBSUser.Name, Id = u.CBSUser.Id, Verified = u.CBSUser.Verified, Email = u.CBSUser.Email, PhoneNumber = u.CBSUser.PhoneNumber, TaxEntity = new TaxEntityViewModel { Id = u.CBSUser.TaxEntity.Id } }
                 }).ToList().FirstOrDefault();
        }


        /// <summary>
        /// Gets verification code with specified id
        /// </summary>
        /// <param name="verificationCodeId"></param>
        /// <returns>VerificationCodeVM</returns>
        public VerificationCodeVM GetVerificationCode(long verificationCodeId)
        {
            try
            {
                return _transactionManager.GetSession().Query<VerificationCode>().
                    Where(u => u.Id == verificationCodeId)
                    .Select(u => new VerificationCodeVM { Id = u.Id, CBSUserVM = new CBSUserVM { Id = u.CBSUser.Id, Name = u.CBSUser.Name, PhoneNumber = u.CBSUser.PhoneNumber, IsAdministrator = u.CBSUser.IsAdministrator, Verified = u.CBSUser.Verified }, ResendCount = u.ResendCount, CreatedAtUtc = u.CreatedAtUtc }).SingleOrDefault();
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Gets verification code with specified id
        /// </summary>
        /// <param name="verificationCodeId"></param>
        /// <param name="verificationType"></param>
        /// <returns>VerificationCodeVM</returns>
        public VerificationCodeVM GetVerificationCode(long verificationCodeId, VerificationType verificationType)
        {
            try
            {
                return _transactionManager.GetSession().Query<VerificationCode>().
                    Where(u => ((u.Id == verificationCodeId) && (u.VerificationType == (int)verificationType)))
                    .Select(u => new VerificationCodeVM { Id = u.Id, CBSUserVM = new CBSUserVM { Id = u.CBSUser.Id, Name = u.CBSUser.Name, PhoneNumber = u.CBSUser.PhoneNumber, IsAdministrator = u.CBSUser.IsAdministrator, Verified = u.CBSUser.Verified }, ResendCount = u.ResendCount, CreatedAtUtc = u.CreatedAtUtc, VerificationType = u.VerificationType }).SingleOrDefault();
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Gets total resend count of code for specified verification type for cbs user with specified id within from and to
        /// </summary>
        /// <param name="cbsUserId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>int</returns>
        public int GetResendCountForTimePeriod(long cbsUserId, Models.Enums.VerificationType verificationType, System.DateTime from, System.DateTime to)
        {
            try
            {
                IEnumerable<int> verificationCodes = _transactionManager.GetSession().Query<VerificationCode>().Where(x => (x.CBSUser.Id == cbsUserId) && (x.VerificationType == (int)verificationType) && (x.CreatedAtUtc >= from) && (x.CreatedAtUtc <= to)).Select(x => x.ResendCount);

                if (verificationCodes == null || !verificationCodes.Any()) { return 0; }

                return verificationCodes.Sum(x => x);
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Updates resend count for verification code with specified id
        /// </summary>
        /// <param name="verificationCodeId"></param>
        /// <param name="resendCount"></param>
        public void UpdateVerificationCodeResendCount(long verificationCodeId, int resendCount)
        {
            try
            {
                string tableName = "Parkway_CBS_Core_" + typeof(VerificationCode).Name;

                var queryText = $"UPDATE {tableName} SET {nameof(VerificationCode.ResendCount)} = :resendCount, {nameof(VerificationCode.UpdatedAtUtc)} = :updateDate WHERE {nameof(VerificationCode.Id)} = :verificationCodeId";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("updateDate", System.DateTime.Now.ToLocalTime());
                query.SetParameter("resendCount", resendCount);
                query.SetParameter("verificationCodeId", verificationCodeId);

                query.ExecuteUpdate();
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

    }

}