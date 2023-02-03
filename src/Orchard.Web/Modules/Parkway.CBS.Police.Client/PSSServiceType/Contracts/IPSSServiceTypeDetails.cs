using Orchard;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models.Enums;

namespace Parkway.CBS.Police.Client.PSSServiceType.Contracts
{
    public interface IPSSServiceTypeDetails : IDependency
    {
        PSSServiceTypeDefinition GetServiceTypeDefinition { get; }


        /// <summary>
        /// Get next move after profile confirmation
        /// </summary>
        /// <returns>RouteNameAndStage</returns>
        RouteNameAndStage GetDirectionAfterUserProfileConfirmation();


        /// <summary>
        /// Get request details
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        dynamic GetRequestDetails(string fileRefNumber, long taxEntityId);


        /// <summary>
        /// Get request info
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        dynamic GetRequestInfo(long requestId);


        /// <summary>
        /// Downloads service specific certificate document
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        CreateCertificateDocumentVM CreateCertificate(string fileRefNumber, long taxEntityId);

    }
}
