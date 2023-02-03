using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.Core.Models;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;
using System;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories
{
    public class SettlementRuleDAOManager : Repository<SettlementRule>, ISettlementRuleDAOManager
    {
        public SettlementRuleDAOManager(IUoW uow) : base(uow)
        { }

        /// <summary>
        /// Set new next schedule date
        /// </summary>
        /// <param name="nextScheduleDate"></param>
        /// <param name="settlemntRuleId"></param>
        /// <returns>int</returns>
        public int UpdateNextScheduleDate(DateTime nextScheduleDate, int settlemntRuleId)
        {
            try
            {
                //Update transaction log column IsSettled
                var queryText = $"UPDATE sr SET sr.NextScheduleDate = :nextScheduleDate FROM Parkway_CBS_Core_SettlementRule sr WHERE sr.Id = :settlemntRuleId";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("settlemntRuleId", settlemntRuleId);
                query.SetParameter("nextScheduleDate", nextScheduleDate);

                return query.ExecuteUpdate(); ;
            }
            catch (Exception)
            { throw; }
        }

    }
}
