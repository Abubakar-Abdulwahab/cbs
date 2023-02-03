using System;
using System.Linq;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.PSSServiceType.Approval.Contracts;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.CoreServices
{
    public class CorePSServiceRequest : ICorePSServiceRequest
    {
        private readonly IPoliceServiceRequestManager<PoliceServiceRequest> _serviceRequestRepo;
        private readonly IPSSFailedApplicationFeeRequestsManager<PSSFailedProcessingFeeConfirmations> _failedRequestsRepo;
        private readonly IPoliceCollectionLogManager<PoliceCollectionLog> _collectionLogManager;
        private readonly Lazy<IApprovalComposition> _approvalComposition;
        private readonly IPSSRequestInvoiceManager<PSSRequestInvoice> _iPSSRequestInvoiceManager;
        private readonly ICBSUserTaxEntityProfileLocationManager<CBSUserTaxEntityProfileLocation> _iCBSUserTaxEntityProfileLocationManager;
        public ILogger Logger { get; set; }

        public CorePSServiceRequest(IPoliceServiceRequestManager<PoliceServiceRequest> serviceRequestRepo, IPSSFailedApplicationFeeRequestsManager<PSSFailedProcessingFeeConfirmations> failedRequestsRepo, Lazy<IApprovalComposition> approvalComposition, IPoliceCollectionLogManager<PoliceCollectionLog> collectionLogManager, IPSSRequestInvoiceManager<PSSRequestInvoice> iPSSRequestInvoiceManager, ICBSUserTaxEntityProfileLocationManager<CBSUserTaxEntityProfileLocation> iCBSUserTaxEntityProfileLocationManager)
        {
            _serviceRequestRepo = serviceRequestRepo;
            _failedRequestsRepo = failedRequestsRepo;
            Logger = NullLogger.Instance;
            _approvalComposition = approvalComposition;
            _collectionLogManager = collectionLogManager;
            _iPSSRequestInvoiceManager = iPSSRequestInvoiceManager;
            _iCBSUserTaxEntityProfileLocationManager = iCBSUserTaxEntityProfileLocationManager;
        }


        /// <summary>
        /// Validate that the request has been fully paid
        /// <para>Here we get the request service details and do a validation on the first of the group
        /// to ascertain that there is no amount due and the invoice has not been cancelled</para>
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <param name="fileRefNumber"></param>
        public IEnumerable<PSServiceRequestInvoiceValidationDTO> ValidateProcessingFeeFullyPaid(Int64 requestId, string invoiceNumber)
        {
            //here we get the service request details for the request
            IEnumerable<PSServiceRequestInvoiceValidationDTO> result = _serviceRequestRepo.GetServiceRequestDetailsWithRequestId(requestId);

            IEnumerable<PSServiceRequestInvoiceValidationDTO> invoiceNumberGrp = result.Where(g => g.InvoiceNumber == invoiceNumber);

            if (invoiceNumberGrp == null || !invoiceNumberGrp.Any())
            { throw new NoInvoicesMatchingTheParametersFoundException("404 for PSS application fee. Request Id " + requestId); }

            if (invoiceNumberGrp.First().AmountDue > 0.00m)
            { throw new InvoiceHasPaymentException("Invoice has pending payments Request Id " + requestId); }


            if (invoiceNumberGrp.First().InvoiceStatus == InvoiceStatus.WriteOff)
            {
                throw new InvoiceCancelledException(string.Format("Invoice has been written off invoice number {0}, request Id {1}, Payment date {2}, Cancellation date {3} ", invoiceNumberGrp.First().InvoiceNumber, requestId, invoiceNumberGrp.First().PaymentDate.HasValue ? invoiceNumberGrp.First().PaymentDate.Value.ToString() : "", invoiceNumberGrp.First().CancellationDate.HasValue ? invoiceNumberGrp.First().CancellationDate.Value.ToString() : ""));
            }

            return result;
        }


        /// <summary>
        /// Save failed validations
        /// </summary>
        /// <param name="failedValidation"></param>
        public void SaveFailedRequestValidation(PSSFailedProcessingFeeConfirmations failedValidation)
        {
            _failedRequestsRepo.Save(failedValidation);
        }


        /// <summary>
        /// Here we update the service request status after we have confirmed that
        /// all payments are complete
        /// </summary>
        /// <param name="requestDeets"></param>
        public void UpdateServiceRequestAfterPaymentConfirmation(IEnumerable<PSServiceRequestInvoiceValidationDTO> requestDeets)
        {
            //here we need to update the service request status request
            _serviceRequestRepo.UpdateServiceRequestsStatus(requestDeets.First().Request.Id, requestDeets.First().DefinitionLevelId, requestDeets.First().ServiceId, PSSRequestStatus.Confirmed, requestDeets.First().InvoiceId);

            //Save the transaction log payment details in PoliceCollectionLog
            _collectionLogManager.SaveCollectionLogPayment(requestDeets.First().InvoiceId, requestDeets.First().Request.Id);
        }


        /// <summary>
        /// Move request to the next defined level/stage
        /// </summary>
        public RequestFlowVM MoveRequestToTheNextStage(IEnumerable<PSServiceRequestInvoiceValidationDTO> requestDeets)
        {
            return _approvalComposition.Value.MoveRequestToNextDefinitionLevel(requestDeets);

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns></returns>
        public ReceiptDisplayVM GetInvoiceReceiptsVM(string invoiceNumber)
        {
            var receiptDetails = _serviceRequestRepo.GetReceipts(invoiceNumber);
            if (receiptDetails == null) { throw new NoRecordFoundException($"Unable to get receipt for invoice with invoice number {invoiceNumber}."); }
            var cbsUser = _iPSSRequestInvoiceManager.GetCBSUserWithInvoiceNumber(invoiceNumber);
            receiptDetails.Recipient = cbsUser.Name;
            receiptDetails.Email = cbsUser.Email;
            receiptDetails.PhoneNumber = cbsUser.PhoneNumber;
            return receiptDetails;
        }


        /// <summary>
        /// Get receipt details from database
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <param name="receiptNumber"></param>
        /// <returns>ReceiptDetailsVM</returns>
        public ReceiptDetailsVM GetInvoiceReceiptVM(string invoiceNumber, string receiptNumber)
        {
            var receiptDetails = _serviceRequestRepo.GetReceipt(invoiceNumber, receiptNumber);
            if(receiptDetails == null) { throw new NoRecordFoundException($"Unable to get receipt for invoice with invoice number {invoiceNumber} and receipt number {receiptNumber}."); }
            var cbsUser = _iPSSRequestInvoiceManager.GetCBSUserWithInvoiceNumber(invoiceNumber);
            receiptDetails.Recipient = cbsUser.Name;
            receiptDetails.Email = cbsUser.Email;
            receiptDetails.PhoneNumber = cbsUser.PhoneNumber;
            receiptDetails.Address = (cbsUser.IsAdministrator) ? receiptDetails.Address : _iCBSUserTaxEntityProfileLocationManager.GetCBSUserLocationWithId(cbsUser.Id).Address;
            return receiptDetails;
        }



        /// <summary>
        /// Validate that the request fee has been fully paid
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>PSServiceRequestInvoiceValidationDTO</returns>
        public IEnumerable<PSServiceRequestInvoiceValidationDTO> ValidateRequestFeeFullyPaid(long requestId)
        {
            //Get the service request detail for request not application fee
            IEnumerable<PSServiceRequestInvoiceValidationDTO> result = _serviceRequestRepo.GetServiceRequestDetailsWithRequestId(requestId);
            if (result == null)
            { throw new NoInvoicesMatchingTheParametersFoundException("404 for PSS application fee. Request Id " + requestId); }

            if (result.First().AmountDue > 0.00m)
            { throw new InvoiceHasPaymentException("Invoice has pending payments Request Id " + requestId); }


            if (result.First().InvoiceStatus == InvoiceStatus.WriteOff)
            {
                throw new InvoiceCancelledException(string.Format("Invoice has been written off invoice number {0}, request Id {1}, Payment date {2}, Cancellation date {3} ", result.First().InvoiceNumber, requestId, result.First().PaymentDate.HasValue ? result.First().PaymentDate.Value.ToString() : "", result.First().CancellationDate.HasValue ? result.First().CancellationDate.Value.ToString() : ""));
            }


            if (((PSSRequestStatus)result.First().Request.Status) != PSSRequestStatus.Pending)
            {
                throw new NoInvoicesMatchingTheParametersFoundException(string.Format("A record for this request was retrieved but the request status {0} doesn't match pending request fee payment, expected status {1}, request Id {2}", result.First().Request.Status, PSSRequestStatus.Pending, requestId));
            }

            return result;
        }


        /// <summary>
        /// We check if the service request has other service requests that have not been confirmed or approved
        /// <para>If we have service requests that have not been confirmed or approved then the request is not
        /// eligible to proceed to the next actionable work flow state</para>
        /// </summary>
        /// <param name="entireServiceRequest"></param>
        /// <param name="invoiceNumberGrp"></param>
        /// <returns>bool</returns>
        public bool HasPendingConfirmation(IEnumerable<PSServiceRequestInvoiceValidationDTO> entireServiceRequest, IEnumerable<PSServiceRequestInvoiceValidationDTO> invoiceNumberServiceRequestGrp)
        {
            return _approvalComposition.Value.CheckIfRequestHasPendingConfirmations(entireServiceRequest, invoiceNumberServiceRequestGrp);
        }



        /// <summary>
        /// Start unit of work for request processing after payment has been confirmed
        /// </summary>
        public void StartProcessingForRequestInvoicePaymentNotification()
        {
            _serviceRequestRepo.StartUOW();
        }



        /// <summary>
        /// End unit of work for request processing after payment has been confirmed
        /// </summary>
        public void EndProcessingForRequestInvoicePaymentNotification()
        {
            _serviceRequestRepo.EndUOW();
        }


        /// <summary>
        /// Roll back transaction
        /// </summary>
        public void RollBackTransaction()
        {
            _serviceRequestRepo.RollBackAllTransactions();
        }


    }
}