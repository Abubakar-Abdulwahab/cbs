using Orchard;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Parkway.CBS.Module.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.Exceptions;
using System;
using System.Web.Mvc;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.HelperModels;
using Orchard.UI.Navigation;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Module.Controllers
{
    [Admin]
    public class SettlementController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        private dynamic Shape { get; set; }
        private readonly ISettlementHandler _handler;
        public ILogger Logger { get; set; }

        private static Localizer _t = NullLocalizer.Instance;

        private static Localizer T { get { return _t; } }

        public SettlementController(IOrchardServices orchardServices, ISettlementHandler handler, IShapeFactory shapeFactory)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            _handler = handler;
            Logger = NullLogger.Instance;
            Shape = shapeFactory;
        }


        /// <summary>
        /// create settlement view controller
        /// </summary>
        public ActionResult CreateSettlementRule()
        {
            try
            {
                return View(_handler.GetCreateSettlementRule(new List<string>{"0"}));
            }
            catch(UserNotAuthorizedForThisActionException exception)
            {
                var message = String.Format("\nUser ID {0} tried to manage admin settlement without permission", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Information(exception, exception.Message + message);
                _notifier.Add(NotifyType.Error, ErrorLang.usernotauthorized());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
            }
            return Redirect("~/Admin");
        }



        /// <summary>
        /// Create settlement view controller POST
        /// </summary>
        /// <param name="model"></param>
        [HttpPost, ActionName("CreateSettlementRule")]
        public ActionResult CreateSettlementRule(SettlementRuleVM model)
        {
            try
            {
                if (!this.TryValidateModel(model)) { throw new DirtyFormDataException { }; }
                _handler.TryCreateSettlementRuleForStaging(this, model);
                return RedirectToRoute("ListOfSettlementRules");
            }
            #region Catch clauses
            catch (DirtyFormDataException)
            {
                _notifier.Add(NotifyType.Error, ErrorLang.errorvalidatingform());
                _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString(string.Join(",", ModelState.SelectMany(x => x.Value.Errors).Select(iner => iner.ErrorMessage).ToArray())));
            }
            catch (RecordAlreadyExistsException)
            {
                _notifier.Add(NotifyType.Warning, ErrorLang.mdarevenueprovidercombinationalreadyexists());
            }
            catch (UserNotAuthorizedForThisActionException exception)
            {
                var message = String.Format("\nUser ID {0} tried to manage admin settlement without permission", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Information(exception, exception.Message + message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
            }
            #endregion

            
            var pageModel = _handler.GetCreateSettlementRule(model.SMDAIds);
            model.PaymentProviders = pageModel.PaymentProviders;
            model.MDAs = pageModel.MDAs;
            model.SelectedMdas = pageModel.SelectedMdas;
            model.PaymentChannels = pageModel.PaymentChannels;
            if (model.RevenueHeadsSelected != null)
            {
                model.ApplyToAllRevenueHeads = model.RevenueHeadsSelected.Contains(0);
            }
            else { model.RevenueHeadsSelected = new List<int> { }; }
            return View(model);
        }


        public ActionResult Settlements(SettlementsViewModel userInput, PagerParameters pagerParameters)
        {
            try
            {
                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);
                int take = pagerParameters.PageSize.HasValue ? pagerParameters.PageSize.Value : 10;
                if (take == 0) { take = 10; pager.PageSize = 10; }
                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * 10 : 0;

                SettlementRulesSearchParams searchData = new SettlementRulesSearchParams
                {
                    Name = userInput.Name,
                    RuleIdentifier = userInput.RuleIdentifier,
                    Skip = skip,
                    Take = take
                };

                SettlementsViewModel model = _handler.GetSettlementRulesReport(this, searchData);

                var pageShape = Shape.Pager(pager).TotalItemCount(model.TotalRules);

                model.Pager = pageShape;

                return View(model);
            }
            #region Catch clauses
            catch (UserNotAuthorizedForThisActionException exception)
            {
                var message = String.Format("\nUser ID {0} tried to manage admin settings without permission", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Warning(exception, exception.Message + message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
            }
            #endregion
            return Redirect("~/Admin");
        }

    }
}