using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSSAdminVerificationCodeItemsManager : BaseManager<PSSAdminVerificationCodeItems>, IPSSAdminVerificationCodeItemsManager<PSSAdminVerificationCodeItems>
    {
        private readonly IRepository<PSSAdminVerificationCodeItems> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public PSSAdminVerificationCodeItemsManager(IRepository<PSSAdminVerificationCodeItems> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }


        /// <summary>
        /// Update verification code item state
        /// </summary>
        /// <param name="verificationCodeId"></param>
        /// <param name="verificationState"></param>
        public void UpdateVerificationItemState(long verificationCodeId, CBS.Core.Models.Enums.VerificationState verificationState)
        {
            try
            {
                string tableName = "Parkway_CBS_Police_Core_" + typeof(PSSAdminVerificationCodeItems).Name;

                var queryText = $"UPDATE {tableName} SET {nameof(PSSAdminVerificationCodeItems.State)} = :used, {nameof(PSSAdminVerificationCodeItems.UpdatedAtUtc)} = :updateDate WHERE {nameof(PSSAdminVerificationCodeItems.VerificationCode)}_Id = :verificationCodeId";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("updateDate", System.DateTime.Now.ToLocalTime());
                query.SetParameter("used", (int)verificationState);
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