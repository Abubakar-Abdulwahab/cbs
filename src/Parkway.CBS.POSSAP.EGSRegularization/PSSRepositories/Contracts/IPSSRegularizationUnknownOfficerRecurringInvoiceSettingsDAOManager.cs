using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories.Contracts
{
    public interface IPSSRegularizationUnknownOfficerRecurringInvoiceSettingsDAOManager : IRepository<PSSRegularizationUnknownOfficerRecurringInvoiceSettings>
    {
        /// <summary>
        /// Get paginated records of all the recurring invoice settings for the specified day.
        /// </summary>
        /// <param name="chunkSize"></param>
        /// <param name="skip"></param>
        /// <param name="today"></param>
        /// <returns>IEnumerable<PSSRegularizationUnknownOfficerRecurringInvoiceSettingsDTO></returns>
        IEnumerable<PSSRegularizationUnknownOfficerRecurringInvoiceSettingsDTO> GetBatchRecurringInvoiceSettings(int chunkSize, int skip, DateTime today);

        /// <summary>
        /// Updates next invoice generation date for settings with specified id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nextInvoiceGenerationDate"></param>
        /// <param name="cronExp"></param>
        void UpdateNextInvoiceGenerationDateAndCronExpForInvoiceSettings(long id, DateTime nextInvoiceGenerationDate, string cronExp);
    }
}
