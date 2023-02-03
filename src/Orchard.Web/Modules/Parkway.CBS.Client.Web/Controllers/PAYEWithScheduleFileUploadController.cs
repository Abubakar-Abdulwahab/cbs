using Newtonsoft.Json;
using Orchard;
using Orchard.Logging;
using Orchard.Security;
using Orchard.UI.Notify;
using Orchard.Users.Events;
using Orchard.Users.Services;
using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Module.Web.Controllers;
using Parkway.CBS.Module.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Module.Web.Middleware.Filters;
using System;
using System.IO;
using System.Web.Mvc;


namespace Parkway.CBS.Client.Web.Controllers
{
    public class PAYEWithScheduleFileUploadController : BaseController
    {
        private readonly IPAYEFileUploadHandler _handler;
        private readonly IOrchardServices _orchardServices;

        private readonly IUserService _userService;
        private readonly IMembershipService _membershipService;
        private readonly ICBSUserManager<CBSUser> _cbsUserService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMembershipValidationService _membershipValidationService;
        private readonly IUserEventHandler _userEventHandler;


        public PAYEWithScheduleFileUploadController(IPAYEFileUploadHandler handler, IOrchardServices orchardServices, IUserService userService, IMembershipService membershipService, ICBSUserManager<CBSUser> cbsUserService, IAuthenticationService authenticationService, IMembershipValidationService membershipValidationService, IUserEventHandler userEventHandler, ICommonBaseHandler commonHandler) : base(orchardServices, userService, membershipService, cbsUserService, authenticationService, membershipValidationService, userEventHandler, commonHandler)
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


        [CBSCollectionAuthorized]
        [HttpPost]
        public ActionResult PAYEFileUpload()
        {
            TempData = null;
            string errorMessage = null;
            try
            {
                Logger.Information("Processing for PAYEFileUpload payment assessment post");
                GenerateInvoiceStepsModel processStage = GetDeserializedSessionObj(ref errorMessage);
                if (!IsStageCorrect(InvoiceGenerationStage.PAYEProcess, processStage.InvoiceGenerationStage))
                {
                    Logger.Error(string.Format("Stage mismatch in NoScheduleOption. Stage is sess: " + processStage.InvoiceGenerationStage));
                    errorMessage = "Your session could not be continued. Please fill in your details and proceed to start new session.";
                    throw new Exception();
                }
                var file = HttpContext.Request.Files.Get("assessmentfile");

                _handler.ValidateFileUpload(file, ref errorMessage);
                _handler.DoProcessingForPAYEFileUpload(processStage, GetLoggedInUserDetails(), file);

                Session.Add("InvoiceGenerationStage", JsonConvert.SerializeObject(processStage));
                return RedirectToRouteX(RouteName.PAYEWithSchedule.ShowScheduleResult);
            }
            catch (FileNotFoundException)
            {
                TempData.Add("Error", "Upload file not found.");
                return RedirectToRouteX(RouteName.PAYEWithSchedule.WithScheduleOption);
            }
            catch (AuthorizedUserNotFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                TempData.Add("Error", ErrorLang.usernotfound().ToString());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                TempData["Error"] = string.IsNullOrEmpty(errorMessage) ? ErrorLang.genericexception().ToString() : errorMessage;
            }
            return RedirectToRouteX(Module.Web.RouteName.Collection.GenerateInvoice);

        }

    }
}