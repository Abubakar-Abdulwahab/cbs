using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories.Contracts
{
    public interface IEscortAmountChartSheetDAOManager : IRepository<EscortAmountChartSheet>
    {
        /// <summary>
        /// get the rate for this command type and day type
        /// </summary>
        /// <param name="commandTypeId"></param>
        /// <param name="dayType"></param>
        /// <returns>decimal</returns>
        decimal GetRateForUnknownOfficer(int commandTypeId, int dayType);
    }
}
