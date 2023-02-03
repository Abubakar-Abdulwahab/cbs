using Orchard.Logging;
using Parkway.CBS.Core.Events.Contracts;
using Parkway.CBS.Core.HTTP.Handlers.Invoicing;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.OSGOF.Web.CoreServices.Invoicing.Contracts;

namespace Parkway.CBS.OSGOF.Web.CoreServices.Invoicing
{
    public class OSGOFSelfAssessmentGeneration : SelfAssessmentInvoiceGeneration, IOSGOFInvoiceGenerationType
    {

        public OSGOFSelfAssessmentGeneration(IInvoiceManager<Invoice> invoiceRepository, IRevenueHeadStatisticsEventHandler statsEventHandler, IInvoicingService invoicingService, ITaxEntityAccountManager<TaxEntityAccount> taxEntityAccountReposirty, ITransactionLogManager<TransactionLog> transactionLogRepository, IAPIRequestManager<APIRequest> apiRequestRepository, IInvoiceItemsManager<InvoiceItems> invoiceItemsRepository, IFormControlRevenueHeadValueManager<FormControlRevenueHeadValue> formValueRepo) : base(invoiceRepository, statsEventHandler, invoicingService, taxEntityAccountReposirty, transactionLogRepository, apiRequestRepository, invoiceItemsRepository, formValueRepo)
        {
            Logger = NullLogger.Instance;
        }

    }
}