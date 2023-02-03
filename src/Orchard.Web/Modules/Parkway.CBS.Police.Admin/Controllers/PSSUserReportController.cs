using Orchard;
using Orchard.DisplayManagement;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.Lang;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Routing;

namespace Parkway.CBS.Police.Admin.Controllers
{
    [Admin]
    public class PSSUserReportController : Controller
    {
        private readonly IPSSUserReportHandler _handler;
        private readonly INotifier _notifier;
        private readonly IOrchardServices _orchardServices;
        public PSSUserReportController(INotifier notifier, IShapeFactory shapeFactory, IOrchardServices orchardServices, IPSSUserReportHandler handler)
        {
            _notifier = notifier;
            _orchardServices = orchardServices;
            Shape = shapeFactory;
            Logger = NullLogger.Instance;
            _handler = handler;
        }

        public ILogger Logger { get; set; }
        dynamic Shape { get; set; }

        // GET: UserReport
        public ActionResult UserReport(PSSUserReportVM userInputModel, PagerParameters pagerParameters)
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanViewUsers);

                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                DateTime startDate = DateTime.Now.Date.AddMonths(-3);
                DateTime endDate = DateTime.Now;

                if (!string.IsNullOrEmpty(userInputModel.From) && !string.IsNullOrEmpty(userInputModel.End))
                {
                    try
                    {
                        startDate = DateTime.ParseExact(userInputModel.From, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        endDate = DateTime.ParseExact(userInputModel.End, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                //get the date up until the final sec
                endDate = endDate.Date.AddDays(1).AddMilliseconds(-1);

                int take = pagerParameters.PageSize ?? 10;

                if (take == 0) { take = 10; pager.PageSize = 10; }
                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * take : 0;


                PSSUserReportSearchParams searchParams = new PSSUserReportSearchParams
                {
                    Take = take,
                    Skip = skip,
                    EndDate = endDate,
                    StartDate = startDate,
                    Name = userInputModel.Name,
                    UserName = userInputModel.UserName,
                    IdentificationNumber = userInputModel.IdentificationNumber
                };

                PSSUserReportVM vm = _handler.GetVMForReports(searchParams);

                var pageShape = Shape.Pager(pager).TotalItemCount(vm.TotalNumberOfUsers);
                pageShape.RouteData(DoRouteDataForUserReport(vm));

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


        public ActionResult RevalidateUser(string payerId)
        {
            try
            {
                Logger.Information($"Admin user with user part record id {_orchardServices.WorkContext.CurrentUser.Id} is revalidating client user with payer id {payerId}");
                string errorMessage;
                _handler.CheckForPermission(Permissions.CanRevalidateUser);
                if (string.IsNullOrEmpty(payerId?.Trim()))
                {
                    Logger.Error("Profile Id not specified");
                    _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString("Profile Id not specified"));
                    return RedirectToRoute(RouteName.PSSUserReport.UserReport);
                }

                _handler.RevalidateUserWithIdentificationNumber(payerId, out errorMessage);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    Logger.Error($"Error revalidating user with payer id {payerId}. Error message - {errorMessage}");
                    _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString(errorMessage));
                    return RedirectToRoute(RouteName.PSSUserReport.UserReport);
                }

                _notifier.Add(NotifyType.Information, PoliceLang.ToLocalizeString($"User has been successfully revalidated."));
                return RedirectToRoute(RouteName.PSSUserReport.UserReport);
            }
            catch (CBSUserNotFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.usernotfound());
                return Redirect("~/Admin");
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                Logger.Error($"Admin user with user part record id {_orchardServices.WorkContext.CurrentUser.Id} not authorized to revalidate user with payer id {payerId}");
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


        private object DoRouteDataForUserReport(PSSUserReportVM model)
        {
            RouteData routeData = new RouteData();
            if (!string.IsNullOrEmpty(model.UserName)) { routeData.Values.Add(nameof(model.UserName), model.UserName); }
            if (!string.IsNullOrEmpty(model.Name)) { routeData.Values.Add(nameof(model.Name), model.Name); }
            if (!string.IsNullOrEmpty(model.End)) { routeData.Values.Add(nameof(model.End), model.End); }
            if (!string.IsNullOrEmpty(model.From)) { routeData.Values.Add(nameof(model.From), model.From); }
            if (!string.IsNullOrEmpty(model.IdentificationNumber)) { routeData.Values.Add(nameof(model.IdentificationNumber), model.IdentificationNumber); }
            return routeData;
        }

    }
}