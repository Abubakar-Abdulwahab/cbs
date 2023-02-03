using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.HelperModels;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSSCharacterCertificateManager<PSSCharacterCertificate> : IDependency, IBaseManager<PSSCharacterCertificate>
    {
        /// <summary>
        /// Gets character certificate details with specified file ref number
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        CharacterCertificateDocumentVM GetCertificateDetails(string fileRefNumber);
    }
}
