using NHibernate.Linq;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.POSSAP.Scheduler.Models;
using Parkway.CBS.POSSAP.Services.HelperModel;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories
{
    public class ExternalDataSourceConfigurationSettingDAOManager : Repository<ExternalDataSourceConfigurationSetting>, IExternalDataSourceConfigurationSettingDAOManager
    {
        public ExternalDataSourceConfigurationSettingDAOManager(IUoW uow) : base(uow)
        { }

        /// <summary>
        /// Get paginated records of all the active POSSAP settlement configurations on the systems.
        /// </summary>
        /// <param name="chunkSize"></param>
        /// <param name="skip"></param>
        /// <param name="today"></param>
        /// <returns>List<PSSSettlementRuleVM></returns>
        public List<PSSExternalDataSourceConfigurationSettingVM> GetBatchActiveExternalDataSourceConfigurations(int chunkSize, int skip, DateTime today)
        {
            return _uow.Session.Query<ExternalDataSourceConfigurationSetting>()
                .Where(x => x.IsActive && x.NextScheduleDate >= today && x.NextScheduleDate <= today.AddHours(6)).Skip(skip).Take(chunkSize)
                .Select(x=> new PSSExternalDataSourceConfigurationSettingVM
                {
                    ExternalDataSourceConfigId = x.Id,
                    ActionName = x.ActionName,
                    ImplementingClass = x.ImplementingClass,
                    CRONValue = x.CRONValue,
                    NextScheduleDate = x.NextScheduleDate
                }).ToList();
        }
    }
}
