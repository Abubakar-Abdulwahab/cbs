using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PoliceofficerDeploymentAllowanceTrackerMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PoliceofficerDeploymentAllowanceTracker).Name,
                table => table
                    .Column<Int64>(nameof(PoliceofficerDeploymentAllowanceTracker.Id), column => column.PrimaryKey().Identity())
                    .Column<Int64>(nameof(PoliceofficerDeploymentAllowanceTracker.Request) + "_Id", column => column.NotNull())
                    .Column<Int64>(nameof(PoliceofficerDeploymentAllowanceTracker.Invoice) + "_Id", column => column.NotNull())
                    .Column<bool>(nameof(PoliceofficerDeploymentAllowanceTracker.IsSettlementCompleted), column => column.NotNull().WithDefault(false))
                    .Column<int>(nameof(PoliceofficerDeploymentAllowanceTracker.NumberOfSettlementDone), column => column.NotNull())
                    .Column<Int64>(nameof(PoliceofficerDeploymentAllowanceTracker.EscortDetails) + "_Id", column => column.NotNull())
                    .Column<DateTime>(nameof(PoliceofficerDeploymentAllowanceTracker.SettlementCycleStartDate), column => column.NotNull())
                    .Column<DateTime>(nameof(PoliceofficerDeploymentAllowanceTracker.SettlementCycleEndDate), column => column.NotNull())
                    .Column<DateTime>(nameof(PoliceofficerDeploymentAllowanceTracker.NextSettlementDate), column => column.NotNull())
                    .Column<DateTime>(nameof(PoliceofficerDeploymentAllowanceTracker.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PoliceofficerDeploymentAllowanceTracker.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }
    }
}