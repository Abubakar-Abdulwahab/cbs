using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Seeds.Contracts;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Seeds
{
    public class CountrySeeds : ICountrySeeds
    {
        private readonly ICountryManager<Country> _repo;

        public CountrySeeds(ICountryManager<Country> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Adds a collection of countries
        /// </summary>
        /// <param name="countries"></param>
        public void AddCountries(IEnumerable<dynamic> countries)
        {
            try
            {
                List<Country> addedCountries = new List<Country> { };
                foreach (dynamic country in countries)
                {
                    addedCountries.Add(new Country
                    {
                        Name = country.name,
                        Code = country.twoLetterCode,
                        IsActive = true
                    });
                }

                _repo.SaveBundle(addedCountries);
            }
            catch(Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }
}