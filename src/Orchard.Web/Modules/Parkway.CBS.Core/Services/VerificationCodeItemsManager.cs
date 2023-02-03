using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System.Linq;

namespace Parkway.CBS.Core.Services
{
    public class VerificationCodeItemsManager : BaseManager<VerificationCodeItems>, IVerificationCodeItemsManager<VerificationCodeItems>
    {
        private readonly IRepository<VerificationCodeItems> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public VerificationCodeItemsManager(IRepository<VerificationCodeItems> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _transactionManager = orchardServices.TransactionManager;
            _repository = repository;
            _orchardServices = orchardServices;
            _user = user;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Update verification code item state
        /// </summary>
        /// <param name="verificationCodeId"></param>
        /// <param name="verificationState"></param>
        public void UpdateVerificationItemState(long verificationCodeId, Models.Enums.VerificationState verificationState)
        {
            try
            {
                string tableName = "Parkway_CBS_Core_" + typeof(VerificationCodeItems).Name;

                var queryText = $"UPDATE {tableName} SET {nameof(VerificationCodeItems.State)} = :used, {nameof(VerificationCodeItems.UpdatedAtUtc)} = :updateDate WHERE {nameof(VerificationCodeItems.VerificationCode)}_Id = :verificationCodeId";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("updateDate", System.DateTime.Now.ToLocalTime());
                query.SetParameter("used", (int)verificationState);
                query.SetParameter("verificationCodeId", verificationCodeId);

                query.ExecuteUpdate();
            }
            catch(System.Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}