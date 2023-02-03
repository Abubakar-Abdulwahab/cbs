using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IExtractDetailsManager<ExtractDetails> : IDependency, IBaseManager<ExtractDetails>
    {
        /// <summary>
        /// Get info of extract request with specified approval number
        /// </summary>
        /// <param name="approvalNumber">approval number</param>
        /// <returns>IEnumerable<ValidatedExtractInfoVM></returns>
        IEnumerable<ValidatedExtractInfoVM> GetExtractInfoWithApprovalNumber(string approvalNumber);

        /// <summary>
        /// Get request details
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>ExtractRequestDetailsVM</returns>
        ExtractRequestDetailsVM GetRequestDetails(long requestId);


        /// <summary>
        /// Get extract request view details
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <param name="taxEntityId"\></param>
        /// <returns>IEnumerable<ExtractDetailsVM></returns>
        IEnumerable<ExtractDetailsVM> GetExtractRequestViewDetails(string fileRefNumber, long taxEntityId);


        /// <summary>
        /// Get extract document info
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>ExtractRequestDetailsVM</returns>
        IEnumerable<ExtractRequestDetailsVM> GetExtractDocumentInfo(long requestId);

        /// <summary>
        /// Updates content, diary serial number, cross referencing, incident date and time for extract details with specified request id
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="userInput"></param>
        void UpdateExtractDetailsContentAndDiaryInfo(long requestId, ExtractRequestDetailsVM userInput);

        /// <summary>
        /// Gets extract document details for request with specified file ref number
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        ExtractDocumentVM GetExtractDocumentDetails(string fileRefNumber);

        /// <summary>
        /// Check if a content detail for an extract has been populated using the FileRefNumber
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns>bool</returns>
        bool CheckExtractContentDetails(string fileRefNumber);

        /// <summary>
        /// Check if extract diary number and incident date time have been populated
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns>bool</returns>
        bool CheckExtractDiaryIncidentDetails(string fileRefNumber);


        /// <summary>
        /// Get a content detail for an extract using the FileRefNumber
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns>string</returns>
        string GetExtractContentDetails(string fileRefNumber);

        /// <summary>
        /// Updates content for extract details with specified request id
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="userInput"></param>
        void UpdateExtractDetailsContent(long requestId, string content);


        /// <summary>
        /// Check if <paramref name="affivdavitNumber"/> does not exist with the user 
        /// with the <paramref name="taxEntityId"/>
        /// </summary>
        /// <param name="affivdavitNumber"></param>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        bool CheckIfExistingAffidavitNumber(string affivdavitNumber, long taxEntityId);


        /// <summary>
        /// Updates extract details DPO Name and Service Number
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="serviceNumber"></param>
        /// <param name="name"></param>
        /// <param name="dpoRankCode"></param>
        /// <param name="adminId"></param>
        void UpdateExtractDPONameAndServiceNumber(long requestId, string serviceNumber, string name, string dpoRankCode, int adminId);

    }
}
