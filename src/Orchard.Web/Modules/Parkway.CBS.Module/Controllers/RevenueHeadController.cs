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
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.HelperModels;
using Orchard.Themes;

namespace Parkway.CBS.Module.Controllers
{
    [Admin]
    [Themed]
    public class RevenueHeadController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        dynamic Shape { get; set; }
        public ILogger Logger { get; set; }
        private IRevenueHeadHandler _handler;


        public RevenueHeadController(IOrchardServices orchardServices, IShapeFactory shapeFactory, IRevenueHeadHandler handler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            _handler = handler;
            Shape = shapeFactory;
            Logger = NullLogger.Instance;
        }

        //Create a revenue head for this mda slug
        public ActionResult CreateFromMDA(string slug) //slug here is the mda slug
        {
            try
            {
                var model = _handler.CreateRevenueHeadView(slug);
                var crumbShape = Shape.AdminBreadCrumb(model.AdminBreadCrumb);
                model.AdminBreadCrumb = crumbShape;
                return View(model);
            }
            #region catch clauses
            catch (UserNotAuthorizedForThisActionException exception)
            {
                var message = String.Format("\nUser ID {0} tried to view create RH without permission", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (MDARecordNotFoundException exception)
            {
                var message = String.Format("\nUser ID {0} tried to create RH but the given MDA slug could not be found", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message);
                _notifier.Add(NotifyType.Warning, ErrorLang.mdacouldnotbefound());
                return RedirectToAction("List", "MDA");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Error(ErrorLang.genericexception());
            }
            #endregion
            return Redirect("~/Admin");
        }

        [HttpPost, ActionName("CreateFromMDA")]
        public ActionResult CreateFromMDAPost(RevenueHeadCreateFromMDAView model, string slug, ICollection<RevenueHead> RevenueHeadsCollection)
        {
            try
            {
                _handler.TryCreateRevenueHead(this, RevenueHeadsCollection, slug);
                return RedirectToAction("ViewMDARevenueHeads", "MDA", new { Slug = slug });
            }
            #region catch clauses
            catch (UserNotAuthorizedForThisActionException exception)
            {
                var message = String.Format("\nUser ID {0} tried to view create RH without permission", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (AuthorizedUserNotFoundException exception)
            {
                var message = String.Format("\nUser ID {0} was not found", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message + exception.Message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotfound());
                return Redirect("~/Admin");
            }
            catch (MissingFieldException exception)
            {
                var message = String.Format("\nInvalid field composition - LastUpdatedBy ID  {0}", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message);
                _notifier.Error(ErrorLang.genericexception());
            }
            catch (MDARecordNotFoundException exception)
            {
                var message = String.Format("\nUser ID {0} tried to create RH but the given MDA slug could not be found", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message);
                _notifier.Add(NotifyType.Warning, ErrorLang.mdacouldnotbefound());
            }
            catch (DirtyFormDataException exception)
            {
                var message = String.Format("\nInvalid model state for rev head creation  - LastUpdatedBy ID  {0}", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message);
            }
            catch (CannotSaveRevenueHeadException exception)
            {
                var message = String.Format("\nUser ID {0} tried to save a revenue head", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message);
                _notifier.Add(NotifyType.Warning, ErrorLang.revenueheadscouldnotbesaved());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Error(ErrorLang.genericexception());
            }
            #endregion
            model.RevenueHeadsCollection = RevenueHeadsCollection;
            if (model.MDAName != null)
            {
                List<SelectListItem> list = new List<SelectListItem>();
                list.Add(new SelectListItem() { Text = model.MDAName, Value = slug, Selected = true });
                model.Mdas = list;
            }
            else
            {
                model.Mdas = _handler.GetMDAList();
            }
            return View(model);
        }

        /// <summary>
        /// Get view to add revenue heads from a revenue head view
        /// </summary>
        /// <param name="slug"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CreateFromRevenueHead(string slug, int id) //slug here is the parent revenue head slug and id
        {
            try
            {
                var model = _handler.CreateRevenueHeadViewFromRevenueHead(slug, id);
                if (model.HasBilling) { throw new RevenueHeadCannotAddSubRevenueHeadBecauseSetupHasAlreadyStartedException(); }
                model.AdminBreadCrumb = GetCrumbShape(model.AdminBreadCrumb);
                return View(model);
            }
            #region catch clauses
            catch (UserNotAuthorizedForThisActionException exception)
            {
                var message = String.Format("\nUser ID {0} tried to view create RH without permission", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (CannotFindRevenueHeadException exception)
            {
                var message = String.Format("\nUser ID {0} tried to create RH but the given MDA slug could not be found", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message);
                _notifier.Add(NotifyType.Warning, ErrorLang.revenueheadnotfound());
            }
            catch (AuthorizedUserNotFoundException exception)
            {
                var message = String.Format("\nUser ID {0} was not found", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message + exception.Message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotfound());
                return Redirect("~/Admin");
            }
            catch (RevenueHeadCannotAddSubRevenueHeadBecauseSetupHasAlreadyStartedException exception)
            {
                var message = String.Format("\nUser ID {0} tried to add a SRH but has already started the setup process", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message);
                return RedirectToAction("RevenueHeadDashBoard", "RevenueHead", new { revenueHeadSlug = slug, revenueHeadId = id });
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Error(ErrorLang.genericexception());
            }
            #endregion
            return Redirect("~/Admin");
        }

        /// <summary>
        /// Get bread crumb shape
        /// </summary>
        /// <param name="bread"></param>
        /// <returns>dynamic</returns>
        private dynamic GetCrumbShape(AdminBreadCrumb bread)
        {
            return Shape.AdminBreadCrumb(bread);
        }


        /// <summary>
        /// Save data from revenue head view
        /// </summary>
        /// <param name="model"></param>
        /// <param name="slug"></param>
        /// <param name="id"></param>
        /// <param name="revenueHeads"></param>
        /// <returns></returns>
        [HttpPost, ActionName("CreateFromRevenueHead")]
        public ActionResult CreateFromRevenueHeadPost(SubRevenueHeadCreateViewModel model, string slug, int id, ICollection<RevenueHead> RevenueHeadsCollection)
        {
            try
            {
                _handler.TryCreateRevenueHeadFromRevenueHead(this, RevenueHeadsCollection, slug, id);
                return RedirectToAction("ViewSubRevenueHeads", "RevenueHead", new { slug = slug, id = id });
            }
            #region catch clauses
            catch (UserNotAuthorizedForThisActionException exception)
            {
                var message = String.Format("\nUser ID {0} tried to view create RH without permission", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (RevenueHeadCannotAddSubRevenueHeadBecauseSetupHasAlreadyStartedException exception)
            {
                var message = String.Format("\nUser ID {0} tried to add a SRH but has already started the setup process", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message);
                return RedirectToAction("RevenueHeadDashBoard", "RevenueHead", new { revenueHeadSlug = slug, revenueHeadId = id });
            }
            catch (CannotFindRevenueHeadException exception)
            {
                var message = String.Format("\nUser ID {0} tried to create RH but the given MDA slug could not be found", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message);
                _notifier.Add(NotifyType.Warning, ErrorLang.revenuehead404());
            }
            catch (DirtyFormDataException exception)
            {
                var message = String.Format("\nInvalid model state for rev head creation  - LastUpdatedBy ID  {0}", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message);
            }
            catch (MissingFieldException exception)
            {
                var message = String.Format("\nInvalid field composition - LastUpdatedBy ID  {0}", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message);
                _notifier.Error(ErrorLang.genericexception());
            }
            catch (MDARecordNotFoundException exception)
            {
                var message = String.Format("\nCould not find mda record for - LastUpdatedBy ID  {0}", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message);
                _notifier.Error(ErrorLang.mdacouldnotbefound());
                return Redirect("~/Admin");
            }
            catch (CannotUpdateRevenueHeadException exception)
            {
                var message = String.Format("\nUser ID {0} tried to save a revenue head", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message);
                _notifier.Add(NotifyType.Warning, ErrorLang.revenuehead404());
            }
            catch (CannotSaveRevenueHeadException exception)
            {
                var message = String.Format("\nUser ID {0} tried to save a revenue head", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message);
                _notifier.Add(NotifyType.Warning, ErrorLang.revenueheadscouldnotbesaved());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Error(ErrorLang.genericexception());
                return Redirect("~/Admin");
            }
            #endregion
            var parentRevenueHead = _handler.GetRevenueHead(slug, id);
            var tree = new AdminBreadCrumb { BreadLoaf = _handler.Tree(parentRevenueHead) };
            model.AdminBreadCrumb = GetCrumbShape(tree);
            model.RevenueHeadsCollection = RevenueHeadsCollection;
            model.SRHId = parentRevenueHead.Id;
            model.SRHSlug = parentRevenueHead.Slug;
            return View(model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="revenueHeadSlug"></param>
        /// <param name="revenueHeadId"></param>
        /// <returns></returns>
        public ActionResult Edit(string revenueHeadSlug, int revenueHeadId)
        {
            RevenueHead revenueHead = new RevenueHead();
            try
            {
                var model = _handler.GetEditRevenueHeadView(revenueHeadSlug, revenueHeadId);
                var crumbShape = Shape.AdminBreadCrumb(model.AdminBreadCrumb);
                model.AdminBreadCrumb = crumbShape;
                return View(model);
            }
            #region catch clauses
            catch (CannotFindRevenueHeadException exception)
            {
                var message = String.Format("\nUser ID {0} revenue head 404. {1} mda slug {2} rev slug ", _orchardServices.WorkContext.CurrentUser.Id, "", revenueHeadSlug);
                Logger.Error(exception, message);
                _notifier.Add(NotifyType.Warning, ErrorLang.revenueheadnotfound());
            }
            catch (UserNotAuthorizedForThisActionException exception)
            {
                var message = String.Format("\nUser ID {0} tried to view create RH without permission", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (MDARecordNotFoundException exception)
            {
                var message = String.Format("\nUser ID {0} tried to editing an MDA which could not be found", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message);
                _notifier.Add(NotifyType.Warning, ErrorLang.mdacouldnotbefound());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Error(Lang.genericexception_text);
            }
            #endregion
            return Redirect("~/Admin");
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditPost(RevenueHeadCreateView model, string revenueHeadSlug, int revenueHeadId)
        {
            RevenueHead revenueHead = new RevenueHead();
            try
            {
                revenueHead = _handler.TryUpdateRevenueHead(this, model.RevenueHead, revenueHeadSlug, revenueHeadId);

                if (revenueHead.Revenuehead == null) { return RedirectToAction("ViewMDARevenueHeads", "MDA", new { Slug = revenueHead.Mda.Slug }); }
                else { return RedirectToAction("ViewSubRevenueHeads", "RevenueHead", new { slug = revenueHead.Revenuehead.Slug, id = revenueHead.Revenuehead.Id }); }
            }
            #region catch clauses
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (AuthorizedUserNotFoundException exception)
            {
                var message = String.Format("\nUser ID {0} was not authorized", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message + exception.Message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (CannotFindRevenueHeadException exception)
            {
                var message = String.Format("\nUser ID {0} tried to create RH but the given MDA slug could not be found", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message);
                _notifier.Add(NotifyType.Warning, ErrorLang.revenuehead404());
                return Redirect("~/Admin");
            }
            catch (DirtyFormDataException exception)
            {
                var message = String.Format("\nInvalid model state for rev head creation  - LastUpdatedBy ID  {0}", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message);
            }
            catch (MissingFieldException exception)
            {
                var message = String.Format("\nInvalid field composition - LastUpdatedBy ID  {0}", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message);
                _notifier.Error(ErrorLang.genericexception());
            }
            catch (CannotUpdateRevenueHeadException exception)
            {
                var message = String.Format("\nUser ID {0} tried to save a revenue head", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message);
                _notifier.Add(NotifyType.Warning, ErrorLang.revenueheadscouldnotbesaved());
            }
            catch(CannotConnectToCashFlowException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Error(ErrorLang.cannotconnettoinvoicingservice());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Error(ErrorLang.genericexception());
                return Redirect("~/Admin");
            }
            #endregion

            revenueHead = _handler.GetRevenueHead(revenueHeadSlug, revenueHeadId);
            var mda = revenueHead.Mda;
            var parent = revenueHead.Revenuehead;
            if (parent != null) { model.ParentSlug = parent.Slug; model.ParentId = parent.Id; }
            model.MDASlug = mda.Slug;
            model.MDAName = mda.NameAndCode();
            model.MDAId = mda.Id;
            var tree = new AdminBreadCrumb { BreadLoaf = _handler.Tree(revenueHead) };
            model.AdminBreadCrumb = GetCrumbShape(tree);
            return View(model);
        }

        public ActionResult List(PagerParameters pagerParameters, RHIndexOptions options, MDARevenueHeadsListPage model)
        {
            try
            {
                var revenueHeads = _handler.GetFirstLevelRevenueHead();
                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                if (options == null)
                    options = new RHIndexOptions();

                var filterName = Enum.GetName(typeof(RevHeadFilter), options.Filter);
                var orderBy = Enum.GetName(typeof(RevHeadOrder), options.Order);

                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * 10 : 0;
                int count = pagerParameters.PageSize.HasValue ? pagerParameters.PageSize.Value : 10;

                var sortedRevenueHeads = _handler.GetFilteredRevenueHeadsCollection(revenueHeads.ToList(), filterName, orderBy, options.Search, options.Direction);

                var pageShape = Shape.Pager(pager).TotalItemCount(sortedRevenueHeads.Count);
                model.RevenueHeads = sortedRevenueHeads.Skip(skip).Take(count == 0 ? sortedRevenueHeads.Count : count).ToList();
                model.Pager = pageShape;
                model.Options = options;
                return View(model);
            }
            #region catch clauses
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Error(ErrorLang.genericexception());
            }
            #endregion
            return Redirect("~/Admin");
        }

        public ActionResult ViewSubRevenueHeads(string slug, int id, RHIndexOptions options, PagerParameters pagerParameters, SubRevenueHeadsListViewModel model)
        {
            RevenueHead revenueHead = new RevenueHead();
            try
            {
                revenueHead = _handler.GetSubRevenueHeadsView(slug, id);

                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                if (options == null)
                    options = new RHIndexOptions();

                var filterName = Enum.GetName(typeof(RevHeadFilter), options.Filter);
                var orderBy = Enum.GetName(typeof(RevHeadOrder), options.Order);

                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * 10 : 0;
                int count = pagerParameters.PageSize.HasValue ? pagerParameters.PageSize.Value : 10;

                var subRevenueHeads = _handler.GetFilteredRevenueHeadsCollection(revenueHead.RevenueHeads.ToList(), filterName, orderBy, options.Search, options.Direction);

                var pageShape = Shape.Pager(pager).TotalItemCount(subRevenueHeads.Count);
                model.ParentRevenueHead = revenueHead;
                model.RevenueHeads = subRevenueHeads.Skip(skip).Take(count == 0 ? subRevenueHeads.Count : count).ToList();
                model.Pager = pageShape;
                model.Options = options;

                var crumbTree = _handler.Tree(revenueHead);
                model.AdminBreadCrumb = new AdminBreadCrumb() { BreadLoaf = crumbTree };
                var crumbShape = Shape.AdminBreadCrumb(model.AdminBreadCrumb);
                model.AdminBreadCrumb = crumbShape;
                return View(model);
            }
            #region catch clauses
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (RevenueHeadCannotAddSubRevenueHeadBecauseSetupHasAlreadyStartedException exception)
            {
                var message = String.Format("\nUser ID {0} tried to add a SRH but has already started the setup process", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message);
                return RedirectToAction("RevenueHeadDashBoard", "RevenueHead", new { revenueHeadSlug = revenueHead.Slug, revenueHeadId = revenueHead.Id });
            }
            catch (CannotFindRevenueHeadException exception)
            {
                var message = String.Format("\nUser ID {0} tried to editing an MDA which could not be found", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message);
                _notifier.Add(NotifyType.Warning, ErrorLang.revenueheadnotfound());
            }
            catch (AlreadyHasBillingException exception)
            {
                Logger.Error(exception, string.Format("This revenue head with slug {0} and Id {1} already has billing info, redirecting to the dashbooard page", slug, id));
                return RedirectToAction("RevenueHeadDashBoard", new { revenueHeadSlug = slug, revenueHeadId = id });
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Error(ErrorLang.genericexception());
            }
            #endregion
            return Redirect("~/Admin");
        }

        [HttpPost, ActionName("ChangeStatus")]
        public ActionResult ChangeStatus(string mdaSlug, int Id, int pageNumber)
        {
            RevenueHead revenueHead = new RevenueHead();
            try
            {
                revenueHead = _handler.ChangeStatus(Id);
            }
            #region catch clauses
            catch (UserNotAuthorizedForThisActionException exception)
            {
                var message = String.Format("\nUser ID {0} tried to view MDA without permission", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (CannotFindRevenueHeadException exception)
            {
                var message = String.Format("\nUser ID {0} tried to editing a rev head which could not be found", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message);
                _notifier.Add(NotifyType.Warning, ErrorLang.revenueheadnotfound());
            }
            catch (CannotUpdateRevenueHeadException exception)
            {
                var message = String.Format("\nCould not update revenue head record for - LastUpdatedBy ID  {0}", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message);
                _notifier.Error(ErrorLang.revenueheadscouldnotbesaved());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Error(ErrorLang.genericexception());
                return Redirect("~/Admin");
            }
            #endregion
            var listPageParams = new PagerParameters();
            listPageParams.Page = pageNumber;

            if (revenueHead.Revenuehead == null) { return RedirectToAction("ViewMDARevenueHeads", "MDA", new { Slug = revenueHead.Mda.Slug, page = pageNumber }); }
            else { return RedirectToAction("ViewSubRevenueHeads", "RevenueHead", new { slug = revenueHead.Revenuehead.Slug, id = revenueHead.Revenuehead.Id, page = pageNumber }); }
        }

        public ActionResult RevenueHeadDashBoard(string revenueHeadSlug, int revenueHeadId)
        {
            try
            {
                var revenueHead = _handler.GetDashBoardView(revenueHeadSlug, revenueHeadId);
                var crumbShape = Shape.AdminBreadCrumb(revenueHead.AdminBreadCrumb);
                revenueHead.AdminBreadCrumb = crumbShape;
                return View(revenueHead);
            }
            #region catch clauses
            catch (UserNotAuthorizedForThisActionException exception)
            {
                var message = String.Format("\nUser ID {0} tried to view MDA without permission", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (CannotFindRevenueHeadException exception)
            {
                var message = String.Format("\nUser ID {0} tried to editing a rev head which could not be found", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, message);
                _notifier.Add(NotifyType.Warning, ErrorLang.revenueheadnotfound());
            }
            catch (HasNoBillingException exception)
            {
                Logger.Error(exception, string.Format("This billing already has no billing info so a dashboard cannot be shown. Revenue head slug {0}, id {1}", revenueHeadSlug, revenueHeadId));
                return RedirectToAction("ViewSubRevenueHeads", new { slug = revenueHeadSlug, id = revenueHeadId });
            }
            #endregion
            return Redirect("~/Admin");
        }

        public ActionResult BillablesList(PagerParameters pagerParameters, RHIndexOptions options, BillablesPageViewModel model)
        {
            try
            {
                var revenueHeads = _handler.GetBillableRevenueHeads();
                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                if (options == null)
                    options = new RHIndexOptions();



                var filterName = Enum.GetName(typeof(RevHeadFilter), options.Filter);
                var orderBy = Enum.GetName(typeof(RevHeadOrder), options.Order);

                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * 10 : 0;
                int count = pagerParameters.PageSize.HasValue ? pagerParameters.PageSize.Value : 10;

                var sortedRevenueHeads = _handler.GetFilteredRevenueHeadsCollection(revenueHeads.ToList(), filterName, orderBy, options.Search != null ? options.Search.ToUpper() : options.Search, options.Direction);

                var trimmedList = sortedRevenueHeads.Skip(skip).Take(count == 0 ? sortedRevenueHeads.Count : count).ToList();

                model.RevenueHeadsAndBillings = trimmedList.Select(r => new RevenueHeadAndBillingInformation()
                {
                    ///BillingInfo = r.Billing,
                    RevennueHeadId = r.Id,
                    RevennueHeadSlug = r.Slug,
                    RevennueHeadName = r.Name,
                    RevennueHeadCode = r.Code,
                    Amount = r.BillingModel.Amount,
                    UpdatedAtUtc = r.UpdatedAtUtc,
                    UpdatedBy = r.LastUpdatedBy.UserName,
                    IsActive = r.IsActive,
                    ///IsPrepaid = r.Billing.IsPrepaid,

                }).ToList<RevenueHeadAndBillingInformation>();

                var pageShape = Shape.Pager(pager).TotalItemCount(revenueHeads.Count());

                model.Pager = pageShape;
                model.Options = options;
                model.TaxEntityCategories = _handler.GetTaxEntityCategories();

                model.PINViewModel = new PINViewModel() { PageNumber = pager.Page };
                return View(model);
            }
            #region catch clauses
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Error(ErrorLang.genericexception());
            }
            #endregion
            return Redirect("~/Admin");
        }


        public ActionResult ViewHierarchy(string revenueHeadSlug, int revenueHeadId)
        {
            try
            {
                HierarchyViewModel mda = _handler.GetHierarchy(revenueHeadSlug, revenueHeadId);
                return View(mda);
            }
            catch (UserNotAuthorizedForThisActionException exception)
            {
                Logger.Error(exception, string.Format("User not authorized to view hierarchy Id: {0} slug: {1}", revenueHeadId, revenueHeadSlug));
                _notifier.Error(ErrorLang.usernotauthorized());
            }
            catch (CannotFindRevenueHeadException exception)
            {
                Logger.Error(exception, string.Format("Revenue head not found Id {0} Slug {1}", revenueHeadId, revenueHeadSlug));
                _notifier.Error(ErrorLang.revenuehead404());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Error(ErrorLang.genericexception());
            }
            return Redirect("~/Admin");
        }

    }
}