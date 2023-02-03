using Orchard;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Client.Controllers.Handlers.Contracts
{
    public interface IValidateIdentificationNumberAJAXHandler : IDependency
    {
        /// <summary>
        /// Gets identification types available for tax category with specified id.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        IEnumerable<IdentificationTypeVM> GetIdentificationTypesForCategory(int categoryId);

        /// <summary>
        /// Gets Identification type with the specified Id
        /// </summary>
        /// <param name="idType"></param>
        /// <returns>IdentificationTypeVM</returns>
        IdentificationTypeVM validateIdType(int idType);

        /// <summary>
        /// Validate identification number using the specified Identification type implementing class name
        /// </summary>
        /// <param name="idNumber"></param>
        /// <param name="idType"></param>
        /// <param name="implementingClassName"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        ValidateIdentificationNumberResponseModel ValidateIdentificationNumber(string idNumber, int idType, string implementingClassName, out string errorMessage);
    }
}
