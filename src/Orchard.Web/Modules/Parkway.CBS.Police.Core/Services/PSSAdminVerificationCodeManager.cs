using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSSAdminVerificationCodeManager : BaseManager<PSSAdminVerificationCode>, IPSSAdminVerificationCodeManager<PSSAdminVerificationCode>
    {
        private readonly IRepository<PSSAdminVerificationCode> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public PSSAdminVerificationCodeManager(IRepository<PSSAdminVerificationCode> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }


        /// <summary>
        /// Gets verification code with specified id
        /// </summary>
        /// <param name="verificationCodeId"></param>
        /// <returns>PSSAdminVerificationCodeVM</returns>
        public PSSAdminVerificationCodeVM GetVerificationCode(long verificationCodeId)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSSAdminVerificationCode>().
                    Where(u => u.Id == verificationCodeId)
                    .Select(u => new PSSAdminVerificationCodeVM { Id = u.Id, AdminUser = new VM.PSSAdminUsersVM { Id = u.AdminUser.Id, Fullname = u.AdminUser.Fullname, PhoneNumber = u.AdminUser.PhoneNumber, Email = u.AdminUser.Email}, ResendCount = u.ResendCount, CreatedAtUtc = u.CreatedAtUtc }).SingleOrDefault();
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Gets total resend count of code for specified verification type for pss admin user with specified id within from and to
        /// </summary>
        /// <param name="adminUserId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>int</returns>
        public int GetResendCountForTimePeriod(int adminUserId, CBS.Core.Models.Enums.VerificationType verificationType, System.DateTime from, System.DateTime to)
        {
            try
            {
                IEnumerable<int> verificationCodes = _transactionManager.GetSession().Query<PSSAdminVerificationCode>().Where(x => (x.AdminUser.Id == adminUserId) && (x.VerificationType == (int)verificationType) && (x.CreatedAtUtc >= from) && (x.CreatedAtUtc <= to)).Select(x => x.ResendCount);

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
                string tableName = "Parkway_CBS_Police_Core_" + typeof(PSSAdminVerificationCode).Name;

                var queryText = $"UPDATE {tableName} SET {nameof(PSSAdminVerificationCode.ResendCount)} = :resendCount, {nameof(PSSAdminVerificationCode.UpdatedAtUtc)} = :updateDate WHERE {nameof(PSSAdminVerificationCode.Id)} = :verificationCodeId";
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