using Orchard;
using Parkway.CBS.CacheProvider;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using System.Collections.Generic;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CoreCountryService : ICoreCountryService
    {
        private readonly ICountryManager<Country> _repo;
        private readonly IOrchardServices _orchardServices;

        public CoreCountryService(ICountryManager<Country> repo, IOrchardServices orchardServices)
        {
            _repo = repo;
            _orchardServices = orchardServices;
        }


        /// <summary>
        /// Gets all active countries
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CountryVM> GetCountries()
        {
            string tenant = _orchardServices.WorkContext.CurrentSite.SiteName;

            IEnumerable<CountryVM> result = ObjectCacheProvider.GetCachedObject<IEnumerable<CountryVM>>(tenant, $"{nameof(CachePrefix.Countries)}");

            if (result == null)
            {
                result = _repo.GetCountries();

                if (result != null)
                {
                    ObjectCacheProvider.TryCache(tenant, $"{nameof(CachePrefix.Countries)}", result);
                }
            }

            return result;
        }


        /// <summary>
        /// Checks if country with specified id exists
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ValidateCountry(int id)
        {
            return _repo.Count(x => x.Id == id) > 0;
        }


        /// <summary>
        /// Checks if country with specified id is Nigeria
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool checkIfCountryIsNigeria(int id)
        {
            return _repo.Count(x => x.Id == id && x.Name.ToLower() == "nigeria") > 0;
        }

        /// <summary>
        /// Get country name for the specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>string</returns>
        public string GetCountryName(int id)
        {
            return _repo.GetCountry(id).Name;
        }
    }
}