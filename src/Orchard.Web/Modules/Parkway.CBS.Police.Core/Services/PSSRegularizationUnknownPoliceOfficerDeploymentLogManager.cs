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
    public class PSSRegularizationUnknownPoliceOfficerDeploymentLogManager : BaseManager<PSSRegularizationUnknownPoliceOfficerDeploymentLog>, IPSSRegularizationUnknownPoliceOfficerDeploymentLogManager<PSSRegularizationUnknownPoliceOfficerDeploymentLog>
    {
        private readonly ITransactionManager _transactionManager;
        public PSSRegularizationUnknownPoliceOfficerDeploymentLogManager(IRepository<PSSRegularizationUnknownPoliceOfficerDeploymentLog> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _transactionManager = orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Gets regularization unknown police officer deployment log with specified command type id, day type id, command id, request id and invoice id
        /// </summary>
        /// <param name="commandTypeId"></param>
        /// <param name="dayTypeId"></param>
        /// <param name="commandId"></param>
        /// <param name="requestId"></param>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public PSSRegularizationUnknownPoliceOfficerDeploymentLogDTO GetPSSRegularizationUnknownPoliceOfficerDeploymentLog(int commandTypeId, int dayTypeId, int commandId, long requestId, long invoiceId)
        {
            try
            {
                return _transactionManager.GetSession()
                    .Query<PSSRegularizationUnknownPoliceOfficerDeploymentLog>()
                    .Where(x => (x.IsActive) && (x.GenerateRequestWithoutOfficersUploadBatchItemsStaging.DayType == dayTypeId) && (x.GenerateRequestWithoutOfficersUploadBatchItemsStaging.CommandType == commandTypeId) && (x.GenerateRequestWithoutOfficersUploadBatchItemsStaging.Command.Id == commandId) && (x.Request.Id == requestId) && (x.Invoice.Id == invoiceId))
                    .Select(x => new PSSRegularizationUnknownPoliceOfficerDeploymentLogDTO
                    {
                        StartDate = x.StartDate,
                        EndDate = x.EndDate
                    }).FirstOrDefault();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

    }
}