using System;
using Orchard;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;


namespace Parkway.CBS.Police.Core.CoreServices.Contracts
{
    public interface ICorePSServiceRequest : IDependency
    {

        /// <summary>
        /// Validate that the request has been fully paid
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <param name="requestId"></param>
        IEnumerable<PSServiceRequestInvoiceValidationDTO> ValidateProcessingFeeFullyPaid(Int64 requestId, string invoiceNumber);


        /// <summary>
        /// Save failed validations
        /// </summary>
        /// <param name="failedValidation"></param>
        void SaveFailedRequestValidation(PSSFailedProcessingFeeConfirmations failedValidation);


        /// <summary>
        /// Here we update the service request status after we have confirmed that
        /// all payments are complete
        /// </summary>
        /// <param name="requestDeets"></param>
        void UpdateServiceRequestAfterPaymentConfirmation(IEnumerable<PSServiceRequestInvoiceValidationDTO> requestDeet);


        /// <summary>
        /// Move request to the next defined level/stage
        /// </summary>
        /// <param name="requestDeets"></param>
        /// <returns>RequestFlowVM</returns>
        RequestFlowVM MoveRequestToTheNextStage(IEnumerable<PSServiceRequestInvoiceValidationDTO> requestDeets);


        /// <summary>
        /// Get invoice receipts details using the invoice number
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns></returns>
        ReceiptDisplayVM GetInvoiceReceiptsVM(string invoiceNumber);


        /// <summary>
        /// Get receipt details from database
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <param name="receiptNumber"></param>
        /// <returns>ReceiptDetailsVM</returns>
        ReceiptDetailsVM GetInvoiceReceiptVM(string invoiceNumber, string receiptNumber);


        /// <summary>
        /// We check if the service request has other service requests that have not been confirmed or approved
        /// <para>If we have service requests that have not been confirmed or approved then the request is not
        /// eligible to proceed to the next actionable work flow state</para>
        /// </summary>
        /// <param name="entireServiceRequest"></param>
        /// <param name="invoiceNumberGrp"></param>
        /// <returns>bool</returns>
        bool HasPendingConfirmation(IEnumerable<PSServiceRequestInvoiceValidationDTO> entireServiceRequest, IEnumerable<PSServiceRequestInvoiceValidationDTO> invoiceNumberServiceRequestGrp);

    }
}
