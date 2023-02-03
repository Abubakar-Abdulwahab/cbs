using Orchard;
using Parkway.CBS.Core.Models.Enums;
using System.Collections.Generic;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICoreMDARevenueAccessRestrictionsStagingService : IDependency
    {
        /// <summary>
        /// Validate and save MDARevenueAccessRestrictions MDAs & Revenue Heads
        /// </summary>
        /// <param name="additions"></param>
        /// <param name="removals"></param>
        /// <param name="providerId"></param>
        /// <param name="userId"></param>
        /// <param name="operationType"></param>
        /// <returns>MDARevenueHeadEntryStaging Reference</returns>
        string ValidateAndSaveStagingData(Dictionary<int, IEnumerable<int>> additions, Dictionary<int, IEnumerable<int>> removals, int providerId, int userId, string implementingClassType);
    }
}
