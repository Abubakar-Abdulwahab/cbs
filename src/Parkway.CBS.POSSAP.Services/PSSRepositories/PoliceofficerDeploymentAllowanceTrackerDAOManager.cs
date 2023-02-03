using NHibernate.Linq;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.Services.HelperModel;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories
{
    public class PoliceofficerDeploymentAllowanceTrackerDAOManager : Repository<PoliceofficerDeploymentAllowanceTracker>, IPoliceofficerDeploymentAllowanceTrackerDAOManager
    {
        public PoliceofficerDeploymentAllowanceTrackerDAOManager(IUoW uow) : base(uow)
        { }

        /// <summary>
        /// Get paginated records of all the deployment allowance that is due for the day
        /// </summary>
        /// <param name="chunkSize"></param>
        /// <param name="skip"></param>
        /// <param name="today"></param>
        /// <returns>List<PSSDeploymentAllowanceTrackerVM></returns>
        public List<PSSDeploymentAllowanceTrackerVM> GetBatchDueDeploymentAllowance(int chunkSize, int skip, DateTime today)
        {
            return _uow.Session.Query<PoliceofficerDeploymentAllowanceTracker>()
                .Where(x => !x.IsSettlementCompleted && x.NextSettlementDate.Date == today).Skip(skip).Take(chunkSize)
                .Select(x=> new PSSDeploymentAllowanceTrackerVM
                {
                    Id = x.Id,
                    RequestId = x.Request.Id,
                    InvoiceId = x.Invoice.Id,
                    NumberOfSettlementDone = x.NumberOfSettlementDone,
                    EscortDetailId = x.EscortDetails.Id,
                    EscortStartDate = x.EscortDetails.StartDate,
                    EscortEndDate = x.EscortDetails.EndDate,
                    NextSettlementDate = x.NextSettlementDate,
                    SettlementCycleStartDate = x.SettlementCycleStartDate,
                    SettlementCycleEndDate = x.SettlementCycleEndDate
                }).ToList();
        }

    }
}
