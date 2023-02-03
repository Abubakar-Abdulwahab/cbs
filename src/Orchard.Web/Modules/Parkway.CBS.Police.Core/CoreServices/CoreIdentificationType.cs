using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.CoreServices
{
    public class CoreIdentificationType : ICoreIdentificationType
    {
        private readonly IIdentificationTypeManager<IdentificationType> _identificationTypeRepo;
        private readonly IIdentificationTypeTaxCategoryManager<IdentificationTypeTaxCategory> _identityTypeTaxCategoryRepo;

        private readonly ICoreTaxPayerService _coreTaxEntity;

        public CoreIdentificationType(IIdentificationTypeManager<IdentificationType> identificationTypeRepo, ICoreTaxPayerService coreTaxEntity, IIdentificationTypeTaxCategoryManager<IdentificationTypeTaxCategory> identityTypeTaxCategoryRepo)
        {
            _identificationTypeRepo = identificationTypeRepo;
            _coreTaxEntity = coreTaxEntity;
            _identityTypeTaxCategoryRepo = identityTypeTaxCategoryRepo;
        }


        /// <summary>
        /// Check if the user profile's identity type
        /// has biometric support
        /// </summary>
        /// <param name="taxEntityId"></param>
        /// <returns>bool</returns>
        public bool CheckIfTaxEntityIdentityTypeHasBiometricSupport(long taxEntityId)
        {
            int identityTypeId = _coreTaxEntity.GetIdentityType(taxEntityId);
            //check that the identity type has biometric support
            return _identificationTypeRepo.HasBiometricSupport(identityTypeId);
        }


        /// <summary>
        /// Get the list of identity types that have biometric support
        /// for this tax category Id
        /// </summary>
        /// <returns>IEnumerable{IdentificationTypeVM}</returns>
        public IEnumerable<IdentificationTypeVM> GetIdentityWithBiometricSupport(int taxCategoryId)
        {
            return _identityTypeTaxCategoryRepo.GetIdentificationTypesWithBiometricSupportForCategoryId(taxCategoryId);
        }

    }
}