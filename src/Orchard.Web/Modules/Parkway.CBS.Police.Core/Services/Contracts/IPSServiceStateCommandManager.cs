using Orchard;
using System.Collections.Generic;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.HelperModels;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSServiceStateCommandManager<PSServiceStateCommand> : IDependency, IBaseManager<PSServiceStateCommand>
    {

        /// <summary>
        /// Get active commands for this state Id and service Id
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="serviceId"></param>
        /// <returns>IEnumerable{CommandVM}</returns>
        IEnumerable<CommandVM> GetActiveCommands(int stateId, int serviceId);

        /// <summary>
        /// Get active command
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="serviceId"></param>
        /// <param name="commandId"></param>
        /// <returns></returns>
        CommandVM GetActiveCommand(int stateId, int serviceId, int commandId);

    }
}
