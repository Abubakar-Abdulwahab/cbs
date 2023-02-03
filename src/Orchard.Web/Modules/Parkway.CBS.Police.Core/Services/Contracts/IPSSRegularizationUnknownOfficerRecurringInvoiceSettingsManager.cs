using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.DTO;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSSRegularizationUnknownOfficerRecurringInvoiceSettingsManager<PSSRegularizationUnknownOfficerRecurringInvoiceSettings> : IDependency, IBaseManager<PSSRegularizationUnknownOfficerRecurringInvoiceSettings>
    {

        /// <summary>
        /// Get escort regularization settings details
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>EscortRegularizationSettingsDTO</returns>
        EscortRegularizationSettingsDTO GetEscortRegularizationSettingsDetails(long requestId);
    }
}
