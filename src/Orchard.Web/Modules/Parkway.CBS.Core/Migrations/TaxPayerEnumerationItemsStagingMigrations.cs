using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Migrations
{
    public class TaxPayerEnumerationItemsStagingMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(TaxPayerEnumerationItemsStaging).Name,
                table => table
                            .Column<long>(nameof(TaxPayerEnumerationItemsStaging.Id), column => column.PrimaryKey().Identity())
                            .Column<string>(nameof(TaxPayerEnumerationItemsStaging.Address), column => column.Nullable())
                            .Column<string>(nameof(TaxPayerEnumerationItemsStaging.Email), column => column.Nullable())
                            .Column<string>(nameof(TaxPayerEnumerationItemsStaging.LGA), column => column.Nullable())
                            .Column<string>(nameof(TaxPayerEnumerationItemsStaging.PhoneNumber), column => column.Nullable())
                            .Column<string>(nameof(TaxPayerEnumerationItemsStaging.TaxPayerName), column => column.Nullable())
                            .Column<string>(nameof(TaxPayerEnumerationItemsStaging.TIN), column => column.Nullable())
                            .Column<long>(nameof(TaxPayerEnumerationItemsStaging.TaxPayerEnumeration) + "_Id", column => column.NotNull())
                            .Column<bool>(nameof(TaxPayerEnumerationItemsStaging.HasErrors), column => column.NotNull())
                            .Column<string>(nameof(TaxPayerEnumerationItemsStaging.ErrorMessages), column => column.Nullable().Unlimited())
                            .Column<DateTime>(nameof(TaxPayerEnumerationItemsStaging.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(TaxPayerEnumerationItemsStaging.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }
    }
}