using NHibernate.Linq;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.POSSAP.Scheduler.Models;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;
using System.Linq;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories
{
    public class PSSStateModelExternalDataStateDAOManager : Repository<PSSStateModelExternalDataState>, IPSSStateModelExternalDataStateDAOManager
    {
        public PSSStateModelExternalDataStateDAOManager(IUoW uow) : base(uow)
        { }

        /// <summary>
        /// Get the last CallLogForExternalSystem_Id
        /// </summary>
        /// <returns> <see cref="long?"/></returns>
        public long? GetLastCallLogForExternalSystemId()
        {
            return _uow.Session.Query<PSSStateModelExternalDataState>().OrderByDescending(x => x.Id)?.FirstOrDefault()?.CallLogForExternalSystem?.Id;
        }
    }
}
