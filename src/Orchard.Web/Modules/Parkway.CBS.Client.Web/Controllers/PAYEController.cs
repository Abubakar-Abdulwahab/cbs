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
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Module.Web.Controllers;
using Parkway.CBS.Module.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Module.Web.Middleware.Filters;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Parkway.CBS.Client.Web.Controllers
{
    public class PAYEController : BaseController
    {
        private readonly IOrchardServices _orchardServices;

        private readonly IUserService _userService;
        private readonly IMembershipService _membershipService;
        private readonly ICBSUserManager<CBSUser> _cbsUserService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMembershipValidationService _membershipValidationService;
        private readonly IUserEventHandler _userEventHandler;

        public PAYEController(IOrchardServices orchardServices, IUserService userService, IMembershipService membershipService, ICBSUserManager<CBSUser> cbsUserService, IAuthenticationService authenticationService, IMembershipValidationService membershipValidationService, IUserEventHandler userEventHandler, ICommonBaseHandler commonHandler) : base(orchardServices, userService, membershipService, cbsUserService, authenticationService, membershipValidationService, userEventHandler, commonHandler)
        {
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
            _userService = userService;
            _membershipService = membershipService;
            _cbsUserService = cbsUserService;
            _authenticationService = authenticationService;
            _membershipValidationService = membershipValidationService;
            _userEventHandler = userEventHandler;
        }


        /// <summary>
        /// route C.SelectPAYEOption
        /// </summary>
        [CBSCollectionAuthorized]
        [BrowserHeaderFilter]
        public async Task<ActionResult> SelectPAYEOption()
        {
            TempData = null;
            string revenueHeadIdentifier = string.Empty;
            string taxCategory = string.Empty;
            string errorMessage = null;

            try
            {
                GenerateInvoiceStepsModel processStage = GetDeserializedSessionObj(ref errorMessage);
                revenueHeadIdentifier = processStage.RevenueHeadId.ToString();
                taxCategory = processStage.CategoryId.ToString();
                //check if the user is logged in
                UserDetailsModel userDetails = GetLoggedInUserDetails();
                if (userDetails == null || userDetails.Entity == null) { throw new AuthorizedUserNotFoundException { }; }

                PAYESelectOptionVM viewModel = new PAYESelectOptionVM { HeaderObj = HeaderFiller(userDetails) };
                processStage.InvoiceGenerationStage = InvoiceGenerationStage.SelectPAYEOption;
                Session.Add("InvoiceGenerationStage", JsonConvert.SerializeObject(processStage));
                return View(viewModel);
            }
            #region catch region
            catch (AuthorizedUserNotFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                TempData.Add("Error", ErrorLang.usernotfound().ToString());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in SelectPAYEOption get {0}", exception.Message));
                if (string.IsNullOrEmpty(errorMessage)) { errorMessage = ErrorLang.genericexception().ToString(); }
                TempData.Add("Error", errorMessage);
            }
            #endregion

            if (!string.IsNullOrEmpty(revenueHeadIdentifier)) { TempData.Add("RevenueHeadIdentifier", revenueHeadIdentifier); }
            if (!string.IsNullOrEmpty(taxCategory)) { TempData.Add("TaxCategory", taxCategory); }

            return RedirectToRouteX(Module.Web.RouteName.Collection.GenerateInvoice);
        }


        /// <summary>
        /// route C.SelectPAYEOption
        /// </summary>
        [HttpPost]
        [CBSCollectionAuthorized]
        [BrowserHeaderFilter, ActionName("SelectPAYEOption")]
        public async Task<ActionResult> SelectPAYEOption(PAYESelectOptionVM model)
        {
            TempData = null;
            string revenueHeadIdentifier = string.Empty;
            string taxCategory = string.Empty;
            string errorMessage = null;

            try
            {
                GenerateInvoiceStepsModel processStage = GetDeserializedSessionObj(ref errorMessage);
                revenueHeadIdentifier = processStage.RevenueHeadId.ToString();
                taxCategory = processStage.CategoryId.ToString();
                //check if the user is logged in
                UserDetailsModel user = GetLoggedInUserDetails();
                if (user == null || user.Entity == null) { throw new AuthorizedUserNotFoundException { }; }

                //do stage check
                if (!IsStageCorrect(InvoiceGenerationStage.SelectPAYEOption, processStage.InvoiceGenerationStage))
                {
                    Logger.Error(string.Format("Stage mismatch in SelectPAYEOption. Stage is sess: " + processStage.InvoiceGenerationStage));
                    errorMessage = "Your session could not be continued. Please fill in your details and proceed to start new session.";
                    throw new Exception();
                }

                string routeName = await Task.Run(() => 
                {
                    if (model == null || model.SelectOption == Core.Models.Enums.PAYEDirection.None)
                    {
                        throw new Exception("No direction set");
                    }
                    return Module.Web.RouteName.Collection.ClientPrefix + model.SelectOption.ToString();
                });

                processStage.InvoiceGenerationStage = InvoiceGenerationStage.PAYEProcess;
                Session.Add("InvoiceGenerationStage", JsonConvert.SerializeObject(processStage));
                return RedirectToRouteX(routeName);
            }
            #region catch region
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in SelectPAYEOption get {0}", exception.Message));
                if (string.IsNullOrEmpty(errorMessage)) { errorMessage = ErrorLang.genericexception().ToString(); }
                TempData.Add("Error", errorMessage);
            }
            #endregion

            if (!string.IsNullOrEmpty(revenueHeadIdentifier)) { TempData.Add("RevenueHeadIdentifier", revenueHeadIdentifier); }
            if (!string.IsNullOrEmpty(taxCategory)) { TempData.Add("TaxCategory", taxCategory); }

            return RedirectToRouteX(Module.Web.RouteName.Collection.GenerateInvoice);
        }

    }
}