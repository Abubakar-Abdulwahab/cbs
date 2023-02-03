using Orchard;
using Orchard.DisplayManagement;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Admin.RouteName;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.Lang;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Parkway.CBS.Police.Admin.Controllers
{
    [Admin]
    public class PSSAdminUserReportController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        private readonly IPSSAdminUserReportHandler _handler;
        dynamic Shape { get; set; }
        public ILogger Logger { get; set; }
        private readonly IPSSAdminUserHandler _adminUserHandler;


        public PSSAdminUserReportController(INotifier notifier, IShapeFactory shapeFactory, IOrchardServices orchardServices, IPSSAdminUserReportHandler handler, IPSSAdminUserHandler adminUserHandler)
        {
            _notifier = notifier;
            _orchardServices = orchardServices;
            Shape = shapeFactory;
            Logger = NullLogger.Instance;
            _handler = handler;
            _adminUserHandler = adminUserHandler;
        }

        /// <summary>
        /// Get a report admin users
        /// </summary>
        /// <param name="pagerParameters"></param>
        /// <returns></returns>
        public ActionResult AdminUserReport(PSSAdminUserReportVM userInputModel, PagerParameters pagerParameters)
        {
            try
            {
                _adminUserHandler.CheckForPermission(Permissions.CanViewAdminUsers);

                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                int take = pagerParameters.PageSize ?? 10;

                if (take == 0) { take = 10; pager.PageSize = 10; }
                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * take : 0;


                AdminUserReportSearchParams searchParams = new AdminUserReportSearchParams
                {
                    Take = take,
                    Skip = skip,
                    CommandId = userInputModel.CommandId,
                    CommandCategoryId = userInputModel.CommandCategoryId,
                    Status = userInputModel.Status,
                    RoleType = userInputModel.RoleTypeId,
                    Username = userInputModel.Username
                };

                PSSAdminUserReportVM vm = _handler.GetVMForReports(searchParams);

                var pageShape = Shape.Pager(pager).TotalItemCount(vm.TotalAdminUserRecord);
                pageShape.RouteData(DoRouteDataForAdminUserReport(vm));

                vm.Pager = pageShape;

                return View(vm);
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, PoliceErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return Redirect("~/Admin");
            }
        }

        public ActionResult ToggleUserStatus(int userPartRecordId, bool status)
        {
            try
            {
                _adminUserHandler.CheckForPermission(Permissions.CanEditAdminUsers);

                if (userPartRecordId <= 0)
                {
                    _notifier.Add(NotifyType.Information, ErrorLang.invalidinputtype());
                    return RedirectToRoute(UserManagement.PSSUserReport);
                }

                if (userPartRecordId == _orchardServices.WorkContext.CurrentUser.Id)
                {
                    _notifier.Add(NotifyType.Information, PoliceLang.ToLocalizeString("You can't disable your own account. Please log in with another account"));
                    return RedirectToRoute(UserManagement.PSSUserReport);
                }

                string userName = _handler.ToggleIsActiveAdminUser(userPartRecordId, isActive: status);

                _notifier.Add(NotifyType.Information, PoliceLang.ToLocalizeString($"You have successfully {(status ? "activated" : "deactivated")} {userName}"));
                return RedirectToRoute(UserManagement.PSSUserReport);
            }
            catch (CBSUserNotFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.usernotfound());
                return Redirect("~/Admin");
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, PoliceErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return Redirect("~/Admin");
            }
        }

        private object DoRouteDataForAdminUserReport(PSSAdminUserReportVM model)
        {
            RouteData routeData = new RouteData();
            if (!string.IsNullOrEmpty(model.Username)) { routeData.Values.Add(nameof(model.Username), model.Username); }
            if (model.CommandId > 0) { routeData.Values.Add("CommandId", model.CommandId); }
            if (model.CommandCategoryId > 0) { routeData.Values.Add("CommandCategoryId", model.CommandCategoryId); }
            if (model.RoleTypeId > 0) { routeData.Values.Add("RoleTypeId", model.RoleTypeId); }
            if (model.Status != (int)Core.Models.Enums.StatusFilter.All) { routeData.Values.Add("Status", model.Status); }
            return routeData;
        }
    }
}