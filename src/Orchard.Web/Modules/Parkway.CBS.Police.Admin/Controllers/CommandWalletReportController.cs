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
using System.Web.Mvc;
using System.Web.Routing;

namespace Parkway.CBS.Police.Admin.Controllers
{
    [Admin]
    public class CommandWalletReportController : Controller
    {
        private readonly IOrchardServices _orchardServices;

        private readonly INotifier _notifier;
        public ILogger Logger { get; set; }
        dynamic Shape { get; set; }

        private readonly ICommandWalletReportHandler _commandWalletReportHandler;


        public CommandWalletReportController(IOrchardServices orchardServices, INotifier notifier, ICommandWalletReportHandler commandReportHandler, IShapeFactory shapeFactory)
        {
            _orchardServices = orchardServices;
            _notifier = notifier;
            Logger = NullLogger.Instance;
            _commandWalletReportHandler = commandReportHandler;
            Shape = shapeFactory;
        }

        /// <summary>
        /// Get a report of all commands wallets
        /// </summary>
        /// <param name="pagerParameters"></param>
        /// <returns></returns>
        public ActionResult CommandWalletReport(CommandReportVM userInputModel, PagerParameters pagerParameters)
        {
            try
            {
                _commandWalletReportHandler.CheckForPermission(Permissions.CanViewCommandWalletReports);

                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                int take = pagerParameters.PageSize ?? 10;

                if (take == 0) { take = 10; pager.PageSize = 10; }
                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * take : 0;

                CommandWalletReportSearchParams searchParams = new CommandWalletReportSearchParams
                {
                    Take = take,
                    Skip = skip,
                    AccountNumber = userInputModel.AccountNumber?.Trim(),
                    CommandName = userInputModel.CommandName?.Trim(),
                    SelectedAccountType = (int)userInputModel.SelectedAccountType
                };

                CommandReportVM vm = _commandWalletReportHandler.GetVMForReports(searchParams);

                var pageShape = Shape.Pager(pager).TotalItemCount(vm.TotalActiveCommandRecord);
                pageShape.RouteData(DoRouteDataForCommandReport(vm));

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

        private object DoRouteDataForCommandReport(CommandReportVM model)
        {
            RouteData routeData = new RouteData();
            if (!string.IsNullOrEmpty(model.AccountNumber)) { routeData.Values.Add("AccountNumber", model.AccountNumber); }
            if (!string.IsNullOrEmpty(model.CommandName)) { routeData.Values.Add("CommandName", model.CommandName); }
            if ((int)model.SelectedAccountType > 0) { routeData.Values.Add("SelectedAccountType", model.SelectedAccountType); }
            return routeData;
        }

    }
}