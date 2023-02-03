using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Client.PSSServiceType.ServiceOptions.Contracts;

namespace Parkway.CBS.Police.Client.PSSServiceType.ServiceOptions.CharacterCertificate
{
    public class Diaspora : IServiceOptionPresentation
    {
        /// <summary>
        /// get the option type for Character certifcate diaspora
        /// </summary>
        public string GetOptionType => nameof(CharacterCertificateOption.Diaspora);


        /// <summary>
        /// Get the route name and process stage for diaspora
        /// </summary>
        RouteNameAndStage IServiceOptionPresentation.GetRouteName => new RouteNameAndStage { Stage = PSSUserRequestGenerationStage.PSSCharacterCertificateRequest, RouteName = RouteName.PSSCharacterCertificateDiaspora.CharacterCertificateDiasporaRequest };

    }
}