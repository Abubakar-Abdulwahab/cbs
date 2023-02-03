using Parkway.CBS.ClientRepository.Repositories.NAGIS.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Entities.VMs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientRepository.Repositories.NAGIS
{
    public class NagisOldInvoiceCustomerResponseDAOManager : Repository<NagisOldInvoiceCustomerResponse>, INagisOldInvoiceCustomerResponseDAOManager
    {
        public NagisOldInvoiceCustomerResponseDAOManager(IUoW uow) : base(uow)
        {

        }

        public void SaveBundle(List<ConcurrentStack<NAGISGenerateCustomerResult>> listOfProcessedCustomers, long batchId)
        {
            List<NAGISGenerateCustomerResult> result = new List<NAGISGenerateCustomerResult> { };
            listOfProcessedCustomers.ForEach(x => result.AddRange(x));

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
                    var dataTable = new DataTable("Parkway_CBS_Core_" + typeof(NagisOldInvoiceCustomerResponse).Name);
                    dataTable.Columns.Add(new DataColumn("BatchIdentifier", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("TaxEntity_Id", typeof(Int64)));
                    dataTable.Columns.Add(new DataColumn("PrimaryContactId", typeof(Int64)));
                    dataTable.Columns.Add(new DataColumn("CashflowCustomerId", typeof(Int64)));
                    dataTable.Columns.Add(new DataColumn("CreatedAtUtc", typeof(DateTime)));
                    dataTable.Columns.Add(new DataColumn("UpdatedAtUtc", typeof(DateTime)));

                    result.Skip(skip).Take(chunkSize).ToList().ForEach(x =>
                    {
                        var row = dataTable.NewRow();
                        row["BatchIdentifier"] = batchId;
                        //row["TaxEntity_Id"] = x.TaxEntityId; //Use this when doing single call to cashflow
                        row["TaxEntity_Id"] = x.CashFlowCustomer.Identifier; //Use this when doing batch call to cashflow
                        row["PrimaryContactId"] = x.CashFlowCustomer.PrimaryContactId;
                        row["CashflowCustomerId"] = x.CashFlowCustomer.Id;
                        row["CreatedAtUtc"] = DateTime.Now.ToLocalTime();
                        row["UpdatedAtUtc"] = DateTime.Now.ToLocalTime();
                        dataTable.Rows.Add(row);
                    });
                    listOfDataTables.Add(dataTable);
                    skip += chunkSize;
                    stopper++;
                }
                //we now have a collection of datatables, lets save the bunch together
                if (!SaveBundle(listOfDataTables, "Parkway_CBS_Core_" + typeof(NagisOldInvoiceCustomerResponse).Name))
                { throw new Exception("Error saving excel file details for batch Id " + batchId); }
            }
            catch (Exception exception)
            {
                throw;
            }
        }
    }
}
