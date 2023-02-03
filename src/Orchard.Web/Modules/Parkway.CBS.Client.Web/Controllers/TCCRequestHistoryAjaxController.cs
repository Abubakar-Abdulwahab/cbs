using Newtonsoft.Json;
using Orchard;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Users.Events;
using Orchard.Users.Services;
using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Client.Web.ViewModels;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Module.Web.Controllers;
using Parkway.CBS.Module.Web.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.Client.Web.Controllers
{
    public class TCCRequestHistoryAjax : BaseCollectionAjaxController
    {
        private readonly IModuleCollectionAjaxHandler _handler;
        private readonly IOrchardServices _orchardServices;

        private readonly IUserService _userService;
        private readonly IMembershipService _membershipService;
        private readonly ICBSUserManager<CBSUser> _cbsUserService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMembershipValidationService _membershipValidationService;
        private readonly IUserEventHandler _userEventHandler;
        private readonly ICommonBaseHandler _commonBaseHandler;
        private readonly IModuleCollectionHandler _collectionHandler;
        private readonly ITCCRequestHistoryHandler _tccRequestHistoryHandler;


        public TCCRequestHistoryAjax(IModuleCollectionAjaxHandler handler, IOrchardServices orchardServices, IUserService userService, IMembershipService membershipService, ICBSUserManager<CBSUser> cbsUserService, IAuthenticationService authenticationService, IMembershipValidationService membershipValidationService, IUserEventHandler userEventHandler, IModuleCollectionHandler collectionHandler, ICommonBaseHandler commonBaseHandler, ITCCRequestHistoryHandler tccRequestHistoryHandler) : base(handler, orchardServices, userService, membershipService, cbsUserService, authenticationService, membershipValidationService, userEventHandler, commonBaseHandler)
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
            _commonBaseHandler = commonBaseHandler;
            _collectionHandler = collectionHandler;
            _tccRequestHistoryHandler = tccRequestHistoryHandler;
        }

        public virtual JsonResult RequestHistoryMoveRight(string token, int? page)
        {
            Logger.Information(string.Format("getting request history page data for batch token - {0} page - {1}", "", page.ToString()));
            return Json(_tccRequestHistoryHandler.GetPagedRequestsData(token, page), JsonRequestBehavior.AllowGet);
        }
    }
}