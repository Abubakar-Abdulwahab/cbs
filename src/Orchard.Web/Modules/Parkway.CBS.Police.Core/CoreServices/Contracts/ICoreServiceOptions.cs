using Orchard;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.CoreServices.Contracts
{
    public interface ICoreServiceOptions : IDependency
    {
        /// <summary>
        /// Get service options
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns>IEnumerable{PSSServiceOptionsVM}</returns>
        IEnumerable<PSServiceOptionsVM> GetActiveOtpions(int serviceId);


        /// <summary>
        /// Check if this s
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="selectedOption"></param>
        /// <returns>PSSServiceOptionsVM</returns>
        PSServiceOptionsVM GetActiveServiceOption(int serviceId, int optionId);

    }
}
