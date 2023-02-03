using Orchard;
using Parkway.CBS.Police.Core.Models.Enums;

namespace Parkway.CBS.Police.Core.PSSServiceType.CharacterCertificateOptions.Contracts
{
    public interface ICharacterCertificateOptions : IDependency
    {

        CharacterCertificateOption GetOptionType { get; }

    }
}
