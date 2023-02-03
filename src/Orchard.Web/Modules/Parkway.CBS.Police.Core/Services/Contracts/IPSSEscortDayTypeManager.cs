using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.DTO;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSSEscortDayTypeManager<PSSEscortDayType> : IDependency, IBaseManager<PSSEscortDayType>
    {
        /// <summary>
        /// Gets escort day types
        /// </summary>
        /// <returns></returns>
        IEnumerable<PSSEscortDayTypeDTO> GetPSSEscortDayTypes();
    }
}
