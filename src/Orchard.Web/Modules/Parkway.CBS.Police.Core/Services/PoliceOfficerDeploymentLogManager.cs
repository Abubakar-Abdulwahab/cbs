using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class PoliceOfficerDeploymentLogManager : BaseManager<PoliceOfficerDeploymentLog>, IPoliceOfficerDeploymentLogManager<PoliceOfficerDeploymentLog>
    {
        private readonly IRepository<PoliceOfficerDeploymentLog> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public ILogger Logger { get; set; }


        public PoliceOfficerDeploymentLogManager(IRepository<PoliceOfficerDeploymentLog> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }


        /// <summary>
        /// Get police officer deployment log vm with specified deployment log id
        /// </summary>
        /// <param name="deploymentLogId"></param>
        /// <returns></returns>
        public PoliceOfficerDeploymentLogVM GetPoliceOfficerDeploymentLogVM(Int64 deploymentLogId)
        {
            return _transactionManager.GetSession().Query<PoliceOfficerDeploymentLog>().Where(x => x.Id == deploymentLogId).Select(log => new PoliceOfficerDeploymentLogVM {
                Id = log.Id,
                Address = log.Address,
                CustomerName = log.Request.CBSUser.Name,
                StartDate = log.StartDate,
                EndDate = log.EndDate,
                ServiceTypeName = log.Request.Service.Name,
                PoliceOfficerName = log.PoliceOfficerLog.Name,
                CommandName = log.Command.Name,
                SelectedState = log.State.Id,
                SelectedLGA = log.LGA.Id,
                InvoiceId = log.Invoice.Id,
                RequestId = log.Request.Id,
                OfficerRankName = log.PoliceOfficerLog.Rank.RankName,
                SelectedOfficerRank = log.PoliceOfficerLog.Rank.Id,
                BatchId = log.PoliceOfficerLog.IdentificationNumber,
                IsActive = log.IsActive,
                Status = log.Status,
                PoliceOfficerDeploymentLog = log
            }).FirstOrDefault();
        }

        /// <summary>
        /// Get count for number of active deployed officers
        /// </summary>
        /// <returns>IEnumerable<ReportStatsVM></returns>
        public IEnumerable<ReportStatsVM> GetActiveDeployedPoliceOfficer()
        {
            return _transactionManager.GetSession().CreateCriteria<PoliceOfficerDeploymentLog>()
                .Add(Restrictions.Where<PoliceOfficerDeploymentLog>(x => x.Status == (int)DeploymentStatus.Running))
            .SetProjection(
                        Projections.ProjectionList()
                            .Add(Projections.Count<PoliceOfficerDeploymentLog>(x => x.Id), "TotalRecordCount")
                    ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }

        /// <summary>
        /// Gets the number of officers assigned to the request with the specified file number
        /// </summary>
        /// <param name="fileNumber"></param>
        /// <returns>int</returns>
        public int GetEscortOfficerAssignedNumber(string fileNumber)
        {
            return _transactionManager.GetSession().Query<PoliceOfficerDeploymentLog>().Count(x => x.Request.FileRefNumber == fileNumber);
        }
    }
}