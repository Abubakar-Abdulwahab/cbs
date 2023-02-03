using System;
using System.Web.Mvc;
using Newtonsoft.Json;
using Orchard.Logging;
using Orchard.Security;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Client.Middleware;
using Parkway.CBS.Police.Core.DTO;

namespace Parkway.CBS.Police.Client.Controllers
{
    public class SelectServiceController : BaseController
    {
        private readonly IHandler _compHandler;
        private readonly IAuthenticationService _authenticationService;
        private readonly ISelectServiceHandler _handler;

        public SelectServiceController(IHandler compHandler, IAuthenticationService authenticationService, ISelectServiceHandler selectServiceHandler)
            : base(authenticationService, compHandler)
        {
            _compHandler = compHandler;
            _authenticationService = authenticationService;
            _handler = selectServiceHandler;
        }


        /// <summary>
        /// Route Name: P.SelectService
        /// </summary>
        public ActionResult SelectService()
        {
            string message = string.Empty;
            bool hasMessage = false;
            FlashObj flashObj = null;
            string serviceIdentifier = string.Empty;
            string categoryType = string.Empty;
            SelectServiceVM viewModel = new SelectServiceVM();
            try
            {
                try
                {
                    if (TempData.ContainsKey("Error"))
                    {
                        hasMessage = true;
                        flashObj = new FlashObj { MessageTitle = "Error!", Message = TempData["Error"].ToString(), FlashType = CBS.Core.Models.Enums.FlashType.Error };
                        TempData.Remove("Error");
                    }
                }
                catch (Exception exception) { Logger.Error(exception, "Error getting error value from temp data " + exception.Message); }
                TempData = null;

                UserDetailsModel userDetails = GetLoggedInUserDetails();

                if (Session["PSSRequestStage"] != null)
                {
                    try
                    {
                        Session.Remove("PSSRequestStage");
                    }
                    catch (Exception exception)
                    {
                        Logger.Error(exception, string.Format("Exception getting removing PSSRequestStage from session value {0}", exception.Message));
                    }
                }

                viewModel = _handler.GetSelectServiceVM(userDetails);
                viewModel.HeaderObj = HeaderFiller(userDetails);
                viewModel.FlashObj = flashObj;
                viewModel.HasMessage = hasMessage;
                viewModel.ServiceIdentifier = serviceIdentifier;
                viewModel.TaxPayerType = userDetails?.CategoryVM.Id.ToString();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error in SelectService block " + exception.Message);
                return RedirectToRoute("P.Error.Page");
            }
            return View(viewModel);
        }


        /// <summary>
        /// RouteName: P.SelectService
        /// </summary>
        /// <param name="taxCategory"></param>
        /// <param name="serviceIdentifier"></param>
        /// <returns></returns>
        [HttpPost, ActionName("SelectService")]
        public ActionResult SelectService(string taxCategory, string serviceIdentifier, string subCategoryIdentifier, string subSubCategoryIdentifier)
        {
            string errorMessage = string.Empty;
            string categoryType = string.Empty;
            SelectServiceVM viewModel = new SelectServiceVM();
            UserDetailsModel userDetails = GetLoggedInUserDetails();

            try
            {
                TempData = null;
                if (Session["PSSRequestStage"] != null) { Session.Remove("PSSRequestStage"); }
                //get logged in user details if applicable

                if (string.IsNullOrEmpty(serviceIdentifier))
                { errorMessage = ErrorLang.custommessage("Service type not found").ToString(); throw new NoRecordFoundException("Service type not found"); }
                if (string.IsNullOrEmpty(taxCategory))
                { errorMessage = ErrorLang.custommessage("Category not found").ToString(); throw new NoCategoryFoundException("Category not found"); }

                if(userDetails != null && userDetails.CategoryVM.Id.ToString() != taxCategory)
                { errorMessage = ErrorLang.custommessage("Category not found").ToString(); throw new NoCategoryFoundException("Category not found"); }

                int serviceId = 0;
                if (!Int32.TryParse(serviceIdentifier, out serviceId))
                { errorMessage = ErrorLang.custommessage("Service type not found").ToString(); throw new NoRecordFoundException("Service type not found. Invalid input"); }
                int categoryId = 0;

                if(!Int32.TryParse(taxCategory, out categoryId))
                { errorMessage = ErrorLang.custommessage("Category type not found").ToString(); throw new NoRecordFoundException("Category type not found. Invalid input"); }


                int subCategoryId = 0;
                bool parsed = true;
                if (!string.IsNullOrEmpty(subCategoryIdentifier))
                {
                    parsed = Int32.TryParse(subCategoryIdentifier, out subCategoryId);
                }

                int subSubCategoryId = 0;
                if (!string.IsNullOrEmpty(subSubCategoryIdentifier))
                {
                    parsed = Int32.TryParse(subSubCategoryIdentifier, out subSubCategoryId);
                }
                else
                {
                    TaxEntitySubSubCategoryVM subCategory = _handler.GetDefaultSubSubCategoryVM(subCategoryId);
                    if (subCategory != null) { subSubCategoryId = subCategory.Id; }
                }
                if (!parsed)
                { errorMessage = ErrorLang.custommessage("Category not found").ToString(); throw new NoCategoryFoundException("Category not found. Invalid input"); }

                //load session with stage value
                PSServiceVM serviceVM = _handler.GetServiceType(serviceId, categoryId);
                serviceVM.ServiceId = serviceId;
                serviceVM.CategoryId = categoryId;
                //do siwtch for logged in users
                dynamic routeAndStage = _handler.GetNextActionModelForSelectService(userDetails != null);
                if(userDetails == null) { userDetails = new UserDetailsModel { CBSUserVM = new CBSUserVM { } }; }

                StartRequestSession(serviceVM, subCategoryId, subSubCategoryId, routeAndStage.Stage, userDetails.CBSUserVM);

                return RedirectToRoute(routeAndStage.RouteName);
            }
            catch (NoRecordFoundException exception)
            {
                Logger.Error(exception, "Error in SelectService post block " + exception.Message);
                if (string.IsNullOrEmpty(errorMessage)) { errorMessage = ErrorLang.norecord404().ToString(); }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error in SelectService post block " + exception.Message);
                if (string.IsNullOrEmpty(errorMessage)) { errorMessage = ErrorLang.genericexception().ToString(); }
            }

            viewModel = _handler.GetSelectServiceVM(userDetails);
            viewModel.HasMessage = true;
            viewModel.HeaderObj = HeaderFiller(userDetails);
            viewModel.TaxPayerType = userDetails != null? userDetails.CategoryVM.Id.ToString() : taxCategory;

            viewModel.ServiceIdentifier = serviceIdentifier;
            viewModel.FlashObj = new FlashObj { FlashType = CBS.Core.Models.Enums.FlashType.Error, Message = errorMessage, MessageTitle = "Error!" };
            return View(viewModel);
        }
    }
}