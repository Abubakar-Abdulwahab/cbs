using Orchard;
using Parkway.CBS.Core.Models;
using System.Collections.Generic;

namespace Parkway.CBS.Core.Validations.Rules.Contracts
{
    public interface IUniqueness : IDependency
    {
        /// <summary>
        /// Checks that the values in the datavalues list are unique, if not 
        /// returns a list of string values which are not unique
        /// </summary>
        /// <typeparam name="T">CBSModel</typeparam>
        /// <param name="dataValues">List{UniqueValidationModel}</param>
        /// <returns>List{string}</returns>
        List<string> BundleUniqueness<T>(List<UniqueValidationModel> dataValues) where T : CBSBaseModel;
    }
}