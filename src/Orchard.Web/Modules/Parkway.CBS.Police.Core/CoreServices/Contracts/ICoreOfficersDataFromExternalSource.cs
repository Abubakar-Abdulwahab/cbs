using Orchard;
using Parkway.CBS.Police.Core.ExternalSourceData.HRSystem;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.CoreServices.Contracts
{
    public interface ICoreOfficersDataFromExternalSource : IDependency
    {

        /// <summary>
        /// Check if the requested chunk exists
        /// </summary>
        /// <param name="requestIdentifier"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns>bool</returns>
        bool CheckIfChunkExists(string requestIdentifier, int take, int skip);


        /// <summary>
        /// Call the external data source for officers data
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="requestIdentifier"></param>
        /// <returns>OfficersRequestResponseModel</returns>
        OfficersRequestResponseModel GetOfficersDataFromExternalSource(PoliceOfficerSearchParams searchParams, string requestIdentifier);

    }
}
