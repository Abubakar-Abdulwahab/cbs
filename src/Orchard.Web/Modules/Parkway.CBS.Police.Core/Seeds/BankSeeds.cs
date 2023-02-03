using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.Seeds.Contracts;
using System;
using System.Collections.Generic;
using System.Data;

namespace Parkway.CBS.Police.Core.Seeds
{
    public class BankSeeds : IBankSeeds
    {
        private readonly IBankManager<Bank> _bankManager;

        public BankSeeds(IBankManager<Bank> bankManager)
        {
            _bankManager = bankManager;
        }

        /// <summary>
        /// Populates banks in <see cref="Bank"/>
        /// </summary>
        /// <param name="banks"></param>
        public void PopBanks(List<BankVM> banks)
        {
            var dataTable = new DataTable("Parkway_CBS_Core_" + typeof(Bank).Name);
            dataTable.Columns.Add(new DataColumn(nameof(Bank.Name), typeof(string)));
            dataTable.Columns.Add(new DataColumn(nameof(Bank.ShortName), typeof(string)));
            dataTable.Columns.Add(new DataColumn(nameof(Bank.Code), typeof(string)));
            dataTable.Columns.Add(new DataColumn(nameof(Bank.IsDeleted), typeof(bool)));
            dataTable.Columns.Add(new DataColumn(nameof(Bank.CreatedAtUtc), typeof(DateTime)));
            dataTable.Columns.Add(new DataColumn(nameof(Bank.UpdatedAtUtc), typeof(DateTime)));

            foreach (var bank in banks)
            {
                DataRow row = dataTable.NewRow();
                row[nameof(Bank.Name)] = bank.Name;
                row[nameof(Bank.ShortName)] = bank.ShortName;
                row[nameof(Bank.Code)] = bank.Code;
                row[nameof(Bank.IsDeleted)] = false;
                row[nameof(Bank.CreatedAtUtc)] = DateTime.Now.ToLocalTime();
                row[nameof(Bank.UpdatedAtUtc)] = DateTime.Now.ToLocalTime();
                dataTable.Rows.Add(row);
            }

            if (!_bankManager.SaveBundle(dataTable, "Parkway_CBS_Core_" + typeof(Bank).Name))
            { throw new Exception("Error seeding banks"); }

        }
    }
}