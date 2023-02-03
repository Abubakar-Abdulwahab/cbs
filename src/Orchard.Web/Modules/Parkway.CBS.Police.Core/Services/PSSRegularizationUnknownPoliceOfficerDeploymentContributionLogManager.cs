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
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSSRegularizationUnknownPoliceOfficerDeploymentContributionLogManager : BaseManager<PSSRegularizationUnknownPoliceOfficerDeploymentContributionLog>, IPSSRegularizationUnknownPoliceOfficerDeploymentContributionLogManager<PSSRegularizationUnknownPoliceOfficerDeploymentContributionLog>
    {
        private readonly IRepository<PSSRegularizationUnknownPoliceOfficerDeploymentContributionLog> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public PSSRegularizationUnknownPoliceOfficerDeploymentContributionLogManager(IRepository<PSSRegularizationUnknownPoliceOfficerDeploymentContributionLog> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Gets tPSSRegularizationUnknownPoliceOfficerDeploymentContributionLogs
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="invoiceId"></param>
        /// <param name="commandId"></param>
        /// <returns></returns>
        public List<PSSRegularizationUnknownPoliceOfficerDeploymentContributionLogDTO> GetPSSRegularizationUnknownPoliceOfficerDeploymentContributionLogs(long requestId, long invoiceId, int commandId)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSSRegularizationUnknownPoliceOfficerDeploymentContributionLog>().Where(x => (x.Request.Id == requestId) && (x.Invoice.Id == invoiceId) && (x.GenerateRequestWithoutOfficersUploadBatchItemsStaging.Command.Id == commandId)).Select(x => new PSSRegularizationUnknownPoliceOfficerDeploymentContributionLogDTO
                {
                    Id = x.Id,
                    NumberOfDays = x.NumberOfDays,
                    DeploymentRate = x.DeploymentRate,
                    DeploymentAllowanceAmount = x.DeploymentAllowanceAmount,
                    DeploymentAllowancePercentage = x.DeploymentAllowancePercentage,
                    CommandTypeId = x.GenerateRequestWithoutOfficersUploadBatchItemsStaging.CommandType,
                    DayTypeId = x.GenerateRequestWithoutOfficersUploadBatchItemsStaging.DayType,
                    NumberOfOfficers = x.GenerateRequestWithoutOfficersUploadBatchItemsStaging.NumberOfOfficers
                }).ToList();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Gets deployment contribution log using specified parameters
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="invoiceId"></param>
        /// <param name="commandId"></param>
        /// <param name="commandTypeId"></param>
        /// <param name="dayTypeId"></param>
        /// <returns></returns>
        public PSSRegularizationUnknownPoliceOfficerDeploymentContributionLogDTO GetDeploymentContributionLog(long requestId, long invoiceId, int commandId, int commandTypeId, int dayTypeId)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSSRegularizationUnknownPoliceOfficerDeploymentContributionLog>().Where(x => (x.Request.Id == requestId) && (x.Invoice.Id == invoiceId) && (x.GenerateRequestWithoutOfficersUploadBatchItemsStaging.Command.Id == commandId) && (x.GenerateRequestWithoutOfficersUploadBatchItemsStaging.CommandType == commandTypeId) && (x.GenerateRequestWithoutOfficersUploadBatchItemsStaging.DayType == dayTypeId))
                    .Select(x => new PSSRegularizationUnknownPoliceOfficerDeploymentContributionLogDTO
                    {
                        DeploymentRate = x.DeploymentRate,
                        DeploymentAllowancePercentage = x.DeploymentAllowancePercentage,
                        NumberOfOfficers = x.GenerateRequestWithoutOfficersUploadBatchItemsStaging.NumberOfOfficers
                    }).SingleOrDefault();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}