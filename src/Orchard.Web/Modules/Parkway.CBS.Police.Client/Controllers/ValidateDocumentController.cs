using Orchard.Logging;
using Orchard.Security;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Client.Middleware;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Client.Controllers
{
    //[PSSAuthorized]
    public class ValidateDocumentController : BaseController
    {
        private readonly IHandler _compHandler;
        private readonly IAuthenticationService _authenticationService;
        private readonly IValidateDocumentHandler _handler;


        public ValidateDocumentController(IHandler compHandler, IAuthenticationService authenticationService, IValidateDocumentHandler handler)
            : base(authenticationService, compHandler)
        {
            _compHandler = compHandler;
            _authenticationService = authenticationService;
            _handler = handler;
        }


        /// <summary>
        /// Route name: P.Validate.Document
        /// Validate document approval number
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ValidateDocument()
        {
            string message = string.Empty;
            bool hasError = false;
            try
            {
                if (TempData.ContainsKey("Error"))
                {
                    message = TempData["Error"].ToString();
                    hasError = true;
                    TempData = null;
                }
                TempData = null;

                return View(new ValidateDocumentVM { HeaderObj = HeaderFiller(GetLoggedInUserDetails()), HasErrors = hasError, ErrorMessage = message });
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                TempData.Add("Error", ErrorLang.genericexception());
                return RedirectToRoute(Client.RouteName.SelectService.ShowSelectService);
            }
        }


        /// <summary>
        /// Route name: P.Validate.Document
        /// Validate document approval number
        /// </summary>
        /// <param name="userInput"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ValidateDocument(ValidateDocumentVM userInput)
        {
            string errorMessage = string.Empty;
            try
            {
                if(userInput == null) { userInput = new ValidateDocumentVM { }; }
                userInput.HeaderObj = HeaderFiller(GetLoggedInUserDetails());
                if (string.IsNullOrEmpty(userInput.ApprovalNumber)) { errorMessage = "Document number not specified"; throw new Exception(errorMessage); }
                else
                {
                    userInput.ApprovalNumber = userInput.ApprovalNumber.Trim();
                    if (userInput.ApprovalNumber.Length == 0) { errorMessage = "Document number not specified"; throw new Exception(errorMessage); }
                    return RedirectToRoute(RouteName.ValidateDocument.ValidatedDocumentInfo, new { approvalNumber = userInput.ApprovalNumber });
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                userInput.HasErrors = (userInput.HasErrors == false) ? true : userInput.HasErrors;
                userInput.ErrorMessage = (string.IsNullOrEmpty(errorMessage)) ? ErrorLang.genericexception().ToString() : errorMessage;
            }
            return View(userInput);
        }


        /// <summary>
        /// Route name: P.Validated.Document.Info
        /// Validated document info
        /// </summary>
        /// <param name="userInput"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ValidatedDocumentInfo(string approvalNumber)
        {
            string errorMessage = string.Empty;
            UserDetailsModel user = null;
            TempData = null;
            try
            {
                if (string.IsNullOrEmpty(approvalNumber)) { errorMessage = "Document number not specified"; throw new Exception(errorMessage); }
                else
                {
                    approvalNumber = approvalNumber.Trim();
                    if (approvalNumber.Length == 0) { errorMessage = "Document number not specified"; throw new Exception(errorMessage); }
                }
                string validatedApprovalNumber = _handler.ValidateDocumentApprovalNumber(approvalNumber);
                if (!string.IsNullOrEmpty(validatedApprovalNumber))
                {
                    user = GetLoggedInUserDetails();
                    //if (user == null) { throw new Exception("User not logged in"); }
                    ValidatedDocumentVM userModel = _handler.GetDocumentInfoWithApprovalNumber(validatedApprovalNumber);
                    if (userModel == null)
                    {
                        errorMessage = $"Request with document number {validatedApprovalNumber} was not found";
                        throw new Exception(errorMessage);
                    }
                    userModel.HeaderObj = HeaderFiller(user);
                    userModel.PartialViewName = userModel.DocumentInfo.ViewName;
                    return View(userModel);
                }
                else { errorMessage = "Document number format is not valid"; throw new Exception(errorMessage); }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                TempData["Error"] = (string.IsNullOrEmpty(errorMessage)) ? ErrorLang.genericexception().ToString() : errorMessage;
            }
            return RedirectToRoute(RouteName.ValidateDocument.ShowValidateDocument);
        }

    }
}