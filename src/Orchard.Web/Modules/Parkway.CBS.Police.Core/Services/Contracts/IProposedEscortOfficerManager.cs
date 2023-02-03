using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IProposedEscortOfficerManager<ProposedEscortOfficer> : IDependency, IBaseManager<ProposedEscortOfficer>
    {

        /// <summary>
        /// Get the details of the proposed officers
        /// </summary>
        /// <param name="escortDetailsId"></param>
        /// <returns>IEnumerable{ProposedEscortOffficerVM}</returns>
        IEnumerable<ProposedEscortOffficerVM> GetProposedOfficersCollection(long escortDetailsId);

        /// <summary>
        /// Get the details of the proposed officers from the specified command
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>IEnumerable{ProposedEscortOffficerVM}</returns>
        IEnumerable<ProposedEscortOffficerVM> GetProposedOfficersCollection(long requestId, int commandId);

    }
}
