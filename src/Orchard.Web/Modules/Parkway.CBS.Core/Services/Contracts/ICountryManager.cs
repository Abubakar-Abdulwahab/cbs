using Orchard;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface ICountryManager<Country> : IDependency, IBaseManager<Country>
    {
        /// <summary>
        /// Gets all active countries
        /// </summary>
        /// <returns></returns>
        IEnumerable<CountryVM> GetCountries();

        /// <summary>
        /// Gets country with specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        CountryVM GetCountry(int id);
    }
}
