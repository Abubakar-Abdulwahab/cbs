using Newtonsoft.Json;
using Orchard;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Themes;
using Orchard.Users.Events;
using Orchard.Users.Services;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Module.Web.Controllers;
using Parkway.CBS.Module.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Module.Web.Middleware.Filters;
using Parkway.CBS.RSTVL.Web.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.RSTVL.Web.Controllers
{
    public class CollectionController : BaseCollectionController
    {
        private readonly IModuleCollectionHandler _handler;
        private readonly IRSTVLHandler _rstvlHandler;
        private readonly IOrchardServices _orchardServices;

        private readonly IUserService _userService;
        private readonly IMembershipService _membershipService;
        private readonly ICBSUserManager<CBSUser> _cbsUserService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMembershipValidationService _membershipValidationService;
        private readonly IUserEventHandler _userEventHandler;


        public CollectionController(IModuleCollectionHandler handler, IOrchardServices orchardServices, IUserService userService, IMembershipService membershipService, ICBSUserManager<CBSUser> cbsUserService, IAuthenticationService authenticationService, IMembershipValidationService membershipValidationService, IUserEventHandler userEventHandler, IRSTVLHandler rstvlHandler) : base(handler, orchardServices, userService, membershipService, cbsUserService, authenticationService, membershipValidationService, userEventHandler)
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
            _rstvlHandler = rstvlHandler;
        }



        /// <summary>
        /// RouteName: Confirm.Bill
        /// </summary>
        /// <returns></returns>
        [BrowserHeaderFilter]
        public override ActionResult ConfirmBill()
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

                ConfirmInvoiceVM returnValue = (ConfirmInvoiceVM)vm.ViewModel;
                returnValue.StateLGAs = _handler.GetStatesAndLGAs();
                returnValue.ListLGAs = returnValue.StateLGAs.ElementAt(0).LGAs.ToList();

                return View($"Collection/{vm.ViewToShow}", returnValue);
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
        public override ActionResult ConfirmBill(ConfirmInvoiceVM model, ICollection<FormControlViewModel> controlCollectionFromUserInput)
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
                if (model.SelectedState <= 0) { errors.Add(new ErrorModel { FieldName = "SelectedState", ErrorMessage = "State is required." }); }
                if (model.SelectedStateLGA <= 0) { errors.Add(new ErrorModel { FieldName = "SelectedStateLGA", ErrorMessage = "State LGA is required." }); }
                if((model.Year < 1990) || (model.Year > DateTime.Now.ToLocalTime().Year))
                { errors.Add(new ErrorModel { FieldName = "Year", ErrorMessage = "Select a valid year." }); }

                if (errors.Count <= 0)
                {
                    //add confirmed model details
                    processStage.InvoiceConfirmedModel = _handler.GetInvoiceConfirmedModel(processStage, model);
                    processStage.InvoiceGenerationStage = InvoiceGenerationStage.GenerateInvoice;
                    processStage.UserFormDetails = dbForms.Select(f => new UserFormDetails { ControlIdentifier = f.ControlIdentifier, FormValue = f.FormValue, FriendlyName = f.FriendlyName, RevenueHeadId = revId }).ToList();
                    processStage.SelectedState = model.SelectedState;
                    processStage.SelectedStateLGA = model.SelectedStateLGA;
                    processStage.SelectedYear = model.Year;
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
                //
                ConfirmInvoiceVM returnValue = (ConfirmInvoiceVM)validationResponse.ViewModel;
                returnValue.StateLGAs = _handler.GetStatesAndLGAs();
                returnValue.ListLGAs = returnValue.StateLGAs.ElementAt(0).LGAs.ToList();
                //
                if (model.SelectedState > 0)
                {
                    var state = returnValue.StateLGAs.Where(s => s.Id == model.SelectedState).SingleOrDefault();
                    returnValue.ListLGAs = state != null ? state.LGAs.ToList() : null;
                }
                returnValue.ErrorMessage = validationResponse.ErrorMessage;
                return View($"Collection/{validationResponse.ViewToShow}", returnValue);
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
        public override ActionResult MakePayment()
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
                //
                _rstvlHandler.SaveLicenceDetails(processStage, invoiceDetails, entity.Id);
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



    }
}