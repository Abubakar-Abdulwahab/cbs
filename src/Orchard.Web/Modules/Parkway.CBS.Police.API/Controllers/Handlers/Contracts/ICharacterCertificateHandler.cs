using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.HelperModels;
using System.Net.Http;

namespace Parkway.CBS.Police.API.Controllers.Handlers.Contracts
{
    public interface ICharacterCertificateHandler : IDependency
    {
        /// <summary>
        /// Get character certificate details using the file number
        /// </summary>
        /// <param name="fileNumber"></param>
        /// <param name="token"></param>
        /// <returns>APIResponse</returns>
        APIResponse GetCharacterCertificateDetails(string fileNumber, string token, HttpRequestMessage httpRequest);

        /// <summary>
        /// Validates and saves <see cref="CharacterCertificateBiometrics"/> 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        APIResponse SaveCharacterCertificateBiometrics(CharacterCertificateBiometricRequestVM model, HttpRequestMessage httpRequest);
    }
}
