using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class IdentificationTypeTaxCategoryMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(IdentificationTypeTaxCategory).Name,
                table => table
                    .Column<int>(nameof(IdentificationTypeTaxCategory.Id), column => column.PrimaryKey().Identity())
                    .Column<int>(nameof(IdentificationTypeTaxCategory.IdentificationType)+"_Id", column => column.NotNull())
                    .Column<int>(nameof(IdentificationTypeTaxCategory.TaxCategory)+"_Id", column => column.NotNull())
                    .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                    .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(IdentificationTypeTaxCategory).Name);
            string queryString = string.Format("ALTER TABLE[dbo].[{0}] ADD constraint IdentificationTypeTaxCategory_Unique_Constraint UNIQUE([{1}], [{2}])", tableName, nameof(IdentificationTypeTaxCategory.IdentificationType)+"_Id", nameof(IdentificationTypeTaxCategory.TaxCategory) + "_Id");
            SchemaBuilder.ExecuteSql(queryString);

            return 1;
        }
    }
}