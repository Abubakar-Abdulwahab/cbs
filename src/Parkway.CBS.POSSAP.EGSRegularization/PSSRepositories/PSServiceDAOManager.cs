using NHibernate.Linq;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories.Contracts;
using System.Linq;

namespace Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories
{
    public class PSServiceDAOManager : Repository<PSService>, IPSServiceDAOManager
    {
        public PSServiceDAOManager(IUoW uow) : base(uow)
        {
        }

        /// <summary>
        /// Get service with specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PSServiceVM GetService(int id)
        {
            return _uow.Session.Query<PSService>().Where(x => x.Id == id).Select(x => new PSServiceVM { ServiceId = x.Id, ServicePrefix = x.ServicePrefix, HasDifferentialWorkFlow = x.HasDifferentialWorkFlow }).SingleOrDefault();
        }


        /// <summary>
        /// Gets flow definition id for service with specified id
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        public int GetFlowDefinitionForService(int serviceId)
        {
            return _uow.Session.Query<PSService>()
                      .Where(s => s.Id == serviceId)
                      .Select(x => x.FlowDefinition.Id).SingleOrDefault();
        }
    }
}
