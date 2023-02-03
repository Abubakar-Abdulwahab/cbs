using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.Services.HelperModel;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories
{
    public class DeploymentAllowanceDAOManager : Repository<PoliceofficerDeploymentAllowance>, IDeploymentAllowanceDAOManager
    {
        public DeploymentAllowanceDAOManager(IUoW uow) : base(uow)
        { }

        /// <summary>
        /// Save bundle of records
        /// </summary>
        /// <param name="deploymentAllowances"></param>
        /// <param name="batchLimit"></param>
        public int SaveRecords(ConcurrentQueue<PoliceOfficerDeploymentAllowanceVM> deploymentAllowances, int batchLimit)
        {
            int chunkSize = batchLimit;
            var dataSize = deploymentAllowances.Count;

            double pageSize = ((double)dataSize / chunkSize);
            int pages = 0;

            if (pageSize < 1 && dataSize >= 1) { pages = 1; }
            else { pages = (int)Math.Ceiling(pageSize); }
            int stopper = 0;
            int skip = 0;

            List<DataTable> listOfDataTables = new List<DataTable> { };
            try
            {
                while (stopper < pages)
                {
                    var dataTable = new DataTable("Parkway_CBS_Police_Core_" + typeof(PoliceofficerDeploymentAllowance).Name);
                    dataTable.Columns.Add(new DataColumn("Status", typeof(int)));
                    dataTable.Columns.Add(new DataColumn("Amount", typeof(decimal)));
                    dataTable.Columns.Add(new DataColumn("EscortDetails_Id", typeof(long)));
                    dataTable.Columns.Add(new DataColumn("Narration", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("ContributedAmount", typeof(decimal)));
                    dataTable.Columns.Add(new DataColumn("Request_Id", typeof(long)));
                    dataTable.Columns.Add(new DataColumn("Invoice_Id", typeof(long)));
                    dataTable.Columns.Add(new DataColumn("PaymentStage", typeof(int)));
                    dataTable.Columns.Add(new DataColumn("Command_Id", typeof(int)));
                    dataTable.Columns.Add(new DataColumn("PoliceOfficerLog_Id", typeof(long)));
                    dataTable.Columns.Add(new DataColumn("CreatedAtUtc", typeof(DateTime)));
                    dataTable.Columns.Add(new DataColumn("UpdatedAtUtc", typeof(DateTime)));

                    deploymentAllowances.Skip(skip).Take(chunkSize).ToList().ForEach(x =>
                    {
                        x.AllowanceItems.ForEach(item =>
                        {
                            var row = dataTable.NewRow();
                            row["Status"] = item.Status;
                            row["Amount"] = item.Amount;
                            row["EscortDetails_Id"] = item.EscortDetailsId;
                            row["Narration"] = item.Narration;
                            row["ContributedAmount"] = item.ContributedAmount;
                            row["PaymentStage"] = item.PaymentStageId;
                            row["Request_Id"] = item.RequestId;
                            row["Invoice_Id"] = item.InvoiceId;
                            row["Command_Id"] = item.CommandId;
                            row["PoliceOfficerLog_Id"] = item.PoliceOfficerLogId;
                            row["CreatedAtUtc"] = DateTime.Now.ToLocalTime();
                            row["UpdatedAtUtc"] = DateTime.Now.ToLocalTime();
                            dataTable.Rows.Add(row);
                        });

                        //Update allowance tracker table
                        UpdateDeploymentTrackerDetails(x);
                    });
                    listOfDataTables.Add(dataTable);
                    skip += chunkSize;
                    stopper++;
                }
                //we now have a collection of datatables, lets save the bunch together
                if (!SaveBundle(listOfDataTables, "Parkway_CBS_Police_Core_" + typeof(PoliceofficerDeploymentAllowance).Name))
                { throw new Exception("Error saving items into  PoliceofficerDeploymentAllowance"); }
            }
            catch (Exception)
            {
                throw;
            }
            return 1;
        }

        /// <summary>
        /// Update deployment allowance tracker details
        /// </summary>
        /// <param name="deploymentAllowanceVM"></param>
        /// <returns>int</returns>
        private int UpdateDeploymentTrackerDetails(PoliceOfficerDeploymentAllowanceVM deploymentAllowanceVM)
        {
            try
            {
                var queryText = $"UPDATE dat SET dat.NextSettlementDate = :nextSettlementDate, dat.NumberOfSettlementDone = :numberOfSettlementDone, dat.IsSettlementCompleted = :isSettlementCompleted, dat.SettlementCycleStartDate = :settlementCycleStartDate, dat.SettlementCycleEndDate = :settlementCycleEndDate FROM Parkway_CBS_Police_Core_PoliceofficerDeploymentAllowanceTracker dat WHERE dat.Id = :deploymentTrackerId";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("deploymentTrackerId", deploymentAllowanceVM.DeploymentAllowanceTrackerId);
                query.SetParameter("nextSettlementDate", deploymentAllowanceVM.NextSettlementDate);
                query.SetParameter("settlementCycleStartDate", deploymentAllowanceVM.NextSettlementCycleStartDate);
                query.SetParameter("settlementCycleEndDate", deploymentAllowanceVM.NextSettlementCycleEndDate);
                query.SetParameter("isSettlementCompleted", deploymentAllowanceVM.IsSettlementCompleted);
                query.SetParameter("numberOfSettlementDone", deploymentAllowanceVM.NumberOfSettlementDone);

                return query.ExecuteUpdate(); ;
            }
            catch (Exception)
            { throw; }
        }

    }
}
