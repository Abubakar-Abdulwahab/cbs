using NHibernate.Criterion;
using NHibernate.Engine;
using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.FileUpload.OSGOFImplementation;
using Parkway.CBS.FileUpload.OSGOFImplementation.Models;
using Parkway.CBS.OSGOF.Admin.Models;
using Parkway.CBS.OSGOF.Admin.Services.Contracts;
using Parkway.CBS.OSGOF.Admin.ViewModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Parkway.CBS.OSGOF.Admin.Services
{
    public class CellSiteStagingManager : BaseManager<CellSitesStaging>, ICellSiteStagingManager<CellSitesStaging>
    {
        private readonly IRepository<CellSitesStaging> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public CellSiteStagingManager(IRepository<CellSitesStaging> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _transactionManager = orchardServices.TransactionManager;
            _repository = repository;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Save cell sites into staging
        /// </summary>
        /// <param name="cellSitesFromFile"></param>
        /// <param name="taxProfileId"></param>
        /// <param name="adminUser"></param>
        /// <param name="loggedInUser"></param>
        /// <param name="filePath"></param>
        public void SaveRecords(ConcurrentStack<OSGOFCellSitesExcelModel> cellSitesFromFile, TaxEntity taxProfile, UserPartRecord adminUser, CBSUser loggedInUser, CellSitesScheduleStaging batchRecord)
        {
            Logger.Information("Saving CellSitesStaging records ");
            //save entities into temp table
            int chunkSize = 500000;
            var dataSize = cellSitesFromFile.Count;

            double pageSize = ((double)dataSize / (double)chunkSize);
            int pages = 0;

            if (pageSize < 1 && dataSize >= 1) { pages = 1; }
            else { pages = (int)Math.Ceiling(pageSize); }
            int stopper = 0;
            int skip = 0;

            bool addedByAdmin = adminUser != null ? true : false;
            int adminUserId = adminUser != null ? adminUser.Id : 0;
            Int64 cbsUserId = loggedInUser != null ? loggedInUser.Id : 0;

            try
            {
                while (stopper < pages)
                {
                    var dataTable = new DataTable("Parkway_CBS_OSGOF_Admin_" + typeof(CellSitesStaging).Name);
                    dataTable.Columns.Add(new DataColumn("SNOnFile", typeof(int)));
                    dataTable.Columns.Add(new DataColumn("SNOnFileFileValue", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("OperatorSiteId", typeof(string)));
                    //dataTable.Columns.Add(new DataColumn("OSGOFId", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("YearOfDeployment", typeof(Int32)));
                    dataTable.Columns.Add(new DataColumn("YearOfDeploymentFileValue", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("HeightOfTower", typeof(decimal)));
                    dataTable.Columns.Add(new DataColumn("HeightOfTowerFileValue", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("MastType", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("Long", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("Lat", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("SiteAddress", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("Region", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("State_Id", typeof(Int32)));
                    dataTable.Columns.Add(new DataColumn("StateFileValue", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("LGA_Id", typeof(Int32)));
                    dataTable.Columns.Add(new DataColumn("LGAFileValue", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("Schedule_Id", typeof(Int64)));
                    dataTable.Columns.Add(new DataColumn("HasErrors", typeof(bool)));
                    dataTable.Columns.Add(new DataColumn("ErrorMessages", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("CreatedAtUtc", typeof(DateTime)));
                    dataTable.Columns.Add(new DataColumn("UpdatedAtUtc", typeof(DateTime)));

                    cellSitesFromFile.Skip(skip).Take(chunkSize).ToList().ForEach(x =>
                    {
                        var row = dataTable.NewRow();
                        //row["AddedByAdmin"] = addedByAdmin;
                        //row["AdminUser_Id"] = adminUserId;
                        //row["OperatorUser_Id"] = cbsUserId;
                        //row["TaxProfile_Id"] = taxProfileId;
                        row["SNOnFile"] = x.SN.Value;
                        row["SNOnFileFileValue"] = x.SN.StringValue;
                        row["OperatorSiteId"] = x.OperatorSiteId.Value;
                        //row["OSGOFId"] = x.Month.Value;
                        row["YearOfDeployment"] = x.YearofDeployment.Value;// x.YearofDeployment..StringValue;
                        row["YearOfDeploymentFileValue"] = x.YearofDeployment.StringValue;// x.YearofDeployment..StringValue;
                        row["HeightOfTower"] = x.HeightofTower.Value;
                        row["HeightOfTowerFileValue"] = x.HeightofTower.StringValue;
                        row["MastType"] = x.TowerMastType.Value;
                        row["Long"] = x.Longitude.Value;
                        row["Lat"] = x.Latitude.Value;
                        row["SiteAddress"] = x.SiteAddress.Value;
                        row["Region"] = x.Region.Value;
                        row["State_Id"] = x.State.ValueId;// x.State.;
                        row["StateFileValue"] = x.State.Value;// x.State.;
                        row["LGA_Id"] = x.LGA.ValueId;// batchRecordId;LGAFileValue
                        row["LGAFileValue"] = x.LGA.Value;// batchRecordId;LGAFileValue
                        row["Schedule_Id"] = batchRecord.Id;// batchRecordId;
                        row["HasErrors"] = x.HasError;
                        row["ErrorMessages"] = x.ErrorMessages;
                        row["CreatedAtUtc"] = DateTime.Now.ToLocalTime();
                        row["UpdatedAtUtc"] = DateTime.Now.ToLocalTime();
                        dataTable.Rows.Add(row);
                    });
                    Logger.Information(string.Format("Insertion for direct assessment batch payee records  has started Size: {0} Skip: {1}", dataSize, skip));

                    if (!SaveBundle(dataTable, "Parkway_CBS_OSGOF_Admin_" + typeof(CellSitesStaging).Name))
                    { throw new Exception("Error saving excel file details for CellSitesStaging "); }

                    skip += chunkSize;
                    stopper++;
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message); throw;
            }
        }


        /// <summary>
        /// Get the IEnumerable of cell sites in the staging table with the given schedule ref
        /// </summary>
        /// <param name="scheduleId"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns>IEnumerable{Models.CellSitesStaging}</returns>
        public IEnumerable<CellSitesStaging> GetCellSitesForScheduleStaging(Int64 scheduleId, int take, int skip)
        {
            return _transactionManager.GetSession().CreateCriteria<CellSitesStaging>()
                .Add(Restrictions.Eq("Schedule", new CellSitesScheduleStaging { Id = scheduleId }))
                .SetFirstResult(skip).SetMaxResults(take).AddOrder(Order.Asc("SNOnFile")).Future<CellSitesStaging>();
        }

        /// <summary>
        /// Get the number of records with errors
        /// </summary>
        /// <param name="id"></param>
        /// <returns>NHibernate.IFutureValue{Int32}</returns>
        public NHibernate.IFutureValue<Int32> GetCellSitesWithErrors(long batchId)
        {
            return _transactionManager.GetSession().CreateCriteria<CellSitesStaging>()
                .Add(Restrictions.Eq("Schedule", new CellSitesScheduleStaging { Id = batchId }))
                .Add(Restrictions.Eq("HasErrors", true))
                .SetProjection(
                Projections.ProjectionList()
                    .Add(Projections.Count("Id")))
                .FutureValue<Int32>();
        }

        /// <summary>
        /// Move valid cell sites from staging to main table
        /// </summary>
        /// <param name="id">Schedule identifier</param>
        public void DoTransferFromStagingToMainTable(CellSitesScheduleStagingVM scheduleVM, UserPartRecord approvedBy)
        {
            using (var session = _transactionManager.GetSession().SessionFactory.OpenStatelessSession())
            {
                using (var tranx = session.BeginTransaction())
                {
                    try
                    {
                        var tableNameForSchedule = "Parkway_CBS_OSGOF_Admin_" + typeof(CellSitesScheduleStaging).Name;
                        var tableNameForCellSites = "Parkway_CBS_OSGOF_Admin_" + typeof(CellSites).Name;
                        var tableNameForCellSitesStaging = "Parkway_CBS_OSGOF_Admin_" + typeof(CellSitesStaging).Name;

                        var queryText =
$"INSERT INTO Parkway_CBS_OSGOF_Admin_CellSites (AddedByAdmin, AdminUser_Id, OperatorUser_Id, TaxProfile_Id, OperatorCategory_Id, OperatorSiteId, OperatorSitePrefix, YearOfDeployment, HeightOfTower, MastType, Long, Lat, SiteAddress, Region, State_Id, LGA_Id, Approved, ApprovedBy_Id, CreatedAtUtc, UpdatedAtUtc) SELECT :AddedByAdmin, :AdminUserId, :OperatorId, :ProfileId, :OperatorCategoryId, OperatorSiteId, :uniqueIdPrefix, YearOfDeployment, HeightOfTower, MastType, Long, Lat, SiteAddress, Region, State_Id, LGA_Id, :approved, :approvedBy, :dateSaved, :dateSaved FROM Parkway_CBS_OSGOF_Admin_CellSitesStaging f	WHERE f.Schedule_Id = :stagingId And f.HasErrors = :hasErrors";

                        var query = session.CreateSQLQuery(queryText);
                        query.SetParameter("AddedByAdmin", scheduleVM.AddedByAdmin);
                        query.SetParameter("AdminUserId", scheduleVM.AdminUserId);
                        query.SetParameter("OperatorId", scheduleVM.OperatorId);
                        query.SetParameter("ProfileId", scheduleVM.ProfileId);
                        query.SetParameter("uniqueIdPrefix", scheduleVM.OSGOFSiteIdPrefix);
                        query.SetParameter("OperatorCategoryId", scheduleVM.OperatorCategoryId);

                        query.SetParameter("approved", true);
                        query.SetParameter("approvedBy", approvedBy.Id);
                        query.SetParameter("dateSaved", DateTime.Now.ToLocalTime());
                        query.SetParameter("stagingId", scheduleVM.StagingId);
                        query.SetParameter("hasErrors", false);

                        query.ExecuteUpdate();
                        //tranx.Commit();
                        //once the cell sites have been moved from staging to main table
                        //lets mark the schedule as treated
                        var updateQuery = $"UPDATE Parkway_CBS_OSGOF_Admin_CellSitesScheduleStaging SET Treated = :treatedValue WHERE Id = :scheduleId";
                        var updateSessionquery = session.CreateSQLQuery(updateQuery);
                        updateSessionquery.SetParameter("treatedValue", true);
                        updateSessionquery.SetParameter("scheduleId", scheduleVM.StagingId);
                        updateSessionquery.ExecuteUpdate();
                        tranx.Commit();
                    }
                    catch (Exception exception)
                    {
                        Logger.Error(exception, string.Format("Could not save object "));
                        tranx.Rollback();
                        throw;
                    }
                }
            }
            Logger.Error("CellSitesStaging Records have been moved to CellSites");
        }
    }
}