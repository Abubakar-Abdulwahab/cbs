using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Police.API.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.RequestWorkFlow.Actions.NoActionImpl.Contracts;
using Parkway.CBS.Police.Core.Services.Contracts;

namespace Parkway.CBS.Police.API.Controllers.Handlers
{
    public class RegularizationRecurringInvoiceHandler : IRegularizationRecurringInvoiceHandler
    {
        private readonly ICorePSServiceRequest _coreServiceRequest;
        private readonly ICoreRequestStatusLog _coreRequestLog;
        private readonly IPoliceCollectionLogManager<PoliceCollectionLog> _collectionLogManager;
        private readonly ICorePSSRequestInvoiceService _corePSSRequestInvoiceService;
        private readonly IEnumerable<IServiceNoActionImpl> _noActionImppl;

        ILogger Logger { get; set; }
        public RegularizationRecurringInvoiceHandler(ICorePSServiceRequest coreServiceRequest, ICoreRequestStatusLog coreRequestLog, IPoliceCollectionLogManager<PoliceCollectionLog> collectionLogManager, ICorePSSRequestInvoiceService corePSSRequestInvoiceService, IEnumerable<IServiceNoActionImpl> noActionImppl)
        {
            _coreServiceRequest = coreServiceRequest;
            _coreRequestLog = coreRequestLog;
            _collectionLogManager = collectionLogManager;
            _corePSSRequestInvoiceService = corePSSRequestInvoiceService;
            _noActionImppl = noActionImppl;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// this method checks to confirm that the invoie generated for this request has been fully paid for
        /// </summary>
        /// <param name="requestToken"></param>
        /// <param name="invoiceNumber"></param>
        /// <returns>IEnumerable{PSServiceRequestInvoiceValidationDTO}</returns>
        public IEnumerable<PSServiceRequestInvoiceValidationDTO> ConfirmRequestInvoiceFee(string requestToken, string invoiceNumber)
        {
            RequestCallbackModel callbackModel = new RequestCallbackModel { RequestId = Int64.Parse(requestToken) };
            try
            {
                return _corePSSRequestInvoiceService.ValidateProcessingFeeFullyPaid(callbackModel.RequestId, invoiceNumber);
            }
            catch (NoInvoicesMatchingTheParametersFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                _coreServiceRequest.SaveFailedRequestValidation(new PSSFailedProcessingFeeConfirmations { InvoiceNumber = invoiceNumber, RequestId = callbackModel.RequestId, Message = string.Format("Token {0} | {1}", requestToken, exception.Message) });
            }
            catch (InvoiceCancelledException exception)
            {
                Logger.Error(exception, exception.Message);
                _coreServiceRequest.SaveFailedRequestValidation(new PSSFailedProcessingFeeConfirmations { InvoiceNumber = invoiceNumber, RequestId = callbackModel.RequestId, Message = string.Format("Token {0} | {1}", requestToken, exception.Message) });
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _coreServiceRequest.SaveFailedRequestValidation(new PSSFailedProcessingFeeConfirmations { InvoiceNumber = invoiceNumber, RequestId = callbackModel.RequestId, Message = string.Format("Token {2} | {0} | {1}", exception.Message, exception.StackTrace, requestToken), NeedsAction = true });
            }

            throw new Exception($"Could not confirm recurring invoice for escort and guards regularization. Invoice Number - {invoiceNumber}");
        }


        /// <summary>
        /// Synchronize recurring invoice payment
        /// </summary>
        /// <param name="invoiceNumberGrp"></param>
        public void DoProcessing(IEnumerable<PSServiceRequestInvoiceValidationDTO> invoiceNumberGrp)
        {
            //proceed with the request processing
            try
            {
                //Save the transaction log payment details in PoliceCollectionLog
                _collectionLogManager.SaveCollectionLogPayment(invoiceNumberGrp.First().InvoiceId, invoiceNumberGrp.First().Request.Id);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _coreServiceRequest.SaveFailedRequestValidation(new PSSFailedProcessingFeeConfirmations { InvoiceNumber = invoiceNumberGrp.First().InvoiceNumber, RequestId = invoiceNumberGrp.First().Request.Id, Message = string.Format("Failed to log payment in collection log after payment confirmation {0} | {1}", exception.Message, exception.StackTrace), NeedsAction = true });
                throw;
            }

            //update status log
            try
            {
                _coreRequestLog.UpdateStatusLogAfterPaymentConfirmation(invoiceNumberGrp.ElementAt(0).Request.Id, invoiceNumberGrp.ElementAt(0).InvoiceId);

                foreach (var impl in _noActionImppl)
                {
                    if (impl.GetServiceType == PSSServiceTypeDefinition.EscortRegularization)
                    {
                        impl.DoServiceImplementationWorkForNoAction(invoiceNumberGrp, invoiceNumberGrp.ElementAt(0).Request.ApprovalNumber);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _coreServiceRequest.SaveFailedRequestValidation(new PSSFailedProcessingFeeConfirmations { InvoiceNumber = invoiceNumberGrp.First().InvoiceNumber, RequestId = invoiceNumberGrp.First().Request.Id, Message = string.Format("Failed to create request after payment confirmation {0} | {1}", exception.Message, exception.StackTrace), NeedsAction = true });
                throw;
            }
        }
    }
}