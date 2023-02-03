using Newtonsoft.Json;
using Orchard;
using Orchard.Logging;
using Orchard.Security;
using Orchard.UI.Notify;
using Orchard.Users.Events;
using Orchard.Users.Services;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Module.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Module.Web.Middleware.Filters;
using Parkway.CBS.Module.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Parkway.CBS.Module.Web.Controllers
{
    public abstract class BaseCollectionController : BaseController
    {
        private readonly IModuleCollectionHandler _handler;
        private readonly IOrchardServices _orchardServices;

        private readonly IUserService _userService;
        private readonly IMembershipService _membershipService;
        private readonly ICBSUserManager<CBSUser> _cbsUserService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMembershipValidationService _membershipValidationService;
        private readonly IUserEventHandler _userEventHandler;


        public BaseCollectionController(IModuleCollectionHandler handler, IOrchardServices orchardServices, IUserService userService, IMembershipService membershipService, ICBSUserManager<CBSUser> cbsUserService, IAuthenticationService authenticationService, IMembershipValidationService membershipValidationService, IUserEventHandler userEventHandler) : base(orchardServices, userService, membershipService, cbsUserService, authenticationService, membershipValidationService, userEventHandler, handler)
        {
            _handler = handler;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
            _userService = userService;
            _membershipService = membershipService;
            _cbsUserService = cbsUserService;
            _authenticationService = authenticationService;
            _membershipValidationService = membershipValidationService;
            _userEventHandler = userEventHandler;
        }


        protected virtual HeaderObj GetIndexVM()
        {
            var userDetails = GetLoggedInUserDetails();
            HeaderObj obj = new HeaderObj { ShowSignin = true };
            if (userDetails != null && userDetails.Entity != null) { obj.IsLoggedIn = true; obj.DisplayText = userDetails.Name; }
            else { obj.IsLoggedIn = false; }
            return obj;
        }


        [HttpPost]
        /// <summary>
        /// sign out a logged in user
        /// </summary>
        /// <param name="returnURL"></param>
        public virtual ActionResult Signout(string returnURL)
        {
            try
            {
                if (Session["InvoiceGenerationStage"] != null) { Session.Remove("InvoiceGenerationStage"); }
                Logger.Information("Signing out");
                _authenticationService.SignOut();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message + " Exception Signing out.");
            }
            return RedirectToRouteX("C.HomePage");
        }

        /// <summary>
        /// route name = BIN.Search
        /// </summary>
        /// <returns></returns>
        [BrowserHeaderFilter]
        public virtual ActionResult SearchByInvoiceNumber()
        {
            string message = string.Empty;
            bool hasError = false;
            string bin = null;

            try
            {
                if (TempData.ContainsKey("NoInvoice"))
                {
                    bin = TempData["NoInvoice"].ToString();
                    message = ErrorLang.invoice404().ToString();
                    hasError = true;
                    TempData.Remove("NoInvoice");
                }
            }
            catch (Exception exception)
            { Logger.Error(exception, exception.Message); }

            TempData = null;
            var user = GetLoggedInUserDetails();
            HeaderObj headerObj = new HeaderObj { ShowSignin = true };
            if (user != null && user.Entity != null) { headerObj.IsLoggedIn = true; headerObj.DisplayText = user.Name; }

            SearchByBINVM obj = new SearchByBINVM { HeaderObj = headerObj, ErrorMessage = message, HasErrors = hasError, BIN = bin };
            return View("Collection/SearchByInvoiceNumber", obj);
        }


        [HttpPost, ActionName("SearchByInvoiceNumber")]
        public virtual ActionResult SearchByInvoiceNumber(SearchByBINVM model)
        {
            TempData = null;
            string errorMessage = string.Empty;
            try
            {
                InvoiceGeneratedResponseExtn invoiceDetails = _handler.SearchForInvoiceForPaymentView(model.BIN);
                if (invoiceDetails != null)
                {
                    TempData.Add("InvoiceDetails", JsonConvert.SerializeObject(invoiceDetails));
                    return RedirectToRouteX("C.MakePayment.Invoice", new { invoiceNumber = invoiceDetails.InvoiceNumber });
                }

                Logger.Error("Invoice not found " + model.BIN);
                errorMessage = ErrorLang.invoice404().ToString();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Error searching for invoice number {0} {1}", model.BIN, exception.Message));
                errorMessage = ErrorLang.genericexception().ToString();
            }
            UserDetailsModel user = GetLoggedInUserDetails();
            HeaderObj headerObj = new HeaderObj { ShowSignin = true };
            if (user != null && user.Entity != null) { headerObj.IsLoggedIn = true; headerObj.DisplayText = user.Name; }
            SearchByBINVM obj = new SearchByBINVM { HeaderObj = headerObj, HasErrors = true, ErrorMessage = errorMessage, BIN = model.BIN };
            return View("Collection/SearchByInvoiceNumber",obj);
        }


        /// <summary>
        /// Route name: C.MakePayment.Invoice
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns></returns>
        [BrowserHeaderFilter]
        public virtual ActionResult MakePaymentByInvoice(string invoiceNumber)
        {
            InvoiceGeneratedResponseExtn invoiceDetails = null;

            if (TempData.ContainsKey("InvoiceDetails"))
            {
                invoiceDetails = JsonConvert.DeserializeObject<InvoiceGeneratedResponseExtn>(TempData["InvoiceDetails"].ToString());
                TempData.Remove("InvoiceDetails");
            }
            else
            {
                invoiceDetails = _handler.SearchForInvoiceForPaymentView(invoiceNumber);
                if (invoiceDetails == null)
                {
                    Logger.Error("Invoice not found " + invoiceNumber);
                    TempData = null;
                    TempData.Add("NoInvoice", invoiceNumber);
                    return RedirectToRouteX("BIN.Search");
                }
            }

            TempData = null;

            UserDetailsModel user = GetLoggedInUserDetails();
            HeaderObj headerObj = new HeaderObj { ShowSignin = true };
            if (user != null && user.Entity != null) { headerObj.IsLoggedIn = true; headerObj.DisplayText = user.Name; }

            var stateConfig = Util.StateConfig().StateConfig.Where(s => s.Value == _orchardServices.WorkContext.CurrentSite.SiteName).FirstOrDefault();
            var gatewayfeeNode = stateConfig.Node.Where(n => n.Key == TenantConfigKeys.GatewayFee.ToString()).FirstOrDefault();

            invoiceDetails.GatewayFee = gatewayfeeNode == null ? "₦0.00" : gatewayfeeNode.Value;
            invoiceDetails.HeaderObj = headerObj;
            invoiceDetails.TenantName = _orchardServices.WorkContext.CurrentSite.SiteName;
            invoiceDetails.MerchantKey = stateConfig.Node.Where(n => n.Key == TenantConfigKeys.NetPayMerchantKey.ToString()).FirstOrDefault().Value;
            invoiceDetails.NetPayMode = stateConfig.Node.Where(n => n.Key == TenantConfigKeys.NetPayMode.ToString()).FirstOrDefault().Value;
            invoiceDetails.NetPayColorCode = stateConfig.Node.Where(n => n.Key == TenantConfigKeys.NetPayColorCode.ToString()).FirstOrDefault().Value;
            invoiceDetails.NetPayCurrencyCode = stateConfig.Node.Where(n => n.Key == TenantConfigKeys.NetPayCurrencyCode.ToString()).FirstOrDefault().Value;

            if (string.IsNullOrEmpty(invoiceDetails.Email)) { invoiceDetails.Email = "admin@parkwayprojects.com"; }

            return View("Collection/MakePayment", invoiceDetails);
        }


        public virtual JsonResult GetNetPayToken(string amount, string desc, string tranxref)
        {
            Logger.Information(string.Format("generating net pay token for - {0} amount - {1} desc - {2} tranxref ", amount, desc, tranxref));
            try
            {
                return Json(new APIResponse { Error = true, ResponseObject = "Not supported yet" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                return Json(new APIResponse { ResponseObject = exception.Message, Error = true }, JsonRequestBehavior.AllowGet);
            }
        }


        [AnonymousOnly]
        public virtual JsonResult UserLogin(string username, string password, string __RequestVerificationToken)
        {
            IUser ouser = _membershipService.ValidateUser(username, password);
            if (ouser == null)
            {
                return Json(new APIResponse { Error = true, ResponseObject = "User record not found" }, JsonRequestBehavior.AllowGet); //return the list objects in json form
            }
            //we have gotten the user, lets check if this user is a CBS user
            CBSUser cuser = _cbsUserService.Get(x => x.UserPartRecord == new Orchard.Users.Models.UserPartRecord { Id = ouser.Id });
            if (cuser == null)
            {
                return Json(new APIResponse { Error = true, ResponseObject = "User record not found." }, JsonRequestBehavior.AllowGet); //return the list objects in json form
            }
            if (cuser.TaxEntity == null)
            {
                return Json(new APIResponse { Error = true, ResponseObject = "User record not found." }, JsonRequestBehavior.AllowGet); //return the list objects in json form
            }
            //var userClaims = _handler.CreateClaims(ouser, __RequestVerificationToken);
            return Json(new APIResponse { Error = false, ResponseObject = _handler.CreateClaims(ouser, __RequestVerificationToken) }, JsonRequestBehavior.AllowGet); //return the list objects in json form
        }


        public virtual ActionResult Index() { return View(GetIndexVM()); }


        /// <summary>
        /// show the page where the tax payer can select a category and a revenue head
        /// <para>Route name is C.SelfAssessment</para>
        /// </summary>
        [BrowserHeaderFilter]
        public virtual ActionResult GenerateInvoice()
        {
            GenerateInvoiceVM viewModel = new GenerateInvoiceVM() { };
            //_cbsUserService.F();
            try
            {
                //check if the user is logged in
                Logger.Information("generate invoice page");
                string errorMessage = string.Empty;
                bool hasErrors = false;
                string revenueHeadIdentifier = string.Empty;
                string categoryType = string.Empty;

                try
                {
                    if (TempData.ContainsKey("Error"))
                    {
                        hasErrors = true;
                        errorMessage = TempData["Error"].ToString();
                        TempData.Remove("Error");
                    }
                }
                catch (Exception exception) { Logger.Error(exception, "Error getting error value from temp data " + exception.Message); }
                TempData = null;

                if (Session["InvoiceGenerationStage"] != null)
                {
                    GenerateInvoiceStepsModel processStage = null;
                    string sProcessStage = System.Web.HttpContext.Current.Session["InvoiceGenerationStage"].ToString();
                    try
                    {
                        processStage = JsonConvert.DeserializeObject<GenerateInvoiceStepsModel>(sProcessStage);
                        revenueHeadIdentifier = processStage.RevenueHeadId.ToString();
                        categoryType = processStage.CategoryId.ToString();
                        Session.Remove("InvoiceGenerationStage");
                    }
                    catch (Exception exception)
                    {
                        Logger.Error(exception, string.Format("Exception getting the deserializing the session value {0} {1}", sProcessStage, exception.Message));
                    }
                }

                string message = string.Empty;
                //check if the user is logged in
                Logger.Information("Checking for logged in user");
                UserDetailsModel userDetails = GetLoggedInUserDetails();

                Logger.Information("Prepare self assessment view");
                viewModel = _handler.GetSelftAssessmentView();
                viewModel.HasErrors = hasErrors;
                viewModel.ErrorMessage = errorMessage;
                if (userDetails != null && userDetails.Entity != null)
                {
                    viewModel.HeaderObj.IsLoggedIn = true;
                    viewModel.HeaderObj.ShowSignin = true;
                    viewModel.HeaderObj.DisplayText = userDetails.Name;
                    var category = userDetails.Entity.TaxEntityCategory;
                    if (category == null)
                    {
                        Logger.Error(string.Format("No category found exception {0}", userDetails.Entity.Id));
                        throw new NoCategoryFoundException(string.Format("No category found exception {0}", userDetails.Entity.Id));
                    }
                    viewModel.AllowCategorySelect = false;
                    categoryType = category.Id.ToString();
                    viewModel.TaxPayerType = categoryType;
                }
                viewModel.RevenueHeadIdentifier = revenueHeadIdentifier;
                //viewModel.TaxPayerType = categoryType;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error in index block " + exception.Message);
                viewModel.HeaderObj = new HeaderObj { };
                viewModel.HasErrors = true;
                viewModel.ErrorMessage = ErrorLang.genericexception().ToString();
            }
            return View("Collection/GenerateInvoice",viewModel);
        }


        /// <summary>
        /// route C.InvoiceProceed
        /// </summary>
        [BrowserHeaderFilter]
        public virtual ActionResult InvoiceProceed()
        {
            TempData = null;
            string revenueHeadIdentifier = string.Empty;
            string taxCategory = string.Empty;
            if (System.Web.HttpContext.Current.Session["InvoiceGenerationStage"] == null)
            {
                Logger.Information(string.Format("User session has expired"));
                TempData.Add("Error", "Your session could not be continued. Please fill in your details and proceed to start new session.");
                return RedirectToRouteX("C.SelfAssessment");
            }
            GenerateInvoiceStepsModel processStage = null;
            var sProcessStage = System.Web.HttpContext.Current.Session["InvoiceGenerationStage"].ToString();

            try { processStage = JsonConvert.DeserializeObject<GenerateInvoiceStepsModel>(sProcessStage); }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception getting the deserializing the session value {0} {1}", sProcessStage, exception.Message));
                TempData.Add("Error", "Your session could not be continued. Please fill in your details and proceed to start new session.");
                return RedirectToRouteX("C.SelfAssessment");
            }

            TempData.Add("RevenueHeadIdentifier", processStage.RevenueHeadId);
            TempData.Add("TaxCategory", processStage.CategoryId);

            string errorMessage = string.Empty;
            try
            {
                string message = string.Empty;
                //check if the user is logged in
                var user = GetLoggedInUserDetails();
                bool loggedIn = true;

                if (user.Entity == null)
                {
                    if (processStage.ProceedWithInvoiceGenerationVM != null)
                    {
                        user.Entity = processStage.ProceedWithInvoiceGenerationVM.Entity;
                        loggedIn = false;
                        //reset whatever we had going on  
                        processStage.ProcessingDirectAssessmentReportVM = new ProcessingReportVM { };
                    }
                }


                if (user == null || user.Entity == null)
                {
                    TempData.Add("Error", ErrorLang.sessionended().ToString());
                    return RedirectToRouteX("C.SelfAssessment");
                }

                Logger.Debug("Prepare payer profile view");
                ViewToShowVM viewModel = _handler.GetDirectAssessmentBillVM(user.Entity, processStage, loggedIn);

                if (loggedIn) { viewModel.ViewModel.InvoiceProceedVM.HeaderObj = new HeaderObj { ShowSignin = true, IsLoggedIn = true, DisplayText = user.Name }; }
                else { viewModel.ViewModel.InvoiceProceedVM.HeaderObj = new HeaderObj { IsLoggedIn = false, ShowSignin = true }; }

                processStage.InvoiceGenerationStage = InvoiceGenerationStage.InvoiceGenerated;
                Session.Add("InvoiceGenerationStage", JsonConvert.SerializeObject(processStage));

                return View($"Collection/{viewModel.ViewToShow}", viewModel.ViewModel);
            }
            #region catch region
            catch (CannotFindRevenueHeadException)
            {
                Session.Remove("InvoiceGenerationStage");
                Logger.Error(string.Format("Could not find revenue head {0} {1}", revenueHeadIdentifier, taxCategory));
                errorMessage = ErrorLang.revenuehead404().ToString();
            }
            catch (NoCategoryFoundException)
            {
                Session.Remove("InvoiceGenerationStage");
                Logger.Error(string.Format("Could not find category {0} {1}", revenueHeadIdentifier, taxCategory));
                errorMessage = ErrorLang.categorynotfound().ToString();
            }
            catch (AuthorizedUserNotFoundException)
            {
                Session.Remove("InvoiceGenerationStage");
                Logger.Error(string.Format("Authorized user not found {0} {1}.", revenueHeadIdentifier, taxCategory));
                errorMessage = ErrorLang.requiressignin().ToString();//add sign in modal link here along with text
            }
            catch (Exception exception)
            {
                Logger.Error("Error in payer profile block");
                Logger.Error(exception, exception.Message);
                errorMessage = ErrorLang.genericexception().ToString();
            }
            #endregion

            if (!string.IsNullOrEmpty(revenueHeadIdentifier)) { TempData.Add("RevenueHeadIdentifier", revenueHeadIdentifier); }
            if (!string.IsNullOrEmpty(taxCategory)) { TempData.Add("TaxCategory", taxCategory); }
            TempData.Add("Error", errorMessage);

            return RedirectToRouteX("C.SelfAssessment");
        }

        /// <summary>
        /// post ops for generate invoice, the request is validated
        /// if the user is logged in a redirect is sent to C.InvoiceProceed,
        /// else the user is redirected to the profile page if the user is not logged in
        /// </summary>
        /// <param name="revenueHeadIdentifier"></param>
        /// <param name="taxCategory"></param>
        /// <param name="claimToken"></param>
        /// <param name="__RequestVerificationToken"></param>
        [HttpPost, ActionName("GenerateInvoice")]
        public virtual ActionResult GenerateInvoice(string revenueHeadIdentifier, string taxCategory, string claimToken, string __RequestVerificationToken, string profileIdentifier)
        {
            //validate input
            string errorMessage = string.Empty;
            UserDetailsModel user = null;
            try
            {
                if (Session["InvoiceGenerationStage"] != null) { Session.Remove("InvoiceGenerationStage"); }

                user = GetLoggedInUserDetails();
                TempData = null;
                //
                if (string.IsNullOrEmpty(revenueHeadIdentifier)) { throw new CannotFindRevenueHeadException(); }
                if (string.IsNullOrEmpty(taxCategory)) { throw new NoCategoryFoundException(); }

                int revenueHeadId = 0;
                bool parsed = Int32.TryParse(revenueHeadIdentifier, out revenueHeadId);
                if (!parsed) { throw new CannotFindRevenueHeadException(); }
                int categoryId = 0;
                parsed = Int32.TryParse(taxCategory, out categoryId);
                if (!parsed) { throw new NoCategoryFoundException(); }
                //
                if (!string.IsNullOrEmpty(claimToken) && user.Entity == null)
                { user = ValidateClaim(claimToken, __RequestVerificationToken); }
                //
                GenerateInvoiceStepsModel processStage = new GenerateInvoiceStepsModel { CategoryId = categoryId, RevenueHeadId = revenueHeadId, InvoiceGenerationStage = InvoiceGenerationStage.InvoiceProceed };
                ProceedWithInvoiceGenerationVM result = null;

                //if the user is a logged in user, please redirect to the approp page
                if (user != null && user.Entity != null)
                {
                    result = _handler.GetModelWhenUserIsSignedIn(user, revenueHeadId, categoryId);
                    if (result.Redirect)
                    {
                        if (Session["InvoiceGenerationStage"] != null) { Session.Remove("InvoiceGenerationStage"); }
                        Session.Add("ExternalRedirect", JsonConvert.SerializeObject(new ExternalRedirect { URL = result.InvoiceGenerationRedirectURL, Stage = InvoiceGenerationStage.ExternalRedirect }));
                        return RedirectToRouteX("C.Invoice.ThirdParty.Redirect", new { url = result.InvoiceGenerationRedirectURL });
                    }
                }
                else
                {
                    result = _handler.GetModelForAnonymous(revenueHeadId, categoryId, profileIdentifier);
                    if (result == null)
                    {
                        Session.Add("InvoiceGenerationStage", JsonConvert.SerializeObject(processStage));
                        return RedirectToRouteX("C.PayerProfile");
                    }

                    if (result.Redirect)
                    {
                        if (Session["InvoiceGenerationStage"] != null) { Session.Remove("InvoiceGenerationStage"); }
                        Session.Add("ExternalRedirect", JsonConvert.SerializeObject(new ExternalRedirect { URL = result.InvoiceGenerationRedirectURL, Stage = InvoiceGenerationStage.ExternalRedirect }));
                        return RedirectToRouteX("C.Invoice.ThirdParty.Redirect", new { url = result.InvoiceGenerationRedirectURL });
                    }
                }
                processStage.BillingType = result.BillingType;
                processStage.ProceedWithInvoiceGenerationVM = result;
                processStage.InvoiceGenerationStage = InvoiceGenerationStage.ShowInvoiceConfirm;
                //now we have gotten the steps model, lets decide on where this bad boy needs to go
                //since this tax payer is logged in, we donot need to create a profile for them,
                //what we need to do is redirect them to what ever page the billing type recommmends
                var vm = _handler.GetConfirmingInvoiceRoute(processStage.BillingType);
                Session.Add("InvoiceGenerationStage", JsonConvert.SerializeObject(processStage));
                return RedirectToRouteX(vm.RouteName);
            }
            #region catch region
            catch (CannotFindRevenueHeadException)
            {
                Logger.Error(string.Format("Could not find revenue head {0} {1}", revenueHeadIdentifier, taxCategory));
                errorMessage = ErrorLang.revenuehead404().ToString();
            }
            catch (NoCategoryFoundException)
            {
                Logger.Error(string.Format("Could not find category {0} {1}", revenueHeadIdentifier, taxCategory));
                errorMessage = ErrorLang.categorynotfound().ToString();
            }
            catch (NoRecordFoundException)
            {
                Logger.Error(string.Format("Could not find tax entity {0} {1} {2}", revenueHeadIdentifier, taxCategory, profileIdentifier));
                errorMessage = ErrorLang.taxpayer404().ToString();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                errorMessage = ErrorLang.genericexception().ToString();
            }
            #endregion

            var viewModel = _handler.GetSelftAssessmentView();
            viewModel.RevenueHeadIdentifier = revenueHeadIdentifier;
            viewModel.HasErrors = true;
            viewModel.ErrorMessage = errorMessage;
            if (user != null && user.Entity != null)
            { viewModel.TaxPayerType = taxCategory; }
            return View("Collection/GenerateInvoice",viewModel);
        }



        /// <summary>
        /// RouteName: C.Invoice.ThirdParty.Redirect
        /// </summary>
        /// <param name="url"></param>
        public virtual ActionResult RedirectForInvoiceGeneration(string url)
        {
            try
            {
                TempData = null;
                if (System.Web.HttpContext.Current.Session["ExternalRedirect"] == null)
                {
                    Logger.Error("ExternalRedirect Session value not found");
                    return View("Collection/RedirectForInvoiceGeneration", new ExternalRedirect { Message = ErrorLang.externalredirect404(url).ToString() });
                }

                ExternalRedirect model = null;
                var sData = System.Web.HttpContext.Current.Session["ExternalRedirect"].ToString();
                Session.Remove("ExternalRedirect");

                try { model = JsonConvert.DeserializeObject<ExternalRedirect>(sData); }
                catch (Exception exception)
                {
                    Logger.Error(exception, string.Format("Exception deserializing the ExternalRedirect session value {0} {1}", sData, exception.Message));
                    TempData.Add("Error", ErrorLang.genericexception().ToString());
                    return RedirectToRouteX("C.SelfAssessment");
                }

                if (model.Stage != InvoiceGenerationStage.ExternalRedirect || model.URL != url)
                {
                    return View("Collection/RedirectForInvoiceGeneration", new ExternalRedirect { Message = ErrorLang.externalredirect404(url).ToString() });
                }
                return View("Collection/RedirectForInvoiceGeneration", new ExternalRedirect { Message = "Invoices for this revenue head are to be generated externally. We are redirecting you shortly", Redirecting = true, URL = model.URL });
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception for RedirectForInvoiceGeneration {0} {1}", url, exception.Message));
                TempData.Add("Error", ErrorLang.errorredireting().ToString());
                return RedirectToRouteX("C.SelfAssessment");
            }
        }


        
        public virtual JsonResult CheckIfRequiresLogin(string staxPayerTypeId, string sRevenueHeadId)
        {
            bool error = false;
            string responseMsg = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(staxPayerTypeId) || string.IsNullOrEmpty(sRevenueHeadId))
                { error = true; responseMsg = "Could not find the details you were looking for."; throw new Exception(); }
                var loggedInUser = GetLoggedInUserDetails();
                if (loggedInUser == null || loggedInUser.Entity == null)
                {
                    int catId = 0;
                    bool parsed = Int32.TryParse(staxPayerTypeId, out catId);
                    bool requireslogin = _handler.RequiresLogin(catId);
                    if (requireslogin) { responseMsg = UserIsLoggedInResult.RequiresLogin.ToString(); }
                    else { responseMsg = UserIsLoggedInResult.DoesNotRequireLogin.ToString(); }
                }
                else
                {
                    responseMsg = UserIsLoggedInResult.AlreadyLoggedIn.ToString();
                }
            }
            #region catch clauses
            catch (NoCategoryFoundException exception)
            {
                error = true;
                responseMsg = ErrorLang.categorynotfound().ToString();
                Logger.Error(exception, string.Format("Exception when checking if category requires login {0} {1}", staxPayerTypeId, sRevenueHeadId));
            }
            catch (Exception exception)
            {
                error = true;
                if (string.IsNullOrEmpty(responseMsg)) { responseMsg = ErrorLang.genericexception().ToString(); };
                Logger.Error(exception, string.Format("Exception when checking if category requires login {0} {1}", staxPayerTypeId, sRevenueHeadId));
            }
            #endregion
            return Json(new APIResponse { Error = error, ResponseObject = responseMsg }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// RouteName: Confirm.Bill
        /// </summary>
        /// <returns></returns>
        [BrowserHeaderFilter]
        public virtual ActionResult ConfirmBill()
        {
            TempData = null;
            string revenueHeadIdentifier = string.Empty;
            string taxCategory = string.Empty;
            if (System.Web.HttpContext.Current.Session["InvoiceGenerationStage"] == null)
            {
                TempData.Add("Error", "Your session could not be continued. Please fill in your details and proceed to start new session.");
                return RedirectToRouteX("C.SelfAssessment");
            }

            var sProcessStage = System.Web.HttpContext.Current.Session["InvoiceGenerationStage"].ToString();
            var processStage = JsonConvert.DeserializeObject<GenerateInvoiceStepsModel>(sProcessStage);

            TempData.Add("RevenueHeadIdentifier", processStage.RevenueHeadId.ToString());
            TempData.Add("TaxCategory", processStage.CategoryId.ToString());

            if (processStage.InvoiceGenerationStage != InvoiceGenerationStage.ShowInvoiceConfirm)
            {
                Logger.Error(string.Format("invalid stage {0} expected {1}", processStage.InvoiceGenerationStage, InvoiceGenerationStage.InvoiceGenerated.ToString()));
                Session.Remove("InvoiceGenerationStage");
                TempData.Add("Error", ErrorLang.sessionended().ToString());
                return RedirectToRouteX("C.SelfAssessment");
            }

            int revId = processStage.RevenueHeadId;
            int catId = processStage.CategoryId;

            var user = GetLoggedInUserDetails();
            try
            {
                var vm = _handler.ConfirmingInvoiceVM(processStage, user);

                return View($"Collection/{vm.ViewToShow}", vm.ViewModel);
            }
            catch (AuthorizedUserNotFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                TempData.Add("Error", ErrorLang.sessionended().ToString());
                return RedirectToRouteX("C.SelfAssessment");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                Session.Remove("InvoiceGenerationStage");
                TempData.Add("Error", ErrorLang.genericexception().ToString());
            }
            return RedirectToRouteX("C.SelfAssessment");
        }



        [HttpPost, ActionName("ConfirmBill")]
        public virtual ActionResult ConfirmBill(ConfirmInvoiceVM model, ICollection<FormControlViewModel> controlCollectionFromUserInput)
        {
            ValidateInvoiceConfirmModel validationResponse = new ValidateInvoiceConfirmModel { };
            var errorMessage = string.Empty;

            TempData = null;
            if (System.Web.HttpContext.Current.Session["InvoiceGenerationStage"] == null)
            {
                TempData.Add("Error", "Your session could not be continued. Please fill in your details and proceed to start new session.");
                return RedirectToRouteX("C.SelfAssessment");
            }

            string sProcessStage = System.Web.HttpContext.Current.Session["InvoiceGenerationStage"].ToString();
            GenerateInvoiceStepsModel processStage = JsonConvert.DeserializeObject<GenerateInvoiceStepsModel>(sProcessStage);

            if (processStage.InvoiceGenerationStage != InvoiceGenerationStage.ShowInvoiceConfirm)
            {
                Logger.Error(string.Format("invalid stage {0} expected {1}", processStage.InvoiceGenerationStage, InvoiceGenerationStage.InvoiceGenerated.ToString()));
                Session.Remove("InvoiceGenerationStage");
                TempData.Add("Error", ErrorLang.sessionended().ToString());
                return RedirectToRouteX("C.SelfAssessment");
            }

            int revId = processStage.RevenueHeadId;
            int catId = processStage.CategoryId;

            try
            {
                List<ErrorModel> errors = new List<ErrorModel> { };
                IEnumerable<FormControlViewModel> dbForms = _handler.GetDBForms(processStage);
                _handler.ValidateForms(controlCollectionFromUserInput, dbForms, ref errors);
                //lets validate the model
                model.Forms = dbForms;
                _handler.ValidateConfirmInvoiceModel(processStage, model, ref errors);
                if (errors.Count <= 0)
                {
                    //add confirmed model details
                    processStage.InvoiceConfirmedModel = _handler.GetInvoiceConfirmedModel(processStage, model);
                    processStage.InvoiceGenerationStage = InvoiceGenerationStage.GenerateInvoice;
                    processStage.UserFormDetails = dbForms.Select(f => new UserFormDetails { ControlIdentifier = f.ControlIdentifier, FormValue = f.FormValue, FriendlyName = f.FriendlyName, RevenueHeadId = revId }).ToList();
                    Session.Add("InvoiceGenerationStage", JsonConvert.SerializeObject(processStage));
                    return RedirectToRouteX("C.MakePayment");
                }

                //if model has some errors lets create the view model so we can post back to view if required
                processStage.ValidatedFormFields = true;
                validationResponse = _handler.GetViewModelForConfirmInvoiceModelPostBack(processStage, model, GetLoggedInUserDetails(), errors);
                _handler.AddErrorsToModel(this, errors);
            }
            catch (DirtyFormDataException exception)
            {
                Logger.Error(exception, "Dirty form data");
                validationResponse.ViewModel.ErrorMessage = validationResponse.ErrorMessage;
                return View($"Collection/{validationResponse.ViewToShow}", validationResponse.ViewModel);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                TempData.Add("Error", ErrorLang.genericexception().ToString());
            }
            TempData.Add("RevenueHeadIdentifier", revId.ToString());
            TempData.Add("TaxCategory", catId.ToString());
            Session.Remove("InvoiceGenerationStage");
            return RedirectToRouteX("C.SelfAssessment");
        }


        [BrowserHeaderFilter]
        public virtual ActionResult MakePayment()
        {
            try
            {
                TempData = null;
                string revenueHeadIdentifier = string.Empty;
                string taxCategory = string.Empty;
                if (System.Web.HttpContext.Current.Session["InvoiceGenerationStage"] == null)
                {
                    TempData.Add("Error", "Your session could not be continued. Please fill in your details and proceed to start new session.");
                    return RedirectToRouteX("C.SelfAssessment");
                }
                UserDetailsModel user = GetLoggedInUserDetails();

                string sProcessStage = System.Web.HttpContext.Current.Session["InvoiceGenerationStage"].ToString();
                Session.Remove("InvoiceGenerationStage");

                GenerateInvoiceStepsModel processStage = JsonConvert.DeserializeObject<GenerateInvoiceStepsModel>(sProcessStage);
                //check that this is the correct stage
                if (processStage.InvoiceGenerationStage != InvoiceGenerationStage.GenerateInvoice)
                {
                    Session.Remove("InvoiceGenerationStage");
                    TempData.Add("Error", "Your session could not be continued. Please fill in your details and proceed to start new session.");
                    return RedirectToRouteX("C.SelfAssessment");
                }
                TempData.Add("RevenueHeadIdentifier", processStage.RevenueHeadId);
                TempData.Add("TaxCategory", processStage.CategoryId);

                TaxEntity entity = null;
                HeaderObj headerObj = new HeaderObj { ShowSignin = true };
                if (user != null && user.Entity != null) { entity = user.Entity; headerObj.IsLoggedIn = true; headerObj.DisplayText = user.Name; }

                if (entity == null)
                {
                    if (processStage.ProceedWithInvoiceGenerationVM != null)
                    {
                        if (processStage.ProceedWithInvoiceGenerationVM.FromTaxProfileSetup)
                        { entity = processStage.ProceedWithInvoiceGenerationVM.Entity; }
                    }
                }

                if (entity == null) { throw new AuthorizedUserNotFoundException(); }
                //get invoice details
                CreateInvoiceModel createInvoiceModel = _handler.GetCreateInvoiceModel(processStage, entity);

                if (createInvoiceModel.ExternalRedirect != null && createInvoiceModel.ExternalRedirect.Redirecting)
                {
                    {
                        if (Session["InvoiceGenerationStage"] != null) { Session.Remove("InvoiceGenerationStage"); }
                        Session.Add("ExternalRedirect", JsonConvert.SerializeObject(new ExternalRedirect { URL = createInvoiceModel.ExternalRedirect.URL, Stage = InvoiceGenerationStage.ExternalRedirect }));
                        return RedirectToRouteX("C.Invoice.ThirdParty.Redirect", new { url = createInvoiceModel.ExternalRedirect.URL });
                    }
                }

                //once we have gotten the invoice model, lets create the invoice
                List<ErrorModel> errors = new List<ErrorModel> { };
                createInvoiceModel.Forms = processStage.UserFormDetails;

                InvoiceGeneratedResponseExtn invoiceDetails = _handler.GetInvoiceDetails(createInvoiceModel, ref errors);

                TempData.Add("InvoiceDetails", JsonConvert.SerializeObject(invoiceDetails));

                return RedirectToRouteX("C.MakePayment.Invoice", new { invoiceNumber = invoiceDetails.InvoiceNumber });
            }
            catch (CannotConnectToCashFlowException exception)
            {
                TempData = null;
                Logger.Error(exception, string.Format("Exception in make payment {0}", exception.Message));
                TempData.Add("Error", ErrorLang.cannotconnettoinvoicingservice().ToString());
                return RedirectToRouteX("C.SelfAssessment");
            }
            catch (Exception exception)
            {
                TempData = null;
                Logger.Error(exception, string.Format("Exception in make payment {0}", exception.Message));
                TempData.Add("Error", ErrorLang.genericexception().ToString());
                return RedirectToRouteX("C.SelfAssessment");
            }
        }


        [BrowserHeaderFilter]
        public virtual ActionResult About()
        {
            try
            {
                var user = GetLoggedInUserDetails();
                var headerObj = new HeaderObj { };
                if (user != null && user.Entity != null) { headerObj.IsLoggedIn = true; headerObj.DisplayText = user.Name; }
                headerObj.ShowSignin = true;
                return View("Collection/About",new AboutVM { HeaderObj = headerObj, TenantName = _orchardServices.WorkContext.CurrentSite.SiteName });
                //return View(new AboutVM { HeaderObj = headerObj, TenantName = _orchardServices.WorkContext.CurrentSite.SiteName });
            }
            catch (Exception exception)
            {
                TempData = null;
                Logger.Error(exception, string.Format("Exception in About {0}", exception.Message));
                TempData.Add("Error", ErrorLang.genericexception().ToString());
                return RedirectToRouteX("C.SelfAssessment");
            }
        }


        /// <summary>
        /// show downloads
        /// </summary>
        /// <returns></returns>
        [BrowserHeaderFilter]
        public virtual ActionResult ShowDownloadsFiles()
        {
            try
            {
                var user = GetLoggedInUserDetails();
                var headerObj = new HeaderObj { };
                if (user != null && user.Entity != null) { headerObj.IsLoggedIn = true; headerObj.DisplayText = user.Name; }
                headerObj.ShowSignin = true;
                return View("Collection/ShowDownloadsFiles", new DownloadsVM { HeaderObj = headerObj });
            }
            catch (Exception exception)
            {
                TempData = null;
                Logger.Error(exception, string.Format("Exception in ShowDownloadsFiles {0}", exception.Message));
                TempData.Add("Error", ErrorLang.genericexception().ToString());
                return RedirectToRouteX("C.SelfAssessment");
            }
        }


        [BrowserHeaderFilter]
        public virtual ActionResult Contact()
        {
            try
            {
                var user = GetLoggedInUserDetails();
                var headerObj = new HeaderObj { };
                if (user != null && user.Entity != null) { headerObj.IsLoggedIn = true; headerObj.DisplayText = user.Name; }
                headerObj.ShowSignin = true;
                return View("Collection/Contact",new ContactVM { HeaderObj = headerObj });
                //return View(new ContactVM { HeaderObj = headerObj });
            }
            catch (Exception exception)
            {
                TempData = null;
                Logger.Error(exception, string.Format("Exception in Contact {0}", exception.Message));
                TempData.Add("Error", ErrorLang.genericexception().ToString());
                return RedirectToRouteX("C.SelfAssessment");
            }
        }


        public async Task<ActionResult> Notify(string paymentRef)
        {
            PaymentAcknowledgeMentModel model = new PaymentAcknowledgeMentModel();
            try
            {
                Logger.Information($"About log NetPay payment information Payment Reference: {paymentRef}");
                UserDetailsModel user = GetLoggedInUserDetails();
                HeaderObj headerObj = new HeaderObj { ShowSignin = true };
                if (user != null && user.Entity != null) { headerObj.IsLoggedIn = true; headerObj.DisplayText = user.Name; }
                model.HeaderObj = headerObj;
                model.PaymentStatus = PaymentStatus.Declined;
                model.PaymentRequestRef = paymentRef;

                //Get the Invoice details attached to this Payment Reference
                PaymentReferenceVM paymentReferenceDetail = _handler.GetPaymentReferenceDetail(paymentRef);

                InvoiceGeneratedResponseExtn invoiceDetails = null;

                invoiceDetails = _handler.SearchForInvoiceForPaymentView(paymentReferenceDetail.InvoiceNumber);
                if (invoiceDetails == null)
                {
                    Logger.Error("Invoice not found " + paymentReferenceDetail.InvoiceNumber);
                    TempData = null;
                    TempData.Add("NoInvoice", paymentReferenceDetail.InvoiceNumber);
                    return RedirectToRouteX("BIN.Search");
                }

                model.Email = invoiceDetails.Email;
                model.PhoneNumber = invoiceDetails.PhoneNumber;
                model.MDAName = invoiceDetails.MDAName;
                model.RevenueHeadName = invoiceDetails.RevenueHeadName;
                model.TIN = invoiceDetails.TIN;
                model.Recepient = invoiceDetails.Recipient;
                model.InvoiceNumber = invoiceDetails.InvoiceNumber;

                try
                {
                    //Save the payment details
                    InvoiceValidationResponseModel response = await _handler.SavePayment(model);
                    model.ReceiptNumber = response.ReceiptNumber;
                    return View("Collection/success", model);
                }
                catch (CannotVerifyNetPayTransaction exception)
                {
                    Logger.Error($"{exception}", ErrorLang.unabletoverifynetpayreference(model.PaymentRequestRef).ToString());
                    model.HasError = true;
                    model.ErrorMessage = ErrorLang.unabletoverifynetpayreference(model.PaymentRequestRef).ToString();
                    return View("Collection/success", model);
                }
                catch (HttpRequestException exception)
                {
                    Logger.Error($"{exception}", ErrorLang.unabletoverifynetpayreference(model.PaymentRequestRef).ToString());
                    model.HasError = true;
                    model.ErrorMessage = ErrorLang.unabletoverifynetpayreference(model.PaymentRequestRef).ToString();
                    return View("Collection/success", model);
                }
                catch (Exception ex)
                {
                    Logger.Error($"{ex}");
                    model.HasError = true;
                    model.ErrorMessage = ErrorLang.genericexception().ToString();
                    return View("Collection/success", model);
                }

            }
            catch (NoRecordFoundException ex)
            {
                string message = ErrorLang.netpayreferencenumber404(model.PaymentRequestRef).ToString();
                Logger.Error(ex, message);
                model.HasError = true;
                model.ErrorMessage = message;
                return View("Collection/success", model);
            }
            catch (Exception ex)
            {
                Logger.Error($"{ex}");
                model.HasError = true;
                model.ErrorMessage = ErrorLang.genericexception().ToString();
                return View("Collection/success", model);
            }
        }


        [AnonymousOnly, BrowserHeaderFilter]
        public virtual ActionResult RegisterUser()
        {
            try
            {
                Logger.Information("register user");
                return View("Collection/RegisterUser",_handler.GetRegsiterView());
                //return View(_handler.GetRegsiterView());
            }
            catch (Exception exception) { Logger.Error("Error in register user get", exception); }
            return RedirectToRouteX("C.HomePage");
        }


        [HttpPost, AnonymousOnly, ActionName("RegisterUser")]
        public virtual ActionResult RegisterUser(RegisterCBSUserObj model)
        {
            string errorMessage = string.Empty;
            try
            {
                bool modelIsValid = this.TryValidateModel(model.RegisterCBSUserModel);
                if (!modelIsValid) { throw new DirtyFormDataException(); }
                //validate phone number
                string sPhoneNumber = model.RegisterCBSUserModel.PhoneNumber;
                sPhoneNumber = sPhoneNumber.Replace(" ", string.Empty);
                sPhoneNumber = sPhoneNumber.Replace("-", string.Empty);
                long phoneNumber = 0;
                bool isANumber = long.TryParse(sPhoneNumber, out phoneNumber);
                if (!isANumber)
                { this.ModelState.AddModelError("RegisterCBSUserModel.PhoneNumber", "Add a valid mobile phone number."); throw new DirtyFormDataException(); }

                DoOtherFormValidations(model);

                _handler.TryRegisterCBSUser(this, model);
                TempData = null;
                TempData.Add("Message", Lang.registrationsuccessful);
                TempData.Add("Username", model.RegisterCBSUserModel.UserName);
                return RedirectToRouteX("C.SignIn");
            }
            #region catch clauses
            catch (DirtyFormDataException exception)
            {
                Logger.Error(exception.Message, exception);
            }
            catch (NoCategoryFoundException exception)
            {
                Logger.Error(exception.Message, exception);
                errorMessage = ErrorLang.categorynotfound().ToString();
            }
            catch (CBSUserAlreadyExistsException)
            {
                errorMessage = ErrorLang.profilealreadyexists().ToString();
            }
            catch (PhoneNumberHasBeenTakenException)
            {
                errorMessage = ErrorLang.phonenumberalreadyexists().ToString();
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
                errorMessage = ErrorLang.genericexception().ToString();
            }
            #endregion

            var viewModel = _handler.GetRegsiterView();
            model.TaxCategories = viewModel.TaxCategories;
            model.StateLGAs = _handler.GetStatesAndLGAs();
            if(model.RegisterCBSUserModel.SelectedState != 0) { model.ListLGAs = model.StateLGAs.Where(x => x.Id == model.RegisterCBSUserModel.SelectedState).Single().LGAs.ToList(); }
            model.HeaderObj = new HeaderObj { };
            model.Error = true;
            model.ErrorMessage = errorMessage;
            return View("Collection/RegisterUser", model);
        }


        /// <summary>
        /// Perform other form validtion for user registeration
        /// </summary>
        /// <param name="model"></param>
        protected virtual void DoOtherFormValidations(RegisterCBSUserObj model) { }


        [AnonymousOnly, BrowserHeaderFilter]
        public virtual ActionResult SignIn(string r)
        {
            string username = string.Empty;
            string message = string.Empty;
            string errorMessage = string.Empty;
            bool hasError = false;
            try
            {
                Logger.Information("Signing in");
                //check if the user is logged in
                if (TempData.ContainsKey("Message"))
                {
                    message = TempData["Message"].ToString();
                    TempData.Remove("Message");
                }

                if (TempData.ContainsKey("Username"))
                {
                    username = TempData["Username"].ToString();
                    TempData.Remove("Username");
                }

                if (TempData.ContainsKey("Error"))
                {
                    hasError = true;
                    errorMessage = TempData["Error"].ToString();
                    TempData.Remove("Error");
                }
                //check if the user is logged in
                Logger.Information("Checking for logged in user");
                var userObj = GetLoggedInUserDetails();
                if (userObj != null && userObj.Entity != null)
                {
                    return RedirectToRouteX("C.SelfAssessment");
                }
                if (Session["InvoiceGenerationStage"] != null) { Session.Remove("InvoiceGenerationStage"); }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message + " Exception Signing in.");
            }
            return View("Collection/SignIn",new SignInObj { HeaderObj = new HeaderObj { }, CBSUserName = username, Message = message, ErrorMessage = errorMessage, Error = hasError, ReturnURL = string.IsNullOrEmpty(r) ? "" : r });
            //return View(new SignInObj { HeaderObj = new HeaderObj { }, CBSUserName = username, Message = message, ErrorMessage = errorMessage, Error = hasError, ReturnURL = string.IsNullOrEmpty(r) ? "" : r });
        }


        [HttpPost, AnonymousOnly]
        public virtual ActionResult SignIn(SignInObj model, string r)
        {
            string routePath = "Collection/SignIn";
            try
            {
                var loggedInUser = _authenticationService.GetAuthenticatedUser();
                if (loggedInUser == null)
                {
                    Logger.Information(string.Format("{0} is signing in", model.CBSUserName));
                    IUser ouser = _membershipService.ValidateUser(model.CBSUserName, model.Password);
                    if (ouser == null)
                    {
                        return View(routePath, new SignInObj { ErrorMessage = "User not found", Error = true, HeaderObj = new HeaderObj { }, CBSUserName = model.CBSUserName });
                    }
                    CBSUser cuser = _cbsUserService.Get(x => x.UserPartRecord == new Orchard.Users.Models.UserPartRecord { Id = ouser.Id });
                    if (cuser == null)
                    {
                        return View(routePath, new SignInObj { ErrorMessage = "User not found", Error = true, HeaderObj = new HeaderObj { }, CBSUserName = model.CBSUserName });
                    }
                    TaxEntity entity = cuser.TaxEntity;
                    if (entity == null)
                    {
                        return View(routePath, new SignInObj { ErrorMessage = "User not found", Error = true, HeaderObj = new HeaderObj { }, CBSUserName = model.CBSUserName });
                    }

                    Logger.Information(string.Format("{0} is about to be signed in", model.CBSUserName));
                    _userEventHandler.LoggingIn(model.CBSUserName, model.Password);
                    Logger.Information(string.Format("{0} is logged in", model.CBSUserName));
                    _authenticationService.SetAuthenticatedUserForRequest(ouser);
                    Logger.Information(string.Format("{0} auth set", model.CBSUserName));
                    _authenticationService.SignIn(ouser, true);
                    Logger.Information(string.Format("{0} SignIn set", model.CBSUserName));
                    _userEventHandler.LoggedIn(ouser);
                    Logger.Information(string.Format("{0} LoggedIn set", model.CBSUserName));
                    //when the user has been signed in, we remove the invoice generation session from memory
                    //for example if an anonymous user was generating an invoice, once a new user is sugned in
                    //we would want that new user's details to be used for invoice generation
                    EmptyInvoiceGenerationSession();
                    Logger.Information(string.Format("{0} session emptied", model.CBSUserName));
                }
                else
                { Logger.Information(string.Format("User already logged in")); }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message + " Exception Signing in.");
                return View(routePath, new SignInObj { ErrorMessage = ErrorLang.genericexception().ToString(), HeaderObj = new HeaderObj { }, Error = true, CBSUserName = model.CBSUserName });
            }
            if (!string.IsNullOrEmpty(r))
            {
                return RedirectToRouteX(GetRouteName(r));
            }
            Logger.Information(string.Format("{0} redirecting emptied", model.CBSUserName));
            return RedirectToRouteX("C.SelfAssessment");
        }


        /// <summary>
        /// Get name of route for return UL after c segment
        /// </summary>
        /// <param name="value"></param>
        /// <returns>string</returns>
        protected virtual string GetRouteName(string value)
        {
            if (value == "receipts") { return "C.PAYE.Tax.Receipts"; }
            else { return "C.SelfAssessment"; }
        }


        protected virtual ActionResult CategoryView(CBSUser cuser, TaxEntity entity)
        {
            var category = entity.TaxEntityCategory;
            if (category.RequiresLogin) { return RedirectToRouteX("C.PayerProfile"); }
            throw new NotImplementedException();
        }


        public virtual ActionResult GetInvoice(string BIN)
        {
            try
            {
                Logger.Information("Getting invoice " + BIN);
                string invoiceURL = _handler.GetInvoiceURL(BIN);
                return View("Collection/GetInvoice",new InvoiceViewVM { URL = invoiceURL });
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return View("Collection/GetInvoice",new InvoiceViewVM { Error = true, ErrorMessage = ErrorLang.invoice404().ToString() });
            //return View(new InvoiceViewVM { Error = true, ErrorMessage = ErrorLang.invoice404().ToString() });
        }


        /// <summary>
        /// List of payments
        /// </summary>
        /// <param name="model"></param>
        /// <param name="datefilter"></param>
        /// <param name="receiptNumber"></param>
        /// <returns></returns>
        public virtual ActionResult Payments(PaymentsVM model, string datefilter, string receiptNumber)
        {
            try
            {
                TempData = null;
                model.DateFilter = datefilter;
                var user = GetLoggedInUserDetails();
                if (user == null || user.Entity == null)
                { TempData.Add("Error", ErrorLang.requiressignin().ToString()); return RedirectToAction("SignIn"); }

                var vm = _handler.GetCollectionReport(user.Entity.Id, model, datefilter, 0);
                vm.HeaderObj.DisplayText = user.Name;
                return View("Collection/Payments",vm);
            }
            catch (Exception exception) { Logger.Error(exception, exception.Message); }
            TempData.Add("Error", ErrorLang.genericexception().ToString());
            return RedirectToRouteX("C.SelfAssessment");
        }



        /// <summary>
        /// Generate pdf receipt
        /// </summary>
        /// <param name="ReceiptNumber"></param>
        public void GetReceipt(string ReceiptNumber)
        {
            try
            {
                CreateReceiptDocumentVM result = _handler.CreateReceiptFile(ReceiptNumber);
                System.Web.HttpContext.Current.Response.ClearHeaders();
                System.Web.HttpContext.Current.Response.ContentType = "application/pdf";
                System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + result.FileName);
                System.Web.HttpContext.Current.Response.TransmitFile(result.SavedPath);
                System.Web.HttpContext.Current.Response.End();
                return;
            }
            catch (UserNotAuthorizedForThisActionException exception)
            {
                var message = String.Format("\nUser ID {0} tried to ViewReceipt without permission", _orchardServices.WorkContext.CurrentUser.UserName);
                Logger.Error(exception, message + exception.Message);
            }
            catch (NoRecordFoundException exception)
            {
                Logger.Error(exception, exception.Message);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
            }
        }

        /// <summary>
        /// route name = Receipt.Search
        /// </summary>
        /// <returns></returns>
        [BrowserHeaderFilter]
        public virtual ActionResult SearchByReceiptNumber()
        {
            string message = string.Empty;
            bool hasError = false;
            string receiptNumber = null;

            try
            {
                if (TempData.ContainsKey("NoReceipt"))
                {
                    receiptNumber = TempData["NoReceipt"].ToString();
                    message = ErrorLang.receipt404(receiptNumber).ToString();
                    hasError = true;
                    TempData.Remove("NoReceipt");
                }
            }
            catch (Exception exception)
            { Logger.Error(exception, exception.Message); }

            TempData = null;
            var user = GetLoggedInUserDetails();
            HeaderObj headerObj = new HeaderObj { ShowSignin = true };
            if (user != null && user.Entity != null) { headerObj.IsLoggedIn = true; headerObj.DisplayText = user.Name; }

            SearchByReceiptNumberVM obj = new SearchByReceiptNumberVM { HeaderObj = headerObj, ErrorMessage = message, HasErrors = hasError, ReceiptNumber = receiptNumber };
            return View("Collection/SearchByReceiptNumber", obj);
        }


        [HttpPost, ActionName("SearchByReceiptNumber")]
        public virtual ActionResult SearchByReceiptNumber(SearchByReceiptNumberVM model)
        {
            TempData = null;
            string errorMessage = string.Empty;
            try
            {
                try
                {
                    ReceiptViewModel receiptDetails = _handler.GetReceiptVM(model.ReceiptNumber);
                    if (receiptDetails != null)
                    {
                        TempData.Add("ReceiptDetails", JsonConvert.SerializeObject(receiptDetails));
                        return RedirectToRouteX("C.ReceiptDetails", new { receiptNumber = receiptDetails.ReceiptNumber });
                    }

                    Logger.Error("Receipt not found " + model.ReceiptNumber);
                    errorMessage = ErrorLang.receipt404().ToString();
                }
                catch (NoRecordFoundException ex)
                {
                    Logger.Error(ex, string.Format("No record found for receipt number {0} {1}", model.ReceiptNumber, ex.Message));
                    errorMessage = ErrorLang.receipt404().ToString();
                }
                catch (Exception exception)
                {
                    Logger.Error(exception, string.Format("Error searching for receipt number {0} {1}", model.ReceiptNumber, exception.Message));
                    errorMessage = ErrorLang.genericexception().ToString();
                }
                UserDetailsModel user = GetLoggedInUserDetails();
                HeaderObj headerObj = new HeaderObj { ShowSignin = true };
                if (user != null && user.Entity != null) { headerObj.IsLoggedIn = true; headerObj.DisplayText = user.Name; }
                SearchByReceiptNumberVM obj = new SearchByReceiptNumberVM { HeaderObj = headerObj, HasErrors = true, ErrorMessage = errorMessage, ReceiptNumber = model.ReceiptNumber };
                return View("Collection/SearchByReceiptNumber", obj);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Error while retriving receipt {0}", model.ReceiptNumber));
            }

            Logger.Error("Receipt not found " + model.ReceiptNumber);
            TempData = null;
            TempData.Add("NoReceipt", model.ReceiptNumber);
            return RedirectToRouteX("Receipt.Search");
        }


        /// <summary>
        /// Route name: C.ReceiptDetails
        /// Path: c/receipt-details/{receiptNumber}
        /// </summary>
        /// <param name="receiptNumber"></param>
        [BrowserHeaderFilter]
        public virtual ActionResult ViewReceiptDetails(string receiptNumber)
        {
            try
            {
                ReceiptViewModel receiptDetails = null;

                if (TempData.ContainsKey("ReceiptDetails"))
                {
                    receiptDetails = JsonConvert.DeserializeObject<ReceiptViewModel>(TempData["ReceiptDetails"].ToString());
                    TempData.Remove("ReceiptDetails");
                }
                else
                {
                    receiptDetails = _handler.GetReceiptVM(receiptNumber);
                }

                TempData = null;

                UserDetailsModel user = GetLoggedInUserDetails();
                HeaderObj headerObj = new HeaderObj { ShowSignin = true };
                if (user != null && user.Entity != null) { headerObj.IsLoggedIn = true; headerObj.DisplayText = user.Name; }
                SearchByReceiptNumberVM obj = new SearchByReceiptNumberVM { HeaderObj = headerObj, HasErrors = true, ReceiptViewModel = receiptDetails, ReceiptNumber = receiptNumber };

                return View("Collection/ViewReceiptDetails", obj);
            }
            catch (NoRecordFoundException ex)
            {
                Logger.Error(ex, string.Format("No record found for receipt number {0} {1}", receiptNumber, ex.Message));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Error searching for receipt number {0} {1}", receiptNumber, exception.Message));
            }

            Logger.Error("Receipt not found " + receiptNumber);
            TempData = null;
            TempData.Add("NoReceipt", receiptNumber);
            return RedirectToRouteX("Receipt.Search");
        }

    }
}