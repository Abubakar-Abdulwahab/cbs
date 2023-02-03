using Orchard;
using Parkway.CBS.Police.Core.HelperModels;

namespace Parkway.CBS.Police.Core.CoreServices.Contracts
{
    public interface ICoreEscortService : IDependency
    {
        /// <summary>
        /// Creates dispatch note
        /// </summary>
        /// <param name="fileNumber"></param>
        /// <param name="returnByte"></param>
        /// <returns></returns>
        CreateCertificateDocumentVM CreateDispatchNote(string fileNumber, bool returnByte = false);

        /// <summary>
        /// Checks if there is an escort request with specified file ref number that has been approved
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        bool CheckIfApprovedEscortRequestExists(string fileRefNumber);

        /// <summary>
        /// Checks if there is a escort request with specified file ref number that has been approved for the currently logged in user
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        bool CheckIfApprovedEscortRequestExists(string fileRefNumber, long taxEntityId);
    }
}
