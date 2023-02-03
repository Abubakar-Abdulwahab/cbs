using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CorePAYEPaymentService : ICorePAYEPaymentService
    {
        private readonly IPAYEPaymentUtilizationManager<PAYEPaymentUtilization> _payePaymentUtilizationManager;
        private readonly IPAYEReceiptTransactionLogManager<PAYEReceiptTransactionLog> _pAYEReceiptTransactionLogRepo;
        private readonly IPAYEBatchRecordManager<PAYEBatchRecord> _payeBatchRecordRepo;
        private readonly IPAYEReceiptManager<PAYEReceipt> _payeReceiptManager;
        private readonly IPAYEBatchItemReceiptManager<PAYEBatchItemReceipt> _payeBatchItemReceiptManager;
        private readonly IPAYEBatchRecordInvoiceManager<PAYEBatchRecordInvoice> _payeBatchRecordInvoiceManager;
        private readonly IRevenueHeadManager<RevenueHead> _revenueHeadRepository;
        public ILogger Logger { get; set; }


        public CorePAYEPaymentService(IPAYEPaymentUtilizationManager<PAYEPaymentUtilization> payePaymentUtilizationManager, IPAYEReceiptManager<PAYEReceipt> payeReceiptManager, IPAYEBatchItemReceiptManager<PAYEBatchItemReceipt> payeBatchItemReceiptManager, IPAYEBatchRecordInvoiceManager<PAYEBatchRecordInvoice> payeBatchRecordInvoiceManager, IRevenueHeadManager<RevenueHead> revenueHeadRepository, IPAYEReceiptTransactionLogManager<PAYEReceiptTransactionLog> pAYEReceiptTransactionLogRepo, IPAYEBatchRecordManager<PAYEBatchRecord> payeBatchRecordRepo)
        {
            Logger = NullLogger.Instance;
            _payePaymentUtilizationManager = payePaymentUtilizationManager;
            _payeReceiptManager = payeReceiptManager;
            _payeBatchItemReceiptManager = payeBatchItemReceiptManager;
            _payeBatchRecordInvoiceManager = payeBatchRecordInvoiceManager;
            _revenueHeadRepository = revenueHeadRepository;
            _pAYEReceiptTransactionLogRepo = pAYEReceiptTransactionLogRepo;
            _payeBatchRecordRepo = payeBatchRecordRepo;
        }


        /// <summary>
        /// Get the reveneue head for PAYE assessment
        /// </summary>
        /// <returns>int</returns>
        public int GetPAYERevenueHeadId()
        {
            int? payeeId = _revenueHeadRepository.Get(r => r.IsPayeAssessment)?.Id;
            if (!payeeId.HasValue) { payeeId = 0; }
            return payeeId.Value;
        }


        /// <summary>
        /// This method is used to do payment processing
        /// for PAYE assessments when payment has been done
        /// </summary>
        /// <param name="transactionLogs">list of transactions that contain the designated PAYE revenue head items</param>
        /// <param name="invoiceId">invoice Id</param>
        /// <param name="receiptId">receipt Id</param>
        public void ProcessPAYEPayment(List<TransactionLogVM> transactionLogs, long invoiceId, Int64 receiptId)
        {
            try
            {
                PAYEReceipt payeReceipt = AddEntryForPAYEReceipt(receiptId);
                //add paye receipt transcation
                AddTransactionLogEntries(payeReceipt, receiptId, transactionLogs);
                //Get the batch record for this invoice number
                //we check the paye batch record invoice table for the batch record that has this invoice number
                List<PAYEBatchRecordVM> batchRecords = _payeBatchRecordInvoiceManager.GetBatchRecordsWithIncompletePayment(invoiceId);
                //if batch record is null, this means we have no batch record attached to this invoice
                if (batchRecords == null || !batchRecords.Any()) { return; }
                DoWorkForBatchRecords(payeReceipt, transactionLogs.Sum(t => t.AmountPaid), batchRecords);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Unable to process PAYE payment for invoice ID {invoiceId}");
                _payePaymentUtilizationManager.RollBackAllTransactions();
                throw;
            }
        }


        /// <summary>
        /// This method is used to do payment processing
        /// for PAYE assessments with batch that do not have an invoice attached
        /// </summary>
        /// <param name="transactionLogs">Transaction log for the PAYE Receipt</param>
        /// <param name="receipt">PAYE receipt</param>
        /// <param name="batch">Batch which the receipt will be applied to</param>
        public void ProcessPAYEPaymentForApplyingReceipt(List<TransactionLogVM> transactionLogs, PAYEReceiptVM receipt, PAYEBatchRecordVM batch)
        {
            try
            {
                PAYEReceipt payeReceipt = new PAYEReceipt { Id = receipt.Id, Receipt = new Receipt { Id = receipt.ReceiptId } };
                //Get the batch record with this batch ref
                List<PAYEBatchRecordVM> batchRecords = new List<PAYEBatchRecordVM> { batch };
                //if batch record is null, this means we have no batch record with the specified batch ref
                DoWorkForBatchRecords(payeReceipt, receipt.AvailableAmount, batchRecords);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Unable to process PAYE payment for PAYE Receipt with receipt number {receipt.ReceiptNumber}");
                _payePaymentUtilizationManager.RollBackAllTransactions();
                throw;
            }
        }


        private void DoWorkForBatchRecords(PAYEReceipt payeReceipt, decimal availableInvoiceAmount, List<PAYEBatchRecordVM> batchRecords)
        {
            decimal batchAmountPaid = 0.0m;
            decimal expectedBatchAmount = 0.00m;

            foreach (PAYEBatchRecordVM batchRecord in batchRecords)
            {
                if (availableInvoiceAmount <= 0.00m) { break; }
                //Total sum of the items on the batch, plus the surcharge on the paye batch record
                expectedBatchAmount = batchRecord.TotalIncomeTaxForPayesInSchedule + batchRecord.RevenueHeadSurCharge;
                //Get total amount paid so far for the batch in the utilization table
                batchAmountPaid = _payePaymentUtilizationManager.GetBatchRecordAmountPaid(batchRecord.BatchRecordId);

                decimal amountDueOnBatch = expectedBatchAmount - batchAmountPaid;

                //here we check that the amount due is less than or equals to the amount paid
                if (amountDueOnBatch <= availableInvoiceAmount)
                {
                    //since the batch amount has been fully paid for we add the PAYE items to the PAYE items receipt table
                    _payeBatchItemReceiptManager.AddPAYEReceiptItems(batchRecord.BatchRecordId);
                    AddEntryIntoUtilizationTable(payeReceipt.Id, amountDueOnBatch, batchRecord.BatchRecordId);
                    //update batch record to payment completed
                    UpdateBatchRecordToPaymentCompleted(batchRecord.BatchRecordId, true);
                    availableInvoiceAmount -= amountDueOnBatch;
                    //Update receipt utilization status
                    var status = availableInvoiceAmount == 0.00m ? Models.Enums.PAYEReceiptUtilizationStatus.FullyUtilized : Models.Enums.PAYEReceiptUtilizationStatus.PartlyUtilized;
                    _payeReceiptManager.UpdatePAYEReceiptUtilizationStatus(status, payeReceipt.Id);
                }
                else if (amountDueOnBatch > availableInvoiceAmount)
                {
                    AddEntryIntoUtilizationTable(payeReceipt.Id, availableInvoiceAmount, batchRecord.BatchRecordId);
                    availableInvoiceAmount = 0.00m;
                    //Update receipt utilization status
                    _payeReceiptManager.UpdatePAYEReceiptUtilizationStatus(Models.Enums.PAYEReceiptUtilizationStatus.FullyUtilized, payeReceipt.Id);
                }
            }
        }



        private void UpdateBatchRecordToPaymentCompleted(long batchRecordId, bool completed)
        {
            _payeBatchRecordRepo.SetPaymentCompletedValue(batchRecordId, completed);
        }


        /// <summary>
        /// Add utilization entry
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="amountUtilized"></param>
        /// <param name="batchRecordId"></param>
        /// <param name="fullyUtilized"></param>
        private void AddEntryIntoUtilizationTable(long payeReceiptId, decimal amountUtilized, long batchRecordId)
        {
            PAYEPaymentUtilization utilization = new PAYEPaymentUtilization { PAYEBatchRecord = new PAYEBatchRecord { Id = batchRecordId }, PAYEReceipt = new PAYEReceipt { Id = payeReceiptId }, UtilizedAmount = amountUtilized };
            if (!_payePaymentUtilizationManager.Save(utilization)) { throw new CouldNotSaveRecord(); }
        }


        /// <summary>
        /// Add transaction log entries for the paye receipt
        /// </summary>
        /// <param name="pAYEReceipt"></param>
        /// <param name="receiptId"></param>
        /// <param name="transactionLogs"></param>
        /// <exception cref="CouldNotSaveRecord"></exception>
        private void AddTransactionLogEntries(PAYEReceipt pAYEReceipt, Int64 receiptId, List<TransactionLogVM> transactionLogs)
        {
            List<PAYEReceiptTransactionLog> logs = new List<PAYEReceiptTransactionLog> { };
            foreach (var item in transactionLogs)
            {
                logs.Add(new PAYEReceiptTransactionLog { PAYEReceipt = pAYEReceipt, Receipt = new Receipt { Id = receiptId }, TransactionLog = new TransactionLog { Id = item.TransactionLogId } });
            }
            if (_pAYEReceiptTransactionLogRepo.SaveBundleUnCommitStatelessWithErrors(logs) != -1)
            { throw new CouldNotSaveRecord("Error saving PAYE receipt transaction log"); }
        }


        /// <summary>
        /// Add PAYEReceipt entry
        /// </summary>
        /// <param name="receiptId"></param>
        /// <returns>PAYEReceipt</returns>
        /// <exception cref="CouldNotSaveRecord"></exception>
        private PAYEReceipt AddEntryForPAYEReceipt(long receiptId)
        {
            PAYEReceipt PAYEReceipt = new PAYEReceipt { Receipt = new Receipt { Id = receiptId } };
            if (!_payeReceiptManager.Save(PAYEReceipt)) { throw new CouldNotSaveRecord(); }
            return PAYEReceipt;
        }

    }
}