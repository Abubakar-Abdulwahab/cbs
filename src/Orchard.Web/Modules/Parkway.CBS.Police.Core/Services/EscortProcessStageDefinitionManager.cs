using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class EscortProcessStageDefinitionManager : BaseManager<EscortProcessStageDefinition>, IEscortProcessStageDefinitionManager<EscortProcessStageDefinition>
    {
        private readonly IRepository<EscortProcessStageDefinition> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;
        public ILogger Logger { get; set; }

        public EscortProcessStageDefinitionManager(IRepository<EscortProcessStageDefinition> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Gets active escort process stage definitions for the specified command type for AIG level upward
        /// </summary>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public IEnumerable<EscortProcessStageDefinitionDTO> GetEscortProcessStageDefinitions(int commandType)
        {
            try
            {
                return _transactionManager.GetSession().Query<EscortProcessStageDefinition>().Where(x => (x.IsActive) && (x.CommandType.Id == commandType) && (x.LevelGroupIdentifier <= 5)).Select(x => new EscortProcessStageDefinitionDTO
                {
                    Id = x.Id,
                    Name = x.Name
                });

            }catch(System.Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Gets active escort process stage definitions for the specified command type
        /// </summary>
        /// <param name="commandType"></param>
        /// <returns>IEnumerable<EscortProcessStageDefinitionDTO></returns>
        public IEnumerable<EscortProcessStageDefinitionDTO> GetAllEscortProcessStageDefinitions(int commandType)
        {
            try
            {
                return _transactionManager.GetSession().Query<EscortProcessStageDefinition>().Where(x => (x.IsActive) && (x.CommandType.Id == commandType)).Select(x => new EscortProcessStageDefinitionDTO
                {
                    Id = x.Id,
                    Name = x.Name
                });

            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }



        /// <summary>
        /// Gets escort process stage definition with specified id and command type
        /// </summary>
        /// <param name="processStageId"></param>
        /// <param name="commandTypeId"></param>
        /// <returns></returns>
        public EscortProcessStageDefinitionDTO GetProcessStageWithCommandTypeAndId(int processStageId, int commandTypeId)
        {
            try
            {
                return _transactionManager.GetSession().Query<EscortProcessStageDefinition>()
                    .Where(x => (x.IsActive) && (x.Id == processStageId) && (x.CommandType.Id == commandTypeId))
                    .Select(x => new EscortProcessStageDefinitionDTO { Id = x.Id, Name = x.Name }).SingleOrDefault();
            }catch(System.Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
       
    }
}