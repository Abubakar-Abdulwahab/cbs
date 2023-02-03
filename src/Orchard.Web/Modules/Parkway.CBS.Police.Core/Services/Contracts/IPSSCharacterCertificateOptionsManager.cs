using Orchard;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;
using Parkway.CBS.Core.Services.Contracts;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSSCharacterCertificateOptionsManager<PSSCharacterCertificateOptions> : IDependency, IBaseManager<PSSCharacterCertificateOptions>
    {

        /// <summary>
        /// Gets active character certificate options
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns>IEnumerable{PSSCharacterCertificateOptionsPageVM}</returns>
        IEnumerable<PSServiceOptionsVM> GetActiveCharacterCertificateOtpions(int serviceId);


        /// <summary>
        /// Get service option with the service Id and option Id that is active
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="optionId"></param>
        /// <returns>PSSServiceOptionsVM</returns>
        PSServiceOptionsVM GetServiceOption(int serviceId, int optionId);

    }
}
