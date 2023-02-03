using Orchard;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Users.Events;
using Orchard.Users.Services;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Module.Web.Controllers;
using Parkway.CBS.Module.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.OSGOF.Web.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.OSGOF.Web.Controllers
{
    public class CollectionAjaxController : BaseCollectionAjaxController
    {
        private readonly IOSGOFCollectionAjaxHandler _handler;
        private readonly IOrchardServices _orchardServices;

        private readonly IUserService _userService;
        private readonly IMembershipService _membershipService;
        private readonly ICBSUserManager<CBSUser> _cbsUserService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMembershipValidationService _membershipValidationService;
        private readonly IUserEventHandler _userEventHandler;
        private readonly ICellSiteHandler _cellSiteHandler;
        private readonly IModuleCollectionHandler _collectionHandler;
        private readonly ICommonBaseHandler _commonBaseHandler;


        public CollectionAjaxController(IOSGOFCollectionAjaxHandler handler, IOrchardServices orchardServices, IUserService userService, IMembershipService membershipService, ICBSUserManager<CBSUser> cbsUserService, IAuthenticationService authenticationService, IMembershipValidationService membershipValidationService, IUserEventHandler userEventHandler, ICellSiteHandler cellSiteHandler, IModuleCollectionHandler collectionHandler, ICommonBaseHandler commonBaseHandler) : base(handler, orchardServices, userService, membershipService, cbsUserService, authenticationService, membershipValidationService, userEventHandler, commonBaseHandler)
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
            _cellSiteHandler = cellSiteHandler;
            _collectionHandler = collectionHandler;
            _commonBaseHandler = commonBaseHandler;
        }


        /// <summary>
        /// Get the list of cell sites that belong to an operator
        /// </summary>
        /// <param name="taxEntityId"></param>
        /// <param name="LGAId"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetCellSites(string operatorTaxEntityId, string operatorLGAId)
        {

            var model = _handler.GetOperatorCellSites(operatorTaxEntityId, operatorLGAId);

            return Json(model, JsonRequestBehavior.AllowGet);


            //if (!cellSites.ContainsKey(operatorLGAId))
            //{
            //    var model = _handler.GetOperatorCellSites(operatorTaxEntityId, operatorLGAId);

            //    List<CellSitesDropdownBindingVM> vm = model.ResponseObject;

            //    foreach (var resp in vm)
            //    {
            //        cellSites.Add(resp.LGAId.ToString(), new CellSitesDropdownBindingVM { Id = resp.Id, LGAId = resp.LGAId, OperatorSiteId = resp.OperatorSiteId });
            //    }
            //    return Json(model, JsonRequestBehavior.AllowGet);
            //}

            //ResponseObject = cellSites[operatorLGAId];
            //return Json(ResponseObject, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCellSite(string cellSiteId)
        {

            var model = _handler.GetCellSite(cellSiteId);

            return Json(model, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// <para>URL Name: get-next-cellsites-page</para>
        /// </summary>
        /// <param name="batchToken"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public virtual JsonResult CellSitesScheduleUploadMoveRight(string scheduleRef, int page)
        {
            Logger.Information(string.Format("getting page data for scheduleRef - {0} page - {1}", scheduleRef, page));
            return Json(_cellSiteHandler.GetPagedCellSiteForScheduleUpload(scheduleRef, page), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// This is use to get the list of cell sites for operator page display
        /// </summary>
        /// <param name="scheduleRef"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public virtual JsonResult CellSiteListMoveRight(long operatorId, int page)
        {
            Logger.Information(string.Format("getting page data for operator Id - {0} page - {1}", operatorId, page));
            return Json(_cellSiteHandler.GetPagedCellSiteList(operatorId, page), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This is use to get the list of cell sites for operator page display
        /// </summary>
        /// <param name="scheduleRef"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public virtual JsonResult PaymentListMoveRight(string datefilter, long operatorId, int page)
        {
            Logger.Information(string.Format("getting page data for operator Id - {0} page - {1}", operatorId, page));
            return Json(_collectionHandler.GetPagedPaymentList(operatorId, page, datefilter), JsonRequestBehavior.AllowGet);
        }

    }
}