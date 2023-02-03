using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Core.Migrations
{
    public class TaxPayerEnumerationItemsMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(TaxPayerEnumerationItems).Name,
                table => table
                            .Column<long>(nameof(TaxPayerEnumerationItems.Id), column => column.PrimaryKey().Identity())
                            .Column<string>(nameof(TaxPayerEnumerationItems.Address), column => column.Nullable())
                            .Column<string>(nameof(TaxPayerEnumerationItems.Email), column => column.Nullable())
                            .Column<string>(nameof(TaxPayerEnumerationItems.LGA), column => column.Nullable())
                            .Column<string>(nameof(TaxPayerEnumerationItems.PhoneNumber), column => column.Nullable())
                            .Column<string>(nameof(TaxPayerEnumerationItems.TaxPayerName), column => column.Nullable())
                            .Column<string>(nameof(TaxPayerEnumerationItems.TIN), column => column.Nullable())
                            .Column<long>(nameof(TaxPayerEnumerationItems.TaxEntity) + "_Id", column => column.Nullable())
                            .Column<long>(nameof(TaxPayerEnumerationItems.TaxPayerEnumeration) + "_Id", column => column.NotNull())
                            .Column<bool>(nameof(TaxPayerEnumerationItems.HasErrors), column => column.NotNull())
                            .Column<string>(nameof(TaxPayerEnumerationItems.ErrorMessages), column => column.Nullable().Unlimited())
                            .Column<DateTime>(nameof(TaxPayerEnumerationItems.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(TaxPayerEnumerationItems.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }
    }
}