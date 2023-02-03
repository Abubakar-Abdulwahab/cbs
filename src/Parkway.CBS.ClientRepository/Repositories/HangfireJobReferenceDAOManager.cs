using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientRepository.Repositories
{
    public class HangfireJobReferenceDAOManager : Repository<HangfireJobReference>, IHangfireJobReferenceDAOManager
    {
        public HangfireJobReferenceDAOManager(IUoW uow) : base(uow)
        { }
    }
}
