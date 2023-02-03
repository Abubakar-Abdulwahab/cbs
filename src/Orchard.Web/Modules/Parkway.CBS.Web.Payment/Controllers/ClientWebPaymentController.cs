using Orchard;
using System.Web.Mvc;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Users.Events;
using Orchard.Users.Services;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Module.Web.Controllers;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Web.Payment.Controllers.Handlers.Contracts;
using Orchard.Themes;
using Parkway.CBS.Core.Exceptions;
using Newtonsoft.Json;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Core.HelperModels;
using Orchard.Mvc.AntiForgery;
using System;
using Parkway.CBS.Core.Lang;
using Parkway.ThirdParty.Payment.Processor.Models;

namespace Parkway.CBS.Web.Payment.Controllers
{
    [Themed(false)]
    public class ClientWebPaymentController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IOrchardServices _orchardServices;
        private readonly IMembershipService _membershipService;
        private readonly ICBSUserManager<CBSUser> _cbsUserService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMembershipValidationService _membershipValidationService;
        private readonly IUserEventHandler _userEventHandler;
        private readonly IClientWebPaymentHandler _handler;

        public ClientWebPaymentController(IOrchardServices orchardServices, IUserService userService, IMembershipService membershipService, ICBSUserManager<CBSUser> cbsUserService, IAuthenticationService authenticationService, IMembershipValidationService membershipValidationService, IUserEventHandler userEventHandler, IClientWebPaymentHandler handler) : base (orchardServices, userService, membershipService, cbsUserService, authenticationService, membershipValidationService, userEventHandler, handler)
        {
            _orchardServices = orchardServices;
            Logger = NullLogger.Instance;
            _authenticationService = authenticationService;
            _userService = userService;
            _membershipService = membershipService;
            _membershipValidationService = membershipValidationService;
            _userEventHandler = userEventHandler;
            _cbsUserService = cbsUserService;
            _handler = handler;
        }


        public ActionResult PayDirectClientMock()
        {
            return View();
        }

        //remove
        /// <summary>
        /// Route Name : WebPaymentTransientPage
        /// This call is only to be used by third party application that want to make payment via this platform
        /// <para>https://weblogs.asp.net/bleroy/opting-out-of-anti-forgery-validation-in-orchard</para>
        /// </summary>
        /// <param name="InvoiceNumber"></param>
        /// <param name="CallBackURL"></param>
        [Themed]
        [HttpPost]
        [ValidateAntiForgeryTokenOrchard(false)]
        public ActionResult WebPaymentTransientPage(PayDirectWebPaymentRequestModel model)
        {
            RedirectToWebPayModel vm = new RedirectToWebPayModel { };
            try
            {
                if (string.IsNullOrEmpty(model.InvoiceNumber) /*|| string.IsNullOrEmpty(model.TimeStamp) || string.IsNullOrEmpty(model.ClientId)*/)
                {
                    vm.Message = "Invoice number is empty";
                    throw new NoInvoicesMatchingTheParametersFoundException("invoice number is empty");
                }
                else { vm.Message = "Processing your request. Redirecting you to payment page shortly"; }
                //compute token
                vm.Token = Util.LetsEncrypt(JsonConvert.SerializeObject(model), AppSettingsConfigurations.EncryptionSecret);
                return View(vm);
            }
            catch (NoInvoicesMatchingTheParametersFoundException exception)
            {
                Logger.Error(exception, exception.Message);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception redirecting to pay direct web payment MSG: " + exception.Message));
                vm.Message = ErrorLang.genericexception().ToString();
            }
            return View(vm);
        }


        /// <summary>
        /// Get list of ref data attached to this state
        /// </summary>
        /// <param name="stateName"></param>
        /// <returns>JsonResult</returns>
        public JsonResult GetWebPayDirectModel(string token)
        {
            //lets get the form model for pay direct web 
            Logger.Information("Getting form details for pay direct web");
            APIResponse response = null;
            string smodel = string.Empty;
            try
            {
                //check for empty values
                if (string.IsNullOrEmpty(token))
                {
                    response = new APIResponse { Error = true, ResponseObject = ErrorLang.invoice404().ToString() };
                }
                else
                {
                    //decrypt token
                    smodel = Util.LetsDecrypt(token, AppSettingsConfigurations.EncryptionSecret);
                    if (string.IsNullOrEmpty(smodel))
                    {
                        Logger.Error("Could not deserialize string model " + smodel);
                        response = new APIResponse { Error = true, ResponseObject = ErrorLang.invoice404().ToString() };
                        throw new Exception();
                    }
                    //deserialize token
                    PayDirectWebPaymentRequestModel tokenModel = JsonConvert.DeserializeObject<PayDirectWebPaymentRequestModel>(smodel);
                    if (tokenModel == null)
                    {
                        Logger.Error("Could not deserialize string model " + smodel);
                        response = new APIResponse { Error = true, ResponseObject = ErrorLang.invoice404().ToString() };
                        throw new Exception();
                    }

                    string stateName = _orchardServices.WorkContext.CurrentSite.SiteName;
                    PayDirectWebPaymentFormModel model = _handler.GetPayDirectWebFormModel(stateName, tokenModel);
                    string innerForm =
                        "<input name = 'product_id' type = 'hidden' value = '" + model.ProductId + "' />" +
                        "<input name = 'pay_item_id' type = 'hidden' value = '" + model.PayItemId + "' />" +
                        "<input name = 'amount' type = 'hidden' value = '" + model.Amount + "' />" +
                        "<input name = 'currency' type = 'hidden' value = '" + model.Currency + "' />" +
                        "<input name = 'site_redirect_url' type = 'hidden' value = '" + model.SiteRedirectURL + "'/>" +
                        "<input name = 'site_name' type = 'hidden' value = '" + stateName + "'/>" +
                        "<input name = 'txn_ref' type = 'hidden' value = '" + model.TxnRef + "'/>" +
                        "<input name = 'cust_id_desc' type = 'hidden' value = '" + model.CustId + "' >" +
                        "<input name = 'cust_id' type = 'hidden' value = '" + model.CustId + "' >" +
                        "<input name = 'cust_name' type = 'hidden' value = '" + model.CustomerName + "' >" +
                        "<input name = 'cust_name_desc' type = 'hidden' value = '" + model.CustomerName + "' >" +
                        "<input name = 'hash' type = 'hidden' value = '" + model.Hash + "' />";

                    response = new APIResponse { ResponseObject = new { InnerForm = innerForm, PostURL = model.ActionURL } };
                }
            }
            catch (NoInvoicesMatchingTheParametersFoundException exception)
            {
                Logger.Error(exception, string.Format("NoInvoicesMatchingTheParametersFoundException for WebPayDirect model: {0}. MSG {1}", string.IsNullOrEmpty(smodel) ? "" : smodel, exception.Message));
                if (response == null)
                { response = new APIResponse { ResponseObject = ErrorLang.invoice404().ToString(), Error = true }; }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("IE for WebPayDirect Invnum: {0}. MSG {1}", string.IsNullOrEmpty(smodel) ? "" : smodel, exception.Message));
                if (response == null)
                { response = new APIResponse { ResponseObject = ErrorLang.genericexception().ToString(), Error = true }; }
            }
            return Json(response, JsonRequestBehavior.DenyGet);
        }


        //remove
        [HttpPost]
        [ValidateAntiForgeryTokenOrchard(false)]
        public ActionResult PayDirectWebMock()
        {
            return View();
        }


        /// <summary>
        /// RouteName: PayDirectPaymentResponse
        /// Action for web payment response from pay direct web
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryTokenOrchard(false)]
        public ActionResult PayDirectPaymentResponse(PayDirectWebPaymentResponseModel model)
        {
            try
            {
                //log the request
                Logger.Information("PAY DIRECT WEB PAYMENT NOTIF: MODEL " + JsonConvert.SerializeObject(model));
                string stateName = _orchardServices.WorkContext.CurrentSite.SiteName;
                PayDirectWebPaymentValidationResponse response = _handler.ProcessPaymentNotifRequestForPayDirectWeb(stateName, model);
                if (!string.IsNullOrEmpty(response.CallBackURL))
                {
                    return Content(
                    "<form id='clientRedirectForm' action='" + response.CallBackURL + "' method='POST'>" +
                    "<input name = 'InvoiceNumber' type = 'hidden' value = '" + response.PaymentNotification.InvoiceNumber + "' />" +
                    "<input name = 'PaymentRef' type = 'hidden' value = '" + response.PaymentNotification.PaymentRef + "' />" +
                    "<input name = 'PaymentDate' type = 'hidden' value = '" + response.PaymentNotification.PaymentDate + "' />" +
                    "<input name = 'BankCode' type = 'hidden' value = '" + response.PaymentNotification.BankCode + "' />" +
                    "<input name = 'BankName' type = 'hidden' value = '" + response.PaymentNotification.BankName + "' />" +
                    "<input name = 'BankBranch' type = 'hidden' value = '" + response.PaymentNotification.BankBranch + "' />" +
                    "<input name = 'AmountPaid' type = 'hidden' value = '" + response.PaymentNotification.AmountPaid + "' />" +
                    "<input name = 'TransactionDate' type = 'hidden' value = '" + response.PaymentNotification.TransactionDate + "' />" +
                    "<input name = 'TransactionRef' type = 'hidden' value = '" + response.PaymentNotification.TransactionRef + "' />" +
                    "<input name = 'Channel' type = 'hidden' value = '" + response.PaymentNotification.Channel + "' />" +
                    "<input name = 'ResponseCode' type = 'hidden' value = '" + response.PaymentNotification.ResponseCode + "' />" +
                    "<input name = 'ResponseMessage' type = 'hidden' value = '" + response.PaymentNotification.ResponseMessage + "' />" +
                    "<input name = 'Mac' type = 'hidden' value = '" + response.PaymentNotification.Mac + "' />" +
                    "</form><script>document.getElementById('clientRedirectForm').submit();</script>");
                }
                return View();
            }
            catch (NoInvoicesMatchingTheParametersFoundException exception)
            {
                Logger.Error(exception, string.Format("PDW paymentresponse. Exception :{0}", exception.Message));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("PDW paymentresponse. Exception :{0}", exception.Message));
            }
            return View();
        }
    }
}