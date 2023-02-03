using System;
using Orchard;
using System.Linq;
using Orchard.Data;
using NHibernate.Linq;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using NHibernate.Criterion;
using Parkway.CBS.Core.HelperModels;
using NHibernate.Transform;
using System.Collections.Generic;

namespace Parkway.CBS.Core.Services
{
    public class PAYEPaymentUtilizationManager : BaseManager<PAYEPaymentUtilization>, IPAYEPaymentUtilizationManager<PAYEPaymentUtilization>
    {
        private readonly IRepository<PAYEPaymentUtilization> _repository;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public PAYEPaymentUtilizationManager(IRepository<PAYEPaymentUtilization> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
            _repository = repository;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Get the total amount paid for a batch using the batch record id
        /// </summary>
        /// <param name="batchRecordId"></param>
        /// <returns>decimal</returns>
        public decimal GetBatchRecordAmountPaid(long batchRecordId)
        {
            try
            {
                return _transactionManager.GetSession().CreateCriteria<PAYEPaymentUtilization>(nameof(PAYEPaymentUtilization))
                    .CreateAlias(nameof(PAYEPaymentUtilization)+"."+nameof(PAYEPaymentUtilization.PAYEBatchRecord), nameof(PAYEPaymentUtilization.PAYEBatchRecord))
                    .Add(Restrictions.Where<PAYEPaymentUtilization>(pu => pu.PAYEBatchRecord.Id == batchRecordId))
                  .SetProjection(Projections.ProjectionList()
                  .Add(Projections.Sum<PAYEPaymentUtilization>(x => (x.UtilizedAmount)), nameof(ReportStatsVM.TotalAmount)))
                  .SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>())
                  .List<ReportStatsVM>().First().TotalAmount;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Get receipts utilized for the schedule with the specified batch record Id
        /// </summary>
        /// <param name="batchRecordId"></param>
        /// <returns></returns>
        public IEnumerable<PAYEReceiptVM> GetUtilizedReceiptsForBatchRecord(long batchRecordId)
        {
            try
            {
                return _transactionManager.GetSession().Query<PAYEPaymentUtilization>().Where(x => x.PAYEBatchRecord.Id == batchRecordId).Select(x => new PAYEReceiptVM
                {
                    Id = x.PAYEReceipt.Id,
                    TotalAmount = x.PAYEReceipt.GetReceiptAmount(),
                    UtilizedAmount = x.PAYEReceipt.UtilizedAmount(),
                    ReceiptNumber = x.PAYEReceipt.Receipt.ReceiptNumber,
                    Status = x.PAYEReceipt.UtilizationStatusId,
                }).ToFuture();
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Get receipts utilized for the schedule with the specified batch ref
        /// </summary>
        /// <param name="batchRef"></param>
        /// <returns></returns>
        public IEnumerable<PAYEReceiptVM> GetUtilizedReceiptsForBatchRecord(string batchRef)
        {
            try
            {
                return _transactionManager.GetSession().Query<PAYEPaymentUtilization>().Where(x => x.PAYEBatchRecord.BatchRef == batchRef).Select(x => new PAYEReceiptVM
                {
                    Id = x.PAYEReceipt.Id,
                    TotalAmount = x.PAYEReceipt.GetReceiptAmount(),
                    UtilizedAmount = x.PAYEReceipt.UtilizedAmount(),
                    ReceiptNumber = x.PAYEReceipt.Receipt.ReceiptNumber,
                    Status = x.PAYEReceipt.UtilizationStatusId,
                    UtilizedAmountForSchedule = x.UtilizedAmount,
                }).ToFuture();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}