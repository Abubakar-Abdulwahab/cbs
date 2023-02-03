using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.POSSAP.Services.HelperModel;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts
{
    public interface ISettlementRuleDAOManager : IRepository<SettlementRule>
    {
        /// <summary>
        /// Set new next schedule date
        /// </summary>
        /// <param name="nextScheduleDate"></param>
        /// <param name="settlemntRuleId"></param>
        /// <returns>int</returns>
        int UpdateNextScheduleDate(DateTime nextScheduleDate, int settlemntRuleId);

    }
}
