using NHibernate.Linq;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories
{
    public class PSServiceRevenueHeadDAOManager : Repository<PSServiceRevenueHead>, IPSServiceRevenueHeadDAOManager
    {
        public PSServiceRevenueHeadDAOManager(IUoW uow) : base(uow)
        {
        }


        /// <summary>
        /// Gets revenue heads with specified service id and flow definition level id
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="isApplicationInvoice"></param>
        /// <returns></returns>
        public IEnumerable<PSServiceRevenueHeadVM> GetRevenueHead(int serviceId, int levelId)
        {
            return _uow.Session.Query<PSServiceRevenueHead>()
           .Where(x => x.Service.Id == serviceId && x.FlowDefinitionLevel == new PSServiceRequestFlowDefinitionLevel { Id = levelId } && x.IsActive)
           .Select(sr => new PSServiceRevenueHeadVM
           {
               ServiceName = sr.Service.Name,
               AmountToPay = sr.RevenueHead.BillingModel.Amount,
               FeeDescription = sr.Description,
               RevenueHeadId = sr.RevenueHead.Id,
               IsGroupHead = sr.RevenueHead.IsGroup,
               Surcharge = sr.RevenueHead.BillingModel.Surcharge
           }).ToFuture();
        }
    }
}
