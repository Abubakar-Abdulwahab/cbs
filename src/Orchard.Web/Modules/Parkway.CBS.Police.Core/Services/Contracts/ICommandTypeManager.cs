using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface ICommandTypeManager<CommandType> : IDependency, IBaseManager<CommandType>
    {
        /// <summary>
        /// Gets all active command types
        /// </summary>
        /// <returns></returns>
        IEnumerable<CommandTypeVM> GetCommandTypes();

        /// <summary>
        /// Gets command type with specified id
        /// </summary>
        /// <param name="commandTypeId"></param>
        /// <returns></returns>
        CommandTypeVM GetCommandType(int commandTypeId);
    }
}
