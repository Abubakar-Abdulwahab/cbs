using NHibernate.Linq;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.Services.HelperModel;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories
{
    public class PoliceOfficerDeploymentLogDAOManager : Repository<PoliceOfficerDeploymentLog>, IPoliceOfficerDeploymentLogDAOManager
    {
        public PoliceOfficerDeploymentLogDAOManager(IUoW uow) : base(uow)
        { }

        /// <summary>
        /// Get the list of deployed officers for a specified request id
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>List<PoliceOfficerDeploymentVM></returns>
        public List<PoliceOfficerDeploymentVM> GetDeployedOfficerForRequest(long requestId)
        {
            return _uow.Session.Query<PoliceOfficerDeploymentLog>()
                .Where(x => x.Request == new PSSRequest { Id = requestId } && x.IsActive)
                .Select(x => new PoliceOfficerDeploymentVM
                {
                    PoliceOfficerLogId = x.PoliceOfficerLog.Id,
                    DeploymentRate = x.DeploymentRate,
                    CommandId = x.Command.Id,
                    OfficerName = x.OfficerName
                }).ToList();
        }
    }
}
