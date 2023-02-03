using Orchard;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Police.Core.CoreServices.Contracts
{
    public interface ICoreDiasporaCharacterCertificateInputValidation : IDependency
    {

        /// <summary>
        /// Validate Identity type
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="errors"></param>
        /// <param name="taxEntityId"></param>
        /// <param name="taxEntityCategoryId"></param>
        void ValideateIdentityDetails(PCCDiasporaUserInputVM userInput, List<ErrorModel> errors, long taxEntityId, int taxEntityCategoryId);

    }
}
