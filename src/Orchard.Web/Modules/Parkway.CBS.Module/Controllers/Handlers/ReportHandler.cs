using Orchard;
using Orchard.Localization;
using Parkway.CBS.Module.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using Parkway.CBS.Module.ViewModels;
using System.Web.Mvc;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.DataFilters.MDAReport.Order;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services;
using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Orchard.Modules.Services;
using Parkway.CBS.Core.HTTP.RemoteClient.Contracts;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Core.DataFilters.TaxPayerReport;
using Parkway.CBS.Core.DataFilters.AssessmentReport;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;

namespace Parkway.CBS.Module.Controllers.Handlers
{
    public class ReportHandler : BaseHandler, IReportHandler
    {
        private readonly IOrchardServices _orchardServices;
        public Localizer T { get; set; }
        private readonly IRevenueHeadHandler _revenueHeadHandler;
        private readonly IMDAHandler _mdaHandler;
        private readonly IInvoiceManager<Invoice> _invoiceHandler;

        private readonly IRoleUserManager<AccessRoleUser> _accessRoleUserRepo;

        private readonly ITaxEntityManager<TaxEntity> _customerRepository;
        private readonly IEnumerable<IMDAReportOrder> _reportOrder;
        private readonly ITaxEntityCategoryManager<TaxEntityCategory> _sectorRepository;
        private readonly IAdminSettingManager<ExpertSystemSettings> _tenantRepository;
        public readonly IAdminSettingManager<ExpertSystemSettings> _settingsRepository;

        private readonly ITaxPayerReportFilter _taxpayerReportFilter;

        private readonly IInvoiceAssessmentsReportFilter _invoiceAssessmentsFilter;

        private readonly ICoreCollectionService _coreCollectionService;


        public ReportHandler(IOrchardServices orchardServices, ITaxEntityManager<TaxEntity> customerRepository,
            IEnumerable<IMDAReportOrder> reportOrder, IMDAHandler mdaHandler,
            IInvoiceManager<Invoice> invoiceHandler, IRevenueHeadHandler revenueHeadHandler, ITaxEntityCategoryManager<TaxEntityCategory> sectorRepository, IAdminSettingManager<ExpertSystemSettings> tenantRepository, IModuleService moduleService, IAdminSettingManager<ExpertSystemSettings> settingsRepository, ITaxPayerReportFilter taxpayerReportFilter, IInvoiceAssessmentsReportFilter invoiceAssessmentsFilter, IRoleUserManager<AccessRoleUser> roleUserRepo, ICoreCollectionService coreCollectionService) : base(orchardServices, settingsRepository)
        {
            _orchardServices = orchardServices;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
            _customerRepository = customerRepository;
            _reportOrder = reportOrder;
            _mdaHandler = mdaHandler;
            _invoiceHandler = invoiceHandler;
            _revenueHeadHandler = revenueHeadHandler;
            _sectorRepository = sectorRepository;
            _tenantRepository = tenantRepository;
            _settingsRepository = settingsRepository;
            _taxpayerReportFilter = taxpayerReportFilter;
            _invoiceAssessmentsFilter = invoiceAssessmentsFilter;
            _accessRoleUserRepo = roleUserRepo;

            _coreCollectionService = coreCollectionService;
        }



        /// <summary>
        /// Get model for invoice assessment report
        /// </summary>
        /// <param name = "searchParams" ></ param >
        /// < returns > MDAReportViewModel </ returns >
        public MDAReportViewModel GetInvoiceAssessmentReport(InvoiceAssessmentSearchParams searchParams)
        {
            //first we need to check if this user has been assigned any role on the access list
            //we check if this user has been constrained to a role
            bool applyAccessRestrictions = _accessRoleUserRepo.UserHasAcessTypeRole(_orchardServices.WorkContext.CurrentUser.Id, AccessType.InvoiceAssessmentReport);

            MDAReportViewModel model = new MDAReportViewModel();

            searchParams.AdminUserId = _orchardServices.WorkContext.CurrentUser.Id;
            //
            //get mda record
            int parsedId = 0;
            searchParams.MDAId = 0;
            if (Int32.TryParse(searchParams.SMDA, out parsedId)) { searchParams.MDAId = parsedId; }
            if (Int32.TryParse(searchParams.SRevenueHeadId, out parsedId)) { searchParams.RevenueHeadId = parsedId; }
            if(Int32.TryParse(searchParams.SCategory, out parsedId)) { searchParams.Category = parsedId; }


            model.MDAs = _mdaHandler.GetMDAsOnAccessList(searchParams.AdminUserId, AccessType.InvoiceAssessmentReport, applyAccessRestrictions);
            //new { ReportRecords, Aggregate }
            dynamic recordsAndAggregate = _invoiceAssessmentsFilter.GetReportViewModel(searchParams, applyAccessRestrictions);
            if (searchParams.MDAId > 0)
            {
                model.RevenueHeads = _revenueHeadHandler.GetRevenueHeadsOnAccessList(searchParams.AdminUserId, searchParams.MDAId, AccessType.InvoiceAssessmentReport, applyAccessRestrictions);
            }
            else
            {
                model.RevenueHeads = new List<RevenueHeadDropDownListViewModel> { };
            }

            model.Categories = _sectorRepository.GetCategories();
            model.ReportRecords = ((IEnumerable<DetailReport>)recordsAndAggregate.ReportRecords);

            model.TotalInvoiceAmount = ((IEnumerable<CollectionReportStats>)recordsAndAggregate.Aggregate).First().TotalAmountOfPayment;
            model.TotalNumberOfInvoicesSent = (int)(((IEnumerable<CollectionReportStats>)recordsAndAggregate.Aggregate).First().RecordCount);

            MDAVM mda = null;
            RevenueHeadDropDownListViewModel revenueHead = null;
            TaxEntityCategoryVM category = null;

            if (searchParams.MDAId > 0)
            {
                mda = model.MDAs.Where(m => m.Id == searchParams.MDAId).FirstOrDefault();
                if(searchParams.RevenueHeadId > 0)
                {
                    revenueHead = model.RevenueHeads.Where(m => m.Id == searchParams.MDAId).FirstOrDefault();
                }
            }
            if(searchParams.Category > 0)
            {
                category = model.Categories.Where(c => c.Id == searchParams.Category).FirstOrDefault();
            }
            model.CategoryName = category == null? "All Categories" : category.Name;
            model.MDAName = mda == null ? "All MDAs" : mda.Name;
            model.RevenueHeadName = revenueHead == null ? "All Revenue Heads" : revenueHead.Name;
            //
            if(model.MDAs == null || !model.MDAs.Any()) { model.MDAs = new List<MDAVM> { }; }
            if(model.RevenueHeads == null || !model.RevenueHeads.Any()) { model.RevenueHeads = new List<RevenueHeadDropDownListViewModel> { }; }
            if(model.Categories == null || !model.Categories.Any()) { model.Categories = new List<TaxEntityCategoryVM> { }; }
            if(model.ReportRecords == null || !model.ReportRecords.Any()) { model.ReportRecords = new List<DetailReport> { }; }
            //
            model.Token = Util.LetsEncrypt(AccessType.InvoiceAssessmentReport.ToString(), AppSettingsConfigurations.EncryptionSecret);

            return model;
        }



        /// <summary>
        /// Get the list of revenue heads that have been assigned to this user
        /// </summary>
        /// <param name="smdaId"></param>
        /// <param name="accessType"></param>
        /// <returns>APIResponse</returns>
        public APIResponse GetRevenueHeads(string smdaId, AccessType accessType)
        {
            bool applyAccessRestrictions = _accessRoleUserRepo.UserHasAcessTypeRole(_orchardServices.WorkContext.CurrentUser.Id, AccessType.InvoiceAssessmentReport);

            int id = 0;
            if (!Int32.TryParse(smdaId, out id))
            {
                return new APIResponse { Error = true, ResponseObject = ErrorLang.mdacouldnotbefound().ToString() };
            }

            IEnumerable<RevenueHeadDropDownListViewModel> revenueHeads = _revenueHeadHandler.GetRevenueHeadsOnAccessList(_orchardServices.WorkContext.CurrentUser.Id, id, AccessType.InvoiceAssessmentReport, applyAccessRestrictions);

            return new APIResponse { ResponseObject = revenueHeads.ToList() ?? new List<RevenueHeadDropDownListViewModel> { } };
        }



        /// <summary>
        /// Get view for collections
        /// </summary>
        /// <param name="searchData"></param>
        /// <returns>CollectionReportViewModel</returns>
        public CollectionReportViewModel GetReportForCollection(CollectionSearchParams searchParams)
        {
            return _coreCollectionService.GetReportForCollection(searchParams);
        }


        public DemandNoticeViewModel GetDemandNoticeReport(DateTime startDate, DateTime endDate, string mdaSelected, string srevenueHeadSelected, PaymentOptions options, int take, int skip)
        {
            IsAuthorized<ReportHandler>(Permissions.ViewRevHeadDashBoard);
            DemandNoticeViewModel model = new DemandNoticeViewModel();
            if (string.IsNullOrEmpty(mdaSelected))
            {
                return model;
            }
            //get mda record
            MDA mda = _mdaHandler.GetMDA(mdaSelected);
            //convert revenue head string to int
            int revenueHeadSelected = 0;
            Int32.TryParse(srevenueHeadSelected, out revenueHeadSelected);
            //get revenue head
            RevenueHead revenueHead = mda.RevenueHeads.Where(rh => rh.Id == revenueHeadSelected).FirstOrDefault();
            //get expected revenue
            string filterCombo = string.IsNullOrEmpty(options.TINText) ? "" : "TIN";
            filterCombo += string.IsNullOrEmpty(options.InvoiceNumber) ? "" : "INVOICE";
            IEnumerable<DetailReport> reports = new List<DetailReport>();
            IEnumerable<InvoicesStatHolder> reportsStats = new List<InvoicesStatHolder>();

            //foreach (var filter in _assessmentSearchFilters)
            //{
            //    if (filter.FilterName() == filterCombo)
            //    {
            //        //GetAggregate(DateTime startDate, DateTime endDate, Models.MDA mda, Models.RevenueHead revenueHead, List < HQLPart > list, TaxEntityCategory sector, PaymentOptions options);
            //        //IEnumerable<DetailReport> GetReport(DateTime startDate, DateTime endDate, Models.MDA mda, Models.RevenueHead revenueHead, List<HQLPart> list, TaxEntityCategory sector, PaymentOptions options, int skip, int take);
            //        reports = filter.GetReport(startDate, endDate, options, null, skip, take);
            //        reportsStats = filter.GetAggregate(startDate, endDate, options, null);
            //        break;
            //    }
            //}
            model.TotalNumberOfInvoicesSent = reportsStats != null ? reportsStats.First().Count : 0;
            model.TotalInvoiceAmount = reportsStats != null ? reportsStats.First().AmountExpected : 0;
            //model.ReportType = reportsStats != null ? (InvoiceStatusList)reportsStats.First().Status : new InvoiceStatusList();
            model.MDAName = mda.Name;
            model.RevenueHeadName = revenueHead.Name;
            model.Report = reports != null ? reports : new List<DetailReport>();

            return model;
        }


        /// <summary>
        /// Check for permission
        /// </summary>
        /// <param name="createMDA"></param>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        public void CheckForPermission(Orchard.Security.Permissions.Permission permission)
        {
            IsAuthorized<ReportHandler>(permission);
        }



        public TaxProfilesReportVM GetTaxProfilesReport(TaxProfilesReportVM model, int page, int pageSize)
        {
            List<TaxEntityReportDetail> taxPayerReport = new List<TaxEntityReportDetail>();
            try
            {
                var searchModel = new TaxProfilesSearchParams { Name = model.Name, PayerId = model.PayerId, PhoneNumber = model.PhoneNumber, TIN = model.TIN, CategoryId = model.TaxCategory };
                var searchRecords = _taxpayerReportFilter.GetReportForTaxProfiles(searchModel, page, pageSize);
                if (searchRecords.Count() > 0)
                {
                    taxPayerReport = searchRecords.Select(x => new TaxEntityReportDetail
                    {
                        Id = x.Id,
                        Address = x.Address,
                        Category = x.TaxEntityCategory?.Name,
                        Name = x.Recipient,
                        Email = x.Email,
                        PhoneNumber = x.PhoneNumber,
                        TaxPayerIdentificationNumber = x.TaxPayerIdentificationNumber,
                        RegNumber = x.RCNumber,
                        PayerId = x.PayerId,
                    }).ToList();
                }
                var result = _taxpayerReportFilter.GetAggregateForTaxProfiles(searchModel).First();

                return new TaxProfilesReportVM
                {
                    TaxCategories = GetTaxCategories(),
                    ReportRecords = taxPayerReport,
                    Name = model.Name,
                    TIN = model.TIN,
                    PayerId = model.PayerId,
                    PhoneNumber = model.PhoneNumber,
                    TaxCategory = model.TaxCategory,
                    TotalNumberOfTaxPayers = result.TotalNumberOfTaxProfiles
                };
            }
            catch (Exception exception)
            {
                Logger.Error(string.Format("Error getting tax profiles report", exception.Message), exception);
                throw;
            }
        }



        public IEnumerable<RevenueHeadDropDownListViewModel> GetRevenueHeads(string mdaSlug)
        {
            var list = _mdaHandler.GetMDA(mdaSlug).RevenueHeads.Where(rh => rh.BillingModel != null);
            if (list != null && list.Count() > 0) { return list.Select(r => new RevenueHeadDropDownListViewModel() { Name = r.Name, Code = r.Code, Slug = r.Slug, Id = r.Id }); }
            else { return new List<RevenueHeadDropDownListViewModel>(); }
        }



        public IEnumerable<MDA> MDAs(MDAFilter filter = MDAFilter.All)
        {
            return _mdaHandler.GetCollection(filter.ToString());
        }



        /// <summary>
        /// Get payment report per revenue head
        /// </summary>
        /// <param name="orderBy"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="count"></param>
        /// <param name="skip"></param>
        /// <param name="slug"></param>
        /// <param name="search"></param>
        /// <param name="direction"></param>
        /// <returns>PerRevenueReportViewModel</returns>
        public PerRevenueReportViewModel GetMDAReportPerRevenueHeadCollection(string orderBy, DateTime startDate, DateTime endDate, int count, int skip, string slug, string search, bool direction)
        {
            //get monthly report per revenue head
            IEnumerable<SelectListItem> mdasDropDown = new List<SelectListItem>();
            var mdas = _mdaHandler.GetCollection("All");
            var mda = mdas.Where(m => m.Slug == slug).Select(m => m).SingleOrDefault();
            if (mda == null)
                mda = mdas.ElementAt(0);

            //get invoice for the revenue heads belonging to the given mda
            IEnumerable<Invoice> invoices = new List<Invoice>();
            var invoicesPerRevenueHead = _invoiceHandler.GetRevenueCollectionPerMDA(mda, startDate, endDate, count, skip);
            //why am I doing this????????????
            var revenueHeads = invoicesPerRevenueHead.Select(r => r.RevenueHead).Distinct();

            var summary = revenueHeads.Where(m => m.Invoices.Any(invc => (invc.CreatedAtUtc >= startDate) && (invc.CreatedAtUtc <= endDate))).Select(invc => invc.Invoices);
            mdasDropDown = mdas.Select(m => new SelectListItem() { Text = m.Name, Value = m.Slug });

            //var _mda = new SelectList(mdasDropDown, new { Text = mda.Name });
            PerRevenueReportViewModel model = new PerRevenueReportViewModel()
            {
                Mdas = mdasDropDown,
            };

            var report = revenueHeads.Select(rh => new RevenueReport()
            {
                AmountPaid = (long)invoicesPerRevenueHead.Where(invc => invc.RevenueHead.Id == rh.Id).Sum(r => r.Amount),
                MDAName = rh.Mda.Name,
                NumberOfInvoices = invoicesPerRevenueHead.Where(invc => invc.RevenueHead.Id == rh.Id).Count(),
                RevenueHeadName = rh.Name,
                Id = rh.Id,
                Slug = rh.Slug,
            });

            model.NumberOfRecords = mda.RevenueHeads.Count();
            model.RevenueReport = report;
            model.TotalAmount = (long)summary.Select(invc => invc.Sum(s => s.Amount)).Sum(inv => inv);
            model.TotalNumberOfInvoices = summary.Select(invc => invc.Select(v => v).Count()).Sum();
            model.MDAName = mda.Name;

            return model;
        }

        /// <summary>
        /// revenue head invoice break down
        /// </summary>
        /// <param name="orderBy"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="count"></param>
        /// <param name="skip"></param>
        /// <param name="slug"></param>
        /// <param name="id"></param>
        /// <param name="search"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public RevenueHeadInvoicesViewModel GetRevenueHeadInvoicesCollection(string orderBy, DateTime startDate, DateTime endDate, int count, int skip, string slug, int id, string search, bool direction)
        {
            var revenueHead = _revenueHeadHandler.GetRevenueHead(slug, id);
            //get invoice belonging to this revenue head
            Dictionary<int, IEnumerable<Invoice>> invoices = new Dictionary<int, IEnumerable<Invoice>>();
            RevenueHeadInvoicesViewModel model = new RevenueHeadInvoicesViewModel();

            invoices = _invoiceHandler.GetRevenueInvoiceCollectionForRevenueHead(revenueHead, startDate, endDate, count, skip);

            IEnumerable<InvoicesReport> reports = new List<InvoicesReport>();

            IEnumerable<Invoice> result = new List<Invoice>();

            foreach (var item in invoices)
            {
                model.NumberOfRecords = item.Key;
                result = item.Value;
            }

            reports = result.Select(invc => new InvoicesReport()
            {
                AmountPaid = (long)invc.Amount,
                MDAName = invc.Mda.Name,
                RevenueHeadName = invc.RevenueHead.Name,
                Date = invc.CreatedAtUtc.ToString("dd MMMM yyyy"),
                InvoiceNumber = invc.InvoiceNumber
            });

            model.RevenueHeaadName = revenueHead.Name;
            model.RevenueReport = reports;
            //model.TotalAmount = 
            model.TotalAmount = (long)revenueHead.Invoices.Where(invc => invc.Id != 0).Sum(invc => invc.Amount);
            model.TotalAmount = (long)revenueHead.Invoices.Where(invc => invc.Id != 0).Sum(invc => invc.Amount);
            model.TotalNumberOfInvoices = revenueHead.Invoices.Count();
            return model;
        }

        public MDAExpectationViewModel GetExpectationForMDACollection(string orderBy, DateTime startDate, DateTime endDate, int count, int skip, string search, bool direction, string slug = "")
        {
            IEnumerable<SelectListItem> mdas = new List<SelectListItem>();
            mdas = _mdaHandler.GetCollection("All").Select(m => new SelectListItem() { Text = m.Name, Value = m.Slug });

            string value = mdas.ElementAt(0).Value;
            //get mda record
            var mda = _mdaHandler.GetMDA(string.IsNullOrEmpty(slug) ? value : slug);

            IEnumerable<Invoice> invoices = new List<Invoice>();
            MDAExpectationViewModel model = new MDAExpectationViewModel();

            model = _invoiceHandler.ExpectationPerMDA(mda, startDate, endDate, skip, count, direction, orderBy);
            model.Mdas = mdas;
            return model;
        }

        public MDAMonthlyPaymentViewModel MDAMonthlyPaymentCollection(string orderBy, DateTime startDate, DateTime endDate, int take, int skip, string search, bool direction)
        {
            IEnumerable<MDA> mdas = new List<MDA>();
            mdas = _mdaHandler.GetCollection("All").OrderBy(m => m.Name);
            int pageSize = mdas.Count();
            var strippedMDAs = mdas.Skip(skip).Take(take);
            MDAMonthlyPaymentViewModel model = new MDAMonthlyPaymentViewModel();
            model = _invoiceHandler.GetMonthlyMDAsPayment(strippedMDAs, startDate, endDate);

            var summary = mdas.Where(m => m.Invoices.Any(invc => (invc.CreatedAtUtc >= startDate) && (invc.CreatedAtUtc <= endDate))).Select(invc => invc.Invoices);


            model.TotalAmount = (long)summary.Select(invc => invc.Sum(s => s.Amount)).Sum(inv => inv);
            model.TotalNumberOfInvoices = summary.Select(invc => invc.Select(v => v).Count()).Sum();

            model.TotalActualIncome = (long)summary.Select(invc => invc.Where(inv => inv.Status == (int)InvoiceStatusList.Paid))
                                            .Select(mi => mi.Sum(inv => inv.Amount)).Sum(m => m);
            model.TotalNumberOfInvoicesPaid = summary.Select(invc => invc.Where(inv => inv.Status == (int)InvoiceStatusList.Paid))
                                                .Select(mi => mi.Count()).Sum(m => m);
            model.NumberOfRecords = pageSize;
            return model;
        }

        public MDAMonthlyPaymentPerRevenueViewModel MDAMonthlyPaymentPerRevenueCollection(string orderBy, DateTime startDate, DateTime endDate, int take, int skip, string search, bool direction, string mdaSlug)
        {
            IEnumerable<SelectListItem> mdaDropDown = new List<SelectListItem>();
            var mdas = _mdaHandler.GetCollection("All");
            mdaDropDown = mdas.Select(m => new SelectListItem() { Text = m.Name, Value = m.Slug });

            //string value = mdas.ElementAt(5).Value;

            MDA mda = new MDA();
            mda = _mdaHandler.GetMDA(string.IsNullOrEmpty(mdaSlug) ? mdas.ElementAt(0).Name : mdaSlug);
            int pageSize = mda.RevenueHeads.Count();
            var strippedRevenHeads = mda.RevenueHeads.Skip(skip).Take(take);

            MDAMonthlyPaymentPerRevenueViewModel model = new MDAMonthlyPaymentPerRevenueViewModel();
            model = _invoiceHandler.GetMonthlyMDAsPaymentPerRevenueHead(strippedRevenHeads, endDate, startDate);
            model.NumberOfRecords = pageSize;

            var summary = mda.Invoices.Where(invc => (invc.CreatedAtUtc >= startDate) && (invc.CreatedAtUtc <= endDate)).Select(invc => invc);

            model.TotalNumberOfInvoices = summary.Count();
            model.TotalAmount = (long)summary.Sum(inv => inv.Amount);
            model.TotalActualIncome = (long)summary.Where(inv => inv.Status == (int)InvoiceStatusList.Paid).Sum(m => m.Amount);
            model.TotalNumberOfInvoicesPaid = summary.Where(inv => inv.Status == (int)InvoiceStatusList.Paid).Count();
            model.Mdas = mdaDropDown;
            model.MDAName = mda.Name;
            return model;
        }

        public RevenueHeadPaymentBreakdownViewModel GetRevenueHeadBreakDownPaymentCollection(string orderBy, DateTime startDate, DateTime endDate, int take, int skip, string slug, int id, string search, bool direction)
        {
            RevenueHead revenueHead = new RevenueHead();
            revenueHead = _revenueHeadHandler.GetRevenueHead(slug, id);
            if (revenueHead == null) { throw new Exception(); }

            IEnumerable<Invoice> invoices = new List<Invoice>();

            invoices = revenueHead.Invoices.Where(invc => (invc.CreatedAtUtc >= startDate) && (invc.CreatedAtUtc <= endDate)).Select(invc => invc);

            int pageSize = invoices.Count();

            var strippedInvoices = invoices.Skip(skip).Take(take);

            List<RevenueBreakDownPaymentReport> reports = new List<RevenueBreakDownPaymentReport>();

            foreach (var item in strippedInvoices)
            {
                reports.Add(new RevenueBreakDownPaymentReport()
                {
                    AmountPaid = (long)item.Amount,
                    Date = item.CreatedAtUtc.ToString("dd MM yyyy"),

                });
            }

            RevenueHeadPaymentBreakdownViewModel model = new RevenueHeadPaymentBreakdownViewModel()
            {
                NumberOfRecords = pageSize,
                RevenueHeadName = revenueHead.Name,
                MDAName = revenueHead.Mda.Name,
                RevenueBreakDownPaymentReport = reports
            };

            return model;
        }


        /// <summary>
        /// Get the list of tax categories that are active
        /// </summary>
        /// <returns>IEnumerable{TaxEntityCategoryVM}</returns>
        public IEnumerable<TaxEntityCategoryVM> GetTaxCategories()
        {
            return _sectorRepository.GetCollection(tc => tc.Status == true).ToList().Select(tc => new TaxEntityCategoryVM { Id = tc.Id, Name = tc.Name });
        }

        public TaxPayerDetailsViewModel GetTaxPayer(string payerId)
        {
            try
            {
                var taxPayer = _customerRepository.GetTaxPayerWithDetails(payerId);
                if (taxPayer == null) { return new TaxPayerDetailsViewModel { }; }

                return new TaxPayerDetailsViewModel { Id = taxPayer.Id, Address = taxPayer.Address, Email = taxPayer.Email, Name = taxPayer.Name, TIN = taxPayer.TIN, PhoneNumber = taxPayer.PhoneNumber, Category = taxPayer.Category, PayerId = taxPayer.PayerId, TaxPayerCode = taxPayer.TaxPayerCode, DateCreated = taxPayer.CreatedAtUtc };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error getting tax payer Id " + payerId);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateTaxPayer(TaxPayerDetailsViewModel model)
        {
            bool response = false;
            TaxPayerWithDetails taxPayerWithDetails = new TaxPayerWithDetails();
            taxPayerWithDetails.TaxPayerCode = model.TaxPayerCode;
            taxPayerWithDetails.PayerId = model.PayerId;

            return _customerRepository.UpdateTaxPayer(taxPayerWithDetails);
        }

        public bool CheckIfTaxPayerCodeExist(string taxPayerCode, string taxPayerId)
        {
            return _customerRepository.CheckIfTaxPayerCodeExist(taxPayerCode, taxPayerId);
        }

    }
}