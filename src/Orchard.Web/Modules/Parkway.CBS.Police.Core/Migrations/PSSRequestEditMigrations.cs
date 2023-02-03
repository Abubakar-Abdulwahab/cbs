using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSRequestEditMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSRequestEdit).Name,
                table => table
                    .Column<Int64>(nameof(PSSRequestEdit.Id), column => column.PrimaryKey().Identity())
                    .Column<Int64>(nameof(PSSRequestEdit.Request) + "_Id", column => column.NotNull())
                    .Column<Int64>(nameof(PSSRequestEdit.RoutedAtRequestCommandWorkFlowLog) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSRequestEdit.AdminUser) + "_Id", column => column.NotNull())
                    .Column<bool>(nameof(PSSRequestEdit.IsActive), column => column.NotNull().WithDefault(false))
                    .Column<DateTime>(nameof(PSSRequestEdit.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSRequestEdit.UpdatedAtUtc), column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(PSSRequestEdit).Name);
            string queryString = $"ALTER TABLE {tableName} add [{nameof(PSSRequestEdit.RequestEditReference)}] as ((concat({nameof(PSSRequestEdit.Request)}_Id,case {nameof(PSSRequestEdit.IsActive)} when 1 then '-0' else CONCAT('-', [{nameof(PSSRequestEdit.Id)}]) end))) PERSISTED";
            SchemaBuilder.ExecuteSql(queryString);

            queryString = $"ALTER TABLE[dbo].[{tableName}] ADD constraint PSSRequestEdit_Unique_Constraint UNIQUE([{nameof(PSSRequestEdit.RequestEditReference)}]); ";
            SchemaBuilder.ExecuteSql(queryString);

            return 1;
        }
    }
}