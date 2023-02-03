using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSServiceCaveatManager<PSServiceCaveat> : IDependency, IBaseManager<PSServiceCaveat>
    {
        /// <summary>
        /// Get caveat for service with specified service id
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        PSServiceCaveatVM GetServiceCaveat(int serviceId);
    }
}
