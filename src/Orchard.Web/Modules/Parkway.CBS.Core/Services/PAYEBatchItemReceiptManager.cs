using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Services
{
    public class PAYEBatchItemReceiptManager : BaseManager<PAYEBatchItemReceipt>, IPAYEBatchItemReceiptManager<PAYEBatchItemReceipt>
    {
        private readonly IRepository<PAYEBatchItemReceipt> _repository;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public PAYEBatchItemReceiptManager(IRepository<PAYEBatchItemReceipt> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
            _repository = repository;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Get the VM for a receipt
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <returns>ReceiptViewModel</returns>
        public PAYEBatchItemReceiptViewModel GetReceiptDetails(string receiptNumber)
        {
            return _transactionManager.GetSession()
                  .Query<PAYEBatchItemReceipt>().Where(r => r.ReceiptNumber == receiptNumber)
                  .Select(r => new PAYEBatchItemReceiptViewModel
                  {
                      ReceiptNumber = r.ReceiptNumber,
                      TaxPayePhoneNumber = r.PAYEBatchItem.TaxEntity.PhoneNumber,
                      TaxAmountPaid = r.PAYEBatchItem.IncomeTaxPerMonth,
                      GrossAnnual = r.PAYEBatchItem.GrossAnnual,
                      TaxPayerAddress = r.PAYEBatchItem.TaxEntity.Address,
                      TaxPayerEmail = r.PAYEBatchItem.TaxEntity.Email,
                      TaxPayerId = r.PAYEBatchItem.TaxEntity.PayerId,
                      Year = r.PAYEBatchItem.Year,
                      TaxPayerName = r.PAYEBatchItem.TaxEntity.Recipient,
                      TaxPayerTIN = r.PAYEBatchItem.TaxEntity.TaxPayerIdentificationNumber,
                      PaymentDate = r.CreatedAtUtc.ToString("dd/MM/yyyy"),
                      Month = (Months)r.PAYEBatchItem.Month
                  }).SingleOrDefault();
        }

        /// <summary>
        /// Create receipt items for batch items for a particular batch record id
        /// </summary>
        /// <param name="batchRecordId"></param>
        public void AddPAYEReceiptItems(long batchRecordId)
        {
            try
            {
                string queryText = $"INSERT INTO Parkway_CBS_Core_PAYEBatchItemReceipt (PAYEBatchItem_Id, CreatedAtUtc, UpdatedAtUtc) SELECT pybi.Id, :date, :date FROM Parkway_CBS_Core_PAYEBatchItems pybi WHERE pybi.PAYEBatchRecord_Id = :batchRecordId";

                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("batchRecordId", batchRecordId);
                query.SetParameter("date", DateTime.Now.ToLocalTime());
                query.ExecuteUpdate();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                RollBackAllTransactions();
                throw;
            }
        }

    }
}