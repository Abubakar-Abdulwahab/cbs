using Parkway.Cashflow.Ng.Models;
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
    public class BatchInvoiceResponseDAOManager : Repository<BatchInvoiceResponse>, IBatchInvoiceResponseDAOManager
    {
        public BatchInvoiceResponseDAOManager(IUoW uow) : base(uow)
        {

        }

        public void SaveBundle(List<CashFlowCreateCustomerAndInvoiceResponse> listOfProcessedInvoices, string batchId)
        {
            List<DataTable> listOfDataTables = new List<DataTable> { };
            try
            {
                var dataTable = new DataTable("Parkway_CBS_Core_" + typeof(BatchInvoiceResponse).Name);
                dataTable.Columns.Add(new DataColumn("BatchIdentifier", typeof(string)));
                dataTable.Columns.Add(new DataColumn("InvoiceUniqueKey", typeof(Int64)));
                dataTable.Columns.Add(new DataColumn("InvoiceNumber", typeof(string)));
                dataTable.Columns.Add(new DataColumn("InvoiceAmount", typeof(decimal)));
                dataTable.Columns.Add(new DataColumn("CashflowInvoiceIdentifier", typeof(string)));
                dataTable.Columns.Add(new DataColumn("PrimaryContactId", typeof(Int64)));
                dataTable.Columns.Add(new DataColumn("CashflowCustomerId", typeof(Int64)));
                dataTable.Columns.Add(new DataColumn("DueDate", typeof(DateTime)));
                dataTable.Columns.Add(new DataColumn("InvoiceDescription", typeof(string)));
                dataTable.Columns.Add(new DataColumn("IntegrationPreviewUrl", typeof(string)));
                dataTable.Columns.Add(new DataColumn("Status", typeof(int)));
                dataTable.Columns.Add(new DataColumn("PDFURL", typeof(string)));
                dataTable.Columns.Add(new DataColumn("CreatedAtUtc", typeof(DateTime)));
                dataTable.Columns.Add(new DataColumn("UpdatedAtUtc", typeof(DateTime)));

                listOfProcessedInvoices.ForEach(x =>
                {
                    var row = dataTable.NewRow();
                    row["BatchIdentifier"] = batchId;
                    row["InvoiceUniqueKey"] = x.CustomerModel.InvoiceUniqueIdentifier;
                    row["InvoiceNumber"] = x.Invoice.Number;
                    row["InvoiceAmount"] = x.Invoice.AmountDue;
                    row["CashflowInvoiceIdentifier"] = x.CustomerModel.Identifier;
                    row["PrimaryContactId"] = x.CustomerModel.PrimaryContactId;
                    row["CashflowCustomerId"] = x.CustomerModel.CustomerId;
                    row["DueDate"] = DateTime.Now.ToLocalTime();
                    row["InvoiceDescription"] = x.Invoice.Title;
                    row["IntegrationPreviewUrl"] = x.Invoice.IntegrationPreviewUrl;
                    row["Status"] = (int)InvoiceStatus.Unpaid;
                    row["PDFURL"] = x.Invoice.PdfUrl;
                    row["CreatedAtUtc"] = DateTime.Now.ToLocalTime();
                    row["UpdatedAtUtc"] = DateTime.Now.ToLocalTime();
                    dataTable.Rows.Add(row);
                });
                listOfDataTables.Add(dataTable);

                if (!SaveBundle(listOfDataTables, "Parkway_CBS_Core_" + typeof(BatchInvoiceResponse).Name))
                { throw new Exception("Error saving excel file details for batch Id " + batchId); }
            }
            catch (Exception exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Save the results of the invoice generation process.
        /// The list contains the results of invoice generation models chunked up
        /// </summary>
        /// <param name="listOfProcessedInvoices"></param>
        /// <param name="batchId"></param>
        public void SaveBundle(List<ConcurrentStack<NAGISGenerateInvoiceResult>> listOfProcessedInvoices, long batchId)
        {
            List<NAGISGenerateInvoiceResult> result = new List<NAGISGenerateInvoiceResult> { };
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
                    var dataTable = new DataTable("Parkway_CBS_Core_" + typeof(NagisOldInvoiceResponseStaging).Name);
                    dataTable.Columns.Add(new DataColumn("BatchIdentifier", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("InvoiceUniqueKey", typeof(Int64)));
                    dataTable.Columns.Add(new DataColumn("InvoiceNumber", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("InvoiceAmount", typeof(decimal)));
                    dataTable.Columns.Add(new DataColumn("CashflowInvoiceIdentifier", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("PrimaryContactId", typeof(Int64)));
                    dataTable.Columns.Add(new DataColumn("CashflowCustomerId", typeof(Int64)));
                    dataTable.Columns.Add(new DataColumn("DueDate", typeof(DateTime)));
                    dataTable.Columns.Add(new DataColumn("InvoiceDescription", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("IntegrationPreviewUrl", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("Status", typeof(int)));
                    dataTable.Columns.Add(new DataColumn("RevenueHeadId", typeof(int)));
                    dataTable.Columns.Add(new DataColumn("MDAId", typeof(int)));
                    dataTable.Columns.Add(new DataColumn("ExpertSystemId", typeof(int)));
                    dataTable.Columns.Add(new DataColumn("PDFURL", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("CreatedAtUtc", typeof(DateTime)));
                    dataTable.Columns.Add(new DataColumn("UpdatedAtUtc", typeof(DateTime)));

                    result.Skip(skip).Take(chunkSize).ToList().ForEach(x =>
                    {
                        var row = dataTable.NewRow();
                        row["BatchIdentifier"] = batchId;
                        row["InvoiceUniqueKey"] = x.NAGISOldInvoiceNumber;
                        row["InvoiceNumber"] = x.IntegrationResponseModel.HasErrors ? null : x.IntegrationResponseModel.ResponseObject.Invoice.Number;
                        row["InvoiceAmount"] = x.IntegrationResponseModel.HasErrors ? null : x.IntegrationResponseModel.ResponseObject.Invoice.AmountDue;
                        row["CashflowInvoiceIdentifier"] = x.IntegrationResponseModel.HasErrors ? null : x.IntegrationResponseModel.ResponseObject.CustomerModel.Identifier;
                        row["PrimaryContactId"] = x.IntegrationResponseModel.HasErrors ? 0 : x.IntegrationResponseModel.ResponseObject.CustomerModel.PrimaryContactId;
                        row["CashflowCustomerId"] = x.IntegrationResponseModel.HasErrors ? 0 : x.IntegrationResponseModel.ResponseObject.CustomerModel.CustomerId;
                        row["DueDate"] = x.DueDate;
                        row["InvoiceDescription"] = x.InvoiceDescription;
                        row["IntegrationPreviewUrl"] = x.IntegrationResponseModel.HasErrors ? null : x.IntegrationResponseModel.ResponseObject.Invoice.IntegrationPreviewUrl;
                        row["Status"] = (int)InvoiceStatus.Unpaid;
                        row["RevenueHeadId"] = x.RevenueHeadId;
                        row["MDAId"] = x.MDAId;
                        row["ExpertSystemId"] = x.ExpertSystemId;
                        row["PDFURL"] = x.IntegrationResponseModel.HasErrors ? null : x.IntegrationResponseModel.ResponseObject.Invoice.PdfUrl;
                        row["CreatedAtUtc"] = DateTime.Now.ToLocalTime();
                        row["UpdatedAtUtc"] = DateTime.Now.ToLocalTime();
                        dataTable.Rows.Add(row);
                    });
                    listOfDataTables.Add(dataTable);
                    skip += chunkSize;
                    stopper++;
                }
                //we now have a collection of datatables, lets save the bunch together
                if (!SaveBundle(listOfDataTables, "Parkway_CBS_Core_" + typeof(NagisOldInvoiceResponseStaging).Name))
                { throw new Exception("Error saving excel file details for batch Id " + batchId); }
            }
            catch (Exception exception)
            {
                throw;
            }
        }

    }
}
