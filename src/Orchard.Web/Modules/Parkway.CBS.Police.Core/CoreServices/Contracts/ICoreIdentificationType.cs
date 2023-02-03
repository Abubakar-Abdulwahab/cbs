using System.Collections.Generic;
using Orchard;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.CoreServices.Contracts
{
    public interface ICoreIdentificationType : IDependency
    {
        /// <summary>
        /// Check if the user profile's identity type
        /// has biometric support
        /// </summary>
        /// <param name="taxEntityId"></param>
        /// <returns>bool</returns>
        bool CheckIfTaxEntityIdentityTypeHasBiometricSupport(long taxEntityId);


        /// <summary>
        /// Get the list of identity types that have biometric support
        /// for this tax category Id
        /// </summary>
        /// <returns>IEnumerable{IdentificationTypeVM}</returns>
        IEnumerable<IdentificationTypeVM> GetIdentityWithBiometricSupport(int taxCategoryId);

    }
}
