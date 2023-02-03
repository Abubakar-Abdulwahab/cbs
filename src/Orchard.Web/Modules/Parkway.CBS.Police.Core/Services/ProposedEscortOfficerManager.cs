using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.Services
{
    public class ProposedEscortOfficerManager : BaseManager<ProposedEscortOfficer>, IProposedEscortOfficerManager<ProposedEscortOfficer>
    {
        private readonly IRepository<ProposedEscortOfficer> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public ProposedEscortOfficerManager(IRepository<ProposedEscortOfficer> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }



        /// <summary>
        /// Get the details of the proposed officers
        /// </summary>
        /// <param name="escortDetailsId"></param>
        /// <returns>IEnumerable{ProposedEscortOffficerVM}</returns>
        public IEnumerable<ProposedEscortOffficerVM> GetProposedOfficersCollection(long escortDetailsId)
        {
            return _transactionManager.GetSession().Query<ProposedEscortOfficer>()
                .Where(peo => (peo.EscortDetails == new PSSEscortDetails { Id = escortDetailsId }))
                .Select(peo => new ProposedEscortOffficerVM
                {
                   PoliceOfficerLogId = peo.OfficerLog.Id,
                   OfficerRankId = peo.OfficerLog.Rank.Id,
                   OfficerCommandId = peo.OfficerLog.Command.Id,
                   OfficerName = peo.OfficerLog.Name,
                   DeploymentRate = peo.EscortRankRate
                });
        }


        /// <summary>
        /// Get the details of the proposed officers from the specified command
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>IEnumerable{ProposedEscortOffficerVM}</returns>
        public IEnumerable<ProposedEscortOffficerVM> GetProposedOfficersCollection(long requestId, int commandId)
        {
            return _transactionManager.GetSession().Query<EscortFormationOfficer>()
                .Where(peo => peo.Group.Request == new PSSRequest { Id = requestId } && peo.PoliceOfficerLog.Command == new Command { Id = commandId } && !peo.IsDeleted)
                .Select(peo => new ProposedEscortOffficerVM
                {
                    OfficerRankName = peo.PoliceOfficerLog.Rank.RankName,
                    OfficerName = peo.PoliceOfficerLog.Name,
                    OfficerIdentificationNumber = peo.PoliceOfficerLog.IdentificationNumber,
                    DateCreated = peo.CreatedAtUtc.ToString("dd/MM/yyyy HH:mm tt"),
                    OfficerCommandName = peo.PoliceOfficerLog.Command.Name
                });
        }

    }
}