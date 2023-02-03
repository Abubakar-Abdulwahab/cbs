using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Core.Migrations
{
    public class TaxCategoryPermissionsMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(TaxCategoryPermissions).Name,
                table => table
                            .Column<int>(nameof(TaxCategoryPermissions.Id), column => column.Identity().PrimaryKey())
                            .Column<string>(nameof(TaxCategoryPermissions.Name), column => column.NotNull().Unique())
                            .Column<bool>(nameof(TaxCategoryPermissions.IsActive), column => column.NotNull().WithDefault(true))
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }
    }
}