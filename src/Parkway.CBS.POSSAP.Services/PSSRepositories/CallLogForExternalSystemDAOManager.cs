using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.POSSAP.Scheduler.Models;
using Parkway.CBS.POSSAP.Services.HelperModel;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories
{
    public class CallLogForExternalSystemDAOManager : Repository<CallLogForExternalSystem>, ICallLogForExternalSystemDAOManager
    {
        public CallLogForExternalSystemDAOManager(IUoW uow) : base(uow)
        { }
    }
}
