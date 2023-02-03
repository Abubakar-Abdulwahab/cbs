using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IPSSCharacterCertificateUpdateDetailsHandler : IDependency
    {
        /// <summary>
        /// Get PCC details using filenumber
        /// </summary>
        /// <param name="fileNumber"></param>
        /// <returns>CharacterCertificateDetailsUpdateVM</returns>
        CharacterCertificateDetailsUpdateVM GetFileNumberDetails(string fileNumber);

        /// <summary>
        /// Gets all active countries
        /// </summary>
        /// <returns>IEnumerable<CountryVM></returns>
        IEnumerable<CountryVM> GetCountries();

        /// <summary>
        /// Update character certificate details
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errors"></param>
        /// <returns>bool</returns>
        bool UpdateCharacterCertificateDetails(CharacterCertificateDetailsUpdateVM model, out List<ErrorModel> errors);
    }
}
