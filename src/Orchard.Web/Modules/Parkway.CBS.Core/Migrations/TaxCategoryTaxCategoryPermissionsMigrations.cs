using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Core.Migrations
{
    public class TaxCategoryTaxCategoryPermissionsMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(TaxCategoryTaxCategoryPermissions).Name,
                table => table
                            .Column<int>(nameof(TaxCategoryTaxCategoryPermissions.Id), column => column.Identity().PrimaryKey())
                            .Column<int>(nameof(TaxCategoryTaxCategoryPermissions.TaxEntityCategory)+"_Id", column => column.NotNull())
                            .Column<int>(nameof(TaxCategoryTaxCategoryPermissions.TaxCategoryPermissions)+"_Id", column => column.NotNull())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(TaxCategoryTaxCategoryPermissions).Name);
            SchemaBuilder.ExecuteSql(string.Format("ALTER TABLE[dbo].[{0}] ADD constraint TaxCategoryTaxCategoryPermissions_Unique_Constraint UNIQUE([{1}], [{2}]); ", tableName, nameof(TaxCategoryTaxCategoryPermissions.TaxEntityCategory) + "_Id", nameof(TaxCategoryTaxCategoryPermissions.TaxCategoryPermissions) + "_Id"));

            return 1;
        }
    }
}