using NHibernate.Linq;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories
{
    public class PSSRegularizationUnknownOfficerRecurringInvoiceSettingsDAOManager : Repository<PSSRegularizationUnknownOfficerRecurringInvoiceSettings>, IPSSRegularizationUnknownOfficerRecurringInvoiceSettingsDAOManager
    {
        public PSSRegularizationUnknownOfficerRecurringInvoiceSettingsDAOManager(IUoW uow) : base(uow)
        {

        }

        /// <summary>
        /// Get paginated records of all the recurring invoice settings for the specified day.
        /// </summary>
        /// <param name="chunkSize"></param>
        /// <param name="skip"></param>
        /// <param name="today"></param>
        /// <returns>IEnumerable<PSSRegularizationUnknownOfficerRecurringInvoiceSettingsDTO></returns>
        public IEnumerable<PSSRegularizationUnknownOfficerRecurringInvoiceSettingsDTO> GetBatchRecurringInvoiceSettings(int chunkSize, int skip, DateTime today)
        {
            return _uow.Session.Query<PSSRegularizationUnknownOfficerRecurringInvoiceSettings>()
                .Where(x => x.NextInvoiceGenerationDate == today).Skip(skip).Take(chunkSize)
                .Select(x => new PSSRegularizationUnknownOfficerRecurringInvoiceSettingsDTO
                {
                    Id = x.Id,
                    WeekDayNumber = x.WeekDayNumber,
                    OffSet = x.OffSet,
                    RequestId = x.Request.Id,
                    GenerateRequestWithoutOfficersUploadBatchStagingId = x.GenerateRequestWithoutOfficersUploadBatchStaging.Id,
                    PaymentBillingType = x.PaymentBillingType,
                    EscortDetailId = x.EscortDetails.Id,
                    CronExpression = x.CronExpression,
                    NextInvoiceGenerationDate = x.NextInvoiceGenerationDate
                });
        }

        /// <summary>
        /// Updates next invoice generation date for settings with specified id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nextInvoiceGenerationDate"></param>
        /// <param name="cronExp"></param>
        public void UpdateNextInvoiceGenerationDateAndCronExpForInvoiceSettings(long id, DateTime nextInvoiceGenerationDate, string cronExp)
        {
            try
            {
                var queryText = $"UPDATE Parkway_CBS_Police_Core_PSSRegularizationUnknownOfficerRecurringInvoiceSettings SET NextInvoiceGenerationDate = :nextInvoiceGenDate, CronExpression = :cronExp, UpdatedAtUtc = :updatedAtDate " +
                    $" WHERE Id = :invoiceSettingsId";
                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("invoiceSettingsId", id);
                query.SetParameter("nextInvoiceGenDate", nextInvoiceGenerationDate);
                query.SetParameter("cronExp", cronExp);
                query.SetParameter("updatedAtDate", DateTime.Now.ToLocalTime());

                query.ExecuteUpdate();
            }
            catch (Exception) { throw; }
        }

    }
}
