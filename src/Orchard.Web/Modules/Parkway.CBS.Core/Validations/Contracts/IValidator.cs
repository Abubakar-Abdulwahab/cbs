using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using System.Collections.Generic;

namespace Parkway.CBS.Core.Validations.Contracts
{
    public interface IValidator : IDependency
    {
        /// <summary>
        /// Get the list of validation errors
        /// </summary>
        /// <returns>List{ErrorModel} <see cref="ErrorModel"/></returns>
        List<ErrorModel> HasValidationErrors();

        /// <summary>
        /// Check through the collection and find elements that have duplicate values
        /// </summary>
        /// <typeparam name="M">Subclass of CBSModel</typeparam>
        /// <param name="mdas"></param>
        /// <returns>IValidator</returns>
        IValidator BundleCollectionUnique<M>(ICollection<UniqueValidationModel> dataValues) where M : CBSBaseModel;

        /// <summary>
        /// Check that the datavalue items are unique
        /// </summary>
        /// <typeparam name="M">CBSModel</typeparam>
        /// <param name="dataValues">List{UniqueValidationModel}</param>
        /// <returns>IValidator</returns>
        IValidator BundleUnique<M>(List<UniqueValidationModel> dataValues) where M : CBSBaseModel;
    }
}
