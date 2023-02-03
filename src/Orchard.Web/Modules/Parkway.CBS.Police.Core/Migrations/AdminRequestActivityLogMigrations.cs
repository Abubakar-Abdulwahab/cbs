using System;
using Orchard.Data.Migration;
using Orchard.Data.Migration.Schema;
using Parkway.CBS.Police.Core.Models;


namespace Parkway.CBS.Police.Core.Migrations
{
    public class AdminRequestActivityLogMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(AdminRequestActivityLog).Name,
                table => table
                            .Column<int>(nameof(AdminRequestActivityLog.Id), column => column.Identity().PrimaryKey())
                            .Column<string>(nameof(AdminRequestActivityLog.ActionName), column => column.NotNull().WithLength(100))
                            .Column<string>(nameof(AdminRequestActivityLog.Message), column => column.NotNull().WithLength(1000))
                            .Column<int>(nameof(AdminRequestActivityLog.Service) + "_Id", column => column.NotNull())
                            .Column<Int64>(nameof(AdminRequestActivityLog.Request) + "_Id", column => column.NotNull())
                            .Column<Int64>(nameof(AdminRequestActivityLog.TaxEntity) + "_Id", column => column.NotNull())
                            .Column<int>(nameof(AdminRequestActivityLog.ActionByAdminUser) + "_Id", column => column.NotNull())
                            .Column<int>(nameof(AdminRequestActivityLog.CommandTimeStamp) + "_Id", column => column.NotNull())
                            .Column<int>(nameof(AdminRequestActivityLog.FlowDefinitionLevelTimeStamp) + "_Id", column => column.NotNull())
                            .Column<DateTime>(nameof(AdminRequestActivityLog.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(AdminRequestActivityLog.UpdatedAtUtc), column => column.NotNull())
                );
            return 1;
        }

    }
}