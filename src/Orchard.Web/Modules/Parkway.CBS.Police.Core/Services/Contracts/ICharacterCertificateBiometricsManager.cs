using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface ICharacterCertificateBiometricsManager<CharacterCertificateBiometrics> : IDependency, IBaseManager<CharacterCertificateBiometrics>
    {
        /// <summary>
        /// Gets Character Certificate Biometrics by <paramref name="requestId"/>
        /// </summary>
        /// <returns></returns>
        CharacterCertificateBiometricsVM GetCharacterCertificateBiometrics(long requestId);
    }
}
