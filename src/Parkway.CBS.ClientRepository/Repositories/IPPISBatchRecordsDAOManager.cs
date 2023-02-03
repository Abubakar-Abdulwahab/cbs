using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Payee.PayeeAdapters.IPPIS;
using System;
using System.Collections.Concurrent;
using System.Data;
using System.Linq;

namespace Parkway.CBS.ClientRepository.Repositories
{
    public class IPPISBatchRecordsDAOManager : Repository<IPPISBatchRecords>, IIPPISBatchRecordsDAOManager
    {
        public IPPISBatchRecordsDAOManager(IUoW uow) : base(uow)
        { }


        /// <summary>
        /// Save cell sites
        /// </summary>
        /// <param name="recordId"></param>
        /// <param name="cellSitesObj"></param>
        public int SaveIPPISRecords(Int64 recordId, ConcurrentStack<IPPISAssessmentLineRecordModel> payees)
        {
            int chunkSize = 500000;
            var dataSize = payees.Count;

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
                    var dataTable = new DataTable("Parkway_CBS_Core_" + typeof(IPPISBatchRecords).Name);
                    dataTable.Columns.Add(new DataColumn("IPPISBatch_Id", typeof(Int64)));
                    dataTable.Columns.Add(new DataColumn("MinistryName", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("TaxPayerCode", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("EmployeeNumber", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("PayeeName", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("GradeLevel", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("Step", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("Address", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("Email", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("PhoneNumber", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("TaxStringValue", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("Tax", typeof(decimal)));
                    dataTable.Columns.Add(new DataColumn("HasErrors", typeof(bool)));
                    dataTable.Columns.Add(new DataColumn("ErrorMessages", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("CreatedAtUtc", typeof(DateTime)));
                    dataTable.Columns.Add(new DataColumn("UpdatedAtUtc", typeof(DateTime)));

                    payees.Skip(skip).Take(chunkSize).ToList().ForEach(x =>
                    {
                        var row = dataTable.NewRow();
                        row["IPPISBatch_Id"] = recordId;
                        row["MinistryName"] = x.Ministry_Name.Value;
                        row["TaxPayerCode"] = x.Org_Code.Value;
                        row["EmployeeNumber"] = x.Employee_Number.Value;
                        row["PayeeName"] = x.Employee_Name.Value;
                        row["GradeLevel"] = x.Grade_Level.Value;
                        row["Step"] = x.Step.Value;
                        row["Address"] = x.Contact_Address.Value;
                        row["Email"] = x.Email_Address.Value;
                        row["PhoneNumber"] = x.Mobile_Phone.Value;
                        row["TaxStringValue"] = x.Tax.StringValue;
                        row["Tax"] = x.Tax.Value;
                        row["HasErrors"] = x.HasError;
                        row["ErrorMessages"] = x.ErrorMessages;
                        row["CreatedAtUtc"] = DateTime.Now.ToLocalTime();
                        row["UpdatedAtUtc"] = DateTime.Now.ToLocalTime();
                        dataTable.Rows.Add(row);
                    });

                    if (!SaveBundle(dataTable, "Parkway_CBS_Core_" + typeof(IPPISBatchRecords).Name))
                    { throw new Exception("Error saving details for batch Id " + recordId); }

                    skip += chunkSize;
                    stopper++;
                }
            }
            catch (Exception)
            { throw; }

            return dataSize;
        }


        public void GroupRecordsFromIPPISBatchRecordsTableByTaxPayerCode(long batchId)
        {
            try
            {
                var queryText =
$"INSERT INTO Parkway_CBS_Core_IPPISTaxPayerSummary (IPPISBatch_Id, TaxPayerCode, NumberofEmployees, TotalTaxAmount, CreatedAtUtc, UpdatedAtUtc) SELECT :batch_Id, TaxPayerCode, Count(Id) as NumberOfRecords, SUM(Tax) as TotalTaxAmount, :dateSaved, :dateSaved  FROM Parkway_CBS_Core_IPPISBatchRecords WHERE IPPISBatch_Id = :batch_Id GROUP BY TaxPayerCode, IPPISBatch_Id";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("dateSaved", DateTime.Now.ToLocalTime());
                query.SetParameter("batch_Id", batchId);

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }


        /// <summary>
        /// Map the TaxPayerCode to the Id of the TaxEntity that has the same agency code as the TaxPayerCode
        /// </summary>
        /// <param name="batchId"></param>
        public void MapTaxPayerCodeToTaxEntityId(long batchId)
        {
            try
            {
                var queryText = $"UPDATE ip SET ip.TaxEntity_Id = t.Id, ip.TaxEntityCategory_Id = t.TaxEntityCategory_Id FROM Parkway_CBS_Core_IPPISTaxPayerSummary ip INNER JOIN Parkway_CBS_Core_TaxEntity as t ON t.TaxPayerCode = ip.TaxPayerCode WHERE ip.IPPISBatch_id = :batch_Id";
                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("batch_Id", batchId);

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }


        /// <summary>
        /// Set the value of TaxEntity_Id == null to unknown tax entity value
        /// </summary>
        /// <param name="batchId"></param>
        public void MapNullTaxPayerCodeToUnknownTaxEntityId(long batchId, Int64 unknownId)
        {
                try
                {
                //var queryText = $"UPDATE ip SET ip.TaxEntity_Id = :unknownId, , ip.TaxEntityCategory_Id = :categoryId FROM Parkway_CBS_Core_IPPISTaxPayerSummary ip WHERE ip.IPPISBatch_id = :batch_Id AND ip.TaxEntity_Id IS NULL";
                var queryText = $"UPDATE ip SET ip.TaxEntity_Id = :unknownId, ip.TaxEntityCategory_Id = t.TaxEntityCategory_Id FROM Parkway_CBS_Core_IPPISTaxPayerSummary ip INNER JOIN Parkway_CBS_Core_TaxEntity as t ON t.Id = :unknownId WHERE ip.IPPISBatch_id = :batch_Id AND ip.TaxEntity_Id IS NULL";

                var query = _uow.Session.CreateSQLQuery(queryText);
                    query.SetParameter("batch_Id", batchId);
                    query.SetParameter("unknownId", unknownId);

                query.ExecuteUpdate();
                }
                catch (Exception)
                { throw; }
        }
    }
}
