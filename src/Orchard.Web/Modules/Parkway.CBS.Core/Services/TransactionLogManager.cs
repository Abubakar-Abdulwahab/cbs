using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Linq;
using Parkway.CBS.Core.Models.Enums;
using NHibernate.Linq;
using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using NHibernate.Criterion;
using NHibernate.Transform;
using System.Collections.Generic;

namespace Parkway.CBS.Core.Services
{
    public class TransactionLogManager : BaseManager<TransactionLog>, ITransactionLogManager<TransactionLog>
    {
        private readonly IRepository<TransactionLog> _transactionLogRepository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public TransactionLogManager(IRepository<TransactionLog> transactionLogRepository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(transactionLogRepository, user, orchardServices)
        {
            _transactionManager = orchardServices.TransactionManager;
            _transactionLogRepository = transactionLogRepository;
            _orchardServices = orchardServices;
            _user = user;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// this gets the total amount paid for the invoice.
        /// <para>Includes credits and debits</para>
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>decimal</returns>
        public decimal GetSumOfAmountPaidForInvoice(string invoiceNumber)
        {
            try
            {
                if (invoiceNumber.Length < 10)
                {
                    return _transactionManager.GetSession().Query<TransactionLog>()
                            .Where(t => (t.Invoice.NAGISInvoiceNumber == invoiceNumber) && (t.TypeID != (int)PaymentType.Bill)).Sum(t => (decimal?)t.AmountPaid) ?? 0;
                }
                return _transactionManager.GetSession().Query<TransactionLog>()
                            .Where(t => (t.Invoice.InvoiceNumber == invoiceNumber) && (t.TypeID != (int)PaymentType.Bill)).Sum(t => (decimal?)t.AmountPaid) ?? 0;
            }
            catch (Exception exception)
            { Logger.Error(exception, exception.Message); throw; }
        }


        /// <summary>
        /// Get transaction log with retrieval reference number
        /// </summary>
        /// <param name="transactionRef"></param>
        /// <param name="paymentProviderId"></param>
        /// <param name="channel"></param>
        /// <returns>TransactionLogGroup</returns>
        public TransactionLogGroup GetGroupedTransactionLogByRetrievalReferenceNumber(string invoiceNumber, string retrievalReferenceNumber, int paymentProviderId, PaymentChannel channel)
        {
            return _transactionManager.GetSession().CreateCriteria<TransactionLog>()
               .Add(Restrictions.Eq("TypeID", (int)PaymentType.Credit))
               .Add(Restrictions.Eq("PaymentProvider", (int)paymentProviderId))
               .Add(Restrictions.Eq("RetrievalReferenceNumber", retrievalReferenceNumber))
               .Add(Restrictions.Eq("InvoiceNumber", invoiceNumber))
               .Add(Restrictions.Eq("Channel", (int)channel))
               .SetProjection(Projections.ProjectionList()
                   .Add(Projections.Property<TransactionLog>(x => (x.RetrievalReferenceNumber)), "RetrievalReferenceNumber")
                   .Add(Projections.Property<TransactionLog>(x => (x.PaymentReference)), "PaymentReference")
                   .Add(Projections.Property<TransactionLog>(x => (x.ThirdPartyReceiptNumber)), "ThirdPartyReceiptNumber")
                   .Add(Projections.Property<TransactionLog>(x => (x.Invoice.Id)), "InvoiceId")
                   .Add(Projections.Property<TransactionLog>(x => (x.TaxEntity.Id)), "TaxEntityId")
                   .Add(Projections.Property<TransactionLog>(x => (x.ReceiptNumber)), "ReceiptNumber")
                   .Add(Projections.Sum<TransactionLog>(t => t.AmountPaid), "TotalAmountPaid")
                   .Add(Projections.Group<TransactionLog>(t => t.RetrievalReferenceNumber))
                   .Add(Projections.Group<TransactionLog>(t => t.PaymentReference))
                   .Add(Projections.Group<TransactionLog>(t => t.ThirdPartyReceiptNumber))
                   .Add(Projections.Group<TransactionLog>(t => t.Invoice.Id))
                   .Add(Projections.Group<TransactionLog>(t => t.TaxEntity.Id))
                   .Add(Projections.Group<TransactionLog>(t => t.ReceiptNumber))
           ).SetResultTransformer(Transformers.AliasToBean<TransactionLogGroup>())
           .List<TransactionLogGroup>().SingleOrDefault();
        }


        /// <summary>
        /// Get details for a particular payment log Id for channel
        /// </summary>
        /// <param name="paymentLogId"></param>
        /// <param name="provider"></param>
        /// <returns>TransactionLogGroup</returns>
        public TransactionLogGroup GetGroupedTransactionLogByPaymentLogId(string paymentLogId, PaymentProvider provider)
        {
            return _transactionManager.GetSession().CreateCriteria<TransactionLog>()
                .Add(Restrictions.Eq("TypeID", (int)PaymentType.Credit))
                .Add(Restrictions.Eq("PaymentProvider", (int)provider))
                .Add(Restrictions.Eq("PaymentLogId", paymentLogId))
                .SetProjection(Projections.ProjectionList()
                    .Add(Projections.Property<TransactionLog>(x => (x.PaymentLogId)), "PaymentLogId")
                    .Add(Projections.Property<TransactionLog>(x => (x.PaymentReference)), "PaymentReference")
                    .Add(Projections.Property<TransactionLog>(x => (x.ThirdPartyReceiptNumber)), "ThirdPartyReceiptNumber")
                    .Add(Projections.Property<TransactionLog>(x => (x.Invoice.Id)), "InvoiceId")
                    .Add(Projections.Property<TransactionLog>(x => (x.TaxEntity.Id)), "TaxEntityId")
                    .Add(Projections.Property<TransactionLog>(x => (x.ReceiptNumber)), "ReceiptNumber")
                    .Add(Projections.Sum<TransactionLog>(t => t.AmountPaid), "TotalAmountPaid")
                    .Add(Projections.Group<TransactionLog>(t => t.PaymentLogId))
                    .Add(Projections.Group<TransactionLog>(t => t.PaymentReference))
                    .Add(Projections.Group<TransactionLog>(t => t.ThirdPartyReceiptNumber))
                    .Add(Projections.Group<TransactionLog>(t => t.Invoice.Id))
                    .Add(Projections.Group<TransactionLog>(t => t.TaxEntity.Id))
                    .Add(Projections.Group<TransactionLog>(t => t.ReceiptNumber))
            ).SetResultTransformer(Transformers.AliasToBean<TransactionLogGroup>())
            .List<TransactionLogGroup>().SingleOrDefault();
        }


        /// <summary>
        /// Get details for a particular payment reference for this channel
        /// </summary>
        /// <param name="paymentRef"></param>
        /// <param name="provider"></param>
        /// <returns>TransactionLogGroup</returns>
        public TransactionLogGroup GetGroupedTransactionLogByPayment(string paymentRef, PaymentProvider provider)
        {
            return _transactionManager.GetSession().CreateCriteria<TransactionLog>()
                .Add(Restrictions.Eq("TypeID", (int)PaymentType.Credit))
                .Add(Restrictions.Eq("PaymentProvider", (int)provider))
                .Add(Restrictions.Eq("PaymentReference", paymentRef))
                .SetProjection(Projections.ProjectionList()
                    .Add(Projections.Property<TransactionLog>(x => (x.PaymentLogId)), "PaymentLogId")
                    .Add(Projections.Property<TransactionLog>(x => (x.PaymentReference)), "PaymentReference")
                    .Add(Projections.Property<TransactionLog>(x => (x.ThirdPartyReceiptNumber)), "ThirdPartyReceiptNumber")
                    .Add(Projections.Property<TransactionLog>(x => (x.Invoice.Id)), "InvoiceId")
                    .Add(Projections.Property<TransactionLog>(x => (x.TaxEntity.Id)), "TaxEntityId")
                    .Add(Projections.Property<TransactionLog>(x => (x.ReceiptNumber)), "ReceiptNumber")
                    .Add(Projections.Sum<TransactionLog>(t => t.AmountPaid), "TotalAmountPaid")
                    .Add(Projections.Group<TransactionLog>(t => t.PaymentLogId))
                    .Add(Projections.Group<TransactionLog>(t => t.PaymentReference))
                    .Add(Projections.Group<TransactionLog>(t => t.ThirdPartyReceiptNumber))
                    .Add(Projections.Group<TransactionLog>(t => t.Invoice.Id))
                    .Add(Projections.Group<TransactionLog>(t => t.TaxEntity.Id))
                    .Add(Projections.Group<TransactionLog>(t => t.ReceiptNumber))
            ).SetResultTransformer(Transformers.AliasToBean<TransactionLogGroup>())
            .List<TransactionLogGroup>().SingleOrDefault();
        }



        /// <summary>
        /// Get the transaction for pay direct reversal
        /// </summary>
        /// <param name="paymentLogId"></param>
        /// <param name="payDirect"></param>
        /// <returns>TransactionLogGroup</returns>
        public TransactionLogGroup GetTransactionForPaydirectReversal(string paymentLogId)
        {
            return _transactionManager.GetSession().CreateCriteria<TransactionLog>()
                .Add(Restrictions.Eq("TypeID", (int)PaymentType.Debit))
                .Add(Restrictions.Eq("PaymentProvider", (int)PaymentProvider.PayDirect))
                .Add(Restrictions.Eq("PaymentLogId", paymentLogId))
                .SetProjection(Projections.ProjectionList()
                    .Add(Projections.Property<TransactionLog>(x => (x.PaymentLogId)), "PaymentLogId")
                    .Add(Projections.Property<TransactionLog>(x => (x.OriginalPaymentLogID)), "OriginalPaymentLogID")
                    .Add(Projections.Property<TransactionLog>(x => (x.OriginalPaymentReference)), "OriginalPaymentReference")
                    .Add(Projections.Property<TransactionLog>(x => (x.PaymentReference)), "PaymentReference")
                    .Add(Projections.Property<TransactionLog>(x => (x.ThirdPartyReceiptNumber)), "ThirdPartyReceiptNumber")
                    .Add(Projections.Property<TransactionLog>(x => (x.Invoice.Id)), "InvoiceId")
                    .Add(Projections.Property<TransactionLog>(x => (x.TaxEntity.Id)), "TaxEntityId")
                    .Add(Projections.Property<TransactionLog>(x => (x.ReceiptNumber)), "ReceiptNumber")
                    .Add(Projections.Sum<TransactionLog>(t => t.AmountPaid), "TotalAmountPaid")
                    .Add(Projections.Group<TransactionLog>(t => t.PaymentLogId))
                    .Add(Projections.Group<TransactionLog>(t => t.OriginalPaymentLogID))
                    .Add(Projections.Group<TransactionLog>(t => t.OriginalPaymentReference))
                    .Add(Projections.Group<TransactionLog>(t => t.PaymentReference))
                    .Add(Projections.Group<TransactionLog>(t => t.ThirdPartyReceiptNumber))
                    .Add(Projections.Group<TransactionLog>(t => t.Invoice.Id))
                    .Add(Projections.Group<TransactionLog>(t => t.TaxEntity.Id))
                    .Add(Projections.Group<TransactionLog>(t => t.ReceiptNumber))
            ).SetResultTransformer(Transformers.AliasToBean<TransactionLogGroup>())
            .List<TransactionLogGroup>().SingleOrDefault();
        }


        /// <summary>
        /// Get details for a particular payment log Id for channel with reversal
        /// Has reversal in group
        /// </summary>
        /// <param name="paymentLogId"></param>
        /// <param name="provider"></param>
        /// <returns>TransactionLogGroup</returns>
        public TransactionLogGroup GetGroupedTransactionLogByPaymentLogIdWithReversal(string paymentLogId, PaymentProvider provider)
        {
            return _transactionManager.GetSession().CreateCriteria<TransactionLog>()
                .Add(Restrictions.Eq("TypeID", (int)PaymentType.Credit))
                .Add(Restrictions.Eq("PaymentProvider", (int)provider))
                .Add(Restrictions.Eq("PaymentLogId", paymentLogId))
                .SetProjection(Projections.ProjectionList()
                    .Add(Projections.Property<TransactionLog>(x => (x.PaymentLogId)), "PaymentLogId")
                    .Add(Projections.Property<TransactionLog>(x => (x.PaymentReference)), "PaymentReference")
                    .Add(Projections.Property<TransactionLog>(x => (x.ThirdPartyReceiptNumber)), "ThirdPartyReceiptNumber")
                    .Add(Projections.Property<TransactionLog>(x => (x.Invoice.Id)), "InvoiceId")
                    .Add(Projections.Property<TransactionLog>(x => (x.TaxEntity.Id)), "TaxEntityId")
                    .Add(Projections.Property<TransactionLog>(x => (x.ReceiptNumber)), "ReceiptNumber")
                    .Add(Projections.Property<TransactionLog>(x => (x.Reversed)), "IsReversed")
                    .Add(Projections.Sum<TransactionLog>(t => t.AmountPaid), "TotalAmountPaid")
                    .Add(Projections.Group<TransactionLog>(t => t.PaymentLogId))
                    .Add(Projections.Group<TransactionLog>(t => t.PaymentReference))
                    .Add(Projections.Group<TransactionLog>(t => t.ThirdPartyReceiptNumber))
                    .Add(Projections.Group<TransactionLog>(t => t.Invoice.Id))
                    .Add(Projections.Group<TransactionLog>(t => t.TaxEntity.Id))
                    .Add(Projections.Group<TransactionLog>(t => t.ReceiptNumber))
                    .Add(Projections.Group<TransactionLog>(t => t.Reversed))
            ).SetResultTransformer(Transformers.AliasToBean<TransactionLogGroup>())
            .List<TransactionLogGroup>().SingleOrDefault();
        }


        /// <summary>
        /// When a reversal has happened, we set the initial transaction reversed flag as true
        /// so as to indicate that this transaction has been reversed
        /// </summary>
        /// <param name="paymentReference"></param>
        /// <param name="payDirect"></param>
        /// <returns>bool</returns>
        public bool UpdateTransactionToReversed(string paymentReference, PaymentProvider provider)
        {
            try
            {
                var queryText = $"UPDATE tr SET tr.Reversed = :trueVal FROM Parkway_CBS_Core_TransactionLog tr WHERE PaymentReference = :ref AND TypeID = :typeId AND PaymentProvider = :provider";

                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("trueVal", true);
                query.SetParameter("ref", paymentReference);
                query.SetParameter("typeId", (int)PaymentType.Credit);
                query.SetParameter("provider", (int)provider);

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            { Logger.Error(exception, exception.Message); return false; }
            return true;
        }

        /// <summary>
        /// Get details for a particular payment reference for this provider
        /// </summary>
        /// <param name="paymentRef"></param>
        /// <param name="provider"></param>
        /// <returns>InvoiceDetails</returns>
        public InvoiceDetails GetTransactionLogByPaymentReference(string paymentRef, PaymentProvider provider)
        {
            try
            {
                InvoiceDetails invoiceDetails = _transactionManager.GetSession().Query<TransactionLog>()
                        .Where(p => p.TypeID == (int)PaymentType.Credit && p.PaymentReference == paymentRef && p.PaymentProvider == (int)provider)
                        .Select(tLog => new InvoiceDetails()
                        {
                            CallBackURL = tLog.Invoice.CallBackURL,
                            RevenueHeadCallBackURL = tLog.Invoice.RevenueHead.CallBackURL,
                            RequestRef = tLog.Invoice.APIRequest != null ? tLog.Invoice.APIRequest.RequestIdentifier : null,
                            InvoiceNumber = tLog.InvoiceNumber,
                            ExpertSystemKey = tLog.Invoice.ExpertSystemSettings.ClientSecret,
                            Transaction = new TransactionLogVM
                            {
                                InvoiceNumber = tLog.InvoiceNumber,
                                PaymentDate = tLog.PaymentDate,
                                BankCode = tLog.BankCode,
                                BankBranch = tLog.BankBranch,
                                Bank = tLog.Bank,
                                Channel = tLog.Channel,
                                PaymentMethod = tLog.PaymentMethod,
                                PaymentProvider = tLog.PaymentProvider,
                                AmountPaid = tLog.AmountPaid,
                                PaymentReference = tLog.PaymentReference,
                                TransactionDate = tLog.TransactionDate,
                                IsReversal = tLog.Reversed
                            }
                        }).ToList().SingleOrDefault();

                return invoiceDetails;
            }
            catch (Exception exception)
            { Logger.Error(exception, exception.Message); throw exception; }
        }

        /// <summary>
        /// Get transaction log vm for receipt with specified receipt number
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <returns></returns>
        public IEnumerable<TransactionLogVM> GetTransactionLogsForReceipt(string receiptNumber)
        {
            try
            {
                var result = _transactionManager.GetSession().Query<TransactionLog>().Where(x => x.ReceiptNumber == receiptNumber);
                if(result == null || !result.Any()) { return null; }
                return result.Select(t => new TransactionLogVM {
                    TransactionLogId = t.Id,
                    RevenueHeadId = t.RevenueHead.Id,
                    AmountPaid = t.AmountPaid
                });
            }
            catch (Exception exception) { Logger.Error(exception, exception.Message); throw exception; }
        }
    }
}