using Parkway.CBS.Core.Events.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.Logging;
using Orchard;
using Orchard.Data;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Services;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using Parkway.CBS.Core.Exceptions;

namespace Parkway.CBS.Core.Events
{
    public class RevenueHeadStatisticsEventHandler : IRevenueHeadStatisticsEventHandler
    {
        public ILogger Logger { get; set; }
        private readonly ITransactionManager _transactionManager;
        private readonly IOrchardServices _orchardServices;
        private readonly IInvoiceStatisticsManager<Stats> _invoiceStatsRepository;

        public RevenueHeadStatisticsEventHandler(IOrchardServices orchardServices, IInvoiceStatisticsManager<Stats> invoiceStatsRepository)
        {
            Logger = NullLogger.Instance;
            _transactionManager = orchardServices.TransactionManager;
            _invoiceStatsRepository = invoiceStatsRepository;
        }

        /// <summary>
        /// Event handler for when invoices have been created. We either update or add to the statistics table
        /// </summary>
        /// <param name="context">StatsContext</param>
        public Stats InvoiceAdded(StatsContext context)
        {
            var invoice = context.Invoice;
            var revenueHead = context.RevenueHead;
            var mda = context.MDA;
            var category = context.TaxEntityCategory;
            bool alreadyPaidFor = context.Paid;

            DateTime duePeriod = GetDuePeriod(invoice.DueDate);

            Stats revenueHeadStats = null;
            revenueHeadStats = _invoiceStatsRepository.GetStats(revenueHead, duePeriod, category);

            if (revenueHeadStats == null)
            { revenueHeadStats = SaveStats(context, duePeriod); }
            else
            {
                revenueHeadStats.AmountExpected += invoice.Amount;
                revenueHeadStats.NumberOfInvoicesSent += 1;
            }

            //check if invoice has already been paid for
            if (alreadyPaidFor)
            {
                revenueHeadStats.AmountPaid += invoice.Amount;
                revenueHeadStats.NumberOfInvoicesPaid += 1;
            }
            return revenueHeadStats;
        }


        /// <summary>
        /// Save stats
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Stats</returns>
        private Stats SaveStats(StatsContext context, DateTime duePeriod)
        {
            Invoice invoice = context.Invoice;
            RevenueHead revenueHead = context.RevenueHead;
            MDA mda = context.MDA;
            TaxEntityCategory category = context.TaxEntityCategory;

            Stats revenueHeadStats = new Stats { RevenueHead = revenueHead, Mda = mda, DueDate = duePeriod, TaxEntityCategory = category, StatsQueryConcat = string.Format("{0}|{1}|{2}", duePeriod.ToString("dd'/'MM'/'yyyy"), revenueHead.Id, category.Id) };

            revenueHeadStats.AmountExpected += invoice.Amount;
            revenueHeadStats.NumberOfInvoicesSent += 1;

            if (!_invoiceStatsRepository.Save(revenueHeadStats))
            {
                Logger.Error(string.Format("Error saving stats {0}", revenueHead.Id));
                throw new CouldNotSaveStatsException(string.Format("Could not save stats for revenue head {0} and invoice number", revenueHead.Id, invoice.Id));
            }
            return revenueHeadStats;
        }


        /// <summary>
        /// Get due date period for stats
        /// </summary>
        /// <param name="invoiceDueDate"></param>
        /// <returns>DateTime</returns>
        private DateTime GetDuePeriod(DateTime invoiceDueDate)
        {
            //month this invoice is due
            //so what we do here is group invoices by due date for that month and year
            return new DateTime(invoiceDueDate.Year, invoiceDueDate.Month, DateTime.DaysInMonth(invoiceDueDate.Year, invoiceDueDate.Month));
        }


        /// <summary>
        /// Update the stats table when an invoice payment notification has been received
        /// </summary>
        /// <param name="context"></param>
        public void InvoicePaymentNotification(StatsContextUpdate context)
        {
            try
            {
                //first we get the invoice stat
                Invoice invoice = context.Invoice;
                RevenueHead revenueHead = context.RevenueHead;
                MDA mda = context.MDA;
                TaxEntityCategory category = context.TaxEntityCategory;
                var revenueHeadStats = _invoiceStatsRepository.GetStats(revenueHead, GetDuePeriod(invoice.DueDate), category);
                //if stats is null, that is for some reason the stats were not created, lets create it
                if (revenueHeadStats == null) { revenueHeadStats = SaveStats(context, GetDuePeriod(invoice.DueDate)); }

                revenueHeadStats.AmountPaid += context.AmountPaid;
                if (!context.PartPayment) { revenueHeadStats.NumberOfInvoicesPaid += 1; }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error at InvoicePaymentNotification MSG: " + exception.Message);
                throw;
            }
        }
    }
}