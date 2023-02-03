using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSSEscortSettingsManager : BaseManager<PSSEscortSettings>, IPSSEscortSettingsManager<PSSEscortSettings>
    {
        private readonly IRepository<PSSEscortSettings> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public ILogger Logger { get; set; }


        public PSSEscortSettingsManager(IRepository<PSSEscortSettings> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }


        /// <summary>
        /// Check if admin can assign officers
        /// </summary>
        /// <param name="adminUserId"></param>
        /// <param name="flowDefinitionId"></param>
        /// <returns>bool</returns>
        public bool CanAdminAssignOfficers(int adminUserId, int flowDefinitionId)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSSEscortSettings>()
                    .Count(sett => ((sett.WorkFlowDefinition == new PSServiceRequestFlowDefinition { Id = flowDefinitionId }) && (sett.AdminToAssignOfficers == new UserPartRecord { Id = adminUserId }))) > 0;
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Get the settings Id for this flow definition
        /// </summary>
        /// <param name="workFlowdefinitionId"></param>
        /// <returns>int</returns>
        public int GetEscortSettingsId(int workFlowdefinitionId)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSSEscortSettings>()
                    .Where(sett => (sett.WorkFlowDefinition == new PSServiceRequestFlowDefinition { Id = workFlowdefinitionId }))
                    .Select(sett => sett.Id).FirstOrDefault();
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

    }
}