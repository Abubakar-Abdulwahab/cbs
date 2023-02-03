using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.DTO;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IEscortProcessStageDefinitionManager<EscortProcessStageDefinition> : IDependency, IBaseManager<EscortProcessStageDefinition>
    {
        /// <summary>
        /// Gets active escort process stage definitions for the specified command type for AIG level upward
        /// </summary>
        /// <param name="commandType"></param>
        /// <returns></returns>
        IEnumerable<EscortProcessStageDefinitionDTO> GetEscortProcessStageDefinitions(int commandType);

        /// <summary>
        /// Gets active escort process stage definitions for the specified command type
        /// </summary>
        /// <param name="commandType"></param>
        /// <returns>IEnumerable<EscortProcessStageDefinitionDTO></returns>
        IEnumerable<EscortProcessStageDefinitionDTO> GetAllEscortProcessStageDefinitions(int commandType);


        /// <summary>
        /// Gets escort process stage definition with specified id and command type
        /// </summary>
        /// <param name="processStageId"></param>
        /// <param name="commandTypeId"></param>
        /// <returns></returns>
        EscortProcessStageDefinitionDTO GetProcessStageWithCommandTypeAndId(int processStageId, int commandTypeId);
    }
}

