using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.SMS.Provider.Contracts;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Parkway.CBS.Core.Utilities;
using System.Linq;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;

namespace Parkway.CBS.Police.Client.Controllers.Handlers
{
    public class PSSMakePaymentHandler : IPSSMakePaymentHandler
    {
        private readonly IOrchardServices _orchardServices;
        private readonly ICoreInvoiceService _coreInvoiceService;
        public ILogger Logger { get; set; }
        private readonly ICorePaymentService _corePaymentService;
        private readonly INotificationMessageLogManager<NotificationMessageLog> _notificationMessageLogManager;
        private readonly IEnumerable<Lazy<ISMSProvider>> _smsProvider;
        private readonly IPSSRequestInvoiceManager<PSSRequestInvoice> _iPSSRequestInvoiceManager;

        public PSSMakePaymentHandler(ICoreInvoiceService coreInvoiceService, ICorePaymentService corePaymentService, IEnumerable<Lazy<ISMSProvider>> smsProvider, IOrchardServices orchardServices, INotificationMessageLogManager<NotificationMessageLog> notificationMessageLogManager, IPSSRequestInvoiceManager<PSSRequestInvoice> iPSSRequestInvoiceManager)
        {
            _coreInvoiceService = coreInvoiceService;
            Logger = NullLogger.Instance;
            _corePaymentService = corePaymentService;
            _smsProvider = smsProvider;
            _orchardServices = orchardServices;
            _notificationMessageLogManager = notificationMessageLogManager;
            _iPSSRequestInvoiceManager = iPSSRequestInvoiceManager;
        }



        /// <summary>
        /// Get invoice details for viewing
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>InvoiceGeneratedResponseExtn</returns>
        /// <exception cref="NoInvoicesMatchingTheParametersFoundException"></exception>
        public InvoiceGeneratedResponseExtn GetInvoiceDetails(string invoiceNumber)
        {
            var result =  _coreInvoiceService.GetInvoiceDetailsForPaymentView(invoiceNumber);
            if(result == null) { throw new NoInvoicesMatchingTheParametersFoundException("Invoice 404 " + invoiceNumber); }
            result.Recipient = GetCBSUserWithInvoiceNumber(invoiceNumber).Name;
            result.HeaderObj = new HeaderObj { };
            return result;
        }


        /// <summary>
        /// Get Payment Reference using the Invoice Number
        /// </summary>
        /// <param name="InvoiceId"></param>
        /// <param name="provider"></param>
        /// <returns>string</returns>
        public APIResponse GetPaymentReferenceNumber(int InvoiceId, string InvoiceNumber, PaymentProvider provider)
        {
            try
            {
                PaymentReference paymentReference = _corePaymentService.SavePaymentReference(new PaymentReference
                {
                    PaymentProvider = (int)provider,
                    Invoice = new Invoice { Id = InvoiceId },
                    InvoiceNumber = InvoiceNumber
                });

                //Evict Payment Reference object
                _corePaymentService.EvictPaymentReferenceObject(paymentReference);

                //Call the payment reference table to get the computed reference number
                return _corePaymentService.GetPaymentReference(paymentReference.Id);
            }
            catch (CouldNotSaveRecord ex)
            {
                Logger.Error($"{ex}");
                Logger.Error(ex, $"Unable save Payment Reference for Invoice Number {InvoiceNumber}");
                return new APIResponse { Error = true, StatusCode = HttpStatusCode.BadRequest, ResponseObject = ErrorLang.genericexception().ToString() };
            }

            catch (InvoiceAlreadyPaidForException exception)
            {
                Logger.Error(exception, exception.Message);
                return new APIResponse { Error = true, StatusCode = HttpStatusCode.ResetContent, ResponseObject = ErrorLang.invoiceFullyPaid(InvoiceNumber).ToString() };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, $"Unable to get Payment Reference for Invoice Number {InvoiceNumber}");
                return new APIResponse { Error = true, StatusCode = HttpStatusCode.BadRequest, ResponseObject = ErrorLang.genericexception().ToString() };
            }
        }

        /// <summary>
        /// Get Payment details using reference number
        /// </summary>
        /// <param name="referenceNumber"></param>
        /// <returns></returns>
        public PaymentReferenceVM GetPaymentReferenceDetail(string referenceNumber)
        {
            return _corePaymentService.GetPaymentReferenceDetail(referenceNumber);
        }

        /// <summary>
        /// Save netpay payment details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<InvoiceValidationResponseModel> SavePayment(PaymentAcknowledgeMentModel model)
        {
            try
            {
                return await _corePaymentService.SaveNetpayPayment(model);
            }
            catch (PaymentNoficationAlreadyExistsException)
            {
                throw;
            }
            catch (CannotVerifyNetPayTransaction)
            {
                throw;
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Send sms notification
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cbsUser"></param>
        public void SendSMSNotification(PaymentAcknowledgeMentModel model, CBSUserVM cbsUser)
        {
            try
            {
                if(_notificationMessageLogManager.Count(x => x.NotificationType == (int)NotificationMessageType.SMS && x.Reference == model.PaymentRequestRef) == 0)
                {
                    Node node = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName)
                    .Node.Where(x => x.Key == TenantConfigKeys.IsSMSEnabled.ToString()).FirstOrDefault();

                    if (node != null && !string.IsNullOrEmpty(node.Value))
                    {
                        bool.TryParse(node.Value, out bool isSMSEnabled);
                        if (isSMSEnabled && !string.IsNullOrEmpty(cbsUser.PhoneNumber))
                        {
                            //Send sms notification
                            int providerId = 0;
                            if (!Int32.TryParse(AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.SMSProvider), out providerId))
                            {
                                providerId = (int)SMSProvider.Pulse;
                            }
                            foreach (var impl in _smsProvider)
                            {
                                if ((SMSProvider)providerId == impl.Value.GetSMSNotificationProvider)
                                {
                                    string message = $"Dear {cbsUser.Name}, your payment of {model.Amount.ToString("F")} for {model.RevenueHeadName} was successful. Receipt Number: {model.ReceiptNumber}";
                                    impl.Value.SendSMS(new List<string> { cbsUser.PhoneNumber }, message, _orchardServices.WorkContext.CurrentSite.SiteName);
                                    break;
                                }
                            }

                            if(!_notificationMessageLogManager.Save(new NotificationMessageLog { NotificationType = (int)NotificationMessageType.SMS, Reference = model.PaymentRequestRef }))
                            { throw new CouldNotSaveRecord(); }
                        }
                    }
                }
            }
            catch (CouldNotSaveRecord)
            {
                Logger.Error($"Could not save log for SMS notification with reference {model.PaymentRequestRef}.");
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
        }


        /// <summary>
        /// Gets cbs user vm for user that generated invoice with specified invoice number
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns></returns>
        public CBSUserVM GetCBSUserWithInvoiceNumber(string invoiceNumber)
        {
            return _iPSSRequestInvoiceManager.GetCBSUserWithInvoiceNumber(invoiceNumber);
        }

    }
}