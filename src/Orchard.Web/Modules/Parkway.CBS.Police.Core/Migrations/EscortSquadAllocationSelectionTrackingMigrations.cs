using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class EscortSquadAllocationSelectionTrackingMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(EscortSquadAllocationSelectionTracking).Name,
                table => table
                    .Column<Int64>(nameof(EscortSquadAllocationSelectionTracking.Id), column => column.PrimaryKey().Identity())
                    .Column<int>(nameof(EscortSquadAllocationSelectionTracking.Command) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(EscortSquadAllocationSelectionTracking.NumberOfOfficers), column => column.NotNull())
                    .Column<bool>(nameof(EscortSquadAllocationSelectionTracking.IsDeleted), column => column.NotNull().WithDefault(false))
                    .Column<string>(nameof(EscortSquadAllocationSelectionTracking.Reference), column => column.NotNull())
                    .Column<Int64>(nameof(EscortSquadAllocationSelectionTracking.AllocationGroup) + "_Id", column => column.NotNull())
                    .Column<DateTime>(nameof(EscortSquadAllocationSelectionTracking.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(EscortSquadAllocationSelectionTracking.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }
    }
}