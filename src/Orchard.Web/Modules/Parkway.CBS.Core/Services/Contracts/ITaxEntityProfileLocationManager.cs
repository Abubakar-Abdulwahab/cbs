using Orchard;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface ITaxEntityProfileLocationManager<TaxEntityProfileLocation> : IDependency, IBaseManager<TaxEntityProfileLocation>
    {
        /// <summary>
        /// Gets locations of tax entity with specified id
        /// </summary>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        IEnumerable<TaxEntityProfileLocationVM> GetTaxEntityLocations(long taxEntityId);

        /// <summary>
        /// Gets tax entity profile location with the specified id
        /// </summary>
        /// <param name="taxEntityId"></param>
        /// <param name="locationId"></param>
        /// <returns></returns>
        TaxEntityProfileLocationVM GetTaxEntityLocationWithId(long taxEntityId, int locationId);

        /// <summary>
        /// Gets tax entity profile location with the specified id
        /// </summary>
        /// <param name="taxEntityId"></param>
        /// <param name="locationId"></param>
        /// <returns></returns>
        TaxEntityProfileLocationVM GetTaxEntityLocationWithId(int locationId);

        /// <summary>
        /// Gets default tax entity profile location for tax entity with specified id
        /// </summary>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        TaxEntityProfileLocationVM GetDefaultTaxEntityLocation(long taxEntityId);

        /// <summary>
        /// Gets id of default tax entity location
        /// </summary>
        /// <param name="payerId"></param>
        /// <returns></returns>
        int GetDefaultTaxEntityLocationId(string payerId);
    }
}
