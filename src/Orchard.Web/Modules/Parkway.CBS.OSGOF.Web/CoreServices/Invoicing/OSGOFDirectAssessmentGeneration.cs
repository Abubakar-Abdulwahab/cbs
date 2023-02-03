using Orchard.Logging;
using Parkway.CBS.Core.Events.Contracts;
using Parkway.CBS.Core.HTTP.Handlers.Invoicing;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.OSGOF.Web.CoreServices.Invoicing.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.OSGOF.Web.CoreServices.Invoicing
{
    public class OSGOFDirectAssessmentGeneration : DirectAssessmentInvoiceGeneration, IOSGOFInvoiceGenerationType
    {
        //public BillingType InvoiceGenerationType => BillingType.DirectAssessment;

        private readonly IDirectAssessmentBatchRecordManager<DirectAssessmentBatchRecord> _directAssessmentRecordRepository;
        private readonly IDirectAssessmentPayeeManager<DirectAssessmentPayeeRecord> _directAssessmentPayeeRepository;
        //private readonly IAPIRequestManager<APIRequest> _apiRequestRepository;

        public OSGOFDirectAssessmentGeneration(IDirectAssessmentBatchRecordManager<DirectAssessmentBatchRecord> directAssessmentRecordRepository, IDirectAssessmentPayeeManager<DirectAssessmentPayeeRecord> directAssessmentPayeeRepository, IInvoiceManager<Invoice> invoiceRepository, IRevenueHeadStatisticsEventHandler statsEventHandler, IInvoicingService invoicingService, ITaxEntityAccountManager<TaxEntityAccount> taxEntityAccountReposirty, ITransactionLogManager<TransactionLog> transactionLogRepository, IAPIRequestManager<APIRequest> apiRequestRepository, IInvoiceItemsManager<InvoiceItems> invoiceItemsRepository, IFormControlRevenueHeadValueManager<FormControlRevenueHeadValue> formValueRepo) : base(directAssessmentRecordRepository, directAssessmentPayeeRepository, invoiceRepository, statsEventHandler, invoicingService, taxEntityAccountReposirty, transactionLogRepository, apiRequestRepository, invoiceItemsRepository, formValueRepo)
        {
            _directAssessmentRecordRepository = directAssessmentRecordRepository;
            Logger = NullLogger.Instance;
            _directAssessmentPayeeRepository = directAssessmentPayeeRepository;
            //_apiRequestRepository = apiRequestRepository;
        }
    }
}