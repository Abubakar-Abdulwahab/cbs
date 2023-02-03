using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSServiceOptionsMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSServiceOptions).Name,
                table => table
                    .Column<int>(nameof(PSServiceOptions.Id), column => column.PrimaryKey().Identity())
                    .Column<string>(nameof(PSServiceOptions.Name), column => column.Nullable().WithLength(200).Unique())
                    .Column<int>(nameof(PSServiceOptions.Service) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSServiceOptions.ServiceOption) + "_Id", column => column.NotNull())
                    .Column<string>(nameof(PSServiceOptions.ServiceOptionType), column => column.NotNull().Unique())
                    .Column<int>(nameof(PSServiceOptions.ServiceOptionTypeId), column => column.NotNull())
                    .Column<bool>(nameof(PSServiceOptions.IsActive), column => column.NotNull().WithDefault(true))
                    .Column<DateTime>(nameof(PSServiceOptions.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSServiceOptions.UpdatedAtUtc), column => column.NotNull())
                );

            SchemaBuilder.ExecuteSql(string.Format("ALTER TABLE [dbo].[{0}] ADD constraint PSServiceOptions_Unique_Constraint UNIQUE([{1}], [{2}]);", SchemaBuilder.TableDbName(typeof(PSServiceOptions).Name), nameof(PSServiceOptions.Service) + "_Id", nameof(PSServiceOptions.ServiceOption) + "_Id"));

            return 1;
        }
    }
}