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
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Lang;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Routing;

namespace Parkway.CBS.Police.Admin.Controllers
{
    [Admin]
    public class CommandStatementReportController : Controller
    {
        private readonly IOrchardServices _orchardServices;

        private readonly INotifier _notifier;
        public ILogger Logger { get; set; }
        dynamic Shape { get; set; }

        private readonly ICommandStatementReportHandler _commandStatementReportHandler;

        private readonly ICommandManager<Command> _commandManager;

        public CommandStatementReportController(IOrchardServices orchardServices, INotifier notifier, IShapeFactory shapeFactory, ICommandStatementReportHandler commandStatementReportHandler, ICommandManager<Command> commandManager)
        {
            _orchardServices = orchardServices;
            _notifier = notifier;
            Logger = NullLogger.Instance;
            Shape = shapeFactory;
            _commandStatementReportHandler = commandStatementReportHandler;
            _commandManager = commandManager;
        }

        /// <summary>
        /// Get a report a commands statement
        /// </summary>
        /// <param name="pagerParameters"></param>
        /// <returns></returns>
        public ActionResult CommandStatementReport(string code, CommandStatementReportVM userInputModel, PagerParameters pagerParameters)
        {
            try
            {
                _commandStatementReportHandler.CheckForPermission(Permissions.CanViewCommandWalletReports);

                if (string.IsNullOrEmpty(code))
                {
                    throw new Exception($"{nameof(code)} is required");
                }

                DateTime startDate = DateTime.Now.AddMonths(-3);
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

                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                int take = pagerParameters.PageSize ?? 10;

                if (take == 0) { take = 10; pager.PageSize = 10; }
                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * take : 0;

                DateTime? valueDate = null;
                if (!string.IsNullOrEmpty(userInputModel.ValueDate)) { valueDate = DateTime.ParseExact(userInputModel.ValueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture); }

                CommandStatementReportSearchParams searchParams = new CommandStatementReportSearchParams
                {
                    Take = take,
                    Skip = skip,
                    EndDate = endDate,
                    StartDate = startDate,
                    CommandCode = code,
                    ValueDate = valueDate,
                    TransactionTypeId = (int)userInputModel.TransactionType,
                    TransactionReference = userInputModel.TransactionReference
                };

                CommandWalletDetailsVM walletDetailsVM = _commandStatementReportHandler.GetCommandWalletDetailsByCommandCode(code);
                if(walletDetailsVM == null)
                {
                    throw new Exception($"Unable to get wallet details for command code {code}");
                }

                string accountBalance = string.Empty;
                if (string.IsNullOrEmpty(userInputModel.Balance) || userInputModel.Balance.Equals("Not available"))
                {
                    try
                    {
                        decimal balance = _commandStatementReportHandler.GetCustomerAccountBalance(walletDetailsVM.AccountNumber);
                        accountBalance = string.Format("{0:n2}", balance);
                    }
                    catch (Exception exception)
                    {
                        Logger.Error(exception, exception.Message);
                        accountBalance = "Not available";
                    }
                }
                else
                {
                    accountBalance = userInputModel.Balance;
                }

                CommandStatementReportVM vm = _commandStatementReportHandler.GetVMForReports(searchParams);
                vm.WalletReportVM = new CommandWalletReportVM { Balance = accountBalance, AccountNumber = walletDetailsVM.AccountNumber, CommandName = walletDetailsVM.Name };
                vm.Balance = accountBalance;

                var pageShape = Shape.Pager(pager).TotalItemCount(vm.TotalCommandWalletStatementRecord);
                pageShape.RouteData(DoRouteDataForCommandStatementReport(vm));

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

        private object DoRouteDataForCommandStatementReport(CommandStatementReportVM model)
        {
            RouteData routeData = new RouteData();
            if (!string.IsNullOrEmpty(model.TransactionReference)) { routeData.Values.Add("TransactionReference", model.TransactionReference); }
            if (model.TransactionType > 0) { routeData.Values.Add("TransactionType", model.TransactionType); }
            if (!string.IsNullOrEmpty(model.ValueDate)) { routeData.Values.Add("ValueDate", model.ValueDate); }
            return routeData;
        }

    }
}