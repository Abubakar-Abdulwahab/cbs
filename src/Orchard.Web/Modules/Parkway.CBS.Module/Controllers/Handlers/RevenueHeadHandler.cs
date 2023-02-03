using Parkway.CBS.Module.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using Parkway.CBS.Core.Models;
using Orchard;
using Orchard.Localization;
using Orchard.Security;
using Orchard.ContentManagement;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Validations.Contracts;
using Orchard.Autoroute.Services;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.DataFilters.RevenueHead.Filter;
using Parkway.CBS.Core.DataFilters.RevenueHead.Order;
using Parkway.CBS.Module.ViewModels;
using Parkway.CBS.Core.Services.Contracts;
using System.Web.Mvc;
using Orchard.Mvc.Routes;
using Orchard.Layouts.Framework.Elements;
using Parkway.CBS.Core.Services;
using System.Globalization;
using Parkway.CBS.Module.ViewModels.Charts;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models.Enums;
using Orchard.Modules.Services;
using Parkway.CBS.Core.HTTP.RemoteClient.Contracts;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Module.Controllers.Handlers
{
    public class RevenueHeadHandler : MDARevenueHeadHandler, IRevenueHeadHandler
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IAuthorizer _authorizer;
        private readonly IContentManager _contentManager;
        public Localizer T { get; set; }
        private readonly IMDAHandler _mdaHandler;
        private readonly IValidator _validator;
        private readonly ISlugService _slugService;
        private readonly IEnumerable<IRevenueHeadFilter> _dataFilters;
        private readonly IEnumerable<IRevenueHeadOrder> _dataOrder;
        private readonly IRevenueHeadManager<RevenueHead> _revenueHeadRepository;
        private readonly IRouteProvider _routes;
        private readonly IStatsManager _statsRepository;
        private readonly ITaxEntityCategoryManager<TaxEntityCategory> _taxCategoriesRepository;

        private readonly ICoreMDARevenueHeadService _coreMDARevenueHeadHandler;
        private readonly ICoreRevenueHeadService _coreRevenueHeadHandler;

        public RevenueHeadHandler(IOrchardServices orchardServices, ISlugService slugService, IValidator validator,
            IMDAHandler mdaHandler, IEnumerable<IRevenueHeadFilter> dataFilters, IEnumerable<IRevenueHeadOrder> dataOrder,
            IRevenueHeadManager<RevenueHead> revenueHeadRepository, IRouteProvider routes, IStatsManager statsRepository, ICoreMDARevenueHeadService coreMDARevenueHeadHandler, ICoreRevenueHeadService coreRevenueHeadHandler, IModuleService moduleService, IAdminSettingManager<ExpertSystemSettings> settingsRepository, ITaxEntityCategoryManager<TaxEntityCategory> taxCategoriesRepository) : base(orchardServices, settingsRepository)
        {
            _orchardServices = orchardServices;
            _authorizer = orchardServices.Authorizer;
            _contentManager = orchardServices.ContentManager;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
            _mdaHandler = mdaHandler;
            _validator = validator;
            _slugService = slugService;
            _dataFilters = dataFilters;
            _dataOrder = dataOrder;
            _revenueHeadRepository = revenueHeadRepository;
            _routes = routes;
            _statsRepository = statsRepository;
            _coreMDARevenueHeadHandler = coreMDARevenueHeadHandler;
            _coreRevenueHeadHandler = coreRevenueHeadHandler;
            _taxCategoriesRepository = taxCategoriesRepository;
        }

        
        #region List operations

        public IEnumerable<RevenueHead> GetRevenueHeadsLike(string queryText)
        {
            return _revenueHeadRepository.GetBillableCollection(queryText);
        }

        /// <summary>
        /// Gets view for the list of revenue heads belonging to this MDA
        /// </summary>
        /// <param name="slug">MDA revenueHeadSlug</param>
        /// <returns></returns>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        /// <exception cref="MDARecordNotFoundException"></exception>
        public MDA GetRevenueHeadsView(string slug)
        {
            IsAuthorized<RevenueHeadHandler>(Permissions.CreateRevenueHead);
            return GetMDA(slug);
        }

        public IEnumerable<RevenueHead> GetFirstLevelRevenueHead()
        {
            IsAuthorized<RevenueHeadHandler>(Permissions.ViewRevenueHeadList);
            var revenueHeads = _revenueHeadRepository.GetCollection(r => (r.Revenuehead == null));
            return revenueHeads != null ? revenueHeads : new List<RevenueHead>();
        }

        public IEnumerable<RevenueHead> GetBillableRevenueHeads()
        {
            var revenueHeads = _revenueHeadRepository.GetCollection(r => (r.BillingModel != null && r.IsActive));
            return revenueHeads != null ? revenueHeads : new List<RevenueHead>();
        }

        public List<string> GetTaxEntityCategories()
        {
            var categories = _taxCategoriesRepository.GetCollection(taxcats => taxcats.Status).ToList();
            return categories.Select(taxcat => taxcat.Name).ToList();
        }


        /// <summary>
        /// Get dashboard view
        /// </summary>
        /// <param name="revenueHeadSlug"></param>
        /// <param name="revenueHeadId"></param>
        /// <returns>RevenueHead</returns>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        public RevenueHeadDashboardViewModel GetDashBoardView(string revenueHeadSlug, int revenueHeadId)
        {
            RevenueHead revenueHead = new RevenueHead();
            IsAuthorized<RevenueHeadHandler>(Permissions.ViewRevHeadDashBoard);
            revenueHead = GetRevenueHead(revenueHeadSlug, revenueHeadId);

            //check if the revenue head has any billing info
            if (!HasBilling(revenueHead)) { throw new HasNoBillingException(); }

            RevenueHeadDashboardViewModel model = new RevenueHeadDashboardViewModel();

            List<CategoryDescriptor> tabCats = new List<CategoryDescriptor>();

            var tab1 = new CategoryDescriptor("DashBoard", T("DashBoard"), T("Dashboard"), 0);
            var tab2 = new CategoryDescriptor("Chart", T("Chart"), T("Chart"), 0);

            tabCats.Add(tab1);
            tabCats.Add(tab2);

            model.Categories = tabCats.ToArray();

            #region quarters
            var statsForTheYearPerQuarter = _statsRepository.GetStatsForRevenueHead(revenueHead, null, null);
            if (statsForTheYearPerQuarter == null || statsForTheYearPerQuarter.Count() <= 0) { statsForTheYearPerQuarter = new List<StatsPerMonth>(); }
            var statsForThisMonth = statsForTheYearPerQuarter.Where(stat => (stat.DueDate.Month == DateTime.Now.Month));
            if (statsForThisMonth == null) { statsForThisMonth = new List<StatsPerMonth>(); }

            model.TotalNumberOfInvoices = statsForTheYearPerQuarter.Where(stat => (stat.CreatedAtUtc.Month == DateTime.Now.Month)).Sum(x => x.NumberOfInvoicesSent);
            model.TotalExpectedIncome = statsForThisMonth.Sum(x => x.AmountExpected);
            model.TotalInvoicePaid = statsForThisMonth.Sum(x => x.NumberOfInvoicesPaid);
            model.ActualIncomeOnInvoicesPaid = statsForThisMonth.Sum(x => x.AmountPaid);
            #endregion

            model.RevenueHead = revenueHead;
            model.AdminBreadCrumb = new AdminBreadCrumb() { BreadLoaf = Tree(revenueHead) };
            model.Month = DateTime.Now.ToString("MMMM", CultureInfo.InvariantCulture);
            //
            MainDashboardChartViewModel charts = new MainDashboardChartViewModel();
            BarChartViewModel barchart = new BarChartViewModel();
            charts.BarChart = BuildMultiBarChartView<RevenueHead>(statsForTheYearPerQuarter);
            model.ChartViewModel = charts;
            //Doughtnuts
            charts.DoughNutCharts = BuildDoughNuts<MDA>(statsForTheYearPerQuarter);
            //expectation bar chart
            charts.LineChart = BuildExpectationLineChart<MDA>(statsForTheYearPerQuarter);
            return model;
        }

        private DoughNutChartViewModel BuildDoughNut(IList<StatsPerMonth> statsForTheLast12Months)
        {
            string[] labels = { "Amount Expected", "Amount Received", "Amount Pending" };
            string[] backGroundColors = { "rgba(255,99,132,1)", "rgba(54, 162, 235, 1)", "rgba(255, 206, 86, 1)" };
            decimal[] data = { statsForTheLast12Months.Select(x => x.AmountExpected).Sum(x => x)
                              , statsForTheLast12Months.Select(x => x.AmountPaid).Sum(x => x)
                              , statsForTheLast12Months.Select(x => (x.AmountExpected - x.AmountPaid)).Sum(x => x) };

            return new DoughNutChartViewModel()
            {
                Description = string.Format("Revenue Summary for the past {0} Months", statsForTheLast12Months.Count),
                Labels = labels,
                BackGroundColors = backGroundColors,
                Data = data,
            };
        }

        private BarChartViewModel BuildMultiBarCharView(IList<StatsPerMonth> statsForTheLast12Months)
        {
            string[][] labels = { };
            List<string[]> Labels = new List<string[]>(statsForTheLast12Months.Count);
            for (int i = -11; i < 1; i++)
            {
                DateTime date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(i);
                Labels.Add(new[] { date.ToString("MMMM", CultureInfo.InvariantCulture), date.Date.Year.ToString() });
            }
            labels = Labels.ToArray();
            //string[] backGroundColors = { "rgba(0, 0, 0, 0)", "rgba(54, 162, 235, 0.2)", "rgba(255, 206, 86, 0.2)",
            //                              "rgba(0, 0, 0, 0)", "rgba(54, 162, 235, 0.2)", "rgba(255, 206, 86, 0.2)",
            //                              "rgba(0, 0, 0, 0)", "rgba(54, 162, 235, 0.2)", "rgba(255, 206, 86, 0.2)",
            //                              "rgba(0, 0, 0, 0)", "rgba(54, 162, 235, 0.2)", "rgba(255, 206, 86, 0.2)" };

            string[] backGroundColors = { "rgba(255,99,132,1)" };

            string[] borderColours = { "rgba(255,99,132,1)", "rgba(54, 162, 235, 1)", "rgba(255, 206, 86, 1)",
                                       "rgba(255,99,132,1)", "rgba(54, 162, 235, 1)", "rgba(255, 206, 86, 1)",
                                       "rgba(255,99,132,1)", "rgba(54, 162, 235, 1)", "rgba(255, 206, 86, 1)",
                                       "rgba(255,99,132,1)", "rgba(54, 162, 235, 1)", "rgba(255, 206, 86, 1)" };

            decimal[] expectedAmountData = statsForTheLast12Months.Select(x => x.AmountExpected).ToArray();
            decimal[] paidAmountData = statsForTheLast12Months.Select(x => x.AmountPaid).ToArray();
            decimal[] pendingAmountData = statsForTheLast12Months.Select(x => (x.AmountExpected - x.AmountPaid)).ToArray();

            return new BarChartViewModel()
            {
                Labels = labels,
                BackGroundColors = backGroundColors,
                BorderColors = borderColours,
                ExpectedAmountData = expectedAmountData,
                PaidAmountData = paidAmountData,
                PendingAmountData = pendingAmountData
            };
        }

        /// <summary>
        /// Gets view for the list of revenue heads belonging to this revenue head
        /// </summary>
        /// <param name="slug"></param>
        /// <param name="id"></param>
        /// <returns>RevenueHead</returns>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        /// <exception cref="CannotFindRevenueHeadException"></exception>
        public RevenueHead GetSubRevenueHeadsView(string slug, int id)
        {
            IsAuthorized<RevenueHeadHandler>(Permissions.CreateRevenueHead);
            var revenueHead = GetRevenueHead(slug, id);
            _coreRevenueHeadHandler.HasBilling(revenueHead);
            return revenueHead;
        }

        /// <summary>
        /// Throw AlreadyHasBillingInfoException if the revenue head already has billing info
        /// </summary>
        /// <param name="revenueHead"></param>
        /// <exception cref="AlreadyHasBillingException"></exception>
        private bool HasBilling(RevenueHead revenueHead)
        {
            if(revenueHead.BillingModel != null) { return true; }
            return false;
        }

        /// <summary>
        /// Get the list of revenue heads filtered by the given filter value, ordered and search text if provided
        /// </summary>
        /// <param name="revenueHead"></param>
        /// <param name="filterName"></param>
        /// <param name="orderBy"></param>
        /// <param name="search"></param>
        /// <returns>List{RevenueHead}</returns>
        public List<RevenueHead> GetFilteredRevenueHeadsCollection(List<RevenueHead> list, string filterName = "Enabled", string orderBy = "Name", string searchText = null, bool ascending = true)
        {
            List<RevenueHead> _revenueHeads = new List<RevenueHead>();
            foreach (var item in _dataFilters)
            {
                if (item.FilterName().Equals(filterName))
                {
                    //check if search text is added
                    if (String.IsNullOrEmpty(searchText))
                    {
                        _revenueHeads = list.Where(item.Filter()).ToList();
                    }
                    else
                    {
                        var lambda = item.Filter();
                        _revenueHeads = list.Where(item.Filter(searchText)).ToList();
                    }
                    break;
                }
            }

            foreach (var item in _dataOrder)
            {
                if (item.OrderName().Contains(orderBy))
                {
                    _revenueHeads = item.Order(_revenueHeads, ascending);
                    break;
                }
            }
            return _revenueHeads;
        }

        #endregion

        #region Create operations

        /// <summary>
        /// Handles the request for create a revenue head view for this MDA
        /// </summary>
        /// <param name="slug">this is the revenueHeadSlug value of the mda you are creating a revenue head for</param>
        /// <returns>RevenueHeadCreateView</returns>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        /// <exception cref="MDARecordNotFoundException"></exception>
        public RevenueHeadCreateFromMDAView CreateRevenueHeadView(string slug)
        {
            IsAuthorized<RevenueHeadHandler>(Permissions.CreateRevenueHead);
            List<SelectListItem> mdas = new List<SelectListItem>();
            MDA mda = new MDA();
            if (string.IsNullOrEmpty(slug))
            {
                mdas = GetMDAList();
            }
            else
            {
                mda = GetMDA(slug);
                mdas.Add(new SelectListItem() { Text = mda.Name, Value = mda.Slug, Selected = true });
            }
            return new RevenueHeadCreateFromMDAView()
            {
                MDAName = mda.Name,
                RevenueHeadsCollection = new List<RevenueHead>(),
                Mdas = mdas,
                CodePrefix = mda.Code,
                MDASlug = mda.Slug,
                AdminBreadCrumb = new AdminBreadCrumb() { BreadLoaf = Tree(mda) }
            };
        }

        /// <summary>
        /// Tries to save the revenue head to the mda with the specified revenueHeadSlug
        /// </summary>
        /// <param name="revenueHeadController"></param>
        /// <param name="model"></param>
        /// <param name="slug"></param>
        /// <returns>RevenueHeadController</returns>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        /// <exception cref="MDARecordNotFoundException">MDARecordNotFoundException</exception>
        /// <exception cref="DirtyFormDataException"></exception>
        /// <exception cref="CannotSaveRevenueHeadException"></exception>
        /// <exception cref="MissingFieldException"></exception>
        public void TryCreateRevenueHead(RevenueHeadController callback, ICollection<RevenueHead> revenueHeads, string slug)
        {
            IsAuthorized<RevenueHeadHandler>(Permissions.CreateMDA).IsModelValid<RevenueHeadHandler, RevenueHeadController>(callback);
            UserPartRecord user = GetUser(_orchardServices.WorkContext.CurrentUser.Id);
            List<ErrorModel> errors = new List<ErrorModel>();
            try
            {
                _coreRevenueHeadHandler.TryCreateRevenueHead(user, ref errors, new CreateRevenueHeadsModel { IsSubRevenueHead = false, ParentMDASlug = slug, RevenueHeads = revenueHeads.Reverse().ToList() });
            }
            catch (DirtyFormDataException) { AddValidationErrorsToCallback<RevenueHeadHandler, RevenueHeadController>(callback, errors); }
            
            Logger.Information(string.Format("Revenue Heads saved successfully from mda slug", slug));
        }


        public void TryCreateRevenueHeadFromRevenueHead(RevenueHeadController callback, ICollection<RevenueHead> revenueHeads, string revenueHeadSlug, int revenueHeadId)
        {
            //parent revenue head
            IsAuthorized<RevenueHeadHandler>(Permissions.CreateRevenueHead).IsModelValid<RevenueHeadHandler, RevenueHeadController>(callback);
            UserPartRecord user = GetUser(_orchardServices.WorkContext.CurrentUser.Id);
            List<ErrorModel> errors = new List<ErrorModel>();
            try
            {
                _coreRevenueHeadHandler.TryCreateRevenueHead(user, ref errors, new CreateRevenueHeadsModel { IsSubRevenueHead = true, ParentRevenueHeadId = revenueHeadId, RevenueHeads = revenueHeads.Reverse().ToList() });
            }
            catch (DirtyFormDataException) { AddValidationErrorsToCallback<RevenueHeadHandler, RevenueHeadController>(callback, errors); }
            
            Logger.Information(string.Format("Revenue Heads saved successfully from revenue head {0} {1}", revenueHeadSlug, revenueHeadId));
        }

        public SubRevenueHeadCreateViewModel CreateRevenueHeadViewFromRevenueHead(string slug, int id)
        {
            IsAuthorized<RevenueHeadHandler>(Permissions.CreateRevenueHead);
            var revenueHead = GetRevenueHead(slug, id);

            _coreRevenueHeadHandler.HasBilling(revenueHead);

            return new SubRevenueHeadCreateViewModel()
            {
                SRHName = revenueHead.NameAndCode(),
                CodePrefix = revenueHead.Code,
                HasBilling = revenueHead.BillingModel != null,
                RevenueHeadsCollection = new List<RevenueHead>() { },
                SRHSlug = revenueHead.Slug,
                SRHId = revenueHead.Id,
                AdminBreadCrumb = new AdminBreadCrumb() { BreadLoaf = Tree(revenueHead) }
            };
        }


        #endregion

        #region Edit Operations


        /// <summary>
        /// Gets the view for revenue head edit
        /// </summary>
        /// <param name="revenueHeadSlug">Slug of the revenue head to be edited</param>
        /// <param name="revenueHeadId">TaxEntityId of the revenue head to be edited</param>
        /// <returns>RevenueHeadCreateView</returns>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        /// <exception cref="CannotFindRevenueHeadException"></exception>
        public RevenueHeadCreateView GetEditRevenueHeadView(string revenueHeadSlug, int revenueHeadId)
        {
            IsAuthorized<RevenueHeadHandler>(Permissions.CreateRevenueHead);
            var revenueHead = GetRevenueHead(revenueHeadSlug, revenueHeadId);
            var mda = revenueHead.Mda;
            if(mda == null) { throw new MDARecordNotFoundException(); }
            var parentRevenueHead = revenueHead.Revenuehead;

            return new RevenueHeadCreateView()
            {
                MDAName = mda.NameAndCode(),
                RevenueHead = revenueHead,
                MDASlug = mda.Slug,
                AdminBreadCrumb = new AdminBreadCrumb() { BreadLoaf = Tree(revenueHead) },
                MDAId = mda.Id,
                ParentId = parentRevenueHead != null ? parentRevenueHead.Id : 0,
                ParentSlug = parentRevenueHead != null ? parentRevenueHead.Slug : null
            };
        }


        /// <summary>
        /// Save edited revenue head record
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="updatedRevenueHead">Unsaved revenue head record</param>
        /// <param name="revenueHeadSlug">Slug of the to be updated revenue head</param>
        /// <param name="revenueHeadId">ID of the revenue head</param>
        /// <returns>RevenueHead</returns>
        /// /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        /// <exception cref="CannotFindRevenueHeadException"></exception>
        /// <exception cref="DirtyFormDataException"></exception>
        /// <exception cref="CannotUpdateRevenueHeadException"></exception>
        public RevenueHead TryUpdateRevenueHead(RevenueHeadController callback, RevenueHead updatedRevenueHead, string revenueHeadSlug, int revenueHeadId)
        {
            IsAuthorized<RevenueHeadHandler>(Permissions.CreateRevenueHead);
            RevenueHead revenueHead = GetRevenueHead(revenueHeadSlug, revenueHeadId);
            UserPartRecord user = GetUser(_orchardServices.WorkContext.CurrentUser.Id);
            List<ErrorModel> errors = new List<ErrorModel>();
            try
            {
                _coreRevenueHeadHandler.TryUpdateRevenueHead(revenueHead, updatedRevenueHead, user, ref errors);
                
            }
            catch (DirtyFormDataException) { AddValidationErrorsToCallback<RevenueHeadHandler, RevenueHeadController>(callback, errors); }
            return revenueHead;
        }


        /// <summary>
        /// Change the isDeleted status of the revenue head with the given revenueHeadId
        /// </summary>
        /// <param name="id">TaxEntityId of the revenue head</param>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        /// <exception cref="CannotFindRevenueHeadException"></exception>
        /// <exception cref="CannotUpdateRevenueHeadException"></exception>
        public RevenueHead ChangeStatus(int id)
        {
            IsAuthorized<RevenueHeadHandler>(Permissions.CreateMDA);
            var revenueHead = GetRevenueHead(id);
            //lets get the new status value
            var status = !revenueHead.IsActive;
            revenueHead.IsActive = status;

            if (status)
            {
                //revenueHead.IsVisible = status;
                //if the revenue head is going from an INACTIVE status TO an ACTIVE status
                TurnVisiblityOnFromAnInactiveState(revenueHead);
            }
            else
            {
                revenueHead.IsVisible = status;
                TurnVisiblityOffFromAnActiveState(revenueHead);
                //TurnOffParentVisibilty(revenueHead);
            }

            revenueHead.LastUpdatedBy = _revenueHeadRepository.User(_orchardServices.WorkContext.CurrentUser.Id);
            if (!_revenueHeadRepository.Update(revenueHead)) { throw new CannotUpdateRevenueHeadException(); }
            return revenueHead;
        }

        #endregion

        public SortedDictionary<string, string> Tree(RevenueHead revenueHead)
        {
            Dictionary<string, string> loaf = new Dictionary<string, string>();
            loaf = GetOtherBreadSlices(revenueHead, loaf, 4);
            loaf.Add(string.Format("MDA:{0}:{1}", revenueHead.Mda.Id, revenueHead.Mda.Slug), revenueHead.Mda.Name);

            SortedDictionary<string, string> slicedLoaf = new SortedDictionary<string, string>(loaf, StringComparer.CurrentCultureIgnoreCase);
            return slicedLoaf;
        }
        

        private Dictionary<string, string> GetOtherBreadSlices(RevenueHead revenueHead, Dictionary<string, string> loaf, int level = 1)
        {
            level--;
            if (revenueHead.Revenuehead == null)
            {
                loaf.Add(string.Format("RH:{0}:{1}", revenueHead.Id, revenueHead.Slug), revenueHead.Name);
            }
            else if (level == 0)
            {
                loaf.Add(string.Format("RH:{0}:{1}", revenueHead.Id, revenueHead.Slug), revenueHead.Name);
            }
            else
            {
                loaf.Add(string.Format("RH:{0}:{1}", revenueHead.Id, revenueHead.Slug), revenueHead.Name);
                GetOtherBreadSlices(revenueHead.Revenuehead, loaf, level);
            }
            return loaf;
        }

        private SortedDictionary<string, string> Tree(MDA mda)
        {
            SortedDictionary<string, string> mdaSlice = new SortedDictionary<string, string>();
            mdaSlice.Add(string.Format("MDA:{0}:{1}", mda.Id, mda.Slug), mda.Name);
            return mdaSlice;
        }

        /// <summary>
        /// Turn the visiblity of the parent revenue head to true, and then finally turn the MDA visiblity to true
        /// </summary>
        /// <param name="revenueHead"></param>
        public void TurnOnParentVisibilty(RevenueHead revenueHead)
        {
            if (revenueHead.Revenuehead == null) { revenueHead.Mda.IsVisible = true; return; }
            //If the parent revenue head is inactive, return.
            if (!revenueHead.Revenuehead.IsActive) { return; }
            // If the parent revenue head is active, proceed to set visibility to true
            revenueHead.Revenuehead.IsVisible = true;
            //perform this same operation up the hierarchy
            TurnOnParentVisibilty(revenueHead.Revenuehead);
        }

        /// <summary>
        /// Turn the visiblity of a revenue head on when it has been set to an active state.
        /// </summary>
        /// <param name="revenueHead"></param>
        /// <remarks>Yes this method could have be shorten, but it has been delibrately left naive</remarks>
        public void TurnVisiblityOnFromAnInactiveState(RevenueHead revenueHead)
        {
            //now that you are enabled, lets check if you have any children if you dont have children
            if (!revenueHead.RevenueHeads.Any())
            {
                //we check if you are billable
                if (revenueHead.BillingModel != null)
                {
                    revenueHead.IsVisible = true;
                    revenueHead.Mda.IsVisible = true;
                }
                return;
            }

            //if you have children we need to check if any of the children is active and visibility is set to true
            var childNodes = revenueHead.RevenueHeads;
            if (childNodes.Where(children => (children.IsActive == true) && (children.IsVisible == true)).Count() > 0)
            {
                //this means that you have child nodes that are billable
                TurnOnParentVisibilty(revenueHead);
                revenueHead.IsVisible = true;
            }
            else
            {
                //if you don't have any child revenue head that is both active and visible
                //there is nothing for you to do here anymore
                return;
            }
        }

        /// <summary>
        /// Turn the visiblity of a revenue head off, we would want to bubble up the hierarchy turning off the visible of any ancestor, iff
        /// they dont have children that are active and visible.
        /// </summary>
        /// <param name="revenueHead"></param>
        public void TurnVisiblityOffFromAnActiveState(RevenueHead revenueHead)
        {
            //here we are only concerned if you have been active and visible
            if (revenueHead.Revenuehead == null)
            {
                //this means you are a root revenue head
                //lets find out if you have any siblings that are active and visible
                if (revenueHead.Mda.RevenueHeads.Where(r => (r.Revenuehead == null) && (r.IsActive == true) && (r.IsVisible == true)).Count() > 0)
                {
                    //this means that there are still revenue heads on your level that still depend on the MDA visibilty being true
                    return;
                }
                else
                {
                    //this means that there are no revenue heads on your level that depend on the visibilty of the MDA
                    revenueHead.Mda.IsVisible = false;
                    return;
                }
            }
            //now lets check if your parent has any children (that is if you have any siblings) is active and visible or 
            //is active and has is billable
            var sibs = revenueHead.Revenuehead.RevenueHeads;

            if (sibs.Where(r => (r.IsActive == true) && (r.IsVisible == true)).Count() > 0)
            {
                //this means that one of your siblings has the need for your parent to remain visible
                return;
            }
            else
            {
                revenueHead.Revenuehead.IsVisible = false;
                TurnVisiblityOffFromAnActiveState(revenueHead.Revenuehead);
            }
        }


        public List<SelectListItem> GetMDAList()
        {
            List<SelectListItem> mdas = new List<SelectListItem>();
            mdas = _mdaHandler.GetCollection("All", "Name").Select(m => new SelectListItem() { Text = m.NameAndCode(), Value = m.Slug }).ToList();
            mdas.Insert(0, new SelectListItem() { Text = "Select MDA", Value = null, Selected = true });
            return mdas;
        }


        /// <summary>
        /// Get revenue head by Id
        /// </summary>
        /// <param name="id">Id of the revenue head</param>
        /// <returns>RevenueHead</returns>
        /// <exception cref="CannotFindRevenueHeadException"></exception>
        public RevenueHead GetRevenueHead(int id)
        {
            var revenueHead = _revenueHeadRepository.Get(id);
            if (revenueHead == null) { throw new CannotFindRevenueHeadException(); }
            return revenueHead;
        }


        /// <summary>
        /// Get revenue head with the revenueHeadSlug and revenueHeadId
        /// </summary>
        /// <param name="slug">parent revenueHeadSlug</param>
        /// <param name="id">parent revenueHeadId</param>
        /// <returns>RevenueHead</returns>
        /// <exception cref="CannotFindRevenueHeadException"></exception>
        public RevenueHead GetRevenueHead(string slug, int id)
        {
            var revenueHead = _revenueHeadRepository.Get(id);
            if (revenueHead == null || !revenueHead.Slug.Equals(slug)) { throw new CannotFindRevenueHeadException(); }
            return revenueHead;
        }

        /// <summary>
        /// Get MDA with revenueHeadSlug
        /// </summary>
        /// <param name="slug">Slug of the MDA</param>
        /// <returns>MDA</returns>
        /// <exception cref="MDARecordNotFoundException"></exception>
        private MDA GetMDA(string slug)
        {
            var mda = _mdaHandler.GetMDA(slug);
            return mda;
        }        


        
        public HierarchyViewModel GetHierarchy(string revenueHeadSlug, int revenueHeadId)
        {
            RevenueHead revenueHead = GetRevenueHead(revenueHeadSlug, revenueHeadId);
            IsAuthorized<RevenueHeadHandler>(Permissions.ViewRevHeadDashBoard);
            var revenueHeads = revenueHead.RevenueHeads;
            return new HierarchyViewModel() { Name = revenueHead.NameAndCode(), RevenueHeads = revenueHeads != null ? revenueHeads : new List<RevenueHead>() };
        }



        /// <summary>
        /// Get the list of revenue heads that belong to the givem MDA Id, can be accessed by the user Id and for the given access type
        /// </summary>
        /// <param name="adminUserId"></param>
        /// <param name="mdaId"></param>
        /// <param name="accessType"></param>
        /// <returns>IEnumerable{RevenueHeadDropDownListViewModel}</returns>
        public IEnumerable<RevenueHeadDropDownListViewModel> GetRevenueHeadsOnAccessList(int adminUserId, int mdaId, AccessType accessType, bool applyAccessRestrictions)
        {
            return _revenueHeadRepository.GetRevenueHeadsOnAccessListForMDA(adminUserId, mdaId, accessType, applyAccessRestrictions);
        }

    }
}