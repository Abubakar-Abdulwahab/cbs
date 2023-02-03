using Orchard;
using System;
using System.Data;
using System.Linq;
using Orchard.Data;
using Orchard.Logging;
using NHibernate.Linq;
using Orchard.Users.Models;
using Parkway.CBS.FileUpload;
using Parkway.CBS.Core.Services;
using Parkway.CBS.OSGOF.Admin.Models;
using Parkway.CBS.OSGOF.Admin.Services.Contracts;
using Parkway.CBS.OSGOF.Admin.ViewModels;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace Parkway.CBS.OSGOF.Admin.Services
{
    public class CellSitesPaymentManager : BaseManager<CellSitesPayment>, ICellSitesPaymentManager<CellSitesPayment>
    {
        private readonly IRepository<CellSitesPayment> _repository;
        private readonly IOrchardServices _orchardServices;
        private readonly IRepository<UserPartRecord> _user;
        private readonly ITransactionManager _transactionManager;

        public CellSitesPaymentManager(IRepository<CellSitesPayment> repository, IRepository<UserPartRecord> user, IOrchardServices orchardService) : base(repository, user, orchardService)
        {
            _transactionManager = orchardService.TransactionManager;
            _repository = repository;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Save cell sites
        /// </summary>
        /// <param name="recordId"></param>
        /// <param name="cellSitesObj"></param>
        public void SaveCellSites(Int64 recordId, CellSitesBreakDown cellSitesObj)
        {
            Logger.Information("Saving direct assessment payee records for batch id " + recordId);
            int chunkSize = 500000;
            var dataSize = cellSitesObj.CellSiteModelV2.Count;

            double pageSize = ((double)dataSize / (double)chunkSize);
            int pages = 0;

            if (pageSize < 1 && dataSize >= 1) { pages = 1; }
            else { pages = (int)Math.Ceiling(pageSize); }
            int stopper = 0;
            int skip = 0;

            try
            {
                while (stopper < pages)
                {
                    var dataTable = new DataTable("Parkway_CBS_OSGOF_Admin_" + typeof(CellSitesPayment).Name);
                    dataTable.Columns.Add(new DataColumn("CellSiteClientPaymentBatch_Id", typeof(Int64)));
                    dataTable.Columns.Add(new DataColumn("CellSiteId", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("Reference", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("Year", typeof(int)));
                    dataTable.Columns.Add(new DataColumn("YearStringValue", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("HasErrors", typeof(bool)));
                    dataTable.Columns.Add(new DataColumn("ErrorMessage", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("Amount", typeof(decimal)));
                    dataTable.Columns.Add(new DataColumn("AssessmentDate", typeof(DateTime)));
                    dataTable.Columns.Add(new DataColumn("CreatedAtUtc", typeof(DateTime)));
                    dataTable.Columns.Add(new DataColumn("UpdatedAtUtc", typeof(DateTime)));

                    cellSitesObj.CellSiteModelV2.Skip(skip).Take(chunkSize).ToList().ForEach(x =>
                    {
                        var row = dataTable.NewRow();
                        row["CellSiteClientPaymentBatch_Id"] = recordId;
                        row["CellSiteId"] = x.CellSiteValue;
                        row["Reference"] = x.Ref;
                        row["Year"] = x.Year;
                        row["YearStringValue"] = x.YearStringValue;
                        row["HasErrors"] = x.HasError;
                        row["ErrorMessage"] = x.ErrorMessages;
                        row["Amount"] = 0.00m;
                        row["AssessmentDate"] = x.AssessmentDate.HasValue ? (object)x.AssessmentDate.Value : DBNull.Value;
                        row["CreatedAtUtc"] = DateTime.Now.ToLocalTime();
                        row["UpdatedAtUtc"] = DateTime.Now.ToLocalTime();
                        dataTable.Rows.Add(row);
                    });
                    Logger.Information(string.Format("Insertion for cell sites for osgof batch payee records  has started Size: {0} Skip: {1}", dataSize, skip));

                    if (!SaveBundle(dataTable, "Parkway_CBS_OSGOF_Admin_" + typeof(CellSitesPayment).Name))
                    { throw new Exception("Error saving details for batch Id " + recordId); }

                    skip += chunkSize;
                    stopper++;
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message); throw;
            }
            Logger.Information(string.Format("SIZE: {0}", dataSize));
        }


        public void RunComparisonForCellSites(long batchId)
        {
            using (var session = _transactionManager.GetSession().SessionFactory.OpenStatelessSession())
            {
                using (var tranx = session.BeginTransaction())
                {
                    try
                    {
                        var queryText =
$"UPDATE cp SET cp.CellSite_Id = c.Id, cp.Amount = op.Amount FROM Parkway_CBS_OSGOF_Admin_CellSitesPayment cp INNER JOIN Parkway_CBS_OSGOF_Admin_CellSites as c ON c.OperatorSiteId = cp.CellSiteId " +
$"INNER JOIN Parkway_CBS_OSGOF_Admin_OperatorCategory as op ON c.OperatorCategory_Id = op.Id WHERE cp.HasErrors = :hasErrors AND cp.CellSiteClientPaymentBatch_Id = :batchId";

                        var query = session.CreateSQLQuery(queryText);
                        query.SetParameter("hasErrors", false);
                        query.SetParameter("batchId", batchId);

                        query.ExecuteUpdate();
                        //tranx.Commit();
                        //once the cell sites have been moved from staging to main table
                        //lets mark the schedule as treated
                        var updateQuery = $"UPDATE Parkway_CBS_OSGOF_Admin_CellSiteClientPaymentBatch SET Processed = :processedValue WHERE Id = :id";
                        var updateSessionquery = session.CreateSQLQuery(updateQuery);
                        updateSessionquery.SetParameter("processedValue", true);
                        updateSessionquery.SetParameter("id", batchId);
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


        /// <summary>
        /// Get the list of cell sites for this record I
        /// </summary>
        /// <param name="record"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public CellSiteReportQueryObj GetRecords(CellSiteClientPaymentBatch record, int take, int skip, bool getStats = false)
        {
            var listResult = _transactionManager.GetSession().Query<CellSitesPayment>()
                            .Where(cellSite => (cellSite.CellSiteClientPaymentBatch == record)).Skip(skip).Take(take)
                            .Select(cellSite => new CellSiteReturnModelVM
                            {
                                CellSiteMatchFound = cellSite.CellSite != null ? true : false,
                                OSGOFId = cellSite.CellSite != null ? cellSite.CellSite.OSGOFId : " ",
                                CellSite = cellSite.CellSiteId,
                                ErrorMessage = cellSite.CellSite != null ? cellSite.ErrorMessage : (cellSite.ErrorMessage + "| No cellsite found").TrimStart('|'),
                                HasError = cellSite.HasErrors || cellSite.CellSite == null,
                                LGA = cellSite.CellSite != null ? cellSite.CellSite.LGA.Name : " ",
                                State = cellSite.CellSite != null ? cellSite.CellSite.State.Name : " ",
                                Year = string.IsNullOrEmpty(cellSite.YearStringValue) ? " " : cellSite.YearStringValue,
                                AmountValue = string.Format("{0:n2}", cellSite.Amount),
                                Address = cellSite.CellSite != null ? cellSite.CellSite.SiteAddress : " ",
                                Coords = cellSite.CellSite != null ? (cellSite.CellSite.Lat + "|" + cellSite.CellSite.Long) : " ",
                                Ref = cellSite.Reference,
                            }).ToFuture();

            if (getStats)
            {
                var statsResultVar = _transactionManager.GetSession().CreateCriteria<CellSitesPayment>()
                .Add(Restrictions.Eq("CellSiteClientPaymentBatch", record))
                .Add(Restrictions.Eq("HasErrors", false))
                .Add(Restrictions.IsNotNull("CellSite"))
                .SetProjection(Projections.ProjectionList()
                .Add(Projections.Sum("Amount"), "TotalAmount")
                .Add(Projections.Count("Id"), "TotalNumberOfValidRecords"))
                .SetResultTransformer(Transformers.AliasToBean<CellSiteStatsHolder>()).Future<CellSiteStatsHolder>();

                var statResult = statsResultVar.FirstOrDefault();
                if (statResult == null) { statResult = new CellSiteStatsHolder { }; }

                return new CellSiteReportQueryObj { CellSites = listResult, RecordsWithoutErrors = statResult.TotalNumberOfValidRecords, TotalAmount = statResult.TotalAmount };
            }
            else
            {
                return new CellSiteReportQueryObj { CellSites = listResult };
            }
            
        }

        /// <summary>
        /// Get the total amount due on the given schedule
        /// </summary>
        /// <param name="record"></param>
        /// <returns>decimal</returns>
        public decimal GetTotalAmountForSchedule(CellSiteClientPaymentBatch record)
        {
            var statsResultVar = _transactionManager.GetSession().CreateCriteria<CellSitesPayment>()
                .Add(Restrictions.Eq("CellSiteClientPaymentBatch", record))
                .Add(Restrictions.Eq("HasErrors", false))
                .Add(Restrictions.IsNotNull("CellSite"))
                .SetProjection(Projections.ProjectionList()
                .Add(Projections.Sum("Amount"), "TotalAmount"))
                .SetResultTransformer(Transformers.AliasToBean<CellSiteStatsHolder>()).Future<CellSiteStatsHolder>();

            var statResult = statsResultVar.FirstOrDefault();
            if (statResult == null) { throw new Exception("Could not find amount for batch record " + record.Id); }
            return statResult.TotalAmount;
        }


        /// <summary>
        /// Delete all the cell site (child) records for this batch, 
        /// </summary>
        /// <param name="record"></param>
        public void Delete(CellSiteClientPaymentBatch record)
        {
            using (var session = _transactionManager.GetSession().SessionFactory.OpenStatelessSession())
            {
                using (var tranx = session.BeginTransaction())
                {
                    try
                    {
                        var query = session.CreateQuery("Delete from " + typeof(CellSitesPayment) + " where CellSiteClientPaymentBatch_Id = :id");
                        query.SetParameter("id", record.Id);
                        query.ExecuteUpdate();
                        tranx.Commit();
                    }
                    catch (Exception exception)
                    {
                        Logger.Error(exception, string.Format("Could not delete batch record object ", record.Id));
                        tranx.Rollback();
                        throw;
                    }
                }
            }
            Logger.Error(string.Format("Records have been deleted {0}", record.Id));
        }

    }
}