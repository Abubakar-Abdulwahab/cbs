using System;
using System.Web.Mvc;
using Orchard.Security;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.Models.Enums;
using Orchard.Logging;
using Parkway.CBS.Core.Utilities;
using System.Linq;
using Orchard;
using Parkway.CBS.Core.Exceptions;
using System.Threading.Tasks;
using Parkway.CBS.Core.Lang;
using System.Net.Http;
using Parkway.CBS.Police.Client.Middleware;

namespace Parkway.CBS.Police.Client.Controllers
{
    [CheckVerificationFilter(false)]
    public class MakePaymentController : BaseController
    {
        private readonly IHandler _compHandler;
        private readonly ICBSUserManager<CBSUser> _cbsUserService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IPSSMakePaymentHandler _handler;
        private readonly IOrchardServices _orchardServices;


        public MakePaymentController(IHandler compHandler,IOrchardServices orchardServices, ICBSUserManager<CBSUser> cbsUserService, IAuthenticationService authenticationService, IPSSMakePaymentHandler handler)
            : base(authenticationService, compHandler)
        {
            _cbsUserService = cbsUserService;
            _authenticationService = authenticationService;
            _handler = handler;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
            _compHandler = compHandler;
        }



        /// <summary>
        /// Route Name: P.Make.Payment
        /// </summary>
        /// <param name="invoiceNumber"></param>
        public ActionResult MakePayment(string invoiceNumber)
        {
            try
            {
                TempData = null;
                if (string.IsNullOrEmpty(invoiceNumber))
                {
                    throw new NoInvoicesMatchingTheParametersFoundException();
                }

                InvoiceGeneratedResponseExtn viewModel = _handler.GetInvoiceDetails(invoiceNumber.Trim());
                UserDetailsModel userDetails = GetLoggedInUserDetails(false);
                viewModel.HeaderObj = HeaderFiller(userDetails);
                var stateConfig = Util.StateConfig().StateConfig.Where(s => s.Value == _orchardServices.WorkContext.CurrentSite.SiteName).FirstOrDefault();
                viewModel.MerchantKey = stateConfig.Node.Where(n => n.Key == TenantConfigKeys.NetPayMerchantKey.ToString()).FirstOrDefault().Value;
                viewModel.NetPayMode = stateConfig.Node.Where(n => n.Key == TenantConfigKeys.NetPayMode.ToString()).FirstOrDefault().Value;
                viewModel.NetPayColorCode = stateConfig.Node.Where(n => n.Key == TenantConfigKeys.NetPayColorCode.ToString()).FirstOrDefault().Value;
                viewModel.NetPayCurrencyCode = stateConfig.Node.Where(n => n.Key == TenantConfigKeys.NetPayCurrencyCode.ToString()).FirstOrDefault().Value;

                return View(viewModel);
            }
            catch(NoInvoicesMatchingTheParametersFoundException exception)
            {
                Logger.Error(exception, string.Format("Exception in MakePayment get {0}", exception.Message));
                TempData = null;
                TempData.Add("NoInvoice", invoiceNumber??"");
                return RedirectToRoute("P.BIN.Search");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in MakePayment get {0}", exception.Message));
                TempData = null;
                TempData.Add("Error", ErrorLang.genericexception().ToString());
            }
            return RedirectToRoute("P.SelectService");
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="invoiceNumber"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual JsonResult GetReferenceNumber(int invoiceId, string invoiceNumber, PaymentProvider provider = PaymentProvider.Bank3D)
        {
            Logger.Information($"About to get payment Reference Number for Invoice Number {invoiceNumber}, Provider:::{provider}");
            APIResponse response = _handler.GetPaymentReferenceNumber(invoiceId, invoiceNumber, provider);
            Logger.Information($"Gotten Payment Reference Number {response.ResponseObject} for Invoice Number {invoiceNumber}");

            return Json(response, JsonRequestBehavior.AllowGet);
        }




        public async Task<ActionResult> Notify(string paymentRef)
        {
            PaymentAcknowledgeMentModel model = new PaymentAcknowledgeMentModel();
            try
            {
                Logger.Information($"About log NetPay payment information Payment Reference: {paymentRef}");
                UserDetailsModel userDetails = GetLoggedInUserDetails(false);
                model.HeaderObj = HeaderFiller(userDetails);
                model.PaymentStatus = PaymentStatus.Declined;
                model.PaymentRequestRef = paymentRef;

                //Get the Invoice details attached to this Payment Reference
                PaymentReferenceVM paymentReferenceDetail = _handler.GetPaymentReferenceDetail(paymentRef);

                model.PayerId = paymentReferenceDetail.PayerId;
                model.Description = paymentReferenceDetail.InvoiceDescription;
                model.Recepient = paymentReferenceDetail.Recipient;
                model.InvoiceNumber = paymentReferenceDetail.InvoiceNumber;
                model.RevenueHeadName = paymentReferenceDetail.RevenueHead;

                try
                {
                    InvoiceValidationResponseModel response = await _handler.SavePayment(model);
                    model.ReceiptNumber = response.ReceiptNumber;
                    model.Amount = response.Amount;
                    CBSUserVM cbsUser = _handler.GetCBSUserWithInvoiceNumber(model.InvoiceNumber);
                    model.Recepient = cbsUser.Name;
                    //send sms notifications here
                    _handler.SendSMSNotification(model, cbsUser);

                    return View("Success", model);
                }
                catch (CannotVerifyNetPayTransaction exception)
                {
                    Logger.Error($"{exception}", ErrorLang.unabletoverifynetpayreference(model.PaymentRequestRef).ToString());
                    model.HasError = true;
                    model.ErrorMessage = ErrorLang.unabletoverifynetpayreference(model.PaymentRequestRef).ToString();
                    return View("Success", model);
                }
                catch (HttpRequestException exception)
                {
                    Logger.Error($"{exception}", ErrorLang.unabletoverifynetpayreference(model.PaymentRequestRef).ToString());
                    model.HasError = true;
                    model.ErrorMessage = ErrorLang.unabletoverifynetpayreference(model.PaymentRequestRef).ToString();
                    return View("Success", model);
                }
                catch (Exception ex)
                {
                    Logger.Error($"{ex}");
                    model.HasError = true;
                    model.ErrorMessage = ErrorLang.genericexception().ToString();
                    return View("Success", model);
                }
            }
            catch (NoRecordFoundException ex)
            {
                string message = ErrorLang.netpayreferencenumber404(model.PaymentRequestRef).ToString();
                Logger.Error(ex, message);
                model.HasError = true;
                model.ErrorMessage = message;
                return View("Success", model);
            }
            catch (Exception ex)
            {
                Logger.Error($"{ex}");
                model.HasError = true;
                model.ErrorMessage = ErrorLang.genericexception().ToString();
                return View("Success", model);
            }
        }

    }
}