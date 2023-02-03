using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;


namespace Parkway.CBS.Police.Core.Migrations
{
    public class EscortProcessFlowMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(EscortProcessFlow).Name,
                table => table
                    .Column<int>(nameof(EscortProcessFlow.Id), column => column.PrimaryKey().Identity())
                    .Column<string>(nameof(EscortProcessFlow.Name), column => column.NotNull().Unique().WithLength(100))
                    .Column<int>(nameof(EscortProcessFlow.AdminUser) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(EscortProcessFlow.Level) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(EscortProcessFlow.AddedBy) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(EscortProcessFlow.LastUpdatedBy) + "_Id", column => column.NotNull())
                    .Column<bool>(nameof(EscortProcessFlow.IsActive), column => column.NotNull().WithDefault(true))
                    .Column<int>(nameof(EscortProcessFlow.CommandType) + "_Id", column => column.NotNull())
                    .Column<DateTime>(nameof(EscortProcessFlow.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(EscortProcessFlow.UpdatedAtUtc), column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(EscortProcessFlow).Name);
            string queryString = string.Format("ALTER TABLE[dbo].[{0}] ADD constraint ESCORT_PROCESS_FLOW_PER_COMMANDTYPE_UNIQUE_CONSTRAINT UNIQUE([{1}], [{2}])", tableName, nameof(EscortProcessFlow.AdminUser) + "_Id", nameof(EscortProcessFlow.CommandType) + "_Id");
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE[dbo].[{0}] ADD constraint ESCORT_PROCESS_FLOW_PER_ROLELEVEL_UNIQUE_CONSTRAINT UNIQUE([{1}], [{2}])", tableName, nameof(EscortProcessFlow.AdminUser) + "_Id", nameof(EscortProcessFlow.Level) + "_Id");
            SchemaBuilder.ExecuteSql(queryString);

            return 1;
        }
    }
}