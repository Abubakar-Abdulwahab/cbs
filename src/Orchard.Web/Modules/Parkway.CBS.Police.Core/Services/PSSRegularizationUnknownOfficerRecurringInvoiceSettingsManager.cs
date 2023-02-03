using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSSRegularizationUnknownOfficerRecurringInvoiceSettingsManager : BaseManager<PSSRegularizationUnknownOfficerRecurringInvoiceSettings>, IPSSRegularizationUnknownOfficerRecurringInvoiceSettingsManager<PSSRegularizationUnknownOfficerRecurringInvoiceSettings>
    {
        private readonly IRepository<PSSRegularizationUnknownOfficerRecurringInvoiceSettings> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;
        public ILogger Logger { get; set; }

        public PSSRegularizationUnknownOfficerRecurringInvoiceSettingsManager(IRepository<PSSRegularizationUnknownOfficerRecurringInvoiceSettings> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Get escort regularization settings details
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>EscortRegularizationSettingsDTO</returns>
        public EscortRegularizationSettingsDTO GetEscortRegularizationSettingsDetails(long requestId)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSSRegularizationUnknownOfficerRecurringInvoiceSettings>()
                  .Where(x => x.Request.Id == requestId)
                  .Select(esd => new EscortRegularizationSettingsDTO
                  {
                      StateId = esd.EscortDetails.State.Id,
                      LGAId = esd.EscortDetails.LGA.Id,
                      WeekDayNumber = esd.WeekDayNumber,
                      Address = esd.EscortDetails.Address,
                      CustomerName = esd.EscortDetails.Request.CBSUser.Name
                  }).SingleOrDefault();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Error getting escort regularization settings details for request Id" + requestId));
                throw;
            }
        }

    }
}