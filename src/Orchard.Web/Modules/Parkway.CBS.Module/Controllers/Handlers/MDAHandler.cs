using Parkway.CBS.Module.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Parkway.CBS.Core.Models;
using Orchard;
using Orchard.Localization;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.DataFilters.MDA.Filter;
using Parkway.CBS.Core.DataFilters.MDA.Order;
using Parkway.CBS.Module.ViewModels;
using Parkway.CBS.Core.DataFilters.RevenueHead.Filter;
using Parkway.CBS.Core.DataFilters.RevenueHead.Order;
using Orchard.Layouts.Framework.Elements;
using Parkway.CBS.Module.ViewModels.Charts;
using Parkway.CBS.Core.Services;
using System.Globalization;
using Orchard.Roles.Services;
using Parkway.Cashflow.Ng.Models;
using Orchard.Logging;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.HelperModels;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.DataFilters.CollectionReport;
using Parkway.CBS.Core.DataFilters.AssessmentReport;

namespace Parkway.CBS.Module.Controllers.Handlers
{
    public class MDAHandler : MDARevenueHeadHandler, IMDAHandler
    {
        private readonly IOrchardServices _orchardServices;
        public Localizer T { get; set; }
        private readonly IMDAManager<MDA> _mdaRepository;
        private readonly IEnumerable<IMDAFilter> _dataFilters;
        private readonly IEnumerable<IMDAOrder> _dataOrder;
        private readonly IRevenueHeadManager<RevenueHead> _revenueHeadRepository;

        private readonly ICollectionReportFilter _collectionFilter;
        private readonly IInvoiceAssessmentsReportFilter _assessmentFilter;


        private readonly IEnumerable<IRevenueHeadFilter> _dataRevenueHeadFilters;
        private readonly IEnumerable<IRevenueHeadOrder> _dataRevenueHeadOrder;

        private readonly IRoleService _roleService;

        public IInvoicingService _invoicingService;

        public readonly ICoreMDAService _coreHandler;
        private readonly ICoreMDARevenueHeadService _coreMDARevenueHeadHandler;
        private readonly IRoleUserManager<AccessRoleUser> _accessRoleUserRepo;

        private readonly IStatsManager _statsRepository;

        public MDAHandler(IOrchardServices orchardServices,
                          IMDAManager<MDA> mdaRepository, IEnumerable<IMDAFilter> dataFilters,
                          IEnumerable<IMDAOrder> dataOrder, IRevenueHeadManager<RevenueHead> revenueHeadRepository,
                          IEnumerable<IRevenueHeadFilter> dataRevenueHeadFilters, IEnumerable<IRevenueHeadOrder> dataRevenueHeadOrder, IStatsManager statsRepository, IRoleService roleService, IInvoicingService invoicingService, IAdminSettingManager<ExpertSystemSettings> settingsRepository, ICoreMDAService coreHandler, ICoreMDARevenueHeadService coreMDARevenueHeadHandler, IRoleUserManager<AccessRoleUser> accessRoleUserRepo, ICollectionReportFilter collectionFilter, IInvoiceAssessmentsReportFilter assessmentFilter)
            : base(orchardServices, settingsRepository)
        {
            _orchardServices = orchardServices;
            T = NullLocalizer.Instance;
            _mdaRepository = mdaRepository;
            _dataFilters = dataFilters;
            _dataOrder = dataOrder;
            _revenueHeadRepository = revenueHeadRepository;
            _dataRevenueHeadFilters = dataRevenueHeadFilters;
            _dataRevenueHeadOrder = dataRevenueHeadOrder;
            _statsRepository = statsRepository;
            _roleService = roleService;
            _invoicingService = invoicingService;
            Logger = NullLogger.Instance;
            _coreHandler = coreHandler;
            _coreMDARevenueHeadHandler = coreMDARevenueHeadHandler;
            _accessRoleUserRepo = accessRoleUserRepo;
            _collectionFilter = collectionFilter;
            _assessmentFilter = assessmentFilter;
        }

        public DashboardViewModel GetDashboardView(DateTime startDate, DateTime endDate, string mdaSelected)
        {
            List<CategoryDescriptor> tabCats = new List<CategoryDescriptor>();

            var tab1 = new CategoryDescriptor("DashBoard", T("DashBoard"), T("Dashboard"), 0);
            var tab2 = new CategoryDescriptor("Chart", T("Chart"), T("Chart"), 0);

            tabCats.Add(tab1);
            tabCats.Add(tab2);

            DashboardViewModel model = BuildDashboard();

            model.Categories = tabCats.ToArray();

            #region quarters

            var statsForTheYearPerQuarter = _statsRepository.GetStatsPerQuarter(null, null);
            if (statsForTheYearPerQuarter == null || statsForTheYearPerQuarter.Count() <= 0) { statsForTheYearPerQuarter = new List<StatsPerMonth>(); }

            var statsForThisMonth = statsForTheYearPerQuarter.Where(stat => (stat.DueDate.Month == DateTime.Now.Month));

            if (statsForThisMonth == null) { statsForThisMonth = new List<StatsPerMonth>(); }

            model.Tenant = _orchardServices.WorkContext.CurrentSite.SiteName;
            model.Month = DateTime.Now.ToString("MMMM", CultureInfo.InvariantCulture);

            MainDashboardChartViewModel charts = new MainDashboardChartViewModel();
            BarChartViewModel barchart = new BarChartViewModel();

            //barchart
            charts.BarChart = BuildMultiBarChartView<MDA>(statsForTheYearPerQuarter);
            model.ChartViewModel = charts;
            //Doughtnuts
            charts.DoughNutCharts = BuildDoughNuts<MDA>(statsForTheYearPerQuarter);
            //expectation bar chart

            charts.LineChart = BuildExpectationLineChart<MDA>(statsForTheYearPerQuarter);

            model.ListOfMdas = _mdaRepository.GetCollection(m => m.Id != 0).Select(m => new MDADropDownListViewModel() { Id = m.Id, Code = m.Code, Name = m.Name, Slug = m.Slug });

            IList<StatsPerMonth> statsForPieChart = new List<StatsPerMonth>();
            //mdaSelected = "ministry-of-lands-mda-land-d";
            MDA mda = new MDA();
            model.MDASelected = mdaSelected == null ? "All" : mdaSelected;

            #region pie charts
            model.ChartViewModel.PieChart = new List<PieChartsViewModel>();
            //if (mdaSelected == null || mdaSelected == "All") { model.ChartViewModel.PieChart = BuildPieCharts(statsForPieChart, mda); }
            //else { model.ChartViewModel.PieChart = BuildPieCharts(statsForPieChart); }
            #endregion

            if (mdaSelected == null || mdaSelected == "All")
            {
                statsForPieChart = _statsRepository.GetStatsForPieChart(startDate, endDate.AddMonths(1).AddMilliseconds(-1), "All");
                model.ChartViewModel.PieChart = BuildPieCharts(statsForPieChart);
            }
            else
            {
                mda = _mdaRepository.Get("Slug", mdaSelected);
                mdaSelected = null;
                if (mda == null) { mdaSelected = "All"; }
                statsForPieChart = _statsRepository.GetStatsForPieChart(startDate, endDate, mdaSelected, mda);
                model.ChartViewModel.PieChart = BuildPieCharts(statsForPieChart, mda);
            }
            #endregion

            return model;
        }


        /// <summary>
        /// Build dashboard view for stats
        /// <para>
        /// Returns invoices sent in month
        /// Expected income: this is the income that has not been paid up until the present month
        /// Income due: this is the income due this month
        /// Invoices paid: is the count of invoices that hsve been psid for this month, CreatedAtUTC month is the present month
        /// Income received:is the income received for CreatedAtUTC present month
        /// </para>
        /// </summary>
        /// <returns>DashboardViewModel</returns>
        private DashboardViewModel BuildDashboard()
        {
            DashboardViewModel model = new DashboardViewModel() { Tenant = _orchardServices.WorkContext.CurrentSite.SiteName };
            DateTime endDate = new DateTime(DateTime.Now.ToLocalTime().Year, DateTime.Now.ToLocalTime().Month, DateTime.DaysInMonth(DateTime.Now.ToLocalTime().Year, DateTime.Now.ToLocalTime().Month)).AddDays(1).AddSeconds(-1);
            DateTime startDate = new DateTime(DateTime.Now.ToLocalTime().Year, DateTime.Now.ToLocalTime().Month, 1);
            DateTime yearStart = new DateTime(DateTime.Now.Year, 1, 1);

            DashboardStatsSearchParams searchParams = new DashboardStatsSearchParams
            {
                AdminUserId = _orchardServices.WorkContext.CurrentUser.Id,
                StartDate = new DateTime(DateTime.Now.ToLocalTime().Year, DateTime.Now.ToLocalTime().Month, 1),
                EndDate = new DateTime(DateTime.Now.ToLocalTime().Year, DateTime.Now.ToLocalTime().Month, DateTime.DaysInMonth(DateTime.Now.ToLocalTime().Year, DateTime.Now.ToLocalTime().Month)).AddDays(1).AddSeconds(-1),
            };

            //we check if this user has been constrained to a role
            bool applyAccessRestrictions = _accessRoleUserRepo.UserHasAcessTypeRole(searchParams.AdminUserId, AccessType.CollectionReport);
            bool applyAccessRestrictionsForInvoiceAssessment = _accessRoleUserRepo.UserHasAcessTypeRole(searchParams.AdminUserId, AccessType.InvoiceAssessmentReport);

            //get number of invoices            
            InvoiceAssessmentSearchParams invoiceParams = new InvoiceAssessmentSearchParams
            {
                AdminUserId = _orchardServices.WorkContext.CurrentUser.Id,
                StartDate = startDate,
                EndDate = endDate,
                Options = new PaymentOptions { PaymentStatus = InvoiceStatus.All }
            };
            var numberOfInvoicesGeneratedForRange = _assessmentFilter.GetAggregate(invoiceParams, applyAccessRestrictionsForInvoiceAssessment);
            // end number of invoices CARD 1

            //expected income, this is income on invoices that have not been paid for and their due date is equal or less than the 
            //given end date range
            //var expectedIncome = _statsRepository.GetExpectedIncomeOnInvoices(searchParams, applyAccessRestrictions, yearStart, searchParams.EndDate);

            invoiceParams.StartDate = yearStart;
            invoiceParams.DateFilterBy = FilterDate.DueDate;
            var expectedIncome = _assessmentFilter.GetAggregate(invoiceParams, applyAccessRestrictionsForInvoiceAssessment);
            //end expected income, CARD 2

            //get expected income this due between this date range
            //var expectedIncomeDue = _statsRepository.GetExpectedIncomeOnInvoices(searchParams, applyAccessRestrictions, searchParams.StartDate, searchParams.EndDate);

            invoiceParams.StartDate = startDate;
            var expectedIncomeDue = _assessmentFilter.GetAggregate(invoiceParams, applyAccessRestrictionsForInvoiceAssessment);
            //end expect income due this month. CARD 3

            //total income received within this date range.
            //var incomeReceivedAndCount = _statsRepository.GetIncomeReceivedAndCount(searchParams, applyAccessRestrictions, searchParams.StartDate, searchParams.EndDate);
            CollectionSearchParams collectionParams = new CollectionSearchParams
            {
                FromRange = startDate,
                EndRange = endDate,
                AdminUserId = _orchardServices.WorkContext.CurrentUser.Id,
                ///
                SelectedPaymentProvider = ((int)PaymentProvider.None).ToString(),
                SelectedPaymentChannel = PaymentChannel.None,
                PaymentDirection = CollectionPaymentDirection.PaymentDate,
                DontPageData = true
            };
            var incomeReceivedAndCount = _collectionFilter.GetAggregate(collectionParams, applyAccessRestrictions);
            //END OF CARD 4 AND 5

            var numberOfMDAs = _mdaRepository.GetAccessList(searchParams.AdminUserId, AccessType.CollectionReport, applyAccessRestrictions);
            var numberOfRHs = _revenueHeadRepository.GetRevenueHeadsOnAccessListForMDA(searchParams.AdminUserId, applyAccessRestrictions);

            //invoice sent this month  ₦ 117,441,642.06 
            model.TotalNumberOfInvoices = numberOfInvoicesGeneratedForRange.FirstOrDefault() == null ? 0 : numberOfInvoicesGeneratedForRange.First().RecordCount;
            //expected income, from year start to now
            model.TotalExpectedIncome = expectedIncome.FirstOrDefault() == null ? 0.00m : expectedIncome.First().TotalAmountOfPayment;
            //income due this month
            model.TotalIncomeDue = expectedIncomeDue.FirstOrDefault() == null ? 0.00m : expectedIncomeDue.First().TotalAmountOfPayment;
            //total invoice paid this month
            model.TotalInvoicePaid = incomeReceivedAndCount.FirstOrDefault() == null ? 0 : incomeReceivedAndCount.First().RecordCount;
            //income received this month
            model.ActualIncomeOnInvoicesPaid = incomeReceivedAndCount.FirstOrDefault() == null ? 0.00m : incomeReceivedAndCount.First().TotalAmountOfPayment;
            //
            model.NumberOfMDAs = numberOfMDAs.Count();
            model.NumberOfRevenueHeads = numberOfRHs.Count();

            return model;
        }

        #region Create Operations


        /// <summary>
        /// Get view to create MDA (company/SME) on cash flow
        /// </summary>
        /// <returns>MDASettingsViewModel</returns>
        /// <exception cref="TenantNotFoundException"></exception>
        public MDASettingsViewModel GetMDASettingsView()
        {
            IsAuthorized<MDAHandler>(Permissions.CreateMDA);
            try
            {
                #region CASHFLOW 
                var context = _invoicingService.StartInvoicingService(new Dictionary<string, dynamic> { { "companyKeyCode", "" } });
                var bankService = _invoicingService.BankService(context);
                var listOfBanks = bankService.ListOfBanks();
                #endregion

                ICollection<ExpertSystemSettings> collectionOfExpertSystems = GetExpertSystems();
                if (collectionOfExpertSystems == null || collectionOfExpertSystems.Count() <= 0) { throw new TenantNotFoundException("No expert systems found"); }
                return new MDASettingsViewModel() { UseTSA = false, Banks = listOfBanks, MDA = new MDA() { BankDetails = new BankDetails(), MDASettings = new MDASettings() { } } };
            }
            catch (TenantNotFoundException) { throw new TenantNotFoundException(); }
            catch (Exception) { throw new CannotConnectToCashFlowException(); }
        }


        /// <summary>
        /// Try persist mda and mda settings
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="mda"></param>
        /// <param name="bankCode"></param>
        /// <param name="files"></param>
        /// <param name="useTSA"></param>
        public void CreateMDASettingsView(MDAController callback, MDA mda, string sbankId, HttpFileCollectionBase files, bool useTSA = false)
        {
            IsAuthorized<MDAHandler>(Permissions.CreateMDA).IsModelValid<MDAHandler, MDAController>(callback);
            //check if tenant settings are set (tots)
            ExpertSystemSettings expertSystem = GetExpertSystem();
            if (expertSystem == null) { throw new TenantNotFoundException("No tenant found while creating MDA"); }

            if (useTSA && !string.IsNullOrEmpty(expertSystem.TSABankNumber))
            {
                mda.BankDetails = new BankDetails() { BankId = expertSystem.TSA, BankAccountNumber = expertSystem.TSABankNumber, BankCode = expertSystem.BankCode };
                mda.UsesTSA = true;
            }
            else
            {
                var bankId = 0;
                if (!Int32.TryParse(sbankId, out bankId)) { throw new CouldNotParseStringValueException(string.Format("Could not parse bank value bank id - {0}", sbankId)); }
                mda.BankDetails.BankId = bankId;
                mda.BankDetails.BankCode = GetBankCode(bankId);
            }
            UserPartRecord user = GetUser(_orchardServices.WorkContext.CurrentUser.Id);
            List<ErrorModel> errors = new List<ErrorModel>();

            try
            {
                _coreHandler.TrySaveMDA(expertSystem, mda, user, ref errors, files);
            }
            catch (DirtyFormDataException)
            {
                AddValidationErrorsToCallback<MDAHandler, MDAController>(callback, errors);
            }
        }


        /// <summary>
        /// Get bank Id from bank name
        /// </summary>
        /// <param name="bankName"></param>
        /// <returns>int</returns>
        private string GetBankCode(int bankId)
        {
            try
            {
                #region CASHFLOW 
                var context = _invoicingService.StartInvoicingService(new Dictionary<string, dynamic> { { "companyKeyCode", "" } });
                var bankService = _invoicingService.BankService(context);
                var listOfBanks = bankService.ListOfBanks();
                return listOfBanks.Where(b => b.Id == bankId).Single().Code;
                #endregion
            }
            catch (Exception) { throw new NoBankDetailsOnCashflowFoundException(); }
        }


        private MDAHandler CreateRole(ICollection<MDA> mdas)
        {
            foreach (var mda in mdas)
            {
                string roleIdentifier = string.Format("MDA_{0}_{1}_{2}_ROLE", mda.Name, mda.Code, Guid.NewGuid().ToString("N"));
                var role = _roleService.GetRoleByName(mda.Name);
            }
            throw new NotImplementedException();
        }

        #endregion

        #region List operations

        /// <summary>
        /// Gets view for the list of revenue heads belonging to this MDA
        /// </summary>
        /// <param name="slug">MDA slug</param>
        /// <returns></returns>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        /// <exception cref="MDARecordNotFoundException"></exception>
        public MDA GetRevenueHeadsView(string slug)
        {
            IsAuthorized<MDAHandler>(Permissions.CreateRevenueHead);
            return GetMDA(slug);
        }

        /// <summary>
        /// Get the list of revenue heads for the given MDA based on the given filter values and search param if provided
        /// </summary>
        /// <param name="mda"></param>
        /// <param name="filterName"></param>
        /// <param name="orderBy"></param>
        /// <param name="searchText"></param>
        /// <returns>List{RevenueHead} </returns>
        public List<RevenueHead> GetRevenueHeadsCollection(List<RevenueHead> revenueHeads, string filterName = "Enabled", string orderBy = "Name", string searchText = null, bool ascending = true)
        {
            List<RevenueHead> _revenueHeads = new List<RevenueHead>();
            foreach (var item in _dataRevenueHeadFilters)
            {
                if (item.FilterName().Equals(filterName))
                {
                    //check if search text is added
                    if (String.IsNullOrEmpty(searchText))
                    {
                        _revenueHeads = revenueHeads.Where(item.Filter()).ToList();
                    }
                    else
                    {
                        var lambda = item.Filter();
                        _revenueHeads = revenueHeads.Where(item.Filter(searchText)).ToList();
                    }
                    break;
                }
            }

            foreach (var item in _dataRevenueHeadOrder)
            {
                if (item.OrderName().Contains(orderBy))
                {
                    _revenueHeads = item.Order(_revenueHeads, ascending);
                    break;
                }
            }
            return _revenueHeads;
        }

        /// <summary>
        /// Get hierarchy view
        /// </summary>
        /// <param name="slug"></param>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        /// <exception cref="MDARecordNotFoundException"></exception>
        public HierarchyViewModel GetHierarchy(string slug)
        {
            IsAuthorized<MDAHandler>(Permissions.CreateMDA);
            var mda = GetMDA(slug);
            var revenueHeads = mda.RevenueHeads.Where(r => r.Revenuehead == null).Select(r => r);
            return new HierarchyViewModel() { Name = mda.NameAndCode(), RevenueHeads = revenueHeads != null ? revenueHeads : new List<RevenueHead>() };
        }

        /// <summary>
        /// Get list of MDAs view
        /// </summary>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        public void ViewList()
        {
            IsAuthorized<MDAHandler>(Permissions.ViewMDAList);
        }


        /// <summary>
        /// Filter MDA based on parameters provided
        /// </summary>
        /// <param name="filterName"></param>
        /// <param name="orderBy"></param>
        /// <param name="searchText"></param>
        /// <returns>List{MDA}</returns>
        public List<MDA> GetCollection(string filterName = "Enabled", string orderBy = "Name", string searchText = null, bool ascending = true)
        {
            List<MDA> mdas = new List<MDA>();

            foreach (var item in _dataFilters)
            {
                if (item.FilterName().Equals(filterName))
                {
                    if (String.IsNullOrEmpty(searchText))
                    {
                        mdas = _mdaRepository.GetCollection(item.Filter()).ToList();
                    }
                    else
                    {
                        mdas = _mdaRepository.GetCollection(item.Filter(searchText)).ToList();
                    }
                    break;
                }
            }

            foreach (var item in _dataOrder)
            {
                if (item.OrderName().Contains(orderBy))
                {
                    mdas = item.Order(mdas, ascending);
                    break;
                }
            }
            return mdas;
        }



        /// <summary>
        /// Get the list of MDAs that this user has access to
        /// </summary>
        /// <returns>IEnumerable{MDAVM}</returns>
        public IEnumerable<MDAVM> GetMDAsOnAccessList(int userId, AccessType accessType, bool applyAccessRestrictions)
        {
            return _mdaRepository.GetAccessList(userId, accessType, applyAccessRestrictions);
        }


        #endregion

        #region Edit Operations


        /// <summary>
        /// Get edit view
        /// </summary>
        /// <param name="slug"></param>
        /// <returns>MDA</returns>
        /// <exception cref="MDARecordNotFoundException"></exception>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        public MDASettingsViewModel GetEditView(string slug)
        {
            Logger.Information("Getting view model for edit mda " + slug);
            ICollection<ExpertSystemSettings> collectionOfExpertSystems = GetExpertSystems();
            if (collectionOfExpertSystems == null || collectionOfExpertSystems.Count() <= 0) { throw new TenantNotFoundException("No expert systems found"); }
            Logger.Information("Checking permissions");
            IsAuthorized<MDAHandler>(Permissions.CreateMDA);
            var mda = GetMDA(slug);
            Logger.Information("Calling cash flow for banks");
            #region CASHFLOW 
            var context = _invoicingService.StartInvoicingService(new Dictionary<string, dynamic> { { "companyKeyCode", "" } });
            var bankService = _invoicingService.BankService(context);
            var listOfBanks = bankService.ListOfBanks();
            #endregion

            var sbankId = mda.UsesTSA ? "" : mda.BankDetails.BankId.ToString();
            return new MDASettingsViewModel() { UseTSA = mda.UsesTSA, Banks = listOfBanks, MDA = mda, SBankId = sbankId };
        }

        /// <summary>
        /// Try update MDA record
        /// </summary>
        /// <param name="callback">MDAController callback</param>
        /// <param name="updatedMDA">updated MDA view model</param>
        /// <param name="files">uploaded files if any</param>
        /// <param name="slug">slug of the mda that is being updated</param>
        /// <param name="bankCode">the selected bank code</param>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        /// <exception cref="MDARecordCouldNotBeUpdatedException"></exception>
        /// <exception cref="DirtyFormDataException"></exception>
        /// <exception cref="MissingFieldException"></exception>
        /// <exception cref="MDARecordNotFoundException"></exception>
        public void TryUpdateMDA(MDAController callback, MDA updatedMDA, HttpFileCollectionBase files, bool useTSA, string slug, string sbankId)
        {
            Logger.Information("Checking for permissions and validating model. Updating mda record " + slug);
            IsAuthorized<MDAHandler>(Permissions.CreateMDA).IsModelValid<MDAHandler, MDAController>(callback);
            Logger.Information("Validating expert system");
            ExpertSystemSettings expertSystem = GetExpertSystem();
            if (expertSystem == null) { throw new TenantNotFoundException("No tenant found while creating MDA"); }
            List<ErrorModel> errors = new List<ErrorModel>();

            if (!useTSA)
            {
                int bankId = 0;
                if (!Int32.TryParse(sbankId, out bankId)) { throw new Exception(string.Format("Bank code value {0} could not be parsed.  ", sbankId)); }
                updatedMDA.BankDetails.BankId = bankId;
                updatedMDA.BankDetails.BankCode = GetBankCode(bankId);
                updatedMDA.UsesTSA = false;
            }
            else
            {
                updatedMDA.BankDetails = new BankDetails() { BankId = expertSystem.TSA, BankAccountNumber = expertSystem.TSABankNumber, BankCode = expertSystem.BankCode };
                updatedMDA.UsesTSA = true;
            }

            UserPartRecord user = GetUser(_orchardServices.WorkContext.CurrentUser.Id);

            try
            {
                _coreHandler.TryUpdate(expertSystem, updatedMDA, 0, user, ref errors, files, slug);
            }
            catch (DirtyFormDataException)
            {
                AddValidationErrorsToCallback<MDAHandler, MDAController>(callback, errors);
            }
        }

        /// <summary>
        /// Change the active status of the MDA
        /// </summary>
        /// <param name="id">TaxEntityId of the MDA</param>
        public void ChangeStatus(int id)
        {
            IsAuthorized<MDAHandler>(Permissions.CreateMDA);
            var mda = GetMDA(id);

            if (mda == null) { throw new MDARecordNotFoundException(); }
            mda.IsActive = !mda.IsActive;
            mda.LastUpdatedBy = _mdaRepository.User(_orchardServices.WorkContext.CurrentUser.Id);
            if (!_mdaRepository.Update(mda)) { throw new MDARecordCouldNotBeUpdatedException(); }
        }

        #endregion

        /// <summary>
        /// Get MDA record by slug
        /// </summary>
        /// <param name="slug"></param>
        /// <returns>MDA</returns>
        /// <exception cref="MDARecordNotFoundException"></exception>
        public MDA GetMDA(string slug)
        {
            var mda = _mdaRepository.Get("Slug", slug);
            if (mda == null) { throw new MDARecordNotFoundException(); }
            return mda;
        }

        /// <summary>
        /// Get MDA record by TaxEntityId
        /// </summary>
        /// <param name="id"></param>
        /// <returns>MDAHandler</returns>
        /// <exception cref="MDARecordNotFoundException"></exception>
        private MDA GetMDA(int id)
        {
            var mda = _mdaRepository.Get(id);
            if (mda == null) { throw new MDARecordNotFoundException(); }
            return mda;
        }

        public List<CashFlowBank> GetBanks()
        {
            #region CASHFLOW 
            var context = _invoicingService.StartInvoicingService(new Dictionary<string, dynamic> { { "companyKeyCode", "" } });
            var bankService = _invoicingService.BankService(context);
            return bankService.ListOfBanks();
            #endregion
        }

        /// <summary>
        /// Check for permission
        /// </summary>
        /// <param name="createMDA"></param>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        public void CheckForPermission(Orchard.Security.Permissions.Permission createMDA)
        {
            IsAuthorized<MDAHandler>(createMDA);
        }
    }
}