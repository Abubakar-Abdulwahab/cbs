using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories
{
    public class WalletStatementScheduleUpdateDAOManager : Repository<WalletStatementScheduleUpdate>, IWalletStatementScheduleUpdateDAOManager
    {
        public WalletStatementScheduleUpdateDAOManager(IUoW uow) : base(uow)
        { }


        /// <summary>
        /// Updates wallet statement schedule date
        /// </summary>
        /// <param name="walletStatementScheduleId"></param>
        /// <param name="scheduleUpdateId"></param>
        public void UpdateScheduleDate(int walletStatementScheduleId, int scheduleUpdateId)
        {
            var queryText = $"UPDATE T2 SET T2.{nameof(WalletStatementSchedule.PeriodStartDate)} = T1.{nameof(WalletStatementScheduleUpdate.NextStartDate)}," +
                $" T2.{nameof(WalletStatementSchedule.PeriodEndDate)} = T1.{nameof(WalletStatementScheduleUpdate.NextEndDate)}," +
                $" T2.{nameof(WalletStatementSchedule.NextScheduleDate)} = T1.{nameof(WalletStatementScheduleUpdate.NextScheduleDate)} FROM Parkway_CBS_Police_Core_{nameof(WalletStatementScheduleUpdate)} T1" +
                $" INNER JOIN Parkway_CBS_Police_Core_{nameof(WalletStatementSchedule)} T2 ON T1.{nameof(WalletStatementScheduleUpdate.WalletStatementSchedule)}_Id = T2.{nameof(WalletStatementSchedule.Id)}" +
                $" WHERE T1.{nameof(WalletStatementScheduleUpdate.WalletStatementSchedule)}_Id = :scheduleId AND T1.{nameof(WalletStatementScheduleUpdate.Id)} = :scheduleUpdateId";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("scheduleId", walletStatementScheduleId);
            query.SetParameter("scheduleUpdateId", scheduleUpdateId);
            query.ExecuteUpdate();
        }
    }
}
