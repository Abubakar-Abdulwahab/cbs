using System;
using System.Collections.Generic;
using System.Net;
using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.PaymentProviderHandlers.Contracts;

namespace Parkway.CBS.Core.PaymentProviderHandlers
{
    public class ReadyCashPaymentProviderHandler : BaseHandler, IReadyCashPaymentProviderHandler
    {
        private readonly Lazy<ICorePaymentService> _corePaymentService;

        public ReadyCashPaymentProviderHandler(Lazy<ICorePaymentService> corePaymentService)
        {
            _corePaymentService = corePaymentService;
        }


        public APIResponse RequeryTransaction(PaymentNotification model, ExternalPaymentProviderVM paymentProvider)
        {
            try
            {
                string hashString = model.InvoiceNumber + model.PaymentRef + model.Channel + paymentProvider.ClientID;
                if (!CheckHash(hashString, paymentProvider.ClientSecret, model.Signature))
                {
                    Logger.Error("Could not compute signature hash " + hashString);
                    return new APIResponse
                    {
                        Error = true,
                        ErrorCode = ResponseCodeLang.could_not_compute_signature,
                        ResponseObject = new List<ErrorModel> { { new ErrorModel { ErrorMessage = ErrorLang.couldnotcomputehash().ToString(), FieldName = "Signature" } } },
                        StatusCode = HttpStatusCode.Forbidden
                    };
                }

                PaymentChannel channel = GetChannel(model.Channel);
                TransactionLogGroup result = _corePaymentService.Value.GetTransactionLogByRetrievalReferenceNumber(model.InvoiceNumber, model.PaymentRef, paymentProvider.Id, channel);
                if (result != null)
                {
                    return new APIResponse
                    {
                        ResponseObject = new { ResponseCode = ResponseCodeLang.ok, ResponseDescription = Lang.Lang.paymentnotificationsuccessful.ToString(), ReceiptNumber = result.ReceiptNumber },
                        StatusCode = HttpStatusCode.OK,
                    };
                }
                else
                {
                    return new APIResponse
                    {
                        Error = true,
                        ErrorCode = ResponseCodeLang.record_404,
                        ResponseObject = new { ResponseCode = ResponseCodeLang.generic_exception_code.ToString(), ResponseDescription = new List<ErrorModel> { { new ErrorModel { ErrorMessage = ErrorLang.record404().ToString(), FieldName = "Payment" } } } },
                        StatusCode = HttpStatusCode.NotFound,
                    };
                }
            }
            catch (Exception exception) { Logger.Error(exception, exception.Message); }
           
            return new APIResponse
            {
                Error = true,
                ErrorCode = ResponseCodeLang.generic_exception_code,
                ResponseObject = new List<ErrorModel> { { new ErrorModel { ErrorMessage = ErrorLang.paymentreference404().ToString(), FieldName = "PaymentRef" } } },
                StatusCode = HttpStatusCode.BadRequest

            };
        }

    }
}