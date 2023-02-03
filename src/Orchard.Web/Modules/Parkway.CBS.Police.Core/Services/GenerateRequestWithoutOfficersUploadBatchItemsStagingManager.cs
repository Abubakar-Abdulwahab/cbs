using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Orchard.Logging;
using System;
using System.Collections.Generic;
using NHibernate.Linq;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class GenerateRequestWithoutOfficersUploadBatchItemsStagingManager : BaseManager<GenerateRequestWithoutOfficersUploadBatchItemsStaging>, IGenerateRequestWithoutOfficersUploadBatchItemsStagingManager<GenerateRequestWithoutOfficersUploadBatchItemsStaging>
    {
        private readonly IRepository<GenerateRequestWithoutOfficersUploadBatchItemsStaging> _repo;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public GenerateRequestWithoutOfficersUploadBatchItemsStagingManager(IRepository<GenerateRequestWithoutOfficersUploadBatchItemsStaging> repo, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repo, user, orchardServices)
        {
            _repo = repo;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Gets items in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        public IEnumerable<GenerateRequestWithoutOfficersUploadBatchItemsStagingDTO> GetItems(long batchId)
        {
            try
            {
                return _transactionManager.GetSession().Query<GenerateRequestWithoutOfficersUploadBatchItemsStaging>().Where(x => x.GenerateRequestWithoutOfficersUploadBatchStaging.Id == batchId && !x.HasError)
                    .Select(x => new GenerateRequestWithoutOfficersUploadBatchItemsStagingDTO
                    {
                        Id = x.Id,
                        NumberOfOfficers = x.NumberOfOfficers,
                        DayType = x.DayType,
                        CommandType = x.CommandType,
                        CommandCode = x.CommandCode,
                        CommandId = x.Command.Id
                    });
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Get total number of requested officers in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        public int GetTotalNumberOfRequestedOfficersInBatch(long batchId)
        {
            try
            {
                return _transactionManager.GetSession().Query<GenerateRequestWithoutOfficersUploadBatchItemsStaging>().Where(x => x.GenerateRequestWithoutOfficersUploadBatchStaging.Id == batchId && !x.HasError)
                    .Sum(x => x.NumberOfOfficers);
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}