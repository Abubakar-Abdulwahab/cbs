using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Parkway.CBS.Core.Events.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;

namespace Parkway.CBS.Core.Events
{
    public class InvoiceCreatedEventHandler : IInvoiceCreatedEventHandler
    {
        public ILogger Logger { get; set; }
        private readonly ITransactionManager _transactionManager;
        private readonly IInvoiceManager<Invoice> _invoiceManager;

        public InvoiceCreatedEventHandler(IOrchardServices orchardServices, IInvoiceManager<Invoice> invoiceManager)
        {
            Logger = NullLogger.Instance;
            _transactionManager = orchardServices.TransactionManager;
            _invoiceManager = invoiceManager;
        }
    }

    public class InvoiceContext
    {
        public Invoice Invoice { get; set; }
    }


    public class InvoicePaymentNoficationContext
    {
        public string InvoiceNumber { get; set; }

        public decimal AmountPaid { get; set; }

        public DateTime PaymentDate { get; set; }

        public bool PartPaid { get; set; }
    }
}