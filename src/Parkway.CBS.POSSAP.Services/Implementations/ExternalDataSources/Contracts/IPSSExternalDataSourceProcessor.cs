using Parkway.CBS.POSSAP.Services.HelperModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.POSSAP.Services.Implementations.ExternalDataSources.Contracts
{
    public interface IPSSExternalDataSourceProcessor
    {
        /// <summary>
        /// This gets extrnal data for processing
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="dataSourceConfigurationSettingVM"></param>
        /// <returns>string</returns>
        string ProcessExternalDataSource(string tenantName, PSSExternalDataSourceConfigurationSettingVM dataSourceConfigurationSettingVM);
    }
}
