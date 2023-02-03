using Orchard;
using Orchard.DisplayManagement;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Parkway.CBS.Module.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Module.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Globalization;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Module.Controllers
{
    [Admin]
    public class MDAController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        private dynamic Shape { get; set; }
        private readonly IMDAHandler _handler;
        public ILogger Logger { get; set; }

        public MDAController(IOrchardServices orchardServices, IMDAHandler handler, IShapeFactory shapeFactory)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            _handler = handler;
            Shape = shapeFactory;
            Logger = NullLogger.Instance;
        }

        public ActionResult MainDashboard(string fromRange, string endRange, string mdaSelected)
        {
            try
            {
                DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

                if (!string.IsNullOrEmpty(fromRange) && !string.IsNullOrEmpty(endRange))
                {
                    try
                    {
                        startDate = DateTime.ParseExact(fromRange, "MM/yyyy", CultureInfo.InvariantCulture);
                        endDate = DateTime.ParseExact(endRange, "MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }

                var model = _handler.GetDashboardView(startDate, endDate, mdaSelected);
                model.FromRange = startDate.ToString("MM'/'yyyy");
                model.EndRange = endDate.ToString("MM'/'yyyy");
                return View(model);
            }
            #region catch clauses
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Error(Lang.genericexception_text);
            }
            #endregion
            return RedirectToAction("List", "MDA", new { });
        }


        public ActionResult ViewHierarchy(string slug)
        {
            try
            {
                HierarchyViewModel mda = _handler.GetHierarchy(slug);
                return View(mda);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Error(Lang.genericexception_text);
                return Redirect("~/Admin");
            }
        }

        /// <summary>
        /// Create mda
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateMDASettings()
        {
            try
            {
                return View(_handler.GetMDASettingsView());
            }
            #region catch clauses
            catch (UserNotAuthorizedForThisActionException exception)
            {
                var message = String.Format("\nUser ID {0} tried to save MDAs without permission", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Information(exception, message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (CannotConnectToCashFlowException exception)
            {
                var message = String.Format("\nUser ID {0} tried to save MDAs, but connection to cashflow failed", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message + exception.Message);
                _notifier.Add(NotifyType.Warning, ErrorLang.cannotconnettoinvoicingservice());
                return Redirect("~/Admin");
            }
            catch (TenantNotFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.tenant404());
                return Redirect("~/Admin");
            }
            catch (Exception exception)
            {
                var message = String.Format("\nLinked - Exception occured on LastUpdatedBy ID {0} ", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message + exception.Message);
                _notifier.Add(NotifyType.Warning, Lang.genericexception_text);
                return Redirect("~/Admin");
            }
            #endregion
        }


        [HttpPost, ActionName("CreateMDASettings")]
        public ActionResult CreateMDASettings(MDASettingsViewModel model, string bank, bool useTSA = false)
        {
            try
            {
                _handler.CreateMDASettingsView(this, model.MDA, bank, HttpContext.Request.Files, useTSA);
                _notifier.Information(Lang.mdasaved);
                Logger.Information(Lang.mdasaved.ToString());
                return RedirectToAction("List", "MDA", new { });
            }
            #region catch clauses
            catch (UserNotAuthorizedForThisActionException exception)
            {
                var message = String.Format("\nUser ID {0} tried to save MDAs without permission", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Information(exception, message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch(AuthorizedUserNotFoundException exception)
            {
                var message = String.Format("\nUser ID not found", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Information(exception, message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotfound());
                return Redirect("~/Admin");
            }
            catch (DirtyFormDataException exception)
            {
                var message = String.Format("\nInvalid model state  - LastUpdatedBy ID  {0}", _orchardServices.WorkContext.CurrentUser.Id);
                if (model.UseTSA) { model.MDA.BankDetails = null; }
                Logger.Information(exception, message);
            }
            catch (CannotConnectToCashFlowException exception)
            {
                var message = String.Format("\nUser ID {0} tried to save MDAs, but connection to cashflow failed", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message + exception.Message);
                _notifier.Add(NotifyType.Warning, ErrorLang.cannotconnettoinvoicingservice());
                return Redirect("~/Admin");
            }
            catch (TenantNotFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.tenant404());
                return Redirect("~/Admin");
            }
            catch (CouldNotParseStringValueException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Warning, ErrorLang.invalidinputtype());
            }
            catch (NoBankDetailsOnCashflowFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Warning, ErrorLang.bank404(string.IsNullOrEmpty(bank) ? "TSA value" : bank));
            }
            catch (Exception exception)
            {
                var message = String.Format("\nException occured on LastUpdatedBy ID {0} ", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message + exception.Message);
                _notifier.Add(NotifyType.Warning, ErrorLang.genericexception());
                return Redirect("~/Admin");
            }
            #endregion

            model.Banks = _handler.GetBanks();
            model.SBankId = bank;
            //model.UseTSA = !string.IsNullOrEmpty(_handler.GetClientTenant().TSABankNumber);
            return View(model);
        }

        /// <summary>
        /// View the list of MDAs
        /// </summary>
        /// <param name="options"></param>
        /// <param name="pagerParameters"></param>
        /// <param name="model"></param>
        /// <param name="breadcrumbs"></param>
        /// <returns></returns>
        public ActionResult List(MDAIndexOptions options, PagerParameters pagerParameters, MDAListViewModel model, AdminBreadCrumb breadcrumbs)
        {
            try
            {

                _handler.ViewList();
                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                if (options == null)
                    options = new MDAIndexOptions();

                var filterName = Enum.GetName(typeof(MDAFilter), options.Filter);
                var orderBy = Enum.GetName(typeof(MDAOrder), options.Order);

                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * 10 : 0;
                int count = pagerParameters.PageSize.HasValue ? pagerParameters.PageSize.Value : 10;

                List<MDA> mdaFiltered = new List<MDA>();

                mdaFiltered = _handler.GetCollection(filterName, orderBy, options.Search, options.Direction).ToList();

                var pageShape = Shape.Pager(pager).TotalItemCount(mdaFiltered.Count);

                model.ListOfMDA = mdaFiltered.Skip(skip).Take(count == 0 ? mdaFiltered.Count : count);
                model.Pager = pageShape;
                model.Options = options;
                return View(model);
            }
            #region catch clauses
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, Lang.usernotauthorized_ex_text);
                return Redirect("~/Admin");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Error(Lang.genericexception_text);
                return Redirect("~/Admin");
            }
            #endregion
        }

        public ActionResult Edit(string Slug)
        {
            try
            {
                return View(_handler.GetEditView(Slug));
            }
            #region catch clauses
            catch (UserNotAuthorizedForThisActionException exception)
            {
                var message = String.Format("\nUser ID {0} tried to view MDA without permission", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (MDARecordNotFoundException exception)
            {
                var message = String.Format("\nUser ID {0} tried to editing an MDA which could not be found", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message);
                _notifier.Add(NotifyType.Warning, ErrorLang.mdacouldnotbefound());
            }
            catch (TenantNotFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.tenant404());
                return Redirect("~/Admin");
            }
            catch(Exception exception)
            {
                var message = String.Format("\nException occured on LastUpdatedBy ID {0} ", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message + exception.Message);
                _notifier.Add(NotifyType.Warning, ErrorLang.genericexception());
                return Redirect("~/Admin");
            }
            #endregion
            return RedirectToAction("List", "MDA", new { });
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditPost(MDASettingsViewModel model, string bank, string slug, bool useTSA = false)
        {
            try
            {
                _handler.TryUpdateMDA(this, model.MDA, HttpContext.Request.Files, useTSA, slug, bank);
                _notifier.Add(NotifyType.Information, Lang.mdaupdated);
                return RedirectToAction("List", "MDA", new { });
            }
            #region catch clauses
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, Lang.usernotauthorized_ex_text);
                return Redirect("~/Admin");
            }
            catch (DirtyFormDataException exception)
            {
                var message = String.Format("\nInvalid model state  - LastUpdatedBy ID  {0}", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message);
            }
            catch (MissingFieldException exception)
            {
                var message = String.Format("\nInvalid field composition - LastUpdatedBy ID  {0}", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message);
                _notifier.Error(Lang.genericexception_text);
            }
            catch (MDARecordNotFoundException exception)
            {
                var message = String.Format("\nCould not find mda record for - LastUpdatedBy ID  {0}", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message);
                _notifier.Error(ErrorLang.mdacouldnotbefound());
                return Redirect("~/Admin");
            }
            catch (MDARecordCouldNotBeUpdatedException exception)
            {
                var message = String.Format("\nCould not save mda record for - LastUpdatedBy ID  {0}", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message);
                _notifier.Error(Lang.cannotsavemdarecord_ex_text);
            }
            catch(CannotConnectToCashFlowException exception)
            {
                var message = String.Format("\nUser ID {0} tried to edit MDAs, but connection to cashflow failed", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message + exception.Message);
                _notifier.Add(NotifyType.Warning, ErrorLang.cannotconnettoinvoicingservice());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Error(Lang.genericexception_text);
                return Redirect("~/Admin");
            }
            #endregion
            model.Banks = _handler.GetBanks();
            model.SBankId = bank;
            //remove account number
            model.MDA.BankDetails.BankAccountNumber = "";
            return View(model);
        }

        [HttpPost]
        public ActionResult ChangeStatus(int Id, int pageNumber)
        {
            try
            {
                _handler.ChangeStatus(Id);
            }
            #region catch clauses
            catch (UserNotAuthorizedForThisActionException exception)
            {
                var message = String.Format("\nUser ID {0} tried to view MDA without permission", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message);
                _notifier.Add(NotifyType.Warning, Lang.usernotauthorized_ex_text);
                return Redirect("~/Admin");
            }
            catch (MDARecordNotFoundException exception)
            {
                var message = String.Format("\nUser ID {0} tried to editing an MDA which could not be found", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message);
                _notifier.Add(NotifyType.Warning, Lang.recordcouldnotbefound_text);
            }
            catch (MDARecordCouldNotBeUpdatedException exception)
            {
                var message = String.Format("\nCould not save mda record for - LastUpdatedBy ID  {0}", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message);
                _notifier.Error(Lang.cannotsavemdarecord_ex_text);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Error(Lang.genericexception_text);
                return Redirect("~/Admin");
            }
            #endregion
            var listPageParams = new PagerParameters();
            listPageParams.Page = pageNumber;

            return RedirectToAction("List", "MDA", new { page = pageNumber });
        }

        public ActionResult ViewMDARevenueHeads(string Slug, RHIndexOptions options, PagerParameters pagerParameters, MDARevenueHeadsListPage model)
        {
            try
            {
                MDA mda = new MDA();
                mda = _handler.GetRevenueHeadsView(Slug);

                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                if (options == null)
                    options = new RHIndexOptions();

                var filterName = Enum.GetName(typeof(RevHeadFilter), options.Filter);
                var orderBy = Enum.GetName(typeof(RevHeadOrder), options.Order);

                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * 10 : 0;
                int count = pagerParameters.PageSize.HasValue ? pagerParameters.PageSize.Value : 10;

                var rootRevHeads = mda.RevenueHeads.Where(x => x.Revenuehead == null).ToList();
                var revenueHeads = _handler.GetRevenueHeadsCollection(rootRevHeads, filterName, orderBy, options.Search, options.Direction);

                var pageShape = Shape.Pager(pager).TotalItemCount(revenueHeads.Count);
                model.Mda = mda;
                model.RevenueHeads = revenueHeads.Skip(skip).Take(count == 0 ? revenueHeads.Count : count).ToList();
                model.Pager = pageShape;
                model.Options = options;
            }
            #region catch clauses
            catch (UserNotAuthorizedForThisActionException exception)
            {
                _notifier.Add(NotifyType.Warning, Lang.usernotauthorized_ex_text);
                return Redirect("~/Admin");
            }
            catch (MDARecordNotFoundException exception)
            {
                var message = String.Format("\nUser ID {0} tried to editing an MDA which could not be found", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message);
                _notifier.Add(NotifyType.Warning, Lang.recordcouldnotbefound_text);
                return RedirectToAction("List", "MDA");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Error(Lang.genericexception_text);
                return Redirect("~/Admin");
            }
            #endregion
            return View(model);
        }
    }
}