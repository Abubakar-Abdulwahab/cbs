using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSServiceFormFieldsMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSServiceFormFields).Name,
                table => table
                    .Column<int>(nameof(PSServiceFormFields.Id), column => column.PrimaryKey().Identity())
                    .Column<int>(nameof(PSServiceFormFields.Service) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSServiceFormFields.FormControl) + "_Id", column => column.NotNull())
                    .Column<bool>(nameof(PSServiceFormFields.IsActive), column => column.NotNull().WithDefault(false))
                    .Column<DateTime>(nameof(PSServiceFormFields.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSServiceFormFields.UpdatedAtUtc), column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(PSServiceFormFields).Name);
            SchemaBuilder.ExecuteSql(string.Format("ALTER TABLE[dbo].[{0}] ADD constraint PSServiceFormFields_Unique_Constraint UNIQUE([{1}], [{2}]); ", tableName, nameof(PSServiceFormFields.Service) + "_Id", nameof(PSServiceFormFields.FormControl) + "_Id"));

            return 1;
        }
    }
}