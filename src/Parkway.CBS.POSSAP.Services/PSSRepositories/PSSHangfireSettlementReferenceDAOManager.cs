using System;
using System.Collections.Generic;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.HangFireInterface.Logger;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.HangFireInterface.Logger.Contracts;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories
{
    public class PSSHangfireSettlementReferenceDAOManager : Repository<PSSHangfireSettlementReference>, IPSSHangfireSettlementReferenceDAOManager
    {
        private static readonly ILogger log = new Log4netLogger();

        public PSSHangfireSettlementReferenceDAOManager(IUoW uow) : base(uow)
        { }


        /// <summary>
        /// save a list of hang fire references
        /// </summary>
        /// <param name="hangfireRefs"></param>
        public void SaveBundle(IList<PSSHangfireSettlementReference> hangfireRefs)
        {
            try
            {
                _uow.BeginStatelessTransaction();
                foreach (var item in hangfireRefs)
                {
                    _uow.Session.Save(item);
                }
                _uow.Commit();
                log.Info($"Saved records for hangfire settlement reference");
            }
            catch (Exception exception)
            {
                _uow.Rollback();
                log.Error($"Error inserting hangfire settlement reference", exception);
            }
        }

    }
}
