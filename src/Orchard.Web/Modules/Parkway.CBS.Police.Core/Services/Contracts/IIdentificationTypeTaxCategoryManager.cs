using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IIdentificationTypeTaxCategoryManager<IdentificationTypeTaxCategory> : IDependency, IBaseManager<IdentificationTypeTaxCategory>
    {

        /// <summary>
        /// Gets all identification types for tax category with specified Id
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        IEnumerable<IdentificationTypeVM> GetIdentificationTypesForCategory(int categoryId);


        /// <summary>
        /// Check that there is a match between Id
        /// and tax category Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="categoryId"></param>
        /// <returns>IdentificationTypeVM</returns>
        IdentificationTypeVM GetIdentityTypeWithTaxCategory(int identityId, int categoryId);


        /// <summary>
        /// Get the list of identity types with biometric support
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns>IEnumerable{IdentificationTypeVM}</returns>
        IEnumerable<IdentificationTypeVM> GetIdentificationTypesWithBiometricSupportForCategoryId(int categoryId);

    }
}
