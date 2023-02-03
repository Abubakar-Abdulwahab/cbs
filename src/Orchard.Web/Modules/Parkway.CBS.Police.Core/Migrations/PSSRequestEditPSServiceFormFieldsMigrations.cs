using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSRequestEditPSServiceFormFieldsMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSRequestEditPSServiceFormFields).Name,
                table => table
                    .Column<Int64>(nameof(PSSRequestEditPSServiceFormFields.Id), column => column.PrimaryKey().Identity())
                    .Column<Int64>(nameof(PSSRequestEditPSServiceFormFields.RequestEdit) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSRequestEditPSServiceFormFields.PSServiceFormFields) + "_Id", column => column.NotNull())
                    .Column<DateTime>(nameof(PSSRequestEditPSServiceFormFields.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSRequestEditPSServiceFormFields.UpdatedAtUtc), column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(PSSRequestEditPSServiceFormFields).Name);
            SchemaBuilder.ExecuteSql(string.Format("ALTER TABLE[dbo].[{0}] ADD constraint PSSRequestEditPSServiceFormFields_Unique_Constraint UNIQUE([{1}], [{2}]); ", tableName, nameof(PSSRequestEditPSServiceFormFields.RequestEdit) + "_Id", nameof(PSSRequestEditPSServiceFormFields.PSServiceFormFields) + "_Id"));

            return 1;
        }
    }
}