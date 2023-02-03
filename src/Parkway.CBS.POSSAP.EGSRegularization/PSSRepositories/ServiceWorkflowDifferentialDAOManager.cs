using NHibernate.Linq;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories.Contracts;
using System.Linq;

namespace Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories
{
    public class ServiceWorkflowDifferentialDAOManager : Repository<ServiceWorkflowDifferential>, IServiceWorkflowDifferentialDAOManager
    {
        public ServiceWorkflowDifferentialDAOManager(IUoW uow) : base(uow)
        {
        }

        /// <summary>
        /// Gets flow definition id for service workflow differential with specified differential details and service id
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="differential"></param>
        /// <returns></returns>
        public int GetFlowDefinitionIdAttachedToServiceWithDifferentialValue(int serviceId, Police.Core.VM.ServiceWorkFlowDifferentialDataParam differential)
        {
            return _uow.Session.Query<ServiceWorkflowDifferential>()
                      .Where(s => (s.Service.Id == serviceId) && (s.DifferentialValue == differential.DifferentialValue) && (s.DifferentialModelName == differential.DifferentialModelName) && s.IsActive)
                      .Select(x => x.FlowDefinition.Id).SingleOrDefault();
        }
    }
}
