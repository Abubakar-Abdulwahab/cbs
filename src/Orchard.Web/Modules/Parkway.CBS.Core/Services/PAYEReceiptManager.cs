using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Linq;

namespace Parkway.CBS.Core.Services
{
    public class PAYEReceiptManager : BaseManager<PAYEReceipt>, IPAYEReceiptManager<PAYEReceipt>
    {
        private readonly IRepository<PAYEReceipt> _repository;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public PAYEReceiptManager(IRepository<PAYEReceipt> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
            _repository = repository;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Move Receipt from TransactionLog table to PAYE Receipt table
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="amountUtilized"></param>
        /// <param name="batchRecordId"></param>
        public void AddPAYEReceiptUtilizations(long invoiceId, decimal amountUtilized, long batchRecordId, int utilizationStatus)
        {
            try
            {
                string queryText = $"INSERT INTO Parkway_CBS_Core_PAYEReceipt (TransactionLog_Id, Receipt_Id, UtilizedAmount, UtilizationStatusId, CreatedAtUtc, UpdatedAtUtc) OUTPUT inserted.Id as PAYEReceipt_Id, :batchRecord_Id as PAYEBatchRecord_Id, :utilizedAmount as UtilizedAmount, CURRENT_TIMESTAMP as CreatedAtUtc, CURRENT_TIMESTAMP as UpdatedAtUtc INTO Parkway_CBS_Core_PAYEPaymentUtilization (PAYEReceipt_Id, PAYEBatchRecord_Id, UtilizedAmount, CreatedAtUtc, UpdatedAtUtc) SELECT tLog.Id, tLog.Receipt_Id, :utilizedAmount, :status, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP FROM Parkway_CBS_Core_TransactionLog tLog LEFT JOIN Parkway_CBS_Core_PAYEReceipt py ON tLog.Id = py.TransactionLog_Id WHERE py.TransactionLog_Id IS NULL and tLog.Invoice_Id = :invoiceId and tLog.TypeID != :typeId";

                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("invoiceId", invoiceId);
                query.SetParameter("batchRecord_Id", batchRecordId);
                query.SetParameter("utilizedAmount", amountUtilized);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("status", utilizationStatus);
                query.ExecuteUpdate();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                RollBackAllTransactions();
                throw;
            }
        }

        /// <summary>
        /// Move Receipt from TransactionLog table to PAYE Receipt table
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="amountUtilized"></param>
        public void AddPAYEReceiptWithoutUtilizations(long invoiceId, decimal amountUtilized, int utilizationStatus)
        {
            try
            {
                string queryText = $"INSERT INTO Parkway_CBS_Core_PAYEReceipt (TransactionLog_Id, UtilizedAmount, UtilizationStatusId, CreatedAtUtc, UpdatedAtUtc) SELECT tLog.Id, :utilizedAmount, :status, :date, :date FROM Parkway_CBS_Core_TransactionLog tLog LEFT JOIN Parkway_CBS_Core_PAYEReceipt py ON tLog.Id = py.TransactionLog_Id WHERE py.TransactionLog_Id IS NULL and tLog.Invoice_Id = :invoiceId and tLog.TypeID != :typeId";

                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("invoiceId", invoiceId);
                query.SetParameter("utilizedAmount", amountUtilized);
                query.SetParameter("typeId", (int)PaymentType.Bill);
                query.SetParameter("status", utilizationStatus);
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

        /// <summary>
        /// Get PAYE Receipt with specified receipt number
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PAYEReceiptVM GetPAYEReceiptWithNumber(string receiptNumber, long userId)
        {
            try
            {
                return _transactionManager.GetSession().Query<PAYEReceipt>().Where(x => x.Receipt.ReceiptNumber == receiptNumber && x.Receipt.Invoice.TaxPayer.Id == userId)
                    .Select(x => new PAYEReceiptVM
                    {
                        Id = x.Id,
                        TotalAmount = x.GetReceiptAmount(),
                        UtilizedAmount = x.UtilizedAmount(),
                        Status = x.UtilizationStatusId,
                        ReceiptNumber = x.Receipt.ReceiptNumber,
                        InvoiceNumber = x.Receipt.Invoice.InvoiceNumber,
                        ReceiptId = x.Receipt.Id,
                        TransactionLogs = x.PAYEReceiptTransactionLog.Select(t => new TransactionLogVM { TransactionLogId = t.TransactionLog.Id }).ToList()
                    }).ToList().First();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, $"Could not get PAYE Receipt with receipt number {receiptNumber}. Exception message {exception.Message}");
                throw new NoRecordFoundException($"Could not get PAYE Receipt {receiptNumber} with available fund.");
            }
        }

        /// <summary>
        /// Update Utilization Status for PAYE Receipt with the specified Utilization Status
        /// </summary>
        /// <param name="status"></param>
        /// <param name="payeReceiptId"></param>
        /// <returns></returns>
        public bool UpdatePAYEReceiptUtilizationStatus(PAYEReceiptUtilizationStatus status, long payeReceiptId)
        {
            try
            {
                string queryText = $"UPDATE Parkway_CBS_Core_PAYEReceipt SET UtilizationStatusId = :status, UpdatedAtUtc = :date WHERE Id = :id";

                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("status", (int)status);
                query.SetParameter("id", payeReceiptId);
                query.SetParameter("date", DateTime.Now.ToLocalTime());
                query.ExecuteUpdate();

                return true;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                RollBackAllTransactions();
                throw;
            }
        }


        public PAYEReceiptVM GetPAYEReceipt(Int64 id)
        {
            try
            {
                var result = _transactionManager.GetSession().Query<PAYEReceipt>().Where(x => x.Id == id);
                if (result == null || !result.Any()) { return null; }
                return result.Select(x => new PAYEReceiptVM
                {
                    Id = x.Id,
                    TotalAmount = x.GetReceiptAmount(),
                    UtilizedAmount = x.UtilizedAmount(),
                    Status = x.UtilizationStatusId,
                    ReceiptNumber = x.Receipt.ReceiptNumber,
                    InvoiceNumber = x.Receipt.Invoice.InvoiceNumber,
                }).First();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, $"Could not get PAYE Receipt with receipt id {id}. Exception message {exception.Message}");
                throw;
            }
        }

    }
}