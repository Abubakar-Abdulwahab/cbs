using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IEscortFormationOfficerManager<EscortFormationOfficer> : IDependency, IBaseManager<EscortFormationOfficer>
    {
        /// <summary>
        /// Gets officers assigned to the request with the specified id
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        IEnumerable<ProposedEscortOffficerVM> GetEscortOfficers(long requestId);


        /// <summary>
        /// Gets rate for officers assigned to the request with the specified id
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        IEnumerable<ProposedEscortOffficerVM> GetEscortOfficersRate(long requestId);

    }
}
