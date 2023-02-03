using Orchard;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface ICBSUserTaxEntityProfileLocationManager<CBSUserTaxEntityProfileLocation> : IDependency, IBaseManager<CBSUserTaxEntityProfileLocation>
    {

        /// <summary>
        /// Gets tax entity profile location id for cbs user with the specified id
        /// </summary>
        /// <param name="cbsUserId"></param>
        /// <returns></returns>
        int GetCBSUserLocationId(long cbsUserId);

        /// <summary>
        /// Gets location for cbs user with specified id
        /// </summary>
        /// <param name="cbsUserId"></param>
        /// <returns></returns>
        TaxEntityProfileLocationVM GetCBSUserLocationWithId(long cbsUserId);

        /// <summary>
        /// Gets cbs user in location with specified id
        /// </summary>
        /// <param name="taxEntityProfileLocationId"></param>
        /// <returns></returns>
        CBSUserVM GetSubUserInLocation(int taxEntityProfileLocationId);
    }
}
