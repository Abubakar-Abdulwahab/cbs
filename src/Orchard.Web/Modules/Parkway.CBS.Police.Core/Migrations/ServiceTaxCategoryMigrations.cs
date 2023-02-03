using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class ServiceTaxCategoryMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(ServiceTaxCategory).Name,
                table => table
                    .Column<int>(nameof(ServiceTaxCategory.Id), column => column.PrimaryKey().Identity())
                    .Column<int>(nameof(ServiceTaxCategory.Service) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(ServiceTaxCategory.TaxCategory) + "_Id", column => column.NotNull())
                    .Column<bool>(nameof(ServiceTaxCategory.IsActive), column => column.NotNull().WithDefault(true))
                    .Column<DateTime>(nameof(ServiceTaxCategory.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(ServiceTaxCategory.UpdatedAtUtc), column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(ServiceTaxCategory).Name);
            SchemaBuilder.ExecuteSql(string.Format("ALTER TABLE[dbo].[{0}] ADD constraint ServiceTaxCategory_Unique_Constraint UNIQUE([{1}], [{2}]); ", tableName, nameof(ServiceTaxCategory.Service) + "_Id", nameof(ServiceTaxCategory.TaxCategory) + "_Id"));

            return 1;
        }

    }
}