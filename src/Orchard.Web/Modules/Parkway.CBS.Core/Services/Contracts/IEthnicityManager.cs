using Orchard;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IEthnicityManager<Ethnicity> : IDependency, IBaseManager<Ethnicity>
    {
        /// <summary>
        /// Gets all active ethnicities
        /// </summary>
        /// <returns></returns>
        IEnumerable<EthnicityVM> GetEthnicities();

        /// <summary>
        /// Get ethnicity with specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        EthnicityVM GetEthnicity(int id);
    }
}
