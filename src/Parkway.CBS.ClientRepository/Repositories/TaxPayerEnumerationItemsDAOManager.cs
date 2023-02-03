using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.TaxPayerEnumerationService.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientRepository.Repositories
{
    public class TaxPayerEnumerationItemsDAOManager : Repository<TaxPayerEnumerationItems>, ITaxPayerEnumerationItemsDAOManager
    {
        public TaxPayerEnumerationItemsDAOManager(IUoW uow) : base(uow)
        {

        }

        /// <summary>
        /// Save enumeration line items as a bundle
        /// </summary>
        /// <param name="lineItems"></param>
        /// <param name="batchId"></param>
        public void SaveEnumerationLineItemsRecords(List<TaxPayerEnumerationLine> lineItems, long batchId)
        {
            //Logger.Information("Saving Enumeration records for batch id " + batchId);
            //save enumeration line items to table
            try
            {
                var dataTable = new DataTable("Parkway_CBS_Core_" + typeof(TaxPayerEnumerationItems).Name);
                dataTable.Columns.Add(new DataColumn(nameof(TaxPayerEnumerationItems.TaxPayerName), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(TaxPayerEnumerationItems.PhoneNumber), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(TaxPayerEnumerationItems.Email), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(TaxPayerEnumerationItems.TIN), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(TaxPayerEnumerationItems.LGA), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(TaxPayerEnumerationItems.Address), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(TaxPayerEnumerationItems.HasErrors), typeof(bool)));
                dataTable.Columns.Add(new DataColumn(nameof(TaxPayerEnumerationItems.ErrorMessages), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(TaxPayerEnumerationItems.TaxPayerEnumeration) + "_Id", typeof(long)));
                dataTable.Columns.Add(new DataColumn(nameof(TaxPayerEnumerationItems.CreatedAtUtc), typeof(DateTime)));
                dataTable.Columns.Add(new DataColumn(nameof(TaxPayerEnumerationItems.UpdatedAtUtc), typeof(DateTime)));

                foreach (var lineItem in lineItems)
                {
                    DataRow row = dataTable.NewRow();
                    row[nameof(TaxPayerEnumerationItems.TaxPayerName)] = lineItem.Name;
                    row[nameof(TaxPayerEnumerationItems.PhoneNumber)] = lineItem.PhoneNumber;
                    row[nameof(TaxPayerEnumerationItems.Email)] = lineItem.Email;
                    row[nameof(TaxPayerEnumerationItems.TIN)] = lineItem.TIN;
                    row[nameof(TaxPayerEnumerationItems.LGA)] = lineItem.LGA;
                    row[nameof(TaxPayerEnumerationItems.Address)] = lineItem.Address;
                    row[nameof(TaxPayerEnumerationItems.HasErrors)] = lineItem.HasError;
                    row[nameof(TaxPayerEnumerationItems.ErrorMessages)] = lineItem.ErrorMessages;
                    row[nameof(TaxPayerEnumerationItems.TaxPayerEnumeration) + "_Id"] = batchId;
                    row[nameof(TaxPayerEnumerationItems.CreatedAtUtc)] = DateTime.Now.ToLocalTime();
                    row[nameof(TaxPayerEnumerationItems.UpdatedAtUtc)] = DateTime.Now.ToLocalTime();
                    dataTable.Rows.Add(row);
                }

                    if (!SaveBundle(dataTable, "Parkway_CBS_Core_" + typeof(TaxPayerEnumerationItems).Name))
                    { throw new Exception("Error saving enumeration schedule excel file details for batch Id " + batchId); }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
