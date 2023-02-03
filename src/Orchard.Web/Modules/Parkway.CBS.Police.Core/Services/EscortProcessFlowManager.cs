using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class EscortProcessFlowManager : BaseManager<EscortProcessFlow>, IEscortProcessFlowManager<EscortProcessFlow>
    {
        private readonly IRepository<EscortProcessFlow> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;
        public ILogger Logger { get; set; }

        public EscortProcessFlowManager(IRepository<EscortProcessFlow> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Get the process flow object for this admin user
        /// </summary>
        /// <param name="userPartId"></param>
        /// <param name="commandTypeId"></param>
        /// <returns>List{EscortProcessFlowDTO}</returns>
        public List<EscortProcessFlowDTO> GetProcessFlowObject(int userPartId, int commandTypeId)
        {
            try
            {
                return _transactionManager.GetSession().Query<EscortProcessFlow>().Where(x => ((x.IsActive) && (x.CommandType == new CommandType { Id = commandTypeId }) && (x.AdminUser.User.Id ==  userPartId)))
                .Select(x => new EscortProcessFlowDTO
                {
                    LevelId = x.Level.Id,
                    LevelGrpId = x.Level.LevelGroupIdentifier
                }).ToList();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Exception getting the EscortProcessFlow for user Id " + userPartId);
                throw;
            }
        }


        /// <summary>
        /// Gets role of admin attached to escort process flow with the specified escort process stage definition id
        /// </summary>
        /// <param name="processStageDefinitionId"></param>
        /// <param name="commandTypeId"></param>
        /// <returns></returns>
        public int GetRoleFromAdminInProcessFlowWithProcessStageDefinition(int processStageDefinitionId, int commandTypeId)
        {
            try
            {
                return _transactionManager.GetSession().Query<EscortProcessFlow>().Where(x => (x.IsActive) && (x.Level.Id == processStageDefinitionId) && (x.CommandType.Id == commandTypeId)).Select(x => x.AdminUser.RoleType.Id).FirstOrDefault();
            }catch(Exception exception)
            {
                Logger.Error(exception, $"Exception getting the Role for the admin attached to the process flow of the specified process stage definition with id {processStageDefinitionId}");
                throw;
            }
        }

    }
}