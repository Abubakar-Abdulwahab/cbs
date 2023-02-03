using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class EscortFormationOfficerManager : BaseManager<EscortFormationOfficer>, IEscortFormationOfficerManager<EscortFormationOfficer>
    {
        private readonly IRepository<EscortFormationOfficer> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;
        public ILogger Logger { get; set; }

        public EscortFormationOfficerManager(IRepository<EscortFormationOfficer> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Gets officers assigned to the request with the specified id
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public IEnumerable<ProposedEscortOffficerVM> GetEscortOfficers(long requestId)
        {
            return _transactionManager.GetSession().Query<EscortFormationOfficer>().Where(x => (x.Group.Request.Id == requestId) && !x.IsDeleted).Select(x => new ProposedEscortOffficerVM
            {
                OfficerName = x.PoliceOfficerLog.Name,
                OfficerRankName = x.PoliceOfficerLog.Rank.RankName,
                OfficerRankId = x.PoliceOfficerLog.Rank.Id,
                PoliceOfficerLogId = x.PoliceOfficerLog.Id,
                OfficerCommandId = x.PoliceOfficerLog.Command.Id,
                OfficerCommandName = x.PoliceOfficerLog.Command.Name,
                OfficerCommandAddress = x.PoliceOfficerLog.Command.Address,
                OfficerCommandStateName = x.PoliceOfficerLog.Command.State.Name,
                OfficerCommandLGAName = x.PoliceOfficerLog.Command.LGA.Name,
                OfficerIdentificationNumber = x.PoliceOfficerLog.IdentificationNumber,
                DeploymentRate = x.EscortRankRate,
            });
        }

        /// <summary>
        /// Gets rate for officers assigned to the request with the specified id
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public IEnumerable<ProposedEscortOffficerVM> GetEscortOfficersRate(long requestId)
        {
            return _transactionManager.GetSession().Query<EscortFormationOfficer>().Where(x => (x.Group.Request.Id == requestId) && !x.IsDeleted).Select(x => new ProposedEscortOffficerVM
            {
                DeploymentRate = x.EscortRankRate,
            });
        }

    }
}