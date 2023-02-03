using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Parkway.CBS.Core.Services
{
    public class PAYEBatchItemsPagesTrackerManager : BaseManager<PAYEAPIBatchItemsPagesTracker>, IPAYEBatchItemsPagesTrackerManager<PAYEAPIBatchItemsPagesTracker>
    {
        private readonly IRepository<PAYEAPIBatchItemsPagesTracker> _repository;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public PAYEBatchItemsPagesTrackerManager(IRepository<PAYEAPIBatchItemsPagesTracker> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
            _repository = repository;
            Logger = NullLogger.Instance;
        }

        public bool BatchItemPageExist(string batchIdentifier, int pageNumber)
        {
            return _transactionManager.GetSession()
                                      .Query<PAYEAPIBatchItemsPagesTracker>()
                                      .Count(l => l.PAYEAPIRequest.BatchIdentifier == batchIdentifier && l.PageNumber == pageNumber) > 0;
        }

        /// <summary>
        /// Returns Only the BatchItemsPagesTracker Id
        /// </summary>
        /// <param name="lambda"></param>
        /// <returns>The Id of the BatchItemsPagesTracker</returns>
        /// <exception cref="NoRecordFoundException"> Thrown when no record is found matching the query</exception>
        public long GetPAYEAPIBatchItemsPagesTrackerId(Expression<Func<PAYEAPIBatchItemsPagesTracker, bool>> lambda)
        {
            try
            {
                return _transactionManager.GetSession().Query<PAYEAPIBatchItemsPagesTracker>().Where(lambda)
                       .Select(txp => txp.Id).Single();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw new NoRecordFoundException();
            }
        }
    }
}