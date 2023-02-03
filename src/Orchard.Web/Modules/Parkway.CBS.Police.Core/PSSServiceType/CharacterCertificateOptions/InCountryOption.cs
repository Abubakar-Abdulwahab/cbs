using System;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.PSSServiceType.CharacterCertificateOptions.Contracts;

namespace Parkway.CBS.Police.Core.PSSServiceType.CharacterCertificateOptions
{
    public class InCountryOption : ICharacterCertificateOptions
    {
        public CharacterCertificateOption GetOptionType => CharacterCertificateOption.In_Country;
    }
}