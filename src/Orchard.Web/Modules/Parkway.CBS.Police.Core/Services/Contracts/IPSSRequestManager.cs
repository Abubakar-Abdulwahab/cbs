using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSSRequestManager<PSSRequest> : IDependency, IBaseManager<PSSRequest>
    {

        /// <summary>
        /// Get request Id by file number
        /// </summary>
        /// <param name="fileNumber"></param>
        /// <returns>long</returns>
        long GetRequestId(string fileNumber);


        /// <summary>
        /// Get request details by request id
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>GenericRequestDetails</returns>
        GenericRequestDetails GetRequestDetails(Int64 requestId);


        /// <summary>
        /// Get the file ref number for this request
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string GetFileRefNumber(long id);


        /// <summary>
        /// Get the service type of this request Id
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>int</returns>
        int GetServiceType(long requestId);


        /// <summary>
        /// Get service type Id by file number
        /// </summary>
        /// <param name="fileNumber"></param>
        /// <returns>long</returns>
        int GetServiceType(string fileNumber);


        /// <summary>
        /// Update the request with the new flow definition level and status
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="newDefinitionLevelId"></param>
        void UpdateRequestFlowId(long requestId, int newDefinitionLevelId, PSSRequestStatus status);


        /// <summary>
        /// Get request invoices
        /// </summary>
        /// <param name="fileNumber"></param>
        /// <returns>List<RequestInvoiceVM></returns>
        ICollection<RequestInvoiceVM> GetRequestInvoices(string fileNumber);

        /// <summary>
        /// Get invoices for request with specified Id
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>List<RequestInvoiceVM></returns>
        ICollection<RequestInvoiceVM> GetRequestInvoices(long requestId);

        /// <summary>
        /// Get request details by file number
        /// </summary>
        /// <param name="fileNumber"></param>
        /// <returns>GenericRequestDetails</returns>
        PSSRequestVM GetRequestDetails(string fileNumber);


        /// <summary>
        /// Gets the service id, service name, customer name and file ref number of request with specified id
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>PSSRequestVM</returns>
        PSSRequestVM GetPSSRequestServiceDetails(long requestId);


        /// <summary>
        /// Get all the form details for this request
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>IEnumerable{FormControlRevenueHeadValueVM}</returns>
        IEnumerable<FormControlRevenueHeadValueVM> GetFormDetails(long requestId);


        /// <summary>
        /// Update the request with the given status
        /// <para>Returns true if successfully saved</para>
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="status"></param>
        /// <returns>bool</returns>
        bool SetRequestStatus(long requestId, PSSRequestStatus status);


        /// <summary>
        /// Set approval number for request with specified Id
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="approvalNumber"></param>
        void SetApprovalNumber(long requestId, string approvalNumber);

        /// <summary>
        /// Get request details by request file ref number
        /// </summary>
        /// <param name="fileNumber"></param>
        /// <returns>GenericRequestDetails</returns>
        GenericRequestDetails GetRequestDetailsByFileNumber(string fileNumber);

        /// <summary>
        /// Get info of request with specified approval number
        /// </summary>
        /// <param name="approvalNumber">approval number</param>
        /// <param name="taxEntityId"></param>
        /// <returns>ValidatedDocumentVM</returns>
        ValidatedDocumentVM GetRequestInfoWithApprovalNumber(string approvalNumber, long taxEntityId);

        /// <summary>
        /// Get info of request with specified approval number
        /// </summary>
        /// <param name="approvalNumber">approval number</param>
        /// <returns>ValidatedDocumentVM</returns>
        ValidatedDocumentVM GetRequestInfoWithApprovalNumber(string approvalNumber);

        /// <summary>
        /// Get info of request with specified approval number
        /// </summary>
        /// <param name="approvalNumber">approval number</param>
        /// <returns>USSDValidateDocumentVM</returns>
        USSDValidateDocumentVM GetRequestDetailsByApprovalNumber(string approvalNumber);
    }
}
