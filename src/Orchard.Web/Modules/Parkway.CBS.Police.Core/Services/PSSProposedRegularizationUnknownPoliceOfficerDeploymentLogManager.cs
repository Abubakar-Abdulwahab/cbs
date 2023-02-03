using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSSProposedRegularizationUnknownPoliceOfficerDeploymentLogManager : BaseManager<PSSProposedRegularizationUnknownPoliceOfficerDeploymentLog>, IPSSProposedRegularizationUnknownPoliceOfficerDeploymentLogManager<PSSProposedRegularizationUnknownPoliceOfficerDeploymentLog>
    {
        private readonly IRepository<PSSProposedRegularizationUnknownPoliceOfficerDeploymentLog> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public PSSProposedRegularizationUnknownPoliceOfficerDeploymentLogManager(IRepository<PSSProposedRegularizationUnknownPoliceOfficerDeploymentLog> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
        }

        /// <summary>
        /// Gets officers log for the request with the specified id
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>IEnumerable<RegularizationRequestWithoutOfficersUploadBatchItemsStagingDTO></returns>
        public IEnumerable<RegularizationRequestWithoutOfficersUploadBatchItemsStagingDTO> GetEscortRegularizationOfficerDeployment(long requestId, long invoiceId)
        {
            return _transactionManager.GetSession().Query<PSSProposedRegularizationUnknownPoliceOfficerDeploymentLog>().Where(x => (x.Request == new PSSRequest { Id = requestId }) && (x.Invoice.Id == invoiceId) && x.IsActive).Select(x => new RegularizationRequestWithoutOfficersUploadBatchItemsStagingDTO
            {
                BatchItemStagingId = x.GenerateRequestWithoutOfficersUploadBatchItemsStaging.Id,
                DeploymentRate = x.DeploymentRate,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                CommandId = x.GenerateRequestWithoutOfficersUploadBatchItemsStaging.Command.Id,
                NumberOfOfficers = x.GenerateRequestWithoutOfficersUploadBatchItemsStaging.NumberOfOfficers
            });
        }

    }
}