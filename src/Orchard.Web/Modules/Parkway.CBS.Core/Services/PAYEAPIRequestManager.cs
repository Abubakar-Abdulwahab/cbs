using NHibernate.Linq;
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
using System.Web;

namespace Parkway.CBS.Core.Services
{
    public class PAYEAPIRequestManager : BaseManager<PAYEAPIRequest>, IPAYEAPIRequestManager<PAYEAPIRequest>
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IRepository<PAYEAPIRequest> _repository;
        private readonly ITransactionManager _transactionManager;

        public PAYEAPIRequestManager(IRepository<PAYEAPIRequest> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
            _repository = repository;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Checks if the batchIdentifier already exist
        /// </summary>
        /// <param name="batchIdentifier"></param>
        /// <returns>Boolean (True or False)</returns>
        public bool BatchIdentifierExist(string batchIdentifier)
        {
            return _transactionManager.GetSession().Query<PAYEAPIRequest>().Count(l => l.BatchIdentifier == batchIdentifier) > 0;
        }

        /// <summary>
        /// Get API Batch details using the batchIdentifier and expertsystemId
        /// </summary>
        /// <param name="batchIdentifier"></param>
        /// <param name="expertSystemId"></param>
        /// <returns>PAYEAPIRequestBatchDetailVM</returns>
        public PAYEAPIRequestBatchDetailVM GetBatchDetails(string batchIdentifier, int expertSystemId)
        {
            try
            {
                return _transactionManager.GetSession().Query<PAYEAPIRequest>().Where(x => x.BatchIdentifier == batchIdentifier && x.RequestedByExpertSystem == new ExpertSystemSettings { Id = expertSystemId }).Select(x => new PAYEAPIRequestBatchDetailVM
                {
                    TaxEntityId = x.TaxEntity.Id,
                    PayerId = x.TaxEntity.PayerId,
                    BatchIdentifier = x.BatchIdentifier,
                    PAYEBatchRecordStagingId = x.PAYEBatchRecordStaging.Id,
                    PAYEAPIRequestId = x.Id,
                    BatchLimit = x.BatchLimit,
                    AdapterValue = x.PAYEBatchRecordStaging.AdapterValue
                }).Single();
            }
            catch (Exception exception)
            {
                Logger.Error($"Could not get batch record with batch identifier {batchIdentifier}. Exception message {exception.Message}");
                throw new NoRecordFoundException($"Could not get batch record with batch identifier {batchIdentifier}.");
            }
        }
    }
}