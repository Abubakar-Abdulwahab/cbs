using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.Services
{
    public class PoliceCollectionLogManager : BaseManager<PoliceCollectionLog>, IPoliceCollectionLogManager<PoliceCollectionLog>
    {

        private readonly IRepository<PoliceCollectionLog> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;
        public ILogger Logger { get; set; }

        public PoliceCollectionLogManager(IRepository<PoliceCollectionLog> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }

        /// <summary>
        /// Get collection log details using the transaction log id
        /// </summary>
        /// <param name="transactionLogId"></param>
        /// <returns></returns>
        public PoliceCollectionLog GetCollectionLogDetails(long transactionLogId)
        {
            return _transactionManager.GetSession().Query<PoliceCollectionLog>().Where(cm => cm.TransactionLog.Id == transactionLogId).FirstOrDefault();
        }

        /// <summary>
        /// Get payment details from transaction log for a particular invoice and save in police collection log table
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="requestId"></param>
        public void SaveCollectionLogPayment(long invoiceId, long requestId)
        {
            try
            {
                string queryText = $"INSERT INTO Parkway_CBS_Police_Core_PoliceCollectionLog (TransactionLog_Id, Request_Id, CreatedAtUtc, UpdatedAtUtc) SELECT tLog.Id, :requestId, :date, :date FROM Parkway_CBS_Core_TransactionLog tLog LEFT JOIN Parkway_CBS_Police_Core_PoliceCollectionLog pcl ON tLog.Id = pcl.TransactionLog_Id WHERE pcl.TransactionLog_Id IS NULL and tLog.Invoice_Id = :invoiceId and tLog.TypeID != :typeId";

                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("invoiceId", invoiceId);
                query.SetParameter("requestId", requestId);
                query.SetParameter("typeId", (int)PaymentType.Bill);
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
        /// Get receipt details using the specified receipt number
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <returns>USSDValidateReceiptVM</returns>
        public USSDValidateReceiptVM GetReceiptInfo(string receiptNumber)
        {
            return _transactionManager.GetSession().Query<PoliceCollectionLog>().Where(x => x.TransactionLog.Receipt.ReceiptNumber == receiptNumber).Select(recpt => new USSDValidateReceiptVM
            {
                ReceiptNumber = recpt.TransactionLog.Receipt.ReceiptNumber,
                ApplicantName = recpt.Request.TaxEntity.Recipient,
                ServiceName = recpt.Request.Service.Name,
                AmountPaid = string.Format("{0:n2}", recpt.TransactionLog.AmountPaid),
                PaymentDate = recpt.TransactionLog.PaymentDate.ToString("dd MMMM yyyy HH:mm")
            }).SingleOrDefault();
        }
    }
}