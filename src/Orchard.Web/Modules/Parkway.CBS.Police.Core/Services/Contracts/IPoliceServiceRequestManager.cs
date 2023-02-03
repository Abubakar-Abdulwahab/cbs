using System;
using Orchard;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.Models.Enums;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPoliceServiceRequestManager<PoliceServiceRequest> : IDependency, IBaseManager<PoliceServiceRequest>
    {

        /// <summary>
        /// Get the invoice details as it pertains to service request table using the request Id
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>IEnumerable{PSServiceRequestInvoiceValidationDTO}</returns>
        IEnumerable<PSServiceRequestInvoiceValidationDTO> GetServiceRequestDetailsWithRequestId(Int64 requestId);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns></returns>
        ReceiptDisplayVM GetReceipts(string invoiceNumber);


        /// <summary>
        /// Get receipt details
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <param name="receiptNumber"></param>
        /// <returns>ReceiptDetailsVM</returns>
        ReceiptDetailsVM GetReceipt(string invoiceNumber, string receiptNumber);


        /// <summary>
        /// here we update the service request status
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="oldDefinitionLevelId"></param>
        /// <param name="serviceId"></param>
        /// <param name="invoiceId"></param>
        /// <param name="status"></param>
        void UpdateServiceRequestsStatus(long requestId, int oldDefinitionLevelId, int serviceId, PSSRequestStatus status, Int64 invoiceId);


        /// <summary>
        /// Get request details for request with form inputs
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>GenericRequestDetailsVM</returns>
        GenericRequestDetailsVM GetServiceRequestDetailsForGenericWithRequestId(long requestId);


        /// <summary>
        /// Get generic police request view details
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        IEnumerable<GenericRequestDetailsVM> GetGenericServiceRequestDetails(string fileRefNumber, long taxEntityId);


        /// <summary>
        /// Get generic police request document info
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        IEnumerable<GenericRequestDetailsVM> GetGenericDocumentInfo(long requestId);


        /// <summary>
        /// Creates a new entry in the police service request table
        /// </summary>
        /// <param name="requestId">current request id</param>
        /// <param name="flowDefinitionLevelId">current flow definition level id</param>
        /// <param name="nextFlowDefinitionLevelId">the flow definition level id of the level the request is being moved to</param>
        void SavePoliceServiceRequest(long requestId, int flowDefinitionLevelId, int nextFlowDefinitionLevelId);

    }
}
