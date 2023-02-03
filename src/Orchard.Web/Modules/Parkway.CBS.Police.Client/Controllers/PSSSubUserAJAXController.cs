﻿using Orchard.Logging;
using Orchard.Security;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using System;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Client.Controllers
{
    public class PSSSubUserAJAXController : BaseController
    {
        private readonly IHandler _compHandler;
        private readonly IAuthenticationService _authenticationService;
        private readonly IPSSSubUserHandler _handler;
        public PSSSubUserAJAXController(IPSSSubUserHandler handler, IAuthenticationService authenticationService, IHandler compHandler) : base(authenticationService, compHandler)
        {
            _authenticationService = authenticationService;
            _compHandler = compHandler;
            _handler = handler;
            Logger = NullLogger.Instance;
        }

        public JsonResult PSSSubUsersMoveRight(string token, int page)
        {
            try
            {
                Logger.Information(string.Format("getting pss sub users page data for batch token - {0} page - {1}", "", page.ToString()));
                return Json(_handler.GetPagedSubUsersData(token, page, GetLoggedInUserDetails().TaxPayerProfileVM.Id), JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().ToString() }, JsonRequestBehavior.AllowGet);
            }

        }


        public JsonResult ToggleSubUserStatus(int subUserUserId, bool isActive)
        {
            Logger.Information($"{(isActive ? "Activating" : "Deactivating")} sub user with user part record id {subUserUserId}");
            return Json(_handler.ToggleSubUserStatus(subUserUserId, isActive), JsonRequestBehavior.AllowGet);
        }
    }
}