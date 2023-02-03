using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.API.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.API.Controllers.Handlers
{
    public class APIRequestHandler : IAPIRequestHandler
    {
        private readonly ICorePSServiceRequest _coreServiceRequest;
        private readonly ICoreRequestStatusLog _coreRequestLog;

        public ILogger Logger { get; set; }


        public APIRequestHandler(ICorePSServiceRequest coreServiceRequest, ICoreRequestStatusLog coreRequestLog)
        {
            _coreServiceRequest = coreServiceRequest;
            Logger = NullLogger.Instance;
            _coreRequestLog = coreRequestLog;
        }


        /// <summary>
        /// this method checks to confirm that the present stage fee for this 
        /// request has been fully paid for
        /// </summary>
        /// <param name="requestToken"></param>
        /// <returns>IEnumerable{PSServiceRequestInvoiceValidationDTO}</returns>
        public IEnumerable<PSServiceRequestInvoiceValidationDTO> ConfirmRequestInvoiceFee(string requestToken, string invoiceNumber)
        {
            RequestCallbackModel callbackModel = new RequestCallbackModel { RequestId = Int32.Parse(requestToken) };
            // JsonConvert.DeserializeObject<RequestCallbackModel>(Util.LetsDecrypt(requestToken));
            try
            {
                return _coreServiceRequest.ValidateProcessingFeeFullyPaid(callbackModel.RequestId, invoiceNumber);
            }
            catch (NoInvoicesMatchingTheParametersFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                _coreServiceRequest.SaveFailedRequestValidation(new PSSFailedProcessingFeeConfirmations { RequestId = callbackModel.RequestId, Message = string.Format("Token {0} | {1}", requestToken, exception.Message) });
            }
            catch (InvoiceCancelledException exception)
            {
                Logger.Error(exception, exception.Message);
                _coreServiceRequest.SaveFailedRequestValidation(new PSSFailedProcessingFeeConfirmations { RequestId = callbackModel.RequestId, Message = string.Format("Token {0} | {1}", requestToken, exception.Message) });
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _coreServiceRequest.SaveFailedRequestValidation(new PSSFailedProcessingFeeConfirmations { RequestId = callbackModel.RequestId, Message = string.Format("Token {2} | {0} | {1}", exception.Message, exception.StackTrace, requestToken), NeedsAction = true });
            }

            throw new Exception("Could not confirm processing fee");
        }



        /// <summary>
        /// When the request fee has been confirmed, we update the service request status
        /// </summary>
        /// <param name="requestDeet"></param>
        public void UpdateServiceRequest(IEnumerable<PSServiceRequestInvoiceValidationDTO> requestDeet)
        {
            try
            {
                _coreServiceRequest.UpdateServiceRequestAfterPaymentConfirmation(requestDeet);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _coreServiceRequest.SaveFailedRequestValidation(new PSSFailedProcessingFeeConfirmations { InvoiceNumber = requestDeet.First().InvoiceNumber, RequestId = requestDeet.First().Request.Id, Message = string.Format("Failed to create request after payment confirmation {0} | {1}", exception.Message, exception.StackTrace), NeedsAction = true });
                throw;
            }
        }


        /// <summary>
        /// Update the status log after payment has been confirmed
        /// </summary>
        /// <param name="requestDeet"></param>
        public void UpdateRequestStatusLog(IEnumerable<PSServiceRequestInvoiceValidationDTO> requestDeet)
        {
            try
            {
                _coreRequestLog.UpdateStatusLogAfterPaymentConfirmation(requestDeet.ElementAt(0).Request.Id, requestDeet.ElementAt(0).DefinitionLevelId, requestDeet.ElementAt(0).InvoiceId);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _coreServiceRequest.SaveFailedRequestValidation(new PSSFailedProcessingFeeConfirmations { InvoiceNumber = requestDeet.First().InvoiceNumber, RequestId = requestDeet.First().Request.Id, Message = string.Format("Failed to create request after payment confirmation {0} | {1}", exception.Message, exception.StackTrace), NeedsAction = true });
                throw;
            }
        }


        /// <summary>
        /// check if request can be moved to the next request stage
        /// </summary>
        /// <param name="result"></param>
        /// <param name="invoiceNumberGrp"></param>
        /// <returns>bool</returns>
        public bool SkipRequestMoveToNextStage(IEnumerable<PSServiceRequestInvoiceValidationDTO> result, IEnumerable<PSServiceRequestInvoiceValidationDTO> invoiceNumberGrp)
        {
            return _coreServiceRequest.HasPendingConfirmation(result, invoiceNumberGrp);          
        }


        /// <summary>
        /// Here we are moving the request to the next definition level
        /// </summary>
        /// <param name="result"></param>
        public APIResponse MoveRequestToTheNextDefinedLevel(IEnumerable<PSServiceRequestInvoiceValidationDTO> requestDeet)
        {
            try
            {
                _coreServiceRequest.MoveRequestToTheNextStage(requestDeet);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _coreServiceRequest.SaveFailedRequestValidation(new PSSFailedProcessingFeeConfirmations { InvoiceNumber = requestDeet.First().InvoiceNumber, RequestId = requestDeet.First().Request.Id, Message = string.Format("Failed to create request after payment confirmation {0} | {1}", exception.Message, exception.StackTrace), NeedsAction = true });
                return new APIResponse { Error = true, ResponseObject = exception.Message };
            }
            return new APIResponse { };
        }



        public APIResponse DoProcessing(IEnumerable<PSServiceRequestInvoiceValidationDTO> invoiceNumberGrp, bool skipMove)
        {
            APIResponse response = new APIResponse { };
            //_coreServiceRequest.StartProcessingForRequestInvoicePaymentNotification();
            //proceed with the request processing
            UpdateServiceRequest(invoiceNumberGrp);
            //update status log
            UpdateRequestStatusLog(invoiceNumberGrp);
            //now we need to check that if this request can move to the next stage
            if (!skipMove)
            {
                return MoveRequestToTheNextDefinedLevel(invoiceNumberGrp);
            }
            //_coreServiceRequest.EndProcessingForRequestInvoicePaymentNotification();
            //move the request to the next level
            return response;
        }
    }
}