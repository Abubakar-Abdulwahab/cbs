using Parkway.CBS.ClientRepository.Repositories.NAGIS.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.FileUpload.NAGISImplementation.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientRepository.Repositories.NAGIS
{
    public class NagisOldInvoicesDAOManager : Repository<NagisOldInvoices>, INagisOldInvoicesDAOManager
    {
        public NagisOldInvoicesDAOManager(IUoW uow) : base(uow)
        {

        }

        public int SaveRecords(string tenantName, long recordId, ConcurrentStack<NAGISDataLineRecordModel> nagisDataLineRecords)
        {
            int chunkSize = 500000;
            var dataSize = nagisDataLineRecords.Count;

            double pageSize = ((double)dataSize / (double)chunkSize);
            int pages = 0;

            if (pageSize < 1 && dataSize >= 1) { pages = 1; }
            else { pages = (int)Math.Ceiling(pageSize); }
            int stopper = 0;
            int skip = 0;
            try
            {
                #region data column

                var dataTable = new DataTable("Parkway_CBS_Core_" + typeof(NagisOldInvoices).Name);
                dataTable.Columns.Add(new DataColumn("NagisDataBatch_Id", typeof(Int64)));
                dataTable.Columns.Add(new DataColumn("TaxEntityCategory_Id", typeof(int)));
                dataTable.Columns.Add(new DataColumn("OperationType_Id", typeof(int)));
                dataTable.Columns.Add(new DataColumn("CustomerName", typeof(string)));
                dataTable.Columns.Add(new DataColumn("Address", typeof(string)));
                dataTable.Columns.Add(new DataColumn("PhoneNumber", typeof(string)));
                dataTable.Columns.Add(new DataColumn("CustomerId", typeof(string)));
                dataTable.Columns.Add(new DataColumn("Amount", typeof(decimal)));
                dataTable.Columns.Add(new DataColumn("TIN", typeof(string)));
                dataTable.Columns.Add(new DataColumn("NagisInvoiceNumber", typeof(string)));
                dataTable.Columns.Add(new DataColumn("NagisInvoiceCreationDate", typeof(DateTime)));
                dataTable.Columns.Add(new DataColumn("RevenueHead_Id", typeof(Int64)));
                dataTable.Columns.Add(new DataColumn("ExternalRefId", typeof(string)));
                dataTable.Columns.Add(new DataColumn("InvoiceDescription", typeof(string)));
                dataTable.Columns.Add(new DataColumn("AmountDue", typeof(decimal)));
                dataTable.Columns.Add(new DataColumn("Quantity", typeof(int)));
                dataTable.Columns.Add(new DataColumn("Status", typeof(int)));
                dataTable.Columns.Add(new DataColumn("GroupId", typeof(int)));
                dataTable.Columns.Add(new DataColumn("CreatedAtUtc", typeof(DateTime)));
                dataTable.Columns.Add(new DataColumn("UpdatedAtUtc", typeof(DateTime)));
                
                #endregion

                Int64 counter = 0;
                while (stopper < pages)
                {
                    counter = (chunkSize * stopper);

                    nagisDataLineRecords.Skip(skip).Take(chunkSize).ToList().ForEach(x =>
                    {
                        var row = dataTable.NewRow();
                        row["NagisDataBatch_Id"] = recordId;
                        row["TaxEntityCategory_Id"] = x.TaxEntityCategoryID.Value;
                        row["OperationType_Id"] = (int)ReferenceDataOperationType.Create;
                        row["CustomerName"] = x.CustomerName.Value;
                        row["Address"] = x.Address.Value;
                        row["PhoneNumber"] = x.PhoneNumber.Value;
                        row["CustomerId"] = x.CustomerId.Value;
                        row["Amount"] = x.Amount.Value;
                        row["TIN"] = x.Tin.Value.Replace("-","");
                        row["NagisInvoiceNumber"] = x.InvoiceNumber.Value;
                        row["NagisInvoiceCreationDate"] = x.CreationDate.Value;
                        row["RevenueHead_Id"] = x.RevenueHeadId.Value;
                        row["ExternalRefId"] = x.ExternalRefId.Value;
                        row["InvoiceDescription"] = x.InvoiceDescription.Value;
                        row["AmountDue"] = x.AmountDue.Value;
                        row["Quantity"] = x.Quantity.Value;
                        row["Status"] = x.Status.Value;
                        row["GroupId"] = x.GroupID.Value;
                        row["CreatedAtUtc"] = DateTime.Now.ToLocalTime();
                        row["UpdatedAtUtc"] = DateTime.Now.ToLocalTime();

                        dataTable.Rows.Add(row);
                    });

                    if (!SaveBundle(dataTable, "Parkway_CBS_Core_" + typeof(NagisOldInvoices).Name))
                    { throw new Exception("Error saving details for batch Id " + recordId); }

                    skip += chunkSize;
                    stopper++;
                }
            }
            catch (Exception ex)
            { throw ex; }

            return dataSize;

        }

        public void UpdateNagisOldInvoicesStagingRecordsOperationType(long batchId)
        {
            try
            {
                var queryText = $"UPDATE nagis SET nagis.OperationType_Id = :OperationType_Id, nagis.TaxEntity_Id = t.Id FROM Parkway_CBS_Core_NagisOldInvoices nagis" +
                    $" INNER JOIN Parkway_CBS_Core_TaxEntity as t ON t.TaxEntityCategory_Id = nagis.TaxEntityCategory_Id AND t.PhoneNumber = nagis.PhoneNumber AND t.Recipient = nagis.CustomerName" +
                    $" AND t.Address = nagis.Address WHERE nagis.NagisDataBatch_Id = :batch_Id";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("OperationType_Id", (int)ReferenceDataOperationType.Update);
                query.SetParameter("batch_Id", batchId);

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }

        public void CreateTaxEntityWithNAGISDataRecords(long batchId)
        {
            try
            {
                var queryText = $"INSERT INTO Parkway_CBS_Core_TaxEntity (TaxPayerIdentificationNumber, Recipient, PhoneNumber, TaxEntityType, TaxEntityCategory_Id, Address, CreatedAtUtc, UpdatedAtUtc)" +
                    $" SELECT nagis.TIN, nagis.CustomerName, nagis.PhoneNumber, nagis.TaxEntityCategory_Id, nagis.TaxEntityCategory_Id, nagis.Address," +
                    $" :dateSaved, :dateSaved FROM Parkway_CBS_Core_NagisOldInvoices nagis INNER JOIN (SELECT CustomerId, MIN(Id) as Id FROM Parkway_CBS_Core_NagisOldInvoices" +
                    $" WHERE NagisDataBatch_Id = :batch_Id AND OperationType_Id = :OperationType_Id AND AmountDue > 0 GROUP BY CustomerId) AS nagisg ON nagis.Id = nagisg.Id";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("dateSaved", DateTime.Now.ToLocalTime());
                query.SetParameter("batch_Id", batchId);
                query.SetParameter("OperationType_Id", (int)ReferenceDataOperationType.Create);

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }

        public void UpdateTaxEntityWithNAGISDataRecords(long batchId)
        {
            try
            {
                var queryText = $"UPDATE t SET t.Address = nagis.Address FROM Parkway_CBS_Core_TaxEntity t" +
                    $" INNER JOIN Parkway_CBS_Core_NagisOldInvoices as nagis ON t.Id = nagis.TaxEntity_Id" +
                    $" WHERE nagis.NagisDataBatch_Id = :batch_Id AND nagis.OperationType_Id = :OperationType_Id";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("batch_Id", batchId);
                query.SetParameter("OperationType_Id", (int)ReferenceDataOperationType.Update);

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }

        public void UpdateNAGISDataStagingRecordsTaxEntityId(long batchId)
        {
            try
            {
                var queryText = $"UPDATE nagis SET nagis.TaxEntity_Id = t.TaxEntity_Id FROM Parkway_CBS_Core_NagisOldInvoices nagis" +
                    $" INNER JOIN Parkway_CBS_Core_NagisOldCustomers as t ON t.CustomerId = nagis.CustomerId WHERE nagis.NagisDataBatch_Id = :batch_Id";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("batch_Id", batchId);

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }

        public void GroupRecordsFromNAGISOldInvoicesRecordsTableByNAGISInvoiceNumber(long batchId)
        {
            try
            {
                var queryText = $"INSERT INTO Parkway_CBS_Core_NagisOldInvoiceSummary (NagisDataBatch_Id, NagisInvoiceNumber, GroupId, TaxEntity_Id, TaxEntityCategory_Id, NumberOfItems, TotalAmount, AmountDue, CreatedAtUtc, UpdatedAtUtc) SELECT :batch_Id, NagisInvoiceNumber, GroupId, TaxEntity_Id, TaxEntityCategory_Id, Count(Id) as NumberOfItems, SUM(Amount) as TotalAmount, SUM(AmountDue) as AmountDue, :dateSaved, :dateSaved  FROM Parkway_CBS_Core_NagisOldInvoices WHERE NagisDataBatch_Id = :batch_Id AND AmountDue > 0 GROUP BY NagisInvoiceNumber, GroupId, TaxEntityCategory_Id, NagisDataBatch_Id, TaxEntity_Id";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("dateSaved", DateTime.Now.ToLocalTime());
                query.SetParameter("batch_Id", batchId);

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }

        public void UpdateNAGISDataStagingRecordsNagisOldInvoiceSummaryId(long batchId)
        {
            try
            {
                var queryText = $"UPDATE nagis SET nagis.NagisOldInvoiceSummary_Id = summary.Id FROM Parkway_CBS_Core_NagisOldInvoices nagis" +
                    $" INNER JOIN Parkway_CBS_Core_NagisOldInvoiceSummary as summary ON summary.NagisInvoiceNumber = nagis.NagisInvoiceNumber AND " +
                    $" summary.NagisDataBatch_Id = nagis.NagisDataBatch_Id WHERE nagis.NagisDataBatch_Id = :batch_Id";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("batch_Id", batchId);
                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }

        public void CreateTaxEntityWithNAGISDataRecordsUsingStagingHelper(long batchId)
        {
            try
            {
                var queryText = $"MERGE Parkway_CBS_Core_TaxEntity tEntity USING Parkway_CBS_Core_NagisOldCustomers NC ON(NC.Id = 0) WHEN NOT MATCHED BY TARGET AND NC.NagisDataBatch_Id = :batch_Id THEN  INSERT (TaxPayerIdentificationNumber, Recipient, PhoneNumber, TaxEntityType, TaxEntityCategory_Id, Address, CreatedAtUtc, UpdatedAtUtc) VALUES(NC.TIN, NC.CustomerName, NC.PhoneNumber, NC.TaxEntityCategory_Id, NC.TaxEntityCategory_Id, NC.Address, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP) OUTPUT NC.Id as SourceId, inserted.Id as TaxEntity_Id, :batch_Id as NagisDataBatch_Id, NC.CustomerId as CustomerId, CURRENT_TIMESTAMP as CreatedAtUtc, CURRENT_TIMESTAMP as UpdatedAtUtc INTO Parkway_CBS_Core_NagisOldInvoiceStagingHelper(SourceId, TaxEntity_Id, NagisDataBatch_Id, CustomerId, CreatedAtUtc, UpdatedAtUtc);";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("batch_Id", batchId);

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }
    }
}
