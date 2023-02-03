using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Payee;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Payee.PayeeAdapters.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using Parkway.CBS.Payee;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Core.DataFilters.CollectionReport;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CoreCollectionService : ICoreCollectionService
    {

        private readonly IDirectAssessmentBatchRecordManager<DirectAssessmentBatchRecord> _directAssessmentBatchRecordRepository;
        private readonly IDirectAssessmentPayeeManager<DirectAssessmentPayeeRecord> _directAssessmentPayeeRepository;

        private readonly ITaxEntityCategoryManager<TaxEntityCategory> _taxCategoriesRepository;
        private readonly IMDAManager<MDA> _mdaRepository;
        private readonly IRevenueHeadManager<RevenueHead> _revenueHeadRepository;

        private readonly ICoreInvoiceService _coreInvoiceService;
        private readonly ICoreFormService _coreFormService;
        private readonly ITenantStateSettings<TenantCBSSettings> _tenantStateSettings;
        public readonly IPayeeAssessmentConfiguration _payeeConfig;

        private readonly ICollectionReportFilter _collectionReportFilter;
        private readonly IRoleUserManager<AccessRoleUser> _accessRoleUserRepo;
        private readonly IExternalPaymentProviderManager<ExternalPaymentProvider> _externalPaymentProvider;
        public ILogger Logger { get; set; }


        public CoreCollectionService(IMDAManager<MDA> mdaRepository, ITaxEntityCategoryManager<TaxEntityCategory> taxCategoriesRepository, ICoreInvoiceService coreInvoiceService, ITenantStateSettings<TenantCBSSettings> tenantStateSettings, IPayeeAssessmentConfiguration payeeConfig, IDirectAssessmentBatchRecordManager<DirectAssessmentBatchRecord> directAssessmentBatchRecordRepository, IDirectAssessmentPayeeManager<DirectAssessmentPayeeRecord> directAssessmentPayeeRepository, IRevenueHeadManager<RevenueHead> revenueHeadRepository, ICollectionReportFilter collectionreportFilter, IRoleUserManager<AccessRoleUser> roleUserRepo, ICoreFormService coreFormService, IExternalPaymentProviderManager<ExternalPaymentProvider> externalPaymentProvider)
        {
            _revenueHeadRepository = revenueHeadRepository;
            _mdaRepository = mdaRepository;
            Logger = NullLogger.Instance;
            _taxCategoriesRepository = taxCategoriesRepository;
            _coreInvoiceService = coreInvoiceService;
            _tenantStateSettings = tenantStateSettings;
            _payeeConfig = payeeConfig;
            _directAssessmentBatchRecordRepository = directAssessmentBatchRecordRepository;
            _directAssessmentPayeeRepository = directAssessmentPayeeRepository;
            _collectionReportFilter = collectionreportFilter;
            _accessRoleUserRepo = roleUserRepo;
            _coreFormService = coreFormService;
            _externalPaymentProvider = externalPaymentProvider;
        }


        /// <summary>
        /// Get view for collections
        /// </summary>
        /// <param name="searchData"></param>
        /// <returns>CollectionReportViewModel</returns>
        public CollectionReportViewModel GetReportForCollection(CollectionSearchParams searchParams, bool populateDropDowns = true)
        {
            bool applyAccessRestrictions = _accessRoleUserRepo.UserHasAcessTypeRole(searchParams.AdminUserId, AccessType.CollectionReport);

            CollectionReportViewModel returnModel = new CollectionReportViewModel();
            returnModel.Banks = Util.GetListOfObjectsFromJSONFile<BankVM>(SettingsFileNames.Banks.ToString());

            int parsedId = 0;
            if (Int32.TryParse(searchParams.SelectedMDA, out parsedId)) { searchParams.MDAId = parsedId; }
            if (Int32.TryParse(searchParams.SRevenueHeadId, out parsedId)) { searchParams.RevenueHeadId = parsedId; }
            if (Int32.TryParse(searchParams.SelectedPaymentProvider, out parsedId)) { searchParams.PaymentProviderId = parsedId; }

            returnModel.ReportRecords = _collectionReportFilter.GetReport(searchParams, applyAccessRestrictions).Select(x =>
                new CollectionDetailReport()
                {
                    InvoiceNumber = x.InvoiceNumber,
                    Amount = Math.Round(x.AmountPaid, 2) + 0.00M,
                    PaymentDate = x.PaymentDate,
                    PaymentRef = x.PaymentReference,
                    RevenueHeadName = x.RevenueHead.Name,
                    TaxPayerName = x.TaxEntity.Recipient,
                    TaxPayerTIN = x.TaxEntity.TaxPayerIdentificationNumber,
                    ReceiptNumber = x.Receipt.ReceiptNumber,
                    PaymentProvider = Util.GetPaymentProviderDescription(x.PaymentProvider),
                    PaymentProviderCode = x.PaymentProvider,
                    Channel = Util.GetPaymentChannelDescription(x.Channel),
                    MDAName = x.MDA.Name,
                    RevenueHeadId = x.RevenueHead.Id,
                    MDAId = x.MDA.Id,
                    Bank = Util.GetBankName(returnModel.Banks, x.BankCode),
                    PayerId = x.TaxEntity.PayerId,
                    PaymentDateStringVal = x.PaymentDate.ToString("dd MMM yyyy"),
                    InvoiceItemCode = x.InvoiceItem.Id
                });


            if (populateDropDowns)
            {
                returnModel.Mdas = _mdaRepository.GetAccessList(searchParams.AdminUserId, AccessType.CollectionReport, applyAccessRestrictions);
                returnModel.PaymentProviders = _externalPaymentProvider.GetProviders();

                if (searchParams.MDAId > 0)
                {
                    returnModel.RevenueHeads = _revenueHeadRepository.GetRevenueHeadsOnAccessListForMDA(searchParams.AdminUserId, searchParams.MDAId, AccessType.CollectionReport, applyAccessRestrictions);
                }
                else
                {
                    returnModel.RevenueHeads = new List<RevenueHeadDropDownListViewModel> { };
                }
            }
            
            var result = _collectionReportFilter.GetAggregate(searchParams, applyAccessRestrictions).First();

            returnModel.TotalAmountPaid = result.TotalAmountOfPayment;
            returnModel.TotalNumberOfPayment = (int)result.RecordCount;

            returnModel.SelectedBank = searchParams.SelectedBankCode;

            returnModel.Token = Util.LetsEncrypt(AccessType.CollectionReport.ToString(), AppSettingsConfigurations.EncryptionSecret);

            return returnModel;
        }



        /// <summary>
        /// Get the assigned form fields
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <param name="categoryId"></param>
        /// <returns>IEnumerable{FormControlViewModel}</returns>
        public IEnumerable<FormControlViewModel> GetRevenueHeadFormFields(int revenueHeadId, int categoryId)
        {
            return _coreFormService.GetRevenueHeadFormFields(revenueHeadId, categoryId);
        }



        /// <summary>
        /// Get record details for collection report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>CollectionReportViewModel</returns>
        public CollectionReportViewModel GetCollectionReport(CollectionSearchParams searchParams)
        {
            bool applyAccessRestrictions = _accessRoleUserRepo.UserHasAcessTypeRole(searchParams.AdminUserId, AccessType.InvoiceAssessmentReport);

            CollectionReportViewModel returnModel = new CollectionReportViewModel();
            var banks = Util.GetListOfObjectsFromJSONFile<BankVM>(SettingsFileNames.Banks.ToString());

            int parsedId = 0;
            if (Int32.TryParse(searchParams.SelectedMDA, out parsedId)) { searchParams.MDAId = parsedId; }
            if (Int32.TryParse(searchParams.SRevenueHeadId, out parsedId)) { searchParams.RevenueHeadId = parsedId; }
            if (Int32.TryParse(searchParams.SelectedPaymentProvider, out parsedId)) { searchParams.PaymentProviderId = parsedId; }

            returnModel.ReportRecords = _collectionReportFilter.GetReport(searchParams, applyAccessRestrictions).Select(x =>
                new CollectionDetailReport()
                {
                    InvoiceNumber = x.InvoiceNumber,
                    Amount = x.AmountPaid,
                    PaymentDate = x.PaymentDate,
                    PaymentRef = x.PaymentReference,
                    RevenueHeadName = x.RevenueHead.Name,
                    TaxPayerName = x.TaxEntity.Recipient,
                    TaxPayerTIN = x.TaxEntity.TaxPayerIdentificationNumber,
                    ReceiptNumber = x.Receipt.ReceiptNumber,
                    PaymentProvider = Util.GetPaymentProviderDescription(x.PaymentProvider),
                    Channel = Util.GetPaymentChannelDescription(x.Channel),
                    MDAName = x.MDA.Name,
                    RevenueHeadId = x.RevenueHead.Id,
                    MDAId = x.MDA.Id,
                    Bank = Util.GetBankName(banks, x.BankCode)
                });

            return returnModel;
        }



        /// <summary>
        /// Get revenue head details by revenue head Id
        /// </summary>
        /// <param name="id">Id of the revenue head</param>
        /// <returns>RevenueHeadDetails</returns>
        /// <exception cref="CannotFindRevenueHeadException"></exception>
        public RevenueHeadDetails GetRevenueHeadDetails(int id)
        {
            var revenueHead = _revenueHeadRepository.GetRevenueHeadDetails(id);
            if (revenueHead == null) { throw new CannotFindRevenueHeadException(); }
            return revenueHead;
        }


        /// <summary>
        /// Get the billing model for this revenue head
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <returns>BillingModel</returns>
        public BillingModel GetRevenueHeadBillingModel(int revenueHeadId)
        {
            BillingModel billing = _revenueHeadRepository.GetRevenueHeadBilling(revenueHeadId);
            if (billing == null) { throw new NoBillingInformationFoundException(); }
            return billing;
        }


        /// <summary>
        /// Get a list of active and visble mdas ordered by name
        /// </summary>
        /// <returns>IEnumerable<MDA></returns>
        public IEnumerable<MDA> GetMDAs()
        {
            return _mdaRepository.GetCollection(m => (m.IsVisible == true) && (m.IsActive == true)).OrderBy(k => k.Name);
        }       


        /// <summary>
        /// Get tax category
        /// </summary>
        /// <param name="categoryType"></param>
        /// <returns>TaxEntityCategory</returns>
        /// <exception cref="NoCategoryFoundException"></exception>
        public TaxEntityCategory GetTaxEntityCategory(int catId)
        {
            Logger.Information("Category string parse. calling db");
            var category = _taxCategoriesRepository.Get(catId);
            if (category == null) { throw new NoCategoryFoundException("No category found for the category name " + catId); }
            return category;
        }


        /// <summary>
        /// Get invoice URL
        /// </summary>
        /// <param name="bin"></param>
        /// <returns></returns>
        /// <exception cref="NoInvoicesMatchingTheParametersFoundException"></exception>
        public string GetInvoiceURL(string bin)
        {
            return _coreInvoiceService.GetInvoiceURL(bin);
        }


        public ReceiptObj GetReciepts(string phoneNumber, string receiptNumber, ReceiptStatus status, DateTime startDate, DateTime endDate, int skip, int take, bool queryForCount = false)
        {
            return _directAssessmentPayeeRepository.GetPayeReceipts(phoneNumber, receiptNumber, status, startDate, endDate, skip, take, queryForCount);
        }



        public DirectAssessmentBatchRecord GetBatchRecord(long batchRecordId)
        {
            return _directAssessmentBatchRecordRepository.Get(batchRecordId);            
        }


        public DirectAssessmentReportVM GetPayeAsessmentReport(DirectAssessmentBatchRecord record, int take, int skip, TaxEntity entity = null)
        {
            List<DirectAssessmentPayeeRecord> listOfPayees = _directAssessmentPayeeRepository.GetRecords(record.Id, take, skip).ToList();

            int count = record.TotalNoOfRowsProcessed;
            int errors = _directAssessmentPayeeRepository.Count(x => (x.DirectAssessmentBatchRecord == record) && (x.HasErrors));

            List<PayeeReturnModelVM> payeeVM = listOfPayees.Select(py => new PayeeReturnModelVM { Exemptions = py.Exemptions, GrossAnnual = py.GrossAnnual, Month = py.Month, TaxableIncome = py.IncomeTaxPerMonth, TIN = py.TIN, Year = py.Year, HasError = py.HasErrors, ErrorMessage = py.ErrorMessages, Email = py.Email, PhoneNumber = py.PhoneNumber, Address = py.Address, LGA = py.LGA, PayeeName = py.PayeeName }).ToList();

            int chunkSize = take;
            var dataSize = count;

            double pageSize = ((double)dataSize / (double)chunkSize);
            int pages = 0;

            if (pageSize < 1 && dataSize >= 1) { pages = 1; }
            else { pages = (int)Math.Ceiling(pageSize); }

            if(entity == null)
            { entity = record.TaxEntity; }


            return new DirectAssessmentReportVM
            {
                Amount = string.Format("{0:n}", record.Amount),
                PageSize = pages,
                Payees = payeeVM,
                Recipient = entity.Recipient,
                TIN = entity.TaxPayerIdentificationNumber,
                PhoneNumber = entity.PhoneNumber,
                Email = entity.Email,
                PayeeExcelReport = new PayeeExcelReport
                {
                    NumberOfValidRecords = count - errors,
                    NumberOfRecords = count,
                }
            };
        }


        public InvoiceGeneratedResponseExtn GenerateInvoice(CreateInvoiceModel invoiceModel, ref List<ErrorModel> errors)
        { invoiceModel.ApplySurcharge = true; return _coreInvoiceService.TryCreateInvoice(invoiceModel, ref errors); }


        /// <summary>
        /// Get invoice VM for make payment view
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>InvoiceGeneratedResponseExtn</returns>
        public InvoiceGeneratedResponseExtn GetInvoiceGeneratedResponseObjectForPaymentView(string invoiceNumber)
        {
            return _coreInvoiceService.GetInvoiceDetailsForPaymentView(invoiceNumber);
        }

        /// <summary>
        /// Get form details from the DB
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public List<FormControlViewModel> GetFormDetailsFromDB(int revenueHeadId, int categoryId)
        {
            return _coreFormService.GetRevenueHeadFormFields(revenueHeadId, categoryId).ToList();
        }


    }
}