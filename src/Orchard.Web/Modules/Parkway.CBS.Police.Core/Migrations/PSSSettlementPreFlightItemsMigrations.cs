using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSSettlementPreFlightItemsMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSSettlementPreFlightItems).Name,
                table => table
                    .Column<Int64>(nameof(PSSSettlementPreFlightItems.Id), column => column.PrimaryKey().Identity())
                    .Column<Int64>(nameof(PSSSettlementPreFlightItems.Batch) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementPreFlightItems.PSSSettlement) + "_Id", column => column.NotNull())
                    .Column<DateTime>(nameof(PSSSettlementPreFlightItems.SettlementScheduleDate), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSSettlementPreFlightItems.StartRange), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSSettlementPreFlightItems.EndRange), column => column.NotNull())
                    .Column<bool>(nameof(PSSSettlementPreFlightItems.AddToSettlementBatch), column => column.NotNull().WithDefault(true))
                    .Column<DateTime>(nameof(PSSSettlementPreFlightItems.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSSettlementPreFlightItems.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }
    }
}