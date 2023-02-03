using Orchard;
using System.Collections.Generic;

namespace Parkway.CBS.Core.Seeds.Contracts
{
    public interface ICountrySeeds : IDependency
    {
        /// <summary>
        /// Adds a collection of countries
        /// </summary>
        /// <param name="countries"></param>
        void AddCountries(IEnumerable<dynamic> countries);
    }
}
