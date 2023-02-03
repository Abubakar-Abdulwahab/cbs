using Orchard;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICoreCountryService : IDependency
    {
        /// <summary>
        /// Gets all active countries
        /// </summary>
        /// <returns></returns>
        IEnumerable<CountryVM> GetCountries();

        /// <summary>
        /// Checks if country with specified id exists
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool ValidateCountry(int id);

        /// <summary>
        /// Checks if country with specified id is Nigeria
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool checkIfCountryIsNigeria(int id);

        /// <summary>
        /// Get country name for the specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>string</returns>
        string GetCountryName(int id);

    }
}
