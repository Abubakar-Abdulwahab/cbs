using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Police.Core.Models;
using System.Collections.Generic;

namespace Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories.Contracts
{
    public interface IPolicerOfficerLogDAOManager : IRepository<PolicerOfficerLog>
    {
        void Save(PolicerOfficerLog policerOfficerLog);

        /// <summary>
        /// Bulk save
        /// </summary>
        /// <param name="policerOfficerLogs"></param>
        void SaveBundle(IEnumerable<PolicerOfficerLog> policerOfficerLogs);
    }
}
