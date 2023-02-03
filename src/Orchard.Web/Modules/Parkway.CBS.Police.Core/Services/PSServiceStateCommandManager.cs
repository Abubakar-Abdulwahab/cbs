using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSServiceStateCommandManager : BaseManager<PSServiceStateCommand>, IPSServiceStateCommandManager<PSServiceStateCommand>
    {
        private readonly IRepository<PSServiceStateCommand> _repository;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public PSServiceStateCommandManager(IRepository<PSServiceStateCommand> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
        }


        /// <summary>
        /// Get active commands for this state Id and service Id
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="serviceId"></param>
        /// <returns>IEnumerable{CommandVM}</returns>
        public IEnumerable<CommandVM> GetActiveCommands(int stateId, int serviceId)
        {
            return _transactionManager.GetSession().Query<PSServiceStateCommand>()
                .Where(ssc => ((ssc.ServiceState.State == new StateModel { Id = stateId }) && (ssc.IsActive) && (ssc.ServiceState.Service == new PSService { Id = serviceId }) && (ssc.ServiceState.IsActive)))
               .Select(r => new CommandVM { Id = r.Command.Id, Code = r.Command.Code, Name = r.Command.Name, }).ToList();
        }


        /// <summary>
        /// Get active command
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="serviceId"></param>
        /// <param name="commandId"></param>
        /// <returns>CommandVM</returns>
        public CommandVM GetActiveCommand(int stateId, int serviceId, int commandId)
        {
            return _transactionManager.GetSession().Query<PSServiceStateCommand>()
                .Where(ssc => ((ssc.Command == new Command { Id = commandId }) && (ssc.ServiceState.State == new StateModel { Id = stateId }) && (ssc.IsActive) && (ssc.ServiceState.Service == new PSService { Id = serviceId }) && (ssc.ServiceState.IsActive)))
               .Select(r => new CommandVM { Id = r.Command.Id, Name = r.Command.Name, Code = r.Command.Code, Address = r.Command.Address, LGAName = r.Command.LGA.Name, StateId = r.Command.State.Id, StateName = r.Command.State.Name }).SingleOrDefault();
        }

    }
}