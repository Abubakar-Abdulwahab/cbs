using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;

namespace Parkway.CBS.Police.Core.Services
{
    public class SecretariatRoutingLevelManager : BaseManager<SecretariatRoutingLevel>, ISecretariatRoutingLevelManager<SecretariatRoutingLevel>
    {
        private readonly IRepository<SecretariatRoutingLevel> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public ILogger Logger { get; set; }


        public SecretariatRoutingLevelManager(IRepository<SecretariatRoutingLevel> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }


        /// <summary>
        /// Save secretariat routing level for request with specified id at selected request stage for admin user
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="selectedRequestStage"></param>
        /// <param name="adminUserId"></param>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public bool SaveSecretariatRoutingLevel(long requestId, int selectedRequestStage, int adminUserId, string modelName)
        {
            try
            {
                string secretariatRoutingLevelTableName = "Parkway_CBS_Police_Core_" + typeof(SecretariatRoutingLevel).Name;
                string secretariatRoutingLevelQueryText = $"INSERT INTO {secretariatRoutingLevelTableName}(Request_Id, StageRoutedTo, StageModelName, AdminUser_Id, CreatedAtUtc, UpdatedAtUtc) VALUES({requestId}, {selectedRequestStage}, '{modelName}', {adminUserId}, GETDATE(), GETDATE())";
                _transactionManager.GetSession().CreateSQLQuery(secretariatRoutingLevelQueryText).ExecuteUpdate();
                return true;
            }catch(Exception exception)
            {
                Logger.Error(exception, $"Error when trying to save secretariat routing level. Request Id:{requestId}, Selected Request Stage:{selectedRequestStage}, Admin User Id:{adminUserId}, Model Name:{modelName}");
                throw;
            }
        }
    }
}