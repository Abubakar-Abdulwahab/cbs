using Orchard;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICoreEthnicityService : IDependency
    {
        /// <summary>
        /// Gets all active ethnicities
        /// </summary>
        /// <returns></returns>
        IEnumerable<EthnicityVM> GetEthnicities();

        /// <summary>
        /// Checks if ehtnicity with specified id exists
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool ValidateEthnicity(int id);
    }
}
