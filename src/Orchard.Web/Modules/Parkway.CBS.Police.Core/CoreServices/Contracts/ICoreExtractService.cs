using Orchard;
using Parkway.CBS.Police.Core.HelperModels;

namespace Parkway.CBS.Police.Core.CoreServices.Contracts
{
    public interface ICoreExtractService : IDependency
    {
        /// <summary>
        /// Retrieves PSS Extract Document
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <param name="returnByte"></param>
        /// <returns></returns>
        CreateCertificateDocumentVM CreateExtractDocument(string fileRefNumber, bool returnByte = false);

        /// <summary>
        /// Checks if there is an extract request with specified file ref number that has been approved
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        bool CheckIfApprovedExtractRequestExists(string fileRefNumber);

        /// <summary>
        /// Checks if there is an extract request with specified file ref number that has been approved for currenly logged in user
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        bool CheckIfApprovedExtractRequestExists(string fileRefNumber, long taxEntityId);

        /// <summary>
        /// Generates and saves extract document for request with specified file ref number
        /// </summary>
        /// <param name="fileRefNumber"></param>
        void CreateAndSaveExtractDocument(string fileRefNumber);

        /// <summary>
        /// Retrieves Default PSS Extract Document before approval
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <param name="returnByte"></param>
        /// <returns></returns>
        CreateCertificateDocumentVM CreateDefaultExtractDocument(string fileRefNumber);

        /// <summary>
        /// Indicates if the extract document with the specified approval number has a signature attached
        /// </summary>
        /// <param name="approvalNumber"></param>
        /// <returns></returns>
        bool CheckIfExtractDocumentIsSigned(string approvalNumber);
    }
}
