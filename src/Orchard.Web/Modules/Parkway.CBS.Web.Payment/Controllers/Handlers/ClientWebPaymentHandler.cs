using Newtonsoft.Json;
using Orchard;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Module.Web.Controllers.CommonHandlers.HelperHandlers.Contracts;
using Parkway.CBS.Module.Web.Controllers.Handlers;
using Parkway.CBS.Module.Web.Controllers.Handlers.BillingImpl.Contracts;
using Parkway.CBS.Web.Payment.Controllers.Handlers.Contracts;
using Parkway.ThirdParty.Payment.Processor.Models;
using Parkway.ThirdParty.Payment.Processor.Processors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Web.Payment.Controllers.Handlers
{
    public class ClientWebPaymentHandler : CommonBaseHandler, IClientWebPaymentHandler
    {
        private readonly IOrchardServices _orchardServices;
        private readonly ITaxEntityCategoryManager<TaxEntityCategory> _taxCategoriesRepository;
        private readonly ICoreCollectionService _coreCollectionService;
        //private readonly IEnumerable<IMDAFilter> _dataFilters;

        private readonly IAdminSettingManager<ExpertSystemSettings> _settingsRepository;
        private readonly IHandlerHelper _handlerHelper;
        private readonly IFormControlRevenueHeadManager<FormControlRevenueHead> _formRevenueHeadRepository;
        private readonly IFormControlsManager<FormControl> _formcontrolsRepository;
        private readonly IEnumerable<IBillingImpl> _billingImpls;

        private readonly ICoreUserService _coreUserService;
        private readonly ICorePaymentService _corePaymentService;
        private readonly ICoreInvoiceService _coreInvoiceService;


        public ClientWebPaymentHandler(IOrchardServices orchardServices, ITaxEntityCategoryManager<TaxEntityCategory> taxCategoriesRepository, ICoreCollectionService coreCollectionService, IAdminSettingManager<ExpertSystemSettings> settingsRepository, IHandlerHelper handlerHelper, ICoreUserService coreUserService, IFormControlRevenueHeadManager<FormControlRevenueHead> formRevenueHeadRepository, IFormControlsManager<FormControl> formcontrolsRepository, IEnumerable<IBillingImpl> billingImpls, ICorePaymentService corePaymentService, ICoreInvoiceService coreInvoiceService) : base(orchardServices, settingsRepository, handlerHelper)
        {
            _coreCollectionService = coreCollectionService;
            _taxCategoriesRepository = taxCategoriesRepository;
            _orchardServices = orchardServices;
            _settingsRepository = settingsRepository;
            _handlerHelper = handlerHelper;
            _coreUserService = coreUserService;
            _formRevenueHeadRepository = formRevenueHeadRepository;
            _formcontrolsRepository = formcontrolsRepository;
            _billingImpls = billingImpls;
            _corePaymentService = corePaymentService;
            _coreInvoiceService = coreInvoiceService;
        }


        /// <summary>
        /// Get model for web pay direct
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>PayDirectWebPaymentFormModel</returns>
        public PayDirectWebPaymentFormModel GetPayDirectWebFormModel(string stateName, PayDirectWebPaymentRequestModel tokenModel)
        {
            PayDirectConfigurations payDirectConfig = PaymentProcessorUtil.GetConfigurations<PayDirectConfigurations>(Util.GetAppRemotePath(), stateName);
            InvoiceGeneratedResponseExtn invoiceDetails = _coreInvoiceService.GetInvoiceDetailsForPaymentView(tokenModel.InvoiceNumber);

            if (invoiceDetails == null) { throw new NoInvoicesMatchingTheParametersFoundException { }; }

            decimal fee = _corePaymentService.GetFeeToBeAppliedForPayDirectWeb(payDirectConfig, invoiceDetails.AmountDue);

            string transctionRef = _corePaymentService.GetTransactionRefForPayDirectWeb(payDirectConfig, invoiceDetails.AmountDue, fee, tokenModel);
            PayDirectWebPaymentFormModel model = _corePaymentService.GetPayDirectWebFormParameters(payDirectConfig, transctionRef, invoiceDetails.AmountDue);
            model.CustomerName = invoiceDetails.Recipient;
            model.CustId = invoiceDetails.PhoneNumber;
            return model;
        }


        /// <summary>
        /// process pay direct web payment notif
        /// </summary>
        /// <param name="stateName"></param>
        /// <param name="model"></param>
        /// <returns>PayDirectWebPaymentValidationResponse</returns>
        public PayDirectWebPaymentValidationResponse ProcessPaymentNotifRequestForPayDirectWeb(string stateName, PayDirectWebPaymentResponseModel model)
        {
            //get state pay direct integration setup details
            PayDirectConfigurations payDirectConfig = PaymentProcessorUtil.GetConfigurations<PayDirectConfigurations>(Util.GetAppRemotePath(), stateName);
            //get the web request
            WebPaymentRequest request = null;// _corePaymentService.GetWebPaymentRequest(model.txnref, PaymentChannelp.PayDirectWeb);
            if (request == null) { throw new NoRecordFoundException("No Payment request record found"); }
            //get the transaction ref and do a call to confirm transaction details
            PayDirectWebServerResponse obj = _corePaymentService.GetPayDirectWebTransaction(payDirectConfig, model);
            //treat response accordingly
            request.ResponseCode = obj.ResponseCode;
            request.ResponseDump = JsonConvert.SerializeObject(model);
            if (obj.ResponseCode != "00")
            { return new PayDirectWebPaymentValidationResponse { PaymentNotification = new PaymentNotification { ResponseMessage = obj.ResponseDescription } }; }

            decimal amountPaidLessTransactionFee = _corePaymentService.GetAmountPaidLessTransactionFeeForPayDirectWeb(payDirectConfig, obj.Amount, request.FeeApplied);
            try
            {
                InvoiceValidationResponseModel response = _corePaymentService.UpdatePayment(new TransactionLogVM
                {
                    Channel = (int)PaymentProvider.PayDirect,
                    InvoiceNumber = request.InvoiceNumber,
                    PaymentReference = obj.PaymentReference,
                    AmountPaid = amountPaidLessTransactionFee,
                    PaymentDate = obj.TransactionDate,
                    TransactionDate = obj.TransactionDate,
                    RequestDump = JsonConvert.SerializeObject(obj),
                    RetrievalReferenceNumber = obj.RetrievalReferenceNumber,
                }, PaymentProvider.PayDirect);
            }
            catch (PaymentNoficationAlreadyExistsException) { }
            catch (InvoiceAlreadyPaidForException) { throw; }
            catch (PartPaymentNotAllowedException) { throw; }
            //string getTransactionURL

            return new PayDirectWebPaymentValidationResponse
            {
                PaymentWasProcessed = true,
                CallBackURL = request.CallBackURL,
                PaymentNotification = new PaymentNotification
                {
                    AmountPaid = amountPaidLessTransactionFee,
                    PaymentRef = obj.PaymentReference,
                    TransactionRef = request.TransactionReference,
                    //Channel = PaymentChannel.PayDirectWeb.ToString(),
                    InvoiceNumber = request.InvoiceNumber,
                    PaymentDate = obj.TransactionDate.ToString("dd'/'MM'/'yyyy hh:mm:ss"),
                    TransactionDate = obj.TransactionDate.ToString("dd'/'MM'/'yyyy hh:mm:ss"),
                    Mac = string.Format("{0}{1}", request.InvoiceNumber, obj.PaymentReference),
                    ResponseCode = obj.ResponseCode == "00" ? AppSettingsConfigurations.GetSettingsValue("PaymentNotificationResponseCode") : "9999",
                    ResponseMessage = obj.ResponseDescription,
                }
            };
        }


    }
}