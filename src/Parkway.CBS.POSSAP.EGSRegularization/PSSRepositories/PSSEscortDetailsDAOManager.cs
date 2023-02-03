using NHibernate.Linq;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories.Contracts;
using System.Linq;

namespace Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories
{
    public class PSSEscortDetailsDAOManager : Repository<PSSEscortDetails>, IPSSEscortDetailsDAOManager
    {
        public PSSEscortDetailsDAOManager(IUoW uow) : base(uow)
        {

        }


        /// <summary>
        /// Gets escort details with specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EscortDetailsDTO GetEscortDetails(long id)
        {
            return _uow.Session.Query<PSSEscortDetails>().Where(x => x.Id == id).Select(x => new EscortDetailsDTO { NumberOfOfficers = x.NumberOfOfficers, CommandTypeId = x.CommandType.Id }).SingleOrDefault();
        }
    }
}
