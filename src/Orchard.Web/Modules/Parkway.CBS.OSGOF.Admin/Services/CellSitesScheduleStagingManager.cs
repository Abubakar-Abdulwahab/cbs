using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.OSGOF.Admin.Models;
using Parkway.CBS.OSGOF.Admin.Services.Contracts;
using Parkway.CBS.OSGOF.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.OSGOF.Admin.Services
{
    public class CellSitesScheduleStagingManager : BaseManager<CellSitesScheduleStaging>, ICellSiteScheduleStagingManager<CellSitesScheduleStaging>
    {
        private readonly IRepository<CellSitesScheduleStaging> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;


        public CellSitesScheduleStagingManager(IRepository<CellSitesScheduleStaging> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _transactionManager = orchardServices.TransactionManager;
            _repository = repository;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Get details for this schedule
        /// </summary>
        /// <param name="scheduleBatchRef"></param>
        /// <returns>CellSitesScheduleVM</returns>
        /// <exception cref="Exception"></exception>
        public CellSitesScheduleVM GetScheduleDetails(string scheduleBatchRef)
        {
            try
            {
                return _transactionManager.GetSession().Query<CellSitesScheduleStaging>()
                            .Where(sc => (sc.BatchRef == scheduleBatchRef))
                            .Select(sc => new CellSitesScheduleVM()
                            {
                                PayerId = sc.TaxProfile.PayerId,
                                Id = sc.Id,
                                TotalNoOfRowsProcessed = sc.TotalNoOfRowsProcessed,
                                ScheduleHasAlreadyBeenTreated = sc.Treated, 
                            }).SingleOrDefault();
            }
            catch (Exception exception)
            { Logger.Error(exception, exception.Message); throw; }
        }


        /// <summary>
        /// Get details for this schedule
        /// </summary>
        /// <param name="scheduleBatchRef"></param>
        /// <returns>CellSitesScheduleStagingVM</returns>
        /// <exception cref="Exception"></exception>
        public CellSitesScheduleStagingVM GetScheduleStatgingDetails(string scheduleBatchRef)
        {
            try
            {
                return _transactionManager.GetSession().Query<CellSitesScheduleStaging>()
                            .Where(sc => (sc.BatchRef == scheduleBatchRef))
                            .Select(sc => new CellSitesScheduleStagingVM()
                            {
                                StagingId = sc.Id,
                                AddedByAdmin = sc.AddedByAdmin,
                                AdminUserId = sc.AdminUser == null? 0 : sc.AdminUser.Id,
                                ProfileId = sc.TaxProfile.Id,
                                OperatorId = sc.OperatorUser == null ? 0 : sc.OperatorUser.Id,
                                ScheduleHasAlreadyBeenTreated = sc.Treated,
                                PayerId = sc.TaxProfile.PayerId,
                                Name = sc.TaxProfile.Recipient,
                                OSGOFSiteIdPrefix = sc.OperatorCategory.ShortName,
                                OperatorCategoryId = sc.OperatorCategory.Id,
                            }).SingleOrDefault();
            }
            catch (Exception exception)
            { Logger.Error(exception, exception.Message); throw; }
        }
    }
}