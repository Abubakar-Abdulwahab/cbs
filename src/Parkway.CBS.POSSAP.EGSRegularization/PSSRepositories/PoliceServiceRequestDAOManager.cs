using NHibernate.Linq;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories
{
    public class PoliceServiceRequestDAOManager : Repository<PoliceServiceRequest>, IPoliceServiceRequestDAOManager
    {
        public PoliceServiceRequestDAOManager(IUoW uow) : base(uow)
        {

        }
    }
}
