using Newtonsoft.Json;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.PaymentProviderHandlers.Contracts;
using Parkway.CBS.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;

namespace Parkway.CBS.Core.PaymentProviderHandlers
{
    public class PaymentProviderHandler : BaseHandler, IPaymentProviderHandler
    {
        private readonly Lazy<ICoreInvoiceService> _coreInvoiceService;
        private readonly Lazy<ICorePaymentService> _corePaymentService;

        public PaymentProviderHandler(Lazy<ICoreInvoiceService> coreInvoiceService, Lazy<ICorePaymentService> corePaymentService)
        {
            _coreInvoiceService = coreInvoiceService;
            _corePaymentService = corePaymentService;
        }


        public APIResponse DoValidation(ValidationRequest model, ExternalPaymentProviderVM paymentProvider, dynamic parameters = null)
        {
            HttpStatusCode httpStatusCode = System.Net.HttpStatusCode.BadRequest;
            string errorCode = ResponseCodeLang.generic_exception_code;
            List<ErrorModel> errors = new List<ErrorModel>(1);
            try
            {
                //get external third party payment provider
                string hashString = model.InvoiceNumber + paymentProvider.ClientID;

                if (!CheckHash(hashString, paymentProvider.ClientSecret, model.Signature))
                {
                    Logger.Error("Could not compute signature hash " + hashString);
                    errors.Add(new ErrorModel { FieldName = "Signature", ErrorMessage = ErrorLang.couldnotcomputehash().ToString() });
                    errorCode = ResponseCodeLang.could_not_compute_signature;
                    httpStatusCode = HttpStatusCode.Forbidden;
                    throw new UserNotAuthorizedForThisActionException();
                }

                //after all done do validation
                InvoiceGeneratedResponseExtn invoiceDetails = _coreInvoiceService.Value.GetInvoiceDetailsForPaymentView(model.InvoiceNumber);
                if (invoiceDetails == null) { throw new NoRecordFoundException(); }

                if (invoiceDetails.AmountDue <= 0.00m)
                {
                    return new APIResponse
                    {
                        ResponseObject = new List<ErrorModel> { { new ErrorModel { ErrorMessage = Lang.Lang.invoicealreadypaidfor.ToString(), FieldName = "Invoice" } } },
                        StatusCode = httpStatusCode,
                        Error = true,
                        ErrorCode = ResponseCodeLang.invoice_already_paid_for,
                    };
                }

                //do check if validation check is required
                if (invoiceDetails.HasPaymentProviderValidationConstraint)
                {
                    if (_coreInvoiceService.Value.CheckForValidationConstraint(invoiceDetails.MDAId, invoiceDetails.RevenueHeadID, paymentProvider.Id))
                    {
                        Logger.Error(string.Format("{0} is restricted from MDA {1} and Revenue head {2} ", paymentProvider.Name, invoiceDetails.MDAId, invoiceDetails.RevenueHeadID));
                        errors.Add(new ErrorModel { FieldName = "Invoice", ErrorMessage = ErrorLang.usernotauthorized().ToString() });
                        errorCode = ResponseCodeLang.user_not_authorized;
                        httpStatusCode = HttpStatusCode.Forbidden;
                        throw new UserNotAuthorizedForThisActionException();
                    }
                }

                string productItemCode = string.IsNullOrEmpty(invoiceDetails.RevenueHeadSettlementCode) ? invoiceDetails.MDASettlementCode : invoiceDetails.RevenueHeadSettlementCode;

                return new APIResponse
                {
                    ResponseObject = new InvoiceValidationModel
                    {
                        ResponseCode = ResponseCodeLang.ok,
                        Amount = Math.Round(invoiceDetails.AmountDue, 2) + 0.00m,
                        PayerId = invoiceDetails.PayerId,
                        Email = invoiceDetails.Email,
                        InvoiceNumber = model.InvoiceNumber,
                        PhoneNumber = invoiceDetails.PhoneNumber,
                        Recipient = invoiceDetails.Recipient,
                        ResponseDescription = string.Format("{0} {1}", invoiceDetails.InvoiceTitle, invoiceDetails.InvoiceDesc),
                        SettlementCode = string.IsNullOrEmpty(invoiceDetails.RevenueHeadSettlementCode) ? invoiceDetails.MDASettlementCode : invoiceDetails.RevenueHeadSettlementCode,
                        SettlementType = invoiceDetails.RevenueHeadSettlementType == 0 ? invoiceDetails.MDASettlementType : invoiceDetails.RevenueHeadSettlementType,
                        IssuerName = invoiceDetails.MDAName,
                        ServiceId = invoiceDetails.RevenueHeadServiceId
                    },
                    StatusCode = HttpStatusCode.OK,
                };
            }
            catch (UserNotAuthorizedForThisActionException) { }
            catch (NoRecordFoundException)
            {
                errors.Add(new ErrorModel { FieldName = "InvoiceNumber", ErrorMessage = ErrorLang.invoice404().ToString() });
                errorCode = ResponseCodeLang.invoice_404;
                httpStatusCode = HttpStatusCode.Forbidden;
            }
            catch (Exception exception) { Logger.Error(exception, exception.Message); }

            return new APIResponse
            {
                ErrorCode = errorCode,
                ResponseObject = errors,
                StatusCode = httpStatusCode,
                Error = true
            };

        }        


        /// <summary>
        /// Do payment sychronization
        /// </summary>
        /// <param name="validationModel"></param>
        /// <param name="paymentProvider"></param>
        /// <param name="parameters"></param>
        /// <returns>APIResponse</returns>
        public APIResponse DoSynchronization(PaymentNotification model, ExternalPaymentProviderVM paymentProvider, dynamic parameters = null)
        {
            HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest;
            string errorCode = ResponseCodeLang.generic_exception_code;
            List<ErrorModel> errors = new List<ErrorModel>(1);

            try
            {
                string hashString = model.InvoiceNumber + model.PaymentRef + model.AmountPaid.ToString("F") + model.PaymentDate + model.Channel;
                if (!CheckHash(hashString, paymentProvider.ClientSecret, model.Signature))
                {
                    Logger.Error("Could not compute signature hash " + hashString);
                    errors.Add(new ErrorModel { FieldName = "Signature", ErrorMessage = ErrorLang.couldnotcomputehash().ToString() });
                    errorCode = ResponseCodeLang.could_not_compute_signature;
                    httpStatusCode = HttpStatusCode.Forbidden;
                    throw new UserNotAuthorizedForThisActionException();
                }

                if (Math.Round(model.AmountPaid, 2) < 0.01m)
                {
                    errors.Add(new ErrorModel { ErrorMessage =  ErrorLang.invoiceamountistoosmall(model.AmountPaid).ToString(), FieldName = "AmountPaid" } );
                    throw new AmountTooSmallException { };
                }

                string bankCode = model.BankCode;
                var banks = Util.GetListOfObjectsFromJSONFile<BankVM>(SettingsFileNames.Banks.ToString());
                var bankItem = banks.Where(b => b.Code == model.BankCode).FirstOrDefault();
                if (bankItem != null) { bankCode = bankItem.Code; }

                PaymentChannel channel = GetChannel(model.Channel);
                PaymentMethods methodType = GetMethod(model.PaymentMethod);

                InvoiceValidationResponseModel response = _corePaymentService.Value.UpdatePayment(new TransactionLogVM
                {
                    Channel = (int)channel,
                    InvoiceNumber = model.InvoiceNumber,
                    PaymentReference = model.PaymentRef,
                    RetrievalReferenceNumber = model.TransactionRef,
                    AmountPaid = Math.Round(model.AmountPaid, 2),
                    PaymentDate = DateTime.ParseExact(model.PaymentDate.Trim(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                    TransactionDate = DateTime.ParseExact(model.TransactionDate.Trim(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                    RequestDump = model.RequestDump,
                    Bank = model.BankName,
                    BankBranch = model.BankBranch,
                    BankCode = model.BankCode,
                    PaymentMethod = methodType.ToString(),
                    PaymentMethodId = (int)methodType,
                    PaymentProvider = paymentProvider.Id,
                    AgentFee = model.AgentFee,
                    AllowAgentFeeAddition = paymentProvider.AllowAgentFeeAddition
                }, (PaymentProvider)paymentProvider.Id);

                return new APIResponse
                {
                    ResponseObject = new { ResponseCode = ResponseCodeLang.ok, ResponseDescription = Lang.Lang.paymentnotificationsuccessful.ToString(), ReceiptNumber = response.ReceiptNumber, MDAName = response.MDAName, RevenueHead = response.RevenueHead, response.PayerName }, StatusCode = HttpStatusCode.OK,
                };

            }
            catch (PaymentNoficationAlreadyExistsException exception)
            {
                Logger.Information(string.Format("A payment with the given payment ref already exists exception message {0} | ref {1}", exception.Message, model.PaymentRef));
                return new APIResponse { StatusCode = HttpStatusCode.OK, ResponseObject = new { ResponseCode = ResponseCodeLang.payment_already_processed, ResponseDescription = Lang.Lang.paymentnotificationalreadyprocess.ToString() } };
            }
            catch (NoInvoicesMatchingTheParametersFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.invoice404(model.InvoiceNumber).ToString(), FieldName = "InvoiceNumber" });
                errorCode = ResponseCodeLang.invoice_404;
            }
            catch (AmountTooSmallException) { }
            catch (UserNotAuthorizedForThisActionException) { }
            catch (NoRecordFoundException)
            {
                errors.Add(new ErrorModel { FieldName = "InvoiceNumber", ErrorMessage = ErrorLang.invoice404().ToString() });
                errorCode = ResponseCodeLang.invoice_404;
                httpStatusCode = HttpStatusCode.Forbidden;
            }
            catch (PartPaymentNotAllowedException)
            {
                errors.Add(new ErrorModel { FieldName = nameof(model.InvoiceNumber), ErrorMessage = ErrorLang.nopartpaymentsallow().ToString() });
                errorCode = ResponseCodeLang.no_part_payments_allowed;
                httpStatusCode = HttpStatusCode.Forbidden;
            }
            catch (InvoiceAlreadyPaidForException exception) 
            {
                Logger.Error(exception, exception.Message);
                errors.Add(new ErrorModel { FieldName = "InvoiceNumber", ErrorMessage = ErrorLang.invoiceFullyPaid(model.InvoiceNumber).ToString() });
                errorCode = ResponseCodeLang.invoice_already_paid_for;
                httpStatusCode = HttpStatusCode.Forbidden;
            }
            catch (Exception exception) { Logger.Error(exception, exception.Message); }

            return new APIResponse
            {
                ErrorCode = errorCode,
                ResponseObject = errors,
                StatusCode = httpStatusCode,
                Error = true
            };
        }
        
    }
}