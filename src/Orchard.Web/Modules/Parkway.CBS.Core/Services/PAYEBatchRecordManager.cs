using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Orchard.Data;
using Orchard;
using Orchard.Users.Models;
using Orchard.Logging;
using System;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using System.Linq;
using NHibernate.Linq;

namespace Parkway.CBS.Core.Services
{
    public class PAYEBatchRecordManager : BaseManager<PAYEBatchRecord>, IPAYEBatchRecordManager<PAYEBatchRecord>
    {
        private readonly IRepository<PAYEBatchRecord> _repository;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public PAYEBatchRecordManager(IRepository<PAYEBatchRecord> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
            _repository = repository;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// set the batch record payment completed value
        /// </summary>
        /// <param name="batchRecordId"></param>
        /// <param name="completed"></param>
        /// <exception cref="CouldNotSaveRecord"></exception>
        public void SetPaymentCompletedValue(long batchRecordId, bool completed)
        {
            string queryText = $"UPDATE Parkway_CBS_Core_{nameof(PAYEBatchRecord)} SET [{nameof(PAYEBatchRecord.PaymentCompleted)}] = :completedVal, [{nameof(PAYEBatchRecord.UpdatedAtUtc)}] = :dateSaved WHERE {nameof(PAYEBatchRecord.Id)} = :batchRecordId";

            var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
            query.SetParameter("completedVal", completed);
            query.SetParameter("batchRecordId", batchRecordId);
            query.SetParameter("dateSaved", DateTime.Now.ToLocalTime());
            if(query.ExecuteUpdate() != 1) throw new CouldNotSaveRecord("Could not update payment completed batch record for staging batch Id " + batchRecordId);
        }

        /// <summary>
        /// get batch record with specified batch ref
        /// </summary>
        /// <param name="batchRef"></param>
        /// <returns></returns>
        public PAYEBatchRecordVM GetBatchRecordWithRef(string batchRef)
        {
            try
            {
                return _transactionManager.GetSession().Query<PAYEBatchRecord>().Where(x => x.BatchRef == batchRef).Select(x => new PAYEBatchRecordVM
                {
                    BatchRecordId = x.Id,
                    BatchRef = x.BatchRef,
                    RevenueHeadSurCharge = x.RevenueHeadSurCharge,
                    TotalIncomeTaxForPayesInSchedule = x.Payees.Sum(tax => tax.IncomeTaxPerMonth),
                    PaymentCompleted = x.PaymentCompleted,
                    PaymentTypeCode = x.PaymentTypeCode,
                    AssessmentType = (Models.Enums.PayeAssessmentType)x.AssessmentType,
                    CreatedAt = x.CreatedAtUtc
                }).First();
            }catch(Exception exception)
            {
                Logger.Error($"Could not get batch record with batch ref {batchRef}. Exception message {exception.Message}");
                throw new NoRecordFoundException($"Could not get batch record with batch ref {batchRef}.");
            }
        }

    }
}