using System;
using System.Linq;
using System.Data;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Entities.VMs;
using System.Collections.Concurrent;
using Parkway.CBS.ClientRepository.Repositories.Contracts;
using System.Collections.Generic;

namespace Parkway.CBS.ClientRepository.Repositories
{
    public class IPPISBatchRecordsInvoiceDAOManager : Repository<IPPISBatchRecordsInvoice>, IIPPISBatchRecordsInvoiceDAOManager
    {
        public IPPISBatchRecordsInvoiceDAOManager(IUoW uow) : base(uow)
        { }


        /// <summary>
        /// Save the results of the invoice generation process.
        /// The list contains the results of invoice generation models chunked up
        /// </summary>
        /// <param name="listOfProcessedInvoices"></param>
        /// <param name="batchId"></param>
        /// <param name="invoiceModel"></param>
        public void SaveBundle(List<ConcurrentStack<IPPISGenerateInvoiceResult>> listOfProcessedInvoices, long batchId, string invoiceModel)
        {
            List<IPPISGenerateInvoiceResult> result = new List<IPPISGenerateInvoiceResult> { };
            listOfProcessedInvoices.ForEach(x => result.AddRange(x));

            int chunkSize = 500000;
            string dbChunk = Core.Utilities.AppSettingsConfigurations.GetSettingsValue(Core.Models.Enums.AppSettingEnum.DBChunkSize);
            if (!string.IsNullOrEmpty(dbChunk))
            {
                bool parsed = Int32.TryParse(dbChunk, out chunkSize);
                if (!parsed)
                {
                    chunkSize = 500000;
                }
            }
            var dataSize = result.Count();

            double pageSize = ((double)dataSize / (double)chunkSize);
            int pages = 0;

            if (pageSize < 1 && dataSize >= 1) { pages = 1; }
            else { pages = (int)Math.Ceiling(pageSize); }
            int stopper = 0;
            int skip = 0;

            List<DataTable> listOfDataTables = new List<DataTable> { };
            try
            {
                var dataTable = new DataTable("Parkway_CBS_Core_" + typeof(IPPISBatchRecordsInvoice).Name);
                dataTable.Columns.Add(new DataColumn("IPPISBatch_Id", typeof(Int64)));
                dataTable.Columns.Add(new DataColumn("TaxEntity_Id", typeof(Int64)));
                dataTable.Columns.Add(new DataColumn("TaxEntityCategory_Id", typeof(int)));
                dataTable.Columns.Add(new DataColumn("IPPISTaxPayerSummary_Id", typeof(Int64)));
                dataTable.Columns.Add(new DataColumn("InvoiceModel", typeof(string)));
                dataTable.Columns.Add(new DataColumn("InvoiceNumber", typeof(string)));
                dataTable.Columns.Add(new DataColumn("InvoiceAmount", typeof(decimal)));
                dataTable.Columns.Add(new DataColumn("CashflowInvoiceIdentifier", typeof(string)));
                dataTable.Columns.Add(new DataColumn("PrimaryContactId", typeof(Int64)));
                dataTable.Columns.Add(new DataColumn("CashflowCustomerId", typeof(Int64)));
                dataTable.Columns.Add(new DataColumn("ErrorOccurred", typeof(bool)));
                dataTable.Columns.Add(new DataColumn("ErrorCode", typeof(string)));
                dataTable.Columns.Add(new DataColumn("ErrorMessage", typeof(string)));
                dataTable.Columns.Add(new DataColumn("DueDate", typeof(DateTime)));
                dataTable.Columns.Add(new DataColumn("InvoiceDescription", typeof(string)));
                dataTable.Columns.Add(new DataColumn("InvoiceURL", typeof(string)));
                dataTable.Columns.Add(new DataColumn("CreatedAtUtc", typeof(DateTime)));
                dataTable.Columns.Add(new DataColumn("UpdatedAtUtc", typeof(DateTime)));

                while (stopper < pages)
                {
                    result.Skip(skip).Take(chunkSize).ToList().ForEach(x =>
                    {
                        var row = dataTable.NewRow();
                        row["IPPISBatch_Id"] = batchId;
                        row["TaxEntity_Id"] = x.TaxProfileId;
                        row["TaxEntityCategory_Id"] = x.TaxProfileCategoryId;
                         row["IPPISTaxPayerSummary_Id"] = x.IPPISTaxPayerSummaryId;
                        row["InvoiceModel"] = invoiceModel;
                        row["InvoiceNumber"] = x.IntegrationResponseModel.HasErrors ? null : x.IntegrationResponseModel.ResponseObject.Invoice.Number;
                        row["InvoiceAmount"] = x.IntegrationResponseModel.HasErrors ? null : x.IntegrationResponseModel.ResponseObject.Invoice.AmountDue;
                        row["CashflowInvoiceIdentifier"] = x.IntegrationResponseModel.HasErrors ? null : x.IntegrationResponseModel.ResponseObject.CustomerModel.Identifier;
                        row["PrimaryContactId"] = x.IntegrationResponseModel.HasErrors ? 0 : x.IntegrationResponseModel.ResponseObject.CustomerModel.PrimaryContactId;
                        row["CashflowCustomerId"] = x.IntegrationResponseModel.HasErrors ? 0 : x.IntegrationResponseModel.ResponseObject.CustomerModel.CustomerId;
                        row["ErrorOccurred"] = x.IntegrationResponseModel.HasErrors;
                        row["ErrorCode"] = x.IntegrationResponseModel.ErrorCode;
                        row["ErrorMessage"] = x.IntegrationResponseModel.HasErrors ? x.IntegrationResponseModel.ResponseObject : null;
                        row["DueDate"] = x.DueDate;
                        row["InvoiceDescription"] = x.InvoiceDescription;
                        row["InvoiceURL"] = x.IntegrationResponseModel.HasErrors ? null : x.IntegrationResponseModel.ResponseObject.Invoice.IntegrationPreviewUrl;
                        row["CreatedAtUtc"] = DateTime.Now.ToLocalTime();
                        row["UpdatedAtUtc"] = DateTime.Now.ToLocalTime();
                        dataTable.Rows.Add(row);
                    });
                    listOfDataTables.Add(dataTable);
                    skip += chunkSize;
                    stopper++;
                }
                //we now have a collection of datatables, lets save the bunch together
                if (!SaveBundle(listOfDataTables, "Parkway_CBS_Core_" + typeof(IPPISBatchRecordsInvoice).Name))
                { throw new Exception("Error saving excel file details for batch Id " + batchId); }
            }
            catch (Exception exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Move mapped records with tax payer code with corresponding tax entity to IPPIS
        /// </summary>
        /// <param name="batchId"></param>
        public void MoveToInvoiceTable(long batchId)
        {
            using (var session = _uow.BeginStatelessTransaction().Session)
            {
                try
                {
                    var queryText =
$"INSERT INTO Parkway_CBS_Core_IPPISBatchRecordsInvoice (IPPISBatch_Id, TaxEntity_Id, TaxPayerCode, CreatedAtUtc, UpdatedAtUtc) SELECT :batch_Id, TaxEntity_Id, TaxPayerCode, :dateSaved, :dateSaved FROM Parkway_CBS_Core_IPPISTaxPayerSummary WHERE TaxEntity_Id IS NOT NULL";

                    var query = session.CreateSQLQuery(queryText);
                    query.SetParameter("dateSaved", DateTime.Now.ToLocalTime());
                    query.SetParameter("batch_Id", batchId);

                    query.ExecuteUpdate();
                }
                catch (Exception)
                { throw; }
            }
        }
    }
}
