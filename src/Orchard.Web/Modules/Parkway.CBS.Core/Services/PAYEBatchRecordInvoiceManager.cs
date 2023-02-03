using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Core.Services
{
    public class PAYEBatchRecordInvoiceManager : BaseManager<PAYEBatchRecordInvoice>, IPAYEBatchRecordInvoiceManager<PAYEBatchRecordInvoice>
    {
        private readonly IRepository<PAYEBatchRecordInvoice> _repository;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public PAYEBatchRecordInvoiceManager(IRepository<PAYEBatchRecordInvoice> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
            _repository = repository;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Get the batch record id using the invoice id
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns>List{PAYEBatchRecordVM}</returns>
        /// <exception cref="Exception"></exception>
        public List<PAYEBatchRecordVM> GetBatchRecordsWithIncompletePayment(long invoiceId)
        {
            try
            {
                return _transactionManager.GetSession().Query<PAYEBatchRecordInvoice>()
                    .Where(x => ((x.Invoice.Id == invoiceId) && (!x.PAYEBatchRecord.PaymentCompleted)))
                    .Select(x=> new PAYEBatchRecordVM
                    {
                        BatchRecordId = x.PAYEBatchRecord.Id,
                        RevenueHeadSurCharge = x.PAYEBatchRecord.RevenueHeadSurCharge,
                        TotalIncomeTaxForPayesInSchedule = x.PAYEBatchRecord.Payees.Sum(y => y.IncomeTaxPerMonth)
                    }).ToList();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, $"Batch record not found for invoice id {invoiceId}. Exception Message {exception.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get invoice number of unpaid invoice for batch with specified batch record id
        /// </summary>
        /// <param name="batchRecordId"></param>
        /// <returns></returns>
        public IEnumerable<string> GetUnpaidInvoiceForBatchWithId(Int64 batchRecordId)
        {
            try
            {
                return _transactionManager.GetSession().Query<PAYEBatchRecordInvoice>()
                    .Where(x => x.PAYEBatchRecord.Id == batchRecordId && x.Invoice.Status == (int)Models.Enums.InvoiceStatus.Unpaid)
                    .Select(x => x.Invoice.InvoiceNumber).ToFuture();
            }catch(Exception exception)
            {
                Logger.Error(exception,$"Error checking if batch with id {batchRecordId} has any unpaid invoices. Exception - {exception.Message}");
                throw;
            }
        }


    }
}