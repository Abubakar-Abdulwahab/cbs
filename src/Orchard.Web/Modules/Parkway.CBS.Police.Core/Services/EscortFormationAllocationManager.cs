using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class EscortFormationAllocationManager : BaseManager<EscortFormationAllocation>, IEscortFormationAllocationManager<EscortFormationAllocation>
    {
        private readonly IRepository<EscortFormationAllocation> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;
        public ILogger Logger { get; set; }

        public EscortFormationAllocationManager(IRepository<EscortFormationAllocation> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Gets formations selected to nominate officers by the AIG of the squad with the specified escort squad allocation id
        /// </summary>
        /// <param name="escortSquadAllocationId"></param>
        /// <param name="escortSquadAllocationGroupId"></param>
        /// <returns></returns>
        public IEnumerable<AIGFormationVM> GetFormationsAllocatedToSquad(long escortSquadAllocationId, long escortSquadAllocationGroupId)
        {
            try
            {
                return _transactionManager.GetSession().Query<EscortFormationAllocation>().Where(x => x.EscortSquadAllocation == new EscortSquadAllocation { Id = escortSquadAllocationId } && x.Group == new EscortSquadAllocationGroup { Id = escortSquadAllocationGroupId } && !x.IsDeleted).Select(x => new AIGFormationVM { FormationId = x.Command.Id, FormationName = x.Command.Name, NumberofOfficers = x.NumberOfOfficers, RequestId = x.Group.Request.Id, DateCreated = x.CreatedAtUtc, NumberOfOfficersProvided = x.SquadronOfficers.Count(b => !b.IsDeleted) });
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Gets number of officers requested from the formation with the specified formation allocation id
        /// </summary>
        /// <param name="formationAllocationId"></param>
        /// <param name="allocationGroupId"></param>
        /// <returns></returns>
        public int GetNumberOfOfficersRequestedFromFormation(long formationAllocationId, long allocationGroupId)
        {
            try
            {
                return _transactionManager.GetSession().Query<EscortFormationAllocation>().Where(x => (x.Id == formationAllocationId) && (x.Group.Id == allocationGroupId)).Select(x => x.NumberOfOfficers).SingleOrDefault();
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}