using NHibernate.Linq;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.Services.HelperModel;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;
using System.Linq;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories
{
    public class WalletStatementScheduleDAOManager : Repository<WalletStatementSchedule>, IWalletStatementScheduleDAOManager
    {
        public WalletStatementScheduleDAOManager(IUoW uow) : base(uow)
        { }


        /// <summary>
        /// Gets first or default schedule
        /// </summary>
        /// <returns></returns>
        public WalletStatementScheduleVM GetSchedule()
        {
            return _uow.Session.Query<WalletStatementSchedule>().Select(x => new WalletStatementScheduleVM { Id = x.Id, CronExpression = x.CronExpression, PeriodStartDate = x.PeriodStartDate, PeriodEndDate = x.PeriodEndDate, NextScheduleDate = x.NextScheduleDate }).FirstOrDefault();
        }
    }
}
