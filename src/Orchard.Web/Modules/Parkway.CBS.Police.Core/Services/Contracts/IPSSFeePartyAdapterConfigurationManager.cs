using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSSFeePartyAdapterConfigurationManager<PSSFeePartyAdapterConfiguration> : IDependency, IBaseManager<PSSFeePartyAdapterConfiguration>
    {
        /// <summary>
        /// Checks if the settlement adapter configuration exists using the <paramref name="adapterId"/>
        /// </summary>
        /// <param name="adapterId"></param>
        /// <returns></returns>
        bool CheckIfSettlementAdapterConfigurationsExistByAdapterId(int adapterId);

        /// <summary>
        /// Gets all active settlement adapter configurations
        /// </summary>
        /// <returns></returns>
        List<FeePartyAdapterConfigurationVM> GetActiveSettlementAdapterConfigurations();

        /// <summary>
        /// Get the Settlement Adapter by <paramref name="adapterId"/> 
        /// </summary>
        /// <param name="adapterId"></param>
        /// <returns></returns>
        FeePartyAdapterConfigurationVM GetSettlementAdapterConfigurationsByAdapterId(int adapterId);
    }
}
