using Newtonsoft.Json;
using Orchard;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Users.Events;
using Orchard.Users.Services;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Module.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Module.Web.Middleware.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.Module.Web.Controllers
{
    public abstract class BaseTaxProfileController : BaseController
    {
        private readonly ITaxProfileHandler _handler;
        private readonly IOrchardServices _orchardServices;

        private readonly IUserService _userService;
        private readonly IMembershipService _membershipService;
        private readonly ICBSUserManager<CBSUser> _cbsUserService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMembershipValidationService _membershipValidationService;
        private readonly IUserEventHandler _userEventHandler;


        public BaseTaxProfileController(IOrchardServices orchardServices, IUserService userService, IMembershipService membershipService, ICBSUserManager<CBSUser> cbsUserService, IAuthenticationService authenticationService, IMembershipValidationService membershipValidationService, IUserEventHandler userEventHandler, ITaxProfileHandler handler) : base(orchardServices, userService, membershipService, cbsUserService, authenticationService, membershipValidationService, userEventHandler, handler)
        {
            _orchardServices = orchardServices;
            Logger = NullLogger.Instance;
            _handler = handler;
            _authenticationService = authenticationService;
            _userService = userService;
            _membershipService = membershipService;
            _membershipValidationService = membershipValidationService;
            _userEventHandler = userEventHandler;
            _cbsUserService = cbsUserService;
        }


        /// <summary>
        /// payer profile, users that are not logged in are redirected here to first fill in their details then proceeded to invoice confirmation.
        /// Only anonymoust users are allowed here
        /// <para>Route Name : C.PayerProfile </para>
        /// </summary>
        [AnonymousOnly, BrowserHeaderFilter]
        public virtual ActionResult PayerProfile()
        {
            TempData = null;
                        
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

            string errorMessage = string.Empty;
            int revId = processStage.RevenueHeadId;
            int catId = processStage.CategoryId;

            try
            {
                string message = string.Empty;
                Logger.Information("Prepare payer profile view");
                TaxProfileFormVM vm = _handler.GetTaxPayerProfileVM(processStage);

                if(vm.ExternalRedirect != null && vm.ExternalRedirect.Redirecting)
                {
                    if (Session["InvoiceGenerationStage"] != null) { Session.Remove("InvoiceGenerationStage"); }
                    Session.Add("ExternalRedirect", JsonConvert.SerializeObject(new ExternalRedirect { URL = vm.ExternalRedirect.URL, Stage = InvoiceGenerationStage.ExternalRedirect }));
                    return RedirectToRouteX("C.Invoice.ThirdParty.Redirect", new { url = vm.ExternalRedirect.URL });
                }
               
                processStage = new GenerateInvoiceStepsModel { };
                processStage.RevenueHeadId = revId;
                processStage.CategoryId = catId;
                processStage.PayerProfileStageObj = vm;
                processStage.InvoiceGenerationStage = InvoiceGenerationStage.PayerProfile;
                processStage.BillingType = vm.BillingType;
                Session.Add("InvoiceGenerationStage", JsonConvert.SerializeObject(processStage));
                return View("TaxProfile/PayerProfile",vm);
                //return View(vm);
            }
            #region catch region
            catch (CannotFindRevenueHeadException)
            {
                Session.Remove("InvoiceGenerationStage");
                Logger.Error(string.Format("Could not find revenue head {0} {1}", revId, catId));
                errorMessage = ErrorLang.revenuehead404().ToString();
            }
            catch (NoCategoryFoundException)
            {
                Session.Remove("InvoiceGenerationStage");
                Logger.Error(string.Format("Could not find category {0} {1}", revId, catId));
                errorMessage = ErrorLang.categorynotfound().ToString();
            }
            catch (AuthorizedUserNotFoundException)
            {
                Session.Remove("InvoiceGenerationStage");
                Logger.Error(string.Format("Authorized user not found {0} {1}.", revId, catId));
                errorMessage = ErrorLang.requiressignin().ToString();//add sign in modal link here along with text
            }
            catch (Exception exception)
            {
                Logger.Error("Error in payer profile block");
                Logger.Error(exception, exception.Message);
                errorMessage = ErrorLang.genericexception().ToString();
            }
            #endregion

            if (revId > 0) { TempData.Add("RevenueHeadIdentifier", revId.ToString()); }
            if (catId > 0) { TempData.Add("TaxCategory", catId.ToString()); }
            TempData.Add("Error", errorMessage);

            return RedirectToRouteX("C.SelfAssessment");
        }

        [AnonymousOnly]
        [HttpPost, ActionName("PayerProfile")]
        public ActionResult PayerProfile(TaxProfileFormVM model, ICollection<CollectionFormVM> additionalFormFields)
        {
            TempData = null;
           
            if (System.Web.HttpContext.Current.Session["InvoiceGenerationStage"] == null)
            {
                TempData.Add("Error", "Your session could not be continued. Please fill in your details and proceed to start new session.");
                return RedirectToRouteX("C.SelfAssessment");
            }

            string sProcessStage = System.Web.HttpContext.Current.Session["InvoiceGenerationStage"].ToString();
            GenerateInvoiceStepsModel processStage = JsonConvert.DeserializeObject<GenerateInvoiceStepsModel>(sProcessStage);
            //check that this is the correct stage
            if (processStage.InvoiceGenerationStage != InvoiceGenerationStage.PayerProfile)
            {
                Session.Remove("InvoiceGenerationStage");
                TempData.Add("Error", "Your session could not be continued. Please fill in your details and proceed to start new session.");
                return RedirectToRouteX("C.SelfAssessment");
            }

            int revId = processStage.RevenueHeadId;
            int catId = processStage.CategoryId;

            TempData.Add("RevenueHeadIdentifier", processStage.RevenueHeadId.ToString());
            TempData.Add("TaxCategory", processStage.CategoryId.ToString());

            string errorMessage = string.Empty;
            try
            {
                Logger.Debug("Prepare payer profile post");
                var isModelValid = this.TryValidateModel(model.TaxEntityViewModel);
                if (!isModelValid) { throw new DirtyFormDataException(); }

                if (!string.IsNullOrEmpty(model.TaxEntityViewModel.PhoneNumber))
                {
                    //validate phone number
                    string sPhoneNumber = model.TaxEntityViewModel.PhoneNumber;
                    sPhoneNumber = sPhoneNumber.Replace(" ", string.Empty);
                    sPhoneNumber = sPhoneNumber.Replace("-", string.Empty);
                    if (sPhoneNumber.Substring(0) == "+") { sPhoneNumber = sPhoneNumber.Replace("+", string.Empty); }
                    long phoneNumber = 0;
                    bool isANumber = long.TryParse(sPhoneNumber, out phoneNumber);
                    if (!isANumber)
                    { this.ModelState.AddModelError("TaxEntityViewModel.PhoneNumber", "Add a valid mobile phone number."); throw new DirtyFormDataException(); }
                }
                
                //validate and save the tax entity
                ProceedWithInvoiceGenerationVM response = _handler.TrySaveTaxEntityProfile(this, processStage.RevenueHeadId, processStage.CategoryId, model.TaxEntityViewModel, processStage.PayerProfileStageObj, additionalFormFields);
                //get the page to route to
                RouteToVM routeToModel = _handler.GetPageToRouteTo(processStage.BillingType);

                BillingType tempBillingType = processStage.BillingType;
                
                processStage = null;
                processStage = new GenerateInvoiceStepsModel { };
                processStage.ProceedWithInvoiceGenerationVM = response;
                processStage.RevenueHeadId = revId;
                processStage.CategoryId = response.Entity.TaxEntityCategory.Id;
                processStage.InvoiceGenerationStage = routeToModel.Stage;
                processStage.ExternalRef = model.TaxEntityViewModel.ExternalBillNumber;
                processStage.BillingType = tempBillingType;

                Session.Add("InvoiceGenerationStage", JsonConvert.SerializeObject(processStage));
                return RedirectToRouteX(routeToModel.RouteName);
            }
            #region catch clauses
            catch (DirtyFormDataException)
            {
                List<string> errorArray = new List<string>();
                foreach (var item in this.ModelState.Keys)
                {
                    var modelState = this.ModelState[item];
                    if (modelState.Errors.Count > 0 && modelState.Value != null) { errorArray.Add(modelState.Errors.FirstOrDefault().ErrorMessage); }
                }
                Logger.Error("Dirty form data");
                //reassigning the additional form fields to the view model for post back
                //model.CollectionFormVM = additionalFormFields;
                processStage.PayerProfileStageObj.TaxEntityViewModel = model.TaxEntityViewModel;
                model = processStage.PayerProfileStageObj;
                return View("TaxProfile/PayerProfile", model);
            }
            catch (FormFieldDoesNotExistsException)
            {
                Logger.Error("Missing form fields");
                processStage.PayerProfileStageObj.TaxEntityViewModel = model.TaxEntityViewModel;
                model = processStage.PayerProfileStageObj;
                //model.CollectionFormVM = additionalFormFields;
                model.ErrorMessage = "Your have some missing form fields that are required.";
                return View("TaxProfile/PayerProfile",model);
            }            
            catch (TenantNotFoundException)
            {
                Logger.Error(string.Format("No tenant info found for {0} {1}", revId, catId));
                errorMessage = ErrorLang.genericexception().ToString();
            }
            catch (MDARecordNotFoundException)
            {
                Logger.Error(string.Format("Could not find mda for {0} {1}", revId, catId));
                errorMessage = ErrorLang.mdacouldnotbefound().ToString();
            }
            catch (CannotFindRevenueHeadException)
            {
                Logger.Error(string.Format("Could not find revenue head {0} {1}", revId, catId));
                errorMessage = ErrorLang.revenuehead404().ToString();
            }
            catch (BillingIsNotAllowedException)
            {
                Logger.Error(string.Format("Billing has expired for {0} {1}", revId, catId));
                errorMessage = ErrorLang.billingisnotallowed().ToString();
            }
            catch (BillingHasEndedException)
            {
                Logger.Error(string.Format("Billing has ended for {0} {1}", revId, catId));
                errorMessage = ErrorLang.billinghasended().ToString();
            }
            catch (NoCategoryFoundException)
            {
                Logger.Error(string.Format("Could not find category {0} {1}", revId, catId));
                errorMessage = ErrorLang.categorynotfound().ToString();
            }
            catch (AuthorizedUserNotFoundException exception)
            {
                Logger.Error(exception, string.Format("Authorized user not founf {0} {1} {2}", revId, catId, exception.Message));
                errorMessage = exception.Message;//add sign in modal link here along with text
            }
            catch (Exception exception)
            {
                Logger.Error("Error in payer profile block");
                Logger.Error(exception, exception.Message);
                errorMessage = ErrorLang.genericexception().ToString();
            }
            #endregion

            TempData = null;
            if (revId > 0) { TempData.Add("RevenueHeadIdentifier", revId.ToString()); }
            if (catId > 0) { TempData.Add("TaxCategory", catId.ToString()); }
            TempData.Add("Error", errorMessage);
            return RedirectToRouteX("C.SelfAssessment");
        }

    }
}