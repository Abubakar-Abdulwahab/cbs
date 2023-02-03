using System;
using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using NHibernate.Criterion;
using System.Linq;
using System.Collections.Generic;
using NHibernate.Linq;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Exceptions;

namespace Parkway.CBS.Core.Services
{
    public class ReceiptManager : BaseManager<Receipt>, IReceiptManager<Receipt>
    {
        private readonly IRepository<Receipt> _receiptRepository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public ReceiptManager(IRepository<Receipt> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _receiptRepository = repository;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
        }



        /// <summary>
        /// Get receipt Id for this receipt number
        /// </summary>
        /// <returns>Int64</returns>
        public Int64 GetReceiptId(string receiptNumber)
        {
            return _transactionManager.GetSession()
                  .Query<Receipt>().Where(r => r.ReceiptNumber == receiptNumber).Select(r => r.Id).FirstOrDefault();
        }

        /// <summary>
        /// Get the VM for a receipt
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <returns>ReceiptViewModel</returns>
        public ReceiptViewModel GetReceiptDetails(string receiptNumber)
        {
            var modelFuture = _transactionManager.GetSession()
                  .Query<Receipt>().Where(r => r.ReceiptNumber == receiptNumber)
                  .Select(r => new ReceiptViewModel
                  {
                      ReceiptNumber = r.ReceiptNumber,
                      TaxPayePhoneNumber = r.Invoice.TaxPayer.PhoneNumber,
                      AmountDue = r.Invoice.InvoiceAmountDueSummary.AmountDue,
                      ExternalRef = r.Invoice.ExternalRefNumber,
                      TaxPayerAddress = r.Invoice.TaxPayer.Address,
                      TaxPayerEmail = r.Invoice.TaxPayer.Email,
                      TaxPayerId = r.Invoice.TaxPayer.PayerId,
                      InvoiceNumber = r.Invoice.InvoiceNumber,
                      TaxPayerName = r.Invoice.TaxPayer.Recipient,
                      TaxPayerTIN = r.Invoice.TaxPayer.TaxPayerIdentificationNumber,
                      InvoiceStatus = (InvoiceStatus)r.Invoice.Status,
                      PaymentDate = r.CreatedAtUtc.ToString("dd/MM/yyyy"),
                      MDAName = r.Invoice.Mda.Name
                  }).ToFuture();

            var modelListFuture = _transactionManager.GetSession()
                .Query<Receipt>().Where(r => r.ReceiptNumber == receiptNumber).Select(r => r.TransactionLogs.Select(t => new TransactionLogInvoiceDetails
                {
                    AmountPaid = Math.Round(t.AmountPaid, 2) + 00.00m,
                    MDACode = t.MDA.Code,
                    MDAName = t.MDA.Name,
                    RevenueHeadCode = t.RevenueHead.Code,
                    RevenueHeadName = t.RevenueHead.Name
                })).ToFuture();

            var model = modelFuture.SingleOrDefault();
            if (model == null) { throw new NoRecordFoundException("404 for receipt " + receiptNumber); }
            model.TransactionLogInvoiceDetails = modelListFuture.FirstOrDefault();

            return model;
        }

    }
}