using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using NHibernate.Linq;
using System.Linq;
using Parkway.CBS.Police.Core.DTO;

namespace Parkway.CBS.Police.Core.Services
{
    public class EscortSquadAllocationGroupManager : BaseManager<EscortSquadAllocationGroup>, IEscortSquadAllocationGroupManager<EscortSquadAllocationGroup>
    {
        private readonly IRepository<EscortSquadAllocationGroup> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public ILogger Logger { get; set; }


        public EscortSquadAllocationGroupManager(IRepository<EscortSquadAllocationGroup> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }


        /// <summary>
        /// Get the current process stage this application is on
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>EscortSquadAllocationGroupDTO</returns>
        public EscortSquadAllocationGroupDTO GetProcessStage(Int64 requestId)
        {
            try
            {
                return _transactionManager.GetSession().Query<EscortSquadAllocationGroup>()
                    .Where(x => ((x.Request.Id == requestId)))
                    .Select(r => new EscortSquadAllocationGroupDTO { Id = r.Id, Fulfilled = r.Fulfilled, RequestLevel = new EscortProcessStageDefinitionDTO { LevelGrpId = r.RequestLevel.LevelGroupIdentifier } }).SingleOrDefault();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Exception getting the EscortSquadAllocationGroup for request Id " + requestId);
                throw;
            }
        }

    }
}