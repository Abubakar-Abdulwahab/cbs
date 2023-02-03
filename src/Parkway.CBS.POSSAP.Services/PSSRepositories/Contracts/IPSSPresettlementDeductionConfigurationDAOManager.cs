using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.Services.HelperModel;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts
{
    public interface IPSSPresettlementDeductionConfigurationDAOManager : IRepository<PSSPresettlementDeductionConfiguration>
    {
        /// <summary>
        /// Get settlement presettled cost of service deductions
        /// </summary>
        /// <param name="settlementRuleId"></param>
        /// <param name="revenueHeadId"></param>
        /// <param name="serviceId"></param>
        /// <param name="paymentProviderId"></param>
        /// <param name="definitionLevelId"></param>
        /// <returns>PSSPresettlementDeductionConfigurationVM</returns>
        PSSPresettlementDeductionConfigurationVM GetPresettlementDeductionConfiguration(int settlementRuleId, int revenueHeadId, int serviceId, int paymentProviderId, int definitionLevelId);
    }
}
