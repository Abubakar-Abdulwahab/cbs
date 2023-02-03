using System.Collections.Generic;
using Orchard;
using Parkway.CBS.Police.Core.HelperModels;

namespace Parkway.CBS.Police.Core.CoreServices.Contracts
{
    public interface ICoreServiceStateCommand : IDependency
    {

        /// <summary>
        /// Get active commands
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="serviceId"></param>
        /// <returns>IEnumerable{CommandVM}</returns>
        IEnumerable<CommandVM> GetActiveCommands(int stateId, int serviceId);


        /// <summary>
        /// Get the active command with the stateId and service Id
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="commandId"></param>
        /// <param name="serviceId"></param>
        /// <returns>CommandVM</returns>
        CommandVM GetActiveCommand(int stateId, int commandId, int serviceId);

    }
}
