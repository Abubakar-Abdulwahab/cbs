using System;
using Orchard.Data.Migration;
using Parkway.CBS.POSSAP.Scheduler.Models;

namespace Parkway.CBS.POSSAP.Scheduler.Migrations
{
    public class CallLogForExternalSystemMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(CallLogForExternalSystem).Name,
                table => table
                            .Column<int>(nameof(CallLogForExternalSystem.Id), column => column.PrimaryKey().Identity())
                            .Column<string>(nameof(CallLogForExternalSystem.URL), column => column.NotNull().WithLength(1000))
                            .Column<string>(nameof(CallLogForExternalSystem.CallDescription), column => column.NotNull().WithLength(1000))
                            .Column<int>(nameof(CallLogForExternalSystem.ExternalDataSourceConfigurationSetting) + "_Id", column => column.NotNull())
                            .Column<int>(nameof(CallLogForExternalSystem.CallStatus), column => column.NotNull())
                            .Column<string>(nameof(CallLogForExternalSystem.JSONCallParameters), column => column.Nullable())
                            .Column<bool>(nameof(CallLogForExternalSystem.CallIsSuccessful), column => column.NotNull().WithDefault(false))
                            .Column<string>(nameof(CallLogForExternalSystem.ErrorMessage), column => column.Nullable().Unlimited())
                            .Column<DateTime>(nameof(CallLogForExternalSystem.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(CallLogForExternalSystem.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }

        public int UpdateFrom1()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(CallLogForExternalSystem).Name);

            string queryString = string.Format($"ALTER TABLE {tableName} ALTER COLUMN {nameof(CallLogForExternalSystem.ExternalDataSourceConfigurationSetting) + "_Id"} int NULL");
            SchemaBuilder.ExecuteSql(queryString);

            return 2;
        }

    }
}