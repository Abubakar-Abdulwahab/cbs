using Orchard;
using Orchard.DisplayManagement;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Admin.RouteName;
using Parkway.CBS.Police.Core.Lang;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Admin.Controllers
{
    [Admin]
    public class SettlementReportFeePartiesController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        public ILogger Logger { get; set; }
        dynamic Shape { get; set; }
        private readonly ISettlementFeePartiesHandler _handler;
        public SettlementReportFeePartiesController(IOrchardServices orchardServices, IShapeFactory shapeFactory, ISettlementFeePartiesHandler handler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            Logger = NullLogger.Instance;
            Shape = shapeFactory;
            _handler = handler;
        }

        public ActionResult ViewParties(int settlementId, PagerParameters pagerParameters)
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanViewSettlementReportParties);

                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                int take = pagerParameters.PageSize ?? 10;

                if (take == 0) { take = 10; pager.PageSize = 10; }
                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * take : 0;

                SettlementFeePartiesSearchParams searchParams = new SettlementFeePartiesSearchParams
                {
                    Take = take,
                    Skip = skip,
                    SettlementId = settlementId
                };

                var vm = _handler.GetVMForReports(searchParams);

                vm.Pager = Shape.Pager(pager).TotalItemCount(vm.TotalNumberOfActiveSettlementFeeParties);

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


        [HttpGet]
        public ActionResult EditParties(int settlementId)
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanEditSettlementReportParties);

                SettlementFeePartiesSearchParams searchParams = new SettlementFeePartiesSearchParams
                {
                    SettlementId = settlementId,
                    DontPageData = true
                };

                var vm = _handler.GetVMForEditParties(searchParams);

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


        [HttpPost]
        public ActionResult EditParties(PSSSettlementFeePartiesVM userInput)
        {
            List<ErrorModel> errors = new List<ErrorModel> { };
            try
            {
                _handler.CheckForPermission(Permissions.CanEditSettlementReportParties);
                _handler.EditSettlementFeeParty(ref errors, userInput);
                _notifier.Add(NotifyType.Information, PoliceLang.savesuccessfully);
                return RedirectToRoute(SettlementReportFeeParties.EditParties, new { settlementId = userInput.SettlementId });
            }
            catch (DirtyFormDataException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString(errors.First().ErrorMessage)); 
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

            PSSSettlementFeePartiesVM model = _handler.GetVMForEditParties(new SettlementFeePartiesSearchParams { SettlementId = userInput.SettlementId});
            userInput.SelectedSettlementFeeParties = model.SelectedSettlementFeeParties;
            userInput.FeePartyAdapters = model.FeePartyAdapters;
            userInput.FeeParties = model.FeeParties;

            return View(userInput);
        }
    }
}