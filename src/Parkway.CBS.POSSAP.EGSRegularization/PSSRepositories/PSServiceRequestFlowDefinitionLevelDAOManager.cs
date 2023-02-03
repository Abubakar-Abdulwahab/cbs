using NHibernate.Linq;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories.Contracts;
using System.Linq;

namespace Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories
{
    public class PSServiceRequestFlowDefinitionLevelDAOManager : Repository<PSServiceRequestFlowDefinitionLevel>, IPSServiceRequestFlowDefinitionLevelDAOManager
    {
        public PSServiceRequestFlowDefinitionLevelDAOManager(IUoW uow) : base(uow)
        {
        }


        /// <summary>
        /// Get the last level defintiion with the specified workflow action value in flow definition with specified id
        /// </summary>
        /// <param name="serviceId"></param>
        public PSServiceRequestFlowDefinitionLevelDTO GetLastLevelDefinitionWithWorkflowActionValue(int definitionId, Police.Core.Models.Enums.RequestDirection actionValue)
        {
            return _uow.Session.Query<PSServiceRequestFlowDefinitionLevel>()
                      .Where(x => x.Definition.Id == definitionId && x.WorkFlowActionValue == (int)actionValue)
                      .Select(ld => new PSServiceRequestFlowDefinitionLevelDTO { Id = ld.Id, PositionDescription = ld.PositionDescription, DefinitionId = ld.Definition.Id, RequestDirectionValue = (Police.Core.Models.Enums.RequestDirection)ld.WorkFlowActionValue })
                      .ToList()
                      .OrderByDescending(defLevel => defLevel.Position)
                      .FirstOrDefault();
        }
    }
}
