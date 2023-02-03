using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class EscortFormationAllocationSelectionTrackingMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(EscortFormationAllocationSelectionTracking).Name,
                table => table
                    .Column<Int64>(nameof(EscortFormationAllocationSelectionTracking.Id), column => column.PrimaryKey().Identity())
                    .Column<int>(nameof(EscortFormationAllocationSelectionTracking.Command) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(EscortFormationAllocationSelectionTracking.NumberOfOfficers), column => column.NotNull())
                    .Column<bool>(nameof(EscortFormationAllocationSelectionTracking.IsDeleted), column => column.NotNull().WithDefault(false))
                    .Column<int>(nameof(EscortFormationAllocationSelectionTracking.State) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(EscortFormationAllocationSelectionTracking.LGA) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(EscortFormationAllocationSelectionTracking.AllocatedByAdminUser) + "_Id", column => column.NotNull())
                    .Column<long>(nameof(EscortFormationAllocationSelectionTracking.Group) + "_Id", column => column.NotNull())
                    .Column<long>(nameof(EscortFormationAllocationSelectionTracking.EscortSquadAllocation) + "_Id", column => column.NotNull())
                    .Column<string>(nameof(EscortFormationAllocationSelectionTracking.Reference), column => column.NotNull())
                    .Column<DateTime>(nameof(EscortFormationAllocationSelectionTracking.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(EscortFormationAllocationSelectionTracking.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }
    }
}