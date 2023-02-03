using Orchard;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Orchard.UI.Navigation;
using Parkway.CBS.Police.Admin.VM;
using System;
using System.Web.Mvc;
using System.Globalization;
using Parkway.CBS.Police.Core.VM;
using Orchard.DisplayManagement;
using System.Web.Routing;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.Lang;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;
using System.Linq;
using Parkway.CBS.Police.Admin.RouteName;

namespace Parkway.CBS.Police.Admin.Controllers
{
    [Admin]
    public class SettlementFeePartiesController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        public ILogger Logger { get; set; }
        dynamic Shape { get; set; }
        private readonly ISettlementFeePartiesHandler _handler;
        public SettlementFeePartiesController(IOrchardServices orchardServices, IShapeFactory shapeFactory, ISettlementFeePartiesHandler handler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            Logger = NullLogger.Instance;
            Shape = shapeFactory;
            _handler = handler;
        }


        public ActionResult FeeParties(PagerParameters pagerParameters)
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanViewSettlementFeeParties);

                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                int take = pagerParameters.PageSize ?? 10;

                if (take == 0) { take = 10; pager.PageSize = 10; }
                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * take : 0;

                FeePartyReportSearchParams settlementFeePartyReportSearchParams = new FeePartyReportSearchParams
                {
                    Skip = skip,
                    Take = take
                };

                SettlementFeePartiesVM settlementFeePartiesVM = _handler.GetSettlementFeePartiesReportVM(settlementFeePartyReportSearchParams);
                settlementFeePartiesVM.Pager = Shape.Pager(pager).TotalItemCount(settlementFeePartiesVM.TotalNumberOfFeePartyConfiguration);

                return View(settlementFeePartiesVM);
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
        public ActionResult AddFeeParty()
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanAddSettlementFeeParties);

                return View(_handler.GetAddSettlementFeePartyVM());
            }catch (UserNotAuthorizedForThisActionException)
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
        public ActionResult AddFeeParty(AddSettlementFeePartyVM userInput)
        {
            List<ErrorModel> errors = new List<ErrorModel> { };
            try
            {
                _handler.CheckForPermission(Permissions.CanAddSettlementFeeParties);
                _handler.AddFeeParty(ref errors, userInput);
                _notifier.Add(NotifyType.Information, PoliceLang.savesuccessfully);
                return RedirectToRoute(SettlementFeeParties.FeeParties);
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

            AddSettlementFeePartyVM model = _handler.GetAddSettlementFeePartyVM();
            userInput.Banks = model.Banks;
            userInput.FeePartyAdapterConfigurations = model.FeePartyAdapterConfigurations;

            return View(userInput);

        }
    }
}