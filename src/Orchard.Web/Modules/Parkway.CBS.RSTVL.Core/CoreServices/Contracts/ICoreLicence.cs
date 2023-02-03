using Orchard;
using Parkway.CBS.RSTVL.Core.Models;

namespace Parkway.CBS.RSTVL.Core.CoreServices.Contracts
{
    public interface ICoreLicence : IDependency
    {

        /// <summary>
        /// Save licence details
        /// </summary>
        /// <param name="licence"></param>
        void SaveDetails(RSTVLicence licence);
    }
}
