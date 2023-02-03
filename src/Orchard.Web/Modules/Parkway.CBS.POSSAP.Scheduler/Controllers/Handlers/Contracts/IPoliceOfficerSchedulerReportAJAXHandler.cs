using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.ExternalSourceData.HRSystem;

namespace Parkway.CBS.POSSAP.Scheduler.Controllers.Handlers.Contracts
{
    public interface IPoliceOfficerSchedulerReportAJAXHandler : IDependency
    {
        OfficersRequestResponseModel GetOfficers(int page);


        /// <summary>
        /// Get the request identifier for the given search params token
        /// </summary>
        /// <param name="searchParametersToken"></param>
        /// <returns>string</returns>
        string GetRequestIdentifier(string searchParametersToken);


        /// <summary>
        /// Here we deserialize the searchParams value, 
        /// check that the request Identifier matches the serialized value
        /// then check if the search params have their page offset cached, if no cache found we return false, else true.
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="requestIdentifier"></param>
        /// <returns>APIResponse</returns>
        APIResponse CheckSearchParamsConstraints(string searchParams, string requestIdentifier);


    }
}
