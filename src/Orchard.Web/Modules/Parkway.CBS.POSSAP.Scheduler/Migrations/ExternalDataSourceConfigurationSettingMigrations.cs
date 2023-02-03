using System;
using Orchard.Data.Migration;
using Parkway.CBS.POSSAP.Scheduler.Models;

namespace Parkway.CBS.POSSAP.Scheduler.Migrations
{
    public class ExternalDataSourceConfigurationSettingMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(ExternalDataSourceConfigurationSetting).Name,
                table => table
                            .Column<int>(nameof(ExternalDataSourceConfigurationSetting.Id), column => column.PrimaryKey().Identity())
                            .Column<string>(nameof(ExternalDataSourceConfigurationSetting.ActionName), column => column.NotNull().Unique().WithLength(100))
                            .Column<string>(nameof(ExternalDataSourceConfigurationSetting.ImplementingClass), column => column.NotNull().Unique().WithLength(500))
                            .Column<string>(nameof(ExternalDataSourceConfigurationSetting.CRONValue), column => column.NotNull().WithLength(100))
                            .Column<DateTime>(nameof(ExternalDataSourceConfigurationSetting.NextScheduleDate), column => column.NotNull())
                            .Column<bool>(nameof(ExternalDataSourceConfigurationSetting.IsActive), column => column.NotNull().WithDefault(true))
                            .Column<DateTime>(nameof(ExternalDataSourceConfigurationSetting.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(ExternalDataSourceConfigurationSetting.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }
    }
}