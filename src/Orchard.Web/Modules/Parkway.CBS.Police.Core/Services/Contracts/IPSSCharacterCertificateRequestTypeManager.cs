using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSSCharacterCertificateRequestTypeManager<PSSCharacterCertificateRequestType> : IDependency, IBaseManager<PSSCharacterCertificateRequestType>
    {
        /// <summary>
        /// Gets all active character certificate request types
        /// </summary>
        /// <returns>IEnumerable<CharacterCertificateRequestTypeVM></returns>
        IEnumerable<CharacterCertificateRequestTypeVM> GetRequestTypes();

        /// <summary>
        /// Gets character certificate request type with specified id
        /// </summary>
        /// <param name="requestTypeId"></param>
        /// <returns>CharacterCertificateRequestTypeVM</returns>
        CharacterCertificateRequestTypeVM GetRequestType(int requestTypeId);
    }
}
