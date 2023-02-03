using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CoreReceiptUtilizationService : ICoreReceiptUtilizationService
    {
        private readonly IPAYEBatchRecordManager<PAYEBatchRecord> _payeBatchRecordRepo;
        private readonly IPAYEPaymentUtilizationManager<PAYEPaymentUtilization> _payePaymentUtilRepo;
        private readonly IPAYEReceiptManager<PAYEReceipt> _payeReceiptRepo;
        private readonly ITransactionLogManager<TransactionLog> _transactionLogRepo;
        private readonly ICorePAYEPaymentService _corePAYEPaymentService;
        private readonly IRevenueHeadManager<RevenueHead> _revHeadRepo;
        private readonly IPAYEBatchRecordInvoiceManager<PAYEBatchRecordInvoice> _payeBatchRecordInvoiceRepo;
        public ILogger Logger { get; set; }

        public CoreReceiptUtilizationService(IPAYEBatchRecordManager<PAYEBatchRecord> payeBatchRecordRepo, IPAYEPaymentUtilizationManager<PAYEPaymentUtilization> payePaymentUtilRepo, IPAYEReceiptManager<PAYEReceipt> payeReceiptRepo, ITransactionLogManager<TransactionLog> transactionLogRepo, ICorePAYEPaymentService corePAYEPaymentService, IRevenueHeadManager<RevenueHead> revHeadRepo, IPAYEBatchRecordInvoiceManager<PAYEBatchRecordInvoice> payeBatchRecordInvoiceRepo)
        {
            _payeBatchRecordRepo = payeBatchRecordRepo;
            _payePaymentUtilRepo = payePaymentUtilRepo;
            _payeReceiptRepo = payeReceiptRepo;
            _transactionLogRepo = transactionLogRepo;
            _corePAYEPaymentService = corePAYEPaymentService;
            _revHeadRepo = revHeadRepo;
            _payeBatchRecordInvoiceRepo = payeBatchRecordInvoiceRepo;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Get batch record with specified batch ref
        /// </summary>
        /// <param name="batchRef"></param>
        /// <returns></returns>
        public PAYEBatchRecordVM GetBatchRecordWithBatchRef(string batchRef)
        {
            try
            {
                return _payeBatchRecordRepo.GetBatchRecordWithRef(batchRef);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Get amount paid for batch with specified Id
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        public decimal GetBatchAmountPaidWithId(Int64 batchId)
        {
            try
            {
                return _payePaymentUtilRepo.GetBatchRecordAmountPaid(batchId);
            }
            catch (NoRecordFoundException) { throw; }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Get PAYE Receipt with specified receipt number
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PAYEReceiptVM GetPAYEReceiptVMWithNumber(string receiptNumber, long userId)
        {
            try
            {
                return _payeReceiptRepo.GetPAYEReceiptWithNumber(receiptNumber, userId);
            }
            catch (NoRecordFoundException) { throw; }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Apply receipt with specified receipt number to batch with specified batch ref
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <param name="batchRef"></param>
        /// <param name="userId"></param>
        public bool ApplyReceiptToBatch(string receiptNumber, string batchRef, long userId)
        {
            try
            {
                PAYEReceiptVM receipt = GetPAYEReceiptVMWithNumber(receiptNumber, userId);
                PAYEBatchRecordVM batch = _payeBatchRecordRepo.GetBatchRecordWithRef(batchRef);
                if (batch.PaymentCompleted) { return false; }
                _corePAYEPaymentService.ProcessPAYEPaymentForApplyingReceipt(receipt.TransactionLogs.ToList(), receipt, batch);
                return true;
            }
            catch (NoRecordFoundException) { throw; }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Get receipts utilized for batch record with the specified Id
        /// </summary>
        /// <param name="batchRecordId"></param>
        /// <returns></returns>
        public IEnumerable<PAYEReceiptVM> GetUtilizedReceiptsForBatch(long batchRecordId)
        {
            try
            {
                return _payePaymentUtilRepo.GetUtilizedReceiptsForBatchRecord(batchRecordId);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Get Revenue Head Id for PAYE Assessment
        /// </summary>
        /// <returns></returns>
        public int GetRevenueHeadIdForPAYE()
        {
            try
            {
                return _revHeadRepo.GetRevenueHeadDetailsForPaye().RevenueHead.Id;
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Get invoice number of unpaid invoice for batch with specified batch record id
        /// </summary>
        /// <param name="batchRecordId"></param>
        /// <returns></returns>
        public IEnumerable<string> GetUnpaidInvoiceNumberForBatch(Int64 batchRecordId)
        {
            try
            {
                return _payeBatchRecordInvoiceRepo.GetUnpaidInvoiceForBatchWithId(batchRecordId);
            }
            catch (Exception) { throw; }
        }
    }
}