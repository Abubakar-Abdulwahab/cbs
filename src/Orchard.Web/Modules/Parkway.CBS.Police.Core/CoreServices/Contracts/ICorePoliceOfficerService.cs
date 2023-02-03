using Orchard;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Police.Core.CoreServices.Contracts
{
    public interface ICorePoliceOfficerService : IDependency
    {

        /// <summary>
        /// Gets police officer with specified service number from HR system
        /// </summary>
        /// <param name="serviceNumber"></param>
        /// <returns></returns>
        APIResponse GetPoliceOfficer(string serviceNumber);
    }
}
