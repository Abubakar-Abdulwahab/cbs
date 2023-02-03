using Parkway.CBS.ClientRepository.Repositories.ReferenceDataImpl.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Entities.VMs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientRepository.Repositories.ReferenceDataImpl
{
   public class ReferenceDataRecordsInvoiceDAOManager : Repository<ReferenceDataRecordsInvoice>, IReferenceDataRecordsInvoiceDAOManager
    {
        public ReferenceDataRecordsInvoiceDAOManager(IUoW uow) : base(uow)
        {

        }

        public void SaveBundle(List<ConcurrentStack<CashFlowBatchInvoiceResponse>> listOfProcessedInvoices, long batchId, string invoiceModel)
        {
            List<Entities.VMs.CashFlowBatchInvoiceResponse> result = new List<Entities.VMs.CashFlowBatchInvoiceResponse> { };
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
                while (stopper < pages)
                {
                    var dataTable = new DataTable("Parkway_CBS_Core_" + typeof(ReferenceDataRecordsInvoice).Name);
                    dataTable.Columns.Add(new DataColumn("OperationType_Id", typeof(int)));
                    dataTable.Columns.Add(new DataColumn("ReferenceDataBatch_Id", typeof(Int64)));
                    dataTable.Columns.Add(new DataColumn("TaxEntity_Id", typeof(Int64)));
                    dataTable.Columns.Add(new DataColumn("TaxEntityCategory_Id", typeof(int)));
                    dataTable.Columns.Add(new DataColumn("InvoiceUniqueKey", typeof(Int64)));
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

                    result.Skip(skip).Take(chunkSize).ToList().ForEach(x =>
                    {
                        var row = dataTable.NewRow();
                        //row["OperationType_Id"] = x.OperationType;
                        //row["ReferenceDataBatch_Id"] = batchId;
                        //row["TaxEntity_Id"] = x.TaxProfileId;
                        //row["TaxEntityCategory_Id"] = x.TaxProfileCategoryId;
                        //row["InvoiceUniqueKey"] = x.WithholdingTaxonRentId;
                        //row["InvoiceModel"] = invoiceModel;
                        //row["InvoiceNumber"] = x.IntegrationResponseModel.HasErrors ? null : x.IntegrationResponseModel.ResponseObject.Invoice.Number;
                        //row["InvoiceAmount"] = x.IntegrationResponseModel.HasErrors ? null : x.IntegrationResponseModel.ResponseObject.Invoice.AmountDue;
                        //row["CashflowInvoiceIdentifier"] = x.IntegrationResponseModel.HasErrors ? null : x.IntegrationResponseModel.ResponseObject.CustomerModel.Identifier;
                        //row["PrimaryContactId"] = x.IntegrationResponseModel.HasErrors ? 0 : x.IntegrationResponseModel.ResponseObject.CustomerModel.PrimaryContactId;
                        //row["CashflowCustomerId"] = x.IntegrationResponseModel.HasErrors ? 0 : x.IntegrationResponseModel.ResponseObject.CustomerModel.CustomerId;
                        //row["ErrorOccurred"] = x.IntegrationResponseModel.HasErrors;
                        //row["ErrorCode"] = x.IntegrationResponseModel.ErrorCode;
                        //row["ErrorMessage"] = x.IntegrationResponseModel.HasErrors ? x.IntegrationResponseModel.ResponseObject : null;
                        //row["DueDate"] = x.DueDate;
                        //row["InvoiceDescription"] = x.InvoiceDescription;
                        //row["InvoiceURL"] = x.IntegrationResponseModel.HasErrors ? null : x.IntegrationResponseModel.ResponseObject.Invoice.IntegrationPreviewUrl;
                        row["CreatedAtUtc"] = DateTime.Now.ToLocalTime();
                        row["UpdatedAtUtc"] = DateTime.Now.ToLocalTime();
                        dataTable.Rows.Add(row);
                    });
                    listOfDataTables.Add(dataTable);
                    skip += chunkSize;
                    stopper++;
                }
                //we now have a collection of datatables, lets save the bunch together
                if (!SaveBundle(listOfDataTables, "Parkway_CBS_Core_" + typeof(ReferenceDataRecordsInvoice).Name))
                { throw new Exception("Error saving excel file details for batch Id " + batchId); }
            }
            catch (Exception exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchId"></param>
        public void UpdateInvoiceStagingWithCashFlowResponse(long batchId)
        {
            try
            {
                var queryText = $"UPDATE RDI SET RDI.InvoiceNumber = BI.InvoiceNumber, RDI.InvoiceAmount = BI.InvoiceAmount, RDI.CashflowInvoiceIdentifier = BI.CashflowInvoiceIdentifier, RDI.PrimaryContactId = BI.PrimaryContactId," +
                    $" RDI.CashflowCustomerId= BI.CashflowCustomerId, RDI.DueDate = BI.DueDate, RDI.InvoiceDescription = BI.InvoiceDescription, RDI.InvoiceURL = BI.IntegrationPreviewUrl FROM Parkway_CBS_Core_ReferenceDataRecordsInvoice RDI" +
                    $" INNER JOIN Parkway_CBS_Core_BatchInvoiceResponse as BI ON BI.CashflowInvoiceIdentifier = RDI.TaxEntity_Id AND BI.InvoiceUniqueKey = RDI.InvoiceUniqueKey" +
                    $" WHERE RDI.ReferenceDataBatch_Id = :batch_Id";
                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("batch_Id", batchId);

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }

    }
}
