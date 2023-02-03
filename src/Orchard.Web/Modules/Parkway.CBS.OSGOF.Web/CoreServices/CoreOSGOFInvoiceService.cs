using Orchard.Logging;
using Parkway.CBS.Core.Events.Contracts;
using Parkway.CBS.Core.HTTP.Handlers;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.OSGOF.Web.CoreServices.Contracts;
using Parkway.CBS.OSGOF.Web.CoreServices.Invoicing.Contracts;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.OSGOF.Web.CoreServices
{
    public class CoreOSGOFInvoiceService : CoreInvoiceService, ICoreOSGOFInvoiceService
    {
        private readonly ICoreRevenueHeadService _revenueHeadCoreService;
        private readonly IBillingScheduleManager<BillingSchedule> _scheduleRepository;
        private readonly IInvoiceManager<Invoice> _invoiceRepository;
        private readonly ITaxEntityCategoryManager<TaxEntityCategory> _taxEntityCategoryRepository;
        private readonly ICoreTaxPayerService _taxPayerService;

        private readonly IInvoiceCreatedEventHandler _invoiceEventHandler;
        private readonly IRevenueHeadStatisticsEventHandler _statsEventHandler;

        private readonly ICoreBillingService _billingService;
        private readonly IEnumerable<IOSGOFInvoiceGenerationType> _invoiceGenerationTypes;

        private readonly IAPIRequestManager<APIRequest> _apiRequestRepository;

        private readonly IInvoiceItemsManager<InvoiceItems> _invoiceItemsRepository;
        private readonly Lazy<ICoreFormService> _coreFormService;
        private readonly IFormControlRevenueHeadValueManager<FormControlRevenueHeadValue> _formValueRepo;


        public CoreOSGOFInvoiceService(IInvoicingService invoicingService, ICoreRevenueHeadService revenueHeadCoreService, IBillingScheduleManager<BillingSchedule> scheduleRepository, IInvoiceManager<Invoice> invoiceRepository, ITaxEntityCategoryManager<TaxEntityCategory> taxPayerCategoryRepository, ICoreTaxPayerService taxPayerService, IInvoiceCreatedEventHandler invoiceEventHandler,
                                    IRevenueHeadStatisticsEventHandler statsEventHandler, ICoreBillingService billingService, IEnumerable<IOSGOFInvoiceGenerationType> invoiceGenerationTypes, IAPIRequestManager<APIRequest> apiRequestRepository, ITransactionLogManager<TransactionLog> tranlogRepository, IInvoiceItemsManager<InvoiceItems> invoiceItemsRepository, IPaymentProviderValidationConstraintManager<PaymentProviderValidationConstraint> validationConstraintRepo, Lazy<ICoreFormService> coreFormService, IFormControlRevenueHeadValueManager<FormControlRevenueHeadValue> formValueRepo)
            : base(invoicingService, revenueHeadCoreService, scheduleRepository, invoiceRepository, taxPayerCategoryRepository, taxPayerService, invoiceEventHandler, statsEventHandler, billingService, invoiceGenerationTypes, apiRequestRepository, tranlogRepository, invoiceItemsRepository, validationConstraintRepo, coreFormService, formValueRepo)
        {
            _invoicingService = invoicingService;
            _revenueHeadCoreService = revenueHeadCoreService;
            Logger = NullLogger.Instance;
            _scheduleRepository = scheduleRepository;
            _invoiceRepository = invoiceRepository;
            _taxEntityCategoryRepository = taxPayerCategoryRepository;
            _taxPayerService = taxPayerService;
            _invoiceEventHandler = invoiceEventHandler;
            _statsEventHandler = statsEventHandler;
            _billingService = billingService;

            _invoiceGenerationTypes = invoiceGenerationTypes;
            _apiRequestRepository = apiRequestRepository;
            _invoiceItemsRepository = invoiceItemsRepository;

            _coreFormService = coreFormService;
            _formValueRepo = formValueRepo;
        }
    }
}