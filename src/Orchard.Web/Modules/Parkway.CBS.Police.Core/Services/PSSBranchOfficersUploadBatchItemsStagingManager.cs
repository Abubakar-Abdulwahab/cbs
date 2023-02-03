using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSSBranchOfficersUploadBatchItemsStagingManager : BaseManager<PSSBranchOfficersUploadBatchItemsStaging>, IPSSBranchOfficersUploadBatchItemsStagingManager<PSSBranchOfficersUploadBatchItemsStaging>
    {
        private readonly IRepository<PSSBranchOfficersUploadBatchItemsStaging> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public PSSBranchOfficersUploadBatchItemsStagingManager(IRepository<PSSBranchOfficersUploadBatchItemsStaging> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }


        /// <summary>
        /// Gets PSS branch officers upload batch items in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        public IEnumerable<PSSBranchOfficersUploadBatchItemsStagingDTO> GetItemsInBatchWithId(long batchId)
        {
            return _transactionManager.GetSession().Query<PSSBranchOfficersUploadBatchItemsStaging>()
                .Where(x => x.PSSBranchOfficersUploadBatchStaging.Id == batchId && !x.HasError)
                .Select(x => new PSSBranchOfficersUploadBatchItemsStagingDTO { Id = x.Id, APNumber = x.APNumber, OfficerCommand = new HelperModels.CommandVM { Id = x.OfficerCommand.Id, Code = x.OfficerCommandCode, ParentCode = x.OfficerCommand.ParentCode, StateId = x.OfficerCommand.State.Id, LGAId = x.OfficerCommand.LGA.Id } });
        }
    }
}