using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.POSSAP.Scheduler.Models;
using Parkway.CBS.POSSAP.Services.HelperModel;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts
{
    public interface IExternalDataSourceConfigurationSettingDAOManager : IRepository<ExternalDataSourceConfigurationSetting>
    {
        /// <summary>
        /// Get paginated records of all the active external data configurations that need to be pulled. 
        /// </summary>
        /// <param name="chunkSize"></param>
        /// <param name="skip"></param>
        /// <param name="today"></param>
        /// <returns>List<PSSSettlementRuleVM></returns>
        List<PSSExternalDataSourceConfigurationSettingVM> GetBatchActiveExternalDataSourceConfigurations(int chunkSize, int skip, DateTime today);
    }
}
